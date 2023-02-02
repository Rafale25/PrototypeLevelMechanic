using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Easing
{
    public static float easeInSine(float x)
    {
        return 1 - Mathf.Cos((x * Mathf.PI) / 2f);
    }

    public static float easeOutSine(float x)
    {
        return Mathf.Sin((x * Mathf.PI) / 2f);
    }

    public static float easeInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
    }

    public static float easeInQuad(float x)
    {
        return x * x;
    }

    public static float easeOutQuad(float x)
    {
        return 1f - (1f - x) * (1f - x);
    }

    public static float easeInOutQuad(float x)
    {
        return x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
    }

    public static float easeInCubic(float x)
    {
        return x * x * x;
    }

    public static float easeOutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    public static float easeInOutCubic(float x)
    {
        return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
    }

    public static float easeInQuart(float x)
    {
        return x * x * x * x;
    }

    public static float easeOutQuart(float x)
    {
        return 1f - Mathf.Pow(1f - x, 4f);
    }

    public static float easeInOutQuart(float x)
    {
        return x < 0.5f ? 8f * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 4f) / 2f;
    }

    public static float easeInQuint(float x)
    {
        return x * x * x * x * x;
    }

    public static float easeOutQuint(float x)
    {
        return 1f - Mathf.Pow(1f - x, 5f);
    }

    public static float easeInOutQuint(float x)
    {
        return x < 0.5f ? 16f * x * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f;
    }

    public static float easeInExpo(float x)
    {
        return x == 0f ? 0f : Mathf.Pow(2f, 10f * x - 10f);
    }

    public static float easeOutExpo(float x)
    {
        return x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
    }

    public static float easeInOutExpo(float x)
    {
        return x == 0f
        ? 0f
        : x == 1f
        ? 1f
        : x < 0.5f ? Mathf.Pow(2f, 20f * x - 10f) / 2f
        : (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;
    }

    public static float easeInCirc(float x)
    {
        return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2f));
    }

    public static float easeOutCirc(float x)
    {
        return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
    }

    public static float easeInOutCirc(float x)
    {
        return x < 0.5f
        ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f
        : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
    }
}
