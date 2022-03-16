using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Interface : MonoBehaviour
{
    [SerializeField] protected Vector3 Vel;          // �ړ���
    [SerializeField] protected Vector3 TotalMoveVel; // �����̍��v�ړ���
    protected Rigidbody Rb;         // ���W�b�h�{�f�B


    virtual public void Init() { } // �X�^�[�g����
    virtual public void Move() { }  // �G�̓�������
    virtual public void Death() { } // �G���S����
    virtual public void OnTriggerEnter(Collider collider) { }    // �����ƏՓ˔���(�g���K�[)
    //virtual public void OnCollisionEnter(Collision collision) { }   // �����ƏՓ˔���(�R���W����)

    protected void Update()
    {
        
    }

    protected void Start() 
    {
        // ������
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
    protected void FixedUpdate() 
    {
        Move();                 // �G�̓�������
        TotalMoveVel += Vel;    // ���v�ړ��ʕύX
        Rb.velocity = Vel;      // �ړ��ʕύX
    }
}
