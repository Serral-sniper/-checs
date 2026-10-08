namespace échecs.Models;

/// <summary>
/// Coordonnée sur l'échiquier.
/// X = ligne (0..7), Y = colonne (0..7).
/// Un record offre l'égalité par valeur : new Position(1, 1) == new Position(1, 1).
/// </summary>
public record Position(int X, int Y)
{
    /// <summary>
    /// Addition d'une Position avec un tuple de 2 entiers.
    /// </summary>
    public static Position operator +(Position position, (int x, int y) adders)
    {
        return new Position(position.X + adders.x, position.Y + adders.y);
    }
}
