using UnityEngine;
using static ColorChange;
using static CharacterManager;

public class Skeletron : EnemyBase
{
    private EnemyPropsSetting   enemyToolSetting;


    private float               horizontal = 0;

    private float               vertical = 0;

    [SerializeField]
    private int                 minStop = 1;
    [SerializeField]
    private int                 maxStop = 4;

    [SerializeField]
    private int                 minMove = 1;
    [SerializeField]
    private int                 maxMove = 5;

    [SerializeField]
    private float               maxDistance = 2.5f;

    [SerializeField]
    private float               attackEnabledRangeDistance = 1.5f;

    [SerializeField]
    private bool                resetDistance = false;

    private bool                pastTracking = false;

    public override void Start()
    {
        base.Start();

        resetDistance = false;

        enemyToolSetting = GetComponent<EnemyPropsSetting>();
        if(enemyToolSetting == null)
        {
            Debug.LogError("enemyToolSettingがアタッチされていません");
        }
        else
        {
            enemyToolSetting.ActiveDamageCollider(false);
        }
        enemyState = EnemyState.ResetPosition;
        currentState = ActionState.Idle;
        pastState = currentState;

        pastTracking = tracking;
    }

    protected override void Update()
    {
        base.Update();
        if (Mathf.Approximately(Time.timeScale, 0f)) { return; }
        PropsSetting();

        if (die) { return; }
        FocusPlayer();

        EndMotion();
        if (pastTracking != tracking)
        {
            ChangeMotionState(ActionState.Idle);
            pastTracking = tracking;
            return;
        }
        switch (enemyState)
        {
            case EnemyState.Tracking:
                TrackingInput();
                TrackingCommand();
                break;
            case EnemyState.ResetPosition:
                InitalizeVelocity();
                ChangeMotionState(ActionState.Idle);
                break;
        }
        Move();
    }

    private void FocusPlayer()
    {
        if (tracking)
        {
            if(player == null) { return; }
            transform.LookAt(player.transform.position);
            // プレイヤーの方向ベクトルを取得
            Vector3 targetObject = player.transform.position;
            targetObject.y = transform.position.y;
            Vector3 targetDirection = (targetObject - transform.position).normalized;
            Quaternion enemyRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            //敵が常にプレイヤーの方向を向く
            transform.rotation = enemyRotation;
        }
    }

    private void PropsSetting()
    {
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        if(currentState == ActionState.Attack&&animInfo.normalizedTime > 0.25f&&
            anim.GetInteger("State") == 6)
        {
            enemyToolSetting.ActiveDamageCollider(true);
        }
    }

    public override void SetState(ActionState _state, EnemyState _enemyState)
    {
        if (die)
        {
            return;
        }
        enemyState = _enemyState;
        if (enemyState == EnemyState.Tracking &&
            !playerController.IsDied())
        {
            tracking = true;
        }
        else
        {
            tracking = false;
        }
    }

    private void TrackingInput()
    {
        //プレイヤーとの距離を保つ
        KeepDistance();
        //ランダムで行動を決定
        SetRandomMove();
        AttackInput();
    }

    private void KeepDistance()
    {
        float dis = Vector3.Distance(transform.position, player.transform.position);
        if(dis > maxDistance)
        {
            resetDistance = true;
            vertical = 1f;
            ChangeMotionState(ActionState.Run);
        }
        else
        {
            resetDistance = false;
            vertical = 0f;
        }
    }
    private void SetRandomMove()
    {
        if(resetDistance) { return; }
        if (timer.GetTimer_Wait().IsEnabled()) { return; }
        if (timer.GetTimer_Move().IsEnabled()) { return; }
        int state = Random.Range(0, 2);
        switch (state)
        {
            case 0:
                StopInput();
                break;
            case 1:
                MoveInput();
                break;
            case 2:
                break;
        }
    }

    private void StopInput()
    {
        timer.GetTimer_Wait().StartTimer(Random.Range(minStop,maxStop));
        horizontal = 0;
        vertical = 0;
        InitalizeVelocity();
        ChangeMotionState(ActionState.Idle);
    }

    private void MoveInput()
    {
        timer.GetTimer_Move().StartTimer(Random.Range(minMove, maxMove));
        int side = Random.Range(0, 2);
        vertical = 0;
        switch (side)
        {
            case 0:
                horizontal = 1;
                break;
            case 1:
                horizontal = -1;
                break;
        }
        ChangeMotionState(ActionState.Run);
    }

    private void AttackInput()
    {
        float dis = Vector3.Distance(transform.position, player.transform.position);
        if(dis < attackEnabledRangeDistance)
        {
            int attcking = Random.Range(0, 2);
            if(attcking > 0)
            {
                horizontal = 0;
                vertical = 0;
                velocity = Vector3.zero;
                ChangeMotionState(ActionState.Attack);
                enemyToolSetting.ActiveShildCollider(false);
            }
        }
    }

    private void TrackingCommand()
    {
        //敵の前を取得
        Vector3 enemyForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
        //敵の右を取得
        Vector3 enemyRight = Vector3.Scale(transform.right, new Vector3(1, 0, 1)).normalized;

        float maxSpeed = scriptableData.MaxSpeed;
        float accele = scriptableData.Acceleration;
        Accele(enemyForward, enemyRight,maxSpeed,accele);
    }

    private void Accele(Vector3 forward,Vector3 right,float maxspeed,float accele)
    {
        Vector3 vel = velocity;
        float h = horizontal;
        float v = vertical;
        vel += (h * right + v * forward) * accele;
        // 現在の速度の大きさを計算
        float currentSpeed = vel.magnitude;
        // もし現在の速度が最大速度未満ならば、加速度を適用する
        // 現在の速度が最大速度以上の場合は速度を最大速度に制限する
        if (currentSpeed >= maxspeed)
        {
            vel = vel.normalized * maxspeed;
        }
        velocity = vel;
    }

    private void ChangeMotionState(ActionState state)
    {
        string name = "State";
        int number = 0;
        switch (state)
        {
            case ActionState.Idle:
                if (tracking)
                {
                    number = 1;
                }
                else
                {
                    number = 0;
                }
                break;
            case ActionState.Run:
                number = SetRunMotion();
                break;
            case ActionState.Attack:
                number = 6;
                break;
            case ActionState.Damage:
                number = 7;
                break;
            case ActionState.Die:
                number = 8;
                break;
        }
        pastState = currentState;
        currentState = state;
        anim.SetInteger(name, number);
    }

    private int SetRunMotion()
    {
        int number = 1;
        if(vertical != 0)
        {
            if(vertical > 0)
            {
                number = 2;
            }
        }
        else
        {
            if(horizontal > 0)
            {
                number = 3;
            }
            else
            {
                number = 5;
            }
        }
        return number;
    }

    private void EndMotion()
    {
        AnimatorStateInfo currentAnimState = anim.GetCurrentAnimatorStateInfo(0);
        if(currentAnimState.normalizedTime >= 1.0f && !anim.IsInTransition(0))
        {
            switch (currentState)
            {
                case ActionState.Attack:
                case ActionState.Damage:
                    enemyToolSetting.ActiveShildCollider(true);
                    enemyToolSetting.ActiveDamageCollider(false);
                    timer.GetTimer_Wait().StartTimer(Random.Range(minStop, maxStop));
                    ChangeMotionState(ActionState.Idle);
                    break;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //自身の武器がプレイヤーの盾に当たった時
        SwordDamage sword = enemyToolSetting.GetDamageObject().GetComponent<SwordDamage>();
        if(sword.HitGuardFlag)
        {
            sword.HitGuardFlag = false;
            Knockback(scriptableData.KnockBackPower);
            return;
        }

        //ダメージを受けた時
        if (other.tag == "Damage")
        {
            if (enemyToolSetting.IsActiveShildCollider()) { return; }
            Damage(other);
        }
    }

    private void Damage(Collider other)
    {
        BaseDamageObject damager = other.GetComponent<BaseDamageObject>();
        if (damager != null && !GetTimer().GetTimer_Invincible().IsEnabled() && !die)
        {
            hp -= DamageMagnification(damager.GetSetDamage);
            enemyUIStatus.SetHp(hp, maxHP);
            if (hp > 0)
            {
                //プレイヤーの剣が当たったらダメージモーション発生
                ChangeMotionState(ActionState.Damage);
            }
            else
            {
                Die();
            }
            seController.NoMotionByPlaySE(EnemySEController.EnemySETag.Damage);
            GetTimer().GetTimer_Invincible().StartTimer(scriptableData.InvincibleTimerCount);
            if (generateEffects != null)
            {
                generateEffects.GenerateEffect((int)EffectState.Damage, other.transform.position);
            }
            BodyChangeByDamageTag(other);
        }
    }

    private void BodyChangeByDamageTag(Collider other)
    {
        if (other.tag == "Damage")
        {
            colorChange.StartColorTransition(Transitions.damage);
            Knockback(scriptableData.KnockBackPower);
        }
    }

    protected override void Die()
    {
        base.Die();
        ChangeMotionState(ActionState.Die);
        enemyToolSetting.ActiveDamageCollider(false);
        enemyToolSetting.ActiveShildCollider(false);
        GetTimer().GetTimer_DieWait().StartTimer(2f);
        GetTimer().GetTimer_DieWait().OnCompleted += () =>
        {
            seController.NoMotionByPlaySE(EnemySEController.EnemySETag.Die);
            DieEffect();
        };
        //エネミーを倒した数をエネミーが死んだ時にカウント
        gameController.KillCount++;
    }

    private float DamageMagnification(float _damage)
    {
        float damage = _damage;
        string[] damageMagnificationname = { "jumpAttack", "spinAttack", "shieldSlach" };
        Animator anim = player.GetComponent<Animator>();
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        foreach (string motion in damageMagnificationname)
        {
            bool magnificationFlag = false;
            //現在は指定されたモーションなら全て1.5倍されるようになる
            if (info.IsName(motion))
            {
                magnificationFlag = true;
            }
            if (magnificationFlag)
            {
                switch (motion)
                {
                    case "jumpAttack":
                        damage *= 1.5f;
                        break;
                    case "spinAttack":
                        damage *= 2.0f;
                        break;
                    case "shieldSlach":
                        damage *= 0.25f;
                        break;
                }
                break;
            }
        }
        return damage;
    }

    private void OnDestroy()
    {
        if(gameObject.name != "BossSkeletron") { return; }
        ClearFlag.SetClearFlag(true);
    }
}
