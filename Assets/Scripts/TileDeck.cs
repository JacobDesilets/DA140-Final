using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDeck
{
    private Tile[] tileArray;
    private Stack<Tile> tiles;

    public bool finished = false;
    public Tile currentTile;


    public TileDeck(Tile[] tileArray)
    {
        tiles = new Stack<Tile>();
        this.tileArray = tileArray;
        Util.Shuffle(this.tileArray);
        foreach (Tile t in this.tileArray)
        {
            tiles.Push(t);
        }

        currentTile = tiles.Peek();
    }

    public void draw()
    {
        if(!finished)
        {
            tiles.Pop();
        }
        
        if (tiles.Count == 0)
        {
            Debug.Log("Drew last tile!");
            finished = true;
            currentTile = null;
            return;
        }
        currentTile = tiles.Peek();
    }
}
