using UnityEngine;
using UnityEngine.UI;
using static CharacterManager;
using static GameDataManager;

public class TitleUIController : MonoBehaviour
{
    [SerializeField]
    private TitleController controller;


    //ボタンのカウント
    private enum TitleUI
    {
        Null = -1,
        titleName,
        startButton,
        endButton,
        easyButton,
        normalButton,
        hardButton,
        gameStartButton,
        DataEnd
    }
    //スタートボタン
    [SerializeField]
    private Button startButton;
    //タイトル名オブジェクト
    [SerializeField]
    private GameObject titleName;
    //エンドボタン
    [SerializeField]
    private Button endButton;
    //easyボタン
    [SerializeField]
    private Button easyButton;
    //normalボタン
    [SerializeField]
    private Button normalButton;
    //Hardボタン
    [SerializeField]
    private Button hardBotton;
    //GameStartボタン
    [SerializeField]
    private Button gameStartButton;
    //スタートに戻るボタン
    [SerializeField]
    private Button retrunButton;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<TitleController>();

        //最初はゼルダモード
        SetZeldaMode();
    }

    public void SetZeldaMode()
    {
        //ゼルダモードの設定をする
        Debug.Log("ゼルダモード");
        controller.SetPlayerTag(DataTag.Zelda);
        controller.SetGameMode(GameDataManager.GameMode.ZeldaMode);
    }

    public void SetRatchetAndClankMode()
    {
        //ラチェット&クランクモードの設定をする
        Debug.Log("ラチェット&クランクモード");
        controller.SetPlayerTag(DataTag.RatchetAndClank);
        controller.SetGameMode(GameDataManager.GameMode.RatchetAndClank);
    }

    public void SetSuperMarioMode()
    {
        Debug.Log("マリオモード");
        //マリオモードの設定をする
        controller.SetPlayerTag(DataTag.SuperMario);
        controller.SetGameMode(GameDataManager.GameMode.SuperMario);
    }

    public void OnChangeScene(string sceneName)
    {
        MoveScene.OnSceneChange(sceneName);
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲーム終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

    public void OnSelectDifficulty()
    {
        ChangeActive(startButton.gameObject);
        ChangeActive(endButton.gameObject);
        ChangeActive(titleName);
        ChangeActive(easyButton.gameObject);
        ChangeActive(normalButton.gameObject);
        ChangeActive(hardBotton.gameObject);
        ChangeActive(gameStartButton.gameObject);
        ChangeActive(retrunButton.gameObject);
    }

    public void ChangeActive(GameObject gameObject)
    {
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
