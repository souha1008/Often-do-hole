using UnityEngine;
using UnityEngine.UI;

public class Direction_UI : MonoBehaviour
{
    [SerializeField] private PlayerMain PlayerScript;   // �v���C���[

    [SerializeField] private float Length = 6.0f;       // �v���C���[�Ƃ̋���

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

        //�łĂ�\��������X�e�[�g�Ȃ�\��
        if ((PlayerScript.refState == EnumPlayerState.ON_GROUND || PlayerScript.refState == EnumPlayerState.MIDAIR) && BulletMain.instance.CanShotFlag)
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


            if (PlayerScript.stickCanShotRange) // �X�e�B�b�N���͈��ȏ゠�邩
            {
                // ���W�ϊ�
                MyRectTf.position = PlayerScript.transform.position + (vec * Length);

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

    //[SerializeField] private PlayerMain PlayerScript;   // �v���C���[
    //[SerializeField] private RectTransform CanvasTf;    // �L�����o�X
    //[SerializeField] private Camera MainCamera;    // ���C���J����
    //[SerializeField] private Camera UICamera;    // UI�J����

    //[SerializeField] private float Length = 6.0f;       // �v���C���[�Ƃ̋���
    //private Transform TargetTf;                         // �v���C���[

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

    //    //�łĂ�\��������X�e�[�g�Ȃ�\��
    //    if (PlayerScript.refState == EnumPlayerState.ON_GROUND || PlayerScript.refState == EnumPlayerState.MIDAIR)
    //    {
    //        //�N�[���^�C���񕜂�����\��
    //        if (PlayerScript.canShotState)
    //        {
    //            DirectionUI.enabled = true;
    //        }
    //        else
    //        {
    //            DirectionUI.enabled = false;
    //        }


    //        if (PlayerScript.stickCanShotRange) // �X�e�B�b�N���͈��ȏ゠�邩
    //        {
    //            // ���W�ϊ�

    //            // �J����
    //            var pos = Vector2.zero;
    //            var uiCamera = UICamera;
    //            var worldCamera = MainCamera;
    //            var canvasRect = CanvasTf;

    //            var screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, TargetTf.position + vec * Length);
    //            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out pos);
    //            MyRectTf.anchoredPosition = pos;

    //            // �I�[�o�[���C
    //            //MyRectTf.anchoredPosition = RectTransformUtility.WorldToScreenPoint(MainCamera, TargetTf.position + vec * Length);


    //            // ��]
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
