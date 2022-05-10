using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainShimokawara: MonoBehaviour
{
    [Header("チェックが入っていたらレール追従")]
    [SerializeField, Tooltip("チェックが入っていたらレール追従")] public bool isRail;        //これにチェックが入っていたら分割

    [SerializeField] private GameObject XObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private属性だけどinspector上で設定できるようにする
    [SerializeField] private GameObject YObj /*= GameObject.Find("CameraCenterPos")*/;         // [SerializeField] private属性だけどinspector上で設定できるようにする
    [SerializeField] public float CAMERA_DISTANCE;      //カメラとプレイヤーの距離

    public static CameraMainShimokawara instance; 
    
    Vector3 TruePos = Vector3.zero;//カメラが本来あるべき場所（振り子の時のみレールからそれる）
    int CameraTrueCnt;//カメラが正しい位置を探索するフレームを数える
    int CAMERA_RETURN_FLAME = 250;//カメラが何フレームかけて正しい場所に行くか

    //振り子やめた瞬間を拾うために、勝手に保存してる
    EnumPlayerState PlayerState;
    EnumPlayerState OldPlayerState;

    private bool stopFlag; //カメラ停止用

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        stopFlag = false;
        CameraTrueCnt = CAMERA_RETURN_FLAME;
        TraceObj();

        PlayerState = OldPlayerState = PlayerMain.instance.refState;
    }

    public void ManualUpdate()
    {
        if (stopFlag == false)
        {
            TraceObj();
        }
    }

    public void CameraReturn()
    {
        if (isRail)
        {
            if (VirtualCamera.instance. isReturn)
            {
                Vector3 Temp = TempCamera.instance.transform.position;
                Temp.z -= CAMERA_DISTANCE;

                transform.position = Temp;
            }
        }
    }

    //private void FixedUpdate()
    //{
    //    TracePlayer();
    //}

    //private void LateUpdate()
    //{
    //    TraceObj();
    //}

    //プレイヤーをカメラの中央に収め続ける
    void TraceObj()
    {
        OldPlayerState = PlayerState;
        PlayerState = PlayerMain.instance.refState;

        if(isRail)
        {
            //振り子じゃなくなったタイミングで、CameraTrueCntを0にする
            if(OldPlayerState == EnumPlayerState.SWING && PlayerState != EnumPlayerState.SWING)
            {
                CameraTrueCnt = 0;
            }


            TruePos.x = XObj.transform.position.x;
            TruePos.y = YObj.transform.position.y;
            TruePos.z = -CAMERA_DISTANCE;


            //いま振り子なら
            if(PlayerMain.instance.refState == EnumPlayerState.SWING)
            {
                this.transform.position = new Vector3(TruePos.x, transform.position.y, transform.position.z);

            }
            //振り子じゃない時
            else
            {                
                //↓Fixedにやってもらう
                //CameraTrueCnt++;

                if(CameraTrueCnt <= CAMERA_RETURN_FLAME)
                {
                    //今のカウントをイージングして、初速と再終息を遅めにする
                    float EasingCnt;
                    if (CameraTrueCnt < CAMERA_RETURN_FLAME / 2)
                    {
                        EasingCnt = CameraTrueCnt * Easing.EasingTypeFloat(EASING_TYPE.QUAD_IN, CameraTrueCnt, (CAMERA_RETURN_FLAME / 2 - 1), 0, 1);
                    }
                    else if(CameraTrueCnt > CAMERA_RETURN_FLAME / 2)
                    {
                        EasingCnt = CameraTrueCnt * Easing.EasingTypeFloat(EASING_TYPE.QUAD_IN, CAMERA_RETURN_FLAME - CameraTrueCnt, (CAMERA_RETURN_FLAME / 2 - 1), 0, 1);

                    }
                    else
                    {
                        EasingCnt = CameraTrueCnt;
                    }

                    //20フレームかけてYを元の場所に戻る
                    float Y = Mathf.Lerp(this.transform.position.y, TruePos.y, (float)EasingCnt / CAMERA_RETURN_FLAME);
                    this.transform.position = new Vector3(TruePos.x, Y, TruePos.z);
                }
                else
                {
                    this.transform.position = TruePos;
                }

                
            }
            
        }
        else
        {
            TruePos = XObj.transform.position;

            TruePos.z -= CAMERA_DISTANCE;
            this.transform.position = TruePos;
        }

    }
    
    public void StopCamera()
    {
        stopFlag = true;
    }

    void FixedUpdate()
    {
        CameraTrueCnt = Mathf.Min(CameraTrueCnt + 1, 500);


        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    CameraShake.instance.Shake();
        //}

    }

    //あるべき場所に寄せる関数
    void Bring()
    {
        //今のカウントをイージングして、初速と再終息を遅めにする
        float EasingCnt;
        //寄せ終わってる
        if (CameraTrueCnt > CAMERA_RETURN_FLAME)
        {
            EasingCnt = CAMERA_RETURN_FLAME;
        }
        //寄せ中
        else
        {
            if (CameraTrueCnt < CAMERA_RETURN_FLAME / 2)
            {
                EasingCnt = CameraTrueCnt * Easing.EasingTypeFloat(EASING_TYPE.QUAD_IN, CameraTrueCnt, (CAMERA_RETURN_FLAME / 2 - 1), 0, 1);
            }
            else if (CameraTrueCnt > CAMERA_RETURN_FLAME / 2)
            {
                EasingCnt = CameraTrueCnt * Easing.EasingTypeFloat(EASING_TYPE.QUAD_IN, CAMERA_RETURN_FLAME - CameraTrueCnt, (CAMERA_RETURN_FLAME / 2 - 1), 0, 1);
            }
            else
            {
                EasingCnt = CameraTrueCnt;
            }
        }

        //20フレームかけて元の場所に戻る
        this.transform.position = Vector3.Lerp(this.transform.position, TruePos, (float)EasingCnt / CAMERA_RETURN_FLAME);
    }
}


