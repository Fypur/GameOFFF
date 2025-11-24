using System;
using System.Collections;
using UnityEngine;

public class TimerSlider : MonoBehaviour
{
    [HideInInspector] public bool pauseTimer;

    private float totalTime;
    [SerializeField] private RectTransform fill;

    public void StartTimer(float totalTime, Action callback)
    {
        this.totalTime = totalTime;
        StopAllCoroutines();
        StartCoroutine(Wait(totalTime, callback));
    }

    private IEnumerator Wait(float totalTime, Action callback)
    {
        float t = totalTime;
        float totalWidth = fill.sizeDelta.x;
        while (t > 0)
        {
            fill.sizeDelta = new Vector2(Math.Max(0, totalWidth * t / totalTime), fill.sizeDelta.y);
            if(!pauseTimer)
                t -= Time.deltaTime * Time.timeScale;
            yield return null;
        }

        fill.sizeDelta = new Vector2(0, fill.sizeDelta.y);
        callback();
    }
}
