using UnityEngine;

// イージングのタイプ
public enum EASING_TYPE
{
    LINEAR = 0,
    QUAD_IN,
    QUAD_OUT,
    QUAD_INOUT,
    CUBIC_IN,
    CUBIC_OUT,
    CUBIC_INOUT,
    QUART_IN,
    QUART_OUT,
    QUART_INOUT,
    QUINT_IN,
    QUINT_OUT,
    QUINT_INOUT,
    SINE_IN,
    SINE_OUT,
    SINE_INOUT,
    EXP_IN,
    EXP_OUT,
    EXP_INOUT,
    CIRC_IN,
    CIRC_OUT,
    CIRC_INOUT,
    ELASTIC_IN,
    ELASTIC_OUT,
    ELASTIC_INOUT,
    BACK_IN,
    BACK_OUT,
    BACK_INOUT,
    BOUNCE_IN,
    BOUNCE_OUT,
    BOUNCE_INOUT
};

public static class Easing
{
    // イージングタイプで動き処理
    public static float EasingTypeFloat(EASING_TYPE EasingType, float NowTime, float MoveTime, float StartPos, float EndPos)
    {
        switch (EasingType)
        {
            case EASING_TYPE.LINEAR:
                return Easing.Linear(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.QUAD_IN:
                return Easing.QuadIn(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.QUAD_OUT:
                return Easing.QuadOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.QUAD_INOUT:
                return Easing.QuadInOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.CUBIC_IN:
                return Easing.CubicIn(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.CUBIC_OUT:
                return Easing.CubicOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.CUBIC_INOUT:
                return Easing.CubicInOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.QUART_IN:
                return Easing.QuartIn(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.QUART_OUT:
                return Easing.QuartOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.QUART_INOUT:
                return Easing.QuartInOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.QUINT_IN:
                return Easing.QuintIn(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.QUINT_OUT:
                return Easing.QuintOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.QUINT_INOUT:
                return Easing.QuintInOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.SINE_IN:
                return Easing.SineIn(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.SINE_OUT:
                return Easing.SineOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.SINE_INOUT:
                return Easing.SineInOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.EXP_IN:
                return Easing.ExpIn(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.EXP_OUT:
                return Easing.ExpOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.EXP_INOUT:
                return Easing.ExpInOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.CIRC_IN:
                return Easing.CircIn(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.CIRC_OUT:
                return Easing.CircOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.CIRC_INOUT:
                return Easing.CircInOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.ELASTIC_IN:
                return Easing.ElasticIn(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.ELASTIC_OUT:
                return Easing.ElasticOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.ELASTIC_INOUT:
                return Easing.ElasticInOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.BACK_IN:
                return Easing.BackIn(NowTime, MoveTime, StartPos, EndPos, Mathf.Abs(StartPos - EndPos) * 0.3f);
            case EASING_TYPE.BACK_OUT:
                return Easing.BackOut(NowTime, MoveTime, StartPos, EndPos, Mathf.Abs(StartPos - EndPos) * 0.3f);
            case EASING_TYPE.BACK_INOUT:
                return Easing.BackInOut(NowTime, MoveTime, StartPos, EndPos, Mathf.Abs(StartPos - EndPos) * 0.3f);
            case EASING_TYPE.BOUNCE_IN:
                return Easing.BounceIn(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.BOUNCE_OUT:
                return Easing.BounceOut(NowTime, MoveTime, StartPos, EndPos);
            case EASING_TYPE.BOUNCE_INOUT:
                return Easing.BounceInOut(NowTime, MoveTime, StartPos, EndPos);
            default:
                return Easing.QuadInOut(NowTime, MoveTime, StartPos, EndPos);
        }
    }
    public static float QuadIn(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime;
        return max * t * t + min;
    }

    public static float QuadOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime;
        return -max * t * (t - 2) + min;
    }

    public static float QuadInOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * t * t + min;

        t = t - 1;
        return -max / 2 * (t * (t - 2) - 1) + min;
    }

    public static float CubicIn(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime;
        return max * t * t * t + min;
    }

    public static float CubicOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t = t / totaltime - 1;
        return max * (t * t * t + 1) + min;
    }

    public static float CubicInOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * t * t * t + min;

        t = t - 2;
        return max / 2 * (t * t * t + 2) + min;
    }

    public static float QuartIn(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime;
        return max * t * t * t * t + min;
    }

    public static float QuartOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t = t / totaltime - 1;
        return -max * (t * t * t * t - 1) + min;
    }

    public static float QuartInOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * t * t * t * t + min;

        t = t - 2;
        return -max / 2 * (t * t * t * t - 2) + min;
    }

    public static float QuintIn(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime;
        return max * t * t * t * t * t + min;
    }

    public static float QuintOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t = t / totaltime - 1;
        return max * (t * t * t * t * t + 1) + min;
    }

    public static float QuintInOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * t * t * t * t * t + min;

        t = t - 2;
        return max / 2 * (t * t * t * t * t + 2) + min;
    }

    public static float SineIn(float t, float totaltime, float min, float max)
    {
        max -= min;
        return -max * Mathf.Cos(t * (Mathf.PI * 90 / 180) / totaltime) + max + min;
    }

    public static float SineOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        return max * Mathf.Sin(t * (Mathf.PI * 90 / 180) / totaltime) + min;
    }

    public static float SineInOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        return -max / 2 * (Mathf.Cos(t * Mathf.PI / totaltime) - 1) + min;
    }

    public static float ExpIn(float t, float totaltime, float min, float max)
    {
        max -= min;
        return t == 0.0 ? min : max * Mathf.Pow(2, 10 * (t / totaltime - 1)) + min;
    }

    public static float ExpOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        return t == totaltime ? max + min : max * (-Mathf.Pow(2, -10 * t / totaltime) + 1) + min;
    }

    public static float ExpInOut(float t, float totaltime, float min, float max)
    {
        if (t == 0.0f) return min;
        if (t == totaltime) return max;
        max -= min;
        t /= totaltime / 2;

        if (t < 1) return max / 2 * Mathf.Pow(2, 10 * (t - 1)) + min;

        t = t - 1;
        return max / 2 * (-Mathf.Pow(2, -10 * t) + 2) + min;

    }

    public static float CircIn(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime;
        return -max * (Mathf.Sqrt(1 - t * t) - 1) + min;
    }

    public static float CircOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t = t / totaltime - 1;
        return max * Mathf.Sqrt(1 - t * t) + min;
    }

    public static float CircInOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return -max / 2 * (Mathf.Sqrt(1 - t * t) - 1) + min;

        t = t - 2;
        return max / 2 * (Mathf.Sqrt(1 - t * t) + 1) + min;
    }

    public static float ElasticIn(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime;

        float s = 1.70158f;
        float p = totaltime * 0.3f;
        float a = max;

        if (t == 0) return min;
        if (t == 1) return min + max;

        if (a < Mathf.Abs(max))
        {
            a = max;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(max / a);
        }

        t = t - 1;
        return -(a * Mathf.Pow(2, 10 * t) * Mathf.Sin((t * totaltime - s) * (2 * Mathf.PI) / p)) + min;
    }

    public static float ElasticOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime;

        float s = 1.70158f;
        float p = totaltime * 0.3f; ;
        float a = max;

        if (t == 0) return min;
        if (t == 1) return min + max;

        if (a < Mathf.Abs(max))
        {
            a = max;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(max / a);
        }

        return a * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * totaltime - s) * (2 * Mathf.PI) / p) + max + min;
    }

    public static float ElasticInOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime / 2;

        float s = 1.70158f;
        float p = totaltime * (0.3f * 1.5f);
        float a = max;

        if (t == 0) return min;
        if (t == 2) return min + max;

        if (a < Mathf.Abs(max))
        {
            a = max;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(max / a);
        }

        if (t < 1)
        {
            return -0.5f * (a * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * totaltime - s) * (2 * Mathf.PI) / p)) + min;
        }

        t = t - 1;
        return a * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * totaltime - s) * (2 * Mathf.PI) / p) * 0.5f + max + min;
    }

    public static float BackIn(float t, float totaltime, float min, float max, float s)
    {
        max -= min;
        t /= totaltime;
        return max * t * t * ((s + 1) * t - s) + min;
    }

    public static float BackOut(float t, float totaltime, float min, float max, float s)
    {
        max -= min;
        t = t / totaltime - 1;
        return max * (t * t * ((s + 1) * t + s) + 1) + min;
    }

    public static float BackInOut(float t, float totaltime, float min, float max, float s)
    {
        max -= min;
        s *= 1.525f;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * (t * t * ((s + 1) * t - s)) + min;

        t = t - 2;
        return max / 2 * (t * t * ((s + 1) * t + s) + 2) + min;
    }

    public static float BounceIn(float t, float totaltime, float min, float max)
    {
        max -= min;
        return max - BounceOut(totaltime - t, totaltime, 0, max) + min;
    }

    public static float BounceOut(float t, float totaltime, float min, float max)
    {
        max -= min;
        t /= totaltime;

        if (t < 1.0f / 2.75f)
        {
            return max * (7.5625f * t * t) + min;
        }
        else if (t < 2.0f / 2.75f)
        {
            t -= 1.5f / 2.75f;
            return max * (7.5625f * t * t + 0.75f) + min;
        }
        else if (t < 2.5f / 2.75f)
        {
            t -= 2.25f / 2.75f;
            return max * (7.5625f * t * t + 0.9375f) + min;
        }
        else
        {
            t -= 2.625f / 2.75f;
            return max * (7.5625f * t * t + 0.984375f) + min;
        }
    }

    public static float BounceInOut(float t, float totaltime, float min, float max)
    {
        if (t < totaltime / 2)
        {
            return BounceIn(t * 2, totaltime, 0, max - min) * 0.5f + min;
        }
        else
        {
            return BounceOut(t * 2 - totaltime, totaltime, 0, max - min) * 0.5f + min + (max - min) * 0.5f;
        }
    }

    public static float Linear(float t, float totaltime, float min, float max)
    {
        return (max - min) * t / totaltime + min;
    }

}

public class Easing2D
{
    public static Vector2 QuadIn(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime;
        return max * t * t + min;
    }

    public static Vector2 QuadOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime;
        return -max * t * (t - 2) + min;
    }

    public static Vector2 QuadInOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * t * t + min;

        t = t - 1;
        return -max / 2 * (t * (t - 2) - 1) + min;
    }

    public static Vector2 CubicIn(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime;
        return max * t * t * t + min;
    }

    public static Vector2 CubicOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t = t / totaltime - 1;
        return max * (t * t * t + 1) + min;
    }

    public static Vector2 CubicInOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * t * t * t + min;

        t = t - 2;
        return max / 2 * (t * t * t + 2) + min;
    }

    public static Vector2 QuartIn(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime;
        return max * t * t * t * t + min;
    }

    public static Vector2 QuartOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t = t / totaltime - 1;
        return -max * (t * t * t * t - 1) + min;
    }

    public static Vector2 QuartInOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * t * t * t * t + min;

        t = t - 2;
        return -max / 2 * (t * t * t * t - 2) + min;
    }

    public static Vector2 QuintIn(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime;
        return max * t * t * t * t * t + min;
    }

    public static Vector2 QuintOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t = t / totaltime - 1;
        return max * (t * t * t * t * t + 1) + min;
    }

    public static Vector2 QuintInOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * t * t * t * t * t + min;

        t = t - 2;
        return max / 2 * (t * t * t * t * t + 2) + min;
    }

    public static Vector2 SineIn(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        return -max * Mathf.Cos(t * (Mathf.PI * 90 / 180) / totaltime) + max + min;
    }

    public static Vector2 SineOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        return max * Mathf.Sin(t * (Mathf.PI * 90 / 180) / totaltime) + min;
    }

    public static Vector2 SineInOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        return -max / 2 * (Mathf.Cos(t * Mathf.PI / totaltime) - 1) + min;
    }

    public static Vector2 ExpIn(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        return t == 0.0 ? min : max * Mathf.Pow(2, 10 * (t / totaltime - 1)) + min;
    }

    public static Vector2 ExpOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        return t == totaltime ? max + min : max * (-Mathf.Pow(2, -10 * t / totaltime) + 1) + min;
    }

    public static Vector2 ExpInOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        if (t == 0.0f) return min;
        if (t == totaltime) return max;
        max -= min;
        t /= totaltime / 2;

        if (t < 1) return max / 2 * Mathf.Pow(2, 10 * (t - 1)) + min;

        t = t - 1;
        return max / 2 * (-Mathf.Pow(2, -10 * t) + 2) + min;

    }

    public static Vector2 CircIn(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime;
        return -max * (Mathf.Sqrt(1 - t * t) - 1) + min;
    }

    public static Vector2 CircOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t = t / totaltime - 1;
        return max * Mathf.Sqrt(1 - t * t) + min;
    }

    public static Vector2 CircInOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime / 2;
        if (t < 1) return -max / 2 * (Mathf.Sqrt(1 - t * t) - 1) + min;

        t = t - 2;
        return max / 2 * (Mathf.Sqrt(1 - t * t) + 1) + min;
    }

    public static Vector2 BackIn(float t, float totaltime, Vector2 min, Vector2 max, float s)
    {
        max -= min;
        t /= totaltime;
        return max * t * t * ((s + 1) * t - s) + min;
    }

    public static Vector2 BackOut(float t, float totaltime, Vector2 min, Vector2 max, float s)
    {
        max -= min;
        t = t / totaltime - 1;
        return max * (t * t * ((s + 1) * t + s) + 1) + min;
    }

    public static Vector2 BackInOut(float t, float totaltime, Vector2 min, Vector2 max, float s)
    {
        max -= min;
        s *= 1.525f;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * (t * t * ((s + 1) * t - s)) + min;

        t = t - 2;
        return max / 2 * (t * t * ((s + 1) * t + s) + 2) + min;
    }

    public static Vector2 BounceIn(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        return max - BounceOut(totaltime - t, totaltime, new Vector2(), max) + min;
    }

    public static Vector2 BounceOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        t /= totaltime;

        if (t < 1.0f / 2.75f)
        {
            return max * (7.5625f * t * t) + min;
        }
        else if (t < 2.0f / 2.75f)
        {
            t -= 1.5f / 2.75f;
            return max * (7.5625f * t * t + 0.75f) + min;
        }
        else if (t < 2.5f / 2.75f)
        {
            t -= 2.25f / 2.75f;
            return max * (7.5625f * t * t + 0.9375f) + min;
        }
        else
        {
            t -= 2.625f / 2.75f;
            return max * (7.5625f * t * t + 0.984375f) + min;
        }
    }

    public static Vector2 BounceInOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        if (t < totaltime / 2)
        {
            return BounceIn(t * 2, totaltime, new Vector2(), max - min) * 0.5f + min;
        }
        else
        {
            return BounceOut(t * 2 - totaltime, totaltime, new Vector2(), max - min) * 0.5f + min + (max - min) * 0.5f;
        }
    }

    public static Vector2 Linear(float t, float totaltime, Vector2 min, Vector2 max)
    {
        return (max - min) * t / totaltime + min;
    }

}