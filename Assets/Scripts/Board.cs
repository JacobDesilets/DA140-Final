using UnityEngine;
using System.Collections.Generic;


public class Board
{
    private Dictionary<Vector3, Tile> cells;

    public Board()
    {
        cells = new Dictionary<Vector3, Tile>();
    }

    public void place(Vector3 pos, Tile t)
    {
        cells.Add(pos, t);
    }

    public Tile getTile(Vector3 pos)
    {
        Tile tile = null;
        bool success = cells.TryGetValue(pos, out tile);

        if(success) { return tile; }
        else { return null; }
    }
   
}
