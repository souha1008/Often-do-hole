using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Title_p2 : MonoBehaviour
{
    [Header("�q�G�����L�[���A�^�b�`")]
    [Tooltip("PressAny�A�j���[�^�[������")][SerializeField] Animator PressAny_animator;
    [Tooltip("PressAny�A�j���[�V�����I����̃f�����[�V����")][SerializeField] int SceneChange_Frame = 200;
    [Tooltip("���j���[�}�l�[�W���[������")][SerializeField] GameObject MenuManager;
    [Tooltip("PressAnyUI������")][SerializeField] GameObject PressAny_GO;
    [Tooltip("SelectUI������")][SerializeField] GameObject Select_GO;


    [System.NonSerialized] public static Title_p2 instance;

    private bool once_press;

    public bool changeScene;

    // �A�j���[�V�����֌W
    int PushButton;
    int Anim_StateIdle2;
    AnimatorStateInfo Anim_CurrentState_Info;

    // �t���[���J�E���g
    int frame_Count;


    void Awake()
    {
        instance = this;

        StartCoroutine("WaitAwake");
    }

    IEnumerator WaitAwake()
    {
        yield return new WaitForEndOfFrame();
    }

    // Start is called before the first frame update
    void Start()
    {
        once_press = false;
        PlayerMain.instance.mode = new PlayerState_Title();

        frame_Count = 0;

        // �ϐ�������
        changeScene = false;

        // �A�j���[�V�����p�����[�^�擾
        PushButton = Animator.StringToHash("PushButton");
        Anim_StateIdle2 = Animator.StringToHash("PressAny_Idle2anim");

        // Menu�֌W��A�N�e�B�u
        MenuManager.SetActive(false);
        Select_GO.SetActive(false);

        // �T�E���h�Đ�
        SoundManager.Instance.PlaySound("Title_BGM", 0.5f, 0.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Anim_CurrentState_Info = PressAny_animator.GetCurrentAnimatorStateInfo(0);

        // PressAnyButton
        if (Input.GetButton("ButtonA") || Input.GetButton("ButtonB") || Input.GetButton("Button_Select") && once_press == false)
        { 
            once_press = true;
            PressAny_animator.SetBool(PushButton, true);
        }

        if (once_press == true)
        {
            if(CheckCurrent_PressAnyIdle() == true)
            {
                frame_Count++;
            }
            if (frame_Count >= SceneChange_Frame)
            {
                //SceneManager.LoadScene("Menu");
                PressAny_GO.SetActive(false);
                MenuManager.SetActive(true);
                Select_GO.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }
    }

    bool CheckCurrent_PressAnyIdle()
    {
        bool isState = Anim_CurrentState_Info.shortNameHash == Anim_StateIdle2;

        return isState;
    }
}
