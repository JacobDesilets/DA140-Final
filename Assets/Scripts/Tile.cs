using UnityEngine;


public class Tile
{
    private EdgeNode[] edges;
    private int topIndex = 0;
    private int rightIndex = 1;
    private int bottomIndex = 2;
    private int leftIndex = 3;

    private int rotation = 0;

    public GameObject prefab;

    public Tile(EdgeType top, EdgeType right, EdgeType bottom, EdgeType left, GameObject prefab)
    {
        edges = new EdgeNode[] { new EdgeNode(top), new EdgeNode(right), new EdgeNode(bottom), new EdgeNode(left)};
        this.prefab = prefab;
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

    public void rotate()
    {
        topIndex = increment(topIndex);
        leftIndex = increment(leftIndex);
        bottomIndex = increment(bottomIndex);
        rightIndex = increment(rightIndex);

        rotation = increment(rotation);
    }

    private int increment(int val)
    {
        int output = val;
        output++;
        if(output > 3) { output = 0; }
        return output;
    }

    public Quaternion getRotation()
    {
        Quaternion qrot = Quaternion.Euler(0, rotation * 90, 0);
        return qrot * prefab.transform.localRotation;
    }


}
