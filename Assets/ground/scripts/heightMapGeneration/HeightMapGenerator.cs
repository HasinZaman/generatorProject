/// <summary>
///     HeightMapGenerator is a interface that declares required methods
/// </summary>
/// <typeparam name="G">Grid</typeparam>
/// <typeparam name="P">Param</typeparam>
public interface HeightMapGenerator <G, P> where G : Grid
{
    /// <summary>
    ///     getHeightMap generates a Grid object that contains the nodes for a HeightMap
    /// </summary>
    /// <returns>
    ///     A grid with the HeightMap data
    /// </returns>
    G getHeightMap(P param);
}
