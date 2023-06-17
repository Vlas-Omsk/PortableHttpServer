namespace PortableHttpServer
{
    public static class FileInfoExtenisons
    {
        private readonly static string[] _suffix = { "B", "KB", "MB", "GB", "TB" };

        public static string FormatBytes(this FileInfo info)
        {
            var bytes = info.Length;
            double dblSByte = bytes;
            int i;

            for (i = 0; i < _suffix.Length && bytes >= 1024; i++, bytes /= 1024)
                dblSByte = bytes / 1024.0;

            return string.Format("{0:0.##} {1}", dblSByte, _suffix[i]);
        }
    }
}
