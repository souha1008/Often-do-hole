using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Aspect
{
    UP,        //上面
    DOWN,      //下面
    LEFT,      //左面
    RIGHT,   //右面
    INVALID,   //例外
}

public static class DetectAspect
{

    /// <summary>
    ///　面向き
    /// </summary>


    /// <summary>
    /// 法線ベクトルによって面の向きを取得
    /// </summary>
    /// <param name="vec">法線</param>
    /// <returns></returns>
    public static Aspect DetetAspect(Vector3 vec)
    {
        Aspect returnAspect = Aspect.INVALID;
        if (Mathf.Abs(vec.y) > 0.5f) //y成分が大きいので縦向き
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
        else if (Mathf.Abs(vec.x) > 0.5f) //x成分が大きいので横向き
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
            Debug.LogError("接触面の法線が斜めの可能性があります");
        }

        return returnAspect;
    }

}

