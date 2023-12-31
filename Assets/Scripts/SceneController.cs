using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //Will change our scene to the string passed in
    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name; 
    }

    //Reloads the current scene we are in
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Loads out Title scene, Must be called Title exactly
    public string ToTitleScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    //Quits our game
    public void QuitGame()
    {
        Application.Quit(); 
    }
}
