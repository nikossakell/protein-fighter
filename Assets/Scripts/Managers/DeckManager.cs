using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class DeckManager
{
    const int DeckSize = 5;

    public enum ValidationResult { Valid, TooFew, TooMany, CardNotOwned, DuplicatesNotAllowed }

    public static ValidationResult Validate(List<string> proposedDeck, PlayerSave save)
    {
        if (proposedDeck == null || proposedDeck.Count < DeckSize) return ValidationResult.TooFew;
        if (proposedDeck.Count > DeckSize) return ValidationResult.TooMany;

        foreach (var id in proposedDeck)
            if (!save.HasCard(id)) return ValidationResult.CardNotOwned;

        var counts = proposedDeck.GroupBy(id => id).ToDictionary(g => g.Key, g => g.Count());
        foreach (var kvp in counts)
            if (save.CopiesOf(kvp.Key) < kvp.Value) return ValidationResult.DuplicatesNotAllowed;

        return ValidationResult.Valid;
    }

    public static bool TrySaveDeck(List<string> deck, PlayerSave save)
    {
        var result = Validate(deck, save);
        if (result != ValidationResult.Valid)
        {
            Debug.LogWarning($"[DeckManager] Save rejected: {result}");
            return false;
        }

        save.activeDeck = new List<string>(deck);
        SaveSystem.Save(save);
        Debug.Log($"[DeckManager] Deck saved: {string.Join(", ", deck)}");
        return true;
    }

    public static string ValidationMessageEl(ValidationResult r) => r switch
    {
        ValidationResult.TooFew              => "Επίλεξε ακριβώς 5 κάρτες!",
        ValidationResult.TooMany             => "Πολλές κάρτες! Μέγιστο 5.",
        ValidationResult.CardNotOwned        => "Μία ή περισσότερες κάρτες δεν ανήκουν στη συλλογή σου.",
        ValidationResult.DuplicatesNotAllowed => "Δεν έχεις αρκετά αντίγραφα για να χρησιμοποιήσεις αυτή την κάρτα δύο φορές.",
        _                                    => ""
    };
}
