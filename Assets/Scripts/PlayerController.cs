using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    public InputActionAsset actions;

    public GameObject cursorPrefab;
    private GameObject cursorVisual;

    private InputAction moveAction;
    private InputAction placeAction;


    private GameObject cursor;
    private Vector3Int selectedPos;

    void Awake()
    {
        moveAction = actions.FindActionMap("Gameplay").FindAction("Move");
        placeAction = actions.FindActionMap("Gameplay").FindAction("Place");


        cursor = transform.Find("Cursor").gameObject;

        selectedPos = new Vector3Int(0, 0, 0);
    }

    void Start()
    {
        cursorVisual = Instantiate(cursorPrefab, new Vector3(0, 0, 0), Quaternion.identity * cursorPrefab.transform.localRotation);
    }

    // Update is called once per frame
    void Update()
    {
        // Move
        Vector2 moveInputVector = moveAction.ReadValue<Vector2>();
        Vector3 moveVector = new Vector3(moveInputVector.x, 0, moveInputVector.y) * moveSpeed;

        transform.Translate(moveVector * Time.deltaTime);

        float cX = cursor.transform.position.x;
        float cZ = cursor.transform.position.z;

        int gridCX = (int)Mathf.RoundToInt(cX / Settings.gridSize);
        int gridCZ = (int)Mathf.RoundToInt(cZ / Settings.gridSize);

        Vector3 gridPos = new Vector3(gridCX, 0.25f, gridCZ);
        cursorVisual.transform.localPosition = gridPos;

        selectedPos.x = (int) gridCX;
        selectedPos.z = (int) gridCZ;

        // Place
        if(placeAction.triggered)
        {
            Debug.Log("Place!");
            GameManager.Instance.testPlace();
        }
    }

    public Vector3Int getCursorPos()
    {
        return selectedPos;
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
