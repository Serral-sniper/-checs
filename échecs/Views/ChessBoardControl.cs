using échecs.Models;
using échecs.Models.Pieces;

namespace échecs.Views;

public sealed class ChessBoardControl : Control
{
    private static readonly Color LightSquare = Color.FromArgb(238, 238, 210);
    private static readonly Color DarkSquare = Color.FromArgb(118, 150, 86);
    private static readonly Color SelectedSquare = Color.FromArgb(246, 194, 62);
    private static readonly Color SelectedBorder = Color.FromArgb(180, 125, 20);

    private readonly View _view;
    private Position? _selectedPosition;

    public event Action<Position>? PieceSelected;

    public Position? SelectedPosition => _selectedPosition;

    public ChessBoardControl(View view)
    {
        _view = view;

        DoubleBuffered = true;
        ResizeRedraw = true;
        TabStop = true;
        BackColor = Color.FromArgb(32, 35, 31);
        Cursor = Cursors.Hand;

        MouseClick += OnMouseClick;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        int boardSize = Math.Min(ClientSize.Width, ClientSize.Height);
        int offsetX = (ClientSize.Width - boardSize) / 2;
        int offsetY = (ClientSize.Height - boardSize) / 2;
        float squareSize = boardSize / 8f;

        using var boardShadow = new SolidBrush(Color.FromArgb(55, 0, 0, 0));
        e.Graphics.FillRectangle(boardShadow, offsetX + 6, offsetY + 8, boardSize, boardSize);

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                RectangleF square = new(
                    offsetX + col * squareSize,
                    offsetY + row * squareSize,
                    squareSize + 0.5f,
                    squareSize + 0.5f);

                bool isSelected = _selectedPosition?.X == row && _selectedPosition?.Y == col;
                Color squareColor = isSelected
                    ? SelectedSquare
                    : ((row + col) % 2 == 0 ? LightSquare : DarkSquare);

                using var brush = new SolidBrush(squareColor);
                e.Graphics.FillRectangle(brush, square);

                if (isSelected)
                {
                    using var border = new Pen(SelectedBorder, Math.Max(2, squareSize * 0.045f));
                    e.Graphics.DrawRectangle(border, square.X + 1, square.Y + 1, square.Width - 3, square.Height - 3);
                }

                Piece? piece = _view.Board[row, col];
                if (piece != null)
                    DrawPiece(e.Graphics, piece, square);
            }
        }
    }

    private void DrawPiece(Graphics graphics, Piece piece, RectangleF square)
    {
        string glyph = piece switch
        {
            King => pieceColor(piece) ? "♔" : "♚",
            Queen => pieceColor(piece) ? "♕" : "♛",
            Rook => pieceColor(piece) ? "♖" : "♜",
            Bishop => pieceColor(piece) ? "♗" : "♝",
            Knight => pieceColor(piece) ? "♘" : "♞",
            Pawn => pieceColor(piece) ? "♙" : "♟",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(glyph))
            return;

        float fontSize = square.Height * 0.78f;
        using var font = new Font("Segoe UI Symbol", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

        using var shadowBrush = new SolidBrush(Color.FromArgb(75, 0, 0, 0));
        using var pieceBrush = new SolidBrush(pieceColor(piece)
            ? Color.FromArgb(248, 248, 242)
            : Color.FromArgb(35, 35, 35));

        StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap
        };

        RectangleF shadowRect = new(square.X + square.Width * 0.025f,
            square.Y + square.Height * 0.035f,
            square.Width,
            square.Height);

        graphics.DrawString(glyph, font, shadowBrush, shadowRect, format);
        graphics.DrawString(glyph, font, pieceBrush, square, format);
    }

    private static bool pieceColor(Piece piece)
    {
        return piece.Color is EColor.white;
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;

        int boardSize = Math.Min(ClientSize.Width, ClientSize.Height);
        int offsetX = (ClientSize.Width - boardSize) / 2;
        int offsetY = (ClientSize.Height - boardSize) / 2;

        if (e.X < offsetX || e.Y < offsetY || e.X >= offsetX + boardSize || e.Y >= offsetY + boardSize)
            return;

        int col = Math.Clamp((int)((e.X - offsetX) / (boardSize / 8f)), 0, 7);
        int row = Math.Clamp((int)((e.Y - offsetY) / (boardSize / 8f)), 0, 7);

        Piece? piece = _view.Board[row, col];

        if (piece == null)
        {
            _selectedPosition = null;
        }
        else
        {
            _selectedPosition = new Position(row, col);
            PieceSelected?.Invoke(_selectedPosition);
        }

        Invalidate();
    }

    public void RefreshBoard()
    {
        Invalidate();
    }
}
