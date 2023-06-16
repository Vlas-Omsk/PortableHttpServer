using System.Collections.Immutable;

namespace PortableHttpServer.Models
{
    public sealed record HomeIndexViewModel(
        ImmutableArray<EntryModel> Entries
    );
}
