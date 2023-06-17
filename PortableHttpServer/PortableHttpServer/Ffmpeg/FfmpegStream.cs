using System.Diagnostics;
using System.Text;

namespace PortableHttpServer.Ffmpeg
{
    public sealed class FfmpegStream : Stream
    {
        private readonly Process _process;
        private readonly StringBuilder _error = new();
        private int _position;

        public FfmpegStream(string path, FfmpegArguments arguments)
        {
            _process = new Process();

            _process.StartInfo.FileName = path;
            _process.StartInfo.Arguments = arguments.ToString() + " -movflags frag_keyframe+empty_moov -";
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _process.EnableRaisingEvents = true;

            _process.ErrorDataReceived += OnProcessErrorDataReceived;
            _process.Exited += OnProcessExited;

            _process.Start();
            _process.BeginErrorReadLine();
        }

        ~FfmpegStream()
        {
            Dispose();
        }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = false;
        public override long Length => Position;
        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            _position += count;

            return _process.StandardOutput.BaseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            _process.ErrorDataReceived -= OnProcessErrorDataReceived;
            _process.Exited -= OnProcessExited;

            try
            {
                _process.Kill(true);

                if (!_process.HasExited)
                    _process.WaitForExit();
            }
            catch
            {
            }

            _process.Dispose();

            base.Dispose(disposing);
        }

        private void OnProcessExited(object? sender, EventArgs e)
        {
            throw new Exception(_error.ToString());
        }

        private void OnProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _error.AppendLine(e.Data);
        }
    }
}
