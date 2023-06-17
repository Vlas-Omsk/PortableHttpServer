namespace PortableHttpServer.Ffmpeg
{
    public sealed class FfmpegArguments
    {
        private readonly Dictionary<string, string?> _arguments = new();

        public FfmpegArguments Add(string name, string value)
        {
            _arguments.Add(name, value);

            return this;
        }

        public FfmpegArguments Add(string name)
        {
            _arguments.Add(name, null);

            return this;
        }

        public override string ToString()
        {
            return string.Join(" ", _arguments.Select(x =>
            {
                var result = $"-{x.Key}";

                if (x.Value != null)
                    result += $" \"{x.Value}\"";

                return result;
            }));
        }
    }
}
