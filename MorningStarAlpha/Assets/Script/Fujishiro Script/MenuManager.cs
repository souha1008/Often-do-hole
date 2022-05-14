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

    [SerializeField] Color Glay;

    Color White = new Color(1.0f, 1.0f, 1.0f, 1.0f);

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
        ColorChange();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetAxis("Vertical") <= -0.8 && input_stick == false)
        {
            nowSelect++;
            ColorChange();
            if(nowSelect > (NOWSELECT)2)
            {
                nowSelect = NOWSELECT.Exit;
            }
            input_stick = true;
        }
        if(Input.GetAxis("Vertical") >= 0.8 && input_stick == false)
        {
            nowSelect--;
            ColorChange();
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
                    SceneManager.LoadScene("StageSelect");
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

    void ColorChange()
    {
        switch (nowSelect)
        {
            case NOWSELECT.StagesSelect:
                StageSelect_UI.color = White;
                Option_UI.color = Glay;
                Exit_UI.color = Glay;
                break;

            case NOWSELECT.Option:
                StageSelect_UI.color = Glay;
                Option_UI.color = White;
                Exit_UI.color = Glay;
                break;

            case NOWSELECT.Exit:
                StageSelect_UI.color = Glay;
                Option_UI.color = Glay;
                Exit_UI.color = White;
                break;
        }
    }

}
