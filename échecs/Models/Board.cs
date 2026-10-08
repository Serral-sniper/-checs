using échecs.Models.Pieces;

namespace échecs.Models;

/// <summary>
/// MODÈLE : l'échiquier et ses pièces.
/// Ne connaît ni les vues, ni les contrôleurs, ni WinForms.
/// Prévient les curieux (vues) via l'événement <see cref="Changed"/>.
/// </summary>
public class Board
{
    public const int Size = 8;

    private readonly Piece?[,] _squares = new Piece?[Size, Size];

    /// <summary>
    /// Déclenché à chaque modification de l'échiquier.
    /// Le modèle ne sait pas qui écoute : c'est le pattern Observer.
    /// </summary>
    public event EventHandler? Changed;

    public Board()
    {
        SetUp();
    }

    private void SetUp()
    {
        // Pièces principales (rangée 0 = noir, rangée 7 = blanc)
        for (int i = 0; i < Size; i++)
        {
            switch (i)
            {
                case 0:
                case 7:
                    _squares[0, i] = new Rook(EColor.Black, new Position(0, i));
                    _squares[7, i] = new Rook(EColor.White, new Position(7, i));
                    break;
                case 1:
                case 6:
                    _squares[0, i] = new Knight(EColor.Black, new Position(0, i));
                    _squares[7, i] = new Knight(EColor.White, new Position(7, i));
                    break;
                case 2:
                case 5:
                    _squares[0, i] = new Bishop(EColor.Black, new Position(0, i));
                    _squares[7, i] = new Bishop(EColor.White, new Position(7, i));
                    break;
                case 3:
                    _squares[0, i] = new Queen(EColor.Black, new Position(0, i));
                    _squares[7, i] = new Queen(EColor.White, new Position(7, i));
                    break;
                case 4:
                    _squares[0, i] = new King(EColor.Black, new Position(0, i));
                    _squares[7, i] = new King(EColor.White, new Position(7, i));
                    break;
            }
        }

        // Pions (rangée 1 = noir, rangée 6 = blanc)
        for (int i = 0; i < Size; i++)
        {
            _squares[1, i] = new Pawn(EColor.Black, new Position(1, i));
            _squares[6, i] = new Pawn(EColor.White, new Position(6, i));
        }
    }

    public static bool IsInside(Position position)
    {
        return position.X >= 0 && position.X < Size &&
               position.Y >= 0 && position.Y < Size;
    }

    public bool HasPiece(Position position)
    {
        return GetCase(position) is not null;
    }

    /// <summary>
    /// Pièce située sur la case, ou null si la case est vide ou hors de l'échiquier.
    /// </summary>
    public Piece? GetCase(Position position)
    {
        return IsInside(position) ? _squares[position.X, position.Y] : null;
    }

    /// <summary>
    /// Déplace (ou capture) : seul endroit où le plateau et Piece.Position changent,
    /// pour que les deux restent toujours synchronisés.
    /// </summary>
    public void Move(Position from, Position to)
    {
        Piece piece = GetCase(from)
                      ?? throw new InvalidOperationException("Aucune pièce sur la case de départ.");

        if (!IsInside(to))
            throw new ArgumentOutOfRangeException(nameof(to), "Case d'arrivée hors de l'échiquier.");

        _squares[to.X, to.Y] = piece;      // écrase une éventuelle pièce capturée
        _squares[from.X, from.Y] = null;
        piece.MoveTo(to);

        Changed?.Invoke(this, EventArgs.Empty);
    }
}
