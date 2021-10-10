using System.IO;

/// <summary>
///     FileWriter creates a file
/// </summary>
public class FileWriter
{
    /// <summary>
    ///     static method that creats a file
    /// </summary>
    /// <param name="name">string of file name</param>
    /// <param name="extension">string of file extension</param>
    /// <param name="directory">string of directory the file is location</param>
    /// <param name="content">String of the contents of file</param>
    public static void createFile(string name, string extension, string directory, string content)
    {
        File.WriteAllText($"{directory}\\{name}.{extension}", content);
    }

    /// <summary>
    ///     static method that creats a file
    /// </summary>
    /// <param name="name">string of file name</param>
    /// <param name="extension">string of file extension</param>
    /// <param name="content">String of the contents of file</param>
    public static void createFile(string name, string extension, string content)
    {
        File.WriteAllText($"{name}.{extension}", content);
    }
}
