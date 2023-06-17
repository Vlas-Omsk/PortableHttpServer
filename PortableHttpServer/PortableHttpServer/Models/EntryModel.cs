namespace PortableHttpServer.Models
{
    public sealed record EntryModel(
        string Name,
        string Url,
        string? Size,
        EntryModelType Type,
        bool IsConvertible
    );

    public enum EntryModelType
    {
        File,
        Directory
    }
}
