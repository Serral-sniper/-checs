using System.Drawing.Drawing2D;
using System.Drawing.Text;
using échecs.Models;
using échecs.Models.Pieces;
using View = échecs.Views.View;

namespace échecs.Controllers;

/// <summary>
/// Contrôle graphique de l'échiquier.
/// La logique des déplacements reste dans le Controller.
/// 
/// Convention du projet actuel :
/// Position.X = ligne (0..7)
/// Position.Y = colonne (0..7)
/// </summary>
public sealed class ChessBoardControl : Control
{
    private static readonly Color LightSquare = Color.FromArgb(238, 238, 210);
    private static readonly Color DarkSquare = Color.FromArgb(118, 150, 86);

    private static readonly Color SelectedSquare = Color.FromArgb(70, 190, 90);
    private static readonly Color MovableSquare = Color.FromArgb(235, 170, 55);
    private static readonly Color CaptureSquare = Color.FromArgb(210, 70, 65);

    private static readonly Color SelectedBorder = Color.FromArgb(35, 120, 50);
    private static readonly Color MovableBorder = Color.FromArgb(170, 105, 15);
    private static readonly Color CaptureBorder = Color.FromArgb(145, 35, 30);
    
    private readonly View _view;

    private Position? _selectedPosition;
    private readonly HashSet<(int X, int Y)> _movableSquares = new();
    private readonly HashSet<(int X, int Y)> _captureSquares = new();

    /// <summary>
    /// Déclenché lorsqu'une pièce est sélectionnée.
    /// Le Controller peut alors calculer les déplacements possibles.
    /// </summary>
    public event Action<Position>? PieceSelected;

    /// <summary>
    /// Déclenché lorsqu'une case est cliquée.
    /// Utile pour laisser le Controller décider quoi faire
    /// d'une case de déplacement ou de capture.
    /// </summary>
    public event Action<Position>? SquareClicked;

    public Position? SelectedPosition => _selectedPosition;

    public ChessBoardControl(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));

        DoubleBuffered = true;
        ResizeRedraw = true;
        TabStop = true;
        BackColor = Color.FromArgb(32, 35, 31);
        Cursor = Cursors.Hand;

        MouseClick += OnMouseClick;
    }

    /// <summary>
    /// Définit les cases où la pièce sélectionnée peut se déplacer.
    /// Ces cases sont affichées en orange.
    /// </summary>
    public void SetMovableSquares(IEnumerable<Position> positions)
    {
        _movableSquares.Clear();

        foreach (Position position in positions)
        {
            if (IsInsideBoard(position))
                _movableSquares.Add((position.X, position.Y));
        }

        Invalidate();
    }

    /// <summary>
    /// Définit les cases contenant une pièce adverse pouvant être capturée.
    /// Ces cases sont affichées en rouge.
    /// </summary>
    public void SetCaptureSquares(IEnumerable<Position> positions)
    {
        _captureSquares.Clear();

        foreach (Position position in positions)
        {
            if (IsInsideBoard(position))
                _captureSquares.Add((position.X, position.Y));
        }

        Invalidate();
    }

    /// <summary>
    /// Efface toutes les cases orange/rouges.
    /// </summary>
    public void ClearMoveHighlights()
    {
        _movableSquares.Clear();
        _captureSquares.Clear();
        Invalidate();
    }

    /// <summary>
    /// Sélectionne directement une position depuis le Controller.
    /// </summary>
    public void SelectPosition(Position? position)
    {
        _selectedPosition = position is not null && IsInsideBoard(position)
            ? new Position(position.X, position.Y)
            : null;

        Invalidate();
    }

    /// <summary>
    /// Efface la sélection et les déplacements affichés.
    /// </summary>
    public void ClearSelection()
    {
        _selectedPosition = null;
        _movableSquares.Clear();
        _captureSquares.Clear();
        Invalidate();
    }

    /// <summary>
    /// À appeler après une modification du View.Board.
    /// </summary>
    public void RefreshBoard()
    {
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        int boardSize = Math.Min(ClientSize.Width, ClientSize.Height);

        if (boardSize <= 0)
            return;

        int offsetX = (ClientSize.Width - boardSize) / 2;
        int offsetY = (ClientSize.Height - boardSize) / 2;
        float squareSize = boardSize / 8f;

        using var boardShadow = new SolidBrush(Color.FromArgb(55, 0, 0, 0));
        e.Graphics.FillRectangle(
            boardShadow,
            offsetX + 6,
            offsetY + 8,
            boardSize,
            boardSize);

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                RectangleF square = new(
                    offsetX + col * squareSize,
                    offsetY + row * squareSize,
                    squareSize + 0.5f,
                    squareSize + 0.5f);

                Color squareColor = GetSquareColor(row, col);

                using var brush = new SolidBrush(squareColor);
                e.Graphics.FillRectangle(brush, square);

                DrawHighlightBorder(e.Graphics, square, row, col);

                Piece? piece = _view.GetCase(new Position(row, col));

                if (piece is not null)
                    DrawPiece(e.Graphics, piece, square);
            }
        }
    }

    private Color GetSquareColor(int row, int col)
    {
        if (IsSelected(row, col))
            return SelectedSquare;

        if (_captureSquares.Contains((row, col)))
            return CaptureSquare;

        if (_movableSquares.Contains((row, col)))
            return MovableSquare;

        return (row + col) % 2 == 0
            ? LightSquare
            : DarkSquare;
    }

    private void DrawHighlightBorder(Graphics graphics, RectangleF square, int row, int col)
    {
        Color? borderColor = null;

        if (IsSelected(row, col))
            borderColor = SelectedBorder;
        else if (_captureSquares.Contains((row, col)))
            borderColor = CaptureBorder;
        else if (_movableSquares.Contains((row, col)))
            borderColor = MovableBorder;

        if (borderColor is null)
            return;

        using var border = new Pen(borderColor.Value, Math.Max(2, square.Width * 0.045f));

        graphics.DrawRectangle(
            border,
            square.X + 1,
            square.Y + 1,
            square.Width - 3,
            square.Height - 3);
    }

    private void DrawPiece(Graphics graphics, Piece piece, RectangleF square)
    {
        string glyph = piece switch
        {
            King => piece.Color == EColor.White ? "♔" : "♚",
            Queen => piece.Color == EColor.White ? "♕" : "♛",
            Rook => piece.Color == EColor.White ? "♖" : "♜",
            Bishop => piece.Color == EColor.White ? "♗" : "♝",
            Knight => piece.Color == EColor.White ? "♘" : "♞",
            Pawn => piece.Color == EColor.White ? "♙" : "♟",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(glyph))
            return;

        float fontSize = square.Height * 0.78f;

        using var font = new Font(
            "Segoe UI Symbol",
            fontSize,
            FontStyle.Regular,
            GraphicsUnit.Pixel);

        using var shadowBrush = new SolidBrush(Color.FromArgb(75, 0, 0, 0));

        using var pieceBrush = new SolidBrush(
            piece.Color == EColor.White
                ? Color.FromArgb(248, 248, 242)
                : Color.FromArgb(35, 35, 35));

        StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap
        };

        RectangleF shadowRect = new(
            square.X + square.Width * 0.025f,
            square.Y + square.Height * 0.035f,
            square.Width,
            square.Height);

        graphics.DrawString(glyph, font, shadowBrush, shadowRect, format);
        graphics.DrawString(glyph, font, pieceBrush, square, format);
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;

        if (!TryGetBoardPosition(e.Location, out Position? position))
            return;

        SquareClicked?.Invoke(position);

        Piece? piece = _view.GetCase(position);

        // Une pièce est cliquée : elle devient la pièce sélectionnée.
        if (piece is not null)
        {
            _selectedPosition = new Position(position.X, position.Y);

            // Les anciennes cases possibles appartenaient à l'ancienne sélection.
            _movableSquares.Clear();
            _captureSquares.Clear();

            PieceSelected?.Invoke(_selectedPosition);
            Invalidate();
            return;
        }

        // Clic sur une case vide : on garde la sélection.
        // Le Controller reçoit le clic via SquareClicked et décide
        // si cette case est un déplacement valide.
        Invalidate();
    }

    private bool TryGetBoardPosition(Point mousePosition, out Position? position)
    {
        int boardSize = Math.Min(ClientSize.Width, ClientSize.Height);

        if (boardSize <= 0)
        {
            position = null;
            return false;
        }

        int offsetX = (ClientSize.Width - boardSize) / 2;
        int offsetY = (ClientSize.Height - boardSize) / 2;

        if (mousePosition.X < offsetX ||
            mousePosition.Y < offsetY ||
            mousePosition.X >= offsetX + boardSize ||
            mousePosition.Y >= offsetY + boardSize)
        {
            position = null;
            return false;
        }

        float squareSize = boardSize / 8f;

        int col = Math.Clamp(
            (int)((mousePosition.X - offsetX) / squareSize),
            0,
            7);

        int row = Math.Clamp(
            (int)((mousePosition.Y - offsetY) / squareSize),
            0,
            7);

        position = new Position(row, col);
        return true;
    }

    private bool IsSelected(int row, int col)
    {
        return _selectedPosition is not null &&
               _selectedPosition.X == row &&
               _selectedPosition.Y == col;
    }

    private static bool IsInsideBoard(Position position)
    {
        return position.X >= 0 &&
               position.X < 8 &&
               position.Y >= 0 &&
               position.Y < 8;
    }
}
