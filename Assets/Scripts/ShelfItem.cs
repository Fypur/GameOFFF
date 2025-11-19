using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShelfItem : MonoBehaviour
{
    public enum ItemType { Fruit, Meat, Fish, Hygiene, Beverage }
    public static ShelfSlot[] shelfSlots;

    public ItemType itemType;
    public bool held = false;
    public bool shelfed = false;
    [HideInInspector] public Vector2 originalLocalPos;

    private RectTransform rectTransform;
    private Vector2 offset;
    private ShelfManager shelfManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        shelfSlots = FindObjectsByType<ShelfSlot>(FindObjectsSortMode.None);
        shelfManager = FindFirstObjectByType<ShelfManager>();
    }

    private void Update()
    {
        if (!shelfed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (Mouse.current.leftButton.wasPressedThisFrame && rectTransform.rect.Contains(rectTransform.InverseTransformPoint(mousePos)))
            {
                held = true;
                offset = (Vector2)transform.position - mousePos;
            }

            if (Mouse.current.leftButton.isPressed && held)
            {
                transform.position = mousePos + offset;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && held)
            {
                held = false;
                //Check if it's within a ShelfItemSlot, if yes put it in there, if not reput it in it's original position

                foreach (ShelfSlot shelfSlot in shelfSlots)
                {
                    RectTransform shelfSlotTransform = shelfSlot.GetComponent<RectTransform>();

                    //Not an actual collision, we just check for middle point being inside of shelf slot (more natural?)
                    if (shelfSlot.itemType == itemType && shelfSlot.heldItem == null && (shelfSlotTransform.rect.Contains(shelfSlotTransform.InverseTransformPoint(rectTransform.position))
                        || shelfSlotTransform.rect.Contains(shelfSlotTransform.InverseTransformPoint(mousePos))))
                    {
                        shelfSlot.Hold(this);
                        shelfManager.GenerateNextItemInHotbar(originalLocalPos);
                        break;
                    }
                    else
                        transform.localPosition = originalLocalPos;
                }
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStaticFields()
    {
        shelfSlots = null;
    }
}
