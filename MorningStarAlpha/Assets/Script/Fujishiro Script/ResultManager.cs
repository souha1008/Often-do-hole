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
    [SerializeField][Range(0.0f, 1.0f)] float duration;
    [SerializeField] float strength;
    [SerializeField] int vibrato;
    [SerializeField] float randomness;
    [SerializeField] bool snapping;
    [SerializeField] bool fadeOut;

    Tweener shaketeener; // DOTween�̂��
    Vector3 initPos;    // ��z���̏����ʒu

    // �X�e�[�W�i���o�[
    [Header("�X�e�[�W�i���o�[")]
    [SerializeField] Text StageNo;

    // UI
    [Header("UI")]
    [SerializeField] GameObject UI_Canvas;
    [SerializeField] GameObject Next_UI;
    [SerializeField] GameObject Next_UI_Big;
    [SerializeField] GameObject StageSelect_UI;
    [SerializeField] GameObject StageSelect_UI_Big;
    [SerializeField] Image Stump_UI;
    [SerializeField] Image Photo_UI;

    UI_COMMAND ui_command;

    // �A�j���[�^�ϐ�
    [Header("�A�j���[�^")]
    public Animator stump_animator;
    public Animator Wanted_animator;

    // �A�j���[�V�����p�����[�^
    [System.NonSerialized] public int Stump_Start;
    [System.NonSerialized] public int Stump_end;
    [System.NonSerialized] public int Shake_Start;
    [System.NonSerialized] public int Shake_End;
    int Wanted_SkipAnime;
    int Stump_SkipAnime;

    // �N���A�����N�p
    Sprite[] Stump_sprite;

    // �f�o�b�O�p
    [Header("�ȉ��f�o�b�O�R���\�[��")]
    [SerializeField] bool debug_check;
    [SerializeField][Range(0, 14)] int debug_stageNo;
    [SerializeField] bool debug_cicktime;
    [SerializeField] int debug_cickFlame;

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
        anim_end = false;
        UI_Canvas.SetActive(false);
        ui_command = UI_COMMAND.NextStage;

        // UI�A�N�e�B�u������
        Next_UI.SetActive(false);
        Next_UI_Big.SetActive(true);
        StageSelect_UI.SetActive(true);
        StageSelect_UI_Big.SetActive(false);
        Stump_UI.color = Color.clear;

        initPos = Wanted_Sprite.transform.position;

        // �A�j���p�����[�^�n�b�V��
        AnimetorHash_Reset();

        // Photo�ݒ�
        Photo_Random();

        // �X�e�[�W�i���o�[��UI�Z�b�g
        StageNo_UISet();

    }

    // Update is called once per frame
    void Update()
    {
        // �{�^������������X�L�b�v
        if(Input.GetButton("Fire1") || Input.GetButton("Jump"))
        {
            Wanted_animator.SetBool(Wanted_SkipAnime, true);
            stump_animator.SetBool(Stump_SkipAnime, true);
            stump_animator.SetBool(Stump_end, true);
            UI_Canvas.SetActive(true);
            Stump_UI.color = new Color(1, 1, 1, 1);
        }

        if (anim_end == true)
        {
            stump_animator.SetBool(Stump_Start, true);
            Stump_UI.color = new Color(1, 1, 1, 1);
        }

        // UI����
        if (stump_animator.GetBool(Stump_end) == true && UI_Canvas.activeSelf == false)
        {
            UI_Canvas.SetActive(true);
            Wanted_animator.SetBool(Shake_Start, true);
        }

        // �A�j���[�V�������I����Ă�����UI����\
        if (stump_animator.GetBool(Stump_end) == true)
        {
            // �X�e�B�b�N��
            if (Input.GetAxis("Vertical") > 0.8f)
            {
                ui_command = UI_COMMAND.NextStage;
                Next_UI.SetActive(false);
                Next_UI_Big.SetActive(true);
                StageSelect_UI.SetActive(true);
                StageSelect_UI_Big.SetActive(false);
            }
            // �X�e�B�b�N��
            if (Input.GetAxis("Vertical") < -0.8)
            {
                ui_command = UI_COMMAND.StageSelect;
                Next_UI.SetActive(true);
                Next_UI_Big.SetActive(false);
                StageSelect_UI.SetActive(false);
                StageSelect_UI_Big.SetActive(true);
            }

            switch (ui_command)
            {
                case UI_COMMAND.NextStage:
                    if (Input.GetButton("Jump"))
                    {
                        GameStateManager.LoadNextStage();
                    }
                    break;

                case UI_COMMAND.StageSelect:
                    if (Input.GetButton("Jump"))
                    {
                        SceneManager.LoadScene("StageSelectScene");
                    }
                    break;
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

    void Skip_Animation()
    {

    }
}
