using System;
using System.IO;

/// <summary>
///     ChunkFileReader turns a ".world" into a chunks
/// </summary>
/// <typeparam name="NF">Node Factory class/child class</typeparam>
/// <typeparam name="N">Node class/child class</typeparam>
/// <typeparam name="G">Grid class/child class</typeparam>
public class ChunkFileReader<NF, N> : FileReaderGenerator<NF, N, Grid, ChunkParam> where NF : NodeFactory.NodeFactory<N> where N : Node
{

    private string saveName = null;

    private NF nodeFactory;

    /// <summary>
    ///     Constructor sets up ChunkFileReader object
    /// </summary>
    public ChunkFileReader(NF nodeFactory) : base("chunk", "worlds")
    {
        this.nodeFactory = nodeFactory;
    }

    /// <summary>
    ///     Constructor sets up ChunkFileReader object
    /// </summary>
    /// <param name="saveName">name of save file</param>
    public ChunkFileReader(NF nodeFactory, string saveName) : base("chunk", "worlds")
    {
        this.nodeFactory = nodeFactory;
        this.saveName = saveName;
    }

    /// <summary>
    ///     setSave method assign value to saveName
    /// </summary>
    /// <param name="saveName">name of save file</param>
    public void setSave(string saveName)
    {
        this.saveName = saveName;
    }

    /// <summary>
    ///     private utility method converts a string into a int array
    /// </summary>
    /// <param name="str">string that is converted into int array</param>
    /// <returns>int array of string input</returns>
    private int[] stringToIntArray(string str)
    {
        string[] tmp1 = str.Split(',');
        int[] tmp2 = new int[tmp1.Length];
        for (int i1 = 0; i1 < tmp2.Length; i1++)
        {
            tmp2[i1] = int.Parse(tmp1[i1]);
        }

        return tmp2;
    }

    /// <summary>
    ///     getHeightMap generates a Grid object that contains the nodes for a HeightMap
    /// </summary>
    /// <param name="param">FileParam/FileParam child object that stores data key in reading file</param>
    /// <returns>
    ///     A grid with the HeightMap data
    /// </returns>
    public override Grid getHeightMap(ChunkParam param)
    {
        if (!validFileCheck($"{this.saveName}\\{param.fileName}.{this.fileExtension}"))
        {
            throw new ArgumentException("File Does not exist");
        }

        string strTmp = File.ReadAllText($"{this.folder}\\{this.saveName}\\{param.fileName}.{this.fileExtension}");

        string[] tmp;
        string[] rawNodes;
        int[] dim, pos;

        Node[] nodes;

        tmp = strTmp.Split('|');

        //assigning dim
        dim = stringToInt(tmp[0]);
        dim = stringToIntArray(tmp[0]);
        pos = new int[2] { 0, 0 };

        nodes = new N[tmp.Length - 1];

        for (int i1 = 1; i1 < tmp.Length; i1++)
        {
            rawNodes = tmp[i1].Split(',');
            for (int i2 = 0; i2 < rawNodes.Length; i2++)
            {
                nodes[pos[0] + dim[0] * (pos[1] + dim[1] * i2)] = (Node)nodeFactory.create(rawNodes[i2]);
            }

            pos[1] = (pos[1] + (pos[0] + 1) / dim[1]) % dim[1];
            pos[0] = (pos[0] + 1) % dim[0];
        }

        return new Grid(nodes, dim);
    }

    public override Grid[] getHeightMaps(ChunkParam param)
    {
        throw new NotImplementedException();
    }
}
