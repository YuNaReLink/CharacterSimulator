using UnityEngine;

//�����o�������邾���̃N���X
public class AppearKey : MonoBehaviour
{

    [SerializeField]
    private GameObject key;

    public void Appear()
    {
        Instantiate(key,transform.position,transform.rotation);
    }

}
