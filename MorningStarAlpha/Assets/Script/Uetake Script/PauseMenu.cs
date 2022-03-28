using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// ゲームシーン内のポーズメニュー管理のクラス
/// 
/// </summary>
public class PauseMenu : MonoBehaviour //ポーズメニューキャンバスにアタッチ
{
    [SerializeField]
    private Canvas PauseCanvas;
    [SerializeField, Tooltip("最初に選択されるボタン")] private Selectable FirstSelect;
    private GameObject oldButton;
    private GameObject nowButton;


    private void Awake()
    {
        PauseCanvas.gameObject.SetActive(false);
        oldButton = nowButton = EventSystem.current.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseCanvas.gameObject.activeSelf)　//ポーズメニューがアクティブなら
        {
            nowButton = EventSystem.current.currentSelectedGameObject;
            Debug.Log(EventSystem.current.currentSelectedGameObject);

            if (Object.ReferenceEquals(nowButton, oldButton) == false)
            {
                oldButton.GetComponent<Image>().color = Color.white;
                nowButton.GetComponent<Image>().color = Color.red;
                Debug.Log("change");
            }

            if (Input.GetButtonDown("Button_Select"))
            {
                EndPause();
            }

            oldButton = nowButton;
        }
        else //ポーズメニューが非アクティブなら
        {
            if (Input.GetButtonDown("Button_Select"))
            {
                StartPause();
            }
        }
    }


    void EndPause()
    {
        PauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void StartPause()
    {
        PauseCanvas.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FirstSelect.gameObject);
        Time.timeScale = 0.0f;
        oldButton = nowButton = EventSystem.current.currentSelectedGameObject;
        FirstSelect.gameObject.GetComponent<Image>().color = Color.red;
    }

    public void ClickResume()
    {
        EndPause();
    }

    public void ClickRetry()
    {
        FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
        Time.timeScale = 1.0f;
    }

    public void ClickBackStageSelect()
    {
        EndPause();
    }
}