using UnityEngine;
using System.Collections.Generic;
using System;


public class Board
{
    private Dictionary<Vector3Int, Tile> cells;
    private List<Feature> features;

    public Board()
    {
        cells = new Dictionary<Vector3Int, Tile>();
        Tile startingTile = new Tile(EdgeType.Road, EdgeType.City, EdgeType.Road, EdgeType.Field, null, false);
        place(new Vector3Int(0, 0, 0), startingTile);

        features = new List<Feature>();
        features.Add(new Feature(EdgeType.Road, startingTile));
        features.Add(new Feature(EdgeType.City, startingTile));
    }



    public void place(Vector3Int pos, Tile t)
    {
        cells.Add(pos, t);

        List<Feature> neighborFeatures = new List<Feature>();
        // Check if neighbors are in features
        Tile[] neighbors = getNeighbors(pos);
        foreach(Tile n in neighbors)
        {
            if(n != null)
            {
                foreach(Feature f in features)
                {
                    if(f.containsTile(n))
                    {

                    }
                }
            }
        }
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
    private List<Tile> tiles;
    public bool complete = false;
    private int roadEndpoints;

    public Feature(EdgeType type, Tile initialTile)
    {
        this.type = type;
        roadEndpoints = 0;

        tiles = new List<Tile>();
        tiles.Add(initialTile);

        if(initialTile.isRoadEndpoint) { roadEndpoints++; }
    }

    public void addTile(Tile t)
    {
        if (roadEndpoints == 2) { Debug.LogError("This road feature already has two endpoints. It should not be possible to extend it."); return; }
        tiles.Add(t);
        if(t.isRoadEndpoint) { roadEndpoints++; }
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
