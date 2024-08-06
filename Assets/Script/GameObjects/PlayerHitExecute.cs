using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitExecute : MonoBehaviour
{
    private bool playerHit = false;

    public bool PlayerHit { get{ return playerHit; }set { playerHit = value; } }

    public void OnTriggerEnter(Collider other)
    {
        playerHit = false;
        if(other.tag != "Player") { return; }
        playerHit = true;
    }
}
