using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardExecute : MonoBehaviour
{
    [SerializeField]
    private bool hitGuardFlag = false;
    public bool HitGuardFlag { get { return hitGuardFlag; } set {  hitGuardFlag = value; } }

    public void GuardEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            hitGuardFlag = true;
        }
    }
}
