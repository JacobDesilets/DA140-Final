
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

    public override string ToString()
    {
        string output = "";
        switch(type)
        {
            case EdgeType.Road:
                output = "Road";
                break;
            case EdgeType.City:
                output = "City";
                break;
            case EdgeType.Field:
                output = "Field";
                break;
        }

        return output;
    }
}
