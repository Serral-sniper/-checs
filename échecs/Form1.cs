using échecs.Models;
using échecs.Views;
using View = échecs.Views.View;

namespace échecs;

public partial class Form1 : Form
{
    private readonly View _view;
    private readonly ChessBoardControl _chessBoard;
    private readonly Label _statusLabel;

    public Form1()
    {
        InitializeComponent();

        _view = new View();
        _chessBoard = new ChessBoardControl(_view)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };

        _statusLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 34,
            Text = "Sélectionne une pièce",
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(220, 220, 220),
            BackColor = Color.FromArgb(32, 35, 31),
            Font = new Font("Segoe UI", 10f)
        };

        _chessBoard.PieceSelected += ChessBoardOnPieceSelected;

        Controls.Add(_chessBoard);
        Controls.Add(_statusLabel);
    }

    private void ChessBoardOnPieceSelected(Position position1)
    {
        _statusLabel.Text = $"Pièce sélectionnée — X: {position1.X}, Y: {position1.Y}";
    }
}
