namespace échecs.Models.Pieces;

public abstract class Piece
{
    public EColor Color { get; protected set; }
    public Position Position { get; protected set; }

    protected Piece(EColor color, Position position)
    {
        Color = color;
        Position = position;
    }

    public abstract void Move();

}