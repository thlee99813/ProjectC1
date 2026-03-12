using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;


public class DollyManager : MonoBehaviour
{

    
    public static DollyManager Instance { get; private set; }
    public GameObject Dollyplayer;
    [SerializeField] private string sceneName = "Main";

    [Header("Camera Settings")]
    public CinemachineCamera Vcam1;
    public CinemachineCamera Vcam2;
    public bool middleflag = false;

    [Header("Scene Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float delay = 1f;
    [SerializeField] private float fadeDuration = 1f;

    private bool isLoading = false;



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
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.blocksRaycasts = false;
    }


    public void MainSceneLoad()
    {
        if (isLoading) return;
        StartCoroutine(LoadMain());
    }

    
    private IEnumerator LoadMain()
    {
        isLoading = true;


        yield return StartCoroutine(FadeOut(fadeCanvasGroup, fadeDuration));


        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOut(CanvasGroup group, float duration)
        {
            float elapsed = 0f;
            float startAlpha = group.alpha;
            float endAlpha = 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                yield return null;
            }

            group.alpha = endAlpha;
        }
   
}
