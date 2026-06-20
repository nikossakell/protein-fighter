using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ResultsUI : MonoBehaviour
{
    [Header("Result display")]
    public TextMeshProUGUI resultBanner;
    public TextMeshProUGUI hpSummary;

    [Header("Buttons")]
    public Button btnNextLevel;
    public Button btnMainMenu;

    void Start()
    {
        if (GameManager.Instance == null) return;

        var result = GameManager.Instance.LastBattleResult;

        if (result == null)
        {
            resultBanner.text = "???";
            hpSummary.text    = "Δεν βρέθηκαν αποτελέσματα.";
            ShowOnlyMainMenu();
            return;
        }

        bool playerWon = result.playerWon;

        // Banner
        resultBanner.text = result.winner switch {
            0 => "ΝΙΚΗ!",
            1 => "ΗΤΤΑ...",
            _ => "ΙΣΟΠΑΛΙΑ"
        };

        int hpA = Mathf.Max(0, result.finalHpA);
        int hpB = Mathf.Max(0, result.finalHpB);
        hpSummary.text = $"HP σου: {hpA}   |   HP εχθρού: {hpB}";

        bool hasNext = GameManager.Instance.HasNextLevel();

        if (playerWon && hasNext)
        {
            btnNextLevel.gameObject.SetActive(true);
            btnMainMenu.gameObject.SetActive(true);
        }
        else if (playerWon && !hasNext)
        {
            resultBanner.text = "ΝΙΚΗΣΕΣ ΟΛΑ ΤΑ ΕΠΙΠΕΔΑ!";
            ShowOnlyMainMenu();
        }
        else
        {
            ShowOnlyMainMenu();
        }
    }

    void ShowOnlyMainMenu()
    {
        if (btnNextLevel != null) btnNextLevel.gameObject.SetActive(false);
        if (btnMainMenu  != null) btnMainMenu.gameObject.SetActive(true);
    }

    public void OnNextLevelClicked()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.StartNextLevel();
    }

    public void OnMainMenuClicked()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.GoToMainMenu();
    }
}
