using UnityEngine;
using UnityEngine.UI;
using static CharacterManager;
using static GameDataManager;

public class TitleUIController : MonoBehaviour
{
    [SerializeField]
    private TitleController controller;


    //�{�^���̃J�E���g
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
    //�X�^�[�g�{�^��
    [SerializeField]
    private Button startButton;
    //�^�C�g�����I�u�W�F�N�g
    [SerializeField]
    private GameObject titleName;
    //�G���h�{�^��
    [SerializeField]
    private Button endButton;
    //easy�{�^��
    [SerializeField]
    private Button easyButton;
    //normal�{�^��
    [SerializeField]
    private Button normalButton;
    //Hard�{�^��
    [SerializeField]
    private Button hardBotton;
    //GameStart�{�^��
    [SerializeField]
    private Button gameStartButton;
    //�X�^�[�g�ɖ߂�{�^��
    [SerializeField]
    private Button retrunButton;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<TitleController>();

        //�ŏ��̓[���_���[�h
        SetZeldaMode();
    }

    public void SetZeldaMode()
    {
        //�[���_���[�h�̐ݒ������
        Debug.Log("�[���_���[�h");
        controller.SetPlayerTag(DataTag.Zelda);
        controller.SetGameMode(GameDataManager.GameMode.ZeldaMode);
    }

    public void SetRatchetAndClankMode()
    {
        //���`�F�b�g&�N�����N���[�h�̐ݒ������
        Debug.Log("���`�F�b�g&�N�����N���[�h");
        controller.SetPlayerTag(DataTag.RatchetAndClank);
        controller.SetGameMode(GameDataManager.GameMode.RatchetAndClank);
    }

    public void SetSuperMarioMode()
    {
        Debug.Log("�}���I���[�h");
        //�}���I���[�h�̐ݒ������
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
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���I��
#else
    Application.Quit();//�Q�[���v���C�I��
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
