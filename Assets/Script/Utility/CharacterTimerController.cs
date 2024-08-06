using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTimer : MonoBehaviour
{
    //�d�͖����^�C�}�[
    protected TimeCountDown timer_NoGravity;
    public TimeCountDown Timer_NoGravity { get { return timer_NoGravity; } set { timer_NoGravity = value; } }
    //�d������
    protected TimeCountDown timer_StopMove;
    public TimeCountDown Timer_StopMove { get { return timer_StopMove; } set { timer_StopMove = value; } }
    //�N�[���_�E��
    protected TimeCountDown timer_CoolDown;
    //�O�i�W�����v�^�C�}�[�J�E���g
    protected TimeCountDown timer_BurstJump;
    public TimeCountDown GetTimer_BurstJump() { return timer_BurstJump; }
    protected bool burstJump = false;
    public bool BurstJumpFlag { get { return burstJump; } set { burstJump = value; } }
    //�W�����v���̓N�[���_�E���^�C�}�[
    protected TimeCountDown timer_JumpNoInput;
    public TimeCountDown Timer_JumpNoInput { get { return timer_JumpNoInput; } set { timer_JumpNoInput = value; } }
    //�����O�W�����v�\�^�C�}�[
    protected TimeCountDown timer_LongJump;
    public TimeCountDown Timer_LongJump { get { return timer_LongJump; } set { timer_LongJump = value; } }
    //�A���̃J�E���g
    protected TimeCountDown timer_BurstAttack;
    public TimeCountDown Timer_BurstAttack { get { return timer_BurstAttack; } set { timer_BurstAttack = value; } }
    //��]�U���������J�E���g
    protected TimeCountDown timer_ReadySpinAttack;
    public TimeCountDown Timer_ReadySpinAttack { get { return timer_ReadySpinAttack; } set { timer_ReadySpinAttack = value; } }
    //��]�U�����J�E���g
    protected TimeCountDown timer_SpinAttack;
    public TimeCountDown Timer_SpinAttack { get { return timer_SpinAttack; } set { timer_SpinAttack = value; } }
    //�U�����ɑO�ɑO�i������J�E���g
    protected TimeCountDown timer_ForwardAccele;
    public TimeCountDown Timer_ForwardAccele { get { return timer_ForwardAccele; } set { timer_ForwardAccele = value; } }
    private TimeCountDown timer_CruchAttack;
    public TimeCountDown Timer_CruchAttack { get { return timer_CruchAttack; } set { timer_CruchAttack = value; } }
    //�K�[�h�̃J�E���g
    protected TimeCountDown timer_Guard;
    //���G���Ԃ̃J�E���g
    protected TimeCountDown timer_Invincible;
    //����d���J�E���g
    protected TimeCountDown timer_DieWait;
    //����J�E���g
    protected TimeCountDown timer_Avoid;
    public TimeCountDown Timer_Avoid { get { return timer_Avoid; } set { timer_Avoid = value; } }

    protected TimeCountDown timer_StepJumpCoolDown;
    public TimeCountDown Timer_StepJumpCoolDown { get { return timer_StepJumpCoolDown; } set { timer_StepJumpCoolDown = value; } }
    protected TimeCountDown timer_WallGlabStoper;
    public TimeCountDown Timer_WallGlabStoper { get { return timer_WallGlabStoper; } set { timer_WallGlabStoper = value; } }
    protected TimeCountDown timer_StopWallAction;
    public TimeCountDown Timer_StopWallAction { get { return timer_StopWallAction; } set { timer_StopWallAction = value; } }
}
