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
        "���̂͂��܂�",
        "��F�̊�̏���",
        "�@�B�d�|���̊X",
        "�����˂���X�C�V���E�J",
        "�X�C�V���E�J�͂������",
        "����������X",
        "���s�X�̃K���}������",
        "���肭��͋����...",
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
