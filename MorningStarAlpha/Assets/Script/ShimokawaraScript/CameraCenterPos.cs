using UnityEngine;

public enum GO_POS_X
{
    LEFT,
    RIGHT,
    STAY
}

public enum GO_POS_Y
{
    UP,
    DOWN,
    STAY
}



public class CameraCenterPos : MonoBehaviour
{
    //[Header("�v���C���[�ƃJ�����̋�����MainCamera�Q��")]


    Vector3 PlayerPos = Vector3.zero;
    Vector3 OldPlayerPos = Vector3.zero;

    //[Header("�J������������")]
    //[Range(-10, -60), SerializeField, Tooltip("�J������������")] private float cameraDistanceZ ;//Z�̋���  

    [Header("�`�F�b�N�������Ă�����Y�Œ�")]
    [SerializeField, Tooltip("�`�F�b�N�������Ă�����Y�Œ�")] private bool FreezeY;        //����Ƀ`�F�b�N�������Ă����番��

    [Header("�hY�Œ莞�̂݁h���̒l�ŃJ������������     �v���X�ŃJ���������")]
    [Range(-10, 20), SerializeField, Tooltip("�hY�Œ莞�̂݁h���̒l�ŃJ������������ \n�v���X�ŃJ���������")] private float cameraDistanceY;   //�X�e�B�b�N������␳����i�v�f���ŕ����j�l�͏オ0�Ŏ��v���ɑ����B0~360�͈̔�

    [Header("�`��̗L��")]
    [SerializeField, Tooltip("�`��̗L��")] private bool isDraw;   //�X�e�B�b�N������␳����i�v�f���ŕ����j�l�͏オ0�Ŏ��v���ɑ����B0~360�͈̔�


    //��
    //���ړ�
    //��
    float RightSavePos = 0;
    float LeftSavePos = 0;

    float DifferenceX = 0.0f;

    [Header("�ړ����x�{�� ��")]
    [Range(0.1f, 3.0f), SerializeField, Tooltip("�ړ����x�{�� ��")] private float SpeedMultiX = 0.5f;

    //10
    [Header("�œ_�ƃv���C���[�̋������ ��")]
    [Range(1,15), SerializeField, Tooltip("�œ_�ƃv���C���[�̋������ ��")] private float MAX_DIFFERENCE_X = 10;


    [Header("1F������̈ړ�������� ��")]
    [Range(0.1f, 3.0f), SerializeField, Tooltip("1F������̈ړ�������� ��")] private float MAX_CAMERA_MOVE_X = 0.6f;

    //�����]�����Đi�񂾋���
    float ReturnMove_X = 0;

    //��
    //�c�ړ�
    //��
    GO_POS_Y GoPosY = GO_POS_Y.DOWN;

    float DifferenceY = 0.0f;

    float SpeedMultiY = 0.5f;

    float UP_CAMEPA_SPEED = 0.08f;

    float UP_DIFFERENCE_Y = 10;
    float DOWN_DIFERENCE_Y = 0;



    


    void Start()
    {
        DifferenceX = MAX_DIFFERENCE_X;
        DifferenceY = UP_DIFFERENCE_Y;

       
    }


    void Update()
    {
        if (!isDraw)
        {
            GetComponent<Renderer>().material.color = Color.clear;
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(1,0,0,0.5f);
        }


#if true
        //����
        if (!FreezeY)
        {
            transform.position = PlayerMain.instance.transform.position + new Vector3(DifferenceX, DifferenceY, 0);

        }
        else
        {
            transform.position = PlayerMain.instance.transform.position + new Vector3(DifferenceX, cameraDistanceY, 0);

        }
#else
        //Y�ɑ΂��ăJ�����ɍ��킹��悤��������

        float PosY = GameObject.Find("Main Camera").transform.position.y;

        if (!FreezeY)
        {
            transform.position = PlayerMain.instance.transform.position + new Vector3(DifferenceX, DifferenceY, 0);

            transform.position = new Vector3(transform.position.x, PosY, transform.position.z);

        }
        else
        {
            transform.position = PlayerMain.instance.transform.position + new Vector3(DifferenceX, cameraDistanceY, 0);

            transform.position = new Vector3(transform.position.x, PosY, transform.position.z);
        }
#endif

    }

    void FixedUpdate()
    {
        //�v���C���[���W
        PlayerPos = PlayerMain.instance.transform.position;

        FixedX();
        //����
        if(!FreezeY)
        {
            FixedY();
        }
        
        OldPlayerPos = PlayerMain.instance.transform.position;

    }

    void FixedX()
    {
        //�i�񂾗�
        float TempVel = PlayerPos.x - OldPlayerPos.x;

        //�J�����̈ړ��� 
        float TempCameraMove = Mathf.Min(TempVel * SpeedMultiX, MAX_CAMERA_MOVE_X);
        TempCameraMove = Mathf.Max(TempVel * SpeedMultiX, -MAX_CAMERA_MOVE_X);

        if (TempVel > 0.1f)
        {
            //�i�s�ɉ����ĕۑ�
            RightSavePos = PlayerMain.instance.transform.position.x;

            //���ȏ�܂�Ԃ�����
            if (PlayerPos.x > LeftSavePos + ReturnMove_X)
            {
                DifferenceX = Mathf.Min(DifferenceX + TempCameraMove, MAX_DIFFERENCE_X);
            }
        }
        if (TempVel < -0.1f)
        {
            //�i�s�ɉ����ĕۑ�
            LeftSavePos = PlayerMain.instance.transform.position.x;

            //���ȏ�܂�Ԃ�����
            if (PlayerPos.x < RightSavePos - ReturnMove_X)
            {
                DifferenceX = Mathf.Max(DifferenceX + TempCameraMove, -MAX_DIFFERENCE_X);
            }
        }

    }

    void FixedY()
    {
        //�i�񂾗�
        float TempVel = PlayerPos.y - OldPlayerPos.y;

        //�J�����̈ړ��� 
        float TempCameraMove = Mathf.Abs(TempVel) * SpeedMultiY;

#if false
        if (TempVel > 0.1f)
        {
            GoPosY = GO_POS_Y.DOWN;
        }
        else if(TempVel <= 0.0f)
        {
            GoPosY = GO_POS_Y.UP;
        }
        else
        {
            GoPosY = GO_POS_Y.STAY;
        }

        switch (GoPosY)
        {
            case GO_POS_Y.UP:
                DifferenceY = Mathf.Min(UP_DIFFERENCE_Y, DifferenceY + TempCameraMove);

                break;
            case GO_POS_Y.DOWN:
                DifferenceY = Mathf.Max(DOWN_DIFERENCE_Y, DifferenceY - TempCameraMove);
                break;
            default:
                break;
        }
#endif

        //�㏸���������Ēn�ɑ��t���Əオ��

        //�㏸��
        if (TempVel > 0.1f)
        {
            GoPosY = GO_POS_Y.DOWN;
        }
        //�n��
        else if (PlayerMain.instance.isOnGround)
        {
            GoPosY = GO_POS_Y.UP;
        }
        else
        {
            GoPosY = GO_POS_Y.STAY;
        }

        switch (GoPosY)
        {
            case GO_POS_Y.UP:
                DifferenceY = Mathf.Min(UP_DIFFERENCE_Y, DifferenceY + UP_CAMEPA_SPEED);

                break;
            case GO_POS_Y.DOWN:
                DifferenceY = Mathf.Max(DOWN_DIFERENCE_Y, DifferenceY - TempCameraMove);
                break;
            default:
                break;
        }

    }


    public void ManualUpdate()
    {
        Update();
    }



#if false

    GO_POS_X GoPos = GO_POS_X.STAY;
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
                GoPos = GO_POS_X.STAY;
            }
            else
            {
                GoPos = GO_POS_X.RIGHT;
            }
        }
        else if (Stick.x < -0.2f)
        {
            RightCnt = 0;
            LeftCnt++;

            if (LeftCnt < 10)
            {
                GoPos = GO_POS_X.STAY;
            }
            else
            {
                GoPos = GO_POS_X.LEFT;
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
            

            case GO_POS_X.RIGHT:
                CameraMoveSpeed = Mathf.Min(CameraMoveSpeed + ADD_SPEED, MAX_SPEED);
                Difference = Mathf.Min(Difference + CameraMoveSpeed, MAX_DIFFERENCE);
                break;

            case GO_POS_X.LEFT:
                CameraMoveSpeed = Mathf.Min(CameraMoveSpeed + ADD_SPEED, MAX_SPEED);
                Difference = Mathf.Max(Difference - CameraMoveSpeed, -MAX_DIFFERENCE);
                break;

            case GO_POS_X.STAY:
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


