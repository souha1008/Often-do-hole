using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock : Gimmick_Main
{
    [Label("��������")]
    public float FallLength = 15.0f;    // �����鋗��

    [Label("���b�����ė������邩")]
    public float FallTime = 3.0f;       // ���b�����ė������邩

    private bool NowFall;               // ��������
    private float NowTime;              // �o�ߎ���
    private Vector3 StartPos;           // �������W
    private GameObject PlayerObject;    // �v���C���[�I�u�W�F�N�g
    private GameObject BulletObject;    // �d�I�u�W�F�N�g
    private BulletMain BulletMainScript;// �d���C���X�N���v�g

    public override void Init()
    {
        // ������
        NowFall = false;
        NowTime = 0.0f;
        StartPos = this.gameObject.transform.position;
        PlayerObject = GameObject.Find("Player");
        BulletObject = null;
        BulletMainScript = null;

        // �R���W����
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // �g���K�[�I�t

        // ���W�b�h�{�f�B
        Rb.isKinematic = true;  // �L�l�}�e�B�b�N�I��
    }

    public override void FixedMove()
    {
        // �|�W�V�����ύX
        if (NowFall)
        {
            Vector3 OldPos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(StartPos.x, Easing.QuartIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
            NowTime += Time.deltaTime;

            // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N����������
            if (PlayerObject != null)
            {
                Ray ray = new Ray(PlayerObject.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1.5f))
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        PlayerObject.gameObject.transform.position =
                            PlayerObject.gameObject.transform.position + new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
                    }
                }
            }

            if (BulletObject != null && BulletMainScript != null)
            {
                if (BulletMainScript.isTouched)
                {
                    BulletObject.gameObject.transform.position =
                            BulletObject.gameObject.transform.position + new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
                }
                else
                {
                    BulletObject = null;
                    BulletMainScript = null;
                }
            }
        }
        if (NowTime > FallTime)
        {
            Death();
        }
    }

    public override void Death()
    {
        // �v���C���[�̕d�����߂�
        if(PlayerObject != null)
        {
            PlayerObject.GetComponent<PlayerMain>().endSwing = true;
        }

        // �������g������
        Destroy(this.gameObject);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        // �v���C���[���d�ƐڐG
        if (collision.gameObject.tag == "Bullet")
        {
            NowFall = true; // ������
            BulletObject = collision.gameObject;
            BulletMainScript = collision.gameObject.GetComponent<BulletMain>();
        }
        else if (collision.gameObject.tag == "Player")
        {
            // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N���������痎��
            Ray ray_1 = new Ray(collision.gameObject.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray_1, out hit, 1.5f))
            {
                NowFall = true; // ������
                PlayerObject = collision.gameObject;
            }
        }
    }
}
