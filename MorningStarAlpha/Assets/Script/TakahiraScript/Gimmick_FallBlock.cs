using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock : Gimmick_Main
{
    [Header("��������")]
    public float FallLength = 15.0f;   // �����鋗��

    [Header("���b�����ė������邩")]
    public float FallTime = 3.0f;       // ���b�����ė������邩

    private bool NowFall;               // ��������
    private float NowTime;              // �o�ߎ���
    private Vector3 StartPos;           // �������W
    private GameObject PlayerObject;    // �v���C���[�I�u�W�F�N�g

    public override void Init()
    {
        // ������
        NowFall = false;
        NowTime = 0.0f;
        StartPos = this.gameObject.transform.position;
        PlayerObject = null;

        // �R���W����
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // �g���K�[�I�t

        // ���W�b�h�{�f�B
        Rb.isKinematic = true;  // �L�l�}�e�B�b�N�I��
    }

    public override void UpdateMove()
    {
        // �|�W�V�����ύX
        if (NowFall)
        {
            //Vector3 OldPos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(StartPos.x, Easing.QuartIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
            NowTime += Time.deltaTime;

            // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N����������
            //if (PlayerObject != null)
            //{
            //    Ray ray = new Ray(PlayerObject.transform.position, Vector3.down);
            //    RaycastHit hit;
            //    if (Physics.Raycast(ray, out hit, 1.5f))
            //    {
            //        if(hit.collider.gameObject == this.gameObject)
            //        {
            //            PlayerObject.gameObject.transform.position =
            //                PlayerObject.gameObject.transform.position + new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
            //        }
            //    }
            //}
        }
        if (NowTime > FallTime)
        {
            Death();
        }
    }

    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        // �v���C���[���d�ƐڐG
        if (collision.gameObject.tag == "Bullet")
        {
            NowFall = true; // ������
            //Rb.isKinematic = false;
        }
        else if (collision.gameObject.tag == "Player")
        {
            // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N���������痎��
            Ray ray_1 = new Ray(collision.gameObject.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray_1, out hit, 1.5f))
            {
                NowFall = true; // ������
                //Rb.isKinematic = false;
                PlayerObject = collision.gameObject;
            }
        }
    }
}
