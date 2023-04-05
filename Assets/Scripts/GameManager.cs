using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public GameObject player;
    private PlayerController pc;

    public GameObject noTileObject;

    public static GameManager Instance { get; private set; }
    public Tile activeTile;

    public Tile previewTile;

    private int playerCount;
    private int turn = 1;
    private int[] scores;

    private Board board;
    private TileDeck td;


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

        td = new TileDeck();

        activeTile = td.currentTile;
        previewTile = activeTile;

    }

    public void setPlayers(int count)
    {
        playerCount = count;
        scores = new int[count];
    }

    public string getCurrentPlayerText()
    {
        return ($"Player: {turn}");
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

    public void placeTile(Vector3Int pos)
    {
        if(td.finished) { return; }
        if(!board.isValidPlaceLocation(pos, activeTile)) { return; }
        Tile newTile = activeTile.copy();
        Debug.Log("Place!");
        board.place(pos, newTile);
        Instantiate(newTile.prefab, pos, activeTile.getRotation());

        td.draw();
        if (!td.finished)
        {
            activeTile = td.currentTile;
            previewTile = activeTile;

            
        } else { 
            Debug.Log("Finished");
            previewTile = new Tile(EdgeType.Field, EdgeType.Field, EdgeType.Field, EdgeType.Field, noTileObject, false);
        }

        pc.refreshPreviewTile();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
