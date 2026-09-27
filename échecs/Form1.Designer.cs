namespace échecs;

partial class Form1
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 900);
        MinimumSize = new Size(620, 680);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Échecs";
        BackColor = Color.FromArgb(32, 35, 31);
    }
}
