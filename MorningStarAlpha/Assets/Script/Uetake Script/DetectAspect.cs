using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///　面向き
/// </summary>
public enum Aspect
{
    UP,        //上面
    DOWN,      //下面
    LEFT,      //左面
    RIGHT,   //右面
    INVALID,   //例外
}

public enum Aspect_8
{
    UP,        //上面
    DOWN,      //下面
    LEFT,      //左面
    RIGHT,     //右面
    UP_RIGHT,  //右上
    UP_LEFT,   //左上
    DOWN_RIGHT,//右下
    DOWN_LEFT,

    INVALID,   //例外
}

public static class DetectAspect
{
    /// <summary>
    /// 法線ベクトルによって面の向きを取得
    /// </summary>
    /// <param name="vec">法線</param>
    /// <returns></returns>
    public static Aspect DetectionAspect(Vector3 vec)
    {
        Aspect returnAspect = Aspect.INVALID;
        if (Mathf.Abs(vec.y) > 0.3f) //y成分が大きいので縦向き
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
        else if (Mathf.Abs(vec.x) > 0.3f) //x成分が大きいので横向き
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
            Debug.LogWarning("接触面の法線が斜めの可能性があります");
        }

        return returnAspect;
    }

    /// <summary>
    /// 点がコライダーに対してどの位置にあるかを調べる
    /// </summary>
    /// <param name="col">計算対象のコライダー(ボックス)</param>
    /// <param name="pos">対称の点</param>
    /// <returns></returns>
    public static Aspect_8 Detection8Pos(BoxCollider collider ,Vector3 pos)
    {
        Aspect_8 returnAsp = Aspect_8.UP;
      
        if (collider.bounds.max.y < pos.y) //上側
        {
            if (collider.bounds.max.x < pos.x)　//右側
            {
                returnAsp = Aspect_8.UP_RIGHT;
            }
            else if (collider.bounds.min.x > pos.x) //左側
            {
                returnAsp = Aspect_8.UP_LEFT;
            }
            else　//x軸は真ん中
            {
                returnAsp = Aspect_8.UP;
            }
        }
        else if (collider.bounds.min.y > pos.y) //下側
        {
            if (collider.bounds.max.x < pos.x)　//右側
            {
                returnAsp = Aspect_8.DOWN_RIGHT;
            }
            else if (collider.bounds.min.x > pos.x) //左側
            {
                returnAsp = Aspect_8.DOWN_LEFT;
            }
            else //x軸は真ん中
            {
                returnAsp = Aspect_8.DOWN;
            }
        }
        else //y軸は真ん中
        {
            if (collider.bounds.max.x < pos.x)　//右側
            {
                returnAsp = Aspect_8.RIGHT;
            }
            else if (collider.bounds.min.x > pos.x) //左側
            {
                returnAsp = Aspect_8.LEFT;
            }
            else　//x軸は真ん中（めり込んでいる）
            {
                returnAsp = Aspect_8.INVALID;
            }
        }

        Debug.Log("8direction : " + returnAsp);


        return returnAsp;
    }

}

