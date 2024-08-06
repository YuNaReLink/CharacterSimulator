using UnityEngine;
using static CharacterManager;

//�U���̏������܂Ƃ߂��N���X
public class ZeldaAttackCommand : InterfaceAttackCommand
{
    private PlayerController controller = null;
    public ZeldaAttackCommand(PlayerController _controller)
    {
        controller = _controller;
    }

    public void Execute()
    {
        //�ʏ�U������
        BasisAttackCommand();
        //��]�a��̏���
        ReadySpinAttackCommand();
        SpinAttackCommand();
        //�W�����v�U������
        JumpAttackCommand();
        //���Ⴊ�ݍU������
        SitAttack();
        //�U���������ɑO�ɏ����ړ������鏈��
        AttackForwardAcceleration();
    }

    public float GetAttackNormalizedTime()
    {
        return 0.65f;
    }

    //�U���֘A����
    private void BasisAttackCommand()
    {
        //�O�i�U���������s��Ȃ�����
        if(controller.GetStateInput().BlockState != ShieldBlockState.Null) { return; }
        if(controller.GetCurrentState() == ActionState.JumpAttack) { return; }
        if (controller.GetStateInput().IsCrouchKey()) { return; }
        if (!controller.Landing) { return; }
        //���N���b�N�����ĘA���J�E���g��3�ȏ�Ȃ�
        if(!controller.GetStateInput().IsMouseLeftDownClick()|| controller.AttackCount > 3) { return; }

        controller.GetPropssetting().SetActiveSwordOnly();
        //�U����ԂɕύX
        controller.CurrentBattleFlag = true;
        //�U���A�j���[�V�����̍Đ����Ԃ��������߂��Ă�����
        bool attackEnd = controller.GetAnim().GetCurrentAnimatorStateInfo(0).normalizedTime > GetAttackNormalizedTime();
        //1���ڂȂ�
        if (!controller.GetTimer().Timer_BurstAttack.IsEnabled())
        {
            //�A�������̐ݒ�
            SetBurstAttackState();
        }
        //2���ڈȍ~�Ȃ�
        else if (attackEnd && controller.AttackCount <= controller.GetScriptableObject().MaxAttackCount && controller.GetTimer().Timer_BurstAttack.IsEnabled())
        {
            //�A�������̐ݒ�
            SetBurstAttackState();
        }
    }
    private void SetBurstAttackState()
    {
        //�A���J�E���g�����Z
        controller.AttackCount++;
        controller.GetTimer().Timer_ForwardAccele.StartTimer(0.25f);
        //�A���J�E���g�ɂ���ď�Ԃ�ύX
        switch (controller.AttackCount)
        {
            case 1:
                controller.AttackState = AttackStateCount.FirstAttack;
                break;
            case 2:
                controller.AttackState = AttackStateCount.SecondAttack;
                break;
            case 3:
                controller.AttackState = AttackStateCount.ThirdAttack;
                break;
        }
        //�A���^�C�}�[���X�^�[�g
        controller.GetTimer().Timer_BurstAttack.StartTimer(controller.GetScriptableObject().MaxBurstAttackCount);
        controller.GetTimer().Timer_BurstAttack.OnCompleted += () => { controller.AttackCount = 0; };
        //���[�V������ݒ�
        controller.ChangeMotionState(ActionState.Attack);
    }
    private void ReadySpinAttackCommand()
    {
        //��]�a��̗\��������s��Ȃ�����
        if (controller.GetStateInput().BlockState != ShieldBlockState.Null) { return; }
        if (controller.GetCurrentState() == ActionState.JumpAttack) { return; }
        if (!controller.Landing) { return; }
        if(controller.AttackState != AttackStateCount.FirstAttack) { return; }

        //��]�a��̗\������̊J�n����
        //�O�i�U���̈�i�ڂ̏I���ɍ��N���b�N�����������Ă����炪����
        if (!controller.GetTimer().Timer_ReadySpinAttack.IsEnabled())
        {
            //�A�j���[�V�����̏ڍ׏����擾
            AnimatorStateInfo animInfo = controller.GetAnim().GetCurrentAnimatorStateInfo(0);

            //��i�ڍU���̃��[�V��������Ȃ���΃��^�[��
            if (!animInfo.IsName("slash1")) { return; }

            //�ȉ��\������̏���
            if (animInfo.normalizedTime < 0.8f ||!controller.GetStateInput().IsMouseLeftHoldClick()||
                controller.GetTimer().Timer_SpinAttack.IsEnabled()) { return; }

            controller.GetTimer().Timer_ReadySpinAttack.StartTimer(controller.GetScriptableObject().ReadySpinAttackTimerCount);
            controller.ChangeMotionState(ActionState.ReadySpinAttack);
        }
        //���������������Ă��鎞�̏���
        //��]�a��̗\������̃^�C�}�[��ݒ肵�����Ă���
        else
        {
            if (!controller.GetStateInput().IsMouseLeftHoldClick()||controller.GetTimer().Timer_SpinAttack.IsEnabled()) { return; }
            
            controller.GetTimer().Timer_ReadySpinAttack.StartTimer(controller.GetScriptableObject().ReadySpinAttackTimerCount);
            controller.ChangeMotionState(ActionState.ReadySpinAttack);
        }
    }
    private void SpinAttackCommand()
    {
        //��]�a�蓮����s��Ȃ�����
        if (controller.GetStateInput().BlockState != ShieldBlockState.Null) { return; }
        if (controller.GetCurrentState() == ActionState.JumpAttack) { return; }
        if (!controller.Landing) { return; }
        if(controller.AttackState != AttackStateCount.FirstAttack) { return; }
        //��]�a��s�����͂����Ă��Ȃ������烊�^�[��
        if (!controller.GetStateInput().IsMouseLeftUpClick() || !controller.GetTimer().Timer_ReadySpinAttack.IsEnabled() ||
            controller.GetTimer().Timer_SpinAttack.IsEnabled()) { return; }

        controller.AttackCount = 0;
        controller.GetStateInput().SetMouseMiddleClick(false);
        controller.GetTimer().Timer_SpinAttack.StartTimer(controller.GetScriptableObject().SpinAttackTimerCount);
        controller.ChangeMotionState(ActionState.SpinAttack);
        controller.AttackState = AttackStateCount.Null;
        controller.GetSEController().SpinSwordAttackSEPlay();
    }

    private void JumpAttackCommand()
    {
        if(controller.GetStateInput().BlockState == ShieldBlockState.SitBlock) { return; }
        if (controller.GetTimer().Timer_BurstAttack.IsEnabled()) { return; }
        SwordJumpAttackCommand();
    }

    private void SwordJumpAttackCommand()
    {
        //�W�����v�A�^�b�N�̏������s��Ȃ�����
        bool noJumpAttackFlag = controller.JumpAttackFlag || !controller.GetStateInput().ShiftKeyDown;
        bool noJumpAttackPattern = !controller.GetStateInput().IsMouseMiddle() || !controller.CurrentBattleFlag;
        if (noJumpAttackFlag) { return; }
        if (noJumpAttackPattern && controller.Landing){return;}
        if(controller.GetStateInput().Horizontalinput != 0 || controller.GetStateInput().VerticalInput < 0) { return; }

        controller.CurrentBattleFlag = true;
        controller.ChangeMotionState(ActionState.JumpAttack);

        if (controller.GetCurrentState() != ActionState.JumpAttack) { return; }

        controller.JumpForce(controller.GetScriptableObject().FirstJumpPower);
        controller.GetTimer().Timer_ForwardAccele.StartTimer(0.5f);
        controller.JumpAttackFlag = true;
    }

    private void SitAttack()
    {
        //�L�����N�^�[�ɂ���ď����𕪊�
        SwordSitAttackCommand();
    }

    private void SwordSitAttackCommand()
    {
        if (controller.GetStateInput().BlockState != ShieldBlockState.SitBlock ||
            !controller.GetStateInput().IsMouseLeftDownClick()) { return; }

        controller.ChangeMotionState(ActionState.CrouchAttack);
        controller.GetTimer().Timer_CruchAttack.StartTimer(0.5f);
        if (controller.GetCurrentState() == ActionState.CrouchAttack)
        {
            controller.CurrentBattleFlag = true;
        }
    }

    public void AttackForwardAcceleration()
    {
        if (controller.GetTimer().Timer_ForwardAccele.IsEnabled())
        {
            float attackforwardpower = controller.GetScriptableObject().AttackForwardPower;
            controller.AttackForwardAccele(attackforwardpower);
        }
    }
}
