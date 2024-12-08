namespace FuseDrill.Core;

public class FileParameter
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public Stream Data { get; set; }

    public FileParameter(Stream content, string fileName, string contentType)
    {
        Data = content;
        FileName = fileName;
        ContentType = contentType;
    }

    public static FileParameter CreateMockedFile()
    {
        // Define hardcoded dependencies
        string content = "This is mocked txt file";
        string fileName = "mocked.txt";
        string contentType = "text/plain";

        // Create a memory stream with the content
        MemoryStream stream = new MemoryStream();
        using (StreamWriter writer = new StreamWriter(stream, leaveOpen: true))
        {
            writer.Write(content);
            writer.Flush();
        }
        stream.Position = 0; // Reset position for reading

        // Return the FileParameter instance
        return new FileParameter(stream, fileName, contentType);
    }

    public static FileParameter FromStream(Stream content, string fileName, string contentType = null)
    {
        return new FileParameter(content, fileName, contentType);
    }
}
