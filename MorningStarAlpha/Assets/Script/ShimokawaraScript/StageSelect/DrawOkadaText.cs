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
        "�`���̊C�������肵�n�A�J�i�^���B\n�`���𖲌������������A��_�C�ɔ�э��񂾁I",
        "�����̖��̓��e�B�B�J�i�^���̍ŉ��ɂ�����߂����āA\n����ȃC�J�����ӂ�܂킷�I",
        "���e�B�͎��̂�������߂ăn�O���}���֌��������B\n�n�O���}�����𗘗p���āA�ǂ�ǂ�˂��i�߁I",
        "�������󂯂Ȃ���ǂ�ǂ�i�ރ��e�B�B\n���̐�ɂ́A�g�Q�g�Q�̃N���X�^�����҂��󂯂Ă����I",
        "�s�v�c�ȃg���l�������������e�B�B\n�[���̍������ނ��̓��A�ɂ́A�s�v�c�ȋ�Ԃ��L�����Ă����I",
        "�������������ꂽ���A���s�X�ɂ���A���Ƃ̂��킳�B\n�ł����s�X�ɂ́A�s�C���Ȃ��킳������悤��......",
        "����Ȃ邨������߂Đ^�钆�̋��s�X��T�����Ă���ƁA\n�ǂ�����Ƃ��Ȃ��C�����I�ǂ����T���B���ڂ��o�܂����悤��......",
        "���s�X�̍Ō�̂������ɓ��ꂽ���e�B�B\n���̒���A�����ƂƂ��ɑ傫�ȉe�����e�B�ɔ��肭��I"
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
