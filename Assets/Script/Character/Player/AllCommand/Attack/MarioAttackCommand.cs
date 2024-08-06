using UnityEngine;
using static CharacterManager;

public class MarioAttackCommand : InterfaceAttackCommand
{
    private PlayerController controller = null;
    public MarioAttackCommand(PlayerController _controller)
    {
        controller = _controller;
    }

    private float[] jumpPower = new float[]
    {
        1000f,
        50f
    };

    private float[] forwardAcceleCount = new float[]
    {
        0.6f,
        0.25f
    };

    private float noGravityCount = 0.5f;

    private float forwardAcceleRatio = 10f;

    public void Execute()
    {
        //�ʏ�U������
        BasisAttackCommand();
        //�W�����v�U������
        JumpAttackCommand();
        //�q�b�v�h���b�v�̏���
        HipDropCommand();
        SitAttack();
        //�U���������ɑO�ɏ����ړ������鏈��
        AttackForwardAcceleration();
    }

    public float GetAttackNormalizedTime()
    {
        return 0.65f;
    }

    private void BasisAttackCommand()
    {
        //�O�i�U���������s��Ȃ�����
        if (controller.GetStateInput().BlockState != ShieldBlockState.Null) { return; }
        if (controller.GetCurrentState() == ActionState.JumpAttack) { return; }
        if (controller.GetStateInput().IsCrouchKey()) { return; }
        if (!controller.Landing) { return; }
        //���N���b�N�����ĘA���J�E���g��3�ȏ�Ȃ�
        if (!controller.GetStateInput().IsMouseLeftDownClick() || controller.AttackCount > 3) { return; }
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
        controller.GetTimer().Timer_ForwardAccele.StartTimer(forwardAcceleCount[1]);
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

    private void JumpAttackCommand()
    {
        if (controller.GetStateInput().BlockState == ShieldBlockState.SitBlock) { return; }
        if (controller.GetTimer().Timer_BurstAttack.IsEnabled()) { return; }
        BodyJumpingAttackCommand();
    }

    private void BodyJumpingAttackCommand()
    {
        if (controller.GetTimer().Timer_ForwardAccele.IsEnabled()) { return; }
        bool jumpingAttack = controller.JumpAttackFlag && controller.GetStateInput().IsMouseLeftDownClick() &&
            controller.GetCurrentState() == ActionState.JumpAttack && !controller.GetTimer().Timer_ForwardAccele.IsEnabled();
        if (jumpingAttack)
        {
            controller.JumpForce(jumpPower[0]);
            controller.GetTimer().Timer_ForwardAccele.StartTimer(forwardAcceleCount[0]);
            controller.GetTimer().Timer_ForwardAccele.OnCompleted += () =>
            {
                controller.EmptyDamageWallHitFlag();
                controller.GetPropssetting().ActiveArmDamageObject(false);
            };
        }
    }

    private void SitAttack()
    {
        FootSitAttackCommand();
    }
    private void FootSitAttackCommand()
    {
        if (controller.GetCurrentState() != ActionState.Crouch || !controller.GetStateInput().IsMouseLeftDownClick()) { return; }
        controller.ChangeMotionState(ActionState.CrouchAttack);
    }

    private void HipDropCommand()
    {
        if (!controller.GetStateInput().ShiftKeyDown || controller.GetCurrentState() != ActionState.HipDrop || !controller.GetStateInput().GetHipDropFlag ||
            !controller.GetCharacterRb().useGravity || controller.GetTimer().Timer_NoGravity.IsEnabled()) { return; }
        controller.RotateFlag = true;
        controller.GetCharacterRb().useGravity = false;
        controller.GetStateMovement().StopVelocityXZ();
        controller.GetCharacterCollider().enabled = false;
        controller.GetStateInput().ShiftKeyDown = false;

        controller.RotateY = controller.transform.rotation.eulerAngles.y;
        controller.GetCharacterRb().velocity = new Vector3();
        controller.GetTimer().Timer_NoGravity.StartTimer(noGravityCount);
        controller.GetTimer().Timer_NoGravity.OnCompleted += () =>
        {
            controller.GetCharacterRb().useGravity = true;
            controller.GetCharacterCollider().enabled = true;
        };
        controller.JumpForce(jumpPower[1]);
        controller.GetPropssetting().ActiveDamageBody(false);
    }

    public void AttackForwardAcceleration()
    {
        if (controller.GetTimer().Timer_ForwardAccele.IsEnabled())
        {
            float attackforwardpower = controller.GetScriptableObject().AttackForwardPower;
            if (controller.JumpAttackFlag)
            {
                attackforwardpower *= forwardAcceleRatio;
            }
            controller.AttackForwardAccele(attackforwardpower);
        }
    }
}
