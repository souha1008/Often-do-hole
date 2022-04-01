using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Bridge : Gimmick_Main
{
    [Label("�|��Ă��鎞��")]
    public float MoveTime = 3.0f;

    private bool StartFlag;
    private bool Endflag;
    private float NowTime;
    private Vector3 StartPos;
    private float OldRad;
    private GameObject Player;

    public override void Init()
    {
        // ������
        StartFlag = false;
        Endflag = false;
        NowTime = 0.0f;
        OldRad = 0.0f;

        // ���W�b�h�{�f�B
        Rb.isKinematic = true;

        // �R���C�_�[
        Cd.isTrigger = false;       

        // �v���C���[�擾
        Player = GameObject.Find("Player");

        // ������]
        Vector3 ThisPos = StartPos = this.gameObject.transform.position; // �����̍��W
        Vector3 RotatePos = new Vector3(ThisPos.x, ThisPos.y, ThisPos.z + this.gameObject.transform.localScale.z * 0.5f); // ��]����W
        Quaternion AngleAxis = Quaternion.AngleAxis(90.0f, Vector3.right); // ��]���Ɗp�x
        
        Vector3 Pos = ThisPos;

        Pos -= RotatePos;
        Pos = AngleAxis * Pos;
        Pos += RotatePos;

        this.gameObject.transform.position = Pos; // ���݂̍��W�X�V

        this.gameObject.transform.rotation *= AngleAxis; // ��]�X�V
    }

    public override void UpdateMove()
    {
        //if(Input.GetKeyDown(KeyCode.Q))
        float Distance = Vector2.Distance(Player.transform.position, StartPos);
        if (Distance < 55 && !StartFlag)
        {
            NowTime = 0.0f;
            StartFlag = true;
            Endflag = false;
        }
    }

    public override void FixedMove()
    {
        if (StartFlag && !Endflag)
        {
            if (NowTime > MoveTime)
            {
                NowTime = MoveTime;
                Endflag = true;
            }

            Rotate();

            NowTime += Time.fixedDeltaTime; // ���ԉ��Z
        }
    }

    public override void Death()
    {
        
    }

    private void Rotate()
    {
        Vector3 ThisPos = this.gameObject.transform.position; // �����̍��W
        Vector3 RotatePos = new Vector3(StartPos.x, StartPos.y, StartPos.z + this.gameObject.transform.localScale.z * 0.5f); // ��]����W
        Quaternion AngleAxis = Quaternion.AngleAxis(-(Easing.BounceOut(NowTime, MoveTime, 0, 90) - OldRad), Vector3.right); // ��]���Ɗp�x
        OldRad = Easing.BounceOut(NowTime, MoveTime, 0, 90);

        Vector3 Pos = ThisPos;

        Pos -= RotatePos;
        Pos = AngleAxis * Pos;
        Pos += RotatePos;

        this.gameObject.transform.position = Pos; // ���݂̍��W�X�V

        this.gameObject.transform.rotation *= AngleAxis; // ��]�X�V
    }
}
