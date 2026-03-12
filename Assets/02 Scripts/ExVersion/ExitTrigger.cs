using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public GameObject Boss;

    void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                {
                    GameManager.Instance.SetPhase(GameManager.Phase.End);
                    SoundManager.Instance.PlaySfx(Sound.Game_Clear, 0.5f);

                    Boss.SetActive(false);
                    this.gameObject.SetActive(false);
                    SoundManager.Instance.BgmStop();

                }    
            }
}
