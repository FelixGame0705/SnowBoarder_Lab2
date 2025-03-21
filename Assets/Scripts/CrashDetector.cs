using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [SerializeField]
    float loadDelay = 0.5f;

    [SerializeField]
    ParticleSystem crashEffect;

    [SerializeField]
    AudioClip crashSFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            crashEffect.Play();
            GetComponent<AudioSource>().PlayOneShot(crashSFX);
            Invoke("LoadNextScene", loadDelay);
        }
        if (collision.gameObject.CompareTag("CoreObstacle"))
        {
            crashEffect.Play();
            Debug.Log(Equals(collision.gameObject.tag, "CoreObstacle"));
            GetComponent<AudioSource>().PlayOneShot(crashSFX);
            Invoke("LoadNextScene", loadDelay);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            crashEffect.Play();
            Debug.Log(Equals(collision.gameObject.tag, "CoreObstacle"));
            GetComponent<AudioSource>().PlayOneShot(crashSFX);
            Invoke("LoadNextScene", loadDelay);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
