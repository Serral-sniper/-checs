namespace échecs.Models.Pieces;

public class Queen : Piece
{
    public Queen(EColor color, Position position) : base(color, position)
    {
    }

    public override List<Position> GetPossibleMoves(Board board)
    {
        List<Position> possibleMoves = new();

        return possibleMoves;
    }
}
