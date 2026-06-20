using UnityEngine;
using UnityEditor;
using System.IO;

// Place this file in Assets/Editor/  (create the Editor folder if it doesn't exist)
// After adding: Unity menu bar → Tools → Protein Fighter → Auto-Assign Card Artwork
//
// What it does:
//   Loads every CardData in Resources/Cards/
//   Looks for a matching Sprite in Art/Cards/ with the same base name
//   Assigns it to CardData.artwork and saves the asset

public class CardArtworkAutoAssign : EditorWindow
{
    [MenuItem("Tools/Protein Fighter/Auto-Assign Card Artwork")]
    static void Run()
    {
        int assigned = 0, missing = 0;

        // Load all CardData assets from Resources/Cards/ (and subfolders)
        var allCards = Resources.LoadAll<CardData>("Cards");

        if (allCards.Length == 0)
        {
            Debug.LogError("[AutoAssign] No CardData assets found in Resources/Cards/. " +
                           "Make sure .asset files are in that folder.");
            return;
        }

        // Find all sprites anywhere under Art/Cards/
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Art/Cards" });

        foreach (var card in allCards)
        {
            bool found = false;

            foreach (var guid in guids)
            {
                string path     = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path).ToLower();

                // Match by: cardId, cardId+"_card", cardId+"card", displayName lower
                string idLower   = card.id?.ToLower() ?? "";
                string nameLower = card.displayNameEl?.ToLower() ?? "";

                if (fileName == idLower ||
                    fileName == idLower + "_card" ||
                    fileName == idLower + "card"  ||
                    fileName == nameLower)
                {
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (sprite != null)
                    {
                        card.artwork = sprite;
                        EditorUtility.SetDirty(card);
                        Debug.Log($"[AutoAssign] ✓ {card.displayNameEl}  ← {Path.GetFileName(path)}");
                        assigned++;
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                Debug.LogWarning($"[AutoAssign] ✗ No sprite found for: {card.displayNameEl} (id={card.id}). " +
                                 $"Assign manually: drag PNG into CardData.artwork in Inspector.");
                missing++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[AutoAssign] Done — {assigned} assigned, {missing} need manual assignment.");
        EditorUtility.DisplayDialog("Auto-Assign Complete",
            $"Assigned: {assigned} cards\nNeeds manual fix: {missing} cards\n\n" +
            (missing > 0 ? "Check Console for which cards need manual artwork." : "All cards assigned!"),
            "OK");
    }
}
