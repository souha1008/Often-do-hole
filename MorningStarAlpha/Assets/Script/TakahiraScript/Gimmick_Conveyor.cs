using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    public MOVE_DIRECTION_X MoveDirection_X ;   // �ړ�����
    public float MovePower = 30;                // �ړ���


    private bool MoveRight; // �ړ�����
    private float Fugou;    // ����
    private GameObject Player;
    private PlayerMain PlayerMainScript;
    private GameObject Bullet;
    private BulletMain BulletMainScript;


    public override void Init()
    {
        // ������
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X);
        Fugou = CalculationScript.FugouChange(MoveRight);

        // �v���C���[�I�u�W�F�N�g�擾
        Player = GameObject.Find("Player");
        PlayerMainScript = Player.GetComponent<PlayerMain>();

        // �d�I�u�W�F�N�gnull
        Bullet = null;
        BulletMainScript = null;

        // ���W�b�h�{�f�B
        Rb.isKinematic = true;

        // �R���W����
        Cd.isTrigger = false; // �g���K�[�I�t

        // �p�x��0�x�Œ�
        Rad = Vector3.zero;
    }

    public override void FixedMove()
    {
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X);
        Fugou = CalculationScript.FugouChange(MoveRight);

        // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N����������v���C���[�̈ړ�
        if (Player != null)
        {
            Ray ray = new Ray(Player.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    //Debug.Log("�v���C���[�u���b�N�̏�ړ���");
                    PlayerMainScript.addVel = new Vector3(MovePower * -Fugou, 0, 0);
                }
            }
        }

        if (Bullet != null)
        {
            if(BulletMainScript.isTouched == true)
            {
                BulletMainScript.transform.position =
                    BulletMainScript.transform.position + new Vector3(MovePower * Time.fixedDeltaTime * Fugou, 0, 0);
            }
            else
            {
                Bullet = null;
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
            Player = collision.gameObject;
        }
        if (collision.gameObject.tag == "Bullet")
        {
            Bullet = collision.gameObject;
            BulletMainScript = Bullet.GetComponent<BulletMain>();
        }
    }

    private bool MoveDirectionBoolChangeX(MOVE_DIRECTION_X MoveDirection_X)
    {
        if (MoveDirection_X == MOVE_DIRECTION_X.MoveRight)
            return true;
        else
            return false;
    }

    private MOVE_DIRECTION_X BoolMoveDirectionChangeX(bool MoveRight)
    {
        if (MoveRight)
            return MOVE_DIRECTION_X.MoveRight;
        else
            return MOVE_DIRECTION_X.MoveLeft;
    }
}
