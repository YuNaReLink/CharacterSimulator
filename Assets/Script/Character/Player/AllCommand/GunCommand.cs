using UnityEditor;
using UnityEngine;
using static CharacterManager;


//èeÇÃèàóùÇÇ‹Ç∆ÇﬂÇΩèàóù
public class GunCommand
{
    private PlayerController controller = null;
    public GunCommand(PlayerController _controller)
    {
        controller = _controller;
    }

    public void Execute()
    {
        if (controller.GetTag() != DataTag.RatchetAndClank) { return; }
        bool shoot = true;
        switch (controller.GetCurrentState())
        {
            case ActionState.Jump:
                if (controller.JumpCount == controller.GetScriptableObject().MaxJumpCount)
                {
                    shoot = false;
                }
                break;
            case ActionState.GlideJump:
            case ActionState.Attack:
            case ActionState.Crouch:
                shoot = false;
                break;
        }
        if (!shoot) { return; }
        if (!controller.GetStateInput().IsMouseRightClick()){return;}
        controller.GetPropssetting().ActiveGun(true);
        controller.GetPropssetting().ActiveSword(false);
        controller.GetBulletShot().FireBullet();
    }
}
