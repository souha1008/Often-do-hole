using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class CalculationScript
{
    // Šp“xŒvZ—p
    public static float AngleCalculation(float AngleDeg)
    {
        // “ü—Í‚³‚ê‚½Šp“x‚ğƒ‰ƒWƒAƒ“‚É•ÏŠ·
        float rad = AngleDeg * Mathf.Deg2Rad;

        return rad;
    }

    // XY¬•ªŒvZ—p
    public static Vector3 AngleVectorXY(float AngleRad)
    {
        // ‚»‚ê‚¼‚ê‚Ì²‚Ì¬•ª‚ğŒvZ
        float x = Mathf.Cos(AngleRad);
        float y = Mathf.Sin(AngleRad);
        float z = 0f;

        // Vector3Œ^‚ÉŠi”[
        return new Vector3(x, y, z);
    }
}
