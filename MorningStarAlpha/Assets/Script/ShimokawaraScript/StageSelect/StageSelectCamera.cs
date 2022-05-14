using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectCamera : MonoBehaviour
{
    public static StageSelectCamera instance;

    //[SerializeField] private GameObject CenterObj;
    public float MASU_DISTANCE_Z;
    public float MASU_DISTANCE_Y;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

#if false
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
#endif

    public void ManualStart()
    {
        Vector3 GoPos = SelectManager.instance.StageObj[SelectManager.instance.NowSelectStage].transform.position;
        GoPos.z -= MASU_DISTANCE_Z;
        GoPos.y += MASU_DISTANCE_Y;

        transform.position = GoPos;
        transform.LookAt(SelectManager.instance.StageObj[SelectManager.instance.NowSelectStage].transform.position);

#if false
        Vector3 CenterToVector = (SelectManager.instance.StageObj[SelectManager.instance.NowSelectStage].transform.position - CenterObj.transform.position).normalized;
        Vector3 GoPos = SelectManager.instance.StageObj[SelectManager.instance.NowSelectStage].transform.position - CenterToVector * MASU_DISTANCE;

        //���̏ꏊ����GoPos�ւ̃x�N�g�����擾
        //Vector3 GoAngle = GoPos - this.transform.position;

        transform.position = GoPos;


        //�v���C���[�̎��_�������
        Vector3 PtoCameraVector = transform.position - CenterObj.transform.position;


        transform.rotation = Quaternion.LookRotation(PtoCameraVector);
#endif
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 GoPos = SelectManager.instance.StageObj[SelectManager.instance.NowSelectStage].transform.position;
        GoPos.z -= MASU_DISTANCE_Z;
        GoPos.y += MASU_DISTANCE_Y;

        //transform.position = GoPos;
        //transform.rotation = Quaternion.FromToRotation(GoPos, SelectManager.instance.StageObj[SelectManager.instance.NowSelectStage].transform.position);

        //���̏ꏊ����GoPos�ւ̃x�N�g�����擾
        //Vector3 GoAngle = GoPos - this.transform.position;

        transform.position = Vector3.Lerp(transform.position, GoPos, 0.1f);
        //�s�b�^�������΃X�^�[�g�\
        if ((transform.position - GoPos).magnitude < 0.1f)
        {
            SelectManager.instance.SetCanStart(true);
            //Debug.Log("�҂�����");
        }

        ////�v���C���[�̎��_�������
        //Vector3 PtoCameraVector = transform.position - CenterObj.transform.position;


        //transform.rotation = Quaternion.LookRotation(PtoCameraVector);

#if false


        //at�_�i�����j���玟�}�X�܂ł̃A���O���𐳋K��
        Vector3 CenterToVector = (SelectManager.instance.StageObj[SelectManager.instance.NowSelectStage].transform.position - CenterObj.transform.position).normalized;
        Vector3 GoPos = SelectManager.instance.StageObj[SelectManager.instance.NowSelectStage].transform.position - CenterToVector * MASU_DISTANCE;

        //���̏ꏊ����GoPos�ւ̃x�N�g�����擾
        //Vector3 GoAngle = GoPos - this.transform.position;

        transform.position = Vector3.Lerp(transform.position, GoPos, 0.1f);
        //�s�b�^�������΃X�^�[�g�\
        if((transform.position - GoPos).magnitude < 0.1f)
        {
            SelectManager.instance.SetCanStart(true);
            //Debug.Log("�҂�����");
        }

        //�v���C���[�̎��_�������
        Vector3 PtoCameraVector = transform.position - CenterObj.transform.position;


        transform.rotation = Quaternion.LookRotation(PtoCameraVector);

#if false
        // �^�[�Q�b�g�����̃x�N�g�����擾
        Vector3 relativePos = SelectManager.instance.StageObj[SelectManager.instance.NowStage].transform.position - this.transform.position;
        // �������A��]���ɕϊ�
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        // ���݂̉�]���ƁA�^�[�Q�b�g�����̉�]����⊮����
        transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, 0.3f);
#endif
#endif
    }

    public void ChangeStage()
    {

    }
}
