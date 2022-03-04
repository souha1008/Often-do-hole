using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// プレイヤーの状態管理をするインターフェース
/// MonoBehaviourは継承しない
/// </summary>
public class PlayerState
{
    virtual public void InputController() { }　//継承先でコントローラーの入力をoverrideする
    virtual public void Move() { }             //継承先で物理挙動（）overrideする

    static public Player player;

    protected const float Movethreshold = 0.8f;
}


/// <summary>
/// 玉を打っていない状態
/// </summary>
public class PlayerStateNone : PlayerState
{
    public override void InputController() { }　
    public override void Move() { }

    static public Player player;

    protected const float Movethreshold = 0.8f;
}

