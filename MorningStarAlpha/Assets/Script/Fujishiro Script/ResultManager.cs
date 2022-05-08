using System.Collections;
using System.Collections.Generic;
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

    UI_COMMAND ui_command;

    // �A�j���[�^�ϐ�
    [Header("�A�j���[�^")]
    [SerializeField]Animator stump_animator;
    public Animator Wanted_animator;

    // �A�j���[�V�����p�����[�^
    int Stump_Start;
    [System.NonSerialized]public int Shake;

    // ���X�N���v�g�ő��삷��ϐ�
    [System.NonSerialized]public bool Stump_end;

    // �f�o�b�O�p
    [Header("�ȉ��f�o�b�O�R���\�[��")]
    [SerializeField] bool debug_check;
    [SerializeField][Range(0, 14)] int debug_stageNo;

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
    void Start()
    {
        anim_end = false;
        Stump_end = false;
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
        Shake = Animator.StringToHash("Shake");
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

        if(anim_end == true)
        {
            stump_animator.SetBool(Stump_Start, true);
            Stump_UI.color = new Color(1, 1, 1, 1);
        }

        // UI����
        if(Stump_end == true && UI_Canvas.activeSelf == false)
        {
            Wanted_animator.SetBool(Shake, true);
            UI_Canvas.SetActive(true);
        }

        // �A�j���[�V�������I����Ă�����UI����\
        if (Stump_end == true)
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
