using System.Collections.Generic;
using UnityEngine;


public class CardViewFactory : MonoBehaviour
{
    public static CardViewFactory Instance { get; private set; }

    [Header("Prefab reference — drag CardView.prefab here")]
    public CardView cardViewPrefab;

    readonly Queue<CardView> _pool = new();
    readonly List<CardView>  _active = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public CardView Spawn(CardData data, Transform parent,
                          System.Action<CardData> onClick = null)
    {
        CardView view = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(cardViewPrefab);
        view.transform.SetParent(parent, worldPositionStays: false);
        view.transform.localScale = Vector3.one;
        view.gameObject.SetActive(true);
        view.Setup(data, onClick);
        _active.Add(view);
        return view;
    }

    public void RecycleAll(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            var cv = container.GetChild(i).GetComponent<CardView>();
            if (cv != null) Recycle(cv);
        }
    }

    public void Recycle(CardView view)
    {
        _active.Remove(view);
        view.gameObject.SetActive(false);
        view.transform.SetParent(transform); 
        _pool.Enqueue(view);
    }
}
