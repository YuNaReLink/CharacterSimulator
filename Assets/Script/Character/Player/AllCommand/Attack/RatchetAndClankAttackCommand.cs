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
        //三段攻撃
        BasisAttackCommand();
        //攻撃中の加速処理
        AttackForwardAcceleration();
    }

    public float GetAttackNormalizedTime()
    {
        return 0.65f;
    }

    private void BasisAttackCommand()
    {
        //三段攻撃処理を行わない条件
        if (controller.GetStateInput().IsCrouchKey()) { return; }
        if (!controller.Landing) { return; }
        //左クリックをして連撃カウントが3以上なら
        if (!controller.GetStateInput().IsMouseLeftDownClick() || controller.AttackCount > 3) { return; }
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

    private void AttackForwardAcceleration()
    {
        if (!controller.GetTimer().Timer_ForwardAccele.IsEnabled()) { return; }
        float attackforwardpower = controller.GetScriptableObject().AttackForwardPower;
        controller.AttackForwardAccele(attackforwardpower);
    }
}
