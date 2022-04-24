using UnityEngine;
using UnityEngine.UI;

public class Direction_UI : MonoBehaviour
{
    [SerializeField] private PlayerMain PlayerScript;   // �v���C���[
    [SerializeField] private RectTransform CanvasTf;    // �L�����o�X

    [SerializeField] private float Length = 6.0f;       // �v���C���[�Ƃ̋���
    private Transform TargetTf;                         // �v���C���[

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

        //�łĂ�\��������X�e�[�g�Ȃ�\��
        if (PlayerScript.refState == EnumPlayerState.ON_GROUND || PlayerScript.refState == EnumPlayerState.MIDAIR)
        {
            //�N�[���^�C���񕜂�����\��
            if (PlayerScript.canShotState)
            {
                DirectionUI.enabled = true;
            }
            else
            {
                DirectionUI.enabled = false;
            }

            // ���W�w��
            if (PlayerScript.stickCanShotRange) // �X�e�B�b�N���͈��ȏ゠�邩
            {
                // ���W�ύX
                MyRectTf.position = RectTransformUtility.WorldToScreenPoint(Camera.main, TargetTf.position + vec * Length);
                // ��]
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
