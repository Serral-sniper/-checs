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

    protected Position? ChangeAdders(EColor color, int x, int y)
    {
        Position newPosition = ComputePosition(color, x, y); // comportement défaut ou override

        return Board.IsInside(newPosition) ? newPosition : null;
    }

// Calcul par défaut, redéfinissable
    protected virtual Position ComputePosition(EColor color, int coeffX, int coeffY)
    {
        return new Position(Position.X + coeffX, Position.Y + coeffY);
    }
    
    protected void RemoveNullFromPositions(List<Position> possibleMoves)
    {
        possibleMoves.RemoveAll(p => p is null);
    }
}
