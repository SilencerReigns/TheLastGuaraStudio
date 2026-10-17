using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LeaderboardSystem
{
    public class LeaderboardUI : MonoBehaviour
    {
        [SerializeField] private EffectsController effectsController;

        [Header("Elements")]
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private Button submitButton;

        [Header("Displays")]
        [SerializeField] private List<TextMeshProUGUI> names;
        [SerializeField] private List<TextMeshProUGUI> tiers;
        [SerializeField] private List<TextMeshProUGUI> scores;
        [SerializeField] private List<RectTransform> displays;
        [SerializeField] private int namesPerDisplay;

        private int usedDisplays;
        private int currentDisplayIndex;
        private int lastDisplayIndex;
        private bool displayBusy;

        private void Awake()
        {
            for (int i = 0; i < names.Count; i++)
            {
                names[i].text = "";
                tiers[i].text = "";
                scores[i].text = "";
            }
        }

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(submitButton.gameObject);

            effectsController.ApplyStartEffects();
        }

        public string GetName()
        {
            string playerName = nameInput.text.Trim();

            playerName = Regex.Replace(playerName, @"\s+", "_");
            playerName = playerName.ToUpper();

            nameInput.text = "";

            return playerName;
        }

        public void UpdateValues(List<LeaderboardEntry> entries)
        {
            for (int i = 0; i < names.Count; i++)
            {
                if (i < entries.Count)
                {
                    var entry = entries[i];

                    string displayName = entry.PlayerName.Split('#')[0].Replace('_', ' ').ToUpper();
                    string displayTier = entry.Tier.Replace('_', ' ').ToUpper();

                    names[i].text = displayName;
                    tiers[i].text = displayTier;
                    scores[i].text = entry.Score.ToString("0");
                }
                else
                {
                    names[i].text = "";
                    tiers[i].text = "";
                    scores[i].text = "";
                }
            }

            //Identifica quantos displays são usados
            int division = Mathf.CeilToInt((float)entries.Count / namesPerDisplay);

            usedDisplays = Mathf.Clamp(division, 1, displays.Count);
        }

        public void SubmitScore()
        {
            nameInput.interactable = false;
            submitButton.interactable = false;
        }

        public void ChangeDisplay()
        {
            if (usedDisplays < 2 || displayBusy)
                return;

            displayBusy = true;

            currentDisplayIndex = (currentDisplayIndex + 1) % usedDisplays;
            lastDisplayIndex = (currentDisplayIndex - 1 + usedDisplays) % usedDisplays;

            RectTransform displayIn = displays[currentDisplayIndex];
            RectTransform displayOut = displays[lastDisplayIndex];

            effectsController.ApplyDisplayEffects(displayIn, displayOut, () => displayBusy = false);
        }
    }
}