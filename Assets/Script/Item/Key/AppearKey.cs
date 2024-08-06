using UnityEngine;

//Œ®‚ğoŒ»‚³‚¹‚é‚¾‚¯‚ÌƒNƒ‰ƒX
public class AppearKey : MonoBehaviour
{

    [SerializeField]
    private GameObject key;

    public void Appear()
    {
        Instantiate(key,transform.position,transform.rotation);
    }

}
