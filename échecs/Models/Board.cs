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
    
    public EColor CurrentTurn { get; private set; } = EColor.White;

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
        if(position is null)
            return false;
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
    public bool Move(Position from, Position to, EPromotionType promotionType = EPromotionType.Queen)
    {
        Piece? piece = GetCase(from);

        if (piece is null)
            return false;

        // Vérifier la légalité AVANT de modifier le plateau.
        if (!GetLegalMoves(from).Contains(to))
            return false;

        // Détecter la prise avant de supprimer les anciens droits.
        bool isEnPassant = IsEnPassantMove(from, to);

        // Un pion peut avoir avancé de deux cases.
        bool movedTwoSquares =
            piece is Pawn && Math.Abs(to.X - from.X) == 2;

        // Retirer le pion capturé en passant.
        if (isEnPassant)
        {
            Position capturedPosition = new Position(from.X, to.Y);
            _squares[capturedPosition.X, capturedPosition.Y] = null;
        }

        // Déplacer la pièce.
        _squares[from.X, from.Y] = null;
        _squares[to.X, to.Y] = piece;
        piece.MoveTo(to);
        
        // Promotion automatique en dame.
        if (piece is Pawn pawn && MustPromote(pawn, to))
        {
            PromotePawn(to, pawn, promotionType);
        }
        
        // Effacer tous les anciens droits.
        ResetEnPassantRights();

        // Accorder un nouveau droit si ce pion a avancé de deux cases.
        if (movedTwoSquares && piece is Pawn movedPawn)
        {
            movedPawn.CanBeCapturedEnPassant = true;
        }

        // Changer le joueur actif.
        CurrentTurn = CurrentTurn == EColor.White
            ? EColor.Black
            : EColor.White;

        Changed?.Invoke(this, EventArgs.Empty);

        return true;
    }
    /// <summary>
    /// Retourne la liste de positions possibles.
    /// Retourne une liste si une position ne respecte pas les règles.
    /// </summary>
    public List<Position> GetLegalMoves(Position from)
    {
        Piece? piece = GetCase(from);

        if (piece is null || piece.Color != CurrentTurn)
            return new List<Position>();

        return piece.GetPossibleMoves(this)
            .Where(to => IsLegalMove(from, to))
            .ToList();
    }

    private bool IsLegalMove(Position from, Position to)
    {
        Piece? piece = GetCase(from);

        if (piece is null || !IsInside(to))
            return false;

        Piece? targetPiece = GetCase(to);

        // Ne pas capturer une pièce de sa propre couleur.
        if (targetPiece is not null && targetPiece.Color == piece.Color)
            return false;

        // Détecter la prise en passant avant de modifier le plateau.
        bool isEnPassant = IsEnPassantMove(from, to);

        Position capturedPosition = isEnPassant
            ? new Position(from.X, to.Y)
            : to;

        Piece? capturedPiece = GetCase(capturedPosition);
        Position oldPosition = piece.Position;

        try
        {
            // Simuler le déplacement.
            _squares[from.X, from.Y] = null;
            _squares[to.X, to.Y] = piece;

            // En prise en passant, le pion capturé est ailleurs.
            if (isEnPassant)
            {
                _squares[capturedPosition.X, capturedPosition.Y] = null;
            }

            piece.SetPositionForSimulation(to);

            // Un coup est légal si le roi du joueur reste en sécurité.
            return !IsInCheck(piece.Color);
        }
        finally
        {
            // Restaurer exactement les cases modifiées.
            _squares[from.X, from.Y] = piece;
            _squares[to.X, to.Y] = targetPiece;

            if (isEnPassant)
            {
                _squares[capturedPosition.X, capturedPosition.Y] =
                    capturedPiece;
            }

            piece.SetPositionForSimulation(oldPosition);
        }
    }
    
    private bool IsEnPassantMove(Position from, Position to)
    {
        Piece? movingPiece = GetCase(from);
        Piece? targetPiece = GetCase(to);

        if (movingPiece is not Pawn pawn)
            return false;

        // La case d'arrivée doit être vide.
        if (targetPiece is not null)
            return false;

        // Vérifier que le pion se déplace d'une case en diagonale.
        if (Math.Abs(to.Y - from.Y) != 1)
            return false;

        // Vérifier que le pion adverse situé à côté
        // possède encore le droit d'être capturé en passant.
        Position adjacentPosition = new Position(from.X, to.Y);

        if (GetCase(adjacentPosition) is not Pawn adjacentPawn)
            return false;

        if (adjacentPawn.Color == pawn.Color)
            return false;

        return adjacentPawn.CanBeCapturedEnPassant;
    }

    private void ResetEnPassantRights()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (_squares[i, j] is Pawn pawn)
                {
                    pawn.CanBeCapturedEnPassant = false;
                }
            }
        }
    }
    public bool IsInCheck(EColor color)
    {
        Position? kingPosition = null;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (_squares[x, y] is King king
                    && king.Color == color)
                {
                    kingPosition = new Position(x, y);
                    break;
                }
            }

            if (kingPosition is not null)
                break;
        }

        // Une position sans roi est un état de plateau invalide.
        if (kingPosition is null)
            throw new InvalidOperationException(
                $"Aucun roi trouvé pour {color}.");

        EColor opponent = color == EColor.White
            ? EColor.Black
            : EColor.White;

        return IsSquareAttacked(kingPosition, opponent);
    }
    private bool IsSquareAttacked(Position target, EColor byColor)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece? piece = _squares[x, y];

                if (piece is null || piece.Color != byColor)
                    continue;

                int dx = target.X - piece.Position.X;
                int dy = target.Y - piece.Position.Y;

                // Le roi attaque les cases voisines.
                if (piece is King)
                {
                    if (Math.Max(Math.Abs(dx), Math.Abs(dy)) == 1)
                        return true;
                }
                // Le cavalier attaque en L.
                else if (piece is Knight)
                {
                    if (Math.Abs(dx) == 2 && Math.Abs(dy) == 1
                        || Math.Abs(dx) == 1 && Math.Abs(dy) == 2)
                        return true;
                }
                // Le pion attaque uniquement en diagonale.
                else if (piece is Pawn)
                {
                    int pawnDirection =
                        byColor == EColor.White ? -1 : 1;

                    if (dx == pawnDirection && Math.Abs(dy) == 1)
                        return true;
                }
                // Les tours, fous et dames attaquent sur leurs lignes.
                else if (piece is Rook || piece is Bishop || piece is Queen)
                {
                    bool straight = dx == 0 || dy == 0;
                    bool diagonal = Math.Abs(dx) == Math.Abs(dy);

                    bool canMoveInDirection =
                        piece is Queen && (straight || diagonal)
                        || piece is Rook && straight
                        || piece is Bishop && diagonal;

                    if (canMoveInDirection &&
                        IsPathClear(piece.Position, target))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    private bool IsPathClear(Position from, Position to)
    {
        int dx = Math.Sign(to.X - from.X);
        int dy = Math.Sign(to.Y - from.Y);

        int x = from.X + dx;
        int y = from.Y + dy;

        while (x != to.X || y != to.Y)
        {
            if (_squares[x, y] is not null)
                return false;

            x += dx;
            y += dy;
        }

        return true;
    }
    
    private bool MustPromote(Pawn pawn, Position destination)
    {
        return pawn.Color == EColor.White
            ? destination.X == 0
            : destination.X == 7;
    }
    
   
    private void PromotePawn(Position position, Pawn pawn, EPromotionType promotionType)
    {
        Piece promotedPiece = promotionType switch
        {
            EPromotionType.Queen => new Queen(pawn.Color, position),
            EPromotionType.Rook => new Rook(pawn.Color, position),
            EPromotionType.Bishop => new Bishop(pawn.Color, position),
            EPromotionType.Knight => new Knight(pawn.Color, position),
            _ => new Queen(pawn.Color, position)
        };

        _squares[position.X, position.Y] = promotedPiece;
    }

}


