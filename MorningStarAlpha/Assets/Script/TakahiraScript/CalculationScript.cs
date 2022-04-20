using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct VecQuaternion
{
    public Vector3 Pos;
    public Quaternion quaternion;
}


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

    // ある点を中心に回転
    //
    // 引数　：　基準点, 回転したい点, 角度, 回転軸(Vector.right, Vector.up　Etc...)
    // 戻り値：　回転後の座標、回転分のクオータニオン
    public static VecQuaternion PointRotate(Vector3 OriginPos, Vector3 TargetPos, float Angle, Vector3 Axis)
    {
        VecQuaternion vecQuaternion ;

        Quaternion AngleAxis = Quaternion.AngleAxis(Angle, Axis); // 回転軸と角度

        Vector3 Pos = TargetPos; // 自身の座標

        Pos -= OriginPos;
        Pos = AngleAxis * Pos;
        Pos += OriginPos;

        vecQuaternion.Pos = Pos; // 現在の座標
        vecQuaternion.quaternion = AngleAxis; // 回転

        return vecQuaternion;
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


    /// <summary>
    /// パーリンノイズ値を取得
    /// </summary>
    /// <param name="offset">オフセット値</param>
    /// <param name="speed">速度</param>
    /// <param name="time">時間</param>
    /// <returns>パーリンノイズ値(-1.0〜1.0)</returns>
    public static float GetPerlinNoiseValue(float offset, float speed, float time)
    {
        // パーリンノイズ値を取得する
        // X: オフセット値 + 速度 * 時間
        // Y: 0.0固定
        var perlinNoise = Mathf.PerlinNoise(offset + speed * time, 0.0f);
        // 0.0〜1.0 -> -1.0〜1.0に変換して返却
        return (perlinNoise - 0.5f) * 2.0f;
    }
}
