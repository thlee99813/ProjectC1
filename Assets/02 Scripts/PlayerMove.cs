using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float movespeed = 6f;

    private Rigidbody rb;
    private Vector3 inputMove;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();
    }


    void FixedUpdate()
    {
        Vector3 pos = rb.position + inputMove * movespeed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
    }

    private void GetInput()
    {        
        Vector2 move = Vector2.zero;
        if(Keyboard.current.aKey.isPressed) move.x -= 1f;
        if(Keyboard.current.dKey.isPressed) move.x += 1f;
        if(Keyboard.current.sKey.isPressed) move.y -= 1f;
        if(Keyboard.current.wKey.isPressed) move.y += 1f;

        move = move.normalized;
        inputMove = new Vector3(move.x, 0f, move.y);
    }
}
