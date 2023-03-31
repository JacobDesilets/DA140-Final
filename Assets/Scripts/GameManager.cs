using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public GameObject player;
    private PlayerController pc;

    public static GameManager Instance { get; private set; }
    public Tile activeTile;

    public Tile previewTile;

    private int playerCount;
    private int turn = 1;
    private int[] scores;

    private Board board;
    private TileDeck td;

    private Tile[] loadTiles() {
        GameObject[] objs = Resources.LoadAll<GameObject>("Tiles");
        Tile[] t = new Tile[objs.Length];
        int i = 0;
        foreach(GameObject o in objs) {
            string code = o.name.Substring(0, 4);
            EdgeType[] et = new EdgeType[4];
            int j = 0;
            foreach(char c in code) {
                switch(c) {
                    case 'F':
                        et[j] = EdgeType.Field;
                        break;
                    case 'R':
                        et[j] = EdgeType.Road;
                        break;
                    case 'C':
                        et[j] = EdgeType.City;
                        break;
                }
                j++;
            }
            
            t[i] = new Tile(et[0], et[1], et[2], et[3], o);
            i++;
        }
        return t;
    }

    private bool isValidPlaceLocation(Vector3Int pos)
    {
        // Case 0: location already occupied
        if (board.getTile(pos) != null) { Debug.Log("Already occupied!"); return false; }

        
        Tile topNeighbor = board.getTile(new Vector3Int(pos.x, 0, pos.z+1));
        Tile rightNeighbor = board.getTile(new Vector3Int(pos.x+1, 0, pos.z));
        Tile bottomNeighbor = board.getTile(new Vector3Int(pos.x, 0, pos.z-1));
        Tile leftNeighbor = board.getTile(new Vector3Int(pos.x-1, 0, pos.z));
        Tile[] neighbors = { topNeighbor, rightNeighbor, bottomNeighbor, leftNeighbor };

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

        Tile[] tileArray = loadTiles();
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

    public void placeTile(Vector3Int pos)
    {
        
        if(!isValidPlaceLocation(pos)) { return; }

        Tile newTile = activeTile.copy();
        Debug.Log("Place!");
        board.place(pos, newTile);
        Instantiate(newTile.prefab, pos, activeTile.getRotation());
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
