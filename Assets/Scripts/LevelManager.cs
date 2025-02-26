using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    float sceneLoadDelay = 2f;

    void Awake()
    {
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Round1");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LoadGameOver()
    {
        StartCoroutine(WaitAndLoad("Over", sceneLoadDelay));
    }
    public void LoadRounds()
    {
        SceneManager.LoadScene("RoundList");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
