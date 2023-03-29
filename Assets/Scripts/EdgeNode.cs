
public class EdgeNode
{
    public EdgeType type;
    public EdgeNode(EdgeType type)
    {
        this.type = type;
    }

    public bool match(EdgeNode other)
    {
        return (other.type == type);
    }
}
