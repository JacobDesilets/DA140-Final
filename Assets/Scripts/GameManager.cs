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
    public Tile activeTile;

    public Tile previewTile;

    private int playerCount;
    private int turn = 1;
    private int[] scores;

    private Board board;
    private TileDeck td;

    private Tile testTile;

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
