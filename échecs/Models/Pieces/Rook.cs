namespace échecs.Models.Pieces;

public class Rook : Piece
{
    public Rook(EColor color, Position position) : base(color, position)
    {
    }

    public override List<Position> GetPossibleMoves(Board board)
    {
        List<Position> possibleMoves = new();

        return possibleMoves;
    }
}
