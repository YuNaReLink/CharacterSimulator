using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CharacterManager;

public class PlayerPropsSetting : PropsSettingBase
{
    public enum PlayerToolTag
    {
        Null = -1,
        Sword,
        Scabbard,
        Shild,
        BombGun,
        Propeller,
        Foot,
        Arm,
        DataEnd
    }
    [SerializeField]
    private List<GameObject> toolObjects = new List<GameObject>();

    //���̃R���C�_�[
    [SerializeField]
    private Collider            swordCollider;
    //��_���[�W�I�u�W�F�N�g
    [SerializeField]
    private Collider            damageBody;
    //���[���̌��̈ʒu
    [SerializeField]
    private GameObject          swordPos;
    //�E��̓���̈ʒu
    [SerializeField]
    private GameObject          battleSwordPos;
    //���[���̏��̈ʒu
    [SerializeField]
    private GameObject          shildPos;
    //����̓���̈ʒu
    [SerializeField]
    private GameObject          battleShildPos;



    public void InitalizeToolSetting(PlayerController controller)
    {
        //�I�u�W�F�N�g��S�Ĕ�\����
        for(int i = 0; i < toolObjects.Count; i++)
        {
            toolObjects[i].SetActive(false);
        }
        //�L�������ƂɃI�u�W�F�N�g��\��������
        if (controller.GetTag() == DataTag.Zelda)
        {
            controller.CurrentBattleFlag = false;
            ActiveSwordCollider(false);
            ActiveSword(true);
            ActiveShild(true);
            ActiveScabbard(true);
        }
        else if (controller.GetTag() == DataTag.RatchetAndClank)
        {
            controller.CurrentBattleFlag = true;
            SetSwordPostion(controller);
            ActiveGun(false);
            ActiveSword(true);
            ActiveShild(false);
            ActiveScabbard(false);
        }
        else if (controller.GetTag() == DataTag.SuperMario)
        {
            ActiveGun(false);
            ActiveSword(false);
            ActiveShild(false);
            ActiveScabbard(false);
        }
    }

    public void PropsUpdateSetting(PlayerController controller)
    {
        //����؂�ւ��鏈��
        if (controller.CurrentBattleFlag != controller.PreviousStateFlag)
        {
            SetSwordPostion(controller);
            controller.PreviousStateFlag = controller.CurrentBattleFlag;
        }
        //����؂�ւ��鏈��
        if (controller.GetStateInput().IsGurid != controller.GetStateInput().IsMouseRightClick() && controller.GetScriptableObject().GuardEnabled != 0)
        {
            controller.GetStateInput().IsGurid = controller.GetStateInput().IsMouseRightClick();
            SetShieldPostion(controller.GetStateInput().IsGurid);
        }
        //�v���y���̕\���E��\����؂�ւ���
        if (controller.GetCurrentState() == ActionState.GlideJump)
        {
            ActivePropeller(true);
        }
        else
        {
            ActivePropeller(false);
        }
        //���t�߂ɂ��鑫�̍U�������؂�ւ��鏈��
        if (!controller.Landing)
        {
            if (controller.GetCurrentState() == ActionState.JumpAttack||
                controller.GetTag() != DataTag.SuperMario) { return; }
            ActiveFootDamageObject(true);
        }
        else
        {
            ActiveFootDamageObject(false);
        }
        //�茳�̍U�������؂�ւ��鏈��
        if (controller.GetTimer().Timer_ForwardAccele.IsEnabled())
        {
            if(controller.GetTag() != DataTag.SuperMario) { return; }
            ActiveArmDamageObject(true);
        }

        if (!controller.GetTimer().Timer_NoGravity.IsEnabled())
        {
            ActiveDamageBody(true);
        }
    }

    public void SetColliderEnabled(PlayerController controller)
    {
        if(controller.GetTag() == DataTag.SuperMario) { return; }
        //���̓����蔻���؂�ւ��鏈��
        SetSwordCollider(controller);
        //���̓����蔻���؂�ւ��鏈��
        SetShildCollider(controller);
    }

    private void SetSwordCollider(PlayerController controller)
    {

        if (SetSwordColliderActive(controller))
        {
            ActiveSwordCollider(true);
        }
        else
        {
            ActiveSwordCollider(false);
        }
    }

    private bool SetSwordColliderActive(PlayerController controller)
    {
        bool states = controller.GetCurrentState() == ActionState.Attack ||
            controller.GetCurrentState() == ActionState.JumpAttack ||
            controller.GetCurrentState() == ActionState.SpinAttack ||
            controller.GetCurrentState() == ActionState.CrouchAttack;
        AnimatorStateInfo animatorStateInfo = controller.GetAnim().GetCurrentAnimatorStateInfo(0);
        bool motionTime = animatorStateInfo.normalizedTime < 0.7f;
        if (states&&motionTime)
        {
            return true;
        }
        return false;
    }

    private void SetShildCollider(PlayerController controller)
    {
        if (controller.GetStateInput().BlockState != ShieldBlockState.Null)
        {
            ActiveShildCollider(true);
        }
        else
        {
            ActiveShildCollider(false);
        }
    }

    public void SetSwordPostion(PlayerController controller)
    {
        bool battlemode = controller.CurrentBattleFlag;
        if(battlemode)
        {
            for (int i = 0; i < swordPos.transform.childCount; i++)
            {
                if (swordPos?.transform.GetChild(i).name == "Sword")
                {
                    toolObjects[(int)PlayerToolTag.Sword].transform.SetParent(null);
                    toolObjects[(int)PlayerToolTag.Sword].transform.SetParent(battleSwordPos.transform);
                    toolObjects[(int)PlayerToolTag.Sword].transform.position = battleSwordPos.transform.position;
                    toolObjects[(int)PlayerToolTag.Sword].transform.rotation = battleSwordPos.transform.rotation;
                }
            }
        }
        else
        {
            toolObjects[(int)PlayerToolTag.Sword].transform.SetParent (null);
            toolObjects[(int)PlayerToolTag.Sword].transform.SetParent(swordPos.transform);
            toolObjects[(int)PlayerToolTag.Sword].transform.position = swordPos.transform.position;
            toolObjects[(int)PlayerToolTag.Sword].transform.rotation = swordPos.transform.rotation;
        }
    }

    public void SetShieldPostion(bool flag)
    {
        if (flag)
        {
            toolObjects[(int)PlayerToolTag.Shild].transform.SetParent(null);
            toolObjects[(int)PlayerToolTag.Shild].transform.SetParent(battleShildPos.transform);
            toolObjects[(int)PlayerToolTag.Shild].transform.position = battleShildPos.transform.position;
            toolObjects[(int)PlayerToolTag.Shild].transform.rotation = battleShildPos.transform.rotation;
        }
        else
        {
            toolObjects[(int)PlayerToolTag.Shild].transform.SetParent(null);
            toolObjects[(int)PlayerToolTag.Shild].transform.SetParent(shildPos.transform);
            toolObjects[(int)PlayerToolTag.Shild].transform.position = shildPos.transform.position;
            toolObjects[(int)PlayerToolTag.Shild].transform.rotation = shildPos.transform.rotation;
        }
    }

    public void SetActiveSwordOnly()
    {
        ActiveGun(false);
        ActiveSword(true);
    }

    public void ActiveSword(bool _enabled)
    {
        if(toolObjects[(int)PlayerToolTag.Sword] == null) { return; }
        if (toolObjects[(int)PlayerToolTag.Sword].activeSelf == _enabled) { return; }
        toolObjects[(int)PlayerToolTag.Sword].SetActive(_enabled);
    }
    public bool IsActiveSword() {  return toolObjects[(int)PlayerToolTag.Sword].activeSelf; }

    public void ActiveScabbard(bool _enabled)
    {
        if (toolObjects[(int)PlayerToolTag.Scabbard] == null) { return; }
        if (toolObjects[(int)PlayerToolTag.Scabbard].activeSelf == _enabled) { return; }
        toolObjects[(int)PlayerToolTag.Scabbard].SetActive(_enabled);
    }

    public void ActiveSwordCollider(bool _enabled)
    {
        if(swordCollider == null) { return; }
        if(swordCollider.enabled == _enabled) { return; }
        swordCollider.enabled = _enabled;
    }

    
    public void ActiveGun(bool _enabled)
    {
        if(toolObjects[(int)PlayerToolTag.BombGun] == null) { return; }
        if(toolObjects[(int)PlayerToolTag.BombGun].activeSelf == _enabled) {  return; }
        toolObjects[(int)PlayerToolTag.BombGun].SetActive(_enabled);
    }
    public bool IsActiveGun() {  return toolObjects[(int)PlayerToolTag.BombGun].activeSelf; }

    public void ActivePropeller(bool _enabled)
    {
        if (toolObjects[(int)PlayerToolTag.Propeller] == null) { return; }
        if(toolObjects[(int)PlayerToolTag.Propeller].activeSelf == _enabled) { return; }
        toolObjects[(int)PlayerToolTag.Propeller].SetActive(_enabled);
    }

    public void ActiveFootDamageObject(bool _enabled)
    {
        if(toolObjects[(int)PlayerToolTag.Foot] == null) { return; }
        if(toolObjects[(int)PlayerToolTag.Foot].activeSelf == _enabled) { return; }
        toolObjects[(int)PlayerToolTag.Foot].SetActive(_enabled);
    }

    public void ActiveArmDamageObject(bool _enabled)
    {
        if (toolObjects[(int)PlayerToolTag.Arm] == null) { return; }
        if(toolObjects[(int)PlayerToolTag.Arm].activeSelf == _enabled) { return; }
        toolObjects[(int)PlayerToolTag.Arm].SetActive(_enabled);
    }

    public void ActiveDamageBody(bool _enabled)
    {
        if (damageBody == null) { return;}
        if(damageBody.enabled == _enabled) { return; }
        damageBody.enabled = _enabled;
    }
}
