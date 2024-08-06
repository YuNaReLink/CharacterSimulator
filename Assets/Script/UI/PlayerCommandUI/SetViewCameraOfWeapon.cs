using System.Collections.Generic;
using UnityEngine;

enum UI_Weapon
{
    Null = -1,
    Sword,
    Gun,
    DataEnd
}

public class SetViewCameraOfWeapon : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]
    private List<GameObject> weaponUI = new List<GameObject>();
    void Start()
    {
        CommandUI commandUI = GetComponentInParent<CommandUI>();
        playerController = commandUI.GetPlayerController();
    }

    void Update()
    {
        SetUIWeaponActive();
    }

    private void SetUIWeaponActive()
    {
        for(int i = 0; i < weaponUI.Count; i++)
        {
            weaponUI[i].SetActive(false);
        }
        if (playerController.GetPropssetting().IsActiveSword())
        {
            weaponUI[(int)UI_Weapon.Sword].SetActive(true);
        }
        else if (playerController.GetPropssetting().IsActiveGun())
        {
            weaponUI[(int)UI_Weapon.Gun].SetActive(true);
        }
    }
}
