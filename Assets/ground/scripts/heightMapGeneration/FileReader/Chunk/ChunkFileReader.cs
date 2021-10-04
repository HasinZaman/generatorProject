using System;
using System.IO;

/// <summary>
///     ChunkFileReader turns a ".world" into a chunks
/// </summary>
/// <typeparam name="NF">Node Factory class/child class</typeparam>
/// <typeparam name="N">Node class/child class</typeparam>
/// <typeparam name="G">Grid class/child class</typeparam>
public class ChunkFileReader<NF, N, G> : FileReaderGenerator<NF, N, G, FileParam> where NF : NodeFactory<N> where N : Node where G : Grid
{
    /// <summary>
    ///     Constructor sets up ChunkFileReader object
    /// </summary>
    public ChunkFileReader() : base(".world", "worlds")
    {

    }

    /// <summary>
    ///     stringToHeightMap converts a string into a grid object
    /// </summary>
    /// <param name="gridRaw">String that will proccessed into a grid object</param>
    /// <returns>Grid object of gridRaw string</returns>
    private G stringToHeightMap(string gridRaw)
    {
        throw new ArgumentException();
    }

    /// <summary>
    ///     getHeightMap generates a Grid object that contains the nodes for a HeightMap
    /// </summary>
    /// <param name="param">FileParam/FileParam child object that stores data key in reading file</param>
    /// <returns>
    ///     A grid with the HeightMap data
    /// </returns>
    public override G getHeightMap(FileParam param)
    {
        if (!validFileCheck(param.fileName))
        {
            throw new ArgumentException("File Does not exist");
        }

        if(param.line < -1)
        {
            throw new ArgumentException("File Does not exist");
        }

        StreamReader file = new StreamReader(this.getFileAddress(param.fileName));
        string[] rawGrid = file.ReadToEnd().Split('\n');

        file.Close();

        if (param.line == -1)
        {
            param.line = 0;
        }

        if(param.line >= rawGrid.Length)
        {
            throw new ArgumentException($"param.line({param.line}) must be less than file lines({rawGrid.Length})");
        }

        return stringToHeightMap(rawGrid[param.line]);
    }
    }
}
