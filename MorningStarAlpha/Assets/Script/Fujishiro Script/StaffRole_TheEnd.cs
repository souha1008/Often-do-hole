using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffRole_TheEnd : MonoBehaviour
{
    private bool once = false;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlaySound("Ending", 0.6f, AudioReverbPreset.City);
        once = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameStateManager.Instance.PressAny())
        //{
        //    if (once == false)
        //    {
        //        // âπí‚é~
        //        SoundManager.Instance.FadeSound(SOUND_FADE_TYPE.OUT, 1.0f, 0.0f, true);
        //        FadeManager.Instance.FadeStart("Title_part2", FADE_KIND.FADE_SCENECHANGE);
        //        once = true;
        //    }
        //}
    }
}
