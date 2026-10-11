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

    protected override Position ComputePosition(EColor color, int coeffX, int coeffY)
    {
        return color is EColor.Black ? base.ComputePosition(color, coeffX, coeffY) : new  Position(Position.X - coeffX, Position.Y - coeffY);
    }
}
