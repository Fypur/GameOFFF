using System;
using System.Collections;
using UnityEngine;
public enum Interactions { Wave, Handshake, NameChoice, SmallTalk, Bowing, AgressiveWave, Robot, Opp }
public enum Direction { Left, Right, Down, Up }

public static class Utils
{
    public static IEnumerator SlideObject(GameObject obj, Vector2 toPosition, float time, Ease.EaseType easeType)
    {
        Vector2 fromPosition = obj.transform.position;
        Func<float, float> easeFunc = easeType.ToFunc();
        float t = 0;
        while (t < time)
        {
            obj.transform.position = Vector2.LerpUnclamped(fromPosition, toPosition, easeFunc(t / time));
            t += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        obj.transform.position = toPosition;
    }
}
