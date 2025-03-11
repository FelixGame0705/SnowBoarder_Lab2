using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField]
    float loadDelay = 2f;

    [SerializeField]
    ParticleSystem finishEffect;

    [SerializeField]
    GameObject winMenu;

    [SerializeField]
    SaveController _saveController;

    [SerializeField]
    string nameScene;

    [SerializeField]
    ScoreManager _scoreManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            finishEffect.Play();
            GetComponent<AudioSource>().Play();
            Invoke("LoadWinPopup", loadDelay);
            _saveController.Save(_scoreManager.GetScore(), nameScene);
        }
    }

    private void LoadWinPopup()
    {
        Time.timeScale = 0;
        winMenu.SetActive(true);
    }
}
