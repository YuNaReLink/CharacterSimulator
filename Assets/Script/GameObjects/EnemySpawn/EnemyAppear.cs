using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAppear : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    private GenerateEffects effects;

    [SerializeField]
    private GameObject enabledEnemy;

    private AppearKey appearKey;

    private bool appearEnemyFlag = false;

    void Start()
    {
        effects = GetComponent<GenerateEffects>();
        if(effects == null)
        {
            Debug.Log("effectsがアタッチされていません");
        }
        appearKey = GetComponentInChildren<AppearKey>();
        if(appearKey == null)
        {
            Debug.LogError("appearKeyがアタッチされていません");
        }
    }

    private void Update()
    {
        if(enabledEnemy == null&&!appearEnemyFlag) {
            return; 
        }
        if(appearKey == null) { return; }
        if(enabledEnemy != null) { return; }
        appearKey.Appear();
        appearKey = null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player") { return; }
        if(effects == null) { return; }
        effects.GenerateEffect(0,gameObject.transform.position);
        effects = null;
        enabledEnemy = Instantiate(enemy,gameObject.transform.position,gameObject.transform.rotation);
        appearEnemyFlag = true;
    }
}
