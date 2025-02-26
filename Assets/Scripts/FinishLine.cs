using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] float loadDelay = 2f;
    [SerializeField] ParticleSystem finishEffect;
    [SerializeField] GameObject winMenu;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            finishEffect.Play();
            GetComponent<AudioSource>().Play();
            Invoke("LoadWinPopup", loadDelay);
        }
    }

    private void LoadWinPopup()
    {
        Time.timeScale = 0;
        winMenu.SetActive(true);
    }
}
