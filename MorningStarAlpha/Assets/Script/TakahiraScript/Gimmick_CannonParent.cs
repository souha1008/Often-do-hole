using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonParent : Gimmick_Main
{
    [Label("�ł��o���e�I�u�W�F�N�g")]
    public GameObject CannonChild;  // �C�䂩��ł��o���e�I�u�W�F�N�g
    [Label("�����o���v���C���[�Ƃ̋���")]
    public float StartLength = 15;       // �C�䂪�����o���v���C���[�Ƃ̋���
    [Label("�ł��o���Ԋu�̎���")]
    public float ShootTime = 20;         // �ł��o���Ԋu
    

    private bool StartFlag;     // �N���t���O
    private float NowShootTime; // �o�ߎ���

    private GameObject PlayerObject;
    

    public override void Init()
    {
        // ������
        StartFlag = false;
        NowShootTime = 0.0f;

        // �v���C���[�I�u�W�F�N�g�擾
        PlayerObject = GameObject.Find("Player");
    }

    public override void FixedMove()
    {
        // �v���C���[�̍��W
        Vector3 PlayerPos = PlayerObject.transform.position;
        Vector3 ThisPos = this.gameObject.transform.position;


        // �v���C���[�Ƃ̋������m�F
        if (Vector2.Distance(PlayerPos, ThisPos) <= StartLength)
            StartFlag = true;
        else
            StartFlag = false;


        // �N�����̏���
        if (StartFlag)
        {
            // �v���C���[�̕����Ɍ���
            Rad.z = CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos);

            // �e���ˏ���
            if (NowShootTime >= ShootTime)
            {
                if (CannonChild != null)
                {
                    Instantiate(CannonChild, ThisPos, Quaternion.Euler(Rad), this.gameObject.transform); // �e�q�w�肵�Ēe����
                }
                NowShootTime = 0.0f; // �o�ߎ��ԃ��Z�b�g
            }

            NowShootTime += Time.fixedDeltaTime; // �o�ߎ��ԉ��Z
        }
    }

    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }
}
