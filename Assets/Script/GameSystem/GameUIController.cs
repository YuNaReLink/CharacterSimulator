using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameDataManager;

public class GameUIController : MonoBehaviour
{
    //�Q�[������UI�̃^�O
    public enum UITag
    {
        LifeFrame,
        Life,
        Weapon,
        Command,
        LockOn,
        Key,
    }

    [SerializeField]
    private List<GameObject> uiGameObjects = new List<GameObject>();

    /// <summary>
    /// �{�^����UI�֌W
    /// </summary>
    [SerializeField]
    [Tooltip("��ʃt�F�[�h�A�E�g�p�l��")]
    private CanvasGroup fadePanel;
    //�Q�[�����ʃL�����o�X�O���[�v
    [SerializeField]
    private CanvasGroup resultCanvasGroup;
    [SerializeField]
    private Text resultText;
    //���g���C�{�^��
    [SerializeField]
    private Button retryButton;
    //�I���{�^��
    [SerializeField]
    private Button endButton;
    //�Q�[���I�[�o�[�t���O
    private bool gameOverFlag = false;
    //�Q�[���N���A�t���O
    private bool gameClearFlag = false;
    //�Q�[�����I���������̃t���O
    private bool gameEndFlag = false;
    //�t�F�[�h�����鎞�Ԃ��w��
    [SerializeField]
    [Tooltip("�t�F�[�h�����鎞��(�b)")]
    private float fadeTime = 1f;
    //�o�ߎ��Ԃ��擾
    private float resultTimer;
    private float endTimer;
    //�Q�[���I�[�o�[UI�\���^�C�}�[
    private CountDown timer_GameEnd;
    //�Q�[���I�[�o�[���̉�ʂ��t�F�C�h�A�E�g������^�C��
    [SerializeField, Tooltip("�Q�[���I�[�o�[���̃t�F�[�h�A�E�g�^�C��")]
    private int gameOverCount = 117;
    //�Q�[���N���A���̉�ʂ��t�F�C�h�A�E�g������^�C��
    [SerializeField, Tooltip("�Q�[���N���A���̃t�F�[�h�A�E�g�^�C��")]
    private int gameClearCount = 50;

    /// <summary>
    /// �v���C���[�֌W
    /// </summary>
    [SerializeField]
    private PlayerController playerController;
    public PlayerController GetPlayerController() { return playerController; }

    /// <summary>
    /// ���ڂ�UI�֌W
    /// </summary>
    [SerializeField]
    private TargetLock_ON_UI lock_ON_UI;
    [SerializeField]
    private Image lockONUIObject;
    public Image LockONUIObject { get { return lockONUIObject; }set { lockONUIObject = value; } }
    
    /// <summary> 
    /// ����UI�֌W
    /// </summary>
    [SerializeField]
    private KeyCount keyCount;

    private int pastKeyCount = 0;
    private void Start()
    {
        InitFlag();
        resultCanvasGroup.alpha = 0;
        resultCanvasGroup.gameObject.SetActive(false);
        GetAllComponent();
        InitializeGameUI();
    }
    private void InitFlag()
    {
        gameOverFlag = false;
        gameClearFlag = false;
        gameEndFlag = false;
    }
    private void GetAllComponent()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        keyCount = GetComponentInChildren<KeyCount>();

        lock_ON_UI = GetComponent<TargetLock_ON_UI>();


        timer_GameEnd = new CountDown();
        if (timer_GameEnd == null)
        {
            Debug.Log("timer_GameOver���R���|�[�l���g����Ă��܂���");
        }
    }
    private void Update()
    {
        if (!gameEndFlag)
        {
            SetGameState();
        }
        TimerUpdate();
        if (CurrentGameState == GameState.GameOver&&gameOverFlag)
        {
            SetGameEndFadeState();
        }
        else if(CurrentGameState == GameState.GameClaer && gameClearFlag)
        {
            SetGameEndFadeState();
        }
        if(CurrentGameState == GameState.GameEnd)
        {
            FadeOutScene();
        }
    }
    private void TimerUpdate()
    {
        if (timer_GameEnd.IsEnabled())
        {
            timer_GameEnd.Update();
        }
        else
        {
            SetEndGameState();
            timer_GameEnd.End();
        }
    }

    private void SetEndGameState()
    {
        if (CurrentGameState == GameState.GameOver && gameEndFlag)
        {
            gameOverFlag = true;
        }
        else if (CurrentGameState == GameState.GameClaer && gameEndFlag)
        {
            gameClearFlag = true;
        }
    }
    private void SetGameEndFadeState()
    {
        if(!resultCanvasGroup.gameObject.activeSelf){return;}
        //�o�ߎ��Ԃ����Z
        resultTimer += Time.deltaTime;
        //�o�ߎ��Ԃ�fadeTime�Ŋ������l��alpha�ɓ����
        resultCanvasGroup.alpha = resultTimer / fadeTime;
        if(resultCanvasGroup.alpha >= 1)
        {
            CurrentGameState = GameState.GameEnd;
        }
    }
    private void FadeOutScene()
    {
        //�o�ߎ��Ԃ����Z
        endTimer += Time.deltaTime;
        //�o�ߎ��Ԃ�fadeTime�Ŋ������l��alpha�ɓ����
        fadePanel.alpha = endTimer / fadeTime;
        if(fadePanel.alpha >= 1)
        {
            SetResultButton();
        }
    }
    private void SetGameState()
    {
        switch (CurrentGameState)
        {
            case GameState.NowGame:
                lock_ON_UI.ActiveLock_ON_UI(this);
                SetKeyCount();
                break;
            case GameState.GameClaer:
                SetResult("Game Clear", Color.white, 190);
                timer_GameEnd.StartTimer(gameClearCount);
                gameEndFlag = true;
                GameEndUI();
                break;
            case GameState.GameOver:
                SetResult("Game Over", Color.red, 200);
                timer_GameEnd.StartTimer(gameOverCount);
                gameEndFlag = true;
                GameEndUI();
                break;
        }
    }

    private void SetKeyCount()
    {
        KeyStates currentKey = GameDataManager.GetKeyStates();
        if(currentKey.Count != pastKeyCount)
        {
            string countName = "�~";
            string count = currentKey.Count.ToString();
            keyCount.GetKeyCountText().text = countName + count;
            pastKeyCount = currentKey.Count;
        }
    }

    private void SetResult(string result,Color color,int fontsize)
    {
        resultCanvasGroup.gameObject.SetActive(true);
        resultText.text = result;
        resultText.color = color;
        resultText.fontSize = fontsize;
        resultText.gameObject.SetActive(true);
    }
    private void SetResultButton()
    {
        retryButton.gameObject.SetActive(true);
        endButton.gameObject.SetActive(true);
        CursorController.SetCursorState(true);
        CursorController.SetCursorLookMode(CursorLockMode.None);
    }

    public void OnSceneChange(string scenename)
    {
        MoveScene.OnSceneChange(scenename);
    }

    private void InitializeGameUI()
    {
        //�Q�[�����ɕ\������UI��S�ĕ\��
        for (int i = 0; i < uiGameObjects.Count; i++)
        {
            if (uiGameObjects[i].gameObject.activeSelf)
            {
                continue;
            }
            uiGameObjects[i].gameObject.SetActive(true);
        }
        //���[�h�ɂ���ē����UI���\����
        switch (CurrentGameMode)
        {
            case GameMode.RatchetAndClank:
            case GameMode.SuperMario:
                uiGameObjects[(int)UITag.Key].gameObject.SetActive(false);
                break;
        }

    }

    private void GameEndUI()
    {
        for(int i = 0; i<uiGameObjects.Count; i++)
        {
            if (!uiGameObjects[i].gameObject.activeSelf)
            {
                continue;
            }
            uiGameObjects[i].gameObject.SetActive(false);
        }
    }
}
