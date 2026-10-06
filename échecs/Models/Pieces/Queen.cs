namespace échecs.Models.Pieces;
using View = échecs.Views.View;

public class Queen : Piece
{
    public Queen(EColor color, Position position) : base(color, position)
    {
        
    }

    public override List<Position> GetPossibleMoves(View view, Piece piece)
    {
        List<Position> possibleMoves = new();
        
        
        
        
        return possibleMoves;
    }
}