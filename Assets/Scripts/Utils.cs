using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
public enum Interactions { Wave, NameChoice, AgressiveWave, Osu, HandShake, SmallTalk, Bowing, Robot, Opp }
public enum Direction { Left, Right, Down, Up }
public enum Accessories { Hat, Cap }

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

    public static IEnumerator SlideUIObject(RectTransform obj, Vector2 toPosition, float time, Ease.EaseType easeType)
    {
        Vector2 fromPosition = obj.anchoredPosition;
        Func<float, float> easeFunc = easeType.ToFunc();
        float t = 0;
        while (t < time)
        {
            obj.anchoredPosition = Vector2.LerpUnclamped(fromPosition, toPosition, easeFunc(t / time));
            t += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        obj.anchoredPosition = toPosition;
    }

    public static IEnumerator Shake(GameObject obj, float time, float amplitude)
    {
        Vector3 initPos = obj.transform.position;

        float t = 0;
        while (t < time)
        {
            float noiseX = (Mathf.PerlinNoise(Time.time * 20f, 0f) - 0.5f) * 2f;
            float noiseY = (Mathf.PerlinNoise(0f, Time.time * 20f) - 0.5f) * 2f;

            obj.transform.position = initPos + new Vector3(noiseX, noiseY, 0) * amplitude;
            t += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        obj.transform.position = initPos;
    }

    public static IEnumerator TimerThen(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    public static void AudioPlay(string eventName)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventName);
        eventInstance.start();
        eventInstance.release();
    }
}
