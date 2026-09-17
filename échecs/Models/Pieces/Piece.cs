namespace échecs;

public abstract class Piece
{
    protected EColor _Color { get; private set; }
    protected Position _Position { get; private set; }

    protected Piece(EColor color, Position position)
    {
        _Color = color;
        _Position = position;
    }

}