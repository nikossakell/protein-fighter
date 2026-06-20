using UnityEngine;


[CreateAssetMenu(fileName = "Monster_", menuName = "ProteinFighter/MonsterDeck")]
public class MonsterDeckData : ScriptableObject
{
    [Header("Ταυτότητα")]
    public string monsterId;            
    public string monsterNameEl;       
    public Sprite avatar;              
    public int difficulty;             

    [Header("Αφήγηση")]
    [TextArea(2, 4)]
    public string introEl;              

    [TextArea(2, 4)]
    public string defeatQuoteEl;        

    [TextArea(2, 4)]
    public string educationHookEl;      

    [Header("Τράπουλα (5 κάρτες — η σειρά μετράει)")]
    public CardData[] deck = new CardData[5];

    // Validation helper — call in OnValidate to catch misconfigs early
    public bool IsValid => deck != null && deck.Length == 5 && System.Array.TrueForAll(deck, c => c != null);
}
