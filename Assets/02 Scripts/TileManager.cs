using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileManager : MonoBehaviour
{
    public GameObject TargetTile;
    private Tile tile;
    public GameObject Laser;

    void Start()
    {
        //tile = TargetTile.GetComponent<Tile>();
        //tile.MoveDown();
       
    }
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("11");
            Laser.GetComponent<Laser>().RefreshLaser();
            Laser.SetActive(true);        
        }
    }

}
