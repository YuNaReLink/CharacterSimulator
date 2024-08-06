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
        // ���݂̑��x�̑傫�����v�Z
        float currentSpeed = vel.magnitude;
        // �������݂̑��x���ő呬�x�����Ȃ�΁A�����x��K�p����
        // ���݂̑��x���ő呬�x�ȏ�̏ꍇ�͑��x���ő呬�x�ɐ�������
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
        // ���݂̑��x�̑傫�����v�Z
        float currentSpeed = v.magnitude;
        controller.DeceleFlag = true;
        // ���݂̑��x���Œᑬ�x�ȏ�̏ꍇ�͑��x���Œᑬ�x�ɐ�������
        if (currentSpeed <= _minspeed)
        {
            v = v.normalized * 0;
            controller.DeceleFlag = false;
        }

        controller.Velocity = v;
    }

}
