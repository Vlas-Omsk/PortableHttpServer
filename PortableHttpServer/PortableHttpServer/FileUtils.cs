using Microsoft.AspNetCore.StaticFiles;

namespace PortableHttpServer
{
    public static class FileUtils
    {
        private readonly static string[] _convertibleVideoFormats =
        {
            "mp4",
            "mp3",
            "m4a"
        };

        public static bool IsVideoConvertible(string name)
        {
            var extension = Path.GetExtension(name);

            if (string.IsNullOrEmpty(extension))
                return false;

            return _convertibleVideoFormats.Contains(extension[1..]);
        }

        public static string GetContentType(string name)
        {
            if (!new FileExtensionContentTypeProvider()
                    .TryGetContentType(name, out var contentType))
                contentType = "application/octet-stream";

            return contentType;
        }
    }
}
