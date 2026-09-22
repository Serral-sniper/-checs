namespace échecs.Models;

public class Position
{
    public int _X { get; private set; }
    public int _Y { get; private set; }

    
    public Position(int x, int y)
    {
        _X = x;
        _Y = y;
    }
    
}