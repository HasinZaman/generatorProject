/// <summary>
///     ChunkParam is paramater used to load .world files
/// </summary>
public class ChunkParam : FileParam
{
    /// <summary>
    ///     Coord struct stores x and y position of chunks
    /// </summary>
    public struct Coord
    {
        public int x;
        public int y;
    }
    
    public Coord start;
    public Coord end;

    /// <summary>
    ///     ChunkParam constructor sets up ChunkParam
    /// </summary>
    public ChunkParam()
    {
        start = new Coord();
        start.x = 0;
        start.y = 0;

        end = new Coord();
        end.x = 0;
        end.y = 0;
    }

}