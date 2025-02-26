using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundListManager : MonoBehaviour
{
    public void LoadRound1()
    {
        SceneManager.LoadScene("Round1");
    }

    public void LoadRound2()
    {
        SceneManager.LoadScene("Round2");
    }

    public void LoadRound3()
    {
        SceneManager.LoadScene("Round3");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
