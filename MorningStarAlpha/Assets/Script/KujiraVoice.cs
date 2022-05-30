using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KujiraVoice : MonoBehaviour
{
    const float VoiceInterval = 10.0f;
    float VoiceTimer;

    // Start is called before the first frame update
    void Start()
    {
        VoiceTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.GetGameState() == GAME_STATE.PLAY)
        {
            VoiceTimer += Time.deltaTime;

            if(VoiceTimer > VoiceInterval)
            {
                int seNum = Random.Range(0, 2);

                switch (seNum)
                {
                    case 0:
                        SoundManager.Instance.PlaySound("sound_66", 0.8f);
                        break;

                    case 1:
                        SoundManager.Instance.PlaySound("sound_67", 0.8f);
                        break;
                }
                VoiceTimer = 0;
            }
        }
    }
}
