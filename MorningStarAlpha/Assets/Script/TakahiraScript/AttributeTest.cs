using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeTest : MonoBehaviour
{
    //+++++++++++++++++++++++++++++++++++++++++++++
    // �A�g���r���[�g�̋@�\�e�X�g�X�N���v�g(�g����)
    //+++++++++++++++++++++++++++++++++++++++++++++


    //==================
    // �\�����Ȃ�
    //==================
    [HideInInspector]
    public float NoOutput = 0.0f;

    //==================
    // �\������
    //==================
    [SerializeField]
    private float Output = 0.0f;

    //==================
    // ���x���̕ύX
    //==================
    [Label("���x����")]
    public float Label = 0.0f;

    //==================
    // �w�b�_�[�̒ǉ�
    //==================
    [Header("�w�b�_�[��")]
    public float Header = 0.0f;

    //==================
    // �󔒂̒ǉ�
    //==================
    [Space(50.0f)] // int, float�^


    //==================
    // �q���g�̕\��(�J�[�\�������킹��ƃ`�b�v�X���ł�)
    //==================
    [Tooltip("�q���g��")]
    public float Tooltip = 0.0f;

    //==================
    // �X���C�_�[�̕\��
    //==================
    [Range(0.0f, 100.0f)] // int, float�^
    public float Range = 0.0f;

    //==================
    // ���s�ǉ�
    //==================
    [Multiline(10)] // int�^
    public string Multiline; // string�^

    //==================
    // �R���e�L�X�g���j���[�ǉ�(�E�N���b�N�ł��炩���ߗp�ӂ����֐��E���\�b�h�����s�o����)
    //==================
    [ContextMenuItem("Random", "RandomNumber")] // "���j���[�̖��O", "�֐���"
    public int speed;

    //==================
    // �e�L�X�g�ǉ�
    //==================
    [TextArea(1, 10)] // "�ŏ��\���s��", "�X�N���[���o�[�o��܂ł̍s��"
    public string Text;



    void RandomNumber()
    {
        speed = Random.Range(0, 1000);
    }

    private void Start()
    {
        NoOutput = Output;
    }
}
