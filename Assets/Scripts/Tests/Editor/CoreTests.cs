// Place this file in Assets/Scripts/Tests/
// Enable via: Window → General → Test Runner → EditMode
// Run all tests before every commit.
//
// Required: in Unity Package Manager, add "Unity Test Framework" (com.unity.test-framework)

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// ─────────────────────────────────────────────────────────────
// NOTE: These tests use mock CardData objects instead of
// ScriptableObjects so they can run in EditMode without
// a full Unity context. Replace with real CardData if
// you restructure CardData as a plain C# class later.
// For now, use the helper at the bottom of this file.
// ─────────────────────────────────────────────────────────────

public class BattleResolverTests
{
    List<CardData> MakeDeck(int ep, int am) =>
        Enumerable.Range(0, 5).Select(_ => MockCard(ep / 3, ep / 3, ep - (ep / 3) * 2,
                                                    am / 3, am / 3, am - (am / 3) * 2)).ToList();

    [Test]
    public void Resolve_Deterministic_SameSeedSameResult()
    {
        var deckA = MakeDeck(45, 45);
        var deckB = MakeDeck(30, 30);

        var r1 = BattleResolver.Resolve(deckA, deckB, seed: 42);
        var r2 = BattleResolver.Resolve(deckA, deckB, seed: 42);

        Assert.AreEqual(r1.winner, r2.winner);
        Assert.AreEqual(r1.finalHpA, r2.finalHpA);
        Assert.AreEqual(r1.finalHpB, r2.finalHpB);
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(r1.rounds[i].dmgToA, r2.rounds[i].dmgToA);
            Assert.AreEqual(r1.rounds[i].dmgToB, r2.rounds[i].dmgToB);
        }
    }

    [Test]
    public void Resolve_DifferentSeedDifferentBattle_NotRequired_ButLogged()
    {
        // Seeds don't affect damage in Phase 1 (no RNG mid-battle) — both results are valid
        var r1 = BattleResolver.Resolve(MakeDeck(45, 45), MakeDeck(45, 45), 1);
        var r2 = BattleResolver.Resolve(MakeDeck(45, 45), MakeDeck(45, 45), 999);
        // They should still be equal since damage is deterministic (no RNG in formula)
        Assert.AreEqual(r1.winner, r2.winner, "Equal decks: draw expected regardless of seed");
    }

    [Test]
    public void Resolve_StrongerDeckWins()
    {
        var strong = MakeDeck(75, 75);
        var weak   = MakeDeck(10, 10);
        var result = BattleResolver.Resolve(strong, weak, 0);
        Assert.AreEqual(0, result.winner, "Stronger deck (A) must win");
    }

    [Test]
    public void Resolve_FiveRoundsAlways()
    {
        var result = BattleResolver.Resolve(MakeDeck(45, 45), MakeDeck(45, 45), 0);
        Assert.AreEqual(5, result.rounds.Count);
    }

    [Test]
    public void Resolve_DamageAtLeastOne()
    {
        // Even if attacker EP is 0 and defender AM is 100, min damage = 1
        var almostNoAttack = MakeDeck(3, 99);
        var maxDefense     = MakeDeck(3, 99);
        var result = BattleResolver.Resolve(almostNoAttack, maxDefense, 0);
        foreach (var r in result.rounds)
        {
            Assert.GreaterOrEqual(r.dmgToA, 1);
            Assert.GreaterOrEqual(r.dmgToB, 1);
        }
    }

    [Test]
    public void Resolve_ThrowsOnInvalidDeckSize()
    {
        Assert.Throws<ArgumentException>(() =>
            BattleResolver.Resolve(MakeDeck(45, 45), new List<CardData>(), 0));
    }

    static CardData MockCard(int prot, int carb, int fat, int vit, int om3, int fib)
    {
        var c = UnityEngine.ScriptableObject.CreateInstance<CardData>();
        c.proteins = prot; c.carbs = carb; c.fats = fat;
        c.vitamins = vit;  c.omega3 = om3; c.fiber = fib;
        c.id = System.Guid.NewGuid().ToString("N")[..8];
        c.displayNameEl = "Test";
        c.rank = CardRank.Bronze;
        c.nutritionFactEl = "Test fact.";
        return c;
    }
}

public class PackGeneratorTests
{
    List<CardData> MakePool(int bronzeCount = 10, int silverCount = 5,
                            int goldCount = 3, int platCount = 2)
    {
        var pool = new List<CardData>();
        for (int i = 0; i < bronzeCount;  i++) pool.Add(MakeCard(CardRank.Bronze,   $"b{i}"));
        for (int i = 0; i < silverCount;  i++) pool.Add(MakeCard(CardRank.Silver,   $"s{i}"));
        for (int i = 0; i < goldCount;    i++) pool.Add(MakeCard(CardRank.Gold,     $"g{i}"));
        for (int i = 0; i < platCount;    i++) pool.Add(MakeCard(CardRank.Platinum, $"p{i}"));
        return pool;
    }

    [Test]
    public void Roll_ReturnCorrectCount()
    {
        var result = PackGenerator.Roll(10, MakePool(), new Random(1));
        Assert.AreEqual(10, result.Count);
    }

    [Test]
    public void Roll_BronzeIsMoreFrequentThanPlatinum()
    {
        // Over 10,000 rolls bronze should be ≥ 50% and platinum ≤ 6%
        var pool = MakePool();
        var all  = PackGenerator.Roll(10_000, pool, new Random(99));
        var bronzeSet = pool.Where(c => c.rank == CardRank.Bronze).Select(c => c.id).ToHashSet();
        var platSet   = pool.Where(c => c.rank == CardRank.Platinum).Select(c => c.id).ToHashSet();

        float bronzeRate = all.Count(id => bronzeSet.Contains(id)) / 10_000f;
        float platRate   = all.Count(id => platSet.Contains(id))   / 10_000f;

        Assert.GreaterOrEqual(bronzeRate, 0.50f, $"Bronze rate {bronzeRate:P1} too low");
        Assert.LessOrEqual(platRate,      0.06f, $"Platinum rate {platRate:P1} too high");
    }

    [Test]
    public void Roll_AllIdsFromPool()
    {
        var pool   = MakePool();
        var poolIds = pool.Select(c => c.id).ToHashSet();
        var result  = PackGenerator.Roll(100, pool, new Random(7));
        foreach (var id in result)
            Assert.IsTrue(poolIds.Contains(id), $"Unknown id '{id}' returned");
    }

    static CardData MakeCard(CardRank rank, string id)
    {
        var c = UnityEngine.ScriptableObject.CreateInstance<CardData>();
        c.rank = rank; c.id = id; c.displayNameEl = id;
        c.proteins = 10; c.carbs = 10; c.fats = 5;
        c.vitamins = 5;  c.omega3 = 5; c.fiber = 5;
        return c;
    }
}

public class DeckManagerTests
{
    PlayerSave MakeSave(params string[] ownedIds)
    {
        var s = new PlayerSave();
        foreach (var id in ownedIds)
            s.collection.Add(new OwnedCard { cardId = id, copies = 1 });
        return s;
    }

    [Test]
    public void Validate_Valid5CardDeck()
    {
        var save = MakeSave("a", "b", "c", "d", "e");
        var result = DeckManager.Validate(new List<string> { "a", "b", "c", "d", "e" }, save);
        Assert.AreEqual(DeckManager.ValidationResult.Valid, result);
    }

    [Test]
    public void Validate_TooFewCards()
    {
        var save = MakeSave("a", "b", "c");
        var result = DeckManager.Validate(new List<string> { "a", "b", "c" }, save);
        Assert.AreEqual(DeckManager.ValidationResult.TooFew, result);
    }

    [Test]
    public void Validate_CardNotOwned()
    {
        var save = MakeSave("a", "b", "c", "d");
        var result = DeckManager.Validate(new List<string> { "a", "b", "c", "d", "X" }, save);
        Assert.AreEqual(DeckManager.ValidationResult.CardNotOwned, result);
    }

    [Test]
    public void Validate_DuplicateWithOneCopy_Rejected()
    {
        var save = MakeSave("a", "b", "c", "d", "e");
        // "a" used twice but player only has 1 copy
        var result = DeckManager.Validate(new List<string> { "a", "a", "b", "c", "d" }, save);
        Assert.AreEqual(DeckManager.ValidationResult.DuplicatesNotAllowed, result);
    }
}
