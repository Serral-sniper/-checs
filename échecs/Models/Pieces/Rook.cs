namespace échecs.Models.Pieces;
using View = échecs.Views.View;

public class Rook : Piece
{
    public Rook(EColor color, Position position) : base(color, position)
    {
        
    }

    public override List<Position> GetPossibleMoves(View view, Piece piece)
    {
        List<Position> possibleMoves = new();
        
        
        
        
        return possibleMoves;
    }
}