using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointManager : SingletonMonoBehaviour<CheckPointManager>
{
    [Header("�v���C���[�I�u�W�F�N�g(�Ȃ��ꍇ��\"Player\"��T��)")]
    public GameObject PlayerObject;     // �v���C���[�I�u�W�F�N�g
    [Header("���݂̃��X�|�[�����W")]
    public Vector3 RespawnPos;          // ���X�|�[�����W

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
    }

    // �`�F�b�N�|�C���g�g�p����
    public void CheckPointAction()
    {
        // �Q�[���V�[���̃��Z�b�g
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // �v���C���[�̈ʒu��ύX
        if (PlayerObject == null)
        {
            PlayerObject = GameObject.Find("Player");
        }
        PlayerObject.GetComponent<Transform>().position = GetCheckPointPos();
        PlayerObject.GetComponent<PlayerMain>().vel = Vector3.zero;
        //Debug.Log("���X�|�[�����W�F" + GetCheckPointPos());
    }


    // �`�F�b�N�|�C���g�̃Z�b�g
    public void SetCheckPoint(CheckPoint checkpoint)
    {
        RespawnPos = checkpoint.RespawnPointObject.transform.position;
    }

    // ���݂̃`�F�b�N�|�C���g�̍��W�Q�b�g
    public Vector3 GetCheckPointPos()
    {
        return RespawnPos;
    }
}