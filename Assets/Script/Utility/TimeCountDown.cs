using System;
using UnityEngine;

//Unity��Time.deltaTime���g�����J�E���g�_�E���̃N���X
//CountDown�Ƃ͈Ⴂ�t���[���ł͂Ȃ��^�C�}�[�����ŃJ�E���g����
public class TimeCountDown : InterfaceCountDown
{

    public event Action OnCompleted;

    private float count = 0;

    public void End()
    {
        count = 0;
        OnCompleted?.Invoke();
        OnCompleted = null;
    }
    //�J�E���g���L�����ǂ���
    public bool IsEnabled() { return count > 0; }

    public bool MaxCount(float _count) { return count >= _count; }

    public void StartTimer(float _count)
    {
        count = _count;
    }

    // Update is called once per frame
    public void Update()
    {
        count -= Time.deltaTime;
        if (count <= 0)
        {
            End();
        }
    }
}
