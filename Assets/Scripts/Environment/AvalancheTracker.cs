using UnityEngine;

public class AvalancheTracker : MonoBehaviour
{
    public static AvalancheTracker Instance;
    private int avalancheState;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        Invoke(nameof(TurnOffAvalanche), 5f);
    }

    public void TurnOn(ref int i, Vector2 positionPlayer)
    {
        GetComponent<AudioSource>().Play();
        gameObject.SetActive(true);
        if (i == 2)
        {
            GetComponent<ParallaxBackground>().IsDead = true;
        }
        Invoke(nameof(TurnOffAvalanche), 5f); // Tự động tắt sau 5 giây
    }

    private void TurnOffAvalanche()
    {
        // Use the stored state
        TurnOff(ref avalancheState);
    }

    public void TurnOff(ref int i)
    {
        i = 0;
        GetComponent<AudioSource>().Stop();
        gameObject.SetActive(false);
    }
}
