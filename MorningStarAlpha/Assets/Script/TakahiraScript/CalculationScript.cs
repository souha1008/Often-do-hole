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
}
