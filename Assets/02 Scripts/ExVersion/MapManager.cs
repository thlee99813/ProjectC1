using UnityEngine;
using Unity.AI.Navigation;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject wallsParent;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject obstacleParent;
    [SerializeField] private NavMeshSurface navMeshSurface; // 배치 완료 후 Bake할 대상

    [Header("Layer Settings")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Spawn Constraints")]
    [SerializeField] private int obstacleCount = 20;
    [SerializeField] private int maxRetryPerObject = 15; // 빈 공간을 찾기 위한 최대 재시도 횟수

    [Header("Map Size")]
    [SerializeField] private int floorSizeX = 20;
    [SerializeField] private int floorSizeZ = 20;
    [SerializeField] private int wallHeight = 10;
    [SerializeField] private int wallThickness = 2;

    [Header("Diversity Settings (Clamp)")]
    // 길이
    [SerializeField, Range(0.5f, 10.0f)] private float minScaleX = 1.0f;
    [SerializeField, Range(0.5f, 10.0f)] private float maxScaleX = 5.0f;

    // 높이
    [SerializeField, Range(0.5f, 5.0f)] private float minHeight = 1.0f;
    [SerializeField, Range(0.5f, 5.0f)] private float maxHeight = 3.0f;

    // 폭
    [SerializeField, Range(0.5f, 5.0f)] private float minScaleZ = 0.8f;
    [SerializeField, Range(0.5f, 5.0f)] private float maxScaleZ = 1.2f;

    // 회전 각도
    [SerializeField, Range(0f, 360f)] private float minRotationY = 0f;
    [SerializeField, Range(0f, 360f)] private float maxRotationY = 360f;

    [Header("Exclusion Zone (Center 0,0)")]
    [SerializeField, Range(0f, 10f)] private float exclusionRange = 3.0f;

    [Header("Spacing Settings")]
    [SerializeField, Range(0f, 5.0f)] private float obstacleSpacing = 1.5f;

    private MeshRenderer floorMR;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        floorMR = floor.GetComponent<MeshRenderer>();
        InitializeMap();
    }

    // 맵 생성
    public void InitializeMap()
    {
        ClearExistingObstacles();
        ResizeMap();
        GenerateObstacles();

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    private void ResizeMap()
    {
        floor.transform.localScale = new Vector3(floorSizeX, 1, floorSizeZ);
        wallsParent.transform.localScale = new Vector3(1, wallHeight, 1);
        GenerateWalls();
        BakeNavigation();
    }

    private void GenerateWalls()
    {
        float halfX = floorSizeX * 5f;
        float halfZ = floorSizeZ * 5f;

        float wallLengthX = floorSizeX * 10f;
        float wallLengthZ = floorSizeZ * 10f + (wallThickness * 2f);

        float halfWallH = wallHeight * 0.5f;
        float halfWallT = wallThickness * 0.5f;

        GameObject wallNorth = Instantiate(
            wallPrefab,
            new Vector3(0, halfWallH, halfZ + halfWallT),
            Quaternion.identity,
            wallsParent.transform
            );
        wallNorth.name = "Wall North";
        wallNorth.transform.localScale = new Vector3(wallLengthX, 1, wallThickness);

        GameObject wallSouth = Instantiate(
            wallPrefab,
            new Vector3(0, halfWallH, -halfZ + -halfWallT),
            Quaternion.identity,
            wallsParent.transform
            );
        wallSouth.name = "Wall South";
        wallSouth.transform.localScale = new Vector3(wallLengthX, 1, wallThickness);

        GameObject wallEast = Instantiate(
            wallPrefab,
            new Vector3(halfX + halfWallT, halfWallH, 0),
            Quaternion.identity,
            wallsParent.transform
            );
        wallEast.name = "Wall East";
        wallEast.transform.localScale = new Vector3(wallThickness, 1, wallLengthZ);

        GameObject wallWest = Instantiate(
            wallPrefab,
            new Vector3(-halfX + -halfWallT, halfWallH, 0),
            Quaternion.identity,
            wallsParent.transform
            );
        wallWest.name = "Wall West";
        wallWest.transform.localScale = new Vector3(wallThickness, 1, wallLengthZ);
    }

    private void BakeNavigation()
    {
        if (navMeshSurface != null)
        {
            // 기존 데이터를 지우고 새로 생성
            navMeshSurface.BuildNavMesh();
        }
    }

    private void GenerateObstacles()
    {
        Bounds bounds = floorMR.bounds;
        float surfaceY = bounds.max.y;

        for (int i = 0; i < obstacleCount; i++)
        {
            for (int attempt = 0; attempt < maxRetryPerObject; attempt++)
            {
                if (GenerateObstacle()) break;
            }
        }
    }

    // 위치, 소환할 프리팹, 각도
    // 소환 성공시 true, 실패시 false를 return
    public bool GenerateObstacle(Vector3? location = null, GameObject summonPrefab = null, Vector3? angle = null, bool isBossObstacle = false)
    {
        GameObject prefabToSpawn = summonPrefab ?? obstaclePrefab;

        Vector3 finalScale = new Vector3(
            Random.Range(minScaleX, maxScaleX),
            Random.Range(minHeight, maxHeight),
            Random.Range(minScaleZ, maxScaleZ)
        );

        float rotationY = angle.HasValue ? angle.Value.y : Random.Range(minRotationY, maxRotationY);

        Vector3 spawnPos;
        if (location.HasValue)
        {
            spawnPos = location.Value;
        }
        else
        {
            // 랜덤 위치 계산 시도
            float safetyRadius = CalculateSafetyRadius(finalScale.x, finalScale.z);
            Vector3? foundPos = GetRandomPosition(safetyRadius);

            if (!foundPos.HasValue) return false; // 예외존

            spawnPos = foundPos.Value;
            // Y축 높이 보정 
            spawnPos.y += finalScale.y / 2f;
        }

        // 물리 충돌 범위 체크
        if (CanPlaceObstacle(spawnPos, finalScale, rotationY))
        {
            SpawnObstacle(spawnPos, rotationY, finalScale, prefabToSpawn);
            Physics.SyncTransforms();
            return true;
        }

        return false;
    }

    private Vector3? GetRandomPosition(float safetyRadius)
    {
        Bounds bounds = floorMR.bounds;
        float surfaceY = bounds.max.y;

        Vector3 randomPos = new Vector3(
            Random.Range(bounds.min.x + safetyRadius, bounds.max.x - safetyRadius),
            surfaceY,
            Random.Range(bounds.min.z + safetyRadius, bounds.max.z - safetyRadius)
        );

        // 제외 구역 체크
        if (Mathf.Abs(randomPos.x) < exclusionRange && Mathf.Abs(randomPos.z) < exclusionRange)
        {
            return null;
        }

        return randomPos;
    }

    private float CalculateSafetyRadius(float x, float z)
    {
        float combinedX = x + obstacleSpacing;
        float combinedZ = z + obstacleSpacing;
        return Mathf.Sqrt((combinedX * combinedX) + (combinedZ * combinedZ)) / 2f;
    }

    private bool CanPlaceObstacle(Vector3 pos, Vector3 scale, float rotY)
    {
        Vector3 checkSize = new Vector3(
            scale.x + obstacleSpacing,
            scale.y,
            scale.z + obstacleSpacing
        ) / 2f;

        return !Physics.CheckBox(pos, checkSize, Quaternion.Euler(0, rotY, 0), wallLayer | obstacleLayer);
    }

    private void SpawnObstacle(Vector3 pos, float rotY, Vector3 scale, GameObject prefab = null)
    {
        // prefab이 전달되지 않으면(null) 멤버 변수인 obstaclePrefab을 사용
        GameObject prefabToSpawn = prefab != null ? prefab : obstaclePrefab;

        GameObject obj = Instantiate(prefabToSpawn, pos, Quaternion.Euler(0, rotY, 0));
        obj.transform.localScale = scale;
        obj.transform.SetParent(obstacleParent.transform);
        obj.layer = GetLayerFromMask(obstacleLayer);
    }

    private int GetLayerFromMask(LayerMask mask)
    {
        int maskValue = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((maskValue & (1 << i)) != 0) return i;
        }
        return 0;
    }

    private void ClearExistingObstacles()
    {
        // 기존에 생성된 장애물 제거
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == (int)Mathf.Log(obstacleLayer.value, 2))
            {
                Destroy(child.gameObject);
            }
        }
    }
}