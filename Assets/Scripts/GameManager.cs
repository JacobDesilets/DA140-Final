using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public GameObject player;
    public PlayerController pc;

    public GameObject noTileObject;

    public static GameManager Instance { get; private set; }
    public Tile activeTile;

    public Tile previewTile;

    public GameObject claimText;

    public int playerCount { get; private set; }
    public int turn { get; private set; }
    private int[] scores;
    private int[] meeples;

    private Board board;
    private TileDeck td;

    public string errorText = "";


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

        begin();

    }

    public void begin()
    {
        turn = 1;
        setPlayers(2);

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
        meeples = new int[count];
    }

    public int claim(int edge, Vector3Int pos)
    {
        EdgeNode edgeToClaim = board.getTile(pos).getEdges()[edge-1];
        if(edgeToClaim.type == EdgeType.Field) { return 1; }
        if(edgeToClaim.type == EdgeType.Road)
        {
            //    if(edgeToClaim.roadFeature != null)
            //    {
            //        if (edgeToClaim.roadFeature.isClaimed())
            //        {
            //            return 2;
            //        } else
            //        {
            //            if (edgeToClaim.roadFeature.claim(turn, 1))
            //            {
            //                Vector3 textPos = new Vector3(pos.x, .3f, pos.z);
            //                switch (edge)
            //                {
            //                    case 1:
            //                        textPos.z += 0.4f;
            //                        break;
            //                    case 2:
            //                        textPos.x += 0.4f;
            //                        break;
            //                    case 3:
            //                        textPos.z -= 0.4f;
            //                        break;
            //                    case 4:
            //                        textPos.x -= 0.4f;
            //                        break;
            //                }
            //                GameObject claimMarker = Instantiate(claimText, textPos, Quaternion.Euler(90, 0, 0)) ;
            //                claimMarker.GetComponent<ClaimText>().text = $"{turn}";


            //                return 0;
            //            } else
            //            {
            //                Debug.Log("Claim returned false");
            //            }
            //        }
            //    } else { Debug.Log("road feature is null"); }
            edgeToClaim.claimant = turn;
            Vector3 textPos = new Vector3(pos.x, .3f, pos.z);
            switch (edge)
            {
                case 1:
                    textPos.z += 0.4f;
                    break;
                case 2:
                    textPos.x += 0.4f;
                    break;
                case 3:
                    textPos.z -= 0.4f;
                    break;
                case 4:
                    textPos.x -= 0.4f;
                    break;
            }
            GameObject claimMarker = Instantiate(claimText, textPos, Quaternion.Euler(90, 0, 0));
            claimMarker.GetComponent<ClaimText>().text = $"{turn}";
            return 0;
        } 

            

        return 3;
    }

    public string getCurrentPlayerText()
    {
        return ($"Player: {turn}");
    }

    public void advanceTurn()
    {
        turn++;
        if(turn > playerCount) { turn = 1; }

        td.draw();
        if (!td.finished)
        {
            activeTile = td.currentTile;
            previewTile = activeTile;


        }
        else
        {
            Debug.Log("Finished");
            previewTile = new Tile(EdgeType.Field, EdgeType.Field, EdgeType.Field, EdgeType.Field, noTileObject, false);
        }

        //int[] newScores = board.getScores();
        //scores = newScores;
        //Debug.Log(string.Join(" ", scores));

        pc.refreshPreviewTile();
    }

    public void addScore(int score)
    {
        scores[turn - 1] += score;
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

    public int getScore(int player)
    {
        return scores[player - 1];
    }

    public bool placeTile(Vector3Int pos)
    {
        if(td.finished) { return false; }
        if(!board.isValidPlaceLocation(pos, activeTile)) { return false; }
        Tile newTile = activeTile.copy();
        Debug.Log("Place!");

        board.place(pos, newTile);
        Instantiate(newTile.prefab, pos, activeTile.getRotation());
        previewTile = new Tile(EdgeType.Field, EdgeType.Field, EdgeType.Field, EdgeType.Field, noTileObject, false);
        return true;
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
