namespace échecs.Models.Pieces;
using View = échecs.Views.View;

public class Pawn : Piece
{
    public bool FirstMove { get; private set; }
    
    public Pawn(EColor color, Position position) : base(color, position)
    {
        FirstMove = true;
    }

    public override List<Position> GetPossibleMoves(View view, Piece piece)
    {
        List<Position> possibleMoves = new();
        Piece[,] board = view.GetBoard();

        if (FirstMove)
        {
            
        }
            
        
        FirstMove = false;
        return possibleMoves;
    }

    protected override (int x, int y) ChangeAdders(EColor color,int x, int y)
    {
        return (color is EColor.Black)? (x, y) : (-x, -y);
    }
}