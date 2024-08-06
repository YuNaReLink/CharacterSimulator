using UnityEngine;
using static CharacterManager;

public class RatchetAndClankAttackCommand : InterfaceAttackCommand
{
    private PlayerController controller = null;
    public RatchetAndClankAttackCommand(PlayerController _controller)
    {
        controller = _controller;
    }
    public void Execute()
    {
        //�O�i�U��
        BasisAttackCommand();
        //�U�����̉�������
        AttackForwardAcceleration();
    }

    public float GetAttackNormalizedTime()
    {
        return 0.65f;
    }

    private void BasisAttackCommand()
    {
        //�O�i�U���������s��Ȃ�����
        if (controller.GetStateInput().IsCrouchKey()) { return; }
        if (!controller.Landing) { return; }
        //���N���b�N�����ĘA���J�E���g��3�ȏ�Ȃ�
        if (!controller.GetStateInput().IsMouseLeftDownClick() || controller.AttackCount > 3) { return; }
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

    private void AttackForwardAcceleration()
    {
        if (!controller.GetTimer().Timer_ForwardAccele.IsEnabled()) { return; }
        float attackforwardpower = controller.GetScriptableObject().AttackForwardPower;
        controller.AttackForwardAccele(attackforwardpower);
    }
}
