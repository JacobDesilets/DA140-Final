//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class Cell
{
    private float x, y, s;
    private Tile contents = null;

    public Cell(float x, float y, float s)
    {
        this.x = x;
        this.y = y;
        this.s = s;
    }

    public void place(Tile t)
    {
        contents = t;
    }

    public bool empty()
    {
        return (contents == null);
    }
}
