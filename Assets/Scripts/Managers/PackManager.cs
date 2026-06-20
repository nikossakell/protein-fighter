using System;
using System.Collections.Generic;
using UnityEngine;

// Place in Assets/Scripts/Managers/  (replace existing PackManager.cs)

public static class PackManager
{
    const int StarterCount   = 10;
    const int DailyPackCount  = 10;

    const int MaxCollectionSize = 30;

    const double CooldownMinutes = 24 * 60;

    public static List<string> GrantStarter(PlayerSave save)
    {
        if (save.collection.Count > 0)
        {
            Debug.LogWarning("[PackManager] GrantStarter on non-empty collection — skipped.");
            return new List<string>();
        }
        var cards = PackGenerator.Roll(StarterCount, CardDatabase.All.Values, new System.Random());
        AddCardsToSave(save, cards);
        Debug.Log($"[PackManager] Starter granted: {string.Join(", ", cards)}");
        return cards;
    }

    public static int TotalCards(PlayerSave save)
    {
        int total = 0;
        foreach (var c in save.collection) total += c.copies;
        return total;
    }

    public static bool CollectionFull(PlayerSave save)
        => TotalCards(save) >= MaxCollectionSize;

    public static bool CanOpenDailyPack(PlayerSave save)
    {
        if (CollectionFull(save)) return false;   

        if (string.IsNullOrEmpty(save.lastPackOpenedUtc)) return true;
        if (DateTime.TryParse(save.lastPackOpenedUtc, null,
                System.Globalization.DateTimeStyles.RoundtripKind, out var last))
            return (DateTime.UtcNow - last).TotalMinutes >= CooldownMinutes;
        return true;
    }

    public static TimeSpan TimeUntilNextPack(PlayerSave save)
    {
        if (string.IsNullOrEmpty(save.lastPackOpenedUtc)) return TimeSpan.Zero;
        if (DateTime.TryParse(save.lastPackOpenedUtc, null,
                System.Globalization.DateTimeStyles.RoundtripKind, out var last))
        {
            var remaining = last.AddMinutes(CooldownMinutes) - DateTime.UtcNow;
            return remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining;
        }
        return TimeSpan.Zero;
    }

    public static List<string> OpenDailyPack(PlayerSave save)
    {
        if (CollectionFull(save))
        {
            Debug.Log("[PackManager] Collection full — no more packs.");
            return new List<string>();
        }
        if (!CanOpenDailyPack(save))
        {
            Debug.LogWarning("[PackManager] Cooldown not expired.");
            return new List<string>();
        }

        int room = MaxCollectionSize - TotalCards(save);
        int drawCount = Math.Min(DailyPackCount, room);

        var cards = PackGenerator.Roll(drawCount, CardDatabase.All.Values, new System.Random());
        AddCardsToSave(save, cards);
        save.lastPackOpenedUtc = DateTime.UtcNow.ToString("o");
        SaveSystem.Save(save);

        if (AudioManager.Instance != null) AudioManager.Instance.PlayPackOpen();

        Debug.Log($"[PackManager] Pack opened ({drawCount} cards): {string.Join(", ", cards)}");
        return cards;
    }

    static void AddCardsToSave(PlayerSave save, List<string> cardIds)
    {
        foreach (var id in cardIds)
        {
            var owned = save.collection.Find(c => c.cardId == id);
            if (owned != null) owned.copies++;
            else save.collection.Add(new OwnedCard { cardId = id, copies = 1 });
        }
    }
}
