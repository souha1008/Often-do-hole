using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ResultManager : MonoBehaviour
{
    [System.NonSerialized] public bool anim_end;
    [System.NonSerialized] public static ResultManager instance;

    // �J����
    [Header("��z���h��֌W")]
    [SerializeField] Image Wanted_Sprite;

    // �X�e�[�W�i���o�[
    [Header("�X�e�[�W�i���o�[")]
    [SerializeField] Text StageNo;

    // UI
    [Header("UI")]
    [SerializeField] GameObject UI_Canvas;
    [SerializeField] GameObject LastStage_UICanvas;
    [SerializeField] Image NextStage_UI;
    [SerializeField] Image StageSelect_UI;
    [SerializeField] Image Last_StageSelect_UI;
    [SerializeField] Image Next_UI;
    [SerializeField] Image Stump_UI;
    [SerializeField] Image Photo_UI;

    Sprite White_NextStage_UI;
    Sprite Glay_NextStage_UI;
    Sprite White_StageSelect_UI;
    Sprite Glay_StageSelect_UI;
    Sprite White_Next_UI;
    Sprite Glay_Next_UI;

    UI_COMMAND ui_command;

    // �A�j���[�^�ϐ�
    [Header("�A�j���[�^")]
    public Animator stump_animator;
    public Animator Wanted_animator;

    // Skybox
    [Header("�X�J�C�{�b�N�X�֌W")]
    [SerializeField] Material Day_Skybox;
    [SerializeField] Material Evening_Skybox;
    [SerializeField] Material Night_Skybox;

    // �A�j���[�V�����p�����[�^
    [System.NonSerialized] public int Stump_Start;
    [System.NonSerialized] public int Stump_end;
    [System.NonSerialized] public int Shake_Start;
    [System.NonSerialized] public int Shake_End;
    //int Wanted_SkipAnime;
    //int Stump_SkipAnime;

    // �擾�R�C��
    [SerializeField] Text coin_Text;
    [SerializeField] Text Coin_AllNum;

    // �N���A�����N�p
    //Sprite[] Stump_sprite;

    // BGM�f�B���C�p
    //bool BGM_Dlay;
    //float NowTime;
    //[SerializeField] float wait_Time = 100;

    // �R���g���[���[
    bool OncePush = false;

    // �f�o�b�O�p
    [Header("�ȉ��f�o�b�O�R���\�[��")]
    [SerializeField] public bool debug_check;
    [SerializeField][Range(0, 14)] public int debug_stageNo;
    [SerializeField] [Range(0, 9)] int debug_coins;

    // ���̂ݑI�𔽉��p
    bool OnceSentakuFlag = false;

    enum ClearRank
    {
        Rank_S = 0,
        Rank_A,
        Rank_B,
    }
    [SerializeField] ClearRank clearRank;

    enum UI_COMMAND
    {
        NextStage = 0,
        StageSelect,
        Next
    };

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        debug_check = false;
#endif

        // UI�p�X�ݒ�
        ResourceSave();

        // �A�j���p�����[�^�n�b�V��
        AnimetorHash_Reset();

        // Photo�ݒ�
        Photo_Random();

        // �X�e�[�W�i���o�[��UI�Z�b�g
        StageNo_UISet();

        // �擾�R�C����UI�Z�b�g
        Coin_UISet();

        // �X�^���v�Z�b�g
        RankStump_Set();

        // �X�J�C�{�b�N�X�Z�b�g
        ChangeSkybox();

        // �X�e�[�W�����̍��W���T�C�Y��������
        TextChange();
        


        anim_end = false;
        UI_Canvas.SetActive(false);
        LastStage_UICanvas.SetActive(false);

        if (debug_check)
        {
            if (debug_stageNo != 7)
            {
                ui_command = UI_COMMAND.NextStage;
                NextStage_UI.sprite = White_NextStage_UI;
                StageSelect_UI.sprite = Glay_StageSelect_UI;
            }
            else
            {
                ui_command = UI_COMMAND.Next;
                Next_UI.sprite = White_Next_UI;
                Last_StageSelect_UI.sprite = Glay_StageSelect_UI;
            }
        }
        else
        {
            if (GameStateManager.GetNowStage() != 7)
            {
                ui_command = UI_COMMAND.NextStage;
                NextStage_UI.sprite = White_NextStage_UI;
                StageSelect_UI.sprite = Glay_StageSelect_UI;
            }
            else
            {
                ui_command = UI_COMMAND.Next;
                Next_UI.sprite = White_Next_UI;
                Last_StageSelect_UI.sprite = Glay_StageSelect_UI;
            }
        }

        //ui_command = UI_COMMAND.StageSelect;
        //StageSelect_UI.sprite = White_StageSelect_UI;


        //initPos = Wanted_Sprite.transform.position;

        SoundManager.Instance.PlaySound("sound_50", 0.5f, 29.0f, SOUND_FADE_TYPE.OUT, 3.0f, 0.0f, true); // ���̉��Đ�
    }

    private void Update()
    {
        // �{�^������������X�L�b�v
        //if (UI_Canvas.activeSelf == false)
        //{
        //    if (Input.GetButtonDown("ButtonA") && OncePush == false)
        //    {
        //        OncePush = true;    // �{�^���������Ă���

        //        // �A�j���[�^�[�ݒ�
        //        Wanted_animator.SetBool(Wanted_SkipAnime, true);
        //        stump_animator.SetBool(Stump_SkipAnime, true);
        //        stump_animator.SetBool(Stump_end, true);

        //        // UI���A�N�e�B�u
        //        UI_Canvas.SetActive(true);
        //    }
        //}

        if (!stump_animator.GetBool(Stump_Start) && anim_end == true)
        {
            stump_animator.SetBool(Stump_Start, true);
            Stump_UI.color = new Color(1, 1, 1, 1);
        }

        // UI����
        if (stump_animator.GetBool(Stump_end) == true && UI_Canvas.activeSelf == false)
        {
            if (debug_check)
            {
                if (debug_stageNo != 7)
                {
                    UI_Canvas.SetActive(true);
                }
                else
                {
                    LastStage_UICanvas.SetActive(true);
                }
            }

            else
            {
                if (GameStateManager.GetNowStage() != 7)
                {
                    UI_Canvas.SetActive(true);
                }
                else
                {
                    LastStage_UICanvas.SetActive(true);
                }
            }
            Wanted_animator.SetBool(Shake_Start, true);
        }

        // �A�j���[�V�������I����Ă�����UI����\
        if (stump_animator.GetBool(Stump_end) == true)
        {
            // ���X�g�X�e�[�W�ȊO
            if (UI_Canvas.activeSelf == true)
            {
                // �X�e�B�b�N�㉺
                if (Input.GetAxis("Vertical") > 0.8f || Input.GetAxis("Vertical") < -0.8)
                {
                    if (!OnceSentakuFlag)
                    {
                        SoundManager.Instance.PlaySound("sound_04_�I����", 1.0f, 0.1f);

                        if (ui_command == UI_COMMAND.NextStage)
                        {
                            ui_command = UI_COMMAND.StageSelect;
                            NextStage_UI.sprite = Glay_NextStage_UI;
                            StageSelect_UI.sprite = White_StageSelect_UI;
                        }
                        else
                        {
                            ui_command = UI_COMMAND.NextStage;
                            NextStage_UI.sprite = White_NextStage_UI;
                            StageSelect_UI.sprite = Glay_StageSelect_UI;
                        }

                        OnceSentakuFlag = true;
                    }                    
                }
                else
                {
                    OnceSentakuFlag = false;
                }

                switch (ui_command)
                {
                    case UI_COMMAND.NextStage:
                        if (Input.GetButtonDown("ButtonA") && OncePush == false)
                        {
                            OncePush = true;
                            // �U��
                            VibrationManager.Instance.StartVibration(0.65f, 0.65f, 0.3f);
                            // ���艹
                            SoundManager.Instance.PlaySound("sound_03_01");
                            SoundManager.Instance.FadeSound("Result_BGM", SOUND_FADE_TYPE.OUT, 1.0f, 0.0f, true);
                            GameStateManager.LoadNextStage();
                        }
                        break;

                    case UI_COMMAND.StageSelect:
                        if (Input.GetButtonDown("ButtonA") && OncePush == false)
                        {
                            OncePush = true;
                            // �U��
                            VibrationManager.Instance.StartVibration(0.65f, 0.65f, 0.3f);
                            // ���艹
                            SoundManager.Instance.PlaySound("sound_03_01");
                            SoundManager.Instance.FadeSound("Result_BGM", SOUND_FADE_TYPE.OUT, 1.0f, 0.0f, true);
                            GameStateManager.LoadStageSelect(true);
                        }
                        break;
                }
            }

            // ���X�g�X�e�[�W
            if (LastStage_UICanvas.activeSelf == true)
            {
                // �X�e�B�b�N�㉺
                if (Input.GetAxis("Vertical") > 0.8f || Input.GetAxis("Vertical") < -0.8)
                {
                    if (!OnceSentakuFlag)
                    {
                        SoundManager.Instance.PlaySound("sound_04_�I����", 1.0f, 0.1f);

                        // �X�e�[�g�l�N�X�g��������ς���
                        if (ui_command == UI_COMMAND.Next)
                        {
                            ui_command = UI_COMMAND.StageSelect;
                            Next_UI.sprite = Glay_Next_UI;
                            Last_StageSelect_UI.sprite = White_StageSelect_UI;
                        }
                        else
                        {
                            ui_command = UI_COMMAND.Next;
                            Next_UI.sprite = White_Next_UI;
                            Last_StageSelect_UI.sprite = Glay_StageSelect_UI;
                        }

                        OnceSentakuFlag = true;
                    }
                }

                else
                {
                    OnceSentakuFlag = false;
                }

                switch (ui_command)
                {
                    case UI_COMMAND.Next:
                        if (Input.GetButtonDown("ButtonA") && OncePush == false)
                        {
                            OncePush = true;
                            // �U��
                            VibrationManager.Instance.StartVibration(0.65f, 0.65f, 0.3f);
                            // ���艹
                            SoundManager.Instance.PlaySound("sound_03_01");
                            SoundManager.Instance.FadeSound("Result_BGM", SOUND_FADE_TYPE.OUT, 1.0f, 0.0f, true);
                            FadeManager.Instance.FadeStart("StaffRoleScene", FADE_KIND.FADE_SCENECHANGE);
                        }
                        break;

                    case UI_COMMAND.StageSelect:
                        if (Input.GetButtonDown("ButtonA") && OncePush == false)
                        {
                            OncePush = true;
                            // �U��
                            VibrationManager.Instance.StartVibration(0.65f, 0.65f, 0.3f);
                            // ���艹
                            SoundManager.Instance.PlaySound("sound_03_01");
                            SoundManager.Instance.FadeSound("Result_BGM", SOUND_FADE_TYPE.OUT, 1.0f, 0.0f, true);
                            GameStateManager.LoadStageSelect(true);
                        }
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{
        
    //}

    void AnimetorHash_Reset()
    {
        Stump_Start = Animator.StringToHash("Stump_Start");
        Stump_end = Animator.StringToHash("Stump_End");
        Shake_Start = Animator.StringToHash("Shake_Start");
        Shake_End = Animator.StringToHash("Shake_End");
        //Wanted_SkipAnime = Animator.StringToHash("Wanted_Skip_Anime");
        //Stump_SkipAnime = Animator.StringToHash("Stump_Skip_Anime");

    }

    void Photo_Random()
    {
        int rand = Random.Range(0, 4);

        //Debug.Log("�����_���l�F" + rand);

        switch(rand)
        {
            case 0:
                Photo_UI.sprite = Resources.Load<Sprite>("Sprite/WantedPoster_Photo/WantedPoster_Photo01_sepia");
                break;

            case 1:
                Photo_UI.sprite = Resources.Load<Sprite>("Sprite/WantedPoster_Photo/WantedPoster_Photo02_sepia");
                break;

            case 2:
                Photo_UI.sprite = Resources.Load<Sprite>("Sprite/WantedPoster_Photo/WantedPoster_Photo03_sepia");
                break;

            case 3:
                Photo_UI.sprite = Resources.Load<Sprite>("Sprite/WantedPoster_Photo/WantedPoster_Photo04_sepia");
                break;
        }
    }

    void StageNo_UISet()
    {
        // �f�o�b�O�p
        if (debug_check)
        {
            switch (debug_stageNo)
            {
                case 0:
                    StageNo.text = "TUTORIAL";
                    break;

                case 1:
                    StageNo.text = "1";
                    break;

                case 2:
                    StageNo.text = "2";
                    break;

                case 3:
                    StageNo.text = "3";
                    break;

                case 4:
                    StageNo.text = "4";
                    break;

                case 5:
                    StageNo.text = "5";
                    break;

                case 6:
                    StageNo.text = "6";
                    break;

                case 7:
                    StageNo.text = "BOSS";
                    break;

                case 8:
                    StageNo.text = "8";
                    break;

                case 9:
                    StageNo.text = "9";
                    break;

                case 10:
                    StageNo.text = "10";
                    break;

                case 11:
                    StageNo.text = "11";
                    break;

                case 12:
                    StageNo.text = "12";
                    break;

                case 13:
                    StageNo.text = "13";
                    break;

                case 14:
                    StageNo.text = "14";
                    break;
            }

        }
        else
        {
            // WANTED�摜����
            switch (GameStateManager.GetNowStage())
            {
                case 0:
                    StageNo.text = "TUTORIAL";
                    break;

                case 1:
                    StageNo.text = "1";
                    break;

                case 2:
                    StageNo.text = "2";
                    break;

                case 3:
                    StageNo.text = "3";
                    break;

                case 4:
                    StageNo.text = "4";
                    break;

                case 5:
                    StageNo.text = "5";
                    break;

                case 6:
                    StageNo.text = "6";
                    break;

                case 7:
                    StageNo.text = "BOSS";
                    break;

                case 8:
                    StageNo.text = "9";
                    break;

                case 9:
                    StageNo.text = "10";
                    break;

                case 10:
                    StageNo.text = "11";
                    break;

                case 11:
                    StageNo.text = "12";
                    break;

                case 12:
                    StageNo.text = "13";
                    break;

                case 13:
                    StageNo.text = "14";
                    break;

                case 14:
                    StageNo.text = "15";
                    break;
            }
        }
    }

    void ChangeSkybox()
    {
        // �f�o�b�O�p
        if (debug_check)
        {
            switch (debug_stageNo)
            {
                case 0:
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 1:
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 2:
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 3:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 4:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 5:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 6:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 7:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 8:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 9:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 10:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 11:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 12:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 13:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 14:
                    RenderSettings.skybox = Night_Skybox;
                    break;
            }

        }
        else
        {
            // WANTED�摜�X�e�[�W��������
            switch (GameStateManager.GetNowStage())
            {
                case 0:
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 1:
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 2:
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 3:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 4:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 5:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 6:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 7:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 8:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 9:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 10:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 11:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 12:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 13:
                    RenderSettings.skybox = Night_Skybox;
                    break;

                case 14:
                    RenderSettings.skybox = Night_Skybox;
                    break;
            }
        }
    }

    void Coin_UISet()
    {
        if (GameStateManager.GetNowStage() == 0)
        {
            Coin_AllNum.text = "/3";
        }
        int result_coin;
        if (debug_check)
        {
            result_coin = debug_coins;
        }
        else 
        {
           result_coin = SaveDataManager.Instance.GetStageData(GameStateManager.GetNowStage()).coin.AllGetCoins;
        }
        coin_Text.text = result_coin.ToString();
    }

    void ResourceSave()
    {
        White_NextStage_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/07_next-stage_btn");
        White_StageSelect_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/01_stageselect_btn");
        White_Next_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/08_next_btn");
        Glay_NextStage_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/07_next-stage2_btn");
        Glay_StageSelect_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/01_stageselect2_btn");
        Glay_Next_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/08_next2_btn");
    }

    void RankStump_Set()
    {
        if (debug_check)
        {
            switch (clearRank)
            {
                case ClearRank.Rank_S:
                    Stump_UI.sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_S");
                    break;

                case ClearRank.Rank_A:
                    Stump_UI.sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_A");
                    break;

                case ClearRank.Rank_B:
                    Stump_UI.sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_B");
                    break;
            }
        }
        else
        {
            switch (GameStateManager.GetGameRank())
            {
                case GAME_RANK.S:
                    Stump_UI.sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_S");
                    break;

                case GAME_RANK.A:
                    Stump_UI.sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_A");
                    break;

                case GAME_RANK.B:
                    Stump_UI.sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_B");
                    break;
            }
        }

    }

    private void TextChange()
    {
        if (debug_check)
        {
            if (debug_stageNo == 0)
            {
                StageNo.gameObject.transform.localPosition = new Vector3(252.0f, -497.0f, 0.0f); // ���W����
                StageNo.fontSize = 80; // �����T�C�Y����
            }
            else if (debug_stageNo == 7)
            {
                StageNo.gameObject.transform.localPosition = new Vector3(252.0f, -497.0f, 0.0f); // ���W����
                StageNo.fontSize = 92; // �����T�C�Y����
            }
        }
        else
        {
            if (GameStateManager.GetNowStage() == 0)
            {
                StageNo.gameObject.transform.localPosition = new Vector3(252.0f, -497.0f, 0.0f); // ���W����
                StageNo.fontSize = 80; // �����T�C�Y����
            }
            else if (GameStateManager.GetNowStage() == 7)
            {
                StageNo.gameObject.transform.localPosition = new Vector3(252.0f, -497.0f, 0.0f); // ���W����
                StageNo.fontSize = 92; // �����T�C�Y����
            }
        }
    }
}
