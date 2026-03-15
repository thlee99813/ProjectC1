using UnityEngine;

public enum ObjectType
    {
        Door,
        Destroy,
        BlockDown,
        BlockUp
    }

public class ObjectCube : MonoBehaviour
{    
    
    

    [SerializeField] private float rotateSpeed = 75f;
    [SerializeField] private GameObject crystal;
    
    [SerializeField] private GameObject rootObject;

    [SerializeField] private GameObject doorObject;

    [SerializeField] private Tile moveTile;

    [SerializeField] private ObjectType cubeType;



    private bool istriggered = false;

    

    void Update()
    {
        crystal.transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    
    
    public void LaserTrigger()
    {
        switch (cubeType)
        {
            case ObjectType.Door:
                
                CameraManager.Instance.PlayImpulseBurst(7);
                doorObject.GetComponent<Tile>().MoveDown(1f);


                break;

            case ObjectType.Destroy:
                rootObject.SetActive(false);
                break;

            case ObjectType.BlockDown:
                if(moveTile.moveRoutine != null)
                {
                    return;
                }

                if(!istriggered)
                {
                    istriggered = true;
                    moveTile.MoveDown();
                }
                else
                {
                    istriggered = false;
                    moveTile.MoveUp();                

                } 

                break;
            case ObjectType.BlockUp:
                if(moveTile.moveRoutine != null)
                {
                    return;
                }
                if(!istriggered)
                {
                    istriggered = true;
                    moveTile.MoveUp();
                }
                else
                {
                    istriggered = false;
                    moveTile.MoveDown();
                
                } 

                break;

            default : 

                break;
        }
    }
    public void ResetCube()
        {
            rootObject.SetActive(true);
        }

}
