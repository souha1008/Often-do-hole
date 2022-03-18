using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock : Gimmick_Main
{
    public float FallPower = 0.1f;     // �������
    private bool NowFall;               // ��������

    public override void Init()
    {
        // ������
        NowFall = false;

        // �R���W����
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // �g���K�[�I�t

        // ���W�b�h�{�f�B
        Rb.mass = 100000.0f; // �d�����ē����Ȃ��悤�ɂ���
    }

    public override void Move()
    {
        // ����ŃC�[�W���O�𗘗p���������ɕύX�\��
        if (NowFall)
        {
            Vel.y += -FallPower;
        }
        if (TotalMoveVel.y <= -500.0f)
        {
            Death();
        }
    }

    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        // �v���C���[���d�ƐڐG
        if (collision.gameObject.tag == "Bullet")
        {
            NowFall = true; // ������
        }
        else if (collision.gameObject.tag == "Player")
        {
            // �v���C���[���烌�C��΂��Đ^���Ƀu���b�N���������痎��
            Ray ray_1 = new Ray(collision.gameObject.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray_1, out hit, 1.5f))
            {
                NowFall = true; // ������
            }
        }
    }

    // �v���C���[�̗����łۂނۂނ���̒����p
    //public void OnCollisionStay(Collision collision)
    //{
    //    // �v���C���[�ƐڐG��
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        // �v���C���[�̉E�ƍ��Ƀ��C���㉺2�{����΂�
    //        Ray ray_1 = new Ray(collision.gameObject.transform.position + new Vector3(0, 0.5f, 0), Vector3.left);
    //        Ray ray_2 = new Ray(collision.gameObject.transform.position + new Vector3(0, -0.5f, 0), Vector3.left);
    //        Ray ray_3 = new Ray(collision.gameObject.transform.position + new Vector3(0, 0.5f, 0), Vector3.right);
    //        Ray ray_4 = new Ray(collision.gameObject.transform.position + new Vector3(0, -0.5f, 0), Vector3.right);

    //        RaycastHit hit;
    //        if (Physics.Raycast(ray_1, out hit, 1.5f) || Physics.Raycast(ray_2, out hit, 1.5f) ||
    //            Physics.Raycast(ray_3, out hit, 1.5f) || Physics.Raycast(ray_4, out hit, 1.5f))
    //        {
    //            return;
    //        }
    //        // �v���C���[�̑��x�������ɂ���
    //        collision.gameObject.GetComponent<PlayerMain>().vel.y = Vel.y;
    //    }
    //}
}
