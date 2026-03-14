using UnityEditor.Rendering;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject TargetTile;
    private Tile tile;
    void Start()
    {
        tile = TargetTile.GetComponent<Tile>();
        tile.MoveDown();

    }


}
