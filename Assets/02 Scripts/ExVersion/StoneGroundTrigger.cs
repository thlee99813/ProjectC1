using UnityEngine;

public class StoneGroundTrigger : MonoBehaviour
{
    public GameObject Boss;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
        {
            GameManager.Instance.SetPhase(GameManager.Phase.Boss);

            Boss.SetActive(true);
            GameManager.Instance.StoneGroundExit.SetActive(true);
            SoundManager.Instance.BgmStop();
            SoundManager.Instance.PlayBGM(Sound.Boss_Bgm);

            this.gameObject.SetActive(false);
        }
    }
}
