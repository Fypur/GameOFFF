using UnityEngine;
using UnityEngine.InputSystem;

public class HealthBar : MonoBehaviour
{
    public float fullHealth = 100;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private Transform cursorLeftBound;
    [SerializeField] private Transform cursorRightBound;

    [HideInInspector] public float health;

    private void Start()
    {
        health = fullHealth;
        UpdateBar();
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, fullHealth);
        UpdateBar();
    }

    public void Damage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            health = 0;
            Death();
        }

        UpdateBar();
    }

    private void UpdateBar()
    {
        cursor.transform.position = Vector2.Lerp(cursorRightBound.transform.position, cursorLeftBound.transform.position, health / fullHealth);
    }

    private void Death()
    {
        GameManager.Instance.Death();
    }
}
