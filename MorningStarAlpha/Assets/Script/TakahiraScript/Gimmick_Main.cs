using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �M�~�b�N���C���N���X(�p�����Ďg��)
public abstract class Gimmick_Main : MonoBehaviour
{
    [System.Serializable]
    public class NowMoveInfo
    {
        [SerializeField, Label("���݂̈ړ���")]
        public Vector3 Vel;          // �ړ���

        //[SerializeField, Label("���v�ړ���")]
        //public Vector3 TotalMoveVel; // �����̍��v�ړ���
    }
    [Label("���݂̈ړ����")]
    public NowMoveInfo MoveInfo;


    // �ϐ�
    protected Vector3 Vel;          // �ړ���
    protected Vector3 TotalMoveVel; // �����̍��v�ړ���

    protected Rigidbody Rb;         // ���W�b�h�{�f�B
    protected Collider Cd;          // �R���C�_�[

    // �p���������
    public abstract void Init(); // �X�^�[�g����
    public virtual void FixedMove() { }  // �M�~�b�N�̓�������
    public virtual void UpdateMove() { }   // Update���g������������
    public abstract void Death(); // �M�~�b�N���S����
    public virtual void OnTriggerEnter(Collider collider) { }    // �����ƏՓˏ���(�g���K�[)
    public virtual void OnCollisionEnter(Collision collision) { }   // �����ƏՓˏ���(�R���W����)


    // �p����Ŏ����œ�������(�v���e�N�g)
    protected void Start() 
    {
        // ������
        Vel = Vector3.zero;
        TotalMoveVel = Vector3.zero;
        Rb = null;

        // ���W�b�h�{�f�B
        if ((Rb = this.GetComponent<Rigidbody>()) == null)
        {
            Rb = this.gameObject.AddComponent<Rigidbody>();
        }
        Rb.isKinematic = false; // �L�l�}�e�B�b�N�I�t
        Rb.useGravity = false;  // �d�̓I�t
        Rb.constraints = RigidbodyConstraints.None;             // �t���[�Y��S������
        Rb.constraints = RigidbodyConstraints.FreezeRotation;   // ��]�̃t���[�Y��S���I��


        // �R���C�_�[
        Cd = this.GetComponent<Collider>(); // �R���C�_�[�擾
        Cd.isTrigger = true;    // �g���K�[�I��

        // �X�^�[�g����
        Init();
    }
    protected void Update()
    {
        UpdateMove();
    }
    protected void FixedUpdate() 
    {
        FixedMove();                 // �M�~�b�N�̓�������
        //TotalMoveVel += Vel;    // ���v�ړ��ʕύX
        Rb.velocity = Vel;      // �ړ��ʕύX


        MoveInfo.Vel = Vel;
        //MoveInfo.TotalMoveVel = TotalMoveVel;
    }
}
