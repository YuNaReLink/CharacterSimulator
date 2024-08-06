using UnityEngine;
using static ColorChange;
using static GameDataManager;
using static CharacterManager;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerController : CharacterController
{
    ///<summary>
    ///PlayerオブジェクトにUnity上でコンポーネントするクラスのインスタンス
    ///</summary>
    [Header("プレイヤーの入力管理クラス")]
    [SerializeField]
    private PlayerInput             stateInput;
    public PlayerInput              GetStateInput() { return stateInput; }
    [Header("プレイヤーの移動管理クラス")]
    [SerializeField]
    private PlayerMovement          stateMovement;
    public PlayerMovement           GetStateMovement() {  return stateMovement; }
    [Header("地形との当たり判定管理クラス")]
    [SerializeField]
    private ObstacleDetect          obstacleDetect;
    public ObstacleDetect           GetWallClimb() {  return obstacleDetect; }
    [Header("SE管理クラス")]
    [SerializeField]
    private PlayerSEController      seController;
    public PlayerSEController       GetSEController() { return seController; }
    /// <summary>
    /// プレイヤーの行動のクラス
    /// motion:モーションの管理
    /// jump  :ジャンプの管理
    /// avoid :回避(ローリング)の管理
    /// gun   :銃の管理
    /// attack:攻撃の管理
    /// guard :防御の管理
    /// </summary>
    private PlayerMotionController      motion = null;
    private InterfaceJumpCommand        jump = null;
    private InterfaceAvoidanceCommand   avoid = null;
    private GunCommand                  gun = null;
    private InterfaceAttackCommand      attack = null;
    private GuardCommand                guard = null;
    /// <summary>
    /// プレイヤーが使う銃の弾を発射するためのクラス
    /// </summary>
    [Header("銃の弾の発射を管理するクラス")]
    [SerializeField]
    private BulletShot              bulletShot;
    public BulletShot               GetBulletShot() { return bulletShot; }
    /// <summary>
    /// Mainカメラに関連の変数、インスタンスなど
    /// </summary>
    [Header("カメラ関係のコンポーネント")]
    [SerializeField]
    private FocusObject             focusObject;
    public FocusObject              GetFocusObject() {  return focusObject; }
    public void                     SetFocusFlag(bool flag) { focusObject.FocusFlag = flag; }
    [SerializeField]
    private GameObject              ReSetFocusObject;
    [SerializeField]
    private TPSCamera               tpsCamera;
    public TPSCamera                GetTPSCamera() { return tpsCamera; }
    /// <summary>
    /// カメラの回転関係
    /// </summary>
    //プレイヤーの現在の位置
    private Vector3                 currentPos;
    public Vector3                  GetCurrentPos() { return currentPos; }
    //プレイヤーの過去の位置
    private Vector3                 pastPos;
    public Vector3                  GetPastPos() { return pastPos; }
    //プレイヤーの移動量
    private Vector3                 cameravelocity;
    //プレイヤーの進行方向に向くクォータニオン
    private Quaternion              playerRot;
    //現在の回転各速度
    private float                   currentAngularVelocity;
    //最大の回転角速度[deg/s]
    [SerializeField]
    private float                   maxAngularVelocity = Mathf.Infinity;
    //進行方向にかかるおおよその時間[s]
    [SerializeField]
    private float                   smoothTime = 0.1f;
    //現在の向きと進行方向の角度
    private float                   diffAngle;
    //現在の回転する角度
    private float                   rotAngle;

    protected bool[]                avoidFlag = new bool[(int)AvoidState.DataEnd];
    public bool[]                   AvoidFlag { get { return avoidFlag; } set { avoidFlag = value; } }
    [SerializeField]
    protected AttackStateCount      attackState = AttackStateCount.Null;
    public AttackStateCount         AttackState { get { return attackState; } set { attackState = value; } }

    /// <summary>
    /// ジャンプ関係
    /// </summary>
    protected int                   jumpCount = 0;
    public int                      JumpCount { get { return jumpCount; } set { jumpCount = value; } }
    private JumpState               jumpState = JumpState.Null;
    public JumpState                JumpState { get { return jumpState; } set { jumpState = value; } }
    [SerializeField, Min(0)]
    private float                   maxJumpTime = 0.1f;
    public float                    GetMaxJumpTime() { return maxJumpTime; }
    private bool                    jumping = false;
    public bool                     JumpingFlag { get { return jumping; } set { jumping = value; } }
    private float                   jumpTime = 0;
    public float                    JumpTime { get { return jumpTime; } set { jumpTime = value; } }
    //キーの長押しでジャンプ力が変わる時に
    //ジャンプしたパワーを保持する変数
    [SerializeField]
    private float                   jumpingPower = 0;
    public float                    JumpingPower { get { return jumpingPower; } set { jumpingPower = value; } }

    /// <summary>
    /// ヒップドロップ関係
    /// </summary>
    private bool                    rotate = false;
    public bool                     RotateFlag { get { return rotate; } set { rotate = value; } }
    private float                   rotateQuantity = 0;
    private float                   rotateY = 0;
    public float                    RotateY { get { return rotateY; } set { rotateY = value; } }

    /// <summary>
    /// UI関係
    /// </summary>
    //LifeGaugeスクリプト
    [SerializeField]
    protected LifeGauge             lifeGauge;
    [SerializeField]
    protected LifeGauge             lifeFrameGauge;

    private float                   maxRotate = 360.0f;
    private float                   rotatePower = 45.0f;


    /// <summary>
    /// 各変数のデータ
    /// </summary>
    private float jumpingHorizontalRaito = 0.1f;

    private float fallPower = 500f;
    protected override void Awake()
    {
        //Start前にタグを代入
        InitalizePlayerTag();
        base.Awake();
    }
    private void InitalizePlayerTag()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            selectTag = PlayerTag;
        }
    }

    public override void Start()
    {
        base.Start();
        AllGetComponent();
        InitializeFlag();
        toolSetting?.InitalizeToolSetting(this);
        obstacleDetect?.SetWallJumpPower(scriptableData.FirstJumpPower);
        

        attackState = AttackStateCount.Null;

        pastPos = transform.position;

        //=====UIの初期化=====
        lifeGauge?.SetLifeGauge(hp);
        lifeFrameGauge?.SetLifeGauge(hp);
    }

    public override void AllGetComponent()
    {
        base.AllGetComponent();
        check =                 GetComponent<GroundCheck>();
        if(check == null) 
        {
            Debug.LogError("checkがアタッチされていません");
        }
        //下記はプレイヤー限定のコンポーネント
        stateInput =            GetComponent<PlayerInput>();
        if (stateInput == null)
        {
            Debug.LogError("stateInputがアタッチされていません");
        }
        else
        {
            stateInput?.SetPlayerController(this);
        }
        obstacleDetect =        GetComponent<ObstacleDetect>();
        if(obstacleDetect == null)
        {
            Debug.LogError("obstacleDetectがアタッチされていません");
        }
        seController =          GetComponent<PlayerSEController>();
        if(seController == null)
        {
            Debug.LogError("seControllerがアタッチされていません");
        }
        toolSetting =          GetComponent<PlayerPropsSetting>();
        if(toolSetting == null)
        {
            Debug.LogError("toolSettingがアタッチされていません");
        }
        //=====Commandクラス生成
        stateMovement =         new PlayerMovement(this);
        motion =                new PlayerMotionController(this);
        switch (selectTag)
        {
            case DataTag.Zelda:
                avoid =         new ZeldaAvoidanceCommand(this);
                attack =        new ZeldaAttackCommand(this);
                guard =         new GuardCommand(this);
                break;
            case DataTag.RatchetAndClank:
                avoid =         new RatchetAndClankAvoidanceCommand(this);
                jump =          new RatchetAndClankJumpCommand(this);
                attack =        new RatchetAndClankAttackCommand(this);
                gun =           new GunCommand(this);
                break;
            case DataTag.SuperMario:
                jump =          new MarioJumpCommand(this);
                attack =        new MarioAttackCommand(this);
                break;
        }

        GameObject g = GameObject.FindGameObjectWithTag("MainCamera");
        if(g == null)
        {
            Debug.LogError("Cameraを取得出来なかった");
        }
        else
        {
            tpsCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TPSCamera>();
        }

        //=====UIのアタッチ=====
        g = GameObject.FindGameObjectWithTag("Life");
        if (g == null && CurrentGameState == GameState.NowGame)
        {
            Debug.LogError("Lifeのタグ付きのオブジェクトが見つかりませんでした");
        }
        else
        {
            lifeGauge =         g.GetComponent<LifeGauge>();
        }
        g = GameObject.FindGameObjectWithTag("LifeFrame");
        if (g == null && CurrentGameState == GameState.NowGame)
        {
            Debug.LogError("LifeFrameのタグ付きのオブジェクトが見つかりませんでした");
        }
        else
        {
            lifeFrameGauge =    g.GetComponent<LifeGauge>();
        }
    }

    protected override void InitializeFlag()
    {
        base.InitializeFlag();
        rotate =    false;
        jumping =   false;
        for (int i = 0; i < (int)AvoidState.DataEnd; i++)
        {
            avoidFlag[i] = false;
        }
    }

    protected override void Update()
    {
        //ゲーム内の時間が止まっているなら早期リターン
        if (Mathf.Approximately(Time.timeScale, 0f)) { return; }
        base.Update();
        
        //特定のアニメーションで条件に合ったら
        motion.MotionPause();

        //入力が終わった時のモーション切り替え
        motion.InputEndMotion(currentState);

        if (die) { return; }
        //スーパーマリオモード時のヒップドロップの回転処理
        HipDropRotateBody();
        toolSetting.PropsUpdateSetting(this);
        toolSetting.SetColliderEnabled(this);

        //入力を行う
        if (currentState != ActionState.Damage && !moveStop)
        {
            KeyInput();
            Command();
        }
    }

    private void HipDropRotateBody()
    {
        //回転フラグがtrueかつ死んでなければ
        if (rotate)
        {
            if (!seController.RotateSEFlag)
            {
                seController.RotateBodySEPlay();
                seController.RotateSEFlag = true;
            }
            if (rotateQuantity <= maxRotate)
            {
                float rotateSpeed = rotateQuantity + rotatePower;
                transform.localRotation = Quaternion.Euler(rotateSpeed, rotateY, 0.0f);
                rotateQuantity += rotatePower;
            }
            else
            {
                rotateQuantity = 0;
                rotate = false;
                seController.RotateSEFlag = false;
                transform.localRotation = Quaternion.Euler(0.0f, rotateY, 0.0f);
            }
        }
    }
    //アタッチしているDamageクラスのWallHitをfalseに
    public void EmptyDamageWallHitFlag()
    {
        for (int i = 0; i < damageObjects.Count; i++)
        {
            damageObjects[i].SetWallHit(false);
        }
    }

    private void KeyInput()
    {
        //入力を解除
        input = false;
        //着地処理
        Landed();
        obstacleDetect.WallActionInput(this);
        InitializeJumpAttackInput();
        stateInput.AllKeyInput();
        //入力の状態をフラグに代入
        input = SetInput(currentState);
    }


    private void Landed()
    {
        //着地判定
        landing = check.CheckGroundStatus();
        //パシフィックマテリアル変更
        SetPhysicMaterial();
        if (!landing){return;}
        //崖ジャンプフラグを可能に
        obstacleDetect.CliffJumpFlag = false;
        //ジャンプ関係
        highJump          = false;
        lengthyJump       = false;
        jumpingPower = 0;
        if (!GetTimer().GetTimer_BurstJump().IsEnabled())
        {
            JudgeNextJump();
        }
        else if (jumpCount == scriptableData.MaxJumpCount)
        {
            jumpState = JumpState.Null;
            jumpCount = 0;
        }
        GetTimer().Timer_LongJump.End();
        //ヒップドロップ関係
        if (stateInput.GetHipDropFlag)
        {
            GetTimer().Timer_StopMove.StartTimer(0.5f);
        }
        stateInput.GetHipDropFlag = false;
        if (currentState == ActionState.HipDrop)
        {
            if (!seController.HipDropSEFlag)
            {
                seController.HipDropSEPlay();
                seController.HipDropSEFlag = true;
            }
            jumpState = JumpState.Null;
            jumpCount = 0;
            burstJump = false;
        }
        else
        {
            seController.HipDropSEFlag = false;
        }
        //重力関係
        GetTimer().Timer_NoGravity.End();
    }

    private void JudgeNextJump()
    {
        if (jumpState == JumpState.Null){return;}
        //1,2回目の着地時の処理
        if (!burstJump && jumpState != JumpState.ThirdJump)
        {
            GetTimer().GetTimer_BurstJump().StartTimer(scriptableData.MaxBurstJumpCount);
            burstJump = true;
        }
        //3回目の着地時の処理
        else if (burstJump || jumpCount == scriptableData.MaxJumpCount)
        {
            jumpState = JumpState.Null;
            jumpCount = 0;
            burstJump = false;
        }
        //それ以外でジャンプ状態がNullじゃなかったら
        else if(!GetTimer().GetTimer_BurstJump().IsEnabled()&& jumpState != JumpState.Null)
        {
            jumpState = JumpState.Null;
            jumpCount = 0;
            burstJump = false;
        }
    }


    protected override void SetPhysicMaterial()
    {
        if (currentState != ActionState.Grab&&(stateInput.JumpDownKey || !landing))
        {
            characterCollider.material = physicMaterials[(int)PhysicState.Jump];
        }
        else
        {
            characterCollider.material = physicMaterials[(int)PhysicState.Land];
        }
    }

    private void InitializeJumpAttackInput()
    {
        if (currentState == ActionState.JumpAttack || !jumpAttack) { return; }
        jumpAttack = false;
    }

    private void Death()
    {
        if (hp > 0) { return; }
        Die();
        colorChange.StartColorTransition(Transitions.damage);
    }

    private void Command()
    {
        //死亡処理
        Death();
        if (GameClearCommand()) { return; }
        //----プレイヤーの移動----
        //カメラに対して前を取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        //カメラに対して右を取得
        Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;
        //フラグによって入力を緩和＆消す
        if (!landing)
        {
            stateInput.Horizontalinput *= jumpingHorizontalRaito;
        }
        //状態によってスピードを変更
        float maxspeed = SetMoveSpeed(scriptableData.MaxSpeed);
        float accele = SetMoveSpeed(scriptableData.Acceleration);
        //入力中なら
        //移動量を足す
        if (input)
        {
            Accele(cameraForward, cameraRight, maxspeed, accele);
        }
        else if(!input && landing)
        {
            //待機状態か入力してないなら
            StopCommand(cameraForward, cameraRight, scriptableData.MinSpeed, scriptableData.Deceleration);
        }
        if(obstacleDetect != null)
        {
            obstacleDetect.Execute(this);
        }
        //ジャンプ処理
        if (jump != null)
        {
            jump.Execute(maxspeed);
        }
        //着地していてローリング状態なら
        if (avoid != null)
        {
            avoid.Execute();
        }
        //銃を撃つ処理
        if (gun != null)
        {
            gun.Execute();
        }
        //マウス操作時の処理
        if (!StopAttackMotion())
        {
            //攻撃処理
            if (attack != null)
            {
                attack.Execute();
            }
            if (stateInput.GetHipDropFlag && !GetTimer().Timer_NoGravity.IsEnabled())
            {
                FastFall(-fallPower);
            }
            //マウス右クリックを押して
            if (guard != null)
            {
                guard.Execute();
            }
            //武器のSE処理
            if (stateInput.IsLeftCtrlKey() && currentState == ActionState.ModeChange)
            {
                seController.WeaponSetSound(currentBattle);
            }
        }
        //移動量を適用する
        Move();
        //----プレイヤーの回転----
        if (focusObject.FocusFlag && stateInput.IsMouseMiddle() && !die)
        {
            Vector3 targetPos = focusObject.GetFocusObjectPosition();
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
        }
        if (StopRotateBody()) { return; }
        //現在の位置
        currentPos = transform.position;
        //移動量計算
        cameravelocity = currentPos - pastPos;
        //yだけ0
        cameravelocity.y = 0;
        //過去の位置を更新
        pastPos = currentPos;
        if (cameravelocity == Vector3.zero&& !focusObject.FocusFlag) { return; }
        //プレイヤーの回転を適用
        transform.rotation = RotationBycamera();
    }

    private bool GameClearCommand()
    {
        bool gameMode = selectTag != DataTag.Zelda;
        if (CurrentGameState == GameState.GameClaer&&
            gameMode)
        {
            FastFall(fallPower);
            if (!gameClearPoseFlag&&landing)
            {
                ChangeMotionState(ActionState.VictoryPose);
                gameClearPoseFlag = true;
            }
            return true;
        }
        return false;
    }

    private float[] dragPowers = new float[]
    {
        10f,
        1f
    };

    protected override float GetDrag()
    {
        float _drag = 0;
        if (currentState == ActionState.GlideJump && !highJump &&
            !GetTimer().Timer_JumpNoInput.IsEnabled())
        {
            _drag = dragPowers[0];
        }
        else if (currentState == ActionState.RangeJump)
        {
            _drag = dragPowers[1];
        }
        else
        {
            _drag = 0;
        }
        return _drag;
    }

    private void Accele(Vector3 forward, Vector3 right, float _maxspeed, float _accele)
    {
        bool stopaccele = GetTimer().Timer_Avoid.IsEnabled();
        if (stopaccele) { return; }
        decele = true;
        velocity = stateMovement.AcceleExecute(forward, right, _maxspeed, _accele);
    }

    private void StopCommand(Vector3 forward, Vector3 right, float _minspeed, float _decele)
    {
        if (!stateInput.IsCrouchKey()&&currentState != ActionState.JumpAttack)
        {
            stateMovement.StopVelocityXZ();
        }
        else
        {
            //減速させる
            stateMovement.Deceleration(forward, right, _minspeed, _decele);
        }
    }

    private float decelePower = 0.9f;

    public override float AddDecelerationSetting(float _decele)
    {
        float d = _decele;
        if (currentState == ActionState.Crouch || currentState == ActionState.JumpAttack)
        {
            d = decelePower;
        }
        else if (!landing && !input)
        {
            d = 0;
        }
        return d;
    }
    public bool CheckAvoidFlag()
    {
        for (int i = 0; i < avoidFlag.Length; i++)
        {
            if (avoidFlag[i])
            {
                return false;
            }
        }
        return true;
    }

    private bool StopAttackMotion()
    {
        bool flag = false;
        switch (currentState)
        {
            case ActionState.Jump:
            case ActionState.Flip:
            case ActionState.RunJump:
            case ActionState.Grab:
            case ActionState.ClimbWall:
                flag = true;
                break;
        }
        return flag;
    }

    private float[] sllowMoveSpeeds = new float[]
    {
        0.4f,
        0.2f
    };

    private float SetMoveSpeed(float _speed)
    {
        switch (currentState)
        {
            case ActionState.ReadySpinAttack:
            _speed *= sllowMoveSpeeds[0];
                break;
            case ActionState.Crawling:
            _speed *= sllowMoveSpeeds[1];
                break;    
        }
        return _speed;
    }

    protected override bool RepelMove()
    {
        if (stateInput.GetHipDropFlag) { return false; }
        base.RepelMove();
        return false;
    }

    private Quaternion RotationBycamera()
    {
        //自身が動いてる場合の注目処理
        if (focusObject.FocusFlag && stateInput.IsMouseMiddle() && !die)
        {
            // 敵の方向ベクトルを取得
            Vector3 targetObject = focusObject.GetFocusObjectPosition();
            targetObject.y = transform.position.y;
            Vector3 enemyDirection = (targetObject - transform.position).normalized;
            //enemyDirection.y = 0;
            // プレイヤーが常に敵の方向を向く
            Quaternion enemyRotation = Quaternion.LookRotation(enemyDirection, Vector3.up);
            return enemyRotation;
        }
        //そうじゃなければ通常の三人称カメラ処理
        else
        {
            playerRot = Quaternion.LookRotation(cameravelocity, Vector3.up);
            diffAngle = Vector3.Angle(transform.forward, cameravelocity);
            //回転速度を計算する
            float targetRotationSpeed = Mathf.Min(diffAngle / smoothTime, maxAngularVelocity);
            // 回転速度を調整する
            currentAngularVelocity = Mathf.MoveTowards(currentAngularVelocity, targetRotationSpeed, maxAngularVelocity * Time.deltaTime);
            // 回転を適用する
            rotAngle = currentAngularVelocity * Time.deltaTime;
            return Quaternion.RotateTowards(transform.rotation, playerRot, rotAngle);
        }
    }

    private bool StopRotateBody()
    {
        if (die) {return false;}
        bool focusFlag = !focusObject.FocusFlag && stateInput.IsMouseMiddle();

        bool zeldaFlag = selectTag == DataTag.Zelda && (currentState == ActionState.ReadySpinAttack|| !landing);

        bool ratchetAndClankFlag = selectTag == DataTag.RatchetAndClank && !CheckAvoidFlag();

        bool superMarioFlag = selectTag == DataTag.SuperMario && !landing;

        bool motionState = ForwardDirectionMotion();

        if (!input||focusFlag || zeldaFlag|| ratchetAndClankFlag|| superMarioFlag||motionState)
        {
            return true;
        }
        return false;
    }

    private bool ForwardDirectionMotion()
    {
        switch (currentState)
        {
            case ActionState.Attack:
            case ActionState.JumpAttack:
            case ActionState.Crouch:
            case ActionState.Grab:
            case ActionState.ClimbWall:
                return true;
        }
        return false;
    }

    public void ChangeMotionState(ActionState state)
    {
        //止める条件が成立したら早期リターン
        if (StopChangeMotion(state))
        {
            return;
        }
        //motionクラスからモーション名を取得する
        string animName = null;
        //エラーチェック
        if (motion != null)
        {
            animName = motion.SetMotion(state);
        }
        else
        {
            Debug.LogWarning("モーションクラスがアタッチされていない");
        }
        //現在の状態を過去に代入し
        pastState = currentState;
        //変更するモーションを現在の状態に代入し
        currentState = state;
        //モーションを再生
        anim.Play(animName);
    }
    private bool StopChangeMotion(ActionState state)
    {
        //モーション更新を絶対に優先する条件
        if (state == ActionState.Damage || state == ActionState.Die||state == ActionState.VictoryPose)
        {
            return false;
        }
        //モーション更新を止める条件↓
        //現在と同じ状態＆攻撃回数が0＆現在の状態が走りじゃなかったら
        bool stopBaseFlag = state == currentState && attackCount == 0 &&
                             currentState != ActionState.Jump&&!GetTimer().Timer_CruchAttack.IsEnabled();

        //指定したモーションなら＆現在のモーションがまだ終わっていなかったら
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool stopMotion = IsNoPlayMotion(animInfo) && animInfo.normalizedTime < endMotionNormalizedTime;

        if(stopBaseFlag ||stopMotion) 
        {
            return true; 
        }
        return false;
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

    protected virtual bool NoPlayMotionName(AnimatorStateInfo info)
    {
        bool flag = false;
        MotionNameCollection motionCollection = new MotionNameCollection();
        //指定されたモーション名ならflagをtrueにする
        //モーション名を別クラスから取得
        string[] noPlayMotions = motionCollection.GetNoPlayMotionName();
        foreach (string motion in noPlayMotions)
        {
            if (info.IsName(motion))
            {
                flag = true;
            }
        }
        return flag;
    }

    private bool NoDamageState()
    {
        MotionNameCollection motionCollection = new MotionNameCollection();
        //モーション名を別クラスから取得
        string[] noDamageMotions = motionCollection.GetNoDamageMotionsName();
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        foreach (string motion in noDamageMotions)
        {
            if (info.IsName(motion))
            {
                return true;
            }
        }
        return false;
    }
    private void Damage(Collider other)
    {
        //当たったもののダメージクラスを取得する
        BaseDamageObject damager = other.GetComponent<BaseDamageObject>();
        if (damager == null ||
            GetTimer().GetTimer_Invincible().IsEnabled() ||
            die)
        {
            return;
        }
        //攻撃中などの状態時はダメージを発生させない
        if (NoDamageState()) { return; }
        //ライフを減らす
        LifeReduce(damager.GetSetDamage);
        if (hp > 0)
        {
            //ダメージエフェクト発生
            if (generateEffects != null)
            {
                generateEffects.GenerateEffect((int)EffectState.Damage, other.transform.position);
            }
            //ノックバック処理
            Knockback(scriptableData.KnockBackPower);
            moveStop = true;
            //ダメージモーション発生
            ChangeMotionState(ActionState.Damage);
        }
        //ライフが0以下なら
        else if (hp <= 0)
        {
            Die();
        }
        //無敵タイマーを起動
        GetTimer().GetTimer_Invincible().StartTimer(scriptableData.InvincibleTimerCount);
        seController.DamageSEPlay();
        //プレイヤーの体を赤に変更(ダメージエフェクト)
        colorChange.StartColorTransition(Transitions.damage);
    }

    private void LifeReduce(float damage)
    {
        hp -= damage;
        if (damage > 0)
        {
            lifeGauge.SetLifeGauge2(damage);
        }
    }

    private float enemyStepJumpPower = 1000f;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "EnemyGuard":
                //敵の盾に防御された時
                Knockback(scriptableData.KnockBackPower);
                break;
            case "Enemy":
                //当たった対象がEnemyタグなら
                //(this)防御の条件に当てはまった時
                if (DefenseCommand())
                {
                    seController.DefenseSEPlay();
                    return;
                }
                //ダメージ処理
                Damage(other);
                break;
            case "EnemyBody":
                //エネミーをジャンプで踏みつけた時の処理
                if (selectTag != DataTag.SuperMario) { return; }
                var enemy = other.GetComponent<Slime>();
                if (enemy.GetFlatBodyFlag)
                {
                    JumpForce(enemyStepJumpPower);
                }
                break;
        }
    }

    private bool DefenseCommand()
    {
        if (stateInput.BlockState != ShieldBlockState.Null &&
            focusObject.FocusFlag)
        {
            return true;
        }
        return false;
    }

    protected override void Die()
    {
        base.Die();
        //銃を表示してたら非表示にする
        toolSetting.ActiveGun(false);
        //死亡エフェクトを発生
        if (generateEffects != null)
        {
            generateEffects.GenerateEffect((int)EffectState.Die, transform.position);
        }
        ChangeMotionState(ActionState.Die);
    }

    private float cliffJumpPower = 2000f;

    private void OnCollisionExit(Collision collision)
    {
        if(selectTag != DataTag.Zelda) { return; }
        //掴まっているか
        if (obstacleDetect.IsGrabFlag()) { return; }
        //登っているか
        if(currentState == ActionState.ClimbWall) { return; }
        if (GetTimer().Timer_StepJumpCoolDown.IsEnabled()) { return; }
        if (obstacleDetect.IsStepJumpFlag()) {  return; }
        if (obstacleDetect.CheckWallNoHit()) { return; }
        if(GetTimer().Timer_WallGlabStoper.IsEnabled()) { return; }
        if (obstacleDetect.CliffJumpFlag) { return; }
        if(collision.collider.tag == "WallFloor")
        {
            if (!obstacleDetect.LowStep())
            {
                JumpForce(cliffJumpPower);
                obstacleDetect.CliffJumpFlag = true;
                seController.StepJumpSEPlay();
            }
        }
    }
}
