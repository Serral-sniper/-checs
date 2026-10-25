using échecs.Models;
using échecs.Models.Pieces;
using échecs.Views;

namespace échecs.Controllers;

/// <summary>
/// CONTRÔLEUR : reçoit les actions de l'utilisateur (via la vue),
/// interroge / modifie le modèle, puis dit à la vue quoi afficher.
/// Il garde l'état d'interaction (quelle pièce est sélectionnée).
/// </summary>
public sealed class GameController
{
    private const string SelectPieceMessage = "Sélectionne une pièce";

    private readonly Board _board;
    private readonly IChessView _view;

    private Position? _selected;
    private List<Position> _possibleMoves = new();

    public GameController(Board board, IChessView view)
    {
        _board = board ?? throw new ArgumentNullException(nameof(board));
        _view = view ?? throw new ArgumentNullException(nameof(view));

        _view.SquareClicked += OnSquareClicked;
        _view.ShowStatus(SelectPieceMessage);
    }

    
    
    private void OnSquareClicked(Position position)
    {
        if (_selected is not null && _possibleMoves.Contains(position))
        {
            Position from = _selected;
            Piece? selectedPiece = _board.GetCase(from);
            Piece? targetPiece = _board.GetCase(position);

            // Sélectionner une autre pièce de sa propre couleur.
            if (selectedPiece is not null &&
                targetPiece is not null &&
                selectedPiece.Color == targetPiece.Color)
            {
                Select(position, targetPiece);
                return;
            }

            EPromotionType promotionType = EPromotionType.Queen;

            // Vérifier si le pion atteint la dernière rangée.
            if (selectedPiece is Pawn pawn)
            {
                bool mustPromote =
                    pawn.Color == EColor.White
                        ? position.X == 0
                        : position.X == 7;

                if (mustPromote)
                {
                    promotionType = _view.ChoosePromotion(pawn.Color);
                }
            }

            // Appliquer le déplacement avec le type de pièce choisi.
            _board.Move(from, position, promotionType);

            ClearSelection();
            return;
        }

        Piece? piece = _board.GetCase(position);

        if (piece is not null)
        {
            Select(position, piece);
            return;
        }

        ClearSelection();
    }



    private void Select(Position position, Piece piece)
    {
        _selected = position;
        _possibleMoves = _board.GetLegalMoves(position);

        // Capture = case occupée par une pièce adverse ; sinon simple déplacement.
        List<Position> captures = _possibleMoves
            .Where(p => _board.GetCase(p) is { } target && target.Color != piece.Color)
            .ToList();
        List<Position> moves = _possibleMoves
            .Except(captures)
            .Where(p => !_board.HasPiece(p))
            .ToList();

        _view.ShowSelection(position, moves, captures);
        _view.ShowStatus($"{captures.Count + moves.Count} déplacement(s) possible(s)");
    }

    private void ClearSelection()
    {
        _selected = null;
        _possibleMoves = new List<Position>();

        _view.ShowSelection(null, Enumerable.Empty<Position>(), Enumerable.Empty<Position>());
        _view.ShowStatus(SelectPieceMessage);
    }
}
