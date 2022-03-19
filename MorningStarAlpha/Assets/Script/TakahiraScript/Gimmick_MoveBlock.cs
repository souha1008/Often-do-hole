using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_MoveBlock : Gimmick_Main
{
    public enum MOVE_DIRECTION
    {
        MoveRight,
        MoveLeft
    }
    // �ϐ�
    [Header("�ړ�����")]
    public float MoveLength = 15.0f;    // ��������

    [Header("���b�����Ĉړ����邩")]
    public float MoveTime = 3.0f;       // ���b�����Ĉړ����邩

    [Header("�ړ�����")]
    public MOVE_DIRECTION MoveDirection = MOVE_DIRECTION.MoveRight; // �ړ�����

    private bool MoveRight;             // �ړ�����(�E)
    private bool NowMove;               // �ړ�����
    private float NowTime;              // �o�ߎ���
    private Vector3 StartPos;           // �������W
    private GameObject PlayerObject;    // �v���C���[�I�u�W�F�N�g
    private float Fugou;

    public override void Init()
    {
        // ������
        NowMove = true;
        NowTime = 0.0f;
        StartPos = this.gameObject.transform.position;
        MoveRight = MoveDirectionBoolChange(MoveDirection);
        Fugou = CalculationScript.FugouChange(MoveRight);

        // �R���W����
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // �g���K�[�I�t

        // ���W�b�h�{�f�B
        Rb.isKinematic = true;
    }

    public override void UpdateMove()
    {
        Vector3 OldPos = this.gameObject.transform.position;
        if (NowMove)
        {
            this.gameObject.transform.position = new Vector3(Easing.QuadInOut(NowTime, MoveTime, StartPos.x, StartPos.x + MoveLength * Fugou), StartPos.y, StartPos.z);
            NowTime += Time.deltaTime;

            // �ړ��I��
            if (NowTime > MoveTime)
                NowMove = false;
        }
        else
        {
            StartPos = this.gameObject.transform.position;
            NowTime = 0.0f;
            MoveRight = CalculationScript.TureFalseChange(MoveRight);   // �������]
            Fugou = CalculationScript.FugouChange(MoveRight);   // �������]
            MoveDirection = BoolMoveDirectionChange(MoveRight); // �����\���ω�
            NowMove = true;
        }

        // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N����������
        if (PlayerObject != null)
        {
            Ray ray = new Ray(PlayerObject.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    //Debug.Log("�v���C���[�u���b�N�̏�ړ���");
                    PlayerObject.gameObject.transform.position =
                        PlayerObject.gameObject.transform.position + new Vector3(this.gameObject.transform.position.x - OldPos.x, 0, 0);
                }
            }
        }
    }

    public override void Death()
    {

    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerObject = collision.gameObject;
        }
    }

    private bool MoveDirectionBoolChange(MOVE_DIRECTION MoveDirection)
    {
        if (MoveDirection == MOVE_DIRECTION.MoveRight)
            return true;
        else
            return false;
    }

    private MOVE_DIRECTION BoolMoveDirectionChange(bool MoveRight)
    {
        if (MoveRight)
            return MOVE_DIRECTION.MoveRight;
        else
            return MOVE_DIRECTION.MoveLeft;
    }
}
