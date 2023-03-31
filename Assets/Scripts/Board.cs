using UnityEngine;
using System.Collections.Generic;


public class Board
{
    private Dictionary<Vector3Int, Tile> cells;

    public Board()
    {
        cells = new Dictionary<Vector3Int, Tile>();
        place(new Vector3Int(0, 0, 0), new Tile(EdgeType.Road, EdgeType.City, EdgeType.Road, EdgeType.Field, null));
    }

    public void place(Vector3Int pos, Tile t)
    {
        cells.Add(pos, t);
    }

    public Tile getTile(Vector3Int pos)
    {
        Tile tile = null;
        bool success = cells.TryGetValue(pos, out tile);

        if(success) { return tile; }
        else { return null; }
    }
   
}
