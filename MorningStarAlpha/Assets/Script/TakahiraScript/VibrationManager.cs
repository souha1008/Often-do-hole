using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class VibrationManager : SingletonMonoBehaviour<VibrationManager>
{
    private bool VibrationFlag = true;   // 振動フラグ
    private Coroutine _Vibration;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }

    private void Start()
    {
        if (SaveDataManager.Instance.MainData != null)
        {
            VibrationFlag = SaveDataManager.Instance.MainData.VibrationFlag;
        }
    }

    private void Update()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
            return;


        // 以下振動テスト


        // A ボタンが押されたら
        if (gamepad.aButton.wasPressedThisFrame)
        {
            // 低周波（左）モーターの強さを 1、
            // 高周波（右）モーターの強さを 0、
            // 0.3f秒間かけて振動させる
            StartVibration(1, 0, 0.3f);
            Debug.LogWarning("Aボタン押した");
            SoundManager.Instance.PlaySound("決定音");
        }
        // B ボタンが押されたら
        else if (gamepad.bButton.wasPressedThisFrame)
        {
            // 低周波（左）モーターの強さを 0、
            // 高周波（右）モーターの強さを 1、
            // 0.3f秒間かけて振動させる
            StartVibration(1, 0, 0.3f);
            Debug.LogWarning("Bボタン押した");
            SoundManager.Instance.PlaySound("決定音");
        }


        // ここまで
    }

    //===============================================
    // 振動機能(開始)
    //===============================================
    // 引数：低周波（左）モーターの強さ（0.0 〜 1.0）
    // 　　　高周波（右）モーターの強さ（0.0 〜 1.0）
    // 　　　振動時間
    // ==============================================
    public void StartVibration(float lowFrequency, float highFrequency, float VibrationTime)
    {
        if (_Vibration != null) StopCoroutine(_Vibration);
        if (VibrationFlag) _Vibration = StartCoroutine(Vibration(lowFrequency, highFrequency, VibrationTime));
    }

    //===============================================
    // 振動機能(停止)
    //===============================================
    public void StopVibration()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.LogWarning("ゲームパッドなし");
            return;
        }
        gamepad.SetMotorSpeeds(0, 0);
    }

    public void SetVibrationFlag(bool vibrationFlag)
    {
        VibrationFlag = vibrationFlag;

        // セーブ
        if (SaveDataManager.Instance.MainData != null)
        {
            SaveDataManager.Instance.MainData.VibrationFlag = VibrationFlag;
        }
    }

    public bool GetVibrationFlag()
    {
        return VibrationFlag;
    }



    private static IEnumerator Vibration
    (
        float lowFrequency, // 低周波（左）モーターの強さ（0.0 〜 1.0）
        float highFrequency, // 高周波（右）モーターの強さ（0.0 〜 1.0）
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
        yield return new WaitForSecondsRealtime(VibrationTime); // VibrationTime振動させる
        gamepad.SetMotorSpeeds(0, 0);
    }
}
