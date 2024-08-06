using System.Collections.Generic;
using UnityEngine;
using static GameDataManager;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private PlayerController        playerController;

    [SerializeField]
    private List<GameObject>        stages;
    [SerializeField]
    private GameObject              appearEnemyObject;

    [SerializeField]
    private int                     killCount = 0;

    public int                      KillCount { get { return killCount; } set { killCount = value; } }

    [SerializeField]
    private SoundData            soundManager;
    [SerializeField]
    private SoundController         soundController;

    private void Awake()
    {
        Application.targetFrameRate = 120;
        ChangeGameState(GameState.NowGame);
        UpdateGameState();
        Time.timeScale = 1f;
    }
    // Start is called before the first frame update
    private void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundData>();
        soundController = soundManager.gameObject.GetComponent<SoundController>();

        InitGameData();
        //プレイヤーコントローラー(Script)をタグで認識して取得する
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.Log("playerControllerがコンポーネントされていない(GameController)");
        }
        InitializeStages();
        ActiveStage(CurrentGameMode);

        soundController.StopBGM();
        soundController.StopSE();
        StartBGM(CurrentGameMode);
    }

    private void InitializeStages()
    {
        for (int i = 0; i < stages.Count; i++)
        {
            stages[i].SetActive(false);
        }
    }

    private void ActiveStage(GameMode gameMode)
    {
        if(gameMode == GameMode.Null) { return; }
        int value = (int)gameMode;
        stages[value].SetActive(true);
    }

    private void StartBGM(GameMode gameMode)
    {
        soundController.BGMVolume = soundController.GetBGMVolumData();
        switch(gameMode)
        {
            case GameMode.ZeldaMode:
                soundController.PlayBgm(soundManager.GetBGMClips()[(int)SoundData.BGMTag.GameBGM01]);
                break;
            case GameMode.RatchetAndClank:
                soundController.PlayBgm(soundManager.GetBGMClips()[(int)SoundData.BGMTag.GameBGM02]);
                break;
            case GameMode.SuperMario:
                soundController.PlayBgm(soundManager.GetBGMClips()[(int)SoundData.BGMTag.GameBGM03]);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        soundController.ChangeBGMUpdtate();
        //ゲーム時間が止まってるかどうかでゲームの状態を設定
        NowGameState();
        ResultState();
        if(CurrentGameMode == GameMode.ZeldaMode)
        {
            ChangeBGMByPlayerState();
        }
    }

    private void ChangeBGMByPlayerState()
    {
        if(CurrentGameState == GameState.GameEnd ||
            CurrentGameState == GameState.GameOver ||
            CurrentGameState == GameState.GameClaer) { return; }
        if (playerController.GetFocusObject().FocusFlag)
        {
            soundController.PlayBgm(soundManager.GetBGMClips()[(int)SoundData.BGMTag.Battle]);
        }
        else
        {
            soundController.PlayBgm(soundManager.GetBGMClips()[(int)SoundData.BGMTag.GameBGM01]);
        }
    }

    private void NowGameState()
    {
        if (CurrentGameState == GameState.GameEnd||
            CurrentGameState == GameState.GameOver||
            CurrentGameState == GameState.GameClaer) { return; }
        if (Time.timeScale < 1f)
        {
            CurrentGameState = GameState.Pause;
        }
        else/* if (Time.timeScale >= 1f)*/
        {
            CurrentGameState = GameState.NowGame;
        }
    }

    private void ResultState()
    {
        switch (CurrentGameMode)
        {
            case GameMode.ZeldaMode:
            case GameMode.RatchetAndClank:
            case GameMode.SuperMario:
                GoalClearState();
                break;
        }
        
    }

    private void GoalClearState()
    {
        if (CurrentGameState != GameState.NowGame) { return; }
        if (playerController.IsDied())
        {
            CurrentGameState = GameState.GameOver;
            soundController.StopBGM();
            switch (CurrentGameMode)
            {
                case GameMode.ZeldaMode:
                    soundController.PlaySe(soundManager.GetSEClips()[(int)SoundData.SETag.GameOver01]);
                    break;
                case GameMode.RatchetAndClank:
                    soundController.PlaySe(soundManager.GetSEClips()[(int)SoundData.SETag.GameOver02]);
                    break;
                case GameMode.SuperMario:
                    soundController.PlaySe(soundManager.GetSEClips()[(int)SoundData.SETag.GameOver03]);
                    break;
            }
        }
        else if (ClearFlag.IsClearFlag() &&
            SceneManager.GetActiveScene().name == "Game")
        {
            CurrentGameState = GameState.GameClaer;
            soundController.StopBGM();
            switch(CurrentGameMode)
            {
                case GameMode.ZeldaMode:
                    soundController.PlayBgm(soundManager.GetBGMClips()[(int)SoundData.BGMTag.GameCler01]);
                    break;
                case GameMode.RatchetAndClank:
                    soundController.PlaySe(soundManager.GetSEClips()[(int)SoundData.SETag.GameCler02]);
                    break;
                case GameMode.SuperMario:
                    soundController.PlaySe(soundManager.GetSEClips()[(int)SoundData.SETag.GameCler03]);
                    break;
            }
        }
    }
}
