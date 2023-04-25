using UnityEngine;
using System;
using System.Collections.Generic;

public class Tile : IEquatable<Tile>
{
    public Guid id;

    private EdgeNode[] edges;
    private int topIndex = 0;
    private int rightIndex = 1;
    private int bottomIndex = 2;
    private int leftIndex = 3;

    protected int rotation = 0;

    public GameObject prefab;

    //public Feature roadFeature;

    public bool isRoadEndpoint;


    public Tile(EdgeType top, EdgeType right, EdgeType bottom, EdgeType left, GameObject prefab, bool isRoadEndpoint)
    {
        id = Guid.NewGuid();
        edges = new EdgeNode[] { new EdgeNode(top, this), new EdgeNode(right, this), new EdgeNode(bottom, this), new EdgeNode(left, this)};
        this.prefab = prefab;
        this.isRoadEndpoint = isRoadEndpoint;

    }

    public bool Equals(Tile t)
    {
        return id == t.id;
    }

    public Tile copy()
    {
        Tile newTile = new Tile(edges[0].type, edges[1].type, edges[2].type, edges[3].type, prefab, isRoadEndpoint);
        newTile.rotation = rotation;
        newTile.topIndex = topIndex;
        newTile.rightIndex = rightIndex;
        newTile.bottomIndex = bottomIndex;
        newTile.leftIndex = leftIndex;

        return newTile;
    }

    public List<EdgeNode> getLocalRoads()
    {
        List<EdgeNode> localRoads = new List<EdgeNode>();
        if(!isRoadEndpoint)
        {
            foreach(EdgeNode e in edges)
            {
                if(e.type == EdgeType.Road) { localRoads.Add(e); }
            }
        }
        return localRoads;
    }

    public EdgeNode getTop()
    {
        return edges[topIndex];
    }

    public EdgeNode getRight()
    {
        return edges[rightIndex];
    }

    public EdgeNode getBottom()
    {
        return edges[bottomIndex];
    }

    public EdgeNode getLeft()
    {
        return edges[leftIndex];
    }

    public EdgeNode getRelevantEdge(int index)  // 0 1 2 3 :: top right bottom left
    {
        EdgeNode output = null;
        switch(index)
        {
            case 0:
                output = getBottom();
                break;
            case 1:
                output = getLeft();
                break;
            case 2:
                output = getTop();
                break;
            case 3:
                output = getRight();
                break;
        }
        return output;
    }

    public EdgeNode getRelevantEdgeActive(int index)  // 0 1 2 3 :: top right bottom left
    {
        EdgeNode output = null;
        switch (index)
        {
            case 0:
                output = getTop();
                break;
            case 1:
                output = getRight();
                break;
            case 2:
                output = getBottom();
                break;
            case 3:
                output = getLeft();
                break;
        }
        return output;
    }

    public EdgeNode[] getEdges()
    {
        EdgeNode[] e = { getTop(), getRight(), getBottom(), getLeft() };
        return e;
    }

    public void rotate()
    {
        topIndex = increment(topIndex);
        leftIndex = increment(leftIndex);
        bottomIndex = increment(bottomIndex);
        rightIndex = increment(rightIndex);

        rotation = increment(rotation);
        EdgeNode[] testEdges = getEdges();
        string edgesString = "";
        foreach(EdgeNode e in testEdges)
        {
            edgesString += " " + e.ToString();
        }
        Debug.Log(edgesString);
    }

    private int increment(int val)
    {
        int output = val;
        output++;
        if (output > 3) { output = 0; }
        return output;
    }

    public Quaternion getRotation()
    {
        Quaternion qrot = Quaternion.Euler(0, rotation * -90, 0);
        return qrot * prefab.transform.localRotation;
    }

}
