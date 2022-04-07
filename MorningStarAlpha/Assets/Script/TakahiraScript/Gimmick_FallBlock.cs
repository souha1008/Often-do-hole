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

    private bool PlayerMoveFlag = false;
    private bool BulletMoveFlag = false;

    public override void Init()
    {
        // ������
        NowFall = false;
        NowTime = 0.0f;
        StartPos = this.gameObject.transform.position;
        PlayerMoveFlag = false;
        BulletMoveFlag = false;

        // �R���W����
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // �g���K�[�I�t

        // ���W�b�h�{�f�B
        Rb.isKinematic = true;  // �L�l�}�e�B�b�N�I��
    }

    public override void FixedMove()
    {
        // ���ړ�
        if (NowFall)
        {
            Vector3 OldPos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(StartPos.x, Easing.QuartIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
            NowTime += Time.fixedDeltaTime;

            // �v���C���[�ړ�
            if (PlayerMoveFlag)
            {
                PlayerMain.instance.transform.position +=
                            new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
                //PlayerMainScript.addVel = new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0) / Time.fixedDeltaTime;
            }

            // �d�ړ�
            if (BulletMoveFlag)
            {
                if (PlayerMain.instance.BulletScript.isTouched)
                {
                    PlayerMain.instance.BulletScript.transform.position +=
                        new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
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
        PlayerMain.instance.endSwing = true;

        // �������g������
        Destroy(this.gameObject);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        // �v���C���[���d�ƐڐG
        if (collision.gameObject.CompareTag("Bullet"))
        {
            NowFall = true; // ������
            BulletMoveFlag = true;
        }
        
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("�v���C���[��������");
            // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N���������痎��
            if (PlayerMain.instance.getFootHit().collider != null &&
                PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                NowFall = true; // ������
                PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.getFootHit().collider != null &&
                PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                NowFall = true; // ������
                PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMoveFlag = false;
        }
    }
}
