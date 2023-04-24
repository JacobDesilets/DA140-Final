using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Assertions;


public class Board
{
    private Dictionary<Vector3Int, Tile> cells;
    private List<Feature> features;

    public Board()
    {
        features = new List<Feature>();
        cells = new Dictionary<Vector3Int, Tile>();

        // set up starting tile
        Tile startingTile = new Tile(EdgeType.Road, EdgeType.City, EdgeType.Road, EdgeType.Field, null, false);
        place(new Vector3Int(0, 0, 0), startingTile);
        startingTile.getTop().roadFeature = new Feature(EdgeType.Road, startingTile.getTop());
        startingTile.getTop().roadFeature.addEdgeNode(startingTile.getBottom());
        startingTile.getBottom().roadFeature = startingTile.getTop().roadFeature;
        features.Add(startingTile.getBottom().roadFeature);
        // TODO starting tile city feature

        
        //features.Add(new Feature(EdgeType.Road, startingTile));
        //features.Add(new Feature(EdgeType.City, startingTile));
    }



    public void place(Vector3Int pos, Tile t)
    {
        cells.Add(pos, t);

        List<Feature> neighborFeatures = new List<Feature>();
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


        // Update features
        Feature f = updateRoadFeatures(t);
        if(f.checkCompletion())
        {
            int score = f.getScore();
            GameManager.Instance.addScore(score);
        }

    }

    

    public Feature updateRoadFeatures(Tile t)
    {
        int claimaint = 0;
        List<EdgeNode> connected = new List<EdgeNode>();
        List<EdgeNode> unexplored;
        EdgeNode startRoad = null;
        // find connected sides
        // find the first road to start with
        foreach(EdgeNode e in t.getEdges())
        {
            if(e.type == EdgeType.Road)
            {
                if (e.claimant != 0 && claimaint == 0) { claimaint = e.claimant; }

                if(startRoad == null)
                {
                    startRoad = e;
                    connected.Add(e);
                } else if (!e.belongsToTile.isRoadEndpoint)
                {
                    connected.Add(e);
                }
            }
        }
        if (startRoad == null) { Debug.Log("Error: tile contains no roads"); }
        unexplored = findAdjacentEdges(connected);

        

        while(unexplored.Count != 0)
        {
            EdgeNode toExplore = unexplored[0];
            unexplored.RemoveAt(0);

            
            connected.Add(toExplore);
            if (toExplore.claimant != 0 && claimaint == 0) { claimaint = toExplore.claimant; }
            List<EdgeNode> new_unexplored = findAdjacentEdges(connected);

            unexplored = (List<EdgeNode>) unexplored.Union(new_unexplored);
        }

        return new Feature(EdgeType.Road, connected, claimaint);




        ////Debug.Log(t == null);
        //EdgeNode[] edges = t.getEdges();
        //List<EdgeNode> connectedRoads = new List<EdgeNode>();

        //Feature roadFeature = null;


        //foreach (EdgeNode e in edges)
        //{
        //    if(e.type == EdgeType.Road)
        //    {
        //        // edgenode is connected
        //        if (e.connectedTo != null)
        //        {
        //            e.connectedTo.roadFeature.addEdgeNode(e);
        //            //e.roadFeature = e.connectedTo.roadFeature;
        //            e.connectedTo.roadFeature.addEdgeNode(e);
        //            roadFeature = e.roadFeature;
                    
        //        } else if(!e.belongsToTile.isRoadEndpoint && roadFeature != null)  // edgenode is not connected and not road endpoint
        //        {
        //            roadFeature.addEdgeNode(e);
        //            //e.roadFeature = roadFeature;
        //        } else if(e.belongsToTile.isRoadEndpoint)
        //        {
        //            e.roadFeature = new Feature(EdgeType.Road, e);
        //            features.Add(e.roadFeature);
        //        }
        //    }
        //}

        ////Find connected roads
        //foreach (EdgeNode e in edges)
        //{
        //    if (e.type == EdgeType.Road && e.roadFeature == null && roadFeature != null)
        //    {
        //        //roadFeature = new Feature(EdgeType.Road, e);
        //        //e.roadFeature = roadFeature;
        //        //break;
        //        //roadFeature.addEdgeNode(e);
        //        e.roadFeature = roadFeature;
                
        //    }
        //}
        
    }

    public List<EdgeNode> findAdjacentEdges(List<EdgeNode> connected)
    {
        List<EdgeNode> adj = new List<EdgeNode>();
        foreach(EdgeNode e in connected)
        {
            if(e.connectedTo != null && !adj.Contains(e.connectedTo) && !connected.Contains(e.connectedTo))
            {
                if(e.type == EdgeType.Road && !e.belongsToTile.isRoadEndpoint)
                {
                    adj.Add(e.connectedTo);
                }
                
            }
        }

        return adj;

    }

    public int[] getScores()
    {
        Debug.Log("Checking scores!");
        int[] scores = new int[GameManager.Instance.playerCount];
        foreach (Feature f in features)
        {
            if (f.isClaimed())
            {
                int k = f.claimant;
                int v = f.getScore();
                scores[k - 1] = v;
            }
        }

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
    //protected Dictionary<int, int> claimants;
    public int claimant { get; private set; }

    public Feature(EdgeType type, EdgeNode initialEdgeNode)
    {
        this.type = type;
        roadEndpoints = 0;


        edgeNodes = new List<EdgeNode>();
        edgeNodes.Add(initialEdgeNode);

        tiles = new List<Tile>();

        if (initialEdgeNode.belongsToTile.isRoadEndpoint) { roadEndpoints++; }
        claimant = 0;
    }

    public Feature(EdgeType type, List<EdgeNode> l, int claimaint)
    {
        this.type = type;
        edgeNodes = l;

        tiles = new List<Tile>();

        foreach (EdgeNode e in edgeNodes.ToList())
        {
            addEdgeNode(e);
        }

        this.claimant = claimant;
    }

    public int getNumMembers()
    {
        return edgeNodes.Count;
    }

    public bool claim(int player, int meepleCount)
    {
        if(claimant == 0)
        {
            claimant = player;
            return true;
        } else { return false;  }
    }

    public bool isClaimed()
    {
        //Debug.Log(claimants.Count);
        return (claimant != 0);
    }

    public int getScore()
    {
        if(type == EdgeType.Road)
        {
            return numUniqueTiles();
        }
        else
        {
            return 0;
        }
    }

    private int numUniqueTiles()
    {
        List<Tile> tiles = new List<Tile>();
        foreach (EdgeNode e in edgeNodes)
        {
            bool unique = true;
            foreach(Tile t in tiles)
            {
                if(t.id == e.belongsToTile.id)
                {
                    unique = false;
                }
            }

            if(unique) { tiles.Add(e.belongsToTile); }
        }

        return tiles.Count;
    }

    public void addEdgeNode(EdgeNode e)
    {
        // Roads
        if (type == EdgeType.Road)
        {
            //if (roadEndpoints == 2) { Debug.LogError("This road feature already has two endpoints. It should not be possible to extend it."); return; }
            if(!tiles.Contains(e.belongsToTile))
            {
                if (e.belongsToTile.isRoadEndpoint) { roadEndpoints++; }
                tiles.Add(e.belongsToTile);
            }
            edgeNodes.Add(e);
            e.roadFeature = this;
            if (complete = checkCompletion()) { Debug.Log("Feature complete!"); }
        }
    }



    public bool checkCompletion()
    {
        if(type == EdgeType.Road)
        {
            return (roadEndpoints == 2);
        }

        return false;
    }
}
