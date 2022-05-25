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

    Tweener shaketeener; // DOTween�̂��
    Vector3 initPos;    // ��z���̏����ʒu

    // �X�e�[�W�i���o�[
    [Header("�X�e�[�W�i���o�[")]
    [SerializeField] Text StageNo;

    // UI
    [Header("UI")]
    [SerializeField] GameObject UI_Canvas;
    [SerializeField] GameObject LastStage_UICanvas;
    [SerializeField] Image Next_UI;
    [SerializeField] Image StageSelect_UI;
    [SerializeField] Image Stump_UI;
    [SerializeField] Image Photo_UI;

    Sprite White_Next_UI;
    Sprite Glay_Next_UI;
    Sprite White_StageSelect_UI;
    Sprite Glay_StageSelect_UI;

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
    int Wanted_SkipAnime;
    int Stump_SkipAnime;

    // �擾�R�C��
    [SerializeField] Text coin_Text;
    [SerializeField] Text Coin_AllNum;

    // �N���A�����N�p
    Sprite[] Stump_sprite;

    // BGM�f�B���C�p
    bool BGM_Dlay;
    int flame_count01;
    [SerializeField] int wait_flame = 100;

    // �R���g���[���[
    bool OncePush = false;

    // �f�o�b�O�p
    [Header("�ȉ��f�o�b�O�R���\�[��")]
    [SerializeField] bool debug_check;
    [SerializeField][Range(0, 14)] int debug_stageNo;
    [SerializeField] [Range(0, 9)] int debug_coins;

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
        StageSelect
    };

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    async void Start()
    {

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


        

        anim_end = false;
        UI_Canvas.SetActive(false);
        LastStage_UICanvas.SetActive(false);
        ui_command = UI_COMMAND.NextStage;
        Next_UI.sprite = White_Next_UI;
        StageSelect_UI.sprite = Glay_StageSelect_UI;

        initPos = Wanted_Sprite.transform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SoundDlay();
        if(!Input.GetButton("ButtonA"))
        {
            OncePush = false;
        }

        // �{�^������������X�L�b�v
        if (UI_Canvas.activeSelf == false)
        {
            if (Input.GetButton("ButtonA") && OncePush == false)
            {
                OncePush = true;    // �{�^���������Ă���

                // �A�j���[�^�[�ݒ�
                Wanted_animator.SetBool(Wanted_SkipAnime, true);
                stump_animator.SetBool(Stump_SkipAnime, true);
                stump_animator.SetBool(Stump_end, true);
                
                // UI���A�N�e�B�u
                UI_Canvas.SetActive(true);
            }
        }

        if (anim_end == true)        {
            stump_animator.SetBool(Stump_Start, true);
            Stump_UI.color = new Color(1, 1, 1, 1);
        }

        // UI����
        if (stump_animator.GetBool(Stump_end) == true && UI_Canvas.activeSelf == false)
        {
            if (GameStateManager.GetNowStage() != 7)
            {
                UI_Canvas.SetActive(true);
            }
            else
            {
                LastStage_UICanvas.SetActive(true);
            }
            Wanted_animator.SetBool(Shake_Start, true);
        }

        // �A�j���[�V�������I����Ă�����UI����\
        if (stump_animator.GetBool(Stump_end) == true)
        {
            // ���X�g�X�e�[�W�ȊO
            if (UI_Canvas.activeSelf == true)
            {
                // �X�e�B�b�N��
                if (Input.GetAxis("Vertical") > 0.8f)
                {
                    ui_command = UI_COMMAND.NextStage;
                    Next_UI.sprite = White_Next_UI;
                    StageSelect_UI.sprite = Glay_StageSelect_UI;
                }
                // �X�e�B�b�N��
                if (Input.GetAxis("Vertical") < -0.8)
                {
                    ui_command = UI_COMMAND.StageSelect;
                    Next_UI.sprite = Glay_Next_UI;
                    StageSelect_UI.sprite = White_StageSelect_UI;
                }

                switch (ui_command)
                {
                    case UI_COMMAND.NextStage:
                        if (Input.GetButton("ButtonA") && OncePush == false)
                        {
                            GameStateManager.LoadNextStage();
                        }
                        break;

                    case UI_COMMAND.StageSelect:
                        if (Input.GetButton("ButtonA") && OncePush == false)
                        {
                            FadeManager.Instance.FadeStart("StageSelectScene", FADE_KIND.FADE_SCENECHANGE);
                        }
                        break;
                }
            }

            // ���X�g�X�e�[�W
            if(LastStage_UICanvas.activeSelf == true)
            {
                if (Input.GetButton("ButtonA") && OncePush == false)
                {
                    FadeManager.Instance.FadeStart("StageSelectScene", FADE_KIND.FADE_SCENECHANGE);
                }
            }
        }
        
    }

    void AnimetorHash_Reset()
    {
        Stump_Start = Animator.StringToHash("Stump_Start");
        Stump_end = Animator.StringToHash("Stump_End");
        Shake_Start = Animator.StringToHash("Shake_Start");
        Shake_End = Animator.StringToHash("Shake_End");
        Wanted_SkipAnime = Animator.StringToHash("Wanted_Skip_Anime");
        Stump_SkipAnime = Animator.StringToHash("Stump_Skip_Anime");

    }

    void SoundDlay()
    {
        if (flame_count01 < wait_flame)
        {
            flame_count01++;
        }

        if (flame_count01 >= wait_flame && BGM_Dlay == false)
        {
            BGM_Dlay = true;
            SoundManager.Instance.PlaySound("Result_BGM");
        }
    }

    void Photo_Random()
    {
        int rand = Random.Range(0, 4);

        Debug.Log("�����_���l�F" + rand);

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
                    StageNo.text = "1-1";
                    break;

                case 1:
                    StageNo.text = "1-2";
                    break;

                case 2:
                    StageNo.text = "1-3";
                    break;

                case 3:
                    StageNo.text = "1-4";
                    break;

                case 4:
                    StageNo.text = "1-5";
                    break;

                case 5:
                    StageNo.text = "2-1";
                    break;

                case 6:
                    StageNo.text = "2-2";
                    break;

                case 7:
                    StageNo.text = "2-3";
                    break;

                case 8:
                    StageNo.text = "2-4";
                    break;

                case 9:
                    StageNo.text = "2-5";
                    break;

                case 10:
                    StageNo.text = "3-1";
                    break;

                case 11:
                    StageNo.text = "3-2";
                    break;

                case 12:
                    StageNo.text = "3-3";
                    break;

                case 13:
                    StageNo.text = "3-4";
                    break;

                case 14:
                    StageNo.text = "3-5";
                    break;
            }

        }
        else
        {
            // WANTED�摜����
            switch (GameStateManager.GetNowStage())
            {
                case 0:
                    StageNo.text = "1-1";
                    break;

                case 1:
                    StageNo.text = "1-2";
                    break;

                case 2:
                    StageNo.text = "1-3";
                    break;

                case 3:
                    StageNo.text = "1-4";
                    break;

                case 4:
                    StageNo.text = "1-5";
                    break;

                case 5:
                    StageNo.text = "2-1";
                    break;

                case 6:
                    StageNo.text = "2-2";
                    break;

                case 7:
                    StageNo.text = "2-3";
                    break;

                case 8:
                    StageNo.text = "2-4";
                    break;

                case 9:
                    StageNo.text = "2-5";
                    break;

                case 10:
                    StageNo.text = "3-1";
                    break;

                case 11:
                    StageNo.text = "3-2";
                    break;

                case 12:
                    StageNo.text = "3-3";
                    break;

                case 13:
                    StageNo.text = "3-4";
                    break;

                case 14:
                    StageNo.text = "3-5";
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
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 4:
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 5:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 6:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 7:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 8:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 9:
                    RenderSettings.skybox = Evening_Skybox;
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
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 4:
                    RenderSettings.skybox = Day_Skybox;
                    break;

                case 5:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 6:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 7:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 8:
                    RenderSettings.skybox = Evening_Skybox;
                    break;

                case 9:
                    RenderSettings.skybox = Evening_Skybox;
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
        White_Next_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/07_next-stage_btn");
        White_StageSelect_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/01_stageselect_btn");
        Glay_Next_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/07_next-stage2_btn");
        Glay_StageSelect_UI = Resources.Load<Sprite>("Sprite/UI/Resulut/01_stageselect2_btn");
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
}
