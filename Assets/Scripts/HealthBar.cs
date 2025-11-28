using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealthBar : MonoBehaviour
{
    public float fullHealth = 100;
    [SerializeField] private float scrollCursorTime = 0.2f;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private Transform cursorLeftBound;
    [SerializeField] private Transform cursorRightBound;

    [HideInInspector] public float health;

    private void Start()
    {
        health = fullHealth;
        cursor.transform.position = cursorLeftBound.position;
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
        StopAllCoroutines();
        StartCoroutine(ScrollCursor(scrollCursorTime));
    }

    private IEnumerator ScrollCursor(float time)
    {
        Vector2 initPos = cursor.transform.position;
        Vector2 targetPos = Vector2.Lerp(cursorRightBound.position, cursorLeftBound.position, health / fullHealth);

        float t = 0;
        while(t < time)
        {
            cursor.transform.position = Vector2.Lerp(initPos, targetPos, Ease.CubicOut(t / time));
            t += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        cursor.transform.position = targetPos;
    }

    private void Death()
    {
        GameManager.instance.Death();
    }
}
