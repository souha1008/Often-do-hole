using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �M�~�b�N���C���N���X(�p�����Ďg��)
public abstract class Gimmick_Main : MonoBehaviour
{
    [System.Serializable]
    public class NowMoveInfo
    {
        [Header("���݂̊p�x")]
        [Tooltip("0�`360�x�ŕ\�����Ă��܂�")]
        public Vector3 Rad;          // �p�x

        [Header("���݂̈ړ���")]
        public Vector3 Vel;          // �ړ���

        [Header("���v�ړ���")]
        public Vector3 TotalMoveVel; // �����̍��v�ړ���
    }
    [Tooltip("���݂̈ړ����")]
    public NowMoveInfo MoveInfo;

    // �ϐ�
    protected Vector3 Rad;          // �p�x
    protected Vector3 Vel;          // �ړ���
    protected Vector3 TotalMoveVel; // �����̍��v�ړ���

    protected Rigidbody Rb;         // ���W�b�h�{�f�B

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
        Rad = this.gameObject.transform.rotation.eulerAngles;
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
        this.GetComponent<Collider>().isTrigger = true; // �g���K�[�I��

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
        TotalMoveVel += Vel;    // ���v�ړ��ʕύX
        Rb.velocity = Vel;      // �ړ��ʕύX

        // 0�`360�x�ɕύX
        if (Rad.x > 360 || Rad.x < 0 ||
            Rad.y > 360 || Rad.y < 0 ||
            Rad.z > 360 || Rad.z < 0)
        {
            if (Rad.x > 360) Rad.x -= 360;
            if (Rad.x < 0) Rad.x += 360;
            if (Rad.y > 360) Rad.y -= 360;
            if (Rad.y < 0) Rad.y += 360;
            if (Rad.z > 360) Rad.z -= 360;
            if (Rad.z < 0) Rad.z += 360;
        }
        Rb.rotation = Quaternion.Euler(Rad);    // �p�x�ύX

        MoveInfo.Rad = Rad;
        MoveInfo.Vel = Vel;
        MoveInfo.TotalMoveVel = TotalMoveVel;
    }
}
