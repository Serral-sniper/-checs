using échecs.Models;

namespace échecs.Views;

/// <summary>
/// Ce que le contrôleur a le droit de savoir de la vue.
/// Le contrôleur dépend de cette interface, pas de Form1 ni de WinForms :
/// on pourrait le tester avec une fausse vue.
/// </summary>
public interface IChessView
{
    /// <summary>
    /// L'utilisateur a cliqué sur une case. La vue ne décide de rien : elle signale.
    /// </summary>
    event Action<Position>? SquareClicked;

    /// <summary>
    /// Affiche la sélection courante : case sélectionnée (orange/vert), cases de
    /// déplacement et cases de capture. selected = null efface tout.
    /// </summary>
    void ShowSelection(Position? selected, IEnumerable<Position> moves, IEnumerable<Position> captures);

    void ShowStatus(string message);
}
