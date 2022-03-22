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

    //方向転換して進んだ距離
    float ReturnMove = 0;

    Vector3 OldPlayerPos = Vector3.zero;

    public float cameraPosY = 5.3f;
    public float cameraDistanceZ = -50;//Zの距離

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
        //プレイヤー座標
        Vector3 PlayerPos = PlayerMain.instance.transform.position;

        //進んだ量
        float TempVel = PlayerPos.x - OldPlayerPos.x;
        Debug.Log(TempVel);

        //カメラの移動量 
        float TempCameraMove = Mathf.Min(TempVel * 0.5f, MAX_CAMERA_MOVE);
        TempCameraMove = Mathf.Max(TempVel * 0.5f, -MAX_CAMERA_MOVE);

        if (TempVel > 0.1f)
        {
            //進行に応じて保存
            RightSavePos = PlayerMain.instance.transform.position.x;

            //一定以上折り返したら
            if (PlayerPos.x > LeftSavePos + ReturnMove)
            {
                Difference = Mathf.Min(Difference + TempCameraMove, MAX_DIFFERENCE);
            }
        }
        if (TempVel < -0.1f)
        {
            //進行に応じて保存
            LeftSavePos = PlayerMain.instance.transform.position.x;

            //一定以上折り返したら
            if (PlayerPos.x < RightSavePos - ReturnMove)
            {
                Debug.Log("通った");
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

    //targetオブジェクトデータ
    private GameObject targetObj;

    //target位置データ
    private Vector3 targetPos;

    public float cameraPosY = 5.3f;
    public float cameraDistanceZ = -30;//Zの距離

    void Start()
    {
        //targetオブジェクトを取得
        targetObj = GameObject.Find("PlayerMain");
        targetPos = targetObj.transform.position;
        
    }


    void Update()
    {

       

        //プレイヤーに追従するだけ
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


