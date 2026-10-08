using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using échecs.Models;
using échecs.Models.Pieces;

namespace échecs.Views;

/// <summary>
/// VUE : dessine l'échiquier et signale les clics.
/// Elle ne décide de rien : pas de sélection, pas de règles, pas de calcul de coups.
/// Elle lit le modèle (Board) pour dessiner et se redessine quand il change.
/// Ce qu'il faut surligner lui est dicté par le contrôleur via ShowSelection.
///
/// Convention du projet :
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
    
    private readonly Board _board;

    private Position? _selectedPosition;
    private readonly HashSet<(int X, int Y)> _movableSquares = new();
    private readonly HashSet<(int X, int Y)> _captureSquares = new();

    /// <summary>
    /// Déclenché quand l'utilisateur clique sur une case de l'échiquier.
    /// C'est le contrôleur qui décide quoi en faire.
    /// </summary>
    public event Action<Position>? SquareClicked;

    public ChessBoardControl(Board board)
    {
        _board = board ?? throw new ArgumentNullException(nameof(board));

        DoubleBuffered = true;
        ResizeRedraw = true;
        TabStop = true;
        BackColor = Color.FromArgb(32, 35, 31);
        Cursor = Cursors.Hand;

        MouseClick += OnMouseClick;
        _board.Changed += OnBoardChanged;   // Observer : le modèle prévient, la vue se redessine
    }

    private void OnBoardChanged(object? sender, EventArgs e)
    {
        Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _board.Changed -= OnBoardChanged;

        base.Dispose(disposing);
    }

    /// <summary>
    /// Affiche ce que le contrôleur demande :
    /// la case sélectionnée, les déplacements possibles (orange) et les captures (rouge).
    /// selected = null efface tout.
    /// </summary>
    public void ShowSelection(Position? selected, IEnumerable<Position> moves, IEnumerable<Position> captures)
    {
        _selectedPosition = selected;

        _movableSquares.Clear();
        foreach (Position position in moves)
        {
            if (Board.IsInside(position))
                _movableSquares.Add((position.X, position.Y));
        }

        _captureSquares.Clear();
        foreach (Position position in captures)
        {
            if (Board.IsInside(position))
                _captureSquares.Add((position.X, position.Y));
        }

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

                Piece? piece = _board.GetCase(new Position(row, col));

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

        if (TryGetBoardPosition(e.Location, out Position? position))
            SquareClicked?.Invoke(position);
    }

    private bool TryGetBoardPosition(Point mousePosition, [NotNullWhen(true)] out Position? position)
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
}
