using UnityEngine;

public enum CardRank { Bronze, Silver, Gold, Platinum }

[CreateAssetMenu(fileName = "Card_", menuName = "ProteinFighter/Card")]
public class CardData : ScriptableObject
{
    [Header("Ταυτότητα")]
    public string id;
    public string displayNameEl;
    public CardRank rank;
    public Sprite artwork;          

    [Header("Επίθεση (ΕΠ) — must sum to EP header value")]
    [Range(0, 100)] public int proteins;
    [Range(0, 100)] public int carbs;
    [Range(0, 100)] public int fats;

    [Header("Άμυνα (ΑΜ) — must sum to AM header value")]
    [Range(0, 100)] public int vitamins;
    [Range(0, 100)] public int omega3;
    [Range(0, 100)] public int fiber;

    [Header("Εκπαίδευση")]
    [TextArea(2, 5)]
    public string nutritionFactEl;

    public int AttackTotal  => proteins + carbs + fats;
    public int DefenseTotal => vitamins + omega3 + fiber;

    public static float RankWeight(CardRank r) => r switch {
        CardRank.Bronze   => 0.55f,
        CardRank.Silver   => 0.30f,
        CardRank.Gold     => 0.12f,
        CardRank.Platinum => 0.03f,
        _ => 0
    };

#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
            id = name.Replace("Card_", "").ToLower();

        int ep = proteins + carbs + fats;
        int am = vitamins + omega3 + fiber;

        if (ep > 0 || am > 0)
        {
            Debug.Log($"[CardData:{displayNameEl}] ΕΠ={ep}  ΑΜ={am}  " +
                      $"(prot={proteins} carb={carbs} fat={fats} | " +
                      $"vit={vitamins} om3={omega3} fib={fiber})");
        }
    }
#endif
}
