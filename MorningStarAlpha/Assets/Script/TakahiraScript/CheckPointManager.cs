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

    public static Vector3 RespawnPos = Vector3.zero;   // ���X�|�[�����W
    public bool RespawnFlag = false;   // �`�F�b�N�|�C���g�Z�b�g�m�F�t���O

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�

        RespawnFlag = false;
    }


    // �`�F�b�N�|�C���g�̃Z�b�g
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        if (RespawnPos != checkpoint.RespawnPointObject.transform.position)
        {
            RespawnPos = checkpoint.RespawnPointObject.transform.position;
            RespawnObject = checkpoint.GetComponentInParent<MeshOnOff>().gameObject; // ���b�V���؂�ւ��̕t�����e�I�u�W�F�N�g�擾
            NowRespawnPos = GetCheckPointPos(); // ���݂̃��X�|�[�����W�X�V

            CoinManager.Instance.SetCheckPointCoinData(); // �R�C���̏������
        }
        RespawnFlag = true;
    }

    // ���݂̃`�F�b�N�|�C���g�̍��W�Q�b�g
    public Vector3 GetCheckPointPos()
    {
        //Debug.LogWarning(RespawnPos);
        return RespawnPos;
    }

    // �`�F�b�N�|�C���g���Z�b�g
    public void ResetCheckPoint()
    {
        RespawnPos = Vector3.zero;
        NowRespawnPos = GetCheckPointPos();
        RespawnObject = null;
        RespawnFlag = false;
    }
}