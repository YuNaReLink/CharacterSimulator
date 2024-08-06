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
                text = "アタック";
                break;
            case CharacterManager.ActionState.Grab:
                text = "つかまり";
                break;
            case CharacterManager.ActionState.ClimbWall:
                text = "のぼる";
                break;
            case CharacterManager.ActionState.Falling:
                text = "おちる";
                break;
            case CharacterManager.ActionState.Idle:
                if (playerController.CurrentBattleFlag)
                {
                    text = "しまう";
                }
                else
                {
                    text = "アタック";
                }
                break;
        }
        return text;
    }
}
