using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GO_POS
{
    LEFT,
    RIGHT,
    STAY
}

public class CameraPos : MonoBehaviour
{
    float RightSavePos = 0;
    float LeftSavePos = 0;

    float Difference = 0.0f;

    //10
    float MAX_DIFFERENCE = 10;
    float MAX_CAMERA_MOVE = 0.6f;

    //�����]�����Đi�񂾋���
    float ReturnMove = 0;

    Vector3 OldPlayerPos = Vector3.zero;

    public float cameraPosY = 5.3f;
    public float cameraDistanceZ = -50;//Z�̋���

    void Start()
    {
        Difference = MAX_DIFFERENCE;
    }


    void Update()
    {
        transform.position = PlayerMain.instance.transform.position + new Vector3(Difference, 2, cameraDistanceZ);
    }

    void FixedUpdate()
    {
        //�v���C���[���W
        Vector3 PlayerPos = PlayerMain.instance.transform.position;

        //�i�񂾗�
        float TempVel = PlayerPos.x - OldPlayerPos.x;
        Debug.Log(TempVel);

        //�J�����̈ړ��� 
        float TempCameraMove = Mathf.Min(TempVel * 0.5f, MAX_CAMERA_MOVE);
        TempCameraMove = Mathf.Max(TempVel * 0.5f, -MAX_CAMERA_MOVE);

        if (TempVel > 0.1f)
        {
            //�i�s�ɉ����ĕۑ�
            RightSavePos = PlayerMain.instance.transform.position.x;

            //���ȏ�܂�Ԃ�����
            if (PlayerPos.x > LeftSavePos + ReturnMove)
            {
                Difference = Mathf.Min(Difference + TempCameraMove, MAX_DIFFERENCE);
            }
        }
        if (TempVel < -0.1f)
        {
            //�i�s�ɉ����ĕۑ�
            LeftSavePos = PlayerMain.instance.transform.position.x;

            //���ȏ�܂�Ԃ�����
            if (PlayerPos.x < RightSavePos - ReturnMove)
            {
                Debug.Log("�ʂ���");
                Difference = Mathf.Max(Difference + TempCameraMove, -MAX_DIFFERENCE);
            }
        }

        OldPlayerPos = PlayerMain.instance.transform.position;
    }

    public void ManualUpdate()
    {
        Update();
    }



#if false

    GO_POS GoPos = GO_POS.STAY;
    float Difference = 0.0f;
    
    float MAX_DIFFERENCE = 10;

    float CameraMoveSpeed = 0.0f;
    float MAX_SPEED = 0.2f;
    float ADD_SPEED = 0.01f;

    int RightCnt = 0;
    int LeftCnt = 0;

    Vector3 OldPlayerPos = Vector3.zero;

    //target�I�u�W�F�N�g�f�[�^
    private GameObject targetObj;

    //target�ʒu�f�[�^
    private Vector3 targetPos;

    public float cameraPosY = 5.3f;
    public float cameraDistanceZ = -30;//Z�̋���

    void Start()
    {
        //target�I�u�W�F�N�g���擾
        targetObj = GameObject.Find("PlayerMain");
        targetPos = targetObj.transform.position;
        
    }


    void Update()
    {

       

        //�v���C���[�ɒǏ]���邾��
        targetPos = targetObj.transform.position;
        transform.position = targetObj.transform.position + new Vector3(Difference,10,cameraDistanceZ);

    }

    void FixedUpdate()
    {
        CheckGoPos();
        CheckDifference();

        OldPlayerPos = PlayerMain.instance.transform.position;
    }



    public void ManualUpdate()
    {
        Update();
    }

    void CheckGoPos()
    {
        Vector2 Stick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Stick.x > 0.2f)
        {
            LeftCnt = 0;
            RightCnt++;

            if(RightCnt < 10)
            {
                GoPos = GO_POS.STAY;
            }
            else
            {
                GoPos = GO_POS.RIGHT;
            }
        }
        else if (Stick.x < -0.2f)
        {
            RightCnt = 0;
            LeftCnt++;

            if (LeftCnt < 10)
            {
                GoPos = GO_POS.STAY;
            }
            else
            {
                GoPos = GO_POS.LEFT;
            }
        }
        else
        {
            //LeftCnt = 0;
            //RightCnt = 0;
        }

    }

    void CheckDifference ()
    {
        switch (GoPos)
        {
            

            case GO_POS.RIGHT:
                CameraMoveSpeed = Mathf.Min(CameraMoveSpeed + ADD_SPEED, MAX_SPEED);
                Difference = Mathf.Min(Difference + CameraMoveSpeed, MAX_DIFFERENCE);
                break;

            case GO_POS.LEFT:
                CameraMoveSpeed = Mathf.Min(CameraMoveSpeed + ADD_SPEED, MAX_SPEED);
                Difference = Mathf.Max(Difference - CameraMoveSpeed, -MAX_DIFFERENCE);
                break;

            case GO_POS.STAY:
                CameraMoveSpeed = 0.0f;
                break;
        }

        if(Difference == MAX_DIFFERENCE || Difference == -MAX_DIFFERENCE)
        {
            CameraMoveSpeed = 0.0f;
        }

    }

#endif

}


