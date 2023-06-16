using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using PortableHttpServer.Models;
using PortableHttpServer.Services;
using System.Collections.Immutable;

namespace PortableHttpServer.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Config _config;
        private readonly LocatorService _locatorService;

        public HomeController(
            ILogger<HomeController> logger,
            Config config,
            LocatorService locatorService
        )
        {
            _logger = logger;
            _config = config;
            _locatorService = locatorService;
        }

        public IActionResult Index(string? publicPath)
        {
            if (publicPath == null)
                return View(new HomeIndexViewModel(
                    GetRootEntries().ToImmutableArray()
                ));

            if (!_locatorService.TryGetFullPath(publicPath, out var fullPath) ||
                !Directory.Exists(fullPath))
                return NotFound();

            return View(new HomeIndexViewModel(
                GetEntries(publicPath, fullPath).ToImmutableArray()
            ));
        }

        public IActionResult Download(string? publicPath)
        {
            if (publicPath == null)
                return NotFound();

            if (!_locatorService.TryGetFullPath(publicPath, out var fullPath) ||
                !System.IO.File.Exists(fullPath))
                return NotFound();

            if (!new FileExtensionContentTypeProvider()
                    .TryGetContentType(fullPath, out var contentType))
                contentType = "application/octet-stream";

            _logger.LogInformation("Downloading {path}", fullPath);

            return File(
                System.IO.File.OpenRead(fullPath),
                contentType,
                true
            );
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

        private static IEnumerable<EntryModel> GetEntries(string publicPath, string fullPath)
        {
            yield return new EntryModel(
                "..",
                $"/{string.Join('/', publicPath.Split('/').SkipLast(1))}",
                EntryModelType.Directory
            );

            foreach (var directory in Directory.GetDirectories(fullPath))
            {
                var name = Path.GetFileName(directory);

                yield return new EntryModel(
                    name,
                    $"/{publicPath}/{name}",
                    EntryModelType.Directory
                );
            }

            foreach (var file in Directory.GetFiles(fullPath))
            {
                var name = Path.GetFileName(file);

                yield return new EntryModel(
                    name,
                    $"/{publicPath}/{name}",
                    EntryModelType.File
                );
            }
        }
    }
}