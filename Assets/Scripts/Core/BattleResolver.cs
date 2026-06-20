using System;
using System.Collections.Generic;
using System.Linq;


[Serializable]
public class RoundLog
{
    public string cardAId;
    public string cardBId;
    public string cardANameEl;
    public string cardBNameEl;
    public int dmgToA;
    public int dmgToB;
    public int hpAAfterRound;
    public int hpBAfterRound;
    public int round;                   
    public string nutritionFactWinner;  
}

[Serializable]
public class BattleResult
{
    public List<RoundLog> rounds = new List<RoundLog>();
    public int finalHpA;
    public int finalHpB;
    public int winner;             
    public bool playerWon => winner == 0;
    public bool balancedBonusA;     
    public bool balancedBonusB;
}

public static class BattleResolver
{
    const int StartingHp = 100;

    public static BattleResult Resolve(List<CardData> deckA, List<CardData> deckB, int seed)
    {
        if (deckA == null || deckA.Count != 5) throw new ArgumentException("deckA must have exactly 5 cards");
        if (deckB == null || deckB.Count != 5) throw new ArgumentException("deckB must have exactly 5 cards");

        var rng = new Random(seed);   // seeded — same seed = same battle
        int hpA = StartingHp;
        int hpB = StartingHp;

        bool balancedA = IsBalanced(deckA);
        bool balancedB = IsBalanced(deckB);

        var result = new BattleResult { balancedBonusA = balancedA, balancedBonusB = balancedB };

        bool decided = false;   

        for (int i = 0; i < 5; i++)
        {
            var cardA = deckA[i];
            var cardB = deckB[i];

            int dmgToB = CalcDamage(cardA, cardB, balancedA);
            int dmgToA = CalcDamage(cardB, cardA, balancedB);

            hpB -= dmgToB;
            hpA -= dmgToA;

            int shownHpA = Math.Max(0, hpA);
            int shownHpB = Math.Max(0, hpB);

            bool aWonRound = cardA.AttackTotal >= cardB.AttackTotal;
            string fact = aWonRound ? cardA.nutritionFactEl : cardB.nutritionFactEl;

            result.rounds.Add(new RoundLog
            {
                round       = i + 1,
                cardAId     = cardA.id,
                cardBId     = cardB.id,
                cardANameEl = cardA.displayNameEl,
                cardBNameEl = cardB.displayNameEl,
                dmgToA      = dmgToA,
                dmgToB      = dmgToB,
                hpAAfterRound = shownHpA,
                hpBAfterRound = shownHpB,
                nutritionFactWinner = fact
            });

            if (hpA <= 0 || hpB <= 0)
            {
                if (hpA <= 0 && hpB <= 0)
                    result.winner = hpA >= hpB ? 0 : 1;   
                else if (hpB <= 0)
                    result.winner = 0;                    
                else
                    result.winner = 1;                    

                decided = true;
                break;   
            }
        }

        result.finalHpA = Math.Max(0, hpA);
        result.finalHpB = Math.Max(0, hpB);

        if (!decided)
            result.winner = hpA == hpB ? 2 : (hpA > hpB ? 0 : 1);

        return result;
    }

    static int CalcDamage(CardData attacker, CardData defender, bool balanced)
    {
        float dmg = MathF.Max(1f, attacker.AttackTotal - defender.DefenseTotal / 2f);
        if (balanced) dmg *= 1.10f;
        if (attacker.rank == CardRank.Platinum) dmg += 5f;
        return (int)MathF.Round(dmg);
    }

    static bool IsBalanced(List<CardData> deck)
        => deck.Sum(c => c.proteins) > 0
        && deck.Sum(c => c.carbs)    > 0
        && deck.Sum(c => c.fats)     > 0
        && deck.Sum(c => c.vitamins) > 0
        && deck.Sum(c => c.omega3)   > 0
        && deck.Sum(c => c.fiber)    > 0;
}