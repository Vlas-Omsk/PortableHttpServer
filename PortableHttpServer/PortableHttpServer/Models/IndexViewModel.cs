using System.Collections.Immutable;

namespace PortableHttpServer.Models
{
    public sealed record IndexViewModel(
        ImmutableArray<EntryModel> Entries
    );
}
