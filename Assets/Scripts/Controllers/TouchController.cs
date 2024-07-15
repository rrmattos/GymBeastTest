using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private InputActionMap touchController;
    
    [SerializeField] private float moveSpeed = 5f;
    
    public AnimationStates MoveState { get; private set; } = AnimationStates.IDLE;
    public Quaternion VisualRotation { get; private set; }
    private Vector2 touchStartPosition;
    private Vector2 touchCurrentPosition;
    private Vector2 moveDirection;
    private bool isTouching;
    public bool isMoveBlocked { get; private set; } = false;
    public void IsMoveBlocked(bool _value) => isMoveBlocked = _value;

    private TouchController(){}
    
    private void Awake()
    {
        if(playerInput == null) playerInput = GetComponent<PlayerInput>();
        touchController = playerInput.actions.FindActionMap("TouchController");
    }
    
    private void Update()
    {
        if (isMoveBlocked) return;
        
        if (isTouching)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // --- Faz a normalização da posição X e Y ---
            Vector2 normalizedStartPosition = new Vector2(
                (touchStartPosition.x / screenWidth) * 2 - 1,
                (touchStartPosition.y / screenHeight) * 2 - 1
            );

            Vector2 normalizedCurrentPosition = new Vector2(
                (touchCurrentPosition.x / screenWidth) * 2 - 1,
                (touchCurrentPosition.y / screenHeight) * 2 - 1
            );

            // --- Calcula a diferença etre o toque inicial e o atual ---
            moveDirection = (normalizedCurrentPosition - normalizedStartPosition).normalized;
            Vector3 move = new Vector3(moveDirection.x, 0, moveDirection.y);
            
            // --- Calcula a rotação do modelo3D do player ---
            float angle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            VisualRotation = Quaternion.Euler(0, angle, 0);
            
            transform.Translate(move * (moveSpeed * Time.deltaTime), Space.Self);
        }
    }

    private void OnTouchStart()
    {
        Debug.Log("TouchStart");
        isTouching = true;
    }
    
    private void OnTouchStartMove()
    {
        touchStartPosition = touchController.FindAction("TouchStartMove").ReadValue<Vector2>();
    }

    private void OnTouchMove()
    {
        if (isMoveBlocked) return;

        touchCurrentPosition = touchController.FindAction("TouchMove").ReadValue<Vector2>();
        if(moveDirection != Vector2.zero && MoveState != AnimationStates.RUN)
            MoveState = AnimationStates.RUN;
    }

    private void OnTouchEnd()
    {
        Debug.Log("TouchEnd");
        isTouching = false;
        MoveState = AnimationStates.IDLE;
    }
}