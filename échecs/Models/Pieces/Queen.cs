namespace échecs.Models.Pieces;

public class Queen : Piece
{
    public Queen(EColor color, Position position) : base(color, position)
    {
    }

    public override List<Position> GetPossibleMoves(Board board)
    {
        List<Position> possibleMoves = new();

        // Les 8 directions : horizontales, verticales et diagonales.
        int[,] directions =
        {
            { -1, -1 }, { -1,  0 }, { -1,  1 },
            {  0, -1 },             {  0,  1 },
            {  1, -1 }, {  1,  0 }, {  1,  1 }
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dx = directions[i, 0];
            int dy = directions[i, 1];

            Position current = new Position(
                Position.X + dx,
                Position.Y + dy
            );

            while (Board.IsInside(current))
            {
                Piece? pieceOnSquare = board.GetCase(current);

                // Case vide : la dame peut continuer.
                if (pieceOnSquare is null)
                {
                    possibleMoves.Add(current);
                }
                else
                {
                    // Pièce adverse : la dame peut la capturer.
                    if (pieceOnSquare.Color != Color)
                        possibleMoves.Add(current);

                    // Une pièce bloque toujours le passage.
                    break;
                }

                current = new Position(
                    current.X + dx,
                    current.Y + dy
                );
            }
        }

        return possibleMoves;
    }
}
