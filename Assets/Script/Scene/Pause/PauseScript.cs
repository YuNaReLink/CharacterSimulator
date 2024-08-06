using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [SerializeField]
    //ポーズした時に表示するUIのプレハブ
    private GameObject pauseUI;
    //ゲーム再開ボタン
    [SerializeField]
    private GameObject reStartButton;

    private bool StopPause()
    {
        bool stop = false;
        if (GameDataManager.CurrentGameState != GameDataManager.GameState.NowGame)
        {
            stop = true;
        }
        if(GameDataManager.CurrentGameState == GameDataManager.GameState.Pause)
        {
            stop = false;
        }
        return stop;
    }

    void Update()
    {
        if(StopPause())
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //ポーズUIのアクティブ、非アクティブを切り替え
            pauseUI.SetActive(!pauseUI.activeSelf);

            //ポーズUIが表示されてる時は停止
            if (pauseUI.activeSelf)
            {
                CursorController.SetCursorState(true);
                CursorController.SetCursorLookMode(CursorLockMode.None);
                Time.timeScale = 0f;
            }
            //ポーズUIが表示されていなければ通常通り進行
            else
            {
                CursorController.SetCursorState(false);
                CursorController.SetCursorLookMode(CursorLockMode.Locked);
                Time.timeScale = 1f;
            }
        }
    }

    public void ReStartButton()
    {
        //ポーズUIのアクティブ、非アクティブを切り替え
        pauseUI.SetActive(!pauseUI.activeSelf);
        CursorController.SetCursorState(false);
        CursorController.SetCursorLookMode(CursorLockMode.Locked);
        //ゲーム時間を再生する
        Time.timeScale = 1f;
    }
}
