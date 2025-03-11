using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;
    private int comboCount = 0;
    private float multiplier = 1.0f;

    [SerializeField]
    float comboResetTime = 2.0f;
    private float comboTimer = 0f;

    // Thời gian fade chung
    [SerializeField]
    float fadeTime = 1f;

    // Text hiển thị tổng điểm
    [SerializeField]
    TextMeshProUGUI textMeshProTotalScore;

    // Text hiển thị điểm combo (cộng dồn trick)
    [SerializeField]
    TextMeshProUGUI textMeshProUGUICombo;

    // Text hiển thị điểm khi nhặt item
    [SerializeField]
    TextMeshProUGUI textMeshProUGUIItem;

    // Text hiển thị điểm khi nhặt item
    [SerializeField]
    TextMeshProUGUI textMeshProUGUIScoreRound;

    // Dùng để hiển thị cộng dồn combo
    private int comboScoreAccumulated = 0;

    // Tham chiếu coroutine để tránh đụng khi combo còn
    private Coroutine fadeComboCoroutine;

    // Tham chiếu coroutine để fade khi ăn item
    private Coroutine fadeItemCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        UpdateScoreUI();
    }

    void Update()
    {
        // Kiểm tra combo
        if (comboCount > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    /// <summary>
    /// Thêm điểm khi nhặt item. Hiển thị +điểm và fade luôn.
    /// </summary>
    public void AddCollectibleScore(int basePoints)
    {
        int pointsAdded = Mathf.RoundToInt(basePoints * multiplier);
        score += pointsAdded;

        UpdateScoreUI();
        ShowItemScoreText(pointsAdded);
    }

    /// <summary>
    /// Thêm điểm khi Trick, tính combo. Điểm hiển thị sẽ giữ lại nếu còn trong combo.
    /// </summary>
    public void AddTrickScore(int trickPoints)
    {
        comboCount++;
        comboTimer = comboResetTime;
        multiplier = 1 + comboCount * 0.1f;

        int pointsToAdd = Mathf.RoundToInt(trickPoints * multiplier);
        score += pointsToAdd;
        UpdateScoreUI();

        comboScoreAccumulated += pointsToAdd;
        ShowComboScoreText();
    }

    /// <summary>
    /// Hiển thị text combo (cộng dồn). Giữ lại text cho đến khi combo hết.
    /// </summary>
    private void ShowComboScoreText()
    {
        // Nếu đang fade combo thì dừng
        if (fadeComboCoroutine != null)
        {
            StopCoroutine(fadeComboCoroutine);
            fadeComboCoroutine = null;
        }

        // Reset alpha = 1
        textMeshProUGUICombo.CrossFadeAlpha(1, 0, false);

        // Đặt text hiển thị số lần combo
        textMeshProUGUICombo.text = $"Combo: {comboCount}";

        // Bật GameObject
        textMeshProUGUICombo.gameObject.SetActive(true);
    }

    /// <summary>
    /// Khi combo hết thời gian, bắt đầu fade-out combo text.
    /// </summary>
    public void ResetCombo()
    {
        comboCount = 0;
        multiplier = 1.0f;

        fadeComboCoroutine = StartCoroutine(FadeOutComboText());
    }

    /// <summary>
    /// Fade-out combo text, sau đó ẩn và reset comboScoreAccumulated.
    /// </summary>
    IEnumerator FadeOutComboText()
    {
        textMeshProUGUICombo.CrossFadeAlpha(0, fadeTime, false);
        yield return new WaitForSeconds(fadeTime);

        textMeshProUGUICombo.gameObject.SetActive(false);
        comboScoreAccumulated = 0;
        fadeComboCoroutine = null;
    }

    /// <summary>
    /// Hiển thị +điểm khi nhặt item và fade ngay.
    /// </summary>
    private void ShowItemScoreText(int points)
    {
        // Nếu đang fade thì dừng
        if (fadeItemCoroutine != null)
        {
            StopCoroutine(fadeItemCoroutine);
            fadeItemCoroutine = null;
        }

        // Reset alpha = 1
        textMeshProUGUIItem.CrossFadeAlpha(1, 0, false);

        // Ghi text
        textMeshProUGUIItem.text = $"+{points}";

        // Bật GameObject
        textMeshProUGUIItem.gameObject.SetActive(true);

        // Gọi coroutine fade
        fadeItemCoroutine = StartCoroutine(FadeOutItemText());
    }

    /// <summary>
    /// Fade text item rồi ẩn
    /// </summary>
    IEnumerator FadeOutItemText()
    {
        textMeshProUGUIItem.CrossFadeAlpha(0, fadeTime, false);
        yield return new WaitForSeconds(fadeTime);

        textMeshProUGUIItem.gameObject.SetActive(false);
        fadeItemCoroutine = null;
    }

    /// <summary>
    /// Cập nhật UI tổng điểm
    /// </summary>
    private void UpdateScoreUI()
    {
        if (textMeshProTotalScore != null)
        {
            textMeshProTotalScore.text = score.ToString();
        }
        ScoreRoundUI();
    }

    private void ScoreRoundUI()
    {
        if (textMeshProUGUIScoreRound != null)
        {
            textMeshProUGUIScoreRound.text = textMeshProTotalScore.text;
        }
    }

    public int GetScore() => score;
}
