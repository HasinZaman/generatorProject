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
    public override G getHeightMap(FileParam param)
    {
        throw new System.NotImplementedException();
    }
}
