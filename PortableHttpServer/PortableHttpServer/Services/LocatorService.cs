using System.Diagnostics.CodeAnalysis;

namespace PortableHttpServer.Services
{
    public sealed class LocatorService
    {
        private readonly Config _config;

        public LocatorService(Config config)
        {
            _config = config;
        }

        public bool TryGetFullPath(string publicPath, [NotNullWhen(true)] out string? fullPath)
        {
            publicPath = publicPath.Replace('/', Path.DirectorySeparatorChar);

            foreach (var configPath in _config.Entries)
            {
                if (publicPath.StartsWith(configPath.Name))
                {
                    fullPath = Path.Combine(configPath.ParentDirectory, publicPath);
                    return true;
                }
            }

            fullPath = null;
            return false;
        }
    }
}
