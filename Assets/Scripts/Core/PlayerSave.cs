using System;
using System.Collections.Generic;


[Serializable]
public class OwnedCard
{
    public string cardId;
    public int copies;
}

[Serializable]
public class PlayerSave
{
    public string playerName = "Παίκτης";

    public List<OwnedCard> collection = new List<OwnedCard>();

    public List<string> activeDeck = new List<string>();

    public string lastPackOpenedUtc = "";   

    public int totalWins;
    public int totalLosses;
    public List<string> defeatedMonsters = new List<string>(); 

    public List<string> answeredQuizIds = new List<string>();

    public bool HasCard(string id) => collection.Exists(c => c.cardId == id && c.copies > 0);
    public int CopiesOf(string id) { var o = collection.Find(c => c.cardId == id); return o?.copies ?? 0; }
    public bool HasValidDeck => activeDeck != null && activeDeck.Count == 5;
    public bool HasDefeated(string monsterId) => defeatedMonsters.Contains(monsterId);
}
