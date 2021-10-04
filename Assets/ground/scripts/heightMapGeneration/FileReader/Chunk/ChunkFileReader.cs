using System;
using System.IO;

/// <summary>
///     ChunkFileReader turns a ".world" into a chunks
/// </summary>
public class ChunkFileReader<NF, N, G> : FileReaderGenerator<NF, N, G> where NF : NodeFactory<N> where N : Node where G : Grid
{
    public ChunkFileReader() : base(".world", "worlds")
    {

    }

    public override G getHeightMap(FileParam param)
    {
        throw new System.NotImplementedException();
    }
}
