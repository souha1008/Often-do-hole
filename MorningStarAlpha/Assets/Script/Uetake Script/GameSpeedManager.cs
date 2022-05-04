using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedManager : SingletonMonoBehaviour<GameSpeedManager>
{
    // Start is called before the first frame update
    private float TimeCounter;
    private bool isHitStop;
    private float stopTime;

    void Awake()
    {
        Time.timeScale = 1.0f;
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない

        //ヒットストップ
        TimeCounter = 0;
        isHitStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.GetGameState() == GAME_STATE.PLAY)
        {
            if (isHitStop)
            {
                TimeCounter += 1.0f / 60;
                Debug.Log("F" + Time.fixedDeltaTime);
                Debug.Log("N" + Time.deltaTime);

                if (TimeCounter > stopTime)
                {
                    isHitStop = false;
                    Time.timeScale = 1;
                    TimeCounter = 0;
                }
            }
        }
    }

    public void StartHitStop()
    {
        isHitStop = true;
        TimeCounter = 0;
        Time.timeScale = 0;

        stopTime = 0.2f;
    }

    public void StartHitStop(float StopSecond)
    {
        isHitStop = true;
        TimeCounter = 0;
        Time.timeScale = 0;

        stopTime = StopSecond;
    }
}
