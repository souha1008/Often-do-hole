using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectCamera : MonoBehaviour
{
    public static StageSelectCamera instance;

    [SerializeField] private GameObject CenterObj;
    public float MASU_DISTANCE;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void ManualStart()
    {
        Vector3 CenterToVector = (SelectManager.instance.StageObj[SelectManager.instance.NowStage].transform.position - CenterObj.transform.position).normalized;
        Vector3 GoPos = SelectManager.instance.StageObj[SelectManager.instance.NowStage].transform.position + CenterToVector * MASU_DISTANCE;

        //���̏ꏊ����GoPos�ւ̃x�N�g�����擾
        //Vector3 GoAngle = GoPos - this.transform.position;

        transform.position = GoPos;


        //�^�񒆂�����
        transform.LookAt(CenterObj.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //at�_�i�����j���玟�}�X�܂ł̃A���O���𐳋K��
        Vector3 CenterToVector = (SelectManager.instance.StageObj[SelectManager.instance.NowStage].transform.position - CenterObj.transform.position).normalized;
        Vector3 GoPos = SelectManager.instance.StageObj[SelectManager.instance.NowStage].transform.position + CenterToVector * MASU_DISTANCE;

        //���̏ꏊ����GoPos�ւ̃x�N�g�����擾
        //Vector3 GoAngle = GoPos - this.transform.position;

        transform.position = Vector3.Lerp(transform.position, GoPos, 0.1f);


        //�^�񒆂�����
        transform.LookAt(CenterObj.transform.position);

#if false
        // �^�[�Q�b�g�����̃x�N�g�����擾
        Vector3 relativePos = SelectManager.instance.StageObj[SelectManager.instance.NowStage].transform.position - this.transform.position;
        // �������A��]���ɕϊ�
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        // ���݂̉�]���ƁA�^�[�Q�b�g�����̉�]����⊮����
        transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, 0.3f);
#endif
    }

    public void ChangeStage()
    {

    }
}
