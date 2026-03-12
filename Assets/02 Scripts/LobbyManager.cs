using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{

    [SerializeField] private string sceneName = "TH_Scene";

    [SerializeField] private GameObject startHoverImage;
    [SerializeField] private GameObject quitHoverImage;

    public void Start()
    {
        startHoverImage.SetActive(false);
        quitHoverImage.SetActive(false);


    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnStartButtonEnter()
    {
        startHoverImage.SetActive(true);
    }
    public void OnQuitButtonEnter()
    {
        quitHoverImage.SetActive(true);
    }
    public void OnStartButtonExit()
    {
        startHoverImage.SetActive(false);
    }
    public void OnQuitButtonExit()
    {
        quitHoverImage.SetActive(false);
    }      



 

}
