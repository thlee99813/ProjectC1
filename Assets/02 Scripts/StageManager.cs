using UnityEngine;
using UnityEngine.InputSystem;


public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    

    public int currentStage = 1;

    public Transform[] respawnPoint;

    public Transform currentRespawnPoint;
    
    [SerializeField] private Transform[] stageRoots;

    [SerializeField] private Transform currentStageRoot;


    void Awake()
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
        currentRespawnPoint = respawnPoint[currentStage - 1];
        currentStageRoot = stageRoots[currentStage - 1];

    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetStage();
        }
   
    }

    public void NextStage()
    {
        currentStage++;
        currentRespawnPoint = respawnPoint[currentStage - 1];
        currentStageRoot = stageRoots[currentStage - 1];
        CameraManager.Instance.ChangeStageCamera(currentStage);
    }

    public void ResetStage()
    {

        Tile[] tiles = currentStageRoot.GetComponentsInChildren<Tile>(true);
        ObjectCube[] objectCubes = currentStageRoot.GetComponentsInChildren<ObjectCube>(true);

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].ResetTile();
        }
        for (int i = 0; i < objectCubes.Length; i++)
        {
            objectCubes[i].ResetCube();
        }
        Player.Instance.transform.position = currentRespawnPoint.position;
        Player.Instance.playerMove.rb.linearVelocity = Vector3.zero;
        Player.Instance.playerMove.rb.angularVelocity = Vector3.zero;

    }

    
}
