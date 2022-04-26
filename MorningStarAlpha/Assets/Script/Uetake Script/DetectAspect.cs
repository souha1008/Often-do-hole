using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///�@�ʌ���
/// </summary>
public enum Aspect
{
    UP,        //���
    DOWN,      //����
    LEFT,      //����
    RIGHT,   //�E��
    INVALID,   //��O
}

public enum Aspect_8
{
    UP,        //���
    DOWN,      //����
    LEFT,      //����
    RIGHT,     //�E��
    UP_RIGHT,  //�E��
    UP_LEFT,   //����
    DOWN_RIGHT,//�E��
    DOWN_LEFT,

    INVALID,   //��O
}

public static class DetectAspect
{
    /// <summary>
    /// �@���x�N�g���ɂ���Ėʂ̌������擾
    /// </summary>
    /// <param name="vec">�@��</param>
    /// <returns></returns>
    public static Aspect DetectionAspect(Vector3 vec)
    {
        Aspect returnAspect = Aspect.INVALID;
        if (Mathf.Abs(vec.y) > 0.3f) //y�������傫���̂ŏc����
        {
            if (vec.y > 0)
            {
                returnAspect = Aspect.UP;
            }
            else
            {
                returnAspect = Aspect.DOWN;
            }
        }
        else if (Mathf.Abs(vec.x) > 0.3f) //x�������傫���̂ŉ�����
        {
            if (vec.x > 0)
            {
                returnAspect = Aspect.RIGHT;
            }
            else
            {
                returnAspect = Aspect.LEFT;
            }
        }
        else
        {
            returnAspect = Aspect.INVALID;
            Debug.LogWarning("�ڐG�ʂ̖@�����΂߂̉\��������܂�");
        }

        return returnAspect;
    }

    /// <summary>
    /// �_���R���C�_�[�ɑ΂��Ăǂ̈ʒu�ɂ��邩�𒲂ׂ�
    /// </summary>
    /// <param name="col">�v�Z�Ώۂ̃R���C�_�[(�{�b�N�X)</param>
    /// <param name="pos">�Ώ̂̓_</param>
    /// <returns></returns>
    public static Aspect_8 Detection8Pos(BoxCollider collider ,Vector3 pos)
    {
        Aspect_8 returnAsp = Aspect_8.UP;
      
        if (collider.bounds.max.y < pos.y) //�㑤
        {
            if (collider.bounds.max.x < pos.x)�@//�E��
            {
                returnAsp = Aspect_8.UP_RIGHT;
            }
            else if (collider.bounds.min.x > pos.x) //����
            {
                returnAsp = Aspect_8.UP_LEFT;
            }
            else�@//x���͐^��
            {
                returnAsp = Aspect_8.UP;
            }
        }
        else if (collider.bounds.min.y > pos.y) //����
        {
            if (collider.bounds.max.x < pos.x)�@//�E��
            {
                returnAsp = Aspect_8.DOWN_RIGHT;
            }
            else if (collider.bounds.min.x > pos.x) //����
            {
                returnAsp = Aspect_8.DOWN_LEFT;
            }
            else //x���͐^��
            {
                returnAsp = Aspect_8.DOWN;
            }
        }
        else //y���͐^��
        {
            if (collider.bounds.max.x < pos.x)�@//�E��
            {
                returnAsp = Aspect_8.RIGHT;
            }
            else if (collider.bounds.min.x > pos.x) //����
            {
                returnAsp = Aspect_8.LEFT;
            }
            else�@//x���͐^�񒆁i�߂荞��ł���j
            {
                returnAsp = Aspect_8.INVALID;
            }
        }

        Debug.Log("8direction : " + returnAsp);


        return returnAsp;
    }

}

