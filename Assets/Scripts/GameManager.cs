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

    private bool isValidPlaceLocation(Vector3Int pos)
    {
        // Case 0: location already occupied
        if (board.getTile(pos) != null) { Debug.Log("Already occupied!"); return false; }

        Tile[] neighbors = board.getNeighbors(pos);

        // Case 1: Location has no neighbors
        bool hasNeighbors = false;
        foreach(Tile n in neighbors)
        {
            if(n != null) { hasNeighbors = true; }
        }

        if(!hasNeighbors) {
            Debug.Log("Invalid place location! Must be adjacent to at least one tile");
            return false; 
        }

        // Case 2: Tiles can't connect

        for (int i = 0; i < neighbors.Length; i++)
        {
            if(neighbors[i] != null)
            {
                if(!neighbors[i].getRelevantEdge(i).match(activeTile.getRelevantEdgeActive(i)))
                {
                    //Debug.Log(i);
                    //Debug.Log($"Top: {activeTile.getTop()} Right: {activeTile.getRight()} Bottom: {activeTile.getBottom()} Left: {activeTile.getLeft()}");
                    Debug.Log($"Invalid place location! Doesn't match neighboring tile -> {neighbors[i].getRelevantEdge(i)} : {activeTile.getRelevantEdgeActive(i)}");
                    return false;
                } else
                {
                    Debug.Log($"Valid place location! Matches neighboring tile -> {neighbors[i].getRelevantEdge(i)} : {activeTile.getRelevantEdgeActive(i)}");
                }
            }
        }

        // Case 3: Tiles can connect
        return true;

    }

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
        if(!isValidPlaceLocation(pos)) { return; }
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
