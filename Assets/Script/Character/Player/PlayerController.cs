using UnityEngine;
using static ColorChange;
using static GameDataManager;
using static CharacterManager;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerController : CharacterController
{
    ///<summary>
    ///Player�I�u�W�F�N�g��Unity��ŃR���|�[�l���g����N���X�̃C���X�^���X
    ///</summary>
    [Header("�v���C���[�̓��͊Ǘ��N���X")]
    [SerializeField]
    private PlayerInput             stateInput;
    public PlayerInput              GetStateInput() { return stateInput; }
    [Header("�v���C���[�̈ړ��Ǘ��N���X")]
    [SerializeField]
    private PlayerMovement          stateMovement;
    public PlayerMovement           GetStateMovement() {  return stateMovement; }
    [Header("�n�`�Ƃ̓����蔻��Ǘ��N���X")]
    [SerializeField]
    private ObstacleDetect          obstacleDetect;
    public ObstacleDetect           GetWallClimb() {  return obstacleDetect; }
    [Header("SE�Ǘ��N���X")]
    [SerializeField]
    private PlayerSEController      seController;
    public PlayerSEController       GetSEController() { return seController; }
    /// <summary>
    /// �v���C���[�̍s���̃N���X
    /// motion:���[�V�����̊Ǘ�
    /// jump  :�W�����v�̊Ǘ�
    /// avoid :���(���[�����O)�̊Ǘ�
    /// gun   :�e�̊Ǘ�
    /// attack:�U���̊Ǘ�
    /// guard :�h��̊Ǘ�
    /// </summary>
    private PlayerMotionController      motion = null;
    private InterfaceJumpCommand        jump = null;
    private InterfaceAvoidanceCommand   avoid = null;
    private GunCommand                  gun = null;
    private InterfaceAttackCommand      attack = null;
    private GuardCommand                guard = null;
    /// <summary>
    /// �v���C���[���g���e�̒e�𔭎˂��邽�߂̃N���X
    /// </summary>
    [Header("�e�̒e�̔��˂��Ǘ�����N���X")]
    [SerializeField]
    private BulletShot              bulletShot;
    public BulletShot               GetBulletShot() { return bulletShot; }
    /// <summary>
    /// Main�J�����Ɋ֘A�̕ϐ��A�C���X�^���X�Ȃ�
    /// </summary>
    [Header("�J�����֌W�̃R���|�[�l���g")]
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
    /// �J�����̉�]�֌W
    /// </summary>
    //�v���C���[�̌��݂̈ʒu
    private Vector3                 currentPos;
    public Vector3                  GetCurrentPos() { return currentPos; }
    //�v���C���[�̉ߋ��̈ʒu
    private Vector3                 pastPos;
    public Vector3                  GetPastPos() { return pastPos; }
    //�v���C���[�̈ړ���
    private Vector3                 cameravelocity;
    //�v���C���[�̐i�s�����Ɍ����N�H�[�^�j�I��
    private Quaternion              playerRot;
    //���݂̉�]�e���x
    private float                   currentAngularVelocity;
    //�ő�̉�]�p���x[deg/s]
    [SerializeField]
    private float                   maxAngularVelocity = Mathf.Infinity;
    //�i�s�����ɂ����邨���悻�̎���[s]
    [SerializeField]
    private float                   smoothTime = 0.1f;
    //���݂̌����Ɛi�s�����̊p�x
    private float                   diffAngle;
    //���݂̉�]����p�x
    private float                   rotAngle;

    protected bool[]                avoidFlag = new bool[(int)AvoidState.DataEnd];
    public bool[]                   AvoidFlag { get { return avoidFlag; } set { avoidFlag = value; } }
    [SerializeField]
    protected AttackStateCount      attackState = AttackStateCount.Null;
    public AttackStateCount         AttackState { get { return attackState; } set { attackState = value; } }

    /// <summary>
    /// �W�����v�֌W
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
    //�L�[�̒������ŃW�����v�͂��ς�鎞��
    //�W�����v�����p���[��ێ�����ϐ�
    [SerializeField]
    private float                   jumpingPower = 0;
    public float                    JumpingPower { get { return jumpingPower; } set { jumpingPower = value; } }

    /// <summary>
    /// �q�b�v�h���b�v�֌W
    /// </summary>
    private bool                    rotate = false;
    public bool                     RotateFlag { get { return rotate; } set { rotate = value; } }
    private float                   rotateQuantity = 0;
    private float                   rotateY = 0;
    public float                    RotateY { get { return rotateY; } set { rotateY = value; } }

    /// <summary>
    /// UI�֌W
    /// </summary>
    //LifeGauge�X�N���v�g
    [SerializeField]
    protected LifeGauge             lifeGauge;
    [SerializeField]
    protected LifeGauge             lifeFrameGauge;

    private float                   maxRotate = 360.0f;
    private float                   rotatePower = 45.0f;


    /// <summary>
    /// �e�ϐ��̃f�[�^
    /// </summary>
    private float jumpingHorizontalRaito = 0.1f;

    private float fallPower = 500f;
    protected override void Awake()
    {
        //Start�O�Ƀ^�O����
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

        //=====UI�̏�����=====
        lifeGauge?.SetLifeGauge(hp);
        lifeFrameGauge?.SetLifeGauge(hp);
    }

    public override void AllGetComponent()
    {
        base.AllGetComponent();
        check =                 GetComponent<GroundCheck>();
        if(check == null) 
        {
            Debug.LogError("check���A�^�b�`����Ă��܂���");
        }
        //���L�̓v���C���[����̃R���|�[�l���g
        stateInput =            GetComponent<PlayerInput>();
        if (stateInput == null)
        {
            Debug.LogError("stateInput���A�^�b�`����Ă��܂���");
        }
        else
        {
            stateInput?.SetPlayerController(this);
        }
        obstacleDetect =        GetComponent<ObstacleDetect>();
        if(obstacleDetect == null)
        {
            Debug.LogError("obstacleDetect���A�^�b�`����Ă��܂���");
        }
        seController =          GetComponent<PlayerSEController>();
        if(seController == null)
        {
            Debug.LogError("seController���A�^�b�`����Ă��܂���");
        }
        toolSetting =          GetComponent<PlayerPropsSetting>();
        if(toolSetting == null)
        {
            Debug.LogError("toolSetting���A�^�b�`����Ă��܂���");
        }
        //=====Command�N���X����
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
            Debug.LogError("Camera���擾�o���Ȃ�����");
        }
        else
        {
            tpsCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TPSCamera>();
        }

        //=====UI�̃A�^�b�`=====
        g = GameObject.FindGameObjectWithTag("Life");
        if (g == null && CurrentGameState == GameState.NowGame)
        {
            Debug.LogError("Life�̃^�O�t���̃I�u�W�F�N�g��������܂���ł���");
        }
        else
        {
            lifeGauge =         g.GetComponent<LifeGauge>();
        }
        g = GameObject.FindGameObjectWithTag("LifeFrame");
        if (g == null && CurrentGameState == GameState.NowGame)
        {
            Debug.LogError("LifeFrame�̃^�O�t���̃I�u�W�F�N�g��������܂���ł���");
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
        //�Q�[�����̎��Ԃ��~�܂��Ă���Ȃ瑁�����^�[��
        if (Mathf.Approximately(Time.timeScale, 0f)) { return; }
        base.Update();
        
        //����̃A�j���[�V�����ŏ����ɍ�������
        motion.MotionPause();

        //���͂��I��������̃��[�V�����؂�ւ�
        motion.InputEndMotion(currentState);

        if (die) { return; }
        //�X�[�p�[�}���I���[�h���̃q�b�v�h���b�v�̉�]����
        HipDropRotateBody();
        toolSetting.PropsUpdateSetting(this);
        toolSetting.SetColliderEnabled(this);

        //���͂��s��
        if (currentState != ActionState.Damage && !moveStop)
        {
            KeyInput();
            Command();
        }
    }

    private void HipDropRotateBody()
    {
        //��]�t���O��true������łȂ����
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
    //�A�^�b�`���Ă���Damage�N���X��WallHit��false��
    public void EmptyDamageWallHitFlag()
    {
        for (int i = 0; i < damageObjects.Count; i++)
        {
            damageObjects[i].SetWallHit(false);
        }
    }

    private void KeyInput()
    {
        //���͂�����
        input = false;
        //���n����
        Landed();
        obstacleDetect.WallActionInput(this);
        InitializeJumpAttackInput();
        stateInput.AllKeyInput();
        //���͂̏�Ԃ��t���O�ɑ��
        input = SetInput(currentState);
    }


    private void Landed()
    {
        //���n����
        landing = check.CheckGroundStatus();
        //�p�V�t�B�b�N�}�e���A���ύX
        SetPhysicMaterial();
        if (!landing){return;}
        //�R�W�����v�t���O���\��
        obstacleDetect.CliffJumpFlag = false;
        //�W�����v�֌W
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
        //�q�b�v�h���b�v�֌W
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
        //�d�͊֌W
        GetTimer().Timer_NoGravity.End();
    }

    private void JudgeNextJump()
    {
        if (jumpState == JumpState.Null){return;}
        //1,2��ڂ̒��n���̏���
        if (!burstJump && jumpState != JumpState.ThirdJump)
        {
            GetTimer().GetTimer_BurstJump().StartTimer(scriptableData.MaxBurstJumpCount);
            burstJump = true;
        }
        //3��ڂ̒��n���̏���
        else if (burstJump || jumpCount == scriptableData.MaxJumpCount)
        {
            jumpState = JumpState.Null;
            jumpCount = 0;
            burstJump = false;
        }
        //����ȊO�ŃW�����v��Ԃ�Null����Ȃ�������
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
        //���S����
        Death();
        if (GameClearCommand()) { return; }
        //----�v���C���[�̈ړ�----
        //�J�����ɑ΂��đO���擾
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        //�J�����ɑ΂��ĉE���擾
        Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;
        //�t���O�ɂ���ē��͂��ɘa������
        if (!landing)
        {
            stateInput.Horizontalinput *= jumpingHorizontalRaito;
        }
        //��Ԃɂ���ăX�s�[�h��ύX
        float maxspeed = SetMoveSpeed(scriptableData.MaxSpeed);
        float accele = SetMoveSpeed(scriptableData.Acceleration);
        //���͒��Ȃ�
        //�ړ��ʂ𑫂�
        if (input)
        {
            Accele(cameraForward, cameraRight, maxspeed, accele);
        }
        else if(!input && landing)
        {
            //�ҋ@��Ԃ����͂��ĂȂ��Ȃ�
            StopCommand(cameraForward, cameraRight, scriptableData.MinSpeed, scriptableData.Deceleration);
        }
        if(obstacleDetect != null)
        {
            obstacleDetect.Execute(this);
        }
        //�W�����v����
        if (jump != null)
        {
            jump.Execute(maxspeed);
        }
        //���n���Ă��ă��[�����O��ԂȂ�
        if (avoid != null)
        {
            avoid.Execute();
        }
        //�e��������
        if (gun != null)
        {
            gun.Execute();
        }
        //�}�E�X���쎞�̏���
        if (!StopAttackMotion())
        {
            //�U������
            if (attack != null)
            {
                attack.Execute();
            }
            if (stateInput.GetHipDropFlag && !GetTimer().Timer_NoGravity.IsEnabled())
            {
                FastFall(-fallPower);
            }
            //�}�E�X�E�N���b�N��������
            if (guard != null)
            {
                guard.Execute();
            }
            //�����SE����
            if (stateInput.IsLeftCtrlKey() && currentState == ActionState.ModeChange)
            {
                seController.WeaponSetSound(currentBattle);
            }
        }
        //�ړ��ʂ�K�p����
        Move();
        //----�v���C���[�̉�]----
        if (focusObject.FocusFlag && stateInput.IsMouseMiddle() && !die)
        {
            Vector3 targetPos = focusObject.GetFocusObjectPosition();
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
        }
        if (StopRotateBody()) { return; }
        //���݂̈ʒu
        currentPos = transform.position;
        //�ړ��ʌv�Z
        cameravelocity = currentPos - pastPos;
        //y����0
        cameravelocity.y = 0;
        //�ߋ��̈ʒu���X�V
        pastPos = currentPos;
        if (cameravelocity == Vector3.zero&& !focusObject.FocusFlag) { return; }
        //�v���C���[�̉�]��K�p
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
            //����������
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
        //���g�������Ă�ꍇ�̒��ڏ���
        if (focusObject.FocusFlag && stateInput.IsMouseMiddle() && !die)
        {
            // �G�̕����x�N�g�����擾
            Vector3 targetObject = focusObject.GetFocusObjectPosition();
            targetObject.y = transform.position.y;
            Vector3 enemyDirection = (targetObject - transform.position).normalized;
            //enemyDirection.y = 0;
            // �v���C���[����ɓG�̕���������
            Quaternion enemyRotation = Quaternion.LookRotation(enemyDirection, Vector3.up);
            return enemyRotation;
        }
        //��������Ȃ���Βʏ�̎O�l�̃J��������
        else
        {
            playerRot = Quaternion.LookRotation(cameravelocity, Vector3.up);
            diffAngle = Vector3.Angle(transform.forward, cameravelocity);
            //��]���x���v�Z����
            float targetRotationSpeed = Mathf.Min(diffAngle / smoothTime, maxAngularVelocity);
            // ��]���x�𒲐�����
            currentAngularVelocity = Mathf.MoveTowards(currentAngularVelocity, targetRotationSpeed, maxAngularVelocity * Time.deltaTime);
            // ��]��K�p����
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
        //�~�߂���������������瑁�����^�[��
        if (StopChangeMotion(state))
        {
            return;
        }
        //motion�N���X���烂�[�V���������擾����
        string animName = null;
        //�G���[�`�F�b�N
        if (motion != null)
        {
            animName = motion.SetMotion(state);
        }
        else
        {
            Debug.LogWarning("���[�V�����N���X���A�^�b�`����Ă��Ȃ�");
        }
        //���݂̏�Ԃ��ߋ��ɑ����
        pastState = currentState;
        //�ύX���郂�[�V���������݂̏�Ԃɑ����
        currentState = state;
        //���[�V�������Đ�
        anim.Play(animName);
    }
    private bool StopChangeMotion(ActionState state)
    {
        //���[�V�����X�V���΂ɗD�悷�����
        if (state == ActionState.Damage || state == ActionState.Die||state == ActionState.VictoryPose)
        {
            return false;
        }
        //���[�V�����X�V���~�߂������
        //���݂Ɠ�����ԁ��U���񐔂�0�����݂̏�Ԃ����肶��Ȃ�������
        bool stopBaseFlag = state == currentState && attackCount == 0 &&
                             currentState != ActionState.Jump&&!GetTimer().Timer_CruchAttack.IsEnabled();

        //�w�肵�����[�V�����Ȃ灕���݂̃��[�V�������܂��I����Ă��Ȃ�������
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
        //�w�肳�ꂽ���[�V�������Ȃ�flag��true�ɂ���
        //���[�V��������ʃN���X����擾
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
        //���[�V��������ʃN���X����擾
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
        //�����������̂̃_���[�W�N���X���擾����
        BaseDamageObject damager = other.GetComponent<BaseDamageObject>();
        if (damager == null ||
            GetTimer().GetTimer_Invincible().IsEnabled() ||
            die)
        {
            return;
        }
        //�U�����Ȃǂ̏�Ԏ��̓_���[�W�𔭐������Ȃ�
        if (NoDamageState()) { return; }
        //���C�t�����炷
        LifeReduce(damager.GetSetDamage);
        if (hp > 0)
        {
            //�_���[�W�G�t�F�N�g����
            if (generateEffects != null)
            {
                generateEffects.GenerateEffect((int)EffectState.Damage, other.transform.position);
            }
            //�m�b�N�o�b�N����
            Knockback(scriptableData.KnockBackPower);
            moveStop = true;
            //�_���[�W���[�V��������
            ChangeMotionState(ActionState.Damage);
        }
        //���C�t��0�ȉ��Ȃ�
        else if (hp <= 0)
        {
            Die();
        }
        //���G�^�C�}�[���N��
        GetTimer().GetTimer_Invincible().StartTimer(scriptableData.InvincibleTimerCount);
        seController.DamageSEPlay();
        //�v���C���[�̑̂�ԂɕύX(�_���[�W�G�t�F�N�g)
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
                //�G�̏��ɖh�䂳�ꂽ��
                Knockback(scriptableData.KnockBackPower);
                break;
            case "Enemy":
                //���������Ώۂ�Enemy�^�O�Ȃ�
                //(this)�h��̏����ɓ��Ă͂܂�����
                if (DefenseCommand())
                {
                    seController.DefenseSEPlay();
                    return;
                }
                //�_���[�W����
                Damage(other);
                break;
            case "EnemyBody":
                //�G�l�~�[���W�����v�œ��݂������̏���
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
        //�e��\�����Ă����\���ɂ���
        toolSetting.ActiveGun(false);
        //���S�G�t�F�N�g�𔭐�
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
        //�͂܂��Ă��邩
        if (obstacleDetect.IsGrabFlag()) { return; }
        //�o���Ă��邩
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
