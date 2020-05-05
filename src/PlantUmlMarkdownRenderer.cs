using System;
using System.IO;
using Microsoft.DocAsCode.Dfm;
using Microsoft.DocAsCode.MarkdownLite;
using PlantUml.Net;

namespace DocFx.Plugins.PlantUml
{
    public class PlantUmlMarkdownRenderer : DfmCustomizedRendererPartBase<IMarkdownRenderer, MarkdownCodeBlockToken, MarkdownBlockContext>
    {
        private const string LANGUAGE_TAG = "plantUml";

        private readonly DocFxPlantUmlSettings settings;
        private readonly RendererFactory rendererFactory;
        private readonly FormatterFactory formatterFactory;

        public override string Name => nameof(PlantUmlMarkdownRenderer);

        public PlantUmlMarkdownRenderer(DocFxPlantUmlSettings settings)
        {
            this.settings = settings;
            rendererFactory = new RendererFactory();
            formatterFactory = new FormatterFactory(settings);
        }

        public override bool Match(IMarkdownRenderer renderer, MarkdownCodeBlockToken token, MarkdownBlockContext context)
        {
            return string.Equals(token.Lang, LANGUAGE_TAG, StringComparison.InvariantCultureIgnoreCase);
        }

        public override StringBuffer Render(IMarkdownRenderer markdownRenderer, MarkdownCodeBlockToken token, MarkdownBlockContext context)
        {
//#if DEBUG
//            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
//#endif

            var sourceFilePath = Path.Combine(context.GetBaseFolder(), context.GetFilePathStack().Peek());
            var dir = Directory.GetParent(sourceFilePath).FullName;

            var plantUmlRenderer = rendererFactory.CreateRenderer(settings, dir);
            var outputFormatter = formatterFactory.CreateOutputFormatter(markdownRenderer.Options);

            try
            {
                byte[] output;
                if (outputFormatter.UseUri)
                {
                    var uri = plantUmlRenderer.RenderAsUri(token.Code, settings.OutputFormat);
                    output = System.Text.Encoding.UTF8.GetBytes(uri.ToString());
                }
                else
                {
                    output = plantUmlRenderer.Render(token.Code, settings.OutputFormat);
                }

                return outputFormatter.FormatOutput(token, output);
            }
            catch (ArgumentException exc)
            {
                Console.WriteLine($"Error while rendering PlantUml code in {sourceFilePath}: {exc}");
                return $"<code class=\"{token.Lang}\">{token.Code}</code>";
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error while rendering PlantUml code in {sourceFilePath}: {exc}");
                throw;
            }
        }
    }
}
