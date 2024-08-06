using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTimer : MonoBehaviour
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
    //死後硬直カウント
    protected TimeCountDown timer_DieWait;
    //回避カウント
    protected TimeCountDown timer_Avoid;
    public TimeCountDown Timer_Avoid { get { return timer_Avoid; } set { timer_Avoid = value; } }

    protected TimeCountDown timer_StepJumpCoolDown;
    public TimeCountDown Timer_StepJumpCoolDown { get { return timer_StepJumpCoolDown; } set { timer_StepJumpCoolDown = value; } }
    protected TimeCountDown timer_WallGlabStoper;
    public TimeCountDown Timer_WallGlabStoper { get { return timer_WallGlabStoper; } set { timer_WallGlabStoper = value; } }
    protected TimeCountDown timer_StopWallAction;
    public TimeCountDown Timer_StopWallAction { get { return timer_StopWallAction; } set { timer_StopWallAction = value; } }
}
