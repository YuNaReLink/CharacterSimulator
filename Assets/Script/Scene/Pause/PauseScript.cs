using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [SerializeField]
    //�|�[�Y�������ɕ\������UI�̃v���n�u
    private GameObject pauseUI;
    //�Q�[���ĊJ�{�^��
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
            //�|�[�YUI�̃A�N�e�B�u�A��A�N�e�B�u��؂�ւ�
            pauseUI.SetActive(!pauseUI.activeSelf);

            //�|�[�YUI���\������Ă鎞�͒�~
            if (pauseUI.activeSelf)
            {
                CursorController.SetCursorState(true);
                CursorController.SetCursorLookMode(CursorLockMode.None);
                Time.timeScale = 0f;
            }
            //�|�[�YUI���\������Ă��Ȃ���Βʏ�ʂ�i�s
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
        //�|�[�YUI�̃A�N�e�B�u�A��A�N�e�B�u��؂�ւ�
        pauseUI.SetActive(!pauseUI.activeSelf);
        CursorController.SetCursorState(false);
        CursorController.SetCursorLookMode(CursorLockMode.Locked);
        //�Q�[�����Ԃ��Đ�����
        Time.timeScale = 1f;
    }
}
