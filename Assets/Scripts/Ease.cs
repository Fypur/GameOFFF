using System;
using System.Collections;
using UnityEngine;

public static class Ease
{
    public enum EaseType { Linear, CubicIn, CubicOut, CubeInAndOut, QuintIn, QuintOut, QuintInAndOut }

    public static Func<float, float> ToFunc(this EaseType easeType)
    {
        switch (easeType)
        {
            case EaseType.Linear:
                return None;
            case EaseType.CubicIn:
                return CubicIn;
            case EaseType.CubicOut:
                return CubicOut;
            case EaseType.CubeInAndOut:
                return CubeInAndOut;
            case EaseType.QuintIn:
                return QuintIn;
            case EaseType.QuintOut:
                return QuintOut;
            case EaseType.QuintInAndOut:
                return QuintInAndOut;
            default:
                throw new NotImplementedException();
        }
    }

    public static float CubicIn(float x)
        => (float)Math.Pow(x, 3);

    public static float CubicOut(float x)
        => 1 - (float)Math.Pow(1 - x, 3);

    public static float CubeInAndOut(float x)
        => x < 0.5f ? 4 * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 3) / 2;

    public static float QuintIn(float x)
        => (float)Math.Pow(x, 5);

    public static float QuintOut(float x)
        => 1 - (float)Math.Pow(1 - x, 5);

    public static float QuintInAndOut(float x)
        => x < 0.5 ? 16 * x * x * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 5) / 2;

    public static float None(float x)
        => x;

    /// <summary>
    /// Make a value that goes from 1 to 0 go from 0 to 1 or the opposite
    /// </summary>
    /// <param name="t">value</param>
    /// <returns></returns>
    public static float Reverse(float t, float coef = 1)
        => -t + coef;
}