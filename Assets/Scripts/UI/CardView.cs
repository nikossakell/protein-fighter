using UnityEngine;
using UnityEngine.UI;


public class CardView : MonoBehaviour
{
    [Header("Wire these 2 fields in the Inspector")]
    public Image  artworkImage; 
    public Button button;        
    public CardData Data { get; private set; }

    // ── Called by CardViewFactory.Spawn() ──────────────────────────────────
    public void Setup(CardData card, System.Action<CardData> onClick = null)
    {
        Data = card;
        artworkImage.sprite = card.artwork;  
        artworkImage.color  = Color.white;    

        button.onClick.RemoveAllListeners();
        if (onClick != null)
            button.onClick.AddListener(() => onClick(card));
    }


    public void SetSelected(bool selected)
    {
        transform.localScale = selected ? Vector3.one * 1.08f : Vector3.one;
    }

    public void SetDefeated()
    {
        artworkImage.color = new Color(0.45f, 0.45f, 0.45f, 1f);
    }

    public void SetActiveInBattle(bool active)
    {
        transform.localScale = active ? Vector3.one * 1.06f : Vector3.one;
    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }
}
