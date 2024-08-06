using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static CharacterManager;

public enum EnemyState
{
    Null = -1,
    Tracking,
    ResetPosition,
    DataEnd,
}


public class EnemyBase : CharacterController
{
    [SerializeField]
    protected EnemyState        enemyState = EnemyState.Null;

    /// <summary>
    /// �v���C���[�I�u�W�F�N�g�̃R���|�[�l���g
    /// </summary>
    protected GameObject        player;
    //�v���C���[�R���g���[���[
    protected PlayerController  playerController;
    public PlayerController     GetPlayerController() { return playerController; }
    public void SetPlayerScript(GameObject _player)
    {
        if (player != null) { return; }
        player = _player;
        playerController = player.GetComponentInParent<PlayerController>();
    }
    public void UnlockPlayerScript(GameObject _player)
    {
        if (player == null) { return; }
        player = null;
    }
    [SerializeField]
    protected SphereCollider sphereCollider;
    /// <summary>
    /// NavMeshAgen�ϐ�
    /// </summary>
    protected NavMeshAgent agent;
    //��ǐՎ��̏���|�C���g
    protected Vector3 targetPosition = Vector3.zero;
    //�ǐՂ��邩�ǂ����̔���t���O
    protected bool tracking = false;

    protected float patrolRadius = 10f;

    protected bool setRandomPos = false;

    //�U����̃N�[���_�E���J�E���g
    protected CountDown timer_AttackCoolDown;

    protected float stopDistance = 2f;

    [SerializeField]
    protected GameController gameController;

    //UI�Ǘ��X�N���v�g(���݂�HP�Q�[�W����)
    [SerializeField]
    protected EnemyUIStatus enemyUIStatus;

    //���ݎ����̓v���C���[�ɒ��ڂ���Ă邩���f����
    [Header("�v���C���[�ɒ��ڂ���Ă��邩")]
    [SerializeField]
    protected bool focusByMeFlag = false;
    public bool GetSetFocusByMeFlag { get { return focusByMeFlag; } set { focusByMeFlag = value; } }

    //�I�u�W�F�N�g�̃T�C�Y��ς���
    protected Vector3 BaseScale;
    //�_���[�W���󂯂����̕ω��̔���
    protected bool flatBodyFlag = false;

    public bool GetFlatBodyFlag { get { return flatBodyFlag; } }


    [SerializeField]
    protected EnemySEController seController;
    public override void Start()
    {
        base.Start();
        AllGetComponent();

        Transform t = enemyUIStatus.GetHPUI().transform.Find("HPBar");
        if(t == null)
        {
            Debug.LogError("HPBar�̃^�O���t�����I�u�W�F�N�g������܂���");
        }
        else
        {
            enemyUIStatus.HPSlider = t.GetComponent<Slider>();
            enemyUIStatus.HPSlider.value = 1f;
        }

        tracking = false;
        setRandomPos = false;
        input = false;

        for(int i = 0; i < damageObjects.Count; i++)
        {
            damageObjects[i].GetSetDamage = scriptableData.AttackPower;
        }
        BaseScale = transform.localScale;
    }
    public override void AllGetComponent()
    {
        base.AllGetComponent();
        characterCollider = GetComponent<Collider>();
        if (characterCollider == null) 
        {
            Debug.LogError("Collider���A�^�b�`����Ă��܂���"); 
        }
        GameObject g = GameObject.FindGameObjectWithTag("Controller");
        if (gameController == null) 
        {
            Debug.LogError("gameController���A�^�b�`����Ă��܂���"); 
        }
        else
        {
            gameController = g.GetComponent<GameController>();
        }
        enemyUIStatus = GetComponent<EnemyUIStatus>();
        if (enemyUIStatus == null) 
        {
            Debug.LogError("enemyUIStatus���A�^�b�`����Ă��܂���"); 
        }
        seController = GetComponent<EnemySEController>();
        if(seController == null) 
        {
            Debug.LogError("enemySEController���A�^�b�`����Ă��܂���"); 
        }
        timer_AttackCoolDown = new CountDown();
        if (timer_AttackCoolDown == null) 
        {
            Debug.LogError("timer_AttackCoolDown���R���|�[�l���g����܂���ł���(Slime)"); 
        }
    }

    protected override void Update()
    {
        base.Update();
        if (focusByMeFlag&&!enemyUIStatus.IsActiveStatusUI())
        {
            enemyUIStatus.ActiveStatusUI(true);
        }
    }

    public virtual void SetState(ActionState _state,EnemyState _enemyState)
    {
        if (die)
        {
            return;
        }
        
        if (currentState == ActionState.Tracking &&
            !playerController.IsDied())
        {
            tracking = true;
        }
        else
        {
            tracking = false;
        }
    }

    protected void InitalizeVelocity()
    {
        velocity = Vector3.zero;
    }

    protected void DieEffect()
    {
        playerController.SetFocusFlag(false);
        if (generateEffects != null)
        {
            generateEffects.GenerateEffect((int)EffectState.Die, transform.position);
        }
        Destroy(gameObject);
    }
}
