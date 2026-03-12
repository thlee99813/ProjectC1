using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI Timer_Text;
    public TextMeshProUGUI HP_Text;

    public GameObject GameOverBackground;
    public GameObject GameOverTextImage;
    public GameObject GameClearTextImage;


    [Header("Phase Color")]
    [SerializeField] private Color phase1Color;
    [SerializeField] private Color phase2Color;
    [SerializeField] private Color phase3Color;

    [SerializeField] private GameObject restartHoverImage;

    [SerializeField] private GameObject quitHoverImage;

    [Header("Damage Feedback")]
    [SerializeField] private CanvasGroup damagePanelGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float maxAlpha = 0.4f;

    private Coroutine damageEffectCoroutine;

    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        GameOverBackground.SetActive(false);
        damagePanelGroup.alpha = 0;

        UpdateTimerText();
    }

    void Update()
    {
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        Timer_Text.text = TimerToText(GameManager.Instance.RemainingTime);
    }

    // 색 변환 함수 
    private void SetTextColor(TMP_Text targetText, Color color)
    {
        if (targetText == null) return;

        targetText.enableVertexGradient = false;
        targetText.color = color;
    }

    public void OnPhaseChanged(GameManager.Phase phase)
    {
        switch (phase)
        {
            case GameManager.Phase.Phase1:
                SetTextColor(Timer_Text, phase1Color);
                break;
            case GameManager.Phase.Phase2:
                SetTextColor(Timer_Text, phase2Color);
                break;
            case GameManager.Phase.Phase3:
                SetTextColor(Timer_Text, phase3Color);
                break;
            case GameManager.Phase.Boss:
                SetTextColor(Timer_Text, phase3Color);
                break;
            default:
                SetTextColor(Timer_Text, phase1Color);
                break;
        }

        Debug.Log($"UI Color Changed {phase}");
    }

    public void TriggerDamageFlash()
    {
        if (damagePanelGroup == null) return;

        if (damageEffectCoroutine != null)
        {
            StopCoroutine(damageEffectCoroutine);
        }

        damageEffectCoroutine = StartCoroutine(DamageFadeRoutine());
    }

    private IEnumerator DamageFadeRoutine()
    {
        float elapsed = 0f;
        damagePanelGroup.alpha = maxAlpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            damagePanelGroup.alpha = Mathf.Lerp(maxAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        damagePanelGroup.alpha = 0f;
    }

    private string TimerToText(float remainingTime)
    {
        int totalSeconds = Mathf.CeilToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes} : {seconds:00}";
    }
    public void GameOver()
    {
        SoundManager.Instance.PlaySfx(Sound.Game_Over, 0.05f);
        SoundManager.Instance.BgmStop();
        GameOverBackground.SetActive(true);
        GameOverTextImage.SetActive(true);
    }
    public void UpdateLifeText(int health)
    {
        HP_Text.text = $"Life : {health}";
    }
    public void OnStartButtonEnter()
    {
        restartHoverImage.SetActive(true);
    }
    public void OnQuitButtonEnter()
    {
        quitHoverImage.SetActive(true);
    }
    public void OnStartButtonExit()
    {
        restartHoverImage.SetActive(false);
    }
    public void OnQuitButtonExit()
    {
        quitHoverImage.SetActive(false);
    }
}
