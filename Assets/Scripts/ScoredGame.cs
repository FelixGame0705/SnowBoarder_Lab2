using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScoredGame : MonoBehaviour
    {
        [SerializeField]
        GameObject scorePopup;

        [SerializeField]
        TextMeshProUGUI scoreScene1;

        [SerializeField]
        TextMeshProUGUI scoreScene2;

        [SerializeField]
        TextMeshProUGUI scoreScene3;

        public void TurnOnScorePopup()
        {
            scorePopup.SetActive(true);
            SaveManager.Load();
            scoreScene1.text = SaveManager.GetInt("TopScoreScene1", 0).ToString();
            scoreScene2.text = SaveManager.GetInt("TopScoreScene2", 0).ToString();
            scoreScene3.text = SaveManager.GetInt("TopScoreScene3", 0).ToString();
        }

        public void TurnOffScorePopup()
        {
            scorePopup.SetActive(false);
        }
    }
}
