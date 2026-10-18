
using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class StyleSystem : MonoBehaviour
{
    [Header("Style Settings")]
    [SerializeField] private int hitsRequired = 7;
    [SerializeField] private float bonusTime = 10f;
    [SerializeField] private float inactivityTime = 7f;

    [Header("UI")]
    [SerializeField] private TMP_Text styleText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text announcementText;

    [Header("Announcement")]
    [SerializeField] private float announcementDuration = 2f;

    private readonly string[] styleRanks = { "D", "C", "B", "A", "S" };

    private int currentRank = 0;
    private int consecutiveHits = 0;

    private float lastHitTime;
    private Coroutine announcementCoroutine;

    public event Action<float> TimeBonusRequested;

    public string CurrentStyle => styleRanks[currentRank];
    public int ConsecutiveHits => consecutiveHits;
    public bool IsAtMaxStyle => currentRank == styleRanks.Length - 1;

    private void Start()
    {
        lastHitTime = Time.time;
        UpdateUI();

        if (announcementText != null)
            announcementText.text = "";
    }

    private void Update()
    {
        if (currentRank > 0 &&
            Time.time - lastHitTime >= inactivityTime)
        {
            DecreaseStyle("STYLE DECAY!");
            lastHitTime = Time.time;
        }
    }

    public void RegisterHit()
    {
        consecutiveHits++;
        lastHitTime = Time.time;

        if (consecutiveHits >= hitsRequired)
        {
            consecutiveHits = 0;
            AdvanceStyle();
        }

        UpdateUI();
    }

    public void RegisterDamageTaken()
    {
        consecutiveHits = 0;
        lastHitTime = Time.time;

        DecreaseStyle("HIT TAKEN!");
    }

    private void AdvanceStyle()
    {
        if (currentRank < styleRanks.Length - 1)
        {
            currentRank++;

            ShowAnnouncement("STYLE UP! " + styleRanks[currentRank]);
        }

        if (IsAtMaxStyle)
        {
            TimeBonusRequested?.Invoke(bonusTime);

            currentRank = 0;
            consecutiveHits = 0;
            lastHitTime = Time.time;

            ShowAnnouncement(
                "STYLISH! +" + bonusTime.ToString("0") + " SECONDS"
            );
        }

        UpdateUI();
    }

    private void DecreaseStyle(string message)
    {
        if (currentRank > 0)
        {
            currentRank--;
            ShowAnnouncement(message + " " + styleRanks[currentRank]);
        }
        else
        {
            ShowAnnouncement(message);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (styleText != null)
            styleText.text = styleRanks[currentRank];

        if (comboText != null)
            comboText.text = consecutiveHits + " / " + hitsRequired;
    }

    private void ShowAnnouncement(string message)
    {
        if (announcementText == null) return;

        if (announcementCoroutine != null)
            StopCoroutine(announcementCoroutine);

        announcementCoroutine = StartCoroutine(DisplayAnnouncement(message));
    }

    private IEnumerator DisplayAnnouncement(string message)
    {
        announcementText.text = message;

        yield return new WaitForSeconds(announcementDuration);

        announcementText.text = "";
        announcementCoroutine = null;
    }
}
