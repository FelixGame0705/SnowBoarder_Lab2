using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGamePlayController : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    public void Next()
    {
        if (SceneManager.GetActiveScene().name != "Round3")
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            SceneManager.LoadScene(0);
        }
    }
    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void Restart()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
