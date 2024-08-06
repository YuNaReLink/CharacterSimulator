using UnityEngine;

public class RotatePropeller : MonoBehaviour
{
    [SerializeField]
    private float                   rotateSpeed = 1.0f;

    [SerializeField]
    private PropellerSEController   propellerSEController;

    void Update()
    {
        if (!gameObject.activeSelf) {
            propellerSEController.PropellerSEStop();
            return;
        }
        propellerSEController.PropellerSEPlay();
        transform.Rotate(0,rotateSpeed,0);
    }
}
