using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �v���C���[�̏�ԊǗ�������C���^�[�t�F�[�X
/// MonoBehaviour�͌p�����Ȃ�
/// </summary>
public class PlayerState
{
    virtual public void InputController() { }�@//�p����ŃR���g���[���[�̓��͂�override����
    virtual public void Move() { }             //�p����ŕ��������i�joverride����

    static public Player player;

    protected const float Movethreshold = 0.8f;
}


/// <summary>
/// �ʂ�ł��Ă��Ȃ����
/// </summary>
public class PlayerStateNone : PlayerState
{
    public override void InputController() { }�@
    public override void Move() { }

    static public Player player;

    protected const float Movethreshold = 0.8f;
}

