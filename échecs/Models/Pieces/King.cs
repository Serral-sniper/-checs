namespace échecs.Models.Pieces;

public class King : Piece
{
    public King(EColor color, Position position)
        : base(color, position)
    {
    }

    public override List<Position> GetPossibleMoves(Board board)
    {
        List<Position> possibleMoves = new();

        // Les 8 directions autour du roi.
        int[,] directions =
        {
            { -1, -1 }, { -1, 0 }, { -1, 1 },
            {  0, -1 },            {  0, 1 },
            {  1, -1 }, {  1, 0 }, {  1, 1 }
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dx = directions[i, 0];
            int dy = directions[i, 1];

            Position target = new Position(
                Position.X + dx,
                Position.Y + dy
            );

            // Ignorer les cases hors du plateau.
            if (!Board.IsInside(target))
                continue;

            Piece? pieceOnSquare = board.GetCase(target);

            // Une pièce alliée bloque le déplacement.
            if (pieceOnSquare is not null &&
                pieceOnSquare.Color == Color)
            {
                continue;
            }

            // Case vide ou pièce adverse.
            possibleMoves.Add(target);
        }

        return possibleMoves;
    }
}