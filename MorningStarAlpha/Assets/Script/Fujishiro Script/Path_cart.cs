using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Path_cart : MonoBehaviour
{
    public static Raen_Path waypoints; // �E�F�C�|�C���g�i�[�p
    public GameObject ikari;    // �C�J���I�u�W�F�N�g�i�[

    public bool Rall_Start = false;
    private bool Rall_Now = false;
    int pointNo;

    void Start()
    {
        Rall_Start = false;
        Rall_Now = false;
        pointNo = waypoints.Path.Length;
    }

    void Update()
    {
        // �f�[�^�ێ�p
        if(waypoints != null)
        {
            if(Rall_Start == true && Rall_Now == false)
            {
                PlayerState.PlayerScript.mode = new PlayerStateRail(); // �X�e�[�g�����[����ԂɈڍs
                ikari.transform.DOPath(
                    waypoints.Path, 
                    6.0f)
                    .OnWaypointChange(pointNo =>{
                        if(pointNo > 0)
                        {

                        }
                    
                    });          // �p�X���œ�����
                Rall_Now = true;                                       // ���̂ݗp�t���O���I��
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Bullet") && Rall_Now == false)
        {
            Rall_Start = true;
        }
    }
}
