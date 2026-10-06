namespace échecs.Models.Pieces;
using View = échecs.Views.View;

public class Knight : Piece
{
    public Knight(EColor color, Position position) : base(color, position)
    {
        
    }


    public override List<Position> GetPossibleMoves(View view, Piece piece)
    {
        List<Position> possibleMoves = new();
        
        
        
        
        return possibleMoves;
    }
    protected override (int x, int y) ChangeAdders(EColor color,int x, int y)
    {
        return (color is EColor.Black)? (x, y) : (-x, -y);
    }
}