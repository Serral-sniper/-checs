namespace échecs.Models.Pieces;

public abstract class Piece
{
    protected EColor Color { get; private set; }
    protected Position Position { get; private set; }

    protected Piece(EColor color, Position position)
    {
        Color = color;
        Position = position;
    }

    public abstract void Move();

}