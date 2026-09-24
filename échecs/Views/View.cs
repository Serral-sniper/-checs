using échecs.Models;
using échecs.Models.Pieces;

namespace échecs.Views;

public class View
{
    public Piece[,] Board;

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
                    Board[0, i] = new Rook(EColor.black, new Position(0, i));
                    Board[7, i] = new Rook(EColor.white, new Position(7, i));
                    break;
                case 1:
                case 6:
                    Board[0, i] = new Knight(EColor.black, new Position(0, i));
                    Board[7, i] = new Knight(EColor.white, new Position(7, i));
                    break;
                case 2:
                case 5:
                    Board[0, i] = new Bishop(EColor.black, new Position(0, i));
                    Board[7, i] = new Bishop(EColor.white, new Position(7, i));
                    break;
                case 3:
                    Board[0, i] = new Queen(EColor.black, new Position(0, i));
                    Board[7, i] = new Queen(EColor.white, new Position(7, i));
                    break;
                case 4:
                    Board[0, i] = new King(EColor.black, new Position(0, i));
                    Board[7, i] = new King(EColor.white, new Position(7, i));
                    break;
            }
        }

        // Pions (rangée 1 = noir, rangée 6 = blanc)
        for (int i = 0; i < 8; i++)
        {
            Board[1, i] = new Pawn(EColor.black, new Position(1, i));
            Board[6, i] = new Pawn(EColor.white, new Position(6, i));
        }
    }
}