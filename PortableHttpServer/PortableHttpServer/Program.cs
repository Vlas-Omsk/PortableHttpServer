using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Collections.Immutable;

namespace PortableHttpServer
{
    public static class Program
    {
        private const string _prefix = "-";

        public static void Main(string[] args)
        {
            var enumrator = args.GetEnumerator();
            var paths = ImmutableArray.CreateBuilder<ConfigPath>();

            while (enumrator.MoveNext())
            {
                var item = (string)enumrator.Current;

                if (item.StartsWith(_prefix))
                    break;

                item = Path.GetFullPath(item);

                if (!Directory.Exists(item))
                    throw new Exception("Directory not found");

                paths.Add(
                    new ConfigPath(
                        Path.GetFileName(item),
                        string.Join('/', item.Split('/', '\\').SkipLast(1))
                    )
                );
            }

            var port = 8080;
            var useHttps = false;

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
                }
            }
            while (enumrator.MoveNext());

            var builder = WebApplication.CreateBuilder();

            builder.WebHost.ConfigureKestrel((context, options) =>
            {
                options.ListenAnyIP(port, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;

                    if (useHttps)
                        listenOptions.UseHttps();
                });
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton(
                new Config(paths.ToImmutable())
            );

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "download",
                pattern: "/download/{*path}",
                defaults: new { controller = "Home", action = "Download" }
            );
            app.MapControllerRoute(
                name: "default",
                pattern: "/{*path}",
                defaults: new { controller = "Home", action = "Index" }
            );

            app.Run();
        }
    }
}