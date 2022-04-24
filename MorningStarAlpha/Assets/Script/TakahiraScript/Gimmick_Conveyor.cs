using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    [EnumPName("��]����", typeof(MOVE_DIRECTION_X))]
    public MOVE_DIRECTION_X MoveDirection_X;   // ��]����
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
        transform.rotation = Quaternion.identity;
    }

    public override void FixedMove()
    {
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X); // ��]�����X�V

        // �R���x�A�̓�������
        conveyorState.Move();
        //Debug.LogWarning(conveyorState); // ���݂̃R���x�A�̃X�e�[�g
    }


    public override void Death()
    {

    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.getFootHit().collider != null &&
                 PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                ConveyorState.PlayerMoveFlag = true;
            }
        }

        //if (collision.gameObject.CompareTag("Bullet"))
        //{
        //    ConveyorState.BulletMoveFlag = true;
        //    // �Փ˓_�擾
        //    foreach (ContactPoint contact in collision.contacts)
        //    {
        //        // �q�b�g�����ʎ擾
        //        if (contact.normal.y == 0)
        //        {
        //            if (contact.normal.x < 0) // �E
        //                ConveyorState.TouchSide = TOUCH_SIDE.RIGHT;
        //            else                        // ��
        //                ConveyorState.TouchSide = TOUCH_SIDE.LEFT;
        //        }
        //        else
        //        {
        //            if (contact.normal.y < 0) // ��
        //                ConveyorState.TouchSide = TOUCH_SIDE.UP;
        //            else                        // ��
        //                ConveyorState.TouchSide = TOUCH_SIDE.DOWN;
        //        }

        //        //Debug.Log(contact.normal);
        //        //Debug.Log(conveyorState.TouchSide);

        //        //============
        //        // �����p����
        //        //============
        //        //print(contact.thisCollider.name + " hit " + contact.otherCollider.name); // �������@hit �����
        //        //Debug.Log(contact.normal); // �@��
        //        //Debug.DrawRay(contact.point, contact.normal, Color.white); // ���������_�Ń��C������
        //    }
        //}
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.getFootHit().collider != null &&
                 PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                ConveyorState.PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ConveyorState.PlayerMoveFlag = false;
        }
        //if (collision.gameObject.CompareTag("Bullet"))
        //{
        //    ConveyorState.BulletMoveFlag = false;
        //}
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

    static public TOUCH_SIDE TouchSide;
    static public Gimmick_Conveyor Conveyor;
    static public bool PlayerMoveFlag;
    static public bool BulletMoveFlag;
}

// �X�^�[�g����(��������n�߂�)
public class ConveyorStart : ConveyorState
{
    public ConveyorStart(Gimmick_Conveyor conveyor) // �R���X�g���N�^
    {
        // ������
        Conveyor = conveyor;
        TouchSide = TOUCH_SIDE.NONE;
        PlayerMoveFlag = false;
        BulletMoveFlag = false;
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
        TouchSide = TOUCH_SIDE.NONE;
        PlayerMoveFlag = false;
        BulletMoveFlag = false;
        PlayerMain.instance.floorVel = Vector3.zero;
    }

    public override void Move()
    {
        if (PlayerMoveFlag)
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
        if (BulletMoveFlag)
        {
            switch (TouchSide)
            {
                case TOUCH_SIDE.NONE:
                    break;
                case TOUCH_SIDE.UP:
                    //if (Conveyor.MoveRight) // �E��]�Ȃ�E�ړ�
                    //{
                    //    StateChange(new ConveyorRight());
                    //}
                    //else // ����]�Ȃ獶�ړ�
                    //{
                    //    StateChange(new ConveyorLeft());
                    //}
                    PlayerMain.instance.ForciblyReleaseMode(true);
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
                    //if (Conveyor.MoveRight)
                    //{
                    //    StateChange(new ConveyorDown());
                    //}
                    //else
                    //{
                    //    StateChange(new ConveyorUp());
                    //}
                    PlayerMain.instance.ForciblyReleaseMode(true);
                    break;
                case TOUCH_SIDE.LEFT:
                    //if (Conveyor.MoveRight)
                    //{
                    //    StateChange(new ConveyorUp());
                    //}
                    //else
                    //{
                    //    StateChange(new ConveyorDown());
                    //}
                    PlayerMain.instance.ForciblyReleaseMode(true);
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
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            PlayerMain.instance.BulletScript.transform.position += new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);
            PlayerMain.instance.floorVel = new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0) * 1 / Time.fixedDeltaTime;

            if (!BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);
                if (Conveyor.MoveRight)
                    StateChange(new ConveyorRight());
                else
                    StateChange(new ConveyorLeft());
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
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            PlayerMain.instance.BulletScript.transform.position += new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
            PlayerMain.instance.floorVel = new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0) * 1 / Time.fixedDeltaTime;

            if (!BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
                if (Conveyor.MoveRight)
                    StateChange(new ConveyorLeft());
                else
                    StateChange(new ConveyorRight());
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
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            //Debug.Log("�d�E�ړ���");
            PlayerMain.instance.BulletScript.transform.position += new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
            PlayerMain.instance.floorVel = new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.x > Conveyor.transform.position.x + Conveyor.transform.lossyScale.x * 0.5f)
                BulletMoveFlag = false;

            if (!BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
                //if (Conveyor.MoveRight)
                //    StateChange(new ConveyorDown());
                //else
                //    StateChange(new ConveyorUp());
                StateChange(new ConveyorNone());
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
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            //Debug.LogWarning("�d���ړ���");
            PlayerMain.instance.BulletScript.transform.position += new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
            PlayerMain.instance.floorVel = new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.x < Conveyor.transform.position.x - Conveyor.transform.lossyScale.x * 0.5f)
                BulletMoveFlag = false;

            if (!BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
                //if (Conveyor.MoveRight)
                //    StateChange(new ConveyorUp());
                //else
                //    StateChange(new ConveyorDown());
                StateChange(new ConveyorNone());
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
        if(PlayerMoveFlag)
        {
            PlayerMain.instance.floorVel = new Vector3(Conveyor.MovePower, 0, 0);
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
        if (PlayerMoveFlag)
        {
            PlayerMain.instance.floorVel = new Vector3(Conveyor.MovePower * -1, 0, 0);
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}
