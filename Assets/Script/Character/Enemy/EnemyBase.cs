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
    /// プレイヤーオブジェクトのコンポーネント
    /// </summary>
    protected GameObject        player;
    //プレイヤーコントローラー
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
    /// NavMeshAgen変数
    /// </summary>
    protected NavMeshAgent agent;
    //非追跡時の巡回ポイント
    protected Vector3 targetPosition = Vector3.zero;
    //追跡するかどうかの判定フラグ
    protected bool tracking = false;

    protected float patrolRadius = 10f;

    protected bool setRandomPos = false;

    //攻撃後のクールダウンカウント
    protected CountDown timer_AttackCoolDown;

    protected float stopDistance = 2f;

    [SerializeField]
    protected GameController gameController;

    //UI管理スクリプト(現在はHPゲージだけ)
    [SerializeField]
    protected EnemyUIStatus enemyUIStatus;

    //現在自分はプレイヤーに注目されてるか判断する
    [Header("プレイヤーに注目されているか")]
    [SerializeField]
    protected bool focusByMeFlag = false;
    public bool GetSetFocusByMeFlag { get { return focusByMeFlag; } set { focusByMeFlag = value; } }

    //オブジェクトのサイズを変える
    protected Vector3 BaseScale;
    //ダメージを受けた時の変化の判別
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
            Debug.LogError("HPBarのタグが付いたオブジェクトがありません");
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
            Debug.LogError("Colliderがアタッチされていません"); 
        }
        GameObject g = GameObject.FindGameObjectWithTag("Controller");
        if (gameController == null) 
        {
            Debug.LogError("gameControllerがアタッチされていません"); 
        }
        else
        {
            gameController = g.GetComponent<GameController>();
        }
        enemyUIStatus = GetComponent<EnemyUIStatus>();
        if (enemyUIStatus == null) 
        {
            Debug.LogError("enemyUIStatusがアタッチされていません"); 
        }
        seController = GetComponent<EnemySEController>();
        if(seController == null) 
        {
            Debug.LogError("enemySEControllerがアタッチされていません"); 
        }
        timer_AttackCoolDown = new CountDown();
        if (timer_AttackCoolDown == null) 
        {
            Debug.LogError("timer_AttackCoolDownがコンポーネントされませんでした(Slime)"); 
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
