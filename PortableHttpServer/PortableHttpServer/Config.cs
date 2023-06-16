using System.Collections.Immutable;

namespace PortableHttpServer
{
    public sealed record Config(
        ImmutableArray<ConfigPath> Paths
    );
}
