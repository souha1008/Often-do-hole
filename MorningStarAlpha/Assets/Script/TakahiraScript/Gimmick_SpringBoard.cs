using UnityEngine;

public class Gimmick_SpringBoard : Gimmick_Main
{
    // �ϐ�
    [Label("�W�����v��̗�")]
    public float SpringPower = 100.0f;   // �W�����v��̗�

    private static float ReUseTime = 1.0f;
    private float Time;

    public override void Init()
    {
        Time = ReUseTime;
    }

    public override void FixedMove()
    {
        if (Time < ReUseTime)
            Time++;
    }

    public override void Death()
    {
        
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && Time >= ReUseTime)
        {
            float Rad;           // ��]�p
            Vector3 VecPower = Vector3.zero;    // ������x�N�g����

            PlayerMain playermain = collider.gameObject.GetComponent<PlayerMain>(); // �v���C���[���C���X�N���v�g�擾
            PlayerMain.instance.BulletScript.rb.position =  PlayerMain.instance.rb.position;   //�v���C���[�ƒe�̂����𓙂�������
            Rad = this.transform.localEulerAngles.z;  // �W�����v��̉�]�p(�x)
            Rad = CalculationScript.AngleCalculation(Rad); // �p�x���W�A���ϊ�
            VecPower = CalculationScript.AngleVectorXY(Rad) * SpringPower;  // ��ԃx�N�g����

            if (VecPower.x < 1 && VecPower.x > -1) VecPower.x = 0;  // �������l�͌덷�Ƃ���0�ɂ���
            if (VecPower.y < 1 && VecPower.y > -1) VecPower.y = 0;

            PlayerMain.instance.mode = new PlayerStateMidair(true, MidairState.BOOST);
            //PlayerMain.instance.ForciblyReleaseMode(false);

            PlayerMain.instance.vel = Vector3.zero;
            PlayerMain.instance.BulletScript.rb.velocity = Vector3.zero;
            PlayerMain.instance.addVel = VecPower;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySound("���艹");

            VibrationManager.Instance.StartVibration(1, 1, 0.2f);

            Time = 0.0f;
        }
    }
}
