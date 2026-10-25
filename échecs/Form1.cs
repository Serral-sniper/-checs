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
    
    

public EPromotionType ChoosePromotion(EColor color)
{
    int boardWidth = _chessBoard.ClientSize.Width;
    int boardHeight = _chessBoard.ClientSize.Height;

    // Dimensions de la fenêtre par rapport à l'échiquier.
    int dialogWidth = Math.Max(440, (int)(boardWidth * 0.65));
    int dialogHeight = Math.Max(230, (int)(boardHeight * 0.50));

    using Form dialog = new Form
    {
        Text = "Promotion du pion",
        ClientSize = new Size(dialogWidth, dialogHeight),
        FormBorderStyle = FormBorderStyle.FixedDialog,
        StartPosition = FormStartPosition.CenterParent,
        MaximizeBox = false,
        MinimizeBox = false,
        ShowInTaskbar = false,
        BackColor = Color.FromArgb(32, 35, 31),
        ForeColor = Color.FromArgb(240, 240, 230),
        Font = new Font("Segoe UI", 11f)
    };

    Label title = new Label
    {
        Text = "Choisissez votre nouvelle pièce",
        Dock = DockStyle.Top,
        Height = 65,
        TextAlign = ContentAlignment.MiddleCenter,
        Font = new Font("Segoe UI", 16f, FontStyle.Bold),
        ForeColor = Color.FromArgb(240, 240, 230)
    };

    FlowLayoutPanel choices = new FlowLayoutPanel
    {
        Dock = DockStyle.Fill,
        Padding = new Padding(12),
        FlowDirection = FlowDirection.LeftToRight,
        WrapContents = false,
        AutoScroll = true,
        BackColor = dialog.BackColor
    };

    EPromotionType[] types =
    {
        EPromotionType.Queen,
        EPromotionType.Rook,
        EPromotionType.Bishop,
        EPromotionType.Knight
    };

    string[] names = { "Dame", "Tour", "Fou", "Cavalier" };
    string[] whiteSymbols = { "♕", "♖", "♗", "♘" };
    string[] blackSymbols = { "♛", "♜", "♝", "♞" };

    int buttonWidth = Math.Max(90, (dialogWidth - 60) / 4);
    int buttonHeight = Math.Max(110, dialogHeight - 100);

    for (int i = 0; i < types.Length; i++)
    {
        EPromotionType selectedType = types[i];

        Button button = new Button
        {
            Size = new Size(buttonWidth, buttonHeight),
            Margin = new Padding(3),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(238, 238, 210),
            ForeColor = Color.FromArgb(35, 35, 35),
            Font = new Font("Segoe UI Symbol", 14f),
            Text =
                (color == EColor.White
                    ? whiteSymbols[i]
                    : blackSymbols[i])
                + Environment.NewLine
                + Environment.NewLine
                + names[i],
            Cursor = Cursors.Hand,
            Tag = selectedType,
            UseVisualStyleBackColor = false
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor =
            Color.FromArgb(205, 220, 170);

        button.Click += (_, _) =>
        {
            dialog.Tag = selectedType;
            dialog.DialogResult = DialogResult.OK;
            dialog.Close();
        };

        choices.Controls.Add(button);
    }

    dialog.Controls.Add(choices);
    dialog.Controls.Add(title);

    return dialog.ShowDialog(this) == DialogResult.OK
        && dialog.Tag is EPromotionType result
            ? result
            : EPromotionType.Queen;
}


}
