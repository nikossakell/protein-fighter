using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DeckBuilderUI : MonoBehaviour
{
    [Header("Collection grid — drag the Content GO (GridLayoutGroup)")]
    public Transform collectionGrid;

    [Header("5 deck slot Images — Size=5")]
    public Image[] deckSlotImages = new Image[5];

    [Header("Buttons")]
    public Button btnSaveDeck;
    public Button btnBack;
    public Button btnDelete;          
    [Header("Feedback")]
    public TextMeshProUGUI validationLabel;

    [Header("Delete-mode indicator (optional)")]
    public GameObject deleteModeIndicator;   
    public Image      deleteButtonImage;      

    readonly List<string> _deck = new List<string>();
    bool _deleteMode = false;

    static readonly Color NormalBtnColor = Color.white;
    static readonly Color DeleteBtnColor = new Color(1f, 0.45f, 0.45f, 1f);

    void Start()
    {
        if (GameManager.Instance == null) return;

        var save = GameManager.Instance.CurrentSave;
        if (save.HasValidDeck)
            _deck.AddRange(save.activeDeck);

        SetDeleteMode(false);
        RefreshCollection();
        RefreshDeckSlots();
        UpdateMessage();
    }

    void RefreshCollection()
    {
        CardViewFactory.Instance.RecycleAll(collectionGrid);

        var save = GameManager.Instance.CurrentSave;

        foreach (var owned in save.collection)
        {
            if (owned.copies <= 0) continue;

            int usedInDeck = _deck.Count(id => id == owned.cardId);
            int available  = owned.copies - usedInDeck;

            var card = CardDatabase.Get(owned.cardId);
            if (card == null) continue;

            for (int i = 0; i < available; i++)
                CardViewFactory.Instance.Spawn(card, collectionGrid, OnCollectionCardClicked);
        }
    }

    void RefreshDeckSlots()
    {
        for (int i = 0; i < deckSlotImages.Length; i++)
        {
            if (deckSlotImages[i] == null) continue;
            if (i < _deck.Count)
            {
                var card = CardDatabase.Get(_deck[i]);
                deckSlotImages[i].sprite = card != null ? card.artwork : null;
                deckSlotImages[i].color  = Color.white;
            }
            else
            {
                deckSlotImages[i].sprite = null;
                deckSlotImages[i].color  = new Color(1f, 1f, 1f, 0.2f);
            }
        }
    }

    void OnCollectionCardClicked(CardData card)
    {
        if (_deleteMode)
        {
            DeleteOneCopy(card);
            return;
        }

        if (_deck.Count >= 5)
        {
            ShowMessage("Η τράπουλα είναι γεμάτη! (5/5)");
            return;
        }
        _deck.Add(card.id);
        RefreshCollection();
        RefreshDeckSlots();
        UpdateMessage();
    }

    void DeleteOneCopy(CardData card)
    {
        var save = GameManager.Instance.CurrentSave;
        var owned = save.collection.Find(c => c.cardId == card.id);
        if (owned == null) return;

        // Don't allow deleting a copy that's currently locked into the deck
        int usedInDeck = _deck.Count(id => id == card.id);
        int available  = owned.copies - usedInDeck;
        if (available <= 0)
        {
            ShowMessage("Αυτή η κάρτα είναι στην τράπουλα — αφαίρεσέ τη πρώτα.");
            return;
        }

        owned.copies--;
        if (owned.copies <= 0)
            save.collection.Remove(owned);

        SaveSystem.Save(save);

        RefreshCollection();
        ShowMessage($"Διαγράφηκε: {card.displayNameEl}. Σύνολο: {PackManager.TotalCards(save)} κάρτες.");
    }

    public void OnDeckSlot0Clicked() => RemoveFromDeck(0);
    public void OnDeckSlot1Clicked() => RemoveFromDeck(1);
    public void OnDeckSlot2Clicked() => RemoveFromDeck(2);
    public void OnDeckSlot3Clicked() => RemoveFromDeck(3);
    public void OnDeckSlot4Clicked() => RemoveFromDeck(4);

    void RemoveFromDeck(int slotIndex)
    {
        if (slotIndex >= _deck.Count) return;
        _deck.RemoveAt(slotIndex);
        RefreshCollection();
        RefreshDeckSlots();
        UpdateMessage();
    }


    public void OnToggleDeleteMode()
    {
        SetDeleteMode(!_deleteMode);
    }

    void SetDeleteMode(bool on)
    {
        _deleteMode = on;

        if (deleteModeIndicator != null)
            deleteModeIndicator.SetActive(on);

        if (deleteButtonImage != null)
            deleteButtonImage.color = on ? DeleteBtnColor : NormalBtnColor;

        UpdateMessage();
    }

    public void OnSaveClicked()
    {
        if (GameManager.Instance == null) return;
        bool saved = DeckManager.TrySaveDeck(_deck, GameManager.Instance.CurrentSave);
        if (saved)
            ShowMessage("Η τράπουλα αποθηκεύτηκε!");
        else
        {
            var r = DeckManager.Validate(_deck, GameManager.Instance.CurrentSave);
            ShowMessage(DeckManager.ValidationMessageEl(r));
        }
    }

    public void OnBackClicked()
    {
        if (GameManager.Instance == null) return;
        CardViewFactory.Instance.RecycleAll(collectionGrid);
        GameManager.Instance.GoToMainMenu();
    }

    void UpdateMessage()
    {
        if (_deleteMode)
            ShowMessage("ΛΕΙΤΟΥΡΓΙΑ ΔΙΑΓΡΑΦΗΣ: πάτα μια κάρτα για μόνιμη διαγραφή.");
        else
            ShowMessage($"{_deck.Count}/5 κάρτες στην τράπουλα");
    }

    void ShowMessage(string msg)
    {
        if (validationLabel != null) validationLabel.text = msg;
    }
}