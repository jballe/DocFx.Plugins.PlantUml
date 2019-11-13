using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            //var dir = Path.Combine(context.GetBaseFolder(), GetFilePath(context));
            //var dir = Path.Combine(context.GetBaseFolder(), context.GetFilePathStack().Peek());
            var dir = Path.Combine(context.GetBaseFolder(), "diagrams");
            IPlantUmlRenderer plantUmlRenderer = rendererFactory.CreateRenderer(settings, dir);
            IOutputFormatter outputFormatter = formatterFactory.CreateOutputFormatter(markdownRenderer.Options);

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

        // private string GetFilePath(MarkdownBlockContext context)
        // {
        //     // Will use reflection as a got assembly version error for System.Collections.Immutable
        //     var stack = context.Variables["FilePathStack"];
        //     var ie = (IEnumerable<string>) stack;
        //     return ie.First();
        // 
        // 
        //     var t = stack.GetType();
        //     var peekMethod = t.GetMethod("Peek");
        //     var result = peekMethod.Invoke(stack, null) as string;
        //     return result;
        // }
    }
}
