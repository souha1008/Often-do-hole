using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CheckPointManager : SingletonMonoBehaviour<CheckPointManager>
{
    [Header("���݂̃��X�|�[�����W")]
    public static Vector3 RespawnPos;          // ���X�|�[�����W
    public static bool noTouchCheckPoint;
    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
        RespawnPos = Vector3.zero;
        noTouchCheckPoint = false;
    }

    // �`�F�b�N�|�C���g�g�p����
    public void CheckPointAction()
    {
        // �Q�[���V�[���̃��Z�b�g
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // �`�F�b�N�|�C���g�̃Z�b�g
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        RespawnPos = checkpoint.RespawnPointObject.transform.position;
        noTouchCheckPoint = true;
    }

    // ���݂̃`�F�b�N�|�C���g�̍��W�Q�b�g
    public static Vector3 GetCheckPointPos()
    {
        return RespawnPos;
    }

    // ���݂̃`�F�b�N�|�C���g�̍��W�Q�b�g
    public static bool isTouchCheckPos()
    {
        return noTouchCheckPoint;
    }
}