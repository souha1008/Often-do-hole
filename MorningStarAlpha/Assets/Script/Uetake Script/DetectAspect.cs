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

}

