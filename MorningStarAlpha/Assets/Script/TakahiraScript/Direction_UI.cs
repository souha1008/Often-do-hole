using UnityEngine;
using UnityEngine.UI;

public class Direction_UI : MonoBehaviour
{
    [SerializeField] private PlayerMain PlayerScript;   // プレイヤー
    [SerializeField] private RectTransform CanvasTf;    // キャンバス

    [SerializeField] private float Length = 6.0f;       // プレイヤーとの距離
    private Transform TargetTf;                         // プレイヤー

    private RectTransform MyRectTf;
    private Image DirectionUI;

    void Start()
    {
        MyRectTf = GetComponent<RectTransform>();
        TargetTf = PlayerScript.transform;
        DirectionUI = GetComponent<Image>();
    }

    void Update()
    {
        //MyRectTf.position
        //    = RectTransformUtility.WorldToScreenPoint(Camera.main, TargetTf.position + Offset);
    }

    void LateUpdate()
    {
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;

        //打てる可能性があるステートなら表示
        if (PlayerScript.refState == EnumPlayerState.ON_GROUND || PlayerScript.refState == EnumPlayerState.MIDAIR)
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

            // 座標指定
            if (PlayerScript.stickCanShotRange) // スティック入力一定以上あるか
            {
                // 座標変更
                MyRectTf.position = RectTransformUtility.WorldToScreenPoint(Camera.main, TargetTf.position + vec * Length);
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
}
