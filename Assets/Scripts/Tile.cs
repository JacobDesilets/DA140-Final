
public class Tile
{
    private EdgeNode[] edges;

    public Tile(EdgeType top, EdgeType right, EdgeType bottom, EdgeType left)
    {
        edges = new EdgeNode[] { new EdgeNode(top), new EdgeNode(right), new EdgeNode(bottom), new EdgeNode(left)};
    }


}
