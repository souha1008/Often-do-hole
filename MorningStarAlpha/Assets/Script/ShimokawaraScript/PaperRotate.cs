using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperRotate : MonoBehaviour
{
    public GameObject Omote;
    public GameObject Ura;

    public GameObject ImageStory;
    public GameObject ImageTime;

    float Y_Rotate = 0;

    float MIN_ANGLE = 0.0f;
    float MAX_ANGLE = 180.0f;
    float ADD_ANGLE = 3.0f;


    public bool GoOmote = true;

    public float StoryAlpha = 1.0f;
    public float TimeAlpha = 0.0f; 

    // Start is called before the first frame update
    void Start()
    {
        Y_Rotate = 0;
        GoOmote = true;

        StoryAlpha = 1.0f;
        TimeAlpha = 0.0f;

        ImageStory.GetComponent<Image>().color = new Color(1, 1, 1, StoryAlpha);
        ImageTime.GetComponent<Image>().color = new Color(1, 1, 1, TimeAlpha);
    }

    void FixedUpdate()
    {
        

        if(GoOmote)
        {
            Y_Rotate = Mathf.Max(MIN_ANGLE, Y_Rotate - ADD_ANGLE);
            if (TimeAlpha > 0.0f)
            {
                TimeAlpha = Mathf.Max(0.0f, TimeAlpha - 0.1f);
            }
            else
            {
                StoryAlpha = Mathf.Min(1.0f, StoryAlpha + 0.1f);
            }
        }
        else
        {
            Y_Rotate = Mathf.Min(MAX_ANGLE, Y_Rotate + ADD_ANGLE);
            if (StoryAlpha > 0.0f)
            {
                StoryAlpha = Mathf.Max(0.0f, StoryAlpha - 0.1f);
            }
            else
            {
                TimeAlpha = Mathf.Min(1.0f, TimeAlpha + 0.1f);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        //R“ü—Í‚³‚ê‚½‚ç
        if (Input.GetButtonDown("Button_R"))
        {
            GoOmote = !GoOmote;
        }

        Omote.GetComponent<RectTransform>().localEulerAngles =
            new Vector3(Omote.GetComponent<RectTransform>().localRotation.x,
            Y_Rotate ,
            Omote.GetComponent<RectTransform>().localRotation.z);

        Ura.GetComponent<RectTransform>().localEulerAngles =
            new Vector3(Ura.GetComponent<RectTransform>().localRotation.x,
            Y_Rotate,
            Ura.GetComponent<RectTransform>().localRotation.z);

        if (Y_Rotate <= 90)
        {
            Omote.SetActive(true);
            Ura.SetActive(false);
        }
        else
        {
            Omote.SetActive(false);
            Ura.SetActive(true);
        }

        ImageStory.GetComponent<Image>().color = new Color(1, 1, 1, StoryAlpha);
        ImageTime.GetComponent<Image>().color = new Color(1, 1, 1, TimeAlpha);
    }
}
