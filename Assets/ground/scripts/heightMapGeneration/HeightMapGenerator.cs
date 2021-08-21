/// <summary>
///     HeightMapGenerator is a interface that declares required methods
/// </summary>
public interface HeightMapGenerator <T>
{
    /// <summary>
    ///     getHeightMap generates a Grid object that contains the nodes for a HeightMap
    /// </summary>
    /// <returns>
    ///     A grid with the HeightMap data
    /// </returns>
    T getHeightMap(object param);
}
