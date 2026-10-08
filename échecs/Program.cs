using échecs.Controllers;
using échecs.Models;

namespace échecs;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        
        var board = new Board();                              // Modèle
        var form = new Form1(board);                          // Vue
        _ = new GameController(board, form);                  // Contrôleur (reste en vie via les abonnements aux événements)

        Application.Run(form);
    }
}
