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
        "�u�`���̂����A���̂͂��܂�v\n�`���̊C�������肵�n�A�J�i�^���B\n�`���𖲌������������A��_�C�ɔ�э��񂾁I",
        "�u��F�̊�̏����v\n�����̖��̓��e�B�B�J�i�^���̍ŉ��ɂ�����߂����āA\n����ȃC�J�����ӂ�܂킷�I",
        "�u�@�B�d�|���̊X�v\n���e�B�͎��̂�������߂ăn�O���}���֌��������B\n�n�O���}�����𗘗p���āA�ǂ�ǂ�˂��i�߁I",
        "�u�����˂���X�C�V���E�J�v\n�������󂯂Ȃ���ǂ�ǂ�i�ރ��e�B�B\n���̐�ɂ́A�g�Q�g�Q�̃N���X�^�����҂��󂯂Ă����I",
        "�u�X�C�V���E�J�͂�����v\n�s�v�c�ȃg���l�������������e�B�B\n�[���̍������ނ��̓��A�ɂ́A�s�v�c�ȋ�Ԃ��L�����Ă����I",
        "�u����������X�v\n�������������ꂽ���A���s�X�ɂ���A���Ƃ̂��킳�B\n�ł����s�X�ɂ́A�s�C���Ȃ��킳������悤��......",
        "�u���s�X�̃K���}�������v\n����Ȃ邨������߂Đ^�钆�̋��s�X��T�����Ă���ƁA\n�ǂ�����Ƃ��Ȃ��C�����I�ǂ����T���B���ڂ��o�܂����悤��......",
        "�u���肭��͋���ȁv\n���s�X�̍Ō�̂������ɓ��ꂽ���e�B�B\n���̒���A�����ƂƂ��ɑ傫�ȉe�����e�B�ɔ��肭��I"
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
