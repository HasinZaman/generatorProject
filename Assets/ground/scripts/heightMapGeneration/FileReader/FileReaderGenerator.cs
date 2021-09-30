/// <summary>
///     FileReaderGenerator abstract class handles the foundation of child class that creates chunks from a file
/// </summary>
public abstract class FileReaderGenerator : HeightMapGenerator<Grid>
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
