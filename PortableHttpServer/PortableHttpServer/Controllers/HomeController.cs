using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using PortableHttpServer.Models;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace PortableHttpServer.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Config _config;

        public HomeController(
            ILogger<HomeController> logger,
            Config config
        )
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index(string? path)
        {
            if (path == null)
                return View(new IndexViewModel(
                    GetRootEntries().ToImmutableArray()
                ));

            if (!TryGetFullPath(path, out var absolutePath) ||
                !Directory.Exists(absolutePath))
                return NotFound();

            return View(new IndexViewModel(
                GetEntries(path, absolutePath).ToImmutableArray()
            ));
        }

        public IActionResult Download(string? path)
        {
            if (path == null)
                return NotFound();

            if (!TryGetFullPath(path, out path) ||
                !System.IO.File.Exists(path))
                return NotFound();

            if (!new FileExtensionContentTypeProvider()
                    .TryGetContentType(path, out var contentType))
                contentType = "application/octet-stream";

            return File(
                System.IO.File.OpenRead(path),
                contentType
            );
        }

        private bool TryGetFullPath(string path, [NotNullWhen(true)] out string? fullPath)
        {
            path = path.Replace('/', Path.DirectorySeparatorChar);

            foreach (var configPath in _config.Paths)
            {
                if (path.StartsWith(configPath.Name))
                {
                    fullPath = Path.Combine(configPath.ParentDirectory, path);
                    return true;
                }
            }

            fullPath = null;
            return false;
        }

        private IEnumerable<EntryModel> GetRootEntries()
        {
            return _config.Paths.Select(
                x => new EntryModel(
                    x.Name,
                    $"/{x.Name}",
                    EntryModelType.Directory
                )
            );
        }

        private static IEnumerable<EntryModel> GetEntries(string path, string absolutePath)
        {
            yield return new EntryModel(
                "..",
                $"/{string.Join('/', path.Split('/').SkipLast(1))}",
                EntryModelType.Directory
            );

            foreach (var directory in Directory.GetDirectories(absolutePath))
            {
                var name = Path.GetFileName(directory);

                yield return new EntryModel(
                    name,
                    $"/{path}/{name}",
                    EntryModelType.Directory
                );
            }

            foreach (var file in Directory.GetFiles(absolutePath))
            {
                var name = Path.GetFileName(file);

                yield return new EntryModel(
                    name,
                    $"/{path}/{name}",
                    EntryModelType.File
                );
            }
        }
    }
}