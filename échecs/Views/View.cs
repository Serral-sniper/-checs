using échecs.Models;
using échecs.Models.Pieces;

namespace échecs.Views;

public class View
{
    private Piece[,] Board;

    public View()
    {
        SetBoard();
    }

    private void SetBoard()
    {
        Board = new Piece[8, 8];

        // Pièces principales (rangée 0 = noir, rangée 7 = blanc)
        for (int i = 0; i < 8; i++)
        {
            switch (i)
            {
                case 0:
                case 7:
                    Board[0, i] = new Rook(EColor.Black, new Position(0, i));
                    Board[7, i] = new Rook(EColor.White, new Position(7, i));
                    break;
                case 1:
                case 6:
                    Board[0, i] = new Knight(EColor.Black, new Position(0, i));
                    Board[7, i] = new Knight(EColor.White, new Position(7, i));
                    break;
                case 2:
                case 5:
                    Board[0, i] = new Bishop(EColor.Black, new Position(0, i));
                    Board[7, i] = new Bishop(EColor.White, new Position(7, i));
                    break;
                case 3:
                    Board[0, i] = new Queen(EColor.Black, new Position(0, i));
                    Board[7, i] = new Queen(EColor.White, new Position(7, i));
                    break;
                case 4:
                    Board[0, i] = new King(EColor.Black, new Position(0, i));
                    Board[7, i] = new King(EColor.White, new Position(7, i));
                    break;
            }
        }

        // Pions (rangée 1 = noir, rangée 6 = blanc)
        for (int i = 0; i < 8; i++)
        {
            Board[1, i] = new Pawn(EColor.Black, new Position(1, i));
            Board[6, i] = new Pawn(EColor.White, new Position(6, i));
        }
    }
    public bool HasPiece(Position position)
    {
        return Board[position.X, position.Y] is not null;
    }

    public Piece? GetCase(Position position)
    {
        return Board[position.X, position.Y];
    }

    public Piece[,] GetBoard()
    {
        return Board;
    }
}