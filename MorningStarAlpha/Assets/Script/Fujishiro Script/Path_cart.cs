using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Path_cart : MonoBehaviour
{
    public static Raen_Path waypoints; // �E�F�C�|�C���g�i�[�p
    public Vector3[] path;
    public GameObject ikari;    // �C�J���I�u�W�F�N�g�i�[
    public Sequence sequence;

    public bool Rall_Start = false;
    private bool Rall_Now = false;
    int pointNo = 0;

    void Start()
    {
        Rall_Start = false;
        Rall_Now = false;
        path = waypoints.positions.Select(target => target.transform.position).ToArray();
    }

    void Update()
    {
        // �f�[�^�ێ�p
        if(waypoints != null)
        {
            if(Rall_Start == true && Rall_Now == false)
            {
                PlayerState.PlayerScript.mode = new PlayerStateRail(); // �X�e�[�g�����[����ԂɈڍs
                if(sequence.IsPlaying()) 
                {
                    sequence.onComplete();
                }
                else
                {
                    sequence = DOTween.Sequence().Append(
                            ikari.transform
                            .DOPath(path, 4f))
                            .onComplete(() =>
                            {
                                Debug.Log("Completed");
                            });
                            
                }
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
