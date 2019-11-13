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

            string imgSrc = null;
            try
            {
                string url = System.Text.Encoding.UTF8.GetString(output);
                if (url.StartsWith("http"))
                {
                    imgSrc = url;
                }
            }
            catch (Exception)
            {
                imgSrc = null;
            }

            if (imgSrc == null)
            {
                imgSrc = $"data:image/{format.ToString().ToLowerInvariant()};base64" + Convert.ToBase64String(output);
            }

            return $"<div class=\"{options.LangPrefix}plantuml-image\"><img src=\"{imgSrc}\" /></div>";
        }
        public bool UseUri => true;

    }
}
