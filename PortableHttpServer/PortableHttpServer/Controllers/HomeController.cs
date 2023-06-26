using Microsoft.AspNetCore.Mvc;
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

            _logger.LogInformation("Downloading {path}", fullPath);

            return File(
                System.IO.File.OpenRead(fullPath),
                FileUtils.GetContentType(fullPath),
                true
            );
        }

        private IEnumerable<EntryModel> GetRootEntries()
        {
            return _config.Entries.Select(
                x => new EntryModel(
                    x.Name,
                    $"{x.Name}",
                    null,
                    EntryModelType.Directory,
                    false
                )
            );
        }

        private static IEnumerable<EntryModel> GetEntries(string publicPath, string fullPath)
        {
            yield return new EntryModel(
                "..",
                $"{string.Join('/', publicPath.Split('/').SkipLast(1))}",
                null,
                EntryModelType.Directory,
                false
            );

            foreach (var directory in Directory.GetDirectories(fullPath))
            {
                var name = Path.GetFileName(directory);

                yield return new EntryModel(
                    name,
                    $"{publicPath}/{name}",
                    null,
                    EntryModelType.Directory,
                    false
                );
            }

            foreach (var file in Directory.GetFiles(fullPath))
            {
                var name = Path.GetFileName(file);

                yield return new EntryModel(
                    name,
                    $"{publicPath}/{name}",
                    new FileInfo(file).FormatBytes(),
                    EntryModelType.File,
                    FileUtils.IsVideoConvertible(name)
                );
            }
        }
    }
}