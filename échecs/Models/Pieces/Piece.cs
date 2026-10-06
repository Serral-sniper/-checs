namespace échecs.Models.Pieces;
using View = échecs.Views.View;

public abstract class Piece
{
    public EColor Color { get; protected set; }
    public Position Position { get; protected set; }
    

    protected Piece(EColor color, Position position)
    {
        Color = color;
        Position = position;
    }

    public abstract List<Position> GetPossibleMoves(View view, Piece piece);
    protected virtual (int x, int y) ChangeAdders(EColor color,int x, int y)
    {
        return (x, y); 
    }
}