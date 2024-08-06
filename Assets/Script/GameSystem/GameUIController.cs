using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameDataManager;

public class GameUIController : MonoBehaviour
{
    //ゲーム中のUIのタグ
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
    /// ボタンのUI関係
    /// </summary>
    [SerializeField]
    [Tooltip("画面フェードアウトパネル")]
    private CanvasGroup fadePanel;
    //ゲーム結果キャンバスグループ
    [SerializeField]
    private CanvasGroup resultCanvasGroup;
    [SerializeField]
    private Text resultText;
    //リトライボタン
    [SerializeField]
    private Button retryButton;
    //終了ボタン
    [SerializeField]
    private Button endButton;
    //ゲームオーバーフラグ
    private bool gameOverFlag = false;
    //ゲームクリアフラグ
    private bool gameClearFlag = false;
    //ゲームが終了した時のフラグ
    private bool gameEndFlag = false;
    //フェードさせる時間を指定
    [SerializeField]
    [Tooltip("フェードさせる時間(秒)")]
    private float fadeTime = 1f;
    //経過時間を取得
    private float resultTimer;
    private float endTimer;
    //ゲームオーバーUI表示タイマー
    private CountDown timer_GameEnd;
    //ゲームオーバー時の画面をフェイドアウトさせるタイム
    [SerializeField, Tooltip("ゲームオーバー時のフェードアウトタイム")]
    private int gameOverCount = 117;
    //ゲームクリア時の画面をフェイドアウトさせるタイム
    [SerializeField, Tooltip("ゲームクリア時のフェードアウトタイム")]
    private int gameClearCount = 50;

    /// <summary>
    /// プレイヤー関係
    /// </summary>
    [SerializeField]
    private PlayerController playerController;
    public PlayerController GetPlayerController() { return playerController; }

    /// <summary>
    /// 注目のUI関係
    /// </summary>
    [SerializeField]
    private TargetLock_ON_UI lock_ON_UI;
    [SerializeField]
    private Image lockONUIObject;
    public Image LockONUIObject { get { return lockONUIObject; }set { lockONUIObject = value; } }
    
    /// <summary> 
    /// 鍵のUI関係
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
            Debug.Log("timer_GameOverがコンポーネントされていません");
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
        //経過時間を加算
        resultTimer += Time.deltaTime;
        //経過時間をfadeTimeで割った値をalphaに入れる
        resultCanvasGroup.alpha = resultTimer / fadeTime;
        if(resultCanvasGroup.alpha >= 1)
        {
            CurrentGameState = GameState.GameEnd;
        }
    }
    private void FadeOutScene()
    {
        //経過時間を加算
        endTimer += Time.deltaTime;
        //経過時間をfadeTimeで割った値をalphaに入れる
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
            string countName = "×";
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
        //ゲーム中に表示するUIを全て表示
        for (int i = 0; i < uiGameObjects.Count; i++)
        {
            if (uiGameObjects[i].gameObject.activeSelf)
            {
                continue;
            }
            uiGameObjects[i].gameObject.SetActive(true);
        }
        //モードによって特定のUIを非表示に
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
