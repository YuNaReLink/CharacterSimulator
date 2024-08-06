using System.Collections.Generic;
using UnityEngine;
using static CharacterManager;


public class CharacterController : MonoBehaviour
{
    /// <summary>
    /// �L�����N�^�[�̃^�O
    /// �^�O�w��Ŋe�ݒ肪�����ŕς��悤�ɂ��Ă���
    /// </summary>
    [Header("�L�����N�^�[���ƂɎw�肷��^�O(���v���C���[�͎����ݒ�)")]
    [SerializeField]
    protected DataTag                   selectTag = DataTag.Null;
    public DataTag                      GetTag() { return selectTag; }
    /// <summary>
    /// �Q�[�����N���A�������̃t���O
    /// </summary>
    protected bool                      gameClearPoseFlag = false;

    /// <summary>
    /// �S�L�����N�^�[�̃f�[�^���i�[���Ă�X�N���v�^�u���I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    CharacterScriptableDataList         datas;
    /// <summary>
    /// �L�����N�^�[�l�Ŏg�p����X�N���v�^�u���I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    protected CharacterScriptableObject scriptableData;
    public CharacterScriptableObject    GetScriptableObject() { return scriptableData; }
    //�ő�HP
    protected float                     maxHP;
    public float                        GetMaxHP() { return maxHP; }
    //�����p��HP�ϐ�
    protected float                     hp;
    public float                        GetHP() {  return hp; }
    /// <summary>
    /// ������ɒ������W�����v���邽�߂̃t���O
    /// </summary>
    protected bool                      highJump = false;
    public bool                         HighJumpFlag { get { return highJump; }set { highJump = value; } }
    /// <summary>
    /// �O�����ɒ������W�����v���邽�߂̃t���O
    /// </summary>
    protected bool                      lengthyJump = false;
    public bool                         LengthyJumpFlag { get{ return lengthyJump; }set { lengthyJump = value; } }

    //�_���[�W�I�u�W�F�N�g�N���X
    [SerializeField]
    protected List<BaseDamageObject>    damageObjects = new List<BaseDamageObject>();
    ///<summary>
    ///Collider
    ///</summary>
    [SerializeField]
    protected Collider                  characterCollider;
    public Collider                     GetCharacterCollider() {  return characterCollider; }
    /// <summary>
    /// �p�V�t�B�b�N�}�e���A��
    /// </summary>
    [SerializeField]
    protected List<PhysicMaterial> physicMaterials = new List<PhysicMaterial>();
    ///<summary>
    ///Rigidbody
    ///</summary>
    protected Rigidbody                 characterRb;
    public Rigidbody                    GetCharacterRb() {  return characterRb; }
    /// <summary>
    /// �L�����N�^�[�̈ړ��֘A�ϐ�
    /// </summary>
    protected Vector3                   velocity;
    /// <summary>
    /// �L�����N�^�[�̈ړ��֘A�ϐ�
    /// </summary>
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
    //���͂��Ă��邩����
    [SerializeField]
    protected bool                      input = false;
    public bool                         InputFlag { get { return input; } set { input = value; } }
    //���Ⴊ�݊֌W
    protected bool                      scrouchFromMove = false;
    //�����t���O
    protected bool                      decele = false;
    public bool                         DeceleFlag { get { return decele; } set { decele = value; } }
    /// <summary>
    /// �U���֘A
    /// </summary>
    //���ݕ�����o���Ă��邩�ǂ���
    protected bool                      currentBattle = false;
    public bool                         CurrentBattleFlag {  get { return currentBattle; } set { currentBattle = value; } }
    //������o���Ă������o���Ă��Ȃ������ǂ���
    protected bool                      previousState = false;
    public bool                         PreviousStateFlag { get { return previousState; } set { previousState = value; } }
    //�A���̌��݂̃J�E���g
    protected int                       attackCount = 0;
    public int                          AttackCount { get { return attackCount; } set { attackCount = value; } }
    //�A�����ł��邩�J�E���g����ϐ�
    protected int                       burstAttackCount = 0;
    //�W�����v�U���t���O
    protected bool                      jumpAttack = false;
    public bool                         JumpAttackFlag { get { return jumpAttack; }set { jumpAttack = value; } }
    //�O�i�W�����v���s�����߂̃t���O
    protected bool                      burstJump = false;
    public bool                         BurstJumpFlag { get { return burstJump; }set { burstJump = value; } }
    /// <summary>
    /// �J�E���g�_�E���N���X
    /// </summary>
    protected CharacterTimerController    timer = null;
    public CharacterTimerController     GetTimer() { return timer; }
    
    //�A�j���[�V����
    [SerializeField]
    protected Animator                  anim;
    public Animator                     GetAnim() {  return anim; }
    /// <summary>
    /// �n�ʐڒn����
    /// </summary>
    protected GroundCheck               check;
    [Header("�L�����N�^�[�̏��")]
    //��Ԃ̃C���X�^���X
    [SerializeField]
    protected ActionState               currentState = ActionState.Null;
    public ActionState                  GetCurrentState() { return currentState;}
    [SerializeField]
    protected ActionState               pastState = ActionState.Null;
    public ActionState                  GetPastState() { return pastState;}
    //���ݔ���
    protected bool                      moveStop = false;
    public bool                         MoveStopFlag { get { return moveStop; }set { moveStop = value; } }
    private float                       repelPower = 5f;      
    //���S����
    protected bool                      die = false;
    public bool                         IsDied() { return die; }
    [SerializeField]
    //���n����
    protected bool                      landing = false;
    public bool                         Landing { get { return landing; } set { landing = value; } }
    //����ʒu�ւ��N���X
    protected PlayerPropsSetting        toolSetting;
    public PlayerPropsSetting           GetPropssetting() { return  toolSetting; }
    //�I�u�W�F�N�g�J���[�`�F���W�N���X
    protected ColorChange               colorChange;
    /// <summary>
    /// �G�t�F�N�g�֘A
    /// </summary>
    public enum EffectState
    {
        Null = -1,
        Die,
        Damage,
        Foot,
        DataEnd
    }
    //�G�t�F�N�g�����N���X
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
            Debug.LogError("�f�[�^���������܂���ł����B");
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
            Debug.LogError("damageObjects���A�^�b�`����Ă��܂���"); 
        }

        characterRb =               GetComponent<Rigidbody>();
        if(characterRb == null)     
        {
            Debug.LogError("RigidBody���A�^�b�`����Ă��܂���");
        }
        anim =                      GetComponent<Animator>();
        if (anim == null)           {
            Debug.LogError("Animtor���A�^�b�`����Ă��܂���");
        }

        timer =                     new CharacterTimerController();
        if(timer == null)
        {
            Debug.LogError("timer����������܂���ł���");
        }
        else
        {
            timer.InitializeAssignTimer();
        }

        colorChange =               new ColorChange();
        if(colorChange == null)
        {
            Debug.LogError("colorChange����������܂���ł���");
        }
        else
        {
            colorChange.GetSetSkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if(colorChange.GetSetSkinnedMeshRenderer == null)
            {
                Debug.LogError("skinnedMeshRenderer���A�^�b�`����Ă��܂���");
            }
            else
            {
                colorChange.InitColor();
            }
        }
        generateEffects =           GetComponent<GenerateEffects>();
        if(generateEffects == null) 
        {
            Debug.LogError("generateEffects���A�^�b�`����Ă��܂���");
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
        //�I�u�W�F�N�g�J���[�̍X�V(��ɐF��ύX�������Ɏg��)
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
    //�U�����鎞�ɃL�����N�^�[��O�i�����郁�\�b�h
    public void AttackForwardAccele(float power)
    {
        characterRb.AddForce(transform.forward * power, ForceMode.VelocityChange);
    }

    public void RollingForwardAccele(Vector3 force)
    {
        characterRb.AddForce(force, ForceMode.Acceleration);
    }
    //�����ŗ^����ړ��ʂ��w��
    public void MoveXZ(Vector3 force)
    {
        velocity += force;
    }

    public void ForwardAccele(Vector3 force)
    {
        characterRb.velocity = Vector3.zero;
        velocity += force;
    }

    //�m�b�N�o�b�N��������(�����̓m�b�N�o�b�N�̈З�)
    protected void Knockback(float power)
    {
        characterRb.AddForce(-transform.forward * power, ForceMode.VelocityChange);
    }

    protected virtual void Die()
    {
        die = true;
        //���S��̂������Ȃ��悤��
        characterRb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
