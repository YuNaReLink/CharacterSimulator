

public class MotionNameCollection
{
    private string[] noPlayMotionName =
    {   "sideFlip",
        "roundhousekick",
        "spinAttack",
        "shieldSlach",
        "jumpingAttack",
        "jumpAttack",
        "rolling",
        "backFlip",
        "leftRolling",
        "leftSideFlip",
        "rightSideFlip",
        "rightRolling",
        "weaponSwitch",
        "shieldBlockSitIdle",
        "damage" ,
        "death" 
    };
    public string[] GetNoPlayMotionName() {  return noPlayMotionName; }

    private string[] noDamageMotionsName =
    {
        "spinAttack",
        "jumpAttack",
        "slash1",
        "slash2",
        "slash3",
        "rolling",
        "backFlip",
        "leftRolling",
        "rightRolling",
        "leftSideFlip",
        "rightSideFlip",
        "hangingIdle",
        "hangToCrouch"
    };

    public string[] GetNoDamageMotionsName() { return noDamageMotionsName;}
}
