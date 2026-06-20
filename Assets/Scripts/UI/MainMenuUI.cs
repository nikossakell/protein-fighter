using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button btnOpenPack;
    public Button btnDeckBuilder;
    public Button btnBattle;

    [Header("Pack timer label")]
    public TextMeshProUGUI packTimerLabel;

    void Start()
    {
        if (GameManager.Instance == null) return;
        UpdatePackUI();
    }

    void Update()
    {
        if (GameManager.Instance == null || packTimerLabel == null) return;
        UpdatePackUI();
    }

    void UpdatePackUI()
    {
        var save = GameManager.Instance.CurrentSave;

        if (PackManager.CollectionFull(save))
        {
            btnOpenPack.interactable = false;
            packTimerLabel.text = "Έχεις πάρα πολλές κάρτες! (μέγιστο όριο)";
            return;
        }

        bool canOpen = PackManager.CanOpenDailyPack(save);
        btnOpenPack.interactable = canOpen;

        if (canOpen)
            packTimerLabel.text = "Πακέτο διαθέσιμο!";
        else
        {
            var remaining = PackManager.TimeUntilNextPack(save);
            packTimerLabel.text = $"Επόμενο πακέτο σε: {remaining:mm\\:ss}";
        }
    }

    public void OnOpenPackClicked()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.GoToPackOpening();
    }

    public void OnDeckBuilderClicked()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.GoToDeckBuilder();
    }

    public void OnBattleClicked()
    {
        if (GameManager.Instance == null) return;

        if (!GameManager.Instance.CurrentSave.HasValidDeck)
        {
            packTimerLabel.text = "Φτιάξε πρώτα μια τράπουλα 5 καρτών!";
            return;
        }
        GameManager.Instance.StartFirstBattle();
    }
}
