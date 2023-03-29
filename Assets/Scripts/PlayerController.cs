using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public InputActionAsset actions;
    public GameObject cursorVisual;

    private InputAction moveAction;
    private GameObject cursor;
    private Vector3 selectedPos;

    void Awake()
    {
        moveAction = actions.FindActionMap("Gameplay").FindAction("Move");
        cursor = transform.Find("Cursor").gameObject;

        selectedPos = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInputVector = moveAction.ReadValue<Vector2>();
        Vector3 moveVector = new Vector3(moveInputVector.x, 0, moveInputVector.y) * moveSpeed;

        transform.Translate(moveVector * Time.deltaTime);

        float cX = cursor.transform.position.x;
        float cZ = cursor.transform.position.z;

        int gridCX = (int)Mathf.RoundToInt(cX / Settings.gridSize);
        int gridCZ = (int)Mathf.RoundToInt(cZ / Settings.gridSize);

        Vector3 gridPos = new Vector3(gridCX, 0, gridCZ);
        cursorVisual.transform.localPosition = gridPos;

        selectedPos.x = gridCX;
        selectedPos.z = gridCZ;
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
