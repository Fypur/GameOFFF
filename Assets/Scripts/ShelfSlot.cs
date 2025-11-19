using UnityEngine;

public class ShelfSlot : MonoBehaviour
{
    public ShelfItem.ItemType itemType;
    public ShelfItem heldItem = null;

    private ShelfManager shelfManager = null;

    private void Awake()
    {
        shelfManager = FindFirstObjectByType<ShelfManager>();
    }

    public void Hold(ShelfItem item)
    {
        heldItem = item;

        item.shelfed = true;
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
    }
}
