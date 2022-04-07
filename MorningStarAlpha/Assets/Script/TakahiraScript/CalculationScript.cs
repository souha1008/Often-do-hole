using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class CalculationScript
{
    // �p�x�v�Z�p
    public static float AngleCalculation(float AngleDeg)
    {
        // ���͂��ꂽ�p�x�����W�A���ɕϊ�
        float rad = AngleDeg * Mathf.Deg2Rad;

        return rad;
    }


    /// <summary>
    /// ��_�Ԃ̊p�x�����߂�
    /// �߂�l�F0�`360
    /// </summary>
    public static float TwoPointAngle360(Vector3 origin, Vector3 target)
    {
        Vector3 dt = target - origin;
        float rad = Mathf.Atan2(dt.x, dt.y);
        float degree = rad * Mathf.Rad2Deg;
        if (degree < 0)//���������Ɏ��v����0~360�̒l�ɕ␳
        {
            degree += 360;
        }
        return degree;
    }

    // ��_�Ԃ̊p�x�����߂�(Unity��Transform�p�x�ƈ�v)
    // �߂�l�F0�`360(Z�p�x)
    public static float UnityTwoPointAngle360(Vector3 origin, Vector3 target)
    {
        Vector3 dt = target - origin;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;
        if (degree < 0) // �E��������Ɏ��v����0~360�̒l�ɕ␳
        {
            degree += 360;
        }
        return degree;
    }

    // XY�����v�Z�p
    public static Vector3 AngleVectorXY(float AngleRad)
    {
        // ���ꂼ��̎��̐������v�Z
        float x = Mathf.Cos(AngleRad);
        float y = Mathf.Sin(AngleRad);
        float z = 0f;

        // Vector3�^�Ɋi�[
        return new Vector3(x, y, z);
    }

    // true�Ȃ�1.0f��false�Ȃ�-1.0f��Ԃ�
    public static float FugouChange(bool TrueorFalse)
    {
        if (TrueorFalse)
            return 1.0f;
        else
            return -1.0f;
    }
}
