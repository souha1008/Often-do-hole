using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirection : MonoBehaviour
{
    [SerializeField] private PlayerMain Player;
    [SerializeField] private float LINE_START_DISTANCE;  //���̊J�n�ʒu�i�v���C���[���S����̒����j
    [SerializeField] private float LINE_LENGTH;          //���̒���
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, Player.transform.position);
        lr.SetPosition(1, Player.transform.position);
    }

    private void LateUpdate()
    {
        Vector3 vec = Player.leftStick.normalized;

        if (Player.canShot)
        {
            //�n�_�ƏI�_��ݒ�
            lr.SetPosition(0, Player.transform.position + (vec * LINE_START_DISTANCE));
            lr.SetPosition(1, Player.transform.position + (vec * (LINE_START_DISTANCE + LINE_LENGTH)));
        }
        else
        {
            //�����ʒu�ɐݒ肵�ď���
            lr.SetPosition(0, Player.transform.position);
            lr.SetPosition(1, Player.transform.position);
        }
       
    }
}
