using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Ease;

public class WaveUI : MonoBehaviour
{
    public float waveTime;
    public float perfectWindowTime;

    private RectTransform rectTransform;
    public RectTransform cursor;
    public RectTransform perfectWindow;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        perfectWindow.sizeDelta = new Vector2(rectTransform.sizeDelta.x * perfectWindowTime / waveTime, perfectWindow.sizeDelta.y);

        StartCoroutine(Scroll());

        Debug.Log(rectTransform.TransformPoint(rectTransform.anchoredPosition));
    }

    private IEnumerator Scroll()
    {
        Vector2 fromPosition = cursor.anchoredPosition;
        //the rect transform position is its center

        //Vector2 toPosition = cursor.InverseTransformPoint(rectTransform.position - rectTransform.TransformPoint(new Vector3(rectTransform.sizeDelta.x / 2, 0, 0)));

        Vector2 toPosition = rectTransform.TransformPoint(rectTransform.anchoredPosition);



        float t = 0;

        while (t < waveTime)
        {
            cursor.anchoredPosition = Vector2.LerpUnclamped(fromPosition, toPosition, t / waveTime);
            t += Time.deltaTime;
            yield return null;
        }

        cursor.anchoredPosition = toPosition;
    }
}
