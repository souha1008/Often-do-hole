using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_SpringBoard : MonoBehaviour
{
    public float SpringPower = 3000.0f;   // �W�����v��̗�

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            float Rad;           // ��]�p
            Vector3 VecPower;    // ������x�N�g����

            PlayerMain playermain = collider.gameObject.GetComponent<PlayerMain>(); // �v���C���[���C���X�N���v�g�擾
            Rigidbody Rb = collider.gameObject.transform.GetComponent<Rigidbody>(); // ���W�b�h�{�f�B
            Rad = this.transform.localEulerAngles.z;  // �W�����v��̉�]�p
            Rad = CalculationScript.AngleCalculation(Rad); // �p�x���W�A���ϊ�
            VecPower = CalculationScript.AngleVectorXY(Rad) * SpringPower;  // ��ԃx�N�g����

            if (VecPower.x < 1 && VecPower.x > -1) VecPower.x = 0;
            if (VecPower.y < 1 && VecPower.y > -1) VecPower.y = 0;

            Debug.Log(VecPower.x);
            Debug.Log(VecPower.y);

            playermain.vel = VecPower; // �v���C���[�̃x�N�g���ʕύX
        }
    }
}
