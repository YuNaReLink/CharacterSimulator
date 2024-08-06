using UnityEngine;
using UnityEngine.Windows;


public class CharacterManager
{
    /// <summary>
    /// キャラクター共通で使う数値
    /// </summary>

    public static readonly float endMotionNormalizedTime = 1.0f;

    public enum DataTag
    {
        Null = -1,
        Zelda,
        RatchetAndClank,
        SuperMario,
        Slime,
        Skeletron,
        BossSkeletron,
        DataEnd
    }
    //状態
    public enum ActionState
    {
        Null = -1,
        Idle,
        Falling,
        Run,
        Jump,
        RunJump,
        GlideJump,
        LongJump,
        RangeJump,
        ChangingDirectionJump,
        HipDrop,
        Crouch,
        Crawling,
        Flip,
        Attack,
        JumpAttack,
        ReadySpinAttack,
        SpinAttack,
        CrouchAttack,
        ModeChange,
        Guard,
        Grab,
        ClimbWall,
        Tracking,
        Damage,
        Die,
        VictoryPose,
        DataEnd
    };

    public static bool SetInput(ActionState state)
    {
        switch (state)
        {
            case ActionState.Run:
            case ActionState.Jump:
            case ActionState.RunJump:
            case ActionState.GlideJump:
            case ActionState.RangeJump:
            case ActionState.Falling:
            case ActionState.Flip:
            case ActionState.Crawling:
            case ActionState.ReadySpinAttack:
                return true;
        }
        return false;
    }

    public static bool AllJumpState(ActionState state)
    {
        switch (state)
        {
            case ActionState.Jump:
            case ActionState.RunJump:
            case ActionState.GlideJump:
            case ActionState.LongJump:
            case ActionState.RangeJump:
            case ActionState.ChangingDirectionJump:
            case ActionState.JumpAttack:
                return true;
        }

        return false;
    }

    public static bool AllObstacleState(ActionState state)
    {
        switch (state)
        {
            case ActionState.Grab:
            case ActionState.ClimbWall:
                return true;
        }
        return false;
    }

    /// <summary>
    /// カメラが注目中にローリングを行う時、
    /// ローリングを行う方向を決めるenum
    /// </summary>
    public enum AvoidState
    {
        Null = -1,
        Up,
        Down,
        Right,
        Left,
        DataEnd
    }

    //ジャンプの状態
    public enum JumpState
    {
        Null = -1,
        FirstJump,
        SecondJump,
        ThirdJump,
        DataEnd
    }

    //三段攻撃のenumクラス
    public enum AttackStateCount
    {
        Null = -1,
        FirstAttack,
        SecondAttack,
        ThirdAttack,
        DataEnd
    }

    public enum ShieldBlockState
    {
        Null = -1,
        SitBlock,
        FocusBlock,
        DataEnd
    }

    //移動状態
    public enum MoveState
    {
        Null = -1,
        maxspeed,
        accele,
        decele,
        stop,
    }

    public enum PhysicState
    {
        Null = -1,
        Land,
        Jump,
        DataEnd,
    }
}
