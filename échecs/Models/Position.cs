namespace échecs.Models;

public class Position
{
    public int X { get; private set; }
    public int Y { get; private set; }

    
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
    //<summary>
    //Addition d'une coordonnée de type Position avec un tuple de 2 valeurs type int.
    //<summary>
    public static Position operator +(Position position, (int x, int y) adders)
    {
        return new Position(position.X + adders.x, position.Y + adders.y);
    }
    
}