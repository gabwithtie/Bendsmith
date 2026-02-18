using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string scene_name;
    [SerializeField] private bool use_loading_screen = true;

    public void LoadScene()
    {
        if (use_loading_screen)
        {
            LoadingScreen.LoadScreen(() =>
            {
                SceneManager.LoadScene(scene_name);
            });
        }
        else
        {
            SceneManager.LoadScene(scene_name);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}