using System;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    public static ShelfManager instance;

    [SerializeField] private int numItemsInHotbar = 5;
    [SerializeField] private float hotbarItemOffset = 1.5f;
    [SerializeField] private int[] numItemsPerShelf = new int[5] { 4, 4, 4, 4, 4 };

    [HideInInspector] public List<ShelfSlot> shelfSlots;
    [HideInInspector] public int shelfSlotsHoldingItemNumber;
    [SerializeField] private GameObject hotbar;
    [SerializeField] private GameObject shelf;
    [SerializeField] private GameObject[] shelfItemPrefab;


    private List<ShelfItem.ItemType> items = new();
    private int currentItemIndex = 0;
    private static System.Random rng = new System.Random();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GenerateItemList();

        for(int i = -numItemsInHotbar / 2; i <= numItemsInHotbar / 2; i++)
        {
            GenerateNextItemInHotbar(new Vector3(hotbarItemOffset * i, 0, 0));
        }
    }

    private void GenerateItemList()
    {
        for(int i = 0; i < numItemsPerShelf.Length; i++)
            for(int j = 0; j < numItemsPerShelf[i]; j++)
                items.Add((ShelfItem.ItemType)i);

        Shuffle(items);
    }

    public void GenerateNextItemInHotbar(Vector2 originalLocalPos)
    {
        if (currentItemIndex >= items.Count)
            return;

        ShelfItem shelfItem = Instantiate(shelfItemPrefab[(int)items[currentItemIndex]], hotbar.transform).GetComponent<ShelfItem>();

        shelfItem.itemType = items[currentItemIndex];
        shelfItem.originalLocalPos = originalLocalPos;
        shelfItem.transform.localPosition = (Vector3)shelfItem.originalLocalPos;

        currentItemIndex++;
    }

    //TODO: Generate Shelf Slots automatically according to given array
    private void GenerateShelfSlots()
    {
        
    }

    public void AddFinishedSlot()
    {
        shelfSlotsHoldingItemNumber++;
        if (shelfSlotsHoldingItemNumber == shelfSlots.Count)
            GameManager.Instance.LevelSuccess();
    }


    //https://stackoverflow.com/questions/273313/randomize-a-listt thanks stackoverflow as always
    private static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStaticFields()
    {
        instance = null;
        rng = new();
    }
}
