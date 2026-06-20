using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [Header("Monster panel")]
    public Image           monsterAvatar;
    public TextMeshProUGUI monsterName;
    public Slider          monsterHP;
    public Transform       monsterCardRow;

    [Header("Player panel")]
    public Slider          playerHP;
    public Image           playerAvatar;
    public Transform       playerCardRow;

    [Header("Round info")]
    public TextMeshProUGUI roundInfo;

    [Header("Damage popups")]
    public TextMeshProUGUI damagePopupA;   
    public TextMeshProUGUI damagePopupB;   

    [Header("Nutrition fact")]
    public GameObject      nutritionFactPanel;
    public TextMeshProUGUI nutritionFactText;

    [Header("Next round button")]
    public Button btnNextRound;

    // ── Runtime ──────────────────────────────────────────────────────────────
    BattleResult _result;
    int          _roundIndex = 0;       
    bool         _roundPlaying = false;
    bool         _battleOver = false;
    readonly List<CardView> _playerViews  = new List<CardView>();
    readonly List<CardView> _monsterViews = new List<CardView>();

    void Start()
    {
        if (GameManager.Instance == null) return;

        var gm      = GameManager.Instance;
        var monster = gm.SelectedMonster;
        var save    = gm.CurrentSave;

        if (monster == null || !save.HasValidDeck)
        {
            Debug.LogError("[BattleUI] No monster or deck.");
            return;
        }

        if (monsterAvatar != null) monsterAvatar.sprite = monster.avatar;
        if (monsterName   != null) monsterName.text     = monster.monsterNameEl;

        var playerDeck  = CardDatabase.GetDeck(save.activeDeck);
        var monsterDeck = new List<CardData>(monster.deck);
        _result = BattleResolver.Resolve(playerDeck, monsterDeck, gm.BattleSeed);

        foreach (var card in playerDeck)
            _playerViews.Add(CardViewFactory.Instance.Spawn(card, playerCardRow));
        foreach (var card in monsterDeck)
            _monsterViews.Add(CardViewFactory.Instance.Spawn(card, monsterCardRow));

        if (monsterHP != null) { monsterHP.maxValue = 100; monsterHP.value = 100; monsterHP.interactable = false; }
        if (playerHP  != null) { playerHP.maxValue  = 100; playerHP.value  = 100; playerHP.interactable  = false; }

        if (damagePopupA != null) damagePopupA.gameObject.SetActive(false);
        if (damagePopupB != null) damagePopupB.gameObject.SetActive(false);
        if (nutritionFactPanel != null) nutritionFactPanel.SetActive(false);

        if (roundInfo != null) roundInfo.text = "Επέλεξε 'Συνέχεια'";
    }

    public void OnNextRound()
    {
        if (_roundPlaying) return;

        if (_battleOver) { FinishBattle(); return; }

        if (_roundIndex >= _result.rounds.Count) { FinishBattle(); return; }

        StartCoroutine(PlayRound(_result.rounds[_roundIndex]));
    }

    IEnumerator PlayRound(RoundLog log)
    {
        _roundPlaying = true;
        btnNextRound.interactable = false;

        int idx = _roundIndex;  

        if (idx < _playerViews.Count)  _playerViews[idx].SetActiveInBattle(true);
        if (idx < _monsterViews.Count) _monsterViews[idx].SetActiveInBattle(true);

        if (roundInfo != null) roundInfo.text = $"Γύρος {idx + 1}/5";

        yield return new WaitForSeconds(0.4f);

        if (damagePopupB != null)
        {
            damagePopupB.text = $"-{log.dmgToB}";
            damagePopupB.gameObject.SetActive(true);
        }
        if (monsterHP != null)
            yield return StartCoroutine(LerpSlider(monsterHP, log.hpBAfterRound, 0.5f));

        if (log.hpBAfterRound <= 0)
        {
            yield return new WaitForSeconds(0.4f);
            EndBattleEarly();
            yield break;
        }

        yield return new WaitForSeconds(0.4f);

        if (damagePopupA != null)
        {
            damagePopupA.text = $"-{log.dmgToA}";
            damagePopupA.gameObject.SetActive(true);
        }
        if (playerHP != null)
            yield return StartCoroutine(LerpSlider(playerHP, log.hpAAfterRound, 0.5f));

        if (log.hpAAfterRound <= 0)
        {
            yield return new WaitForSeconds(0.4f);
            EndBattleEarly();
            yield break;
        }

        yield return new WaitForSeconds(0.3f);

        if (damagePopupA != null) damagePopupA.gameObject.SetActive(false);
        if (damagePopupB != null) damagePopupB.gameObject.SetActive(false);

        if (idx < _playerViews.Count)  _playerViews[idx].SetDefeated();
        if (idx < _monsterViews.Count) _monsterViews[idx].SetDefeated();

        if (nutritionFactPanel != null && !string.IsNullOrEmpty(log.nutritionFactWinner))
        {
            if (nutritionFactText != null) nutritionFactText.text = log.nutritionFactWinner;
            nutritionFactPanel.SetActive(true);
        }

        _roundIndex++;

        if (_roundIndex >= _result.rounds.Count)
        {
            _battleOver = true;
            SetButtonLabel("Αποτελέσματα →");
        }
        else
        {
            SetButtonLabel("Επόμενος Γύρος →");
        }

        _roundPlaying = false;
        btnNextRound.interactable = true;
    }

    void EndBattleEarly()
    {
        _battleOver = true;
        _roundPlaying = false;
        SetButtonLabel("Αποτελέσματα →");
        btnNextRound.interactable = true;
    }

    IEnumerator LerpSlider(Slider slider, float target, float duration)
    {
        float start = slider.value, t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            slider.value = Mathf.Lerp(start, Mathf.Max(0, target), t / duration);
            yield return null;
        }
        slider.value = Mathf.Max(0, target);
    }

    void SetButtonLabel(string text)
    {
        var label = btnNextRound.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = text;
    }

    public void OnQuitClicked()
    {
        if (GameManager.Instance == null) return;
        CardViewFactory.Instance.RecycleAll(playerCardRow);
        CardViewFactory.Instance.RecycleAll(monsterCardRow);
        GameManager.Instance.GoToMainMenu();
    }

    void FinishBattle()
    {
        if (_result.playerWon)
        {
            AudioManager.Instance.PlayWin();
        }
        else
        {
            AudioManager.Instance.PlayLose();
        }
        CardViewFactory.Instance.RecycleAll(playerCardRow);
        CardViewFactory.Instance.RecycleAll(monsterCardRow);
        GameManager.Instance.SubmitBattleResult(_result);
    }
}