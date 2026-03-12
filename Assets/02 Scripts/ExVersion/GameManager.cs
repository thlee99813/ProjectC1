using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public enum Phase
    {
        Ready,
        Phase1,
        Phase2,
        Phase3,
        Boss,
        Death,
        End
    }

    public static GameManager Instance { get; private set; }

    public float RemainingTime;

    public bool IsTimerRunning { get; private set; }

    public GameObject StoneGroundTrigger;
    public GameObject StoneGroundExit;

    public Phase currentPhase = Phase.Ready;
    public Phase CurrentPhase => currentPhase;

    [SerializeField] private string sceneName = "LobbyScene";


    [Header("페이즈 시간")]

    public float ReadyPhaseTime = 3f;

    public float TotalTime = 300f;

    public float phase1time = 100f;
    public float phase2time = 100f;
    public float phase3time = 100f;

    [Header("소환 풀 (부모)")]

    [SerializeField] public Transform EnemyPool;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Cursor.visible = false;
        ResetTimer();
        IsTimerRunning = false;
        StartCoroutine(StartPhaseAfterDelay(ReadyPhaseTime));
    }

    private void Update()
    {
        if (currentPhase == Phase.Death)
        {
            EnemyPool.gameObject.SetActive(false);
            PlayerMove.instance.isMoveable = false;
            UIManager.Instance.GameOver();
            Cursor.visible = true;
            return;
        }
        if (currentPhase == Phase.End)
        {
            EnemyPool.gameObject.SetActive(false);
            PlayerMove.instance.isMoveable = false;
            UIManager.Instance.GameOverBackground.SetActive(true);
            UIManager.Instance.GameClearTextImage.SetActive(true);
            Cursor.visible = true;

            return;
        }


        if (!IsTimerRunning)
        {
            return;
        }

        RemainingTime -= Time.deltaTime;
        if (RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            SoundManager.Instance.PlaySfx(Sound.Game_Clear, 0.5f);
            SetPhase(Phase.End);
            IsTimerRunning = false;
            return;
        }
        if (RemainingTime <= (TotalTime - phase1time - phase2time) && currentPhase == Phase.Phase2)
        {
            NextPhase();
        }
        else if (RemainingTime <= (TotalTime - phase1time) && currentPhase == Phase.Phase1)
        {
            NextPhase();
        }
    }
    public void StartPhase()
    {
        SetPhase(Phase.Phase1);
        SoundManager.Instance.PlayBGM(Sound.InGame_Bgm, true);
        ResumeTimer();

    }

    public void SetPhase(Phase phase)
    {
        currentPhase = phase;
        UIManager.Instance.OnPhaseChanged(currentPhase);
        Debug.Log($"Phase Setted {phase}");
    }

    public void NextPhase()
    {
        if (currentPhase == Phase.Death)
        {
            return;
        }
        if (currentPhase == Phase.Ready)
        {
            StartPhase();
        }
        else if (currentPhase == Phase.Phase1)
        {
            SetPhase(Phase.Phase2);
        }
        else if (currentPhase == Phase.Phase2)
        {
            SetPhase(Phase.Phase3);
        }
    }


    public void PauseTimer()
    {
        IsTimerRunning = false;
    }

    public void ResumeTimer()
    {
        if (RemainingTime > 0f)
        {
            IsTimerRunning = true;
        }
    }

    public void ResetTimer()
    {
        RemainingTime = TotalTime;
    }

    private IEnumerator StartPhaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextPhase();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GotoLobby()
    {
        SceneManager.LoadScene(sceneName);

    }
}
