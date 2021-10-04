using System.IO;
/// <summary>
///     FileReaderGenerator abstract class handles the foundation of child class that creates chunks from a file
/// </summary>
/// <typeparam name="NF">NodeFactory class/child class</typeparam>
/// <typeparam name="N">Node class/child class</typeparam>
/// <typeparam name="G">Grid class/child class</typeparam>
/// <typeparam name="P">FileParam class/child class</typeparam>
public abstract class FileReaderGenerator<NF, N, G, P> : HeightMapGenerator<G, P> where NF : NodeFactory<N> where N : Node where G : Grid where P : FileParam
{
    /// <summary>
    ///     folder in which file is stored
    /// </summary>
    private string folder = "";

    /// <summary>
    ///     extension of file
    /// </summary>
    private string fileExtension = "";


    /// <summary>
    ///     constructor sets up FileReaderGenerator object
    /// </summary>
    /// <param name="fileExtension"></param>
    /// <param name="folder"></param>
    public FileReaderGenerator(string fileExtension, string folder)
    {
        this.fileExtension = fileExtension;
        this.folder = folder;
    }

    /// <summary>
    ///     validFileCheck determines if a file exists and has the correct file extensions
    /// </summary>
    /// <param name="file">string of the file that is being checked</param>
    /// <returns>bool of whether the file exists and is in the correct format</returns>
    protected bool validFileCheck(string file)
    {
        int index;

        //checking file extension is correct
        index = file.LastIndexOf('.');

        if (index == -1)
        {
            return false;
        }

        if (!file.Substring(index + 1).Equals(this.fileExtension))
        {
            return false;
        }

        //checks if file exists
        return File.Exists($"{folder}//{file}");
    }

    /// <summary>
    ///     getHeightMap generates a Grid object that contains the nodes for a HeightMap
    /// </summary>
    /// <param name="param">FileParam/FileParam child object that stores data key in reading file</param>
    /// <returns>
    ///     A grid with the HeightMap data
    /// </returns>
    public abstract G getHeightMap(FileParam param);
     
    public abstract G getHeightMap(P param);
}
