namespace échecs.Models.Pieces;

public class Pawn : Piece
{
    public bool FirstMove { get; private set; }

    public Pawn(EColor color, Position position) : base(color, position)
    {
        FirstMove = true;
    }

    public override List<Position> GetPossibleMoves(Board board)
    {
        List<Position> possibleMoves = new(){ChangeAdders(Color, 1, 1), ChangeAdders(Color, 1, -1)};
        
        
        if (FirstMove)
        {
            
        }
        RemoveNullFromPositions(possibleMoves);
        return possibleMoves;
    }

    /// <summary>
    /// Le pion perd son "premier coup" quand il bouge réellement,
    /// et non plus quand on calcule simplement ses déplacements possibles.
    /// </summary>
    public override void MoveTo(Position destination)
    {
        base.MoveTo(destination);
        FirstMove = false;
    }

    protected override Position ComputePosition(EColor color, int coeffX, int coeffY)
    {
        return color is EColor.Black ? base.ComputePosition(color, coeffX, coeffY) : new  Position(Position.X - coeffX, Position.Y - coeffY);
    }
}
