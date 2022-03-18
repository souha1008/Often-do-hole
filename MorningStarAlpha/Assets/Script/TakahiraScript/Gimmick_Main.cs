using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �M�~�b�N���C���N���X(�p�����Ďg��)
public abstract class Gimmick_Main : MonoBehaviour
{
    // �ϐ�
    [SerializeField] protected Vector3 Rad;          // �p�x
    [SerializeField] protected Vector3 Vel;          // �ړ���
    [SerializeField] protected Vector3 TotalMoveVel; // �����̍��v�ړ���
    protected Rigidbody Rb;         // ���W�b�h�{�f�B

    // �p���������
    public abstract void Init(); // �X�^�[�g����
    public abstract void Move();  // �M�~�b�N�̓�������
    public abstract void Death(); // �M�~�b�N���S����
    public abstract void OnTriggerEnter(Collider collider);    // �����ƏՓˏ���(�g���K�[)
    //public abstract void OnCollisionEnter(Collision collision);   // �����ƏՓˏ���(�R���W����)


    // �p����Ŏ����œ�������(�v���e�N�g)
    protected void Start() 
    {
        // ������
        Rad = this.gameObject.transform.rotation.eulerAngles;
        Vel = new Vector3(0, 0, 0);
        TotalMoveVel = new Vector3(0, 0, 0);
        Rb = null;

        // ���W�b�h�{�f�B
        if ((Rb = this.GetComponent<Rigidbody>()) == null)
        {
            Rb = this.gameObject.AddComponent<Rigidbody>();
        }
        Rb.isKinematic = false; // �L�l�}�e�B�b�N�I�t
        Rb.useGravity = false;  // �d�̓I�t

        // �R���C�_�[
        this.GetComponent<Collider>().isTrigger = true; // �g���K�[�I��

        // �X�^�[�g����
        Init();
    }
    protected void Update()
    {

    }
    protected void FixedUpdate() 
    {
        Move();                 // �M�~�b�N�̓�������
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
    }
}
