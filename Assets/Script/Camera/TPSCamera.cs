using UnityEngine;
public class TPSCamera : MonoBehaviour
{
    private GameObject          player;
    //���ڂ���Ώۂ����Ȃ����̒��ڏꏊ
    [SerializeField]
    private GameObject          focusPos;

    private float               rotation_hor;
    private float               rotation_ver;
    [SerializeField]
    private float               distance_base = 5.0f;
    [SerializeField]
    private float               mouseXSpeed = 3.0f;
    [SerializeField]
    private float               mouseYSpeed = 1.5f;

    private Vector3             playertrack;

    private bool                focusMode = false;

    public bool                 FocusModeFlag { get { return focusMode; } set { focusMode = value; } }

    private bool                reset = false;

    public bool                 ResetFlag { get { return reset; } set { reset = value; } }

    private PlayerController    playerController;
    private FocusObject         focusObject;

    [SerializeField]
    private float               desiredDistanceBehindPlayer = 3.0f;
    [SerializeField]
    private float               focusCameraPosX = 1.5f;
    [SerializeField]
    private float               focusCameraPosY = 2.0f;

    [SerializeField]
    private Vector3             initCameraRotation = new Vector3(0, 0.2f, -5);


    [SerializeField]
    private float               baseDistance = 2; // �J�����ƃv���C���[�̋���
    [SerializeField]
    private float               height = 1.0f; // �J�����̍���
    [SerializeField]
    private float               damping = 10.0f; // �J�����̓����̊��炩��

    private bool                cameraChange = false;

    private TimeCountDown       timer_CameraStop;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        if(player == null)
        {
            Debug.LogError("player�I�u�W�F�N�g���A�^�b�`����Ă��܂���");
        }

        rotation_hor = 0f;
        rotation_ver = 0f;
        playertrack = Vector3.zero;


        //cursor lock : Esc to exit
        CursorController.SetCursorState(false);
        CursorController.SetCursorLookMode(CursorLockMode.Locked);

        focusMode = false;
        playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("playerController���A�^�b�`����Ă��܂���(�J����)");
        }
        focusObject = player.GetComponentInChildren<FocusObject>();
        if (focusObject == null)
        {
            Debug.LogError("focusObject���A�^�b�`����Ă��܂���(�J����)");
        }
        timer_CameraStop = new TimeCountDown();
        if(timer_CameraStop == null)
        {
            Debug.LogError("timer_CameraStop����������܂���ł���(�J����)");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�^�C�}�[����
        if (timer_CameraStop.IsEnabled())
        {
            timer_CameraStop.Update();
        }
        else
        {
            timer_CameraStop.End();
        }

        if (playerController.InputFlag)
        {
            timer_CameraStop.StartTimer(1f);
        }


        //�J�����ƃv���C���[�Ƃ̋�����1.0f�ɐݒ�
        distance_base -= Input.mouseScrollDelta.y * 0.5f;
        if (distance_base < 1.0f)
        {
            distance_base = 1.0f;
        }
        else if(distance_base > 5.0f)
        {
            distance_base = 5.0f;
        }

        if (playerController.GetStateInput().IsMouseMiddle())
        {
            focusMode = true;
        }
        else
        {
            focusMode = false;
        }
        
    }

    void FixedUpdate()
    {
        MouseInputCamera();
    }

    private void ReSetFoucusCamera()
    {
        reset = true;
        Vector3 directionToEnemy = focusPos.transform.position - transform.position;
        // �v���C���[�̃��[�J�����W�n�ł̃J�����̃I�t�Z�b�g
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        Vector3 offset = new Vector3(0, focusCameraPosY, -desiredDistanceBehindPlayer);
        // �v���C���[�̉�]�ɍ��킹�ă��[�J�����W�n�̃I�t�Z�b�g��ϊ�
        Vector3 rotatedOffset = player.transform.rotation * offset;
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
        // ���[���h���W�n�ɕϊ����ăJ�����̈ʒu�ɓK�p
        transform.position = Vector3.Lerp(transform.position, player.transform.position + rotatedOffset, Time.deltaTime * 10.0f);
    }

    private void MouseInputCamera()
    {
        if (player == null)
        {
            return;
        }
        if (focusMode && focusObject.FocusFlag)
        {
            reset = false;
            Vector3 directionToEnemy = focusObject.GetFocusObjectPosition() - transform.position;
            // �v���C���[�̃��[�J�����W�n�ł̃J�����̃I�t�Z�b�g
            Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
            Vector3 offset = new Vector3(focusCameraPosX, focusCameraPosY, -desiredDistanceBehindPlayer);
            // �v���C���[�̉�]�ɍ��킹�ă��[�J�����W�n�̃I�t�Z�b�g��ϊ�
            Vector3 rotatedOffset = player.transform.rotation * offset;
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
            // ���[���h���W�n�ɕϊ����ăJ�����̈ʒu�ɓK�p
            transform.position = Vector3.Lerp(transform.position, player.transform.position + rotatedOffset, Time.deltaTime * 2.0f);
            return;
        }
        else if (focusMode && !focusObject.FocusFlag)
        {
            ReSetFoucusCamera();
            return;
        }
        reset = false;
        //�J��������]����X�s�[�h��ݒ�
        rotation_hor += Input.GetAxis("Mouse X") * mouseXSpeed;
        rotation_ver -= Input.GetAxis("Mouse Y") * mouseYSpeed;

        //restrict vertical angle to -90 ~ +90
        if (Mathf.Abs(rotation_ver) > 90)
            rotation_ver = Mathf.Sign(rotation_ver) * 90;

        //base vector to rotate
        var rotation = Vector3.Normalize(initCameraRotation); //base(normalized)
        rotation = Quaternion.Euler(rotation_ver, rotation_hor, 0) * rotation; //rotate vector

        //�J�����̖��܂��h�����߂Ƀ��C���[���w�肷��
        RaycastHit hit;
        int layermask = 1 << 3; //1�̃r�b�g��3���C���[��(Floor_obstacle������ꏊ)�������V�t�g
        float distance = distance_base; //copy default(mouseScroll zoom)
        //�X�t�B�A���C�L���X�g�Ŗ��܂�h�~
        if (Physics.SphereCast(playertrack + Vector3.up * 1.7f, 0.5f,
        rotation, out hit, distance, layermask))
        {
            distance = hit.distance; //overwrite copy
        }

        //turn self
        transform.rotation = Quaternion.Euler(rotation_ver, rotation_hor, 0); //Quaternion IN!!

        //turn around + zoom
        transform.position = rotation * distance;

        //rotation center to neck-level
        var necklevel = Vector3.up * 1.7f;
        transform.position += necklevel;

        //track
        playertrack = Vector3.Lerp(
            playertrack, player.transform.position, Time.deltaTime * 10);
        transform.position += playertrack;
    }

    private void MarioCamera()
    {
        if (!player)
        {
            return;
        }
        if (cameraChange)
        {
            baseDistance = 2;
        }
        else
        {
            baseDistance = 5;
        }

        Vector3 playerDis = player.transform.position;
        Vector3 cameraDis = transform.position;
        float currentdistance = Vector3.Distance(cameraDis,playerDis);
        if(currentdistance < baseDistance&& playerController.InputFlag&&
            playerController.GetStateInput().VerticalInput < 0&&
            playerController.Landing)
        {
            return;
        }
        Vector3 _player = player.transform.position;
        _player.y += 1.5f;
        if (CameraMoveEnabled(currentdistance))
        {
            // �J�����̖ڕW�ʒu���v�Z
            Vector3 targetPosition = _player - (player.transform.forward * baseDistance) + (Vector3.up * height);

            // ���炩�ɃJ�������ړ�������
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * damping);
        }

        // �J�����̌������v�Z
        Quaternion targetRotation = Quaternion.LookRotation(_player - transform.position);

        // ���炩�ɃJ�����̌�����ύX����
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
    }

    private bool CameraMoveEnabled(float _currentdistance)
    {
        bool enabled = false;
        if (cameraChange)
        {
            if (!playerController.InputFlag)
            {
                enabled = true;
            }
            else if (playerController.GetStateInput().Horizontalinput != 0 &&
                playerController.GetStateInput().VerticalInput == 0 &&
                _currentdistance > baseDistance)
            {
                enabled = true;
            }
            else if (playerController.GetStateInput().Horizontalinput != 0 ||
                playerController.GetStateInput().VerticalInput != 0)
            {
                enabled = true;
            }
        }
        else
        {
            if (!timer_CameraStop.IsEnabled()&&!playerController.InputFlag)
            {
                enabled = true;
            }
            if (playerController.GetStateInput().Horizontalinput != 0 &&
                playerController.GetStateInput().VerticalInput != 0)
            {
                enabled = true;
            }
        }
        return enabled;
    }
}
