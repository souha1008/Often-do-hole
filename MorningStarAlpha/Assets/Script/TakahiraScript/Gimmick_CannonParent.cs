using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonParent : Gimmick_Main
{
    [Header("[�C��̐ݒ�]")]
    [Label("�ł��o���e�I�u�W�F�N�g")]
    public GameObject CannonChild;  // �C�䂩��ł��o���e�I�u�W�F�N�g
    [Label("�����o���v���C���[�Ƃ̋���")]
    public float StartLength = 15;       // �C�䂪�����o���v���C���[�Ƃ̋���
    [Label("�ł��o���Ԋu�̎���")]
    public float ShootTime = 5;         // �ł��o���Ԋu

    [Header("[�e�̐ݒ�]")]
    [Label("�e�̑��x")]
    public float Speed = 5;        // �e�̑��x
    [Label("�e�̐�������")]
    public float LifeTime = 5;     // �e�̐�������



    private bool StartFlag;     // �N���t���O
    private float NowShootTime; // �o�ߎ���

    [HideInInspector] public GameObject PlayerObject; // �v���C���[�I�u�W�F�N�g
    

    public override void Init()
    {
        // ������
        StartFlag = false;
        NowShootTime = ShootTime; // �ŏ���1��e����

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
                Shoot();
                NowShootTime = 0.0f; // �o�ߎ��ԃ��Z�b�g
            }

            NowShootTime += Time.fixedDeltaTime; // �o�ߎ��ԉ��Z
        }
    }

    // �e����
    public void Shoot()
    {
        if (CannonChild != null)
        {
            GameObject Child = Instantiate(CannonChild, gameObject.transform.position, Quaternion.Euler(Rad)); // �e����

            Child.GetComponent<Gimmick_CannonChild>().SetCannonChild(PlayerObject, Speed, LifeTime); // �e�̒l�Z�b�g
        }
    }

    public override void Death()
    {
        // �������g������
        Destroy(this.gameObject);
    }
}
