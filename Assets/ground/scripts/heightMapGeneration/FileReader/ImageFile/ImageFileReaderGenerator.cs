using System;
using System.Drawing;

public class ImageFileReaderGenerator<NF, N> : FileReaderGenerator<NF, N, Grid, ChunkParam> where NF : NodeFactory.NodeFactory<N> where N : Node
{
    private NF nodeFactory;
    public ImageFileReaderGenerator(NF nodeFactory) : base("png", "worlds")
    {
        this.nodeFactory = nodeFactory;
    }

    public override Grid getHeightMap(ChunkParam param)
    {
        if (!validFileCheck($"{param.fileName}.{this.fileExtension}"))
        {
            throw new ArgumentException($"File \"{getFileAddress($"{param.fileName}.{this.fileExtension}")}\" Does not exist");
        }

        throw new System.NotImplementedException();
    }

    public override Grid[] getHeightMaps(ChunkParam param)
    {
        throw new System.NotImplementedException();
    }
}
