using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawOkadaText : MonoBehaviour
{
    private string[] StageNames = {
#if false
        "Stage1",
        "Stage2",
        "Stage3",
        "Stage4",
        "Stage5",
        "Stage6",
        "Stage7",
        "Stage8",
        "Stage9",
        "Stage10",
        "Stage11"
#endif
        "�`���̊C�������肵�n�A�J�i�^���B\n�`���𖲌������������A\n��_�C�ɔ�э��񂾁I",
        "�����̖��̓��e�B�B\n�J�i�^���̍ŉ��ɂ���\n���߂����āA\n����ȃC�J����\n�ӂ�܂킷�I",
        "���e�B�͎���\n��������߂�\n�n�O���}���֌��������B\n�n�O���}�����𗘗p����\n�ǂ�ǂ�˂��i�߁I",
        "�������󂯂Ȃ���\n�ǂ�ǂ�i�ރ��e�B�B\n���̐�ɂ́A�g�Q�g�Q�̃N���X�^����\n�҂��󂯂Ă����I",
        "�s�v�c�ȃg���l����\n���������e�B�B\n�[���̍�������\n���̓��A�ɂ́A\n�s�v�c�ȋ�Ԃ�\n�L�����Ă����I",
        "�������������ꂽ���A���s�X��\n����A���Ƃ̂��킳�B\n�ł����s�X�ɂ́A\n�s�C���Ȃ��킳��\n����悤��......",
        "����Ȃ邨������߂�\n�^�钆�̋��s�X��\n�T�����Ă���ƁA\n�ǂ�����Ƃ��Ȃ�\n�C�����I\n�ǂ����T���B��\n�ڂ��o�܂����悤��...",
        "���s�X�̍Ō�̂����\n��ɓ��ꂽ���e�B�B\n���̒���A�����ƂƂ��ɑ傫�ȉe�����e�B��\n���肭��I"
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SelectManager.instance.CanStart)
            gameObject.GetComponent<TextMeshProUGUI>().text = StageNames[SelectManager.instance.NowSelectStage];
    }
}
