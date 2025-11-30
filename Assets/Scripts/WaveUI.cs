using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Ease;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private float healedAmount;
    [SerializeField] private float damage;

    [HideInInspector] public float waveTime;
    [HideInInspector] public float perfectWindowTime; //not really used as of right now

    [HideInInspector] public Person parentPerson;

    [SerializeField] private RectTransform cursor;
    [SerializeField] private RectTransform waveBox;
    private Button waveBoxButton;
    [SerializeField] private RectTransform perfectWindow;

    private void Awake()
    {
        waveBoxButton = waveBox.gameObject.GetComponent<Button>();
        waveBoxButton.onClick.AddListener(Success);
        perfectWindow.sizeDelta = new Vector2(waveBox.sizeDelta.x * perfectWindowTime / waveTime, perfectWindow.sizeDelta.y);

        StartCoroutine(Scroll());

        //Debug.Log(rectTransform.TransformPoint(rectTransform.anchoredPosition));
    }

    public void Success()
    {
        Utils.AudioPlay("event:/Menu UI/button_click");
        GameManager.instance.Heal(healedAmount);
        End();
    }

    public void OnButtonHover()
    {
        Utils.AudioPlay("event:/Menu UI/button_hover");
    }

    private void OnFail()
    {
        GameManager.instance.Damage(damage);
        End();
    }

    private void End()
    {
        parentPerson.Leave();
        Destroy(gameObject);
    }

    private IEnumerator Scroll()
    {
        Vector2 fromPosition = cursor.anchoredPosition;
        //Vector2 toPosition = (Vector2)cursor.InverseTransformPoint((fourCornersArray[0] + fourCornersArray[1]) / 2) + new Vector2(cursor.sizeDelta.x / 2, 0);
        Vector2 toPosition = new Vector2(-waveBox.rect.width, 0);


        float t = 0;

        while (t < waveTime)
        {
            cursor.anchoredPosition = Vector2.LerpUnclamped(fromPosition, toPosition, t / waveTime);
            t += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        cursor.anchoredPosition = toPosition;

        OnFail();
    }
}
