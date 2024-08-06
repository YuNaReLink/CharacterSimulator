using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    private float       groundCheckRadius = 0.4f;

    [SerializeField]
    private float       groundCheckOffsetY = 0.45f;
    [SerializeField]
    private float       groundCheckDistance = 0.2f;
    [SerializeField]
    private LayerMask   groundLayers = 0;

    private RaycastHit  hit;

    private void FixedUpdate()
    {
        Debug.DrawLine(transform.position, transform.position + groundCheckOffsetY * Vector3.up);
    }

    public bool CheckGroundStatus()
    {
        return Physics.SphereCast(transform.position + groundCheckOffsetY * Vector3.up,
            groundCheckRadius, Vector3.down, out hit, groundCheckDistance, groundLayers,
            QueryTriggerInteraction.Ignore);
    }
}
