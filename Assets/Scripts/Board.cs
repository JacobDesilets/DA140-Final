using UnityEngine;
using System.Collections.Generic;


public class Board
{
    private Dictionary<Vector3Int, Tile> cells;

    public Board()
    {
        cells = new Dictionary<Vector3Int, Tile>();
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
