using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float movespeed = 6f;
    [SerializeField] private float mouseSensitivity = 100f;

    private Rigidbody rb;
    private Vector3 inputMove;
    private Vector3 targetPos;
    private bool isMoving;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        GetInput();
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 dir = hitPoint - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {

                    targetPos = ray.GetPoint(distance);
                    targetPos.y = rb.position.y;
                    isMoving = true;
            }
        }
        

    }


    void FixedUpdate()
    {
        Vector3 pos = rb.position + inputMove * movespeed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
        if (isMoving)
        {
            Vector3 dir = targetPos - rb.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.05f)
            {
                isMoving = false;
                return;
            }

            dir = dir.normalized;

            Vector3 nextPos = rb.position + dir * movespeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);
        }
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
