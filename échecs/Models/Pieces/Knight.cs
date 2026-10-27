namespace échecs.Models.Pieces;

public class Knight : Piece
{
    public Knight(EColor color, Position position) : base(color, position)
    {
    }

    public override List<Position> GetPossibleMoves(Board board)
    {
        List<Position> possibleMoves = new();
        
        //Ajouter les déplacements géométriquement possible. (cavalier)
        
        return possibleMoves;
    }

    protected override Position ComputePosition(EColor color, int coeffX, int coeffY)
    {
        return color is EColor.Black ? base.ComputePosition(color, coeffX, coeffY) : new  Position(Position.X - coeffX, Position.Y - coeffY);
    }
}
