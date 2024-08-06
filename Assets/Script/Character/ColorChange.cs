using UnityEngine;

public class ColorChange
{
    /// <summary>
    /// モデルのマテリアルを変化させるクラス
    /// </summary>
    private SkinnedMeshRenderer     skinnedMeshRenderer;
    private Color[]                 originalColors;
    private Color                   targetColor = Color.red;
    // 色が変わるのにかかる時間
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
        // 各マテリアルの元の色を取得します。
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
        // オリジナルの色に戻します。
        // SkinnedMeshRendererの全てのマテリアルの色を切り替えます。
        for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
        {
            // オリジナルの色に戻します。
            skinnedMeshRenderer.materials[i].color = originalColors[i];
        }
    }

    public void ColorTransitionUpdate()
    {
        if (transitions == Transitions.damage)
        {
            // 指定された時間内に色を変化させる
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);

            // 元の色から目標の色に向かって補間
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                skinnedMeshRenderer.materials[i].color = Color.Lerp(targetColor, originalColors[i], t);
            }

            // 時間が経過したら処理を停止
            if (transitionTimer >= transitionDuration)
            {
                transitions = Transitions.Null;
            }
        }
    }

    // 色の遷移を開始するメソッド
    public void StartColorTransition(Transitions _transitions)
    {
        transitions = _transitions;
        transitionTimer = 0f;
    }
}
