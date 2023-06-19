

namespace Server;

/// <summary>
/// Writes lines into log.txt or clears the log-file
/// </summary>
public class LogWriter
{
    private static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Log.txt");

    private readonly object log = new();
    
    /// <summary>
    /// Writes the given line into the log-file and the console
    /// </summary>
    /// <param name="line">string - the content of this log message</param>
    public void WriteLogLine(string line)
    {
        lock (this.log)
        {
            using var writer = new StreamWriter(FilePath, true);
            writer.WriteLineAsync(line);
            Console.WriteLine(line);
        }
    }

    /// <summary>
    /// Empties log.txt
    /// </summary>
    public async void ClearLogFile()
    {
        await File.WriteAllTextAsync(FilePath, string.Empty);
    }
}