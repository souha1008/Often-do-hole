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
}
