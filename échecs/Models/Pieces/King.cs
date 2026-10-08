namespace échecs.Models.Pieces;

public class King : Piece
{
    public King(EColor color, Position position) : base(color, position)
    {
    }

    public override List<Position> GetPossibleMoves(Board board)
    {
        List<Position> possibleMoves = new();

        return possibleMoves;
    }

    protected override (int x, int y) ChangeAdders(EColor color, int x, int y)
    {
        return color is EColor.Black ? (x, y) : (-x, -y);
    }
}
