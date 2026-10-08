using échecs.Models;
using échecs.Views;

namespace échecs;

/// <summary>
/// VUE (fenêtre) : assemble les contrôles et implémente IChessView.
/// Aucune règle du jeu ici : elle relaie les clics et affiche ce qu'on lui dit.
/// </summary>
public partial class Form1 : Form, IChessView
{
    private readonly ChessBoardControl _chessBoard;
    private readonly Label _statusLabel;

    public event Action<Position>? SquareClicked;

    public Form1(Board board)
    {
        InitializeComponent();

        _chessBoard = new ChessBoardControl(board)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };

        _statusLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 34,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(220, 220, 220),
            BackColor = Color.FromArgb(32, 35, 31),
            Font = new Font("Segoe UI", 10f)
        };

        // On relaie simplement le clic vers le contrôleur.
        _chessBoard.SquareClicked += position => SquareClicked?.Invoke(position);

        Controls.Add(_chessBoard);
        Controls.Add(_statusLabel);
    }

    public void ShowSelection(Position? selected, IEnumerable<Position> moves, IEnumerable<Position> captures)
    {
        _chessBoard.ShowSelection(selected, moves, captures);
    }

    public void ShowStatus(string message)
    {
        _statusLabel.Text = message;
    }
}
