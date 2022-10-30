using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StageTitleText : MonoBehaviour
{
    private TextMeshProUGUI TMProObj;   // �e�L�X�g�I�u�W�F�N�g
    //private Image ImageObj;   // �C���[�W�I�u�W�F�N�g
    //[SerializeField] private Sprite[] StageTitleSprites; // �X�e�[�W�^�C�g���C���[�W


    private string[] TitleText =
    {
        "�`���̎n�܂�",
        "StageTitle2",
        "StageTitle3",
        "StageTitle4",
        "StageTitle5",
        "StageTitle6",
        "StageTitle7",
        "StageTitle8",
        "StageTitle9"
    };

    // Start is called before the first frame update
    void Start()
    {
        // �擾
        TMProObj = GetComponent<TextMeshProUGUI>();
        //ImageObj = GetComponent<Image>();

        if (TMProObj != null)
        {
            TMProObj.text = TitleText[GameStateManager.GetNowStage()];  // �����ύX
        }
    }
}
