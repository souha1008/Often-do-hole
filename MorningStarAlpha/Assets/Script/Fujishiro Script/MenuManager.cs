using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // UI
    [SerializeField] Image StageSelect_UI;
    [SerializeField] Image Option_UI;
    [SerializeField] Image Exit_UI;
    [SerializeField] OptionManager optionManager;

    // UIÉäÉ\Å[ÉXï€ë∂
    Sprite White_StageSelect_UI;
    Sprite White_Option_UI;
    Sprite White_Exit_UI;
    Sprite Glay_StageSelect_UI;
    Sprite Glay_Option_UI;
    Sprite Glay_Exit_UI;

    private bool MenuFlag = true;
    private bool OnceFlag = false;

    enum NOWSELECT
    {
        StagesSelect = 0,
        Option,
        Exit,
    }

    NOWSELECT nowSelect;

    bool input_stick;


    // Start is called before the first frame update
    void Start()
    {
        MenuFlag = true;
        OnceFlag = false;
        nowSelect = NOWSELECT.StagesSelect;
        ResourceSave();
        SpriteChange();
    }

    // Update is called once per frame
    void Update()
    {
        if (MenuFlag)
        {
            if (Input.GetAxis("Vertical") <= -0.8 && input_stick == false)
            {
                nowSelect++;
                SpriteChange();
                if (nowSelect > (NOWSELECT)2)
                {
                    nowSelect = NOWSELECT.Exit;
                }
                input_stick = true;
            }
            if (Input.GetAxis("Vertical") >= 0.8 && input_stick == false)
            {
                nowSelect--;
                SpriteChange();
                if (nowSelect < 0)
                {
                    nowSelect = NOWSELECT.StagesSelect;
                }
                input_stick = true;
            }
            if (Input.GetAxis("Vertical") < 0.1 && Input.GetAxis("Vertical") > -0.1)
            {
                input_stick = false;
            }

            switch (nowSelect)
            {
                case NOWSELECT.StagesSelect:
                    if (Input.GetButtonDown("Jump") && OnceFlag)
                    {
                        GameStateManager.LoadStageSelect();
                    }
                    break;

                case NOWSELECT.Option:
                    if (Input.GetButtonDown("Jump") && OnceFlag)
                    {
                        optionManager.StartPause();
                    }
                    break;

                case NOWSELECT.Exit:
                    if (Input.GetButtonDown("Jump") && OnceFlag)
                    {
                        Application.Quit();
                    }
                    break;
            }
            OnceFlag = true;
        }
    }

    void SpriteChange()
    {
        switch (nowSelect)
        {
            case NOWSELECT.StagesSelect:
                StageSelect_UI.sprite = White_StageSelect_UI;
                Option_UI.sprite = Glay_Option_UI;
                Exit_UI.sprite = Glay_Exit_UI;
                break;

            case NOWSELECT.Option:
                StageSelect_UI.sprite = Glay_StageSelect_UI;
                Option_UI.sprite = White_Option_UI;
                Exit_UI.sprite = Glay_Exit_UI;
                break;

            case NOWSELECT.Exit:
                StageSelect_UI.sprite = Glay_StageSelect_UI;
                Option_UI.sprite = Glay_Option_UI;
                Exit_UI.sprite = White_Exit_UI;
                break;
        }
    }

    void ResourceSave()
    {
        White_StageSelect_UI = Resources.Load<Sprite>("Sprite/UI/Menu/01_stageselect_btn");
        White_Option_UI = Resources.Load<Sprite>("Sprite/UI/Menu/01_option_btn");
        White_Exit_UI = Resources.Load<Sprite>("Sprite/UI/Menu/01_exit_btn");

        Glay_StageSelect_UI = Resources.Load<Sprite>("Sprite/UI/Menu/01_stageselect2_btn");
        Glay_Option_UI = Resources.Load<Sprite>("Sprite/UI/Menu/01_option2_btn");
        Glay_Exit_UI = Resources.Load<Sprite>("Sprite/UI/Menu/01_exit2_btn");
    }

    public void OnMenu()
    {
        MenuFlag = true;
        StageSelect_UI.enabled = true;
        Option_UI.enabled = true;
        Exit_UI.enabled = true;
        OnceFlag = false;
    }

    public void OffMenu()
    {
        MenuFlag = false;
        StageSelect_UI.enabled = false;
        Option_UI.enabled = false;
        Exit_UI.enabled = false;
    }
}
