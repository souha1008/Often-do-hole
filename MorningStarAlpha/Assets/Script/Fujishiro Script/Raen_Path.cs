using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Raen_Path : MonoBehaviour
{
    public GameObject[] positions; // �E�F�C�|�C���g�z��
    public GameObject[] player_postions;
    [SerializeField, Tooltip("�J�n����I���ɂ����鎞��")] private float Time = 4.0f;

    private Vector3[] path;
    private Vector3[] player_path;


    private bool Rall_Start = false;
    private bool Rall_Now = false;

    void Start()
    {
        Rall_Start = false;
        Rall_Now = false;
    }

    void Update()
    {
      
        if (Rall_Start == true && Rall_Now == false)
        {
            PlayerMain.instance.mode = new PlayerState_Rail();

            positions[0] = BulletMain.instance.gameObject;
            player_postions[0] = PlayerMain.instance.gameObject;

            path = positions.Select(target => target.transform.position).ToArray();
            player_path = player_postions.Select(target => target.transform.position).ToArray();

            BulletMain.instance.transform
            .DOPath(path, Time, PathType.Linear)
            .OnUpdate(() =>
            {

            })
            .OnComplete(() =>
             {
                 Debug.Log("Complete");
                 PlayerState.PlayerScript.mode = new PlayerStateMidair(true, MidairState.NORMAL);
                 Rall_Start = false;
                 Rall_Now = false; 
             });

            PlayerMain.instance.transform
            .DOPath(player_path, Time);

            Rall_Now = true; // ���̂ݗp�t���O���I��
        }
        
}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Bullet") && Rall_Now == false)
        {
            Rall_Start = true;
        }
    }

}
