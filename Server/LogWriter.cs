using System.IO;
using System.Runtime.InteropServices.JavaScript;

namespace Server;

public class LogWriter
{
    private const string FilePath = @"C:\Users\pauls\Documents\GitHub\puvsChatProject\Server\Log.txt";

    private object log = new Object();

    public void WriteLogLine(string line)
    {
        using var writer = new StreamWriter(FilePath, true);
            writer.WriteLineAsync(line);
            Console.WriteLine(line);
    }

    public async void ClearLogFile()
    {
        await File.WriteAllTextAsync(FilePath, string.Empty);
    }
}