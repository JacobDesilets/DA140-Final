using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public InputActionAsset actions;

    private InputAction moveAction;

    void Awake()
    {
        moveAction = actions.FindActionMap("Gameplay").FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInputVector = moveAction.ReadValue<Vector2>();
        Vector3 moveVector = new Vector3(moveInputVector.x, 0, moveInputVector.y) * moveSpeed;

        transform.Translate(moveVector * Time.deltaTime);
    }

    void OnEnable()
    {
        actions.FindActionMap("gameplay").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("gameplay").Disable();
    }
}
