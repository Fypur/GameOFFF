using UnityEngine;

public class ShelfSlot : MonoBehaviour
{
    public ShelfItem.ItemType itemType;
    [HideInInspector] public ShelfItem heldItem = null;
    [SerializeField] private GameObject explosionParticle;

    private void Start()
    {
        ShelfManager.instance.shelfSlots.Add(this);
    }

    public void Hold(ShelfItem item)
    {
        heldItem = item;

        item.shelfed = true;
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;

        Instantiate(explosionParticle, transform);

        ShelfManager.instance.AddFinishedSlot();
    }
}
