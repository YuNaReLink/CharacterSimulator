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
        //通常攻撃処理
        BasisAttackCommand();
        //ジャンプ攻撃処理
        JumpAttackCommand();
        //ヒップドロップの処理
        HipDropCommand();
        SitAttack();
        //攻撃した時に前に少し移動させる処理
        AttackForwardAcceleration();
    }

    public float GetAttackNormalizedTime()
    {
        return 0.65f;
    }

    private void BasisAttackCommand()
    {
        //三段攻撃処理を行わない条件
        if (controller.GetStateInput().BlockState != ShieldBlockState.Null) { return; }
        if (controller.GetCurrentState() == ActionState.JumpAttack) { return; }
        if (controller.GetStateInput().IsCrouchKey()) { return; }
        if (!controller.Landing) { return; }
        //左クリックをして連撃カウントが3以上なら
        if (!controller.GetStateInput().IsMouseLeftDownClick() || controller.AttackCount > 3) { return; }
        //攻撃アニメーションの再生時間が半分より過ぎていたら
        bool attackEnd = controller.GetAnim().GetCurrentAnimatorStateInfo(0).normalizedTime > GetAttackNormalizedTime();
        //1撃目なら
        if (!controller.GetTimer().Timer_BurstAttack.IsEnabled())
        {
            //連撃処理の設定
            SetBurstAttackState();
        }
        //2撃目以降なら
        else if (attackEnd && controller.AttackCount <= controller.GetScriptableObject().MaxAttackCount && controller.GetTimer().Timer_BurstAttack.IsEnabled())
        {
            //連撃処理の設定
            SetBurstAttackState();
        }
    }

    private void SetBurstAttackState()
    {
        //連撃カウントを加算
        controller.AttackCount++;
        controller.GetTimer().Timer_ForwardAccele.StartTimer(forwardAcceleCount[1]);
        //連撃カウントによって状態を変更
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
        //連撃タイマーをスタート
        controller.GetTimer().Timer_BurstAttack.StartTimer(controller.GetScriptableObject().MaxBurstAttackCount);
        controller.GetTimer().Timer_BurstAttack.OnCompleted += () => { controller.AttackCount = 0; };
        //モーションを設定
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
