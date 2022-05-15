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

    // UIÉäÉ\Å[ÉXï€ë∂
    Sprite White_StageSelect_UI;
    Sprite White_Option_UI;
    Sprite White_Exit_UI;
    Sprite Glay_StageSelect_UI;
    Sprite Glay_Option_UI;
    Sprite Glay_Exit_UI;

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
        nowSelect = NOWSELECT.StagesSelect;
        ResourceSave();
        SpriteChange();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetAxis("Vertical") <= -0.8 && input_stick == false)
        {
            nowSelect++;
            SpriteChange();
            if(nowSelect > (NOWSELECT)2)
            {
                nowSelect = NOWSELECT.Exit;
            }
            input_stick = true;
        }
        if(Input.GetAxis("Vertical") >= 0.8 && input_stick == false)
        {
            nowSelect--;
            SpriteChange();
            if (nowSelect < 0)
            {
                nowSelect = NOWSELECT.StagesSelect;
            }
            input_stick = true;
        }
        if(Input.GetAxis("Vertical") < 0.1 && Input.GetAxis("Vertical") > -0.1)
        {
            input_stick = false;
        }

        switch (nowSelect)
        {
            case NOWSELECT.StagesSelect:
                if(Input.GetButton("Jump"))
                {
                    SceneManager.LoadScene("StageSelectScene");
                }
                break;

            case NOWSELECT.Option:
                if(Input.GetButton("Jump"))
                {

                }
                break;

            case NOWSELECT.Exit:
                if(Input.GetButton("Jump"))
                {
                    Application.Quit();
                }
                break;
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
}
