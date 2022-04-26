using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CheckPointManager : SingletonMonoBehaviour<CheckPointManager>
{
    [Label("���݂̃��X�|�[���I�u�W�F�N�g")]
    public GameObject RespawnObject = null;
    [Label("���݂̃��X�|�[�����W")]    
    public Vector3 NowRespawnPos;       // ���݂̃��X�|�[�����W(���邾��)

    public static Vector3 RespawnPos;   // ���X�|�[�����W
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

        // �������X�|�[���Z�b�g
        if (RespawnObject != null)
        {
            SetCheckPoint(RespawnObject.GetComponentInChildren<CheckPoint>());
        }
    }


    // �`�F�b�N�|�C���g�̃Z�b�g
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        RespawnPos = checkpoint.RespawnPointObject.transform.position;
        noTouchCheckPoint = true;
        RespawnObject = checkpoint.GetComponentInParent<MeshOnOff>().gameObject; // ���b�V���؂�ւ��̕t�����e�I�u�W�F�N�g�擾
        NowRespawnPos = GetCheckPointPos(); // ���݂̃��X�|�[�����W�X�V
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