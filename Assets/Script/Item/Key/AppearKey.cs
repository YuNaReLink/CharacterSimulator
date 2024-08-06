using UnityEngine;

//鍵を出現させるだけのクラス
public class AppearKey : MonoBehaviour
{

    [SerializeField]
    private GameObject key;

    public void Appear()
    {
        Instantiate(key,transform.position,transform.rotation);
    }

}
