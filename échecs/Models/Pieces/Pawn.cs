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
        List<Position> possibleMoves = new();
        
        if (FirstMove)
        {
            
        }

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

    protected override (int x, int y) ChangeAdders(EColor color, int x, int y)
    {
        return color is EColor.Black ? (x, y) : (-x, -y);
    }
}
