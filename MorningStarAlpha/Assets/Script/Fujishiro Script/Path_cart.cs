using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class Path_cart : MonoBehaviour 
{
    static public Raen_Path waypoint; // �X�N���v�g��� 
    public bool RaenStart = false; // ���[���X�^�[�g�p�t���O
    private bool NowRaen = false; // ���̂ݎ��s�p�t���O
    [SerializeField] GameObject ikari; // �������I�u�W�F�N�g�w��

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        // �t���O���I���Ȃ玩���Ń��[���ɏ�点��
        if (RaenStart)
        {


            // �p�X�������ɁA���b�ňړ�����
            ikari.transform.DOPath(waypoint.Waypoint, 10f);

            // �t���O�ݒ�
            RaenStart = false;
            NowRaen = true;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Bullet") && NowRaen == false)
        {
            RaenStart = true;
        }
    }

}