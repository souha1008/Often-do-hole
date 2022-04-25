using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    [EnumPName("��]����", typeof(MOVE_DIRECTION_X))]
    public MOVE_DIRECTION_X MoveDirection_X;   // ��]����
    [Label("�ړ���")]
    public float MovePower = 30;                // �ړ���


    [HideInInspector] public ConveyorStateMain conveyorState;
    [HideInInspector] public bool MoveRight;    // ��]����


    public override void Init()
    {
        // �R���x�A�X�e�[�g������
        conveyorState = new ConveyorStateMain(this);

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
                conveyorState.PlayerMoveFlag = true;
            }
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            conveyorState.BulletMoveFlag = true;
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
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.getFootHit().collider != null &&
                 PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                conveyorState.PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            conveyorState.PlayerMoveFlag = false;
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

public class ConveyorStateMain
{
    public ConveyorStateMain(Gimmick_Conveyor conveyor)
    {
        Conveyor = conveyor;
        ConveyorState = new ConveyorNone(this);
        TouchSide = TOUCH_SIDE.NONE;
        PlayerMoveFlag = false;
        BulletMoveFlag = false;
    }

    // �R���x�A�̓����ƃX�e�[�g�ڍs����
    public void Move() 
    {
        ConveyorState.Move();
    }

    // �X�e�[�g�ڍs
    public void StateChange(ConveyorState State)
    {
        ConveyorState = State;
    }

    public Gimmick_Conveyor Conveyor;
    public ConveyorState ConveyorState;
    public TOUCH_SIDE TouchSide;
    public bool PlayerMoveFlag;
    public bool BulletMoveFlag;
}

// �R���x�A�̃X�e�[�g
public abstract class ConveyorState
{
    public ConveyorState(ConveyorStateMain conveyorMain)
    {
        ConveyorMain = conveyorMain;
    }
    public virtual void Move() { } // �R���x�A�̓����ƃX�e�[�g�ڍs����

    public ConveyorStateMain ConveyorMain;
}

// �������Ȃ�
public class ConveyorNone : ConveyorState
{
    public ConveyorNone(ConveyorStateMain conveyorMain) : base (conveyorMain)
    {
        ConveyorMain.TouchSide = TOUCH_SIDE.NONE;
        ConveyorMain.PlayerMoveFlag = false;
        ConveyorMain.BulletMoveFlag = false;
        PlayerMain.instance.floorVel = Vector3.zero;
    }
    public override void Move()
    {
        Debug.LogWarning(ConveyorMain.Conveyor);
        if (ConveyorMain.PlayerMoveFlag)
        {
            if (ConveyorMain.Conveyor.MoveRight)
            {
                ConveyorMain.StateChange(new ConveyorPlayerMoveRight(ConveyorMain)); // �v���C���[�E�ړ�
            }
            else
            {
                ConveyorMain.StateChange(new ConveyorPlayerMoveLeft(ConveyorMain)); // �v���C���[���ړ�
            }
        }
        if (ConveyorMain.BulletMoveFlag)
        {
            switch (ConveyorMain.TouchSide)
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
                    //PlayerMain.instance.ForciblyReleaseMode(true);
                    ConveyorMain.BulletMoveFlag = false;
                    break;
                case TOUCH_SIDE.DOWN:
                    if (ConveyorMain.Conveyor.MoveRight) // �E��]�Ȃ獶�ړ�
                    {
                        ConveyorMain.StateChange(new ConveyorLeft(ConveyorMain));
                    }
                    else // ����]�Ȃ�E�ړ�
                    {
                        ConveyorMain.StateChange(new ConveyorRight(ConveyorMain));
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
                    //PlayerMain.instance.ForciblyReleaseMode(true);
                    ConveyorMain.BulletMoveFlag = false;
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
                    //PlayerMain.instance.ForciblyReleaseMode(true);
                    ConveyorMain.BulletMoveFlag = false;
                    break;
            }
        }
    }
}

// �d�I�u�W�F�N�g�����
public class ConveyorUp : ConveyorState
{
    public ConveyorUp(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            PlayerMain.instance.BulletScript.transform.position += new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0);
            PlayerMain.instance.floorVel = new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0) * 1 / Time.fixedDeltaTime;

            if (!ConveyorMain.BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0);
                if (ConveyorMain.Conveyor.MoveRight)
                    ConveyorMain.StateChange(new ConveyorRight(ConveyorMain));
                else
                    ConveyorMain.StateChange(new ConveyorLeft(ConveyorMain));
            }
        }
        else
        {
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain.Conveyor.conveyorState));
        }
    }
}

// �d�I�u�W�F�N�g������
public class ConveyorDown : ConveyorState
{
    public ConveyorDown(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            PlayerMain.instance.BulletScript.transform.position += new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
            PlayerMain.instance.floorVel = new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0) * 1 / Time.fixedDeltaTime;

            if (!ConveyorMain.BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
                if (ConveyorMain.Conveyor.MoveRight)
                    ConveyorMain.StateChange(new ConveyorLeft(ConveyorMain));
                else
                    ConveyorMain.StateChange(new ConveyorRight(ConveyorMain));
            }
        }
        else
        {
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}

// �d�I�u�W�F�N�g�E����
public class ConveyorRight : ConveyorState
{
    public ConveyorRight(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            //Debug.Log("�d�E�ړ���");
            PlayerMain.instance.BulletScript.transform.position += new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
            PlayerMain.instance.floorVel = new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.x > ConveyorMain.Conveyor.transform.position.x + ConveyorMain.Conveyor.transform.lossyScale.x * 0.5f)
                ConveyorMain.BulletMoveFlag = false;

            if (!ConveyorMain.BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
                //if (Conveyor.MoveRight)
                //    StateChange(new ConveyorDown());
                //else
                //    StateChange(new ConveyorUp());
                ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
            }
        }
        else
        {
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}

// �d�I�u�W�F�N�g������
public class ConveyorLeft : ConveyorState
{
    public ConveyorLeft(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            //Debug.LogWarning("�d���ړ���");
            PlayerMain.instance.BulletScript.transform.position += new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
            PlayerMain.instance.floorVel = new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.x < ConveyorMain.Conveyor.transform.position.x - ConveyorMain.Conveyor.transform.lossyScale.x * 0.5f)
                ConveyorMain.BulletMoveFlag = false;

            if (!ConveyorMain.BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
                //if (Conveyor.MoveRight)
                //    StateChange(new ConveyorUp());
                //else
                //    StateChange(new ConveyorDown());
                ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
            }
        }
        else
        {
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}

// �v���C���[�I�u�W�F�N�g�E�ړ�
public class ConveyorPlayerMoveRight : ConveyorState
{
    public ConveyorPlayerMoveRight(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if(ConveyorMain.PlayerMoveFlag)
        {
            PlayerMain.instance.floorVel = new Vector3(ConveyorMain.Conveyor.MovePower, 0, 0);
        }
        else
        {
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}


// �v���C���[�I�u�W�F�N�g���ړ�
public class ConveyorPlayerMoveLeft : ConveyorState
{
    public ConveyorPlayerMoveLeft(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (ConveyorMain.PlayerMoveFlag)
        {
            PlayerMain.instance.floorVel = new Vector3(ConveyorMain.Conveyor.MovePower * -1, 0, 0);
        }
        else
        {
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}
