using UnityEngine;
using UnityEngine.UI;

public class Direction_UI : MonoBehaviour
{
    [SerializeField] private PlayerMain PlayerScript;   // プレイヤー

    [SerializeField] private float Length = 6.0f;       // プレイヤーとの距離

    private Image DirectionUI;
    private RectTransform MyRectTf;

    void Start()
    {
        MyRectTf = GetComponent<RectTransform>();
        DirectionUI = GetComponent<Image>();

        DirectionUI.enabled = false;
    }

    public void LateUpdate()
    {
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;

        //打てる可能性があるステートなら表示
        if ((PlayerScript.refState == EnumPlayerState.ON_GROUND || PlayerScript.refState == EnumPlayerState.MIDAIR) && BulletMain.instance.CanShotFlag)
        {
            //クールタイム回復したら表示
            if (PlayerScript.canShotState)
            {
                DirectionUI.enabled = true;
            }
            else
            {
                DirectionUI.enabled = false;
            }


            if (PlayerScript.stickCanShotRange) // スティック入力一定以上あるか
            {
                // 座標変換
                MyRectTf.position = PlayerScript.transform.position + (vec * Length);

                // 回転
                MyRectTf.rotation = Quaternion.Euler(0, 0, CalculationScript.UnityTwoPointAngle360(Vector3.zero, vec) - 90);
            }
            else
            {
                DirectionUI.enabled = false;
            }
        }
        else
        {
            DirectionUI.enabled = false;
        }
    }

    //[SerializeField] private PlayerMain PlayerScript;   // プレイヤー
    //[SerializeField] private RectTransform CanvasTf;    // キャンバス
    //[SerializeField] private Camera MainCamera;    // メインカメラ
    //[SerializeField] private Camera UICamera;    // UIカメラ

    //[SerializeField] private float Length = 6.0f;       // プレイヤーとの距離
    //private Transform TargetTf;                         // プレイヤー

    //private RectTransform MyRectTf;
    //private Image DirectionUI;

    //void Start()
    //{
    //    MyRectTf = GetComponent<RectTransform>();
    //    TargetTf = PlayerScript.transform;
    //    DirectionUI = GetComponent<Image>();
    //    DirectionUI.enabled = false;
    //}

    //public void UpdateDirectionUI()
    //{
    //    Vector3 vec = PlayerScript.adjustLeftStick.normalized;

    //    //打てる可能性があるステートなら表示
    //    if (PlayerScript.refState == EnumPlayerState.ON_GROUND || PlayerScript.refState == EnumPlayerState.MIDAIR)
    //    {
    //        //クールタイム回復したら表示
    //        if (PlayerScript.canShotState)
    //        {
    //            DirectionUI.enabled = true;
    //        }
    //        else
    //        {
    //            DirectionUI.enabled = false;
    //        }


    //        if (PlayerScript.stickCanShotRange) // スティック入力一定以上あるか
    //        {
    //            // 座標変換

    //            // カメラ
    //            var pos = Vector2.zero;
    //            var uiCamera = UICamera;
    //            var worldCamera = MainCamera;
    //            var canvasRect = CanvasTf;

    //            var screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, TargetTf.position + vec * Length);
    //            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out pos);
    //            MyRectTf.anchoredPosition = pos;

    //            // オーバーレイ
    //            //MyRectTf.anchoredPosition = RectTransformUtility.WorldToScreenPoint(MainCamera, TargetTf.position + vec * Length);


    //            // 回転
    //            MyRectTf.rotation = Quaternion.Euler(0, 0, CalculationScript.UnityTwoPointAngle360(Vector3.zero, vec) - 90);
    //        }
    //        else
    //        {
    //            DirectionUI.enabled = false;
    //        }
    //    }
    //    else
    //    {
    //        DirectionUI.enabled = false;
    //    }   
    //}
}
