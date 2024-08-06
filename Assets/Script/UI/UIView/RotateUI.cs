using UnityEngine;

public class RotateUI : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = 1.5f;
    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed);
    }
}
