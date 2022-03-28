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

    private void Start()
    {
        instance = this;
        CameraTrueCnt = CAMERA_RETURN_FLAME;
        TraceObj();
    }

    public void ManualUpdate()
    {
        TraceObj();
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
        if(isRail)
        {


            TruePos.x = XObj.transform.position.x;
            TruePos.y = YObj.transform.position.y;
            TruePos.z = -CAMERA_DISTANCE;


            //いま振り子なら
            if(PlayerMain.instance.refState == EnumPlayerState.SWING)
            {
                this.transform.position = new Vector3(TruePos.x, transform.position.y, transform.position.z);

                CameraTrueCnt = 0;
            }
            //振り子じゃない時
            else
            {
                //Fixedにやってもらう
                //CameraTrueCnt++;

                if(CameraTrueCnt <= CAMERA_RETURN_FLAME)
                {
                    //20フレームかけて元の場所に戻る
                    this.transform.position = Vector3.Lerp(this.transform.position, TruePos,(float)CameraTrueCnt / CAMERA_RETURN_FLAME);
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

    void FixedUpdate()
    {
        CameraTrueCnt = Mathf.Min(CameraTrueCnt + 1, 500);
    }
}


