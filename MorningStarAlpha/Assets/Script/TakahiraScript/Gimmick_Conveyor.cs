using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    [EnumPName("��]����", typeof(MOVE_DIRECTION_X))]
    public MOVE_DIRECTION_X MoveDirection_X ;   // ��]����
    [Label("�ړ���")]
    public float MovePower = 30;                // �ړ���


    [HideInInspector] public ConveyorState conveyorState;
    [HideInInspector] public bool MoveRight;    // ��]����


    public override void Init()
    {
        // �R���x�A�X�e�[�g������
        conveyorState = new ConveyorStart(this);

        // ��]�����X�V
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X);

        // ���W�b�h�{�f�B
        Rb.isKinematic = true;

        // �R���W����
        Cd.isTrigger = false; // �g���K�[�I�t

        // �p�x��0�x�Œ�
        Rad = Vector3.zero;
    }

    public override void FixedMove()
    {
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X); // ��]�����X�V

        // �R���x�A�̓�������
        conveyorState.Move();
        //Debug.Log(conveyorState); // ���݂̃R���x�A�̃X�e�[�g
    }


    public override void Death()
    {
        
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // �v���C���[�I�u�W�F�N�g�擾
            ConveyorState.Player = collision.gameObject;
            ConveyorState.PlayerMainScript = collision.gameObject.GetComponent<PlayerMain>();
        }
        if (collision.gameObject.tag == "Bullet")
        {
            // �Փ˓_�擾
            foreach (ContactPoint contact in collision.contacts)
            {
                // �q�b�g�����ʎ擾
                if (contact.normal.y == 0)
                {
                    if (contact.normal.x < 0) // �E
                        conveyorState.TouchSide = TOUCH_SIDE.RIGHT;
                    else                        // ��
                        conveyorState.TouchSide = TOUCH_SIDE.LEFT;
                }
                else
                {
                    if (contact.normal.y < 0) // ��
                        conveyorState.TouchSide = TOUCH_SIDE.UP;
                    else                        // ��
                        conveyorState.TouchSide = TOUCH_SIDE.DOWN;
                }

                //Debug.Log(contact.normal);
                //Debug.Log(conveyorState.TouchSide);

                //============
                // �����p����
                //============
                //print(contact.thisCollider.name + " hit " + contact.otherCollider.name); // �������@hit �����
                //Debug.Log(contact.normal); // �@��
                //Debug.DrawRay(contact.point, contact.normal, Color.white); // ���������_�Ń��C������
            }

            // �d�I�u�W�F�N�g�擾
            ConveyorState.Bullet = collision.gameObject;
            ConveyorState.BulletMainScript = collision.gameObject.GetComponent<BulletMain>();
        }
    }

    public bool MoveDirectionBoolChangeX(MOVE_DIRECTION_X MoveDirection_X)
    {
        if (MoveDirection_X == MOVE_DIRECTION_X.MoveRight)
            return true;
        else
            return false;
    }

    public MOVE_DIRECTION_X BoolMoveDirectionChangeX(bool MoveRight)
    {
        if (MoveRight)
            return MOVE_DIRECTION_X.MoveRight;
        else
            return MOVE_DIRECTION_X.MoveLeft;
    }
}

public enum TOUCH_SIDE
{
    NONE = 0,
    RIGHT,
    LEFT,
    UP,
    DOWN
}

// �R���x�A�̃X�e�[�g
public abstract class ConveyorState
{
    public virtual void Move() { } // �R���x�A�̓����ƃX�e�[�g�ڍs����
    public void StateChange(ConveyorState state) // �X�e�[�g�ڍs
    {
        Conveyor.conveyorState = state;
    }

    public TOUCH_SIDE TouchSide;
    static public Gimmick_Conveyor Conveyor;
    static public GameObject Player;
    static public PlayerMain PlayerMainScript;
    static public GameObject Bullet;
    static public BulletMain BulletMainScript;
}

// �X�^�[�g����(��������n�߂�)
public class ConveyorStart : ConveyorState
{
    public ConveyorStart(Gimmick_Conveyor conveyor) // �R���X�g���N�^
    {
        // ������
        Conveyor = conveyor;
        TouchSide = TOUCH_SIDE.NONE;

        // �v���C���[�I�u�W�F�N�gnull
        Player = null;
        PlayerMainScript = null;

        // �d�I�u�W�F�N�gnull
        Bullet = null;
        BulletMainScript = null;
    }

    public override void Move()
    {
        StateChange(new ConveyorNone());
    }
}

// �������Ȃ�
public class ConveyorNone : ConveyorState
{
    public ConveyorNone() // �R���X�g���N�^
    {
        if (Bullet != null && BulletMainScript != null)
        {
            BulletMainScript.isTouched = false;
        }

        TouchSide = TOUCH_SIDE.NONE;
    }

    public override void Move()
    {
        if (Player != null && PlayerMainScript != null)
        {
            // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N����������v���C���[�I�u�W�F�N�g�擾
            Ray ray_1 = new Ray(Player.gameObject.transform.position + new Vector3(Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
            Ray ray_2 = new Ray(Player.gameObject.transform.position + new Vector3(-Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
            RaycastHit hit;
            float RayLength = Player.gameObject.transform.localScale.y * 0.5f + 1.0f;
            if (Physics.Raycast(ray_1, out hit, RayLength) || Physics.Raycast(ray_2, out hit, RayLength))
            {
                if (hit.collider.gameObject == Conveyor.gameObject)
                {
                    if (Conveyor.MoveRight)
                    {
                        StateChange(new ConveyorPlayerMoveRight()); // �v���C���[�E�ړ�
                    }
                    else
                    {
                        StateChange(new ConveyorPlayerMoveLeft()); // �v���C���[���ړ�
                    }
                }
            }
            else // ���C��������Ȃ�������null�����
            {
                Player = null;
                PlayerMainScript = null;
            }
        }

        if (Bullet != null && BulletMainScript != null)
        {
            switch (TouchSide)
            {
                case TOUCH_SIDE.NONE:
                    break;
                case TOUCH_SIDE.UP:
                    if (Conveyor.MoveRight) // �E��]�Ȃ�E�ړ�
                    {
                        StateChange(new ConveyorRight());
                    }
                    else // ����]�Ȃ獶�ړ�
                    {
                        StateChange(new ConveyorLeft());
                    }
                    break;
                case TOUCH_SIDE.DOWN:
                    if (Conveyor.MoveRight) // �E��]�Ȃ獶�ړ�
                    {
                        StateChange(new ConveyorLeft());
                    }
                    else // ����]�Ȃ�E�ړ�
                    {
                        StateChange(new ConveyorRight());
                    }
                    break;
                case TOUCH_SIDE.RIGHT:
                    if (Conveyor.MoveRight)
                    {
                        StateChange(new ConveyorDown());
                    }
                    else
                    {
                        StateChange(new ConveyorUp());
                    }
                    break;
                case TOUCH_SIDE.LEFT:
                    if (Conveyor.MoveRight)
                    {
                        StateChange(new ConveyorUp());
                    }
                    else
                    {
                        StateChange(new ConveyorDown());
                    }
                    break;
            }
        }
    }
}

// �d�I�u�W�F�N�g�����
public class ConveyorUp : ConveyorState
{
    public override void Move()
    {
        if (BulletMainScript.isTouched == true)
        {
            BulletMainScript.transform.position += new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);

            // �d���烌�C��΂��Đ^���Ƀu���b�N������������X�e�[�g�ύX
            if (Conveyor.MoveRight)
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(0, -Bullet.transform.localScale.y * 0.5f, 0), Vector3.right);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(0, Bullet.transform.localScale.y * 0.5f, 0), Vector3.right);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.x * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);
                    StateChange(new ConveyorRight());
                }
            }
            else
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(0, -Bullet.transform.localScale.y * 0.5f, 0), Vector3.left);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(0, Bullet.transform.localScale.y * 0.5f, 0), Vector3.left);
                float RayLength = BulletMainScript.gameObject.transform.localScale.x * 0.5f + 1.0f;
                RaycastHit hit;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);
                    StateChange(new ConveyorLeft());
                }
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}

// �d�I�u�W�F�N�g������
public class ConveyorDown : ConveyorState
{
    public override void Move()
    {
        if (BulletMainScript.isTouched == true)
        {
            BulletMainScript.transform.position += new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);

            // �d���烌�C��΂��Đ^���Ƀu���b�N������������X�e�[�g�ύX
            if (Conveyor.MoveRight)
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(0, -Bullet.transform.localScale.y * 0.5f, 0), Vector3.left);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(0, Bullet.transform.localScale.y * 0.5f, 0), Vector3.left);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.x * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
                    StateChange(new ConveyorLeft());
                }
            }
            else
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(0, -Bullet.transform.localScale.y * 0.5f, 0), Vector3.right);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(0, Bullet.transform.localScale.y * 0.5f, 0), Vector3.right);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.x * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
                    StateChange(new ConveyorRight());
                }
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}

// �d�I�u�W�F�N�g�E����
public class ConveyorRight : ConveyorState
{
    public override void Move()
    {
        if (BulletMainScript.isTouched == true)
        {
            //Debug.Log("�d�E�ړ���");
            BulletMainScript.transform.position += new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);

            // �d���烌�C��΂��Đ^��Ƀu���b�N������������X�e�[�g�ύX
            if (Conveyor.MoveRight)
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(-Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.y * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
                    StateChange(new ConveyorDown());
                }
            }
            else
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(-Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.up);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.up);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.y * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
                    StateChange(new ConveyorUp());
                }
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}

// �d�I�u�W�F�N�g������
public class ConveyorLeft : ConveyorState
{
    public override void Move()
    {
        if(BulletMainScript.isTouched == true)
        {
            //Debug.Log("�d���ړ���");
            BulletMainScript.transform.position += new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);

            // �d���烌�C��΂��Đ^��Ƀu���b�N������������X�e�[�g�ύX
            if (Conveyor.MoveRight)
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(-Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.up);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.up);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.y * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {              
                    BulletMainScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
                    StateChange(new ConveyorUp());
                }
            }
            else
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(-Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.y * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
                    StateChange(new ConveyorDown());
                }
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}

// �v���C���[�I�u�W�F�N�g�E�ړ�
public class ConveyorPlayerMoveRight : ConveyorState
{
    public override void Move()
    {
        // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N������������X�e�[�g�ύX
        Ray ray_1 = new Ray(Player.gameObject.transform.position + new Vector3(Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
        Ray ray_2 = new Ray(Player.gameObject.transform.position + new Vector3(-Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
        RaycastHit hit;
        float RayLength = Player.gameObject.transform.localScale.y * 0.5f + 1.0f;
        if (Physics.Raycast(ray_1, out hit, RayLength) || Physics.Raycast(ray_2, out hit, RayLength))
        {
            if (hit.collider.gameObject == Conveyor.gameObject)
            {
                //Debug.Log("�v���C���[�E�Ɉړ���");
                PlayerMainScript.addVel = new Vector3(Conveyor.MovePower, 0, 0);
            }
            else
            {
                StateChange(new ConveyorNone());
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}


// �v���C���[�I�u�W�F�N�g���ړ�
public class ConveyorPlayerMoveLeft : ConveyorState
{
    public override void Move()
    {
        // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N������������X�e�[�g�ύX
        Ray ray_1 = new Ray(Player.gameObject.transform.position + new Vector3(Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
        Ray ray_2 = new Ray(Player.gameObject.transform.position + new Vector3(-Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
        RaycastHit hit;
        float RayLength = Player.gameObject.transform.localScale.y * 0.5f + 1.0f;
        if (Physics.Raycast(ray_1, out hit, RayLength) || Physics.Raycast(ray_2, out hit, RayLength))
        {
            if (hit.collider.gameObject == Conveyor.gameObject)
            {
                //Debug.Log("�v���C���[���Ɉړ���");
                PlayerMainScript.addVel = new Vector3(Conveyor.MovePower * -1, 0, 0);
            }
            else
            {
                StateChange(new ConveyorNone());
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}
