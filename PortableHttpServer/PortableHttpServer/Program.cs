using Microsoft.AspNetCore.Server.Kestrel.Core;
using PortableHttpServer.Ffmpeg;
using PortableHttpServer.Services;
using System.Collections.Immutable;

namespace PortableHttpServer
{
    public static class Program
    {
        private const string _prefix = "-";

        public static void Main(string[] args)
        {
            var config = ParseConfig(args);
            var builder = WebApplication.CreateBuilder();

            builder.WebHost.ConfigureKestrel((context, options) =>
            {
                options.ListenAnyIP(config.Port, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;

                    if (config.UseHttps)
                        listenOptions.UseHttps();
                });
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<LocatorService>();
            builder.Services.AddSingleton(
                new FfmpegProcessor(config.FfmpegPath)
            );
            builder.Services.AddSingleton(config);

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "convert_video_download",
                pattern: "/convert_video/download/{*publicPath}",
                defaults: new { controller = "ConvertVideo", action = "Download" }
            );
            app.MapControllerRoute(
                name: "convert_video",
                pattern: "/convert_video/{*publicPath}",
                defaults: new { controller = "ConvertVideo", action = "Index" }
            );
            app.MapControllerRoute(
                name: "download",
                pattern: "/download/{*publicPath}",
                defaults: new { controller = "Home", action = "Download" }
            );
            app.MapControllerRoute(
                name: "default",
                pattern: "/{*publicPath}",
                defaults: new { controller = "Home", action = "Index" }
            );

            app.Run();
        }

        private static Config ParseConfig(string[] args)
        {
            var enumrator = args.GetEnumerator();
            var entries = ImmutableArray.CreateBuilder<Entry>();

            while (enumrator.MoveNext())
            {
                var item = (string)enumrator.Current;

                if (item.StartsWith(_prefix))
                    break;

                item = Path.GetFullPath(item);

                if (!Directory.Exists(item))
                    throw new Exception("Directory not found");

                entries.Add(
                    new Entry(
                        Path.GetFileName(item),
                        string.Join('/', item.Split('/', '\\').SkipLast(1))
                    )
                );
            }

            var port = 8080;
            var useHttps = false;
            var ffmpegPath = "ffmpeg";
            var videoConvertMaxThreads = (int?)null;

            if (enumrator.Current != null)
            {
                do
                {
                    var item = (string)enumrator.Current;

                    if (!item.StartsWith(_prefix))
                        throw new Exception("Invalid arguments syntax");

                    var split = item[_prefix.Length..].Split('=');

                    if (split.Length is not (2 or 1))
                        throw new Exception("Invalid arguments syntax");

                    switch (split[0])
                    {
                        case "port":
                            port = int.Parse(split[1]);
                            break;
                        case "https":
                            useHttps = true;
                            break;
                        case "ffmpeg":
                            ffmpegPath = split[1];
                            break;
                        case "videoConvertMaxThreads":
                            videoConvertMaxThreads = int.Parse(split[1]);
                            break;

                    }
                }
                while (enumrator.MoveNext());
            }

            return new Config(
                port,
                useHttps,
                entries.ToImmutable(),
                ffmpegPath,
                videoConvertMaxThreads
            );
        }
    }
}