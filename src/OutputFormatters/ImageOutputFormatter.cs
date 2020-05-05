using System;
using Microsoft.DocAsCode.MarkdownLite;
using PlantUml.Net;

namespace DocFx.Plugins.PlantUml.OutputFormatters
{
    public class ImageOutputFormatter : IOutputFormatter
    {
        private readonly Options options;
        private readonly OutputFormat format;

        public ImageOutputFormatter(Options options, OutputFormat format)
        {
            this.options = options;
            this.format = format;
        }

        public StringBuffer FormatOutput(MarkdownCodeBlockToken token, byte[] output)
        {
            if (!(output?.Length > 0))
            {
                return StringBuffer.Empty;
            }

            var imgSrc = $"data:image/{format.ToString().ToLowerInvariant()};base64,{Convert.ToBase64String(output)}";
            return $"<div class=\"{options.LangPrefix}plantuml-image\"><img src=\"{imgSrc}\" /></div>";
        }
        public bool UseUri => false;

    }
}
