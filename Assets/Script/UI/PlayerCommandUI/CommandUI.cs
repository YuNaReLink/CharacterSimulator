using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour
{
    private PlayerController playerController;
    public PlayerController GetPlayerController() {return  playerController;}

    [SerializeField]
    private Text commandText;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        commandText.text = SetCommandText();
    }

    private string SetCommandText()
    {
        string text = null;
        switch (playerController.GetCurrentState())
        {
            case CharacterManager.ActionState.Run:
            case CharacterManager.ActionState.Flip:
            case CharacterManager.ActionState.Attack:
            case CharacterManager.ActionState.ReadySpinAttack:
            case CharacterManager.ActionState.SpinAttack:
            case CharacterManager.ActionState.CrouchAttack:
            case CharacterManager.ActionState.JumpAttack:
            case CharacterManager.ActionState.Guard:
                text = "�A�^�b�N";
                break;
            case CharacterManager.ActionState.Grab:
                text = "���܂�";
                break;
            case CharacterManager.ActionState.ClimbWall:
                text = "�̂ڂ�";
                break;
            case CharacterManager.ActionState.Falling:
                text = "������";
                break;
            case CharacterManager.ActionState.Idle:
                if (playerController.CurrentBattleFlag)
                {
                    text = "���܂�";
                }
                else
                {
                    text = "�A�^�b�N";
                }
                break;
        }
        return text;
    }
}
