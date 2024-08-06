using UnityEngine;
using static GameDataManager;
using static CharacterManager;

public class TitleController : MonoBehaviour
{
    private SoundData soundManager;
    private SoundController soundController;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;
        ChangeGameState(GameState.Title);
        UpdateGameState();
    }

    private void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundData>();
        soundController = soundManager.gameObject.GetComponent<SoundController>();

        soundController.StopBGM();
        soundController.StopSE();
        soundController.BGMVolume = soundController.GetBGMVolumData();
        soundController.PlayBgm(soundManager.GetBGMClips()[(int)SoundData.BGMTag.TitleBGM]);
    }

    public void SetPlayerTag(DataTag _tag)
    {
        PlayerTag = _tag;
    }

    public void SetGameMode(GameMode gameMode)
    {
        CurrentGameMode = gameMode;
    }

    private void Update()
    {
        soundController.ChangeBGMUpdtate();
    }
}
