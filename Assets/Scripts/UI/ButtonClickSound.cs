using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(PlayClick);
    }

    void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
}

#if UNITY_EDITOR
public static class ButtonClickSoundTool
{
    [UnityEditor.MenuItem("Tools/Protein Fighter/Add Click Sound To All Buttons")]
    static void AddToAll()
    {
        var buttons = Object.FindObjectsByType<Button>();
        int added = 0;
        foreach (var b in buttons)
        {
            if (b.GetComponent<ButtonClickSound>() == null)
            {
                b.gameObject.AddComponent<ButtonClickSound>();
                UnityEditor.EditorUtility.SetDirty(b.gameObject);
                added++;
            }
        }
        Debug.Log($"[ButtonClickSound] Added to {added} buttons in the open scene. " +
                  "Save the scene to keep changes.");
    }
}
#endif
