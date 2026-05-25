using UnityEngine.SceneManagement;
using UnityEngine;
using NaughtyAttributes;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField, Scene] private string _gameScene;
    public void OnClickStart()
    {
        SceneManager.LoadScene(_gameScene);
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
}
