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

    public Tile previewTile;

    private int playerCount;
    private int turn = 1;
    private int[] scores;

    private Board board;
    private TileDeck td;

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

        pc = player.GetComponent<PlayerController>();
        board = new Board();

        testTile = new Tile(EdgeType.Field, EdgeType.Field, EdgeType.Field, EdgeType.Field, testTileObject);

        Tile[] tileArray = { testTile };

        td = new TileDeck(tileArray);

        activeTile = td.currentTile;
        previewTile = activeTile;
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

    public GameObject getPreviewTileObject(Vector3 pos)
    {
        return Instantiate(previewTile.prefab, pos, previewTile.getRotation());
    }

    public void rotateActiveTile()
    {
        Debug.Log("Rotate!");
        activeTile.rotate();
    }

    public void score(int points)
    {
        scores[turn - 1] += points;
    }

    public void placeTile()
    {
        if (board.getTile(cursorPos) != null) { Debug.Log("Already occupied!"); return; }
        Debug.Log("Place!");
        board.place(cursorPos, activeTile);
        Instantiate(activeTile.prefab, cursorPos, activeTile.getRotation());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cursorPos = pc.getCursorPos();
    }

}
