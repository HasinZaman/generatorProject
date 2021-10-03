/// <summary>
///     FileReaderGenerator abstract class handles the foundation of child class that creates chunks from a file
/// </summary>
/// <typeparam name="NF">NodeFactory class/child class</typeparam>
/// <typeparam name="N">Node class/child class</typeparam>
/// <typeparam name="G">Grid class/child class</typeparam>
public abstract class FileReaderGenerator<NF, N, G> : HeightMapGenerator<G, FileParam> where NF : NodeFactory<N> where N : Node where G : Grid
{
    private string folder = "";
    private string fileExtension = "";

    /// <summary>
    ///     fileName is the name of file that will be read from
    /// </summary>
    private string fileName;

    /// <summary>
    ///     constructor sets up FileReaderGenerator object
    /// </summary>
    /// <param name="fileName">name of the file that will be read</param>
    public FileReaderGenerator(string fileName, string fileExtension, string folder)
    {
        this.fileExtension = fileExtension;
        this.folder = folder;
    }

    public FileReaderGenerator(string fileExtension, string folder)
    {
        this.fileExtension = fileExtension;
        this.folder = folder;
    }

    protected abstract Node readNextNode();

    /// <summary>
    ///     getHeightMap generates a Grid object that contains the nodes for a HeightMap
    /// </summary>
    /// <returns>
    ///     A grid with the HeightMap data
    /// </returns>
    public abstract Grid getHeightMap(object param);
}
