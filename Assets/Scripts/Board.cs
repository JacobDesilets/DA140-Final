using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;


public class Board
{
    private Dictionary<Vector3Int, Tile> cells;
    private List<Feature> features;

    public Board()
    {
        features = new List<Feature>();
        cells = new Dictionary<Vector3Int, Tile>();
        Tile startingTile = new Tile(EdgeType.Road, EdgeType.City, EdgeType.Road, EdgeType.Field, null, false);
        place(new Vector3Int(0, 0, 0), startingTile);

        
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
        updateRoadFeatures(t);

    }

    public void updateRoadFeatures(Tile t)
    {
        //Debug.Log(t == null);
        EdgeNode[] edges = t.getEdges();
        List<EdgeNode> connectedRoads = new List<EdgeNode>();

        // Find connected roads
        foreach(EdgeNode e in edges)
        {
            if(e.type == EdgeType.Road)
            {
                if(e.connectedTo != null) { connectedRoads.Add(e.connectedTo); }
            }
        }

        // Create new feature if no connected roads
        if(connectedRoads.Count == 0)
        {
            t.roadFeature = new Feature(EdgeType.Road, t);
            features.Add(t.roadFeature);
            
        } else  // Add to existing road feature, merge if needed
        {
            if(connectedRoads.Count == 1)
            {
                connectedRoads[0].belongsToTile.roadFeature.addTile(t);
            } else if(connectedRoads.Count == 2)
            {
                connectedRoads[0].belongsToTile.roadFeature.addTile(t);
                connectedRoads[0].belongsToTile.roadFeature.merge(connectedRoads[1].belongsToTile.roadFeature);
            }
        }

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
                    return false;
                }
                else
                {
                    Debug.Log($"Valid place location! Matches neighboring tile -> {neighbors[i].getRelevantEdge(i)} : {activeTile.getRelevantEdgeActive(i)}");
                }
            }
        }

        // Case 3: Tiles can connect
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
    protected List<Tile> tiles;
    public bool complete = false;
    protected int roadEndpoints;
    protected Dictionary<int, int> claimants;

    public Feature(EdgeType type, Tile initialTile)
    {
        this.type = type;
        roadEndpoints = 0;

        tiles = new List<Tile>();
        tiles.Add(initialTile);

        if(initialTile.isRoadEndpoint) { roadEndpoints++; }
        claimants = new Dictionary<int, int>();
    }

    public bool claim(int player, int meepleCount)
    {
        if(claimants.Count == 0)
        {
            claimants.Add(player, meepleCount);
            return true;
        } else { return false;  }
    }

    public void merge(Feature other)
    {
        Debug.Log("Merging features!");
        Assert.AreEqual(type, other.type);
        Assert.AreEqual(other.complete, false);

        foreach(var p in other.claimants)
        {
            if(claimants.ContainsKey(p.Key))
            {
                claimants[p.Key] = Math.Max(p.Value, claimants[p.Key]);
            } else
            {
                claimants.Add(p.Key, p.Value);
            }
        }

        foreach (Tile t in other.tiles)
        {
            addTile(t);
            if (t.roadFeature != null)
            {
                t.roadFeature = this;
            }
        }


    }

  

    public void addTile(Tile t)
    {
        // Roads
        if(type == EdgeType.Road)
        {
            if (roadEndpoints == 2) { Debug.LogError("This road feature already has two endpoints. It should not be possible to extend it."); return; }
            tiles.Add(t);
            if (t.isRoadEndpoint) { roadEndpoints++; }
            t.roadFeature = this;
            
            if(complete = checkCompletion()) { Debug.Log("Feature complete!"); }
        }
        
    }

    public bool containsTile(Tile t)
    {
        Guid id = t.id;

        foreach(Tile tile in tiles)
        {
            if(tile.id == id) { return true; }
        }

        return false;
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
