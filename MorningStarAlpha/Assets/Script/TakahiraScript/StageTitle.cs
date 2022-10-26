using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTitle : MonoBehaviour
{
    [SerializeField] private float TitleTime = 2.5f;
    private float NowTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        NowTime = TitleTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (NowTime <= 0.0f || FadeManager.GetNowState() != FADE_STATE.FADE_NONE)
            return;


        NowTime -= Time.fixedDeltaTime;

        if (NowTime <= 0.0f)
            GameStateManager.LoadStage(GameStateManager.GetNowStage());
    }
}
