using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    public InputActionAsset actions;

    //public GameObject cursorPrefab;
    private GameObject cursorVisual;

    private InputAction moveAction;
    private InputAction placeAction;
    private InputAction rotateAction;
    private InputAction claimAction;

    public bool claimingStage = false;

    private GameObject cursor;
    private Vector3Int selectedPos;
    private Vector3Int lastPlacedPos;

    void Awake()
    {
        moveAction = actions.FindActionMap("Gameplay").FindAction("Move");
        placeAction = actions.FindActionMap("Gameplay").FindAction("Place");
        rotateAction = actions.FindActionMap("Gameplay").FindAction("Rotate");
        claimAction = actions.FindActionMap("Gameplay").FindAction("Claim");


        cursor = transform.Find("Cursor").gameObject;

        selectedPos = new Vector3Int(0, 0, 0);
        lastPlacedPos = new Vector3Int(0, 0, 0);
    }

    void Start()
    {
        cursorVisual = GameManager.Instance.getPreviewTileObject(selectedPos);
    }

    public void refreshPreviewTile()
    {
        Destroy(cursorVisual);
        cursorVisual = GameManager.Instance.getPreviewTileObject(selectedPos);
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
        if(placeAction.triggered && !claimingStage)
        {
            if(GameManager.Instance.placeTile(selectedPos))
            {
                claimingStage = true;
                GameManager.Instance.errorText = "";
                lastPlacedPos = new Vector3Int(selectedPos.x, 0, selectedPos.z);
            }

            refreshPreviewTile();
            
        }

        if(claimingStage)
        {
            if (claimAction.triggered)
            {
                
                int edge = (int)claimAction.ReadValue<float>();
                //Debug.Log(edge);
                if (edge > 0 && edge < 6)
                {
                    int success = GameManager.Instance.claim(edge, lastPlacedPos);
                    if(success == 0)
                    {
                        claimingStage = false;
                        GameManager.Instance.advanceTurn();
                        GameManager.Instance.errorText = "";
                    } else if (success == 1)
                    {
                        GameManager.Instance.errorText = "You can not claim fields!";
                    } else if (success == 2)
                    {
                        GameManager.Instance.errorText = "This feature is already claimed!";
                    } else
                    {
                        Debug.Log(success);
                        GameManager.Instance.errorText = "Claiming failed. Try again!";
                    }
                }
                else if (edge == 6)
                {
                    claimingStage = false;
                    GameManager.Instance.advanceTurn();
                }
                

                
            }
        }

        // Rotate
        if (rotateAction.triggered)
        {
            GameManager.Instance.rotateActiveTile();
            Destroy(cursorVisual);
            cursorVisual = GameManager.Instance.getPreviewTileObject(selectedPos);
        }

        //Debug.Log($"{selectedPos.x}, {selectedPos.z}");
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
