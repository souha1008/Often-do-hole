using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageTitleImage : MonoBehaviour
{
    private TextMeshProUGUI TMProObj;   // �e�L�X�g�I�u�W�F�N�g

    // Start is called before the first frame update
    void Start()
    {
        // �擾
        TMProObj = GetComponent<TextMeshProUGUI>();

        if (TMProObj != null)
        {
            TMProObj.text = GameStateManager.GetNowStage().ToString();  // �����ύX
        }    
    }
}
