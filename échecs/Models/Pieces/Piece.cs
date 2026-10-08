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

    /// <summary>
    /// Calcule les cases atteignables. Ne modifie JAMAIS l'état : c'est une simple lecture.
    /// </summary>
    public abstract List<Position> GetPossibleMoves(Board board);

    /// <summary>
    /// Appelé par Board.Move quand le coup est réellement joué.
    /// </summary>
    public virtual void MoveTo(Position destination)
    {
        Position = destination;
    }

    protected virtual (int x, int y) ChangeAdders(EColor color, int x, int y)
    {
        return (x, y);
    }
}
