using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class VibrationManager : SingletonMonoBehaviour<VibrationManager>
{
    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }

    private void Update()
    {
        var gamepad = Gamepad.current;


        // A ボタンが押されたら
        if (gamepad.aButton.wasPressedThisFrame)
        {
            // 低周波（左）モーターの強さを 1、
            // 高周波（右）モーターの強さを 0、
            // 0.3f秒間かけて振動させる
            StartCoroutine(Vibration(1, 0, 0.3f));
            Debug.LogWarning("Aボタン押した");
        }
        // B ボタンが押されたら
        else if (gamepad.bButton.wasPressedThisFrame)
        {
            // 低周波（左）モーターの強さを 0、
            // 高周波（右）モーターの強さを 1、
            // 0.3f秒間かけて振動させる
            StartCoroutine(Vibration(0, 1, 0.3f));
            Debug.LogWarning("Bボタン押した");
        }
    }

    private static IEnumerator Vibration
    (
        float lowFrequency, // 低周波（左）モーターの強さ（0.0 ～ 1.0）
        float highFrequency, // 高周波（右）モーターの強さ（0.0 ～ 1.0）
        float VibrationTime // 振動時間
    )
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.LogWarning("ゲームパッドなし");
            yield break;
        }

        gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
        yield return new WaitForSeconds(VibrationTime); // 1 秒間振動させる
        gamepad.SetMotorSpeeds(0, 0);
    }
}
