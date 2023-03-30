using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public GameObject player;
    private PlayerController pc;
    private Vector3Int cursorPos;

    public static GameManager Instance { get; private set; }
    public GameObject testTileObject;
    public Tile activeTile;

    private int playerCount;
    private int turn = 1;
    private int[] scores;

    private Board board;

    private Tile testTile;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void setPlayers(int count)
    {
        playerCount = count;
        scores = new int[count];
    }

    public void advanceTurn()
    {
        turn++;
        if(turn > playerCount) { turn = 1; }
    }

    public void score(int points)
    {
        scores[turn - 1] += points;
    }

    public void testPlace()
    {
        if(board.getTile(cursorPos) != null) { Debug.Log("Already occupied!"); return; }
        board.place(cursorPos, testTile);
        Instantiate(testTileObject, cursorPos, Quaternion.identity * testTileObject.transform.localRotation);
    }

    // Start is called before the first frame update
    void Start()
    {
        pc = player.GetComponent<PlayerController>();
        board = new Board();
        testTile = new Tile(EdgeType.Field, EdgeType.Field, EdgeType.Field, EdgeType.Field, testTileObject);
        activeTile = testTile;
    }

    // Update is called once per frame
    void Update()
    {
        cursorPos = pc.getCursorPos();
    }

}
