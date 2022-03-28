using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirection : MonoBehaviour
{
    [SerializeField] private PlayerMain PlayerScript;
    [SerializeField] private float LINE_START_DISTANCE;  //���̊J�n�ʒu�i�v���C���[���S����̒����j
    [SerializeField] private float LINE_LENGTH;          //���̒���

    Color activeScol, activeEcol;
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, PlayerScript.transform.position);
        lr.SetPosition(1, PlayerScript.transform.position);

        activeScol = lr.startColor;
        activeEcol = lr.endColor;
    }

    private void LateUpdate()
    {
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;


        //�łĂ�\��������X�e�[�g�Ȃ�\��
        if (PlayerScript.refState == EnumPlayerState.ON_GROUND || PlayerScript.refState == EnumPlayerState.MIDAIR)
        {
            //�N�[���^�C���񕜂�����F����
            if (PlayerScript.canShotState)
            {
                lr.startColor = activeScol;
                lr.endColor = activeEcol;
            }
            else
            {
                lr.startColor = Color.gray;
                lr.endColor = Color.gray;
            }

            if (PlayerScript.stickCanShotRange)
            {
                //�n�_�ƏI�_��ݒ�
                lr.SetPosition(0, PlayerScript.transform.position + (vec * LINE_START_DISTANCE));
                lr.SetPosition(1, PlayerScript.transform.position + (vec * (LINE_START_DISTANCE + LINE_LENGTH)));
            }
            else
            {
                //�����ʒu�ɐݒ肵�ď���
                lr.SetPosition(0, PlayerScript.transform.position);
                lr.SetPosition(1, PlayerScript.transform.position);
            }
        }
        else
        {
            //�����ʒu�ɐݒ肵�ď���
            lr.SetPosition(0, PlayerScript.transform.position);
            lr.SetPosition(1, PlayerScript.transform.position);
        }



    }
}
