using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.AddressableAssets;

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

    UI_COMMAND ui_command;

    // �A�j���[�^�ϐ�
    [Header("�A�j���[�^")]
    public Animator stump_animator;
    public Animator Wanted_animator;

    // �A�j���[�V�����p�����[�^
    [System.NonSerialized]public int Stump_Start;
    [System.NonSerialized]public int Stump_end;
    [System.NonSerialized]public int Shake_Start;
    [System.NonSerialized] public int Shake_End;

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
        Stump_UI.color = new Color(0, 0, 0, 0);

        initPos = Wanted_Sprite.transform.position;

        // �A�j���p�����[�^�n�b�V��
        Stump_Start = Animator.StringToHash("Stump_Start");
        Stump_end = Animator.StringToHash("Stump_End");
        Shake_Start = Animator.StringToHash("Shake_Start");
        Shake_End = Animator.StringToHash("Shake_End");
    }

    // Update is called once per frame
    void Update()
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
                    if (SpriteManager.Instance.LoadTexture("Stump_S") != null)
                    {
                        Debug.Log(SpriteManager.Instance.LoadTexture("Stump_S"));
                        Stump_sprite[0] = SpriteManager.Instance.LoadTexture("Stump_S");
                    }
                        Debug.Log(Stump_sprite[0]);
                        Stump_UI.sprite = Stump_sprite[0];
                        break;

                    case ClearRank.Rank_A:
                        Stump_sprite[1] = SpriteManager.Instance.LoadTexture("Stump_A");
                        Debug.Log(Stump_sprite[1]);
                        Stump_UI.sprite = Stump_sprite[1];
                        break;

                    case ClearRank.Rank_B:
                        Stump_sprite[2] = SpriteManager.Instance.LoadTexture("Stump_B");
                        Debug.Log(Stump_sprite[2]);
                        Stump_UI.sprite = Stump_sprite[2];
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
            //if (stump_animator.GetBool(Shake) == true && UI_Canvas.activeSelf == false)
            //{
            //    UI_Canvas.SetActive(true);
            //}
            //if (UI_Canvas.activeSelf == false && stump_animator.GetBool(Shake_End) == true)
            //{
            //    bool end = ResultManager.instance.Wanted_animator.GetBool(ResultManager.instance.Shake_End);
            //    Debug.Log("Shake_End:" + end);
            //    UI_Canvas.SetActive(true);
            //}

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

    void Wanted_Shake(float duration, float strength, int vibrato, float randomness, bool fadeout)
    {
        if(shaketeener != null)
        {
            shaketeener.Kill();
            Wanted_Sprite.transform.position = initPos;
        }

        shaketeener = Wanted_Sprite.rectTransform.DOShakePosition(duration, strength, vibrato, randomness, false, fadeout);
    }
}
