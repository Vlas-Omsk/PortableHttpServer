using System.Collections.Immutable;

namespace PortableHttpServer
{
    public sealed record Config(
        int Port,
        bool UseHttps,
        ImmutableArray<Entry> Entries,
        string FfmpegPath,
        int? VideoConvertMaxThreads
    );
}
