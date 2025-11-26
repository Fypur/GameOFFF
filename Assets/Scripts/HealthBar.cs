using UnityEngine;
using UnityEngine.InputSystem;

public class HealthBar : MonoBehaviour
{
    public float fullHealth = 100;
    [HideInInspector] public float health;

    [SerializeField] private RectTransform fill;
    private float maxFillHeight;

    private void Start()
    {
        health = fullHealth;
        maxFillHeight = fill.sizeDelta.y;
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
        fill.sizeDelta = new Vector2(fill.sizeDelta.x, maxFillHeight * health / fullHealth);
    }

    private void Death()
    {
        GameManager.Instance.Death();
    }

    private void Update()
    {
        if(Keyboard.current.kKey.isPressed)
            Heal(10);
        else if(Keyboard.current.lKey.isPressed)
            Damage(10);
    }
}
