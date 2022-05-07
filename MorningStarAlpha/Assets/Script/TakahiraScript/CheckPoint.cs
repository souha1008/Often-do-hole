using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // �ϐ�
    [Label("�������W�I�u�W�F�N�g")]
    public GameObject RespawnPointObject;                       // �������W�I�u�W�F�N�g
    [Label("���b�V���؂�ւ��p�X�N���v�g�������I�u�W�F�N�g")]
    [SerializeField] private GameObject MeshOnOffObject;        // ���b�V���̕\����\���؂�ւ��p

    private void Awake()
    {
        // �������X�Z�b�g
        if (MeshOnOffObject.GetComponent<MeshOnOff>().InitCheckPoint)
        {
            CheckPointManager.Instance.SetCheckPoint(this);
        }
    }


    // Start is called before the first frame update
    private void Start()
    {
        // �R���C�_�[
        this.gameObject.GetComponent<Collider>().isTrigger = true;  // �g���K�[�I��
        RespawnPointObject.GetComponent<Collider>().isTrigger = true;     // �g���K�[�I��

        // �`�F�b�N�|�C���g�̃��b�V���I���I�t�p�X�N���v�g���Q�Ƃ��āA������ or �����Ȃ�����
        if (MeshOnOffObject.GetComponent<MeshOnOff>().MeshOn)
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            RespawnPointObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            RespawnPointObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            // �`�F�b�N�|�C���g��HitBox�ɐG�ꂽ��`�F�b�N�|�C���g�X�V
            CheckPointManager.Instance.SetCheckPoint(this);
        }
    }
}
