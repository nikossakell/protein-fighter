using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MonsterLadder
{
    static List<MonsterDeckData> _ladder;

    public static List<MonsterDeckData> All
    {
        get { if (_ladder == null) Init(); return _ladder; }
    }

    static void Init()
    {
        _ladder = Resources.LoadAll<MonsterDeckData>("Monsters")
                           .OrderBy(m => m.difficulty)
                           .ToList();

        Debug.Log($"[MonsterLadder] Loaded {_ladder.Count} monsters: " +
                  string.Join(" → ", _ladder.Select(m => m.monsterNameEl)));
    }

    public static int Count => All.Count;

    public static MonsterDeckData GetLevel(int index)
    {
        if (index < 0 || index >= All.Count)
        {
            Debug.LogWarning($"[MonsterLadder] Level {index} out of range (0..{All.Count - 1}).");
            return null;
        }
        return All[index];
    }

    public static bool HasNextLevel(int currentIndex) => currentIndex + 1 < All.Count;

    public static int IndexOf(string monsterId)
    {
        for (int i = 0; i < All.Count; i++)
            if (All[i].monsterId == monsterId) return i;
        return -1;
    }
}
