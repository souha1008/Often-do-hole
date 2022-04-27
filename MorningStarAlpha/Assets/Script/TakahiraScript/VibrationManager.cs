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

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
    }

    private void Update()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
            return;


        // �ȉ��U���e�X�g


        // A �{�^���������ꂽ��
        if (gamepad.aButton.wasPressedThisFrame)
        {
            // ����g�i���j���[�^�[�̋����� 1�A
            // �����g�i�E�j���[�^�[�̋����� 0�A
            // 0.3f�b�Ԃ����ĐU��������
            StartCoroutine(Vibration(1, 0, 0.3f));
            Debug.LogWarning("A�{�^��������");
            SoundManager.Instance.PlaySound("���艹");
        }
        // B �{�^���������ꂽ��
        else if (gamepad.bButton.wasPressedThisFrame)
        {
            // ����g�i���j���[�^�[�̋����� 0�A
            // �����g�i�E�j���[�^�[�̋����� 1�A
            // 0.3f�b�Ԃ����ĐU��������
            StartCoroutine(Vibration(0, 1, 0.3f));
            Debug.LogWarning("B�{�^��������");
            SoundManager.Instance.PlaySound("���艹");
        }


        // �����܂�
    }

    //===============================================
    // �U���@�\(�J�n)
    //===============================================
    // �����F����g�i���j���[�^�[�̋����i0.0 �` 1.0�j
    // �@�@�@�����g�i�E�j���[�^�[�̋����i0.0 �` 1.0�j
    // �@�@�@�U������
    // ==============================================
    public void StartVibration(float lowFrequency, float highFrequency, float VibrationTime)
    {
        Vibration (lowFrequency, highFrequency, VibrationTime);
    }

    //===============================================
    // �U���@�\(��~)
    //===============================================
    public void StopVibration()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.LogWarning("�Q�[���p�b�h�Ȃ�");
            return;
        }
        gamepad.SetMotorSpeeds(0, 0);
    }



    private static IEnumerator Vibration
    (
        float lowFrequency, // ����g�i���j���[�^�[�̋����i0.0 �` 1.0�j
        float highFrequency, // �����g�i�E�j���[�^�[�̋����i0.0 �` 1.0�j
        float VibrationTime // �U������
    )
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.LogWarning("�Q�[���p�b�h�Ȃ�");
            yield break;
        }

        gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
        yield return new WaitForSeconds(VibrationTime); // VibrationTime�U��������
        gamepad.SetMotorSpeeds(0, 0);
    }
}
