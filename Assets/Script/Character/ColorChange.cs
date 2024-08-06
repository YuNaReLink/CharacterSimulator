using UnityEngine;

public class ColorChange
{
    /// <summary>
    /// ���f���̃}�e���A����ω�������N���X
    /// </summary>
    private SkinnedMeshRenderer     skinnedMeshRenderer;
    private Color[]                 originalColors;
    private Color                   targetColor = Color.red;
    // �F���ς��̂ɂ����鎞��
    private float                   transitionDuration = 1.0f; 

    public enum Transitions
    {
        Null = -1,
        damage,
        die,
        DataEnd
    }
    private Transitions             transitions = Transitions.Null;
    private float                   transitionTimer = 0f;

    public SkinnedMeshRenderer      GetSetSkinnedMeshRenderer {  get { return skinnedMeshRenderer; } set { skinnedMeshRenderer = value; } }

    public void InitColor()
    {
        // �e�}�e���A���̌��̐F���擾���܂��B
        originalColors = new Color[skinnedMeshRenderer.materials.Length];
        for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
        {
            if(skinnedMeshRenderer.materials[i].shader.name == "Unlit/Transparent")
            {
                continue;
            }
            originalColors[i] = skinnedMeshRenderer.materials[i].color;
        }
    }

    public void SetOriginalColor()
    {
        // �I���W�i���̐F�ɖ߂��܂��B
        // SkinnedMeshRenderer�̑S�Ẵ}�e���A���̐F��؂�ւ��܂��B
        for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
        {
            // �I���W�i���̐F�ɖ߂��܂��B
            skinnedMeshRenderer.materials[i].color = originalColors[i];
        }
    }

    public void ColorTransitionUpdate()
    {
        if (transitions == Transitions.damage)
        {
            // �w�肳�ꂽ���ԓ��ɐF��ω�������
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);

            // ���̐F����ڕW�̐F�Ɍ������ĕ��
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                skinnedMeshRenderer.materials[i].color = Color.Lerp(targetColor, originalColors[i], t);
            }

            // ���Ԃ��o�߂����珈�����~
            if (transitionTimer >= transitionDuration)
            {
                transitions = Transitions.Null;
            }
        }
    }

    // �F�̑J�ڂ��J�n���郁�\�b�h
    public void StartColorTransition(Transitions _transitions)
    {
        transitions = _transitions;
        transitionTimer = 0f;
    }
}
