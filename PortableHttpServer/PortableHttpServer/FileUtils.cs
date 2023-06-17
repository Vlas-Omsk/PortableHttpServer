using Microsoft.AspNetCore.StaticFiles;

namespace PortableHttpServer
{
    public static class FileUtils
    {
        private readonly static string[] _convertibleFormats =
        {
            "mp4",
            "mp3",
            "m4a"
        };

        public static bool IsFileConvertible(string name)
        {
            return _convertibleFormats.Contains(
                Path.GetExtension(name)[1..]
            );
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
