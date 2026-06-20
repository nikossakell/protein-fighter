using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PackOpeningUI : MonoBehaviour
{
    [Header("Pack images — drag from Hierarchy")]
    public Image packClosedImage;    
    public Image packOpenedImage;    

    [Header("Card grid")]
    public Transform cardRevealGrid; 

    [Header("Buttons")]
    public Button btnOpen;
    public Button btnBack;

    [Header("Status text")]
    public TextMeshProUGUI statusLabel; 

    bool _alreadyOpened = false;

    void Start()
    {
        if (GameManager.Instance == null) return;

        packClosedImage.gameObject.SetActive(true);
        packOpenedImage.gameObject.SetActive(false);

        bool canOpen = PackManager.CanOpenDailyPack(GameManager.Instance.CurrentSave);
        btnOpen.interactable = canOpen;

        if (!canOpen && statusLabel != null)
            statusLabel.text = "Έχεις ήδη ανοίξει πακέτο σήμερα!";

        btnOpen.onClick.AddListener(OnOpenClicked);
        btnBack.onClick.AddListener(OnBackClicked);
    }

    void OnOpenClicked()
    {
        if (_alreadyOpened || GameManager.Instance == null) return;
        _alreadyOpened = true;
        btnOpen.interactable = false;

        var cardIds = PackManager.OpenDailyPack(GameManager.Instance.CurrentSave);
        StartCoroutine(RevealCards(cardIds));
    }

    IEnumerator RevealCards(List<string> cardIds)
    {
        packClosedImage.gameObject.SetActive(false);
        packOpenedImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.4f);

        foreach (var id in cardIds)
        {
            var card = CardDatabase.Get(id);
            if (card == null) continue;

            CardViewFactory.Instance.Spawn(card, cardRevealGrid);
            yield return new WaitForSeconds(0.3f);
        }

        if (statusLabel != null)
            statusLabel.text = "Πήρες " + cardIds.Count + " νέες κάρτες!";
    }

    public void OnBackClicked()
    {
        if (GameManager.Instance == null) return;
        CardViewFactory.Instance.RecycleAll(cardRevealGrid);
        GameManager.Instance.GoToMainMenu();
    }
}
