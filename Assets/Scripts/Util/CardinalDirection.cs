public enum CardinalDirection
{
    NORTH, EAST, SOUTH, WEST
}

public static class CardinalDirectionExtensions
{
    public static CardinalDirection GetOpposite(this CardinalDirection dir)
    {
        switch (dir)
        {
            case CardinalDirection.NORTH:
                return CardinalDirection.SOUTH;
            case CardinalDirection.SOUTH:
                return CardinalDirection.NORTH;
            case CardinalDirection.EAST:
                return CardinalDirection.WEST;
            case CardinalDirection.WEST:
                return CardinalDirection.EAST;
            default:
                return CardinalDirection.NORTH;
        }
    }
}
