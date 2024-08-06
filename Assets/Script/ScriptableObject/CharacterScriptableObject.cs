using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CharacterScriptableObject : ScriptableObject
{
    [SerializeField]
    private string          name;
    [SerializeField]
    private string          info;
    [SerializeField]
    private float           maxHP;
    [SerializeField]
    private float           acceleration;
    [SerializeField]
    private float           maxSpeed;
    [SerializeField]
    private float           deceleration;
    [SerializeField]
    private float           minSpeed;
    [SerializeField]
    private int             maxJumpCount;
    [SerializeField]
    private float           firstJumpPower;
    [SerializeField]
    private float           secondJumpPower;
    [SerializeField]
    private float           thirdJumpPower;
    [SerializeField]
    private float           maxLongJumpCount;
    [SerializeField]
    private float           keepJumpForce;
    [SerializeField]
    private float           maxBurstJumpCount;
    [SerializeField]
    private float           maxNoJumpInputCount;
    [SerializeField]
    private float           flipJumpPower;
    [SerializeField]
    private float           rollingPower;
    [SerializeField]
    private float           knockBackPower;
    [SerializeField]
    private float           attackPower;
    [SerializeField]
    private float           footAttackPower;
    [SerializeField]
    private float           armAttackPower;
    [SerializeField]
    private float           attackForwardPower;
    [SerializeField]
    private float           maxAttackCount;
    [SerializeField]
    private float           maxBurstAttackCount;
    [SerializeField]
    private float           jumpAttackTimerCount;
    [SerializeField]
    private float           readySpinAttackTimerCount;
    [SerializeField]
    private float           spinAttackTimerCount;
    [SerializeField]
    private float           guardEnabled;
    [SerializeField]
    private float           forwardAcceleTimerCount;
    [SerializeField]
    private float           backAcceleTimerCount;
    [SerializeField]
    private float           horizontalTimerCount;
    [SerializeField]
    private float           invincibleTimerCount;
    public string Name
    { get { return name; } set { name = value; } }
    public string Info
    { get { return info; }}
    public float MaxHP
    { get { return maxHP; }}
    public float Acceleration
    { get { return acceleration; }}
    public float MaxSpeed
    { get { return maxSpeed; }}
    public float Deceleration
    { get { return deceleration; }}
    public float MinSpeed
    { get { return minSpeed; }}
    public int MaxJumpCount
    { get { return maxJumpCount; } set { maxJumpCount = value; } }
    public float FirstJumpPower
    { get { return firstJumpPower; } set { firstJumpPower = value; } }
    public float SecondJumpPower
    { get { return secondJumpPower; } set { secondJumpPower = value; } }
    public float ThirdJumpPower
    { get { return thirdJumpPower; } set { thirdJumpPower = value; } }
    public float MaxLongJumpCount
    { get { return maxLongJumpCount; } set { maxLongJumpCount = value; } }
    public float KeepJumpForce
    { get { return keepJumpForce; } set { keepJumpForce = value; } }
    public float MaxBurstJumpCount
    { get { return maxBurstJumpCount; } set { maxBurstJumpCount = value; } }
    public float MaxNoJumpInputCount
    { get { return maxNoJumpInputCount; } set { maxNoJumpInputCount = value; } }
    public float FlipJumpPower
    { get { return flipJumpPower; } set { flipJumpPower = value; } }
    public float RollingPower
    { get { return rollingPower; } set { rollingPower = value; } }
    public float KnockBackPower
    { get { return knockBackPower; } set { knockBackPower = value; } }
    public float AttackPower
    { get { return attackPower; } set { attackPower = value; } }
    public float FootAttackPower
    { get { return footAttackPower; } set {  footAttackPower = value; } }
    public float ArmAttackPower
    { get { return armAttackPower; } set { armAttackPower = value; } }
    public float AttackForwardPower
    { get { return attackForwardPower; } set { attackForwardPower = value; } }
    public float MaxAttackCount
    { get { return maxAttackCount; } set { maxAttackCount = value; } }
    public float MaxBurstAttackCount
    { get { return maxBurstAttackCount; } set { maxBurstAttackCount = value; } }
    public float JumpAttackTimerCount
    { get { return jumpAttackTimerCount; } set { jumpAttackTimerCount = value; } }
    public float ReadySpinAttackTimerCount
    { get { return readySpinAttackTimerCount; } set { readySpinAttackTimerCount = value; } }
    public float SpinAttackTimerCount
    { get { return spinAttackTimerCount; } set { spinAttackTimerCount = value; } }
    public float GuardEnabled
    { get { return guardEnabled; } set { guardEnabled = value; } }
    public float ForwardAcceleTimerCount
    { get { return forwardAcceleTimerCount; } set { forwardAcceleTimerCount = value; } }
    public float BackAcceleTimerCount
    { get { return backAcceleTimerCount; } set { backAcceleTimerCount = value; } }
    public float HorizontalTimerCount
    { get { return horizontalTimerCount; } set { horizontalTimerCount = value; } }
    public float InvincibleTimerCount
    { get { return invincibleTimerCount;} set { invincibleTimerCount = value; } }
}
