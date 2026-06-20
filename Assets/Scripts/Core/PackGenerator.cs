using System;
using System.Collections.Generic;
using System.Linq;


public static class PackGenerator
{
    static readonly (CardRank rank, float weight)[] RankWeights =
    {
        (CardRank.Bronze,   0.55f),
        (CardRank.Silver,   0.30f),
        (CardRank.Gold,     0.12f),
        (CardRank.Platinum, 0.03f),
    };

    public static List<string> Roll(int count, IEnumerable<CardData> pool, Random rng)
    {
        var byRank = pool
            .GroupBy(c => c.rank)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = new List<string>(count);

        for (int i = 0; i < count; i++)
        {
            var rank = PickRank(rng, byRank);
            var list = byRank[rank];
            result.Add(list[rng.Next(list.Count)].id);
        }

        return result;
    }

    static CardRank PickRank(Random rng, Dictionary<CardRank, List<CardData>> byRank)
    {
        float roll = (float)rng.NextDouble();
        float acc = 0f;

        foreach (var (rank, w) in RankWeights)
        {
            acc += w;
            if (roll <= acc && byRank.ContainsKey(rank))
                return rank;
        }

        return byRank.Keys.First();
    }
}
