using UnityEngine;
using UnityEngine.AI;
using static ColorChange;
using static CharacterManager;


public class Slime : EnemyBase
{
    private EnemyPropsSetting enemyTool;
    public override void Start()
    {
        base.Start();
        
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) 
        {
            Debug.Log("NavMeshAgentがアタッチされていません"); 
        }
        enemyTool = GetComponent<EnemyPropsSetting>();
        if(enemyTool == null)
        {
            Debug.LogError("enemyToolがアタッチされていません");
        }
        InitializeFlag();
        SetNavMeshAgentData();
        SetRandomTargetPostion();
    }

    private void SetNavMeshAgentData()
    {
        agent.speed = scriptableData.MaxSpeed;
        agent.acceleration = scriptableData.Acceleration;
    }
    private int RandomWaitCount()
    {
        int count = 0;
        count = Random.Range(1, 10);
        return count;
    }

    private void SetRandomTargetPostion()
    {
        // ランダムな位置を設定する
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            targetPosition = hit.position;
        }
        else
        {
            SetRandomTargetPostion();
        }
        setRandomPos = true;
        //NavMeshAgentの目標位置を設定する
        agent.SetDestination(targetPosition);
    }
    protected override void Update()
    {
        base.Update();
        if (Mathf.Approximately(Time.timeScale, 0f)) { return; }
        ReturnBaseSize();
        TimerUpdate();
        if (!die)
        {
            if(currentState == ActionState.Attack)
            {
                if (!enemyTool.IsActiveDamageObject())
                {
                    enemyTool.ActiveDamageObject(true);
                }
            }
            else
            {
                if (enemyTool.IsActiveDamageObject())
                {
                    enemyTool.ActiveDamageObject(false);
                }
            }
            //入力を行う
            StateInput();
            Command();
        }
        //アニメーションが終了したら
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            //入力が終わった時のモーション切り替え
            InputEndMotion(currentState);
        }
    }

    private void TimerUpdate()
    {
        if (!tracking)
        {
            if (!timer.GetTimer_Wait().IsEnabled())
            {
                if (!setRandomPos)
                {
                    SetRandomTargetPostion();
                }
            }
        }
        if (timer_AttackCoolDown.IsEnabled())
        {
            timer_AttackCoolDown.Update();
        }
        else
        {
            timer_AttackCoolDown.End();
        }
        /*
        if (die)
        {
            if (GetTimer().GetTimer_DieWait().IsEnabled())
            {
                GetTimer().GetTimer_DieWait().Update();
            }
            else
            {
                GetTimer().GetTimer_DieWait().End();
            }
        }
        */
    }

    private void ReturnBaseSize()
    {
        if (flatBodyFlag&&!die)
        {
            if (BaseScale.y > transform.localScale.y)
            {
                // Y軸のスケールを変更する
                float newYScale = Mathf.Lerp(transform.localScale.y, BaseScale.y, 1f * Time.deltaTime);
                transform.localScale = new Vector3(transform.localScale.x, newYScale, transform.localScale.z);
            }
            else
            {
                transform.localScale = BaseScale;
                flatBodyFlag = false;
            }
        }
    }
    public override void SetState(ActionState _state,EnemyState _enemyState)
    {
        if (die)
        {
            return;
        }
        ChangeMotionState(_state);
        if(currentState == ActionState.Tracking&&
            !playerController.IsDied())
        {
            tracking = true;
        }
        else
        {
            tracking = false;
            GetTimer().GetTimer_Wait().StartTimer(RandomWaitCount());
        }
    }
    private void StateInput()
    {
        if (die)
        {
            return;
        }
        input = false;
    }

    private void Command()
    {
        if (die)
        {
            return;
        }
        Patrolling();
        //下は追跡中の処理
        MoveTracking();
    }
    private void Patrolling()
    {
        if (!GetTimer().GetTimer_Wait().IsEnabled() && !tracking && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (setRandomPos)
            {
                setRandomPos = false;
                GetTimer().GetTimer_Wait().StartTimer(RandomWaitCount());
                ChangeMotionState(ActionState.Run);
            }
        }
        else if (!GetTimer().GetTimer_Wait().IsEnabled() && !tracking)
        {
            input = true;
            ChangeMotionState(ActionState.Run);
        }
    }
    private void MoveTracking()
    {
        if (!tracking) { return; }
        //プレイヤーとの距離を計算
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // プレイヤーが指定した距離以内にいる場合は追跡を停止
        if (!GetTimer().GetTimer_Invincible().IsEnabled() && !timer_AttackCoolDown.IsEnabled() &&
            distanceToPlayer <= stopDistance)
        {
            agent.isStopped = true;
            agent.SetDestination(transform.position);
            input = true;
            timer_AttackCoolDown.StartTimer(50);
            //目的地を現在の位置に設定する
            transform.LookAt(player.transform.position);
            Attack();
        }
        else if (distanceToPlayer > stopDistance)
        {
            // プレイヤーが指定した距離より遠い場合は追跡を再開
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
    }
    private void Attack()
    {
        ChangeMotionState(ActionState.Attack);
        AttackForwardAccele(scriptableData.AttackForwardPower);
    }

    private void ChangeMotionState(ActionState state)
    {
        if (currentState == state)
        {
            return;
        }
        string animName = null;
        switch (state)
        {
            case ActionState.Idle:
                animName = "Idle";
                break;
            case ActionState.Run:
                animName = "Walk";
                break;
            case ActionState.Jump:
                animName = "Jump";
                break;
            case ActionState.Attack:
                animName = "Attack";
                break;
            case ActionState.Tracking:
                animName = "Walk";
                break;
            case ActionState.Damage:
                animName = "Damage_00";
                break;
            case ActionState.Die:
                animName = "Damage_02";
                break;
        }
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool runAction = IsNoPlayMotion(animInfo);
        if (runAction && animInfo.normalizedTime < 1.0f)
        {
            return;
        }
        currentState = state;
        anim.Play(animName);
    }

    private bool IsNoPlayMotion(AnimatorStateInfo info)
    {
        bool noPlay = false;
        if (NoPlayMotionName(info))
        {
            noPlay = true;
        }
        return noPlay;
    }

    private bool NoPlayMotionName(AnimatorStateInfo info)
    {
        bool flag = false;
        string[] noPlayMotions = { "Attack" , "Damage_00" , "Damage_02" };
        foreach (string motion in noPlayMotions)
        {
            if (info.IsName(motion))
            {
                flag = true;
            }
        }
        return flag;
    }

    private void InputEndMotion(ActionState state)
    {
        switch (state)
        {
            case ActionState.Run:
            case ActionState.Jump:
            case ActionState.Attack:
            case ActionState.Tracking:
                ChangeMotionState(ActionState.Idle);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "DamageFoot")
        {
            Damage(other);
        }
        if (other.tag == "Damage" || other.tag == "Bullet")
        {
            Damage(other);
        }
        else if (other.tag == "Guard")
        {
            Repelled(other);
        }
    }

    private void Damage(Collider other)
    {
        BaseDamageObject damager = other.GetComponent<BaseDamageObject>();
        if (damager != null && !GetTimer().GetTimer_Invincible().IsEnabled() && !die)
        {
            hp -= DamageMagnification(damager.GetSetDamage);
            enemyUIStatus.SetHp(hp,maxHP);
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
            if(generateEffects != null)
            {
                generateEffects.GenerateEffect((int)EffectState.Damage, transform.position);
            }
            BodyChangeByDamageTag(other);
        }
    }

    private void BodyChangeByDamageTag(Collider other)
    {
        if(other.tag == "Damage"||other.tag == "Bullet")
        {
            colorChange.StartColorTransition(Transitions.damage);
            Knockback(scriptableData.KnockBackPower);
        }
        else if(other.tag == "DamageFoot")
        {
            colorChange.StartColorTransition(Transitions.damage);
            FlatBody();
        }
    }

    private void FlatBody()
    {
        flatBodyFlag = true;
        transform.localScale = new Vector3(transform.localScale.x, 0.1f, transform.localScale.z);
    }

    protected override void Die()
    {
        base.Die();
        for(int i = 0; i < damageObjects.Count; i++)
        {
            Destroy(damageObjects[i].gameObject);
        }
        ChangeMotionState(ActionState.Damage);
        agent.SetDestination(transform.position);
        GetTimer().GetTimer_DieWait().StartTimer(1f);
        GetTimer().GetTimer_DieWait().OnCompleted += () =>
        {
            seController.NoMotionByPlaySE(EnemySEController.EnemySETag.Die);
            DieEffect();
        };
        //エネミーを倒した数をエネミーが死んだ時にカウント
        gameController.KillCount++;
    }

    private void Repelled(Collider other)
    {
        agent.SetDestination(transform.position);
        Knockback(100);
    }

    private float DamageMagnification(float _damage)
    {
        float damage = _damage;
        string[] damageMagnificationname = { "jumpAttack", "spinAttack" , "shieldSlach" };
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
}
