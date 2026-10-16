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
        // 1. Une pièce est sélectionnée et la case cliquée est un déplacement
        //    (ou une capture) possible : on joue le coup dans le modèle.
        Piece? piece;
        if (_selected is not null && _possibleMoves.Contains(position))
        {
            if (_board.GetCase(position) is not null && _board.GetCase(_selected).Color == _board.GetCase(position).Color)
            {
                piece = _board.GetCase(position);
                Select(position, piece);
                return;
            }
            _board.Move(_selected, position);   // le modèle notifie la vue via Board.Changed
            ClearSelection();
            return;
        }

        // 2. Sinon, si la case contient une pièce : on la sélectionne.
        piece = _board.GetCase(position);
        if (piece is not null)
        {
            Select(position, piece);
            return;
        }

        // 3. Case vide qui n'est pas un coup valide : on désélectionne.
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
