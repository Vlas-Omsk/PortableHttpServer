using Microsoft.AspNetCore.Mvc;
using PortableHttpServer.Ffmpeg;
using PortableHttpServer.Models;
using PortableHttpServer.Services;

namespace PortableHttpServer.Controllers
{
    public sealed class ConvertController : Controller
    {
        private readonly ILogger<ConvertController> _logger;
        private readonly Config _config;
        private readonly FfmpegProcessor _ffmpegProcessor;
        private readonly LocatorService _locatorService;

        public ConvertController(
            ILogger<ConvertController> logger,
            Config config,
            FfmpegProcessor ffmpegProcessor,
            LocatorService locatorService
        )
        {
            _logger = logger;
            _config = config;
            _ffmpegProcessor = ffmpegProcessor;
            _locatorService = locatorService;
        }

        public IActionResult Index(string? publicPath)
        {
            if (publicPath == null)
                return NotFound();

            return View(new ConvertIndexViewModel(publicPath));
        }

        public IActionResult Download(string? publicPath, ConvertOptionsModel options)
        {
            if (publicPath == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_locatorService.TryGetFullPath(publicPath, out var fullPath) ||
                !System.IO.File.Exists(fullPath))
                return NotFound();

            var arguments = new FfmpegArguments()
                .Add("hwaccel", "auto")
                //.Add("threads", "2")
                .Add("i", fullPath)
                .Add("f", options.Output);

            if (!string.IsNullOrWhiteSpace(options.Size))
            {
                var split = options.Size.Split("x");

                if (split.Length != 2 ||
                    !int.TryParse(split[0], out var width) ||
                    !int.TryParse(split[1], out var height))
                    return Content("Invalid size syntax");

                arguments.Add("s", $"{width}x{height}");
            }

            if (options.VideoBitrate.HasValue)
                arguments.Add("b:v", options.VideoBitrate.Value.ToString());

            if (options.AudioBitrate.HasValue)
                arguments.Add("b:a", options.AudioBitrate.Value.ToString());

            if (options.Crf.HasValue)
                arguments.Add("crf", options.Crf.Value.ToString());

            if (!string.IsNullOrWhiteSpace(options.Cut))
            {
                var split = options.Cut.Split("-");

                if (split.Length != 2 ||
                    split[0].Split(":").Length != 3 ||
                    split[1].Split(":").Length != 3)
                    return Content("Invalid cut syntax");

                arguments.Add("ss", split[0]);
                arguments.Add("to", split[1]);
            }

            return File(
                _ffmpegProcessor.Start(arguments),
                FileUtils.GetContentType(options.Output),
                true
            );
        }
    }
}
