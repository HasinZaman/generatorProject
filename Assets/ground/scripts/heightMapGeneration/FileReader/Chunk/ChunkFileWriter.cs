using System;
using System.IO;
using System.Linq;

/// <summary>
///     ChunkWriter class creates world files
/// </summary>
public class ChunkFileWriter
{
    /// <summary>
    ///     Array of illegalChar that cannot be in worldName
    /// </summary>
    private static string[] illegalChar = new string[] { "#", "%", "&", "{", "}", "/", "$", "!", "\'", "\"", ":", "@", "<", ">", "*", "?", "/", "+", "`", "|", "="};

    /// <summary>
    ///     writes an array of chunks in worldName Folder
    /// </summary>
    /// <param name="chunks">Array of Chunk object to write</param>
    /// <param name="worldName">name of world</param>
    public static void write(Chunk[] chunks, string worldName)
    {
        for (int i1 = 0; i1 < chunks.Length; i1++)
        {
            ChunkFileWriter.write(chunks[i1], worldName);
        }
    }

    /// <summary>
    ///     writes a array of chunks in worldName Folder
    /// </summary>
    /// <param name="chunk">Chunk object to write</param>
    /// <param name="worldName">name of world</param>
    public static void write(Chunk chunk, string worldName)
    {
        if (chunk == null)
        {
            throw new NullReferenceException("chunk cannot be null");
        }

        if (ChunkFileWriter.worldNameCheck(worldName))
        {
            throw new NullReferenceException($"Illegal Char({string.Concat(chunk.transform.name.TakeWhile((c, i) => c == string.Join(",", illegalChar)[i]))}) in worldName({worldName})");
        }

        if (ChunkFileWriter.worldNameCheck(chunk.transform.name))
        {
            throw new NullReferenceException($"Illegal Char({string.Concat(chunk.transform.name.TakeWhile((c, i) => c == string.Join(",", illegalChar)[i]))}) in chunk({chunk.transform.name})");
        }
        if(!Directory.Exists($"worlds/{worldName}"))
        {
            Directory.CreateDirectory($"worlds/{worldName}");
        }
        File.WriteAllText($"worlds/{worldName}/C{chunk.transform.name}.chunk", chunk.repr());
    }

    private static bool worldNameCheck(string worldName)
    {
        for(int i1 = 0; i1 < ChunkFileWriter.illegalChar.Length; i1++)
        {
            if(worldName.Contains(ChunkFileWriter.illegalChar[i1]))
            {
                return true;
            }
        }
        return false;
    }
}
