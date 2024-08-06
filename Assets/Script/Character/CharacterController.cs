using System.Collections.Generic;
using UnityEngine;
using static CharacterManager;


public class CharacterController : MonoBehaviour
{
    /// <summary>
    /// キャラクターのタグ
    /// タグ指定で各設定が自動で変わるようにしている
    /// </summary>
    [Header("キャラクターごとに指定するタグ(※プレイヤーは自動設定)")]
    [SerializeField]
    protected DataTag                   selectTag = DataTag.Null;
    public DataTag                      GetTag() { return selectTag; }
    /// <summary>
    /// ゲームをクリアした時のフラグ
    /// </summary>
    protected bool                      gameClearPoseFlag = false;

    /// <summary>
    /// 全キャラクターのデータを格納してるスクリプタブルオブジェクト
    /// </summary>
    [SerializeField]
    CharacterScriptableDataList         datas;
    /// <summary>
    /// キャラクター個人で使用するスクリプタブルオブジェクト
    /// </summary>
    [SerializeField]
    protected CharacterScriptableObject scriptableData;
    public CharacterScriptableObject    GetScriptableObject() { return scriptableData; }
    //最大HP
    protected float                     maxHP;
    public float                        GetMaxHP() { return maxHP; }
    //加減用のHP変数
    protected float                     hp;
    public float                        GetHP() {  return hp; }
    /// <summary>
    /// 上方向に長距離ジャンプするためのフラグ
    /// </summary>
    protected bool                      highJump = false;
    public bool                         HighJumpFlag { get { return highJump; }set { highJump = value; } }
    /// <summary>
    /// 前方向に長距離ジャンプするためのフラグ
    /// </summary>
    protected bool                      lengthyJump = false;
    public bool                         LengthyJumpFlag { get{ return lengthyJump; }set { lengthyJump = value; } }

    //ダメージオブジェクトクラス
    [SerializeField]
    protected List<BaseDamageObject>    damageObjects = new List<BaseDamageObject>();
    ///<summary>
    ///Collider
    ///</summary>
    [SerializeField]
    protected Collider                  characterCollider;
    public Collider                     GetCharacterCollider() {  return characterCollider; }
    /// <summary>
    /// パシフィックマテリアル
    /// </summary>
    [SerializeField]
    protected List<PhysicMaterial> physicMaterials = new List<PhysicMaterial>();
    ///<summary>
    ///Rigidbody
    ///</summary>
    protected Rigidbody                 characterRb;
    public Rigidbody                    GetCharacterRb() {  return characterRb; }
    /// <summary>
    /// キャラクターの移動関連変数
    /// </summary>
    protected Vector3                   velocity;
    /// <summary>
    /// キャラクターの移動関連変数
    /// </summary>
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
    //入力しているか判定
    [SerializeField]
    protected bool                      input = false;
    public bool                         InputFlag { get { return input; } set { input = value; } }
    //しゃがみ関係
    protected bool                      scrouchFromMove = false;
    //減速フラグ
    protected bool                      decele = false;
    public bool                         DeceleFlag { get { return decele; } set { decele = value; } }
    /// <summary>
    /// 攻撃関連
    /// </summary>
    //現在武器を出しているかどうか
    protected bool                      currentBattle = false;
    public bool                         CurrentBattleFlag {  get { return currentBattle; } set { currentBattle = value; } }
    //武器を出していたか出していなかったどうか
    protected bool                      previousState = false;
    public bool                         PreviousStateFlag { get { return previousState; } set { previousState = value; } }
    //連撃の現在のカウント
    protected int                       attackCount = 0;
    public int                          AttackCount { get { return attackCount; } set { attackCount = value; } }
    //連撃ができるかカウントする変数
    protected int                       burstAttackCount = 0;
    //ジャンプ攻撃フラグ
    protected bool                      jumpAttack = false;
    public bool                         JumpAttackFlag { get { return jumpAttack; }set { jumpAttack = value; } }
    //三段ジャンプを行うためのフラグ
    protected bool                      burstJump = false;
    public bool                         BurstJumpFlag { get { return burstJump; }set { burstJump = value; } }
    /// <summary>
    /// カウントダウンクラス
    /// </summary>
    protected CharacterTimerController    timer = null;
    public CharacterTimerController     GetTimer() { return timer; }
    
    //アニメーション
    [SerializeField]
    protected Animator                  anim;
    public Animator                     GetAnim() {  return anim; }
    /// <summary>
    /// 地面接地判定
    /// </summary>
    protected GroundCheck               check;
    [Header("キャラクターの状態")]
    //状態のインスタンス
    [SerializeField]
    protected ActionState               currentState = ActionState.Null;
    public ActionState                  GetCurrentState() { return currentState;}
    [SerializeField]
    protected ActionState               pastState = ActionState.Null;
    public ActionState                  GetPastState() { return pastState;}
    //怯み判定
    protected bool                      moveStop = false;
    public bool                         MoveStopFlag { get { return moveStop; }set { moveStop = value; } }
    private float                       repelPower = 5f;      
    //死亡判定
    protected bool                      die = false;
    public bool                         IsDied() { return die; }
    [SerializeField]
    //着地判定
    protected bool                      landing = false;
    public bool                         Landing { get { return landing; } set { landing = value; } }
    //武器位置替えクラス
    protected PlayerPropsSetting        toolSetting;
    public PlayerPropsSetting           GetPropssetting() { return  toolSetting; }
    //オブジェクトカラーチェンジクラス
    protected ColorChange               colorChange;
    /// <summary>
    /// エフェクト関連
    /// </summary>
    public enum EffectState
    {
        Null = -1,
        Die,
        Damage,
        Foot,
        DataEnd
    }
    //エフェクト生成クラス
    protected GenerateEffects           generateEffects;

    public GenerateEffects              GetGenerateEffects() {  return generateEffects; }

    protected virtual void Awake()
    {
        Load(GetTag());
    }

    private bool Load(DataTag tag)
    {
        CharacterScriptableDataList setCharacterScriptableData = datas;
        scriptableData = setCharacterScriptableData.SetData(selectTag);
        if(scriptableData == null)
        {
            Debug.LogError("データが代入されませんでした。");
            return false;
        }
        return true;
    }

    public virtual void Start()
    {
        maxHP =     scriptableData.MaxHP;
        hp =        maxHP;
        velocity =  Vector3.zero;
        currentState = ActionState.Null;
        pastState = currentState;

        previousState = currentBattle;
        SetDamageData();
    }

    protected virtual void InitializeFlag()
    {
        gameClearPoseFlag = false;
        highJump =          false;
        lengthyJump =       false;
        input =             false;
        decele =            false;
        previousState =     false;
        burstJump =         false;
        moveStop =          false;
        die =               false;
        jumpAttack =        false;
        landing =           false;
    }
    public virtual void AllGetComponent()
    {
        if (damageObjects == null)  
        {
            Debug.LogError("damageObjectsがアタッチされていません"); 
        }

        characterRb =               GetComponent<Rigidbody>();
        if(characterRb == null)     
        {
            Debug.LogError("RigidBodyがアタッチされていません");
        }
        anim =                      GetComponent<Animator>();
        if (anim == null)           {
            Debug.LogError("Animtorがアタッチされていません");
        }

        timer =                     new CharacterTimerController();
        if(timer == null)
        {
            Debug.LogError("timerが生成されませんでした");
        }
        else
        {
            timer.InitializeAssignTimer();
        }

        colorChange =               new ColorChange();
        if(colorChange == null)
        {
            Debug.LogError("colorChangeが生成されませんでした");
        }
        else
        {
            colorChange.GetSetSkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if(colorChange.GetSetSkinnedMeshRenderer == null)
            {
                Debug.LogError("skinnedMeshRendererがアタッチされていません");
            }
            else
            {
                colorChange.InitColor();
            }
        }
        generateEffects =           GetComponent<GenerateEffects>();
        if(generateEffects == null) 
        {
            Debug.LogError("generateEffectsがアタッチされていません");
        }

    }

    protected void SetDamageData()
    {
        for (int i = 0; i < damageObjects.Count; i++)
        {
            if (damageObjects == null || damageObjects.Count == 0)
            { Debug.Log("here"); }
            damageObjects[i].GetSetDamage = AttackData(damageObjects[i]);
        }
    }

    private float AttackData(BaseDamageObject _damageType)
    {
        float power = 0;
        BaseDamageObject.DamageType type = _damageType.GetDamageType();
        switch (type)
        {
            case BaseDamageObject.DamageType.Sword:
                power = scriptableData.AttackPower;
                break;
            case BaseDamageObject.DamageType.Foot:
                power = scriptableData.FootAttackPower;
                break;
            case BaseDamageObject.DamageType.Arm:
                power = scriptableData.ArmAttackPower;
                break;
        }
        return power;
    }

    protected virtual void SetPhysicMaterial()
    {
        if (!landing)
        {
            characterCollider.material = physicMaterials[(int)PhysicState.Jump];
        }
        else
        {
            characterCollider.material = physicMaterials[(int)PhysicState.Land];
        }
    }

    protected virtual void Update()
    {
        //オブジェクトカラーの更新(主に色を変更した時に使う)
        colorChange.ColorTransitionUpdate();
        timer.TimerUpdate();
    }

    public virtual void JumpForce(float _jumppower)
    {
        characterRb.AddForce(transform.up * _jumppower, ForceMode.Impulse);
    }

    public void KeepLongJump(float _jumppower)
    {
        characterRb.AddForce(transform.up * _jumppower, ForceMode.Acceleration);
    }

    public void FastFall(float _jumppower)
    {
        characterRb.AddForce(transform.up * _jumppower, ForceMode.Acceleration);
    }
    public virtual float AddDecelerationSetting(float _decele){ return _decele; }

    protected virtual bool RepelMove()
    {
        for (int i = 0; i < damageObjects.Count; i++)
        {
            if (damageObjects[i].IsWallHit() && RepelMoveEnabled())
            {
                characterRb.velocity = Vector3.zero;
                MoveXZ(-transform.forward * repelPower);
                return true;
            }
        }
        return false;
    }

    private bool RepelMoveEnabled()
    {
        switch (currentState)
        {
            case ActionState.Attack:
            case ActionState.JumpAttack:
            case ActionState.SpinAttack:
            case ActionState.CrouchAttack:
                return true;
        }
        return false;
    }

    protected void Move()
    {
        if (RepelMove()) { return; }
        float drag = 0;
        drag = GetDrag();
        characterRb.drag = drag;
        characterRb.velocity = new Vector3(velocity.x, characterRb.velocity.y, velocity.z);
    }

    protected virtual float GetDrag()
    {
        float _drag = 0;
        return _drag;
    }
    //攻撃する時にキャラクターを前進させるメソッド
    public void AttackForwardAccele(float power)
    {
        characterRb.AddForce(transform.forward * power, ForceMode.VelocityChange);
    }

    public void RollingForwardAccele(Vector3 force)
    {
        characterRb.AddForce(force, ForceMode.Acceleration);
    }
    //引数で与える移動量を指定
    public void MoveXZ(Vector3 force)
    {
        velocity += force;
    }

    public void ForwardAccele(Vector3 force)
    {
        characterRb.velocity = Vector3.zero;
        velocity += force;
    }

    //ノックバックをさせる(引数はノックバックの威力)
    protected void Knockback(float power)
    {
        characterRb.AddForce(-transform.forward * power, ForceMode.VelocityChange);
    }

    protected virtual void Die()
    {
        die = true;
        //死亡後体が動かないように
        characterRb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
