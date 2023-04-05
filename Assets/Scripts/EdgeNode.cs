
public class EdgeNode
{
    public EdgeType type;
    public Tile belongsToTile { get; private set; }
    public EdgeNode connectedTo { get; set; }
    public EdgeNode(EdgeType type, Tile belongsToTile)
    {
        this.type = type;
        this.belongsToTile = belongsToTile;
        this.connectedTo = null;
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
