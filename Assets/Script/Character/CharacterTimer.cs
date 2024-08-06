using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターが使うタイマーをまとめたクラス
/// </summary>
public class CharacterTimerController : TimerController
{
    //重力無効タイマー
    protected TimeCountDown timer_NoGravity;
    public TimeCountDown Timer_NoGravity { get { return timer_NoGravity; } set { timer_NoGravity = value; } }
    //硬直時間
    protected TimeCountDown timer_StopMove;
    public TimeCountDown Timer_StopMove { get { return timer_StopMove; } set { timer_StopMove = value; } }
    //クールダウン
    protected TimeCountDown timer_CoolDown;
    //三段ジャンプタイマーカウント
    protected TimeCountDown timer_BurstJump;
    public TimeCountDown GetTimer_BurstJump() { return timer_BurstJump; }
    protected bool burstJump = false;
    public bool BurstJumpFlag { get { return burstJump; } set { burstJump = value; } }
    //ジャンプ入力クールダウンタイマー
    protected TimeCountDown timer_JumpNoInput;
    public TimeCountDown Timer_JumpNoInput { get { return timer_JumpNoInput; } set { timer_JumpNoInput = value; } }
    //ロングジャンプ可能タイマー
    protected TimeCountDown timer_LongJump;
    public TimeCountDown Timer_LongJump { get { return timer_LongJump; } set { timer_LongJump = value; } }
    //連撃のカウント
    protected TimeCountDown timer_BurstAttack;
    public TimeCountDown Timer_BurstAttack { get { return timer_BurstAttack; } set { timer_BurstAttack = value; } }
    //回転攻撃準備中カウント
    protected TimeCountDown timer_ReadySpinAttack;
    public TimeCountDown Timer_ReadySpinAttack { get { return timer_ReadySpinAttack; } set { timer_ReadySpinAttack = value; } }
    //回転攻撃中カウント
    protected TimeCountDown timer_SpinAttack;
    public TimeCountDown Timer_SpinAttack { get { return timer_SpinAttack; } set { timer_SpinAttack = value; } }
    //攻撃時に前に前進させるカウント
    protected TimeCountDown timer_ForwardAccele;
    public TimeCountDown Timer_ForwardAccele { get { return timer_ForwardAccele; } set { timer_ForwardAccele = value; } }
    private TimeCountDown timer_CruchAttack;
    public TimeCountDown Timer_CruchAttack { get { return timer_CruchAttack; } set { timer_CruchAttack = value; } }
    //ガードのカウント
    protected TimeCountDown timer_Guard;
    //無敵時間のカウント
    protected TimeCountDown timer_Invincible;
    public TimeCountDown GetTimer_Invincible() {  return timer_Invincible; }
    //死後硬直カウント
    protected TimeCountDown timer_DieWait;
    public TimeCountDown GetTimer_DieWait() {  return timer_DieWait; }
    //回避カウント
    protected TimeCountDown timer_Avoid;
    public TimeCountDown Timer_Avoid { get { return timer_Avoid; } set { timer_Avoid = value; } }

    protected TimeCountDown timer_StepJumpCoolDown;
    public TimeCountDown Timer_StepJumpCoolDown { get { return timer_StepJumpCoolDown; } set { timer_StepJumpCoolDown = value; } }
    protected TimeCountDown timer_WallGlabStoper;
    public TimeCountDown Timer_WallGlabStoper { get { return timer_WallGlabStoper; } set { timer_WallGlabStoper = value; } }
    protected TimeCountDown timer_StopWallAction;
    public TimeCountDown Timer_StopWallAction { get { return timer_StopWallAction; } set { timer_StopWallAction = value; } }


    //非巡回時の移動クールダウンタイマー
    protected TimeCountDown timer_Wait;
    public TimeCountDown GetTimer_Wait() {  return timer_Wait; }

    private TimeCountDown timer_Move;
    public TimeCountDown GetTimer_Move() {  return timer_Move; }

    public override void InitializeAssignTimer()
    {
        timer_Invincible = new TimeCountDown();
        timer_DieWait = new TimeCountDown();
        timer_Avoid = new TimeCountDown();
        timer_NoGravity = new TimeCountDown();
        timer_StopMove = new TimeCountDown();
        timer_CoolDown = new TimeCountDown();
        timer_BurstJump = new TimeCountDown();
        timer_LongJump = new TimeCountDown();
        timer_JumpNoInput = new TimeCountDown();
        timer_BurstAttack = new TimeCountDown();
        timer_ReadySpinAttack = new TimeCountDown();
        timer_SpinAttack = new TimeCountDown();
        timer_ForwardAccele = new TimeCountDown();
        timer_CruchAttack = new TimeCountDown();
        timer_Guard = new TimeCountDown();
        timer_StepJumpCoolDown = new TimeCountDown();
        timer_WallGlabStoper = new TimeCountDown();
        timer_StopWallAction = new TimeCountDown();

        timer_Wait = new TimeCountDown();
        timer_Move = new TimeCountDown();

        updateCountDowns = new InterfaceCountDown[]
        {
            timer_NoGravity,
            timer_StopMove,
            timer_CoolDown,
            timer_BurstJump,
            timer_JumpNoInput,
            timer_LongJump,
            timer_BurstAttack,
            timer_ReadySpinAttack,
            timer_SpinAttack,
            timer_ForwardAccele,
            timer_CruchAttack,
            timer_Guard,
            timer_Invincible,
            timer_DieWait,
            timer_Avoid,
            timer_StepJumpCoolDown,
            timer_WallGlabStoper,
            timer_StopWallAction,
            timer_Wait,
            timer_Move
        };
    }
}
