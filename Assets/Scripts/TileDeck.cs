using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDeck
{
    private Tile[] tileArray;
    private Stack<Tile> tiles;

    public bool finished = false;
    public Tile currentTile;


    public TileDeck()
    {
        tiles = new Stack<Tile>();
        this.tileArray = loadTiles();
        Util.Shuffle(this.tileArray);
        foreach (Tile t in this.tileArray)
        {
            tiles.Push(t);
        }

        currentTile = tiles.Peek();
    }

    private Tile[] loadTiles()
    {
        GameObject[] objs = Resources.LoadAll<GameObject>("Tiles");
        //Tile[] t = new Tile[objs.Length];
        List<Tile> t = new List<Tile>();
        int i = 0;
        foreach (GameObject o in objs)
        {
            bool isRoadEndpoint = (o.name[4] == 'Y');
            int tileCount = o.name[5] - '0';
            string code = o.name.Substring(0, 4);
            EdgeType[] et = new EdgeType[4];
            int j = 0;
            foreach (char c in code)
            {
                switch (c)
                {
                    case 'F':
                        et[j] = EdgeType.Field;
                        break;
                    case 'R':
                        et[j] = EdgeType.Road;
                        break;
                    case 'C':
                        et[j] = EdgeType.City;
                        break;
                }
                j++;
            }

            for(int k = 0; k < tileCount; k++)
            {
                t.Add(new Tile(et[0], et[1], et[2], et[3], o, isRoadEndpoint));
                i++;
            }
        }
        return t.ToArray();
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
