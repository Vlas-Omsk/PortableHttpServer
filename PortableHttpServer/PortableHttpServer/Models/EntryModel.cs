namespace PortableHttpServer.Models
{
    public sealed record EntryModel(
        string Name,
        string Url,
        EntryModelType Type
    );

    public enum EntryModelType
    {
        File,
        Directory
    }
}
