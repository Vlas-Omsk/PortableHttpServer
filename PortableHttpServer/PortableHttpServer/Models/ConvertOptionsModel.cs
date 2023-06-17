using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PortableHttpServer.Models
{
    public sealed class ConvertOptionsModel
    {
        [BindRequired]
        [BindProperty(Name = "output")]
        public string Output { get; set; } = null!;
        [BindProperty(Name = "size")]
        public string? Size { get; set; }
        [BindProperty(Name = "video_bitrate")]
        public int? VideoBitrate { get; set; }
        [BindProperty(Name = "audio_bitrate")]
        public int? AudioBitrate { get; set; }
        [BindProperty(Name = "crf")]
        public int? Crf { get; set; }
        [BindProperty(Name = "cut")]
        public string? Cut { get; set; }
    }
}
