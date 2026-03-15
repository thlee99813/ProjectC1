using UnityEngine;

public class NextStageTrigger : MonoBehaviour
{
    //[SerializeField] private Transform currentStageWalls;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StageManager.Instance.NextStage();

            CameraManager.Instance.ChangeStageCamera(StageManager.Instance.currentStage);


            //Tile[] tiles = currentStageWalls.GetComponentsInChildren<Tile>(true);
            /*
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].MoveDown(8f);
            }
            */

            // 부딪히면 다음스테이지로 넘기고
            // 부딪히면 다음 카메라로 바꾸고 두개만 하면됨.

            

            this.gameObject.SetActive(false);
        }
    }
}
