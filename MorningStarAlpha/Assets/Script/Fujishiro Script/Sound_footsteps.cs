using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_footsteps : MonoBehaviour
{
    private float volume;
    int play_flame;

    // [SerializeField] SoundManager soundMane;

    SOUND_NO soundNo;

    SOUND_NO old_soundNo;

    int flame_count;

    enum SOUND_NO
    {
        SOUND_1 = 0,
        SOUND_2,
        SOUND_3,
        SOUND_4 = 3
    }

    // Start is called before the first frame update
    void Start()
    {
        flame_count = 0;

        volume = 0.6f;
        play_flame = 30;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Mathf.Abs(PlayerMain.instance.vel.x) > 15.0f)
        {
            volume = 0.4f;
            play_flame = 30;
        }
        else
        {
            volume = 0.3f;
            play_flame = 40;
        }

        flame_count++;
        if (flame_count >= play_flame)
        {
            if (PlayerMain.instance.refState == EnumPlayerState.ON_GROUND)
            {
                if (PlayerMain.instance.animator.GetBool("isRunning"))
                {
                    int random = Random.Range(0, 3 + 1);
                    //Debug.Log("Number:" + random);
                    if (random != (int)old_soundNo)
                    {
                        soundNo = (SOUND_NO)random;
                        switch (soundNo)
                        {
                            case SOUND_NO.SOUND_1:
                                // à»â∫ínñ Ç…ÇÊÇ¡Çƒï™äÚ
                                SoundManager.Instance.PlaySound("sound_10_2_1", volume, 0.0f); // êŒè∞
                                                                                               //Debug.Log("ñ¬Ç¡ÇΩ");
                                break;

                            case SOUND_NO.SOUND_2:
                                SoundManager.Instance.PlaySound("sound_10_2_2", volume, 0.0f);
                                //Debug.Log("ñ¬Ç¡ÇΩ");
                                break;

                            case SOUND_NO.SOUND_3:
                                SoundManager.Instance.PlaySound("sound_10_2_3", volume, 0.0f);
                                //Debug.Log("ñ¬Ç¡ÇΩ");
                                break;

                            case SOUND_NO.SOUND_4:
                                SoundManager.Instance.PlaySound("sound_10_2_4", volume, 0.0f);
                                //Debug.Log("ñ¬Ç¡ÇΩ");
                                break;
                        }
                        old_soundNo = (SOUND_NO)random;
                        flame_count = 0;
                    }
                }
            }
        }
    }
}