using System.Collections;
using System.Collections.Generic;
using TouchControlsKit;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject spawnCircle;

    private Player player;
    private CharacterController characterController;
    private LineRenderer spawnCircleLine;

    private float spawnCircleWidth;

    public GameObject dischargeButton;
    public GameObject joystickControl;

    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.DownArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode dischargeKey = KeyCode.Space;

    void Awake()
    {
        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();

        //LineRenderer on another object because of the shockwave debug circle
        spawnCircleLine = spawnCircle.GetComponent<LineRenderer>();
        spawnCircleWidth = gameObject.GetComponent<Renderer>().bounds.size.x;

        //Utilities.DrawCircle(spawnCircle, spawnCircleWidth, 0.2f, false);
        spawnCircleLine.enabled = false;

#if UNITY_EDITOR_WIN
        dischargeButton.SetActive(false);
        joystickControl.SetActive(false);
#endif
    }

    void Update()
    {
#if UNITY_ANDROID
        if (TCKInput.GetAction("ExplodeButton", EActionEvent.Up)) player.shockWave();
#elif UNITY_EDITOR_WIN
        if (Input.GetKeyDown(dischargeKey)) player.shockWave();
#endif

        //TODO: Draw spawn circle, circle has performance issues!?        
        if (!characterController.isGrounded)
        {
            //Only ray to the boundary layer
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Boundary"))))
            {
                Debug.Log("hit");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                //Utilities.DrawCircle(spawnCircle, spawnCircleWidth, 0.2f, false);
                //spawnCircle.transform.position = hit.point;
                //spawnCircleLine.enabled = true;
            }
        }
    }

    void FixedUpdate()
    {
        //Android joystick or Windows arrow controls
        Vector2 move = Vector2.zero;
#if UNITY_ANDROID
        move = TCKInput.GetAxis("Joystick");
#elif UNITY_EDITOR_WIN
        if (Input.GetKey(upKey)) move = Vector2.up;
        else if (Input.GetKey(downKey)) move = Vector2.down;
        if (Input.GetKey(leftKey)) move += Vector2.left;
        else if (Input.GetKey(rightKey)) move += Vector2.right;
#endif
        //Player movement in the air while spawning
        if (!characterController.isGrounded)
        {
            move.x *= 4;
            move.y *= 4;
        }

        movePlayer(move.x, move.y);
    }
    
    private void movePlayer(float x, float y)
    {
        bool grounded = characterController.isGrounded;
        Vector3 moveDirection = player.transform.forward * y;
        moveDirection += player.transform.right * x;
        moveDirection.y = -10f;
        if (grounded) moveDirection *= 7f;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
