using UnityEngine.SceneManagement;

public class MoveScene
{
    public static void OnSceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
