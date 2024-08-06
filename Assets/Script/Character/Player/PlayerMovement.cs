using UnityEngine;

public class PlayerMovement
{
    private PlayerController controller = null;
    public PlayerMovement(PlayerController _controller)
    {
        controller = _controller;
    }

    public Vector3 AcceleExecute(Vector3 forward, Vector3 right, float _maxspeed, float _accele)
    {
        Vector3 vel = controller.Velocity;
        float h = controller.GetStateInput().Horizontalinput;
        
        float v = controller.GetStateInput().VerticalInput;
        
        vel += (h * right + v * forward) * _accele;
        // 現在の速度の大きさを計算
        float currentSpeed = vel.magnitude;
        // もし現在の速度が最大速度未満ならば、加速度を適用する
        // 現在の速度が最大速度以上の場合は速度を最大速度に制限する
        if (currentSpeed >= _maxspeed)
        {
            vel = vel.normalized * _maxspeed;
        }
        return vel;
    }

    public void StopVelocityXZ()
    {
        if (!controller.GetTimer().Timer_Avoid.IsEnabled())
        {
            controller.Velocity = new Vector3(0, controller.GetCharacterRb().velocity.y,0);
            controller.GetCharacterRb().velocity = new Vector3(0, controller.GetCharacterRb().velocity.y, 0);
        }
    }

    public void StopVelocityXYZ()
    {
        if (!controller.GetTimer().Timer_Avoid.IsEnabled())
        {
            controller.Velocity = Vector3.zero;
            controller.GetCharacterRb().velocity = Vector3.zero;
        }
    }

    public void Deceleration(Vector3 forward, Vector3 right, float _minspeed, float _decele)
    {

        Vector3 v = controller.Velocity;
        _decele = controller.AddDecelerationSetting(_decele);
        v *= _decele;
        // 現在の速度の大きさを計算
        float currentSpeed = v.magnitude;
        controller.DeceleFlag = true;
        // 現在の速度が最低速度以上の場合は速度を最低速度に制限する
        if (currentSpeed <= _minspeed)
        {
            v = v.normalized * 0;
            controller.DeceleFlag = false;
        }

        controller.Velocity = v;
    }

}
