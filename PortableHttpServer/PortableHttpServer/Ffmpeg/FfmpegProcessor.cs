namespace PortableHttpServer.Ffmpeg
{
    public sealed class FfmpegProcessor
    {
        private readonly string _path;

        public FfmpegProcessor(string path)
        {
            _path = path;
        }

        public FfmpegStream Start(FfmpegArguments arguments)
        {
            return new FfmpegStream(_path, arguments);
        }
    }
}
