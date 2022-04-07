using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class CalculationScript
{
    // 角度計算用
    public static float AngleCalculation(float AngleDeg)
    {
        // 入力された角度をラジアンに変換
        float rad = AngleDeg * Mathf.Deg2Rad;

        return rad;
    }


    /// <summary>
    /// 二点間の角度を求める
    /// 戻り値：0～360
    /// </summary>
    public static float TwoPointAngle360(Vector3 origin, Vector3 target)
    {
        Vector3 dt = target - origin;
        float rad = Mathf.Atan2(dt.x, dt.y);
        float degree = rad * Mathf.Rad2Deg;
        if (degree < 0)//上方向を基準に時計回りに0~360の値に補正
        {
            degree += 360;
        }
        return degree;
    }

    // 二点間の角度を求める(UnityのTransform角度と一致)
    // 戻り値：0～360(Z角度)
    public static float UnityTwoPointAngle360(Vector3 origin, Vector3 target)
    {
        Vector3 dt = target - origin;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;
        if (degree < 0) // 右方向を基準に時計回りに0~360の値に補正
        {
            degree += 360;
        }
        return degree;
    }

    // XY成分計算用
    public static Vector3 AngleVectorXY(float AngleRad)
    {
        // それぞれの軸の成分を計算
        float x = Mathf.Cos(AngleRad);
        float y = Mathf.Sin(AngleRad);
        float z = 0f;

        // Vector3型に格納
        return new Vector3(x, y, z);
    }

    // trueなら1.0fをfalseなら-1.0fを返す
    public static float FugouChange(bool TrueorFalse)
    {
        if (TrueorFalse)
            return 1.0f;
        else
            return -1.0f;
    }


    // <summary>
    /// 球の内側か
    /// (x - a)^2 + (y - b)^2 + (z - c)^2 <= r^2
    /// </summary>
    /// <param name="p">球の中心座標</param>
    /// <param name="r">半径</param>
    /// <param name="c">対象となる点</param>
    /// <returns></returns>
    public static bool InSphere(Vector3 p, float r, Vector3 c)
    {
        var sum = 0f;
        for (var i = 0; i < 3; i++)
            sum += Mathf.Pow(p[i] - c[i], 2);
        return sum <= Mathf.Pow(r, 2f);
    }
}
