using UnityEngine;
using static CharacterManager;

//攻撃の処理をまとめたクラス
public class ZeldaAttackCommand : InterfaceAttackCommand
{
    private PlayerController controller = null;
    public ZeldaAttackCommand(PlayerController _controller)
    {
        controller = _controller;
    }

    public void Execute()
    {
        //通常攻撃処理
        BasisAttackCommand();
        //回転斬りの処理
        ReadySpinAttackCommand();
        SpinAttackCommand();
        //ジャンプ攻撃処理
        JumpAttackCommand();
        //しゃがみ攻撃処理
        SitAttack();
        //攻撃した時に前に少し移動させる処理
        AttackForwardAcceleration();
    }

    public float GetAttackNormalizedTime()
    {
        return 0.65f;
    }

    //攻撃関連処理
    private void BasisAttackCommand()
    {
        //三段攻撃処理を行わない条件
        if(controller.GetStateInput().BlockState != ShieldBlockState.Null) { return; }
        if(controller.GetCurrentState() == ActionState.JumpAttack) { return; }
        if (controller.GetStateInput().IsCrouchKey()) { return; }
        if (!controller.Landing) { return; }
        //左クリックをして連撃カウントが3以上なら
        if(!controller.GetStateInput().IsMouseLeftDownClick()|| controller.AttackCount > 3) { return; }

        controller.GetPropssetting().SetActiveSwordOnly();
        //攻撃状態に変更
        controller.CurrentBattleFlag = true;
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
        controller.GetTimer().Timer_ForwardAccele.StartTimer(0.25f);
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
    private void ReadySpinAttackCommand()
    {
        //回転斬りの予備動作を行わない条件
        if (controller.GetStateInput().BlockState != ShieldBlockState.Null) { return; }
        if (controller.GetCurrentState() == ActionState.JumpAttack) { return; }
        if (!controller.Landing) { return; }
        if(controller.AttackState != AttackStateCount.FirstAttack) { return; }

        //回転斬りの予備動作の開始条件
        //三段攻撃の一段目の終わりに左クリック長押しをしていたらが条件
        if (!controller.GetTimer().Timer_ReadySpinAttack.IsEnabled())
        {
            //アニメーションの詳細情報を取得
            AnimatorStateInfo animInfo = controller.GetAnim().GetCurrentAnimatorStateInfo(0);

            //一段目攻撃のモーションじゃなければリターン
            if (!animInfo.IsName("slash1")) { return; }

            //以下予備動作の条件
            if (animInfo.normalizedTime < 0.8f ||!controller.GetStateInput().IsMouseLeftHoldClick()||
                controller.GetTimer().Timer_SpinAttack.IsEnabled()) { return; }

            controller.GetTimer().Timer_ReadySpinAttack.StartTimer(controller.GetScriptableObject().ReadySpinAttackTimerCount);
            controller.ChangeMotionState(ActionState.ReadySpinAttack);
        }
        //長押しをし続けている時の処理
        //回転斬りの予備動作のタイマーを設定し続けている
        else
        {
            if (!controller.GetStateInput().IsMouseLeftHoldClick()||controller.GetTimer().Timer_SpinAttack.IsEnabled()) { return; }
            
            controller.GetTimer().Timer_ReadySpinAttack.StartTimer(controller.GetScriptableObject().ReadySpinAttackTimerCount);
            controller.ChangeMotionState(ActionState.ReadySpinAttack);
        }
    }
    private void SpinAttackCommand()
    {
        //回転斬り動作を行わない条件
        if (controller.GetStateInput().BlockState != ShieldBlockState.Null) { return; }
        if (controller.GetCurrentState() == ActionState.JumpAttack) { return; }
        if (!controller.Landing) { return; }
        if(controller.AttackState != AttackStateCount.FirstAttack) { return; }
        //回転斬り行う入力をしていなかったらリターン
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
        //ジャンプアタックの処理を行わない条件
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
        //キャラクターによって条件を分岐
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
