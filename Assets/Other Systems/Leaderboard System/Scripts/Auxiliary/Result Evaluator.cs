using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LeaderboardSystem
{
    public class ResultEvaluator : MonoBehaviour
    {
        [SerializeField] private GeneralData generalData;

        [SerializeField] private Image targetImage;
        [SerializeField] private TextMeshProUGUI tierText;

        private void Start()
        {
            if (!generalData.evaluatesResult)
            {
                targetImage.color = Color.clear;
                tierText.maxVisibleCharacters = 0;

                return;
            }

            int score = PlayerPrefs.GetInt("Score", 0);
            int maxScore = generalData.maxScore;

            if (score < maxScore / 3)
                targetImage.sprite = generalData.badResultImage;
            else if (score < maxScore / 1.5)
                targetImage.sprite = generalData.mediocreResultImage;
            else
                targetImage.sprite = generalData.goodResultImage;

            int tier = 0;
            int length = generalData.tierThresholds.Length;

            for (int i = 1; i < length; i++)
            {
                //Verifica se a posição seguinte é maior
                if (score < generalData.tierThresholds[i])
                    break;

                tier = i + 1;
            }

            tierText.text = generalData.tierNames[tier];
        }
    }
}