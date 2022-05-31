using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLogoManager : MonoBehaviour
{
    private float nowTime;
    private bool once;

    [SerializeField] LogoFade logoFade;
    // Start is called before the first frame update
    void Start()
    {
        nowTime = 0.0f;
        once = false;
    }

    // Update is called once per frame
    void Update()
    {
        nowTime += Time.deltaTime;

        if (once == false)
        {

            if (logoFade.VoiceOnce)
            {
                if (GameStateManager.Instance.PressAny())
                {
                    nowTime = 5.0f;
                }
            }


            if (nowTime > 4.0f)
            {
                once = true;
                FadeManager.Instance.FadeStart("Title_part2", FADE_KIND.FADE_STAGECHANGE);
            }
        }
    }
}
