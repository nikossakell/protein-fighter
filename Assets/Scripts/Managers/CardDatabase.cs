using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class CardDatabase
{
    static Dictionary<string, CardData> _byId;

    public static IReadOnlyDictionary<string, CardData> All
    {
        get
        {
            if (_byId == null) Init();
            return _byId;
        }
    }

    static void Init()
    {
        var all = Resources.LoadAll<CardData>("Cards");
        _byId = all.ToDictionary(c => c.id, c => c);
        Debug.Log($"[CardDatabase] Loaded {_byId.Count} cards.");

        foreach (var card in all)
        {
            Debug.Log($"  {card.displayNameEl}: ΕΠ={card.AttackTotal} ΑΜ={card.DefenseTotal} rank={card.rank}");
        }
    }

    public static CardData Get(string id)
    {
        if (All.TryGetValue(id, out var card)) return card;
        Debug.LogError($"[CardDatabase] Card '{id}' not found.");
        return null;
    }

    public static List<CardData> GetDeck(List<string> ids)
        => ids.Select(id => Get(id)).Where(c => c != null).ToList();

    public static List<CardData> AllByRank(CardRank rank)
        => All.Values.Where(c => c.rank == rank).ToList();
}
