using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Assertions;


public class Board
{
    private Dictionary<Vector3Int, Tile> cells;
    //private List<Feature> features;

    public Board()
    {
        //features = new List<Feature>();
        cells = new Dictionary<Vector3Int, Tile>();

        // set up starting tile
        Tile startingTile = new Tile(EdgeType.Road, EdgeType.City, EdgeType.Road, EdgeType.Field, null, false);
        place(new Vector3Int(0, 0, 0), startingTile, 0, null);
    }



    public void place(Vector3Int pos, Tile t, int claimant, EdgeNode claimedEdge)
    {
        cells.Add(pos, t);

        //List<Feature> neighborFeatures = new List<Feature>();
        Tile[] neighbors = getNeighbors(pos);
        EdgeNode[] activeTileEdges = t.getEdges();

        // Create EdgeNode connections
        for (int i = 0; i < neighbors.Length; i++)
        {
            Tile n = neighbors[i];
            EdgeNode activeEdge = activeTileEdges[i];
            if(n != null)
            {
                EdgeNode neighborEdge = n.getRelevantEdge(i);
                activeEdge.connectedTo = neighborEdge;
                neighborEdge.connectedTo = activeEdge;

            }
        }

        updateRoadFeatures(t, claimant, claimedEdge);
    }

    
    // returns player who has the claim - null if none, string of numbers if tied
    public void updateRoadFeatures(Tile t, int claimant, EdgeNode claimedEdge)
    {

        List<EdgeNode> roadsToDo = new List<EdgeNode>();
        foreach (EdgeNode e in t.getEdges())
        {
            if(t.isRoadEndpoint && e.type == EdgeType.Road)
            {
                roadsToDo.Add(e);
            } else if (e.type == EdgeType.Road)
            {
                roadsToDo.Add(e);
                break;
            }
        }

        while(roadsToDo.Count > 0)
        {
            Dictionary<int, int> meeples = new Dictionary<int, int>();

            

            EdgeNode currentRoad = roadsToDo[0];
            if(claimant != 0 && currentRoad.Equals(claimedEdge))
            {
                meeples[claimant] = 1;
                currentRoad.claimant = claimant;
            }

            roadsToDo.RemoveAt(0);

            Feature f = exploreRoad(currentRoad, claimant);
        }
    }

    public Feature exploreRoad(EdgeNode e, int claimant)
    {
        Dictionary<int, int> meeples = new Dictionary<int, int>();
        List<EdgeNode> connected = new List<EdgeNode>();
        List<EdgeNode> unexplored;
        List<int> claimants = new List<int>();

        int c1;
        meeples.TryGetValue(e.claimant, out c1);
        meeples[claimant] = c1 + 1;


        e.claimant = claimant;

        if (!e.belongsToTile.isRoadEndpoint) { connected = e.belongsToTile.getLocalRoads(); }
        else { connected.Add(e); }

        unexplored = findAdjacentEdges(connected);

        while (unexplored.Count != 0)
        {
            EdgeNode toExplore = unexplored[0];
            unexplored.RemoveAt(0);

            if (toExplore.claimant != 0)
            {
                int c2;
                meeples.TryGetValue(toExplore.claimant, out c2);
                meeples[toExplore.claimant] = c2 + 1;
            }

            connected.Add(toExplore);
            //if (toExplore.claimant != 0 && claimaint == 0) { claimaint = toExplore.claimant; }
            List<EdgeNode> new_unexplored = findAdjacentEdges(connected);

            unexplored = new List<EdgeNode>(unexplored.Union(new_unexplored));
            Debug.Log($"Unexplored count: {unexplored.Count}");
        }

        
        

        int max = meeples.FirstOrDefault(x => x.Value == meeples.Values.Max()).Key;
        foreach(var KVP in meeples)
        {
            if(KVP.Value == max)
            {
                claimants.Add(KVP.Key);
            }
        }

        Feature f = new Feature(EdgeType.Road, connected, claimants);
        Debug.Log($"Found road with {f.numUniqueTiles()} tiles");
        return f;
    }

    public bool canClaim(EdgeNode e)
    {
        Feature testFeature = exploreRoad(e, 0);
        if(testFeature.claimants.Count > 1) { return false; }
        return true;
    }

    public List<EdgeNode> findAdjacentEdges(List<EdgeNode> connected)
    {
        List<EdgeNode> adj = new List<EdgeNode>();
        List<EdgeNode> visited = new List<EdgeNode>();

        //Debug.Log($"Finding adjacents to {connected.Count} edges");
        foreach (EdgeNode e in connected)
        {

            if (e.connectedTo != null && !adj.Contains(e.connectedTo) && !connected.Contains(e.connectedTo))
            {
                if (e.type == EdgeType.Road)
                {
                    adj.Add(e.connectedTo);
                }

            }

            if(!e.belongsToTile.isRoadEndpoint && e.type == EdgeType.Road)
            {
                foreach(EdgeNode a_e in e.belongsToTile.getLocalRoads())
                {
                    if(!adj.Contains(a_e) && !connected.Contains(a_e))
                    {
                        adj.Add(a_e);
                    }
                }
            }
        }
        Debug.Log($"Adj size: {adj.Count}");
        return adj;
        
    }

    public int[] getScores()
    {
        Debug.Log("Checking scores!");
        int[] scores = new int[GameManager.Instance.playerCount];
        Debug.Log(scores.Length);
        return scores;
    }

    public bool isValidPlaceLocation(Vector3Int pos, Tile activeTile)
    {
        // Case 0: location already occupied
        if (getTile(pos) != null) { Debug.Log("Already occupied!"); return false; }

        Tile[] neighbors = getNeighbors(pos);

        // Case 1: Location has no neighbors
        bool hasNeighbors = false;
        foreach (Tile n in neighbors)
        {
            if (n != null) { hasNeighbors = true; }
        }

        if (!hasNeighbors)
        {
            Debug.Log("Invalid place location! Must be adjacent to at least one tile");
            GameManager.Instance.errorText = "Invalid place location! Must be adjacent to at least one tile";
            return false;
        }

        // Case 2: Tiles can't connect

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                if (!neighbors[i].getRelevantEdge(i).match(activeTile.getRelevantEdgeActive(i)))
                {
                    //Debug.Log(i);
                    //Debug.Log($"Top: {activeTile.getTop()} Right: {activeTile.getRight()} Bottom: {activeTile.getBottom()} Left: {activeTile.getLeft()}");
                    Debug.Log($"Invalid place location! Doesn't match neighboring tile -> {neighbors[i].getRelevantEdge(i)} : {activeTile.getRelevantEdgeActive(i)}");
                    GameManager.Instance.errorText = $"Invalid place location! Doesn't match neighboring tile -> {neighbors[i].getRelevantEdge(i)} : {activeTile.getRelevantEdgeActive(i)}";
                    return false;
                }
                else
                {
                    Debug.Log($"Valid place location! Matches neighboring tile -> {neighbors[i].getRelevantEdge(i)} : {activeTile.getRelevantEdgeActive(i)}");
                }
            }
        }

        // Case 3: Tiles can connect
        GameManager.Instance.errorText = "";
        return true;

    }

    public Tile getTile(Vector3Int pos)
    {
        Tile tile = null;
        bool success = cells.TryGetValue(pos, out tile);

        if(success) { return tile; }
        else { return null; }
    }

    public Tile[] getNeighbors(Vector3Int pos)
    {
        Tile topNeighbor = getTile(new Vector3Int(pos.x, 0, pos.z + 1));
        Tile rightNeighbor = getTile(new Vector3Int(pos.x + 1, 0, pos.z));
        Tile bottomNeighbor = getTile(new Vector3Int(pos.x, 0, pos.z - 1));
        Tile leftNeighbor = getTile(new Vector3Int(pos.x - 1, 0, pos.z));
        Tile[] neighbors = { topNeighbor, rightNeighbor, bottomNeighbor, leftNeighbor };
        return neighbors;
    }
   
}

public class Feature
{
    public EdgeType type;
    protected List<EdgeNode> edgeNodes;
    protected List<Tile> tiles;
    public bool complete = false;
    protected int roadEndpoints;
    public List<int> claimants;


    public Feature(EdgeType type, List<EdgeNode> l, List<int> claimants)
    {
        this.type = type;
        edgeNodes = l;

        tiles = new List<Tile>();

        foreach (EdgeNode e in edgeNodes.ToList())
        {
            addEdgeNode(e);
        }

        this.claimants = claimants;
    }

    public int getNumMembers()
    {
        return edgeNodes.Count;
    }

    public int getScore()
    {
        if (type == EdgeType.Road)
        {
            return numUniqueTiles();
        }
        else
        {
            return 0;
        }
    }

    public int numUniqueTiles()
    {
        List<Tile> tiles = new List<Tile>();
        foreach (EdgeNode e in edgeNodes)
        {
            bool unique = true;
            foreach (Tile t in tiles)
            {
                if (t.id == e.belongsToTile.id)
                {
                    unique = false;
                }
            }

            if (unique) { tiles.Add(e.belongsToTile); }
        }

        return tiles.Count;
    }

    public void addEdgeNode(EdgeNode e)
    {
        // Roads
        if (type == EdgeType.Road)
        {
            //if (roadEndpoints == 2) { Debug.LogError("This road feature already has two endpoints. It should not be possible to extend it."); return; }
            if (!tiles.Contains(e.belongsToTile))
            {
                if (e.belongsToTile.isRoadEndpoint) { roadEndpoints++; }
                tiles.Add(e.belongsToTile);
            }
            edgeNodes.Add(e);
            //e.roadFeature = this;
            if (complete = checkCompletion()) { Debug.Log("Feature complete!"); }
        }
    }



    public bool checkCompletion()
    {
        if (type == EdgeType.Road)
        {
            return (roadEndpoints == 2);
        }

        return false;
    }
}
