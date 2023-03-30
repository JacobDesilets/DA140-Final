using UnityEngine;


public class Tile
{
    private EdgeNode[] edges;
    GameObject prefab;

    public Tile(EdgeType top, EdgeType right, EdgeType bottom, EdgeType left, GameObject prefab)
    {
        edges = new EdgeNode[] { new EdgeNode(top), new EdgeNode(right), new EdgeNode(bottom), new EdgeNode(left)};
        this.prefab = prefab;
    }


}
