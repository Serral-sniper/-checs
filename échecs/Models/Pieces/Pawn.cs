using System.Diagnostics;

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
        possibleMoves.Add(ComputeDiagonal(ChangeAdders(Color, 1, 1), board));
        possibleMoves.Add(ComputeDiagonal(ChangeAdders(Color, 1, -1), board));
        Position? tempCase = ChangeAdders(Color, 1, 0);

        AddForwardMoves(possibleMoves, board);
        
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

    public Position? ComputeDiagonal(Position position, Board board)
    {
        if (board.HasPiece(position) && board.GetCase(position).Color != Color)
        {
            return position;
        }
        return null;
    }
    private bool CanAdvanceTo(Position? pos, Board board) => pos is not null && !board.HasPiece(pos);

    private void AddForwardMoves(List<Position> moves, Board board)
    {
        Position? oneStep = ChangeAdders(Color, 1, 0);
        if (!CanAdvanceTo(oneStep, board))
            return;

        moves.Add(oneStep!);

        Position? twoSteps = ChangeAdders(Color, 2, 0);
        if (FirstMove && CanAdvanceTo(twoSteps, board))
            moves.Add(twoSteps!);
    }
}
