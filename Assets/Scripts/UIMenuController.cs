using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject Tutorial;

    public void CloseTutorial()
    {
        Tutorial.SetActive(false);
    }

    public void OpenTutorial()
    {
        Tutorial.SetActive(true);
    }
}
