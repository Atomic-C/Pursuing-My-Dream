using UnityEngine.SceneManagement;

/// <summary>
/// Script used as a scene loader helper class
/// </summary>
public static class SceneLoader
{
    /// <summary>
    /// Function that loads a specified scene
    /// </summary>
    /// <param name="scene">Enum that indicates a scene</param>
    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }

    /// <summary>
    /// Function that loads a specified scene
    /// </summary>
    /// <param name="scene">String that indicates a scene</param>
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Enum that indicates a scene
    /// </summary>
    public enum Scene
    {
        First_Level,
        Shop
    }
}
