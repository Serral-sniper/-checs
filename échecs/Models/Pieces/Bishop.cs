using System.Runtime.CompilerServices;
using View = échecs.Views.View;

namespace échecs.Models.Pieces;

public class Bishop : Piece
{
    public Bishop(EColor color, Position position) : base(color, position)
    {
        
    }

    public override List<Position> GetPossibleMoves(View view, Piece piece)
    {
        List<Position> possibleMoves = new();
        
        
        
        
        return possibleMoves;
    }
}