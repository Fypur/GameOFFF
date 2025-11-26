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
        //SetAnchorsKeepPosition(fill, new Vector2(0, 0), new Vector2(0, 1));

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

    private static void SetAnchorsKeepPosition(RectTransform rt, Vector2 newMin, Vector2 newMax)
    {
        // Save current position in parent space
        Vector3 oldPos = rt.localPosition;

        // Save size & pivot offset
        Vector2 oldSize = rt.sizeDelta;
        Vector2 oldPivot = rt.pivot;

        // Apply new anchors
        rt.anchorMin = newMin;
        rt.anchorMax = newMax;

        // Restore the local position (prevents jump)
        rt.localPosition = oldPos;
    }
}
