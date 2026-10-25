namespace échecs.Models.Pieces;

public class Rook : Piece
{
    public Rook(EColor color, Position position)
        : base(color, position)
    {
    }

    public override List<Position> GetPossibleMoves(Board board)
    {
        List<Position> possibleMoves = new();

        // Les 4 directions de la tour :
        // haut, bas, gauche et droite.
        int[,] directions =
        {
            { -1, 0 },
            {  1, 0 },
            {  0,-1 },
            {  0, 1 }
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dx = directions[i, 0];
            int dy = directions[i, 1];

            Position target = new Position(
                Position.X + dx,
                Position.Y + dy
            );

            while (Board.IsInside(target))
            {
                Piece? pieceOnSquare = board.GetCase(target);

                if (pieceOnSquare is null)
                {
                    // La case est libre : on peut continuer.
                    possibleMoves.Add(target);
                }
                else
                {
                    // On peut capturer une pièce adverse.
                    if (pieceOnSquare.Color != Color)
                        possibleMoves.Add(target);

                    // Toute pièce bloque le passage.
                    break;
                }

                target = new Position(
                    target.X + dx,
                    target.Y + dy
                );
            }
        }

        return possibleMoves;
    }
}