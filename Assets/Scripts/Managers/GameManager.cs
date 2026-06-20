using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerSave       CurrentSave      { get; private set; }
    public MonsterDeckData  SelectedMonster  { get; private set; }
    public int              CurrentLevel     { get; private set; }   
    public int              BattleSeed       { get; private set; }
    public BattleResult     LastBattleResult { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    void Init()
    {
        _ = CardDatabase.All;        
        _ = MonsterLadder.All;        

        CurrentSave = SaveSystem.Load();

        if (CurrentSave.collection.Count == 0)
        {
            PackManager.GrantStarter(CurrentSave);
            SaveSystem.Save(CurrentSave);
        }

        Debug.Log($"[GameManager] Ready. Collection: {CurrentSave.collection.Count} entries.");

        GoToMainMenu();
    }

    // ── Scene transitions ───────────────────────────────────────────────────
    public void GoToMainMenu()    => SceneManager.LoadScene("MainMenu");
    public void GoToDeckBuilder() => SceneManager.LoadScene("DeckBuilder");
    public void GoToPackOpening() => SceneManager.LoadScene("PackOpening");
    public void GoToResults()     => SceneManager.LoadScene("Results");


    public void StartFirstBattle()
    {
        StartBattleAtLevel(0);
    }

    public void StartBattleAtLevel(int level)
    {
        var monster = MonsterLadder.GetLevel(level);
        if (monster == null)
        {
            Debug.LogError($"[GameManager] No monster at level {level}.");
            return;
        }

        CurrentLevel    = level;
        SelectedMonster = monster;
        BattleSeed      = System.Environment.TickCount;
        SceneManager.LoadScene("Battle");
    }

    public void StartNextLevel()
    {
        if (MonsterLadder.HasNextLevel(CurrentLevel))
            StartBattleAtLevel(CurrentLevel + 1);
        else
        {
            Debug.Log("[GameManager] No more levels — returning to menu.");
            GoToMainMenu();
        }
    }

    public bool HasNextLevel() => MonsterLadder.HasNextLevel(CurrentLevel);

    // ── Result submission ───────────────────────────────────────────────────
    public void SubmitBattleResult(BattleResult result)
    {
        LastBattleResult = result;

        if (result.playerWon)
        {
            CurrentSave.totalWins++;
            if (SelectedMonster != null &&
                !CurrentSave.defeatedMonsters.Contains(SelectedMonster.monsterId))
                CurrentSave.defeatedMonsters.Add(SelectedMonster.monsterId);
        }
        else if (result.winner == 1)
        {
            CurrentSave.totalLosses++;
        }

        SaveSystem.Save(CurrentSave);
        GoToResults();
    }

    public void SaveGame() => SaveSystem.Save(CurrentSave);
}
