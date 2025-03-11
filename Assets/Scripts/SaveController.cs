using TMPro;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    public void Save(int score, string scene)
    {
        if (score > SaveManager.GetInt("TopScoreScene1", 0) && scene.Equals("1"))
            SaveManager.SetInt("TopScoreScene1", score);
        if (score > SaveManager.GetInt("TopScoreScene2", 0) && scene.Equals("2"))
            SaveManager.SetInt("TopScoreScene1", score);
        if (score > SaveManager.GetInt("TopScoreScene3", 0) && scene.Equals("3"))
            SaveManager.SetInt("TopScoreScene1", score);
        SaveManager.Save();
    }
}
