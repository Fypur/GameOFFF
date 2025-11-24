using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Ease;

public class WaveUI : MonoBehaviour
{
    [HideInInspector] public float waveTime;
    [HideInInspector] public float perfectWindowTime; //not really used as of right now

    [HideInInspector] public Person parentPerson;

    private RectTransform rectTransform;
    public RectTransform cursor;
    public RectTransform perfectWindow;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        perfectWindow.sizeDelta = new Vector2(rectTransform.sizeDelta.x * perfectWindowTime / waveTime, perfectWindow.sizeDelta.y);

        StartCoroutine(Scroll());

        //Debug.Log(rectTransform.TransformPoint(rectTransform.anchoredPosition));
    }

    public void OnClick()
    {
        parentPerson.Leave();

        Destroy(gameObject);
    }

    private void OnFail()
    {
        Debug.Log("fail!!");
        parentPerson.Leave();

        Destroy(gameObject);
    }

    private IEnumerator Scroll()
    {
        Vector2 fromPosition = cursor.anchoredPosition;
        //Vector2 toPosition = (Vector2)cursor.InverseTransformPoint((fourCornersArray[0] + fourCornersArray[1]) / 2) + new Vector2(cursor.sizeDelta.x / 2, 0);
        Vector2 toPosition = new Vector2(-rectTransform.rect.width, 0);


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
