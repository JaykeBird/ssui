using XmlDocMarkdown.Core;

namespace SolidShineUi.XmlDocGen
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return XmlDocMarkdownApp.Run(args);

            //if (args.Length == 0)
            //{
            //    return XmlDocMarkdownApp.Run(new List<string>{"SolidShineUi.dll", "docs"});
            //}
            //else
            //{
            //    return XmlDocMarkdownApp.Run(args);
            //}
        }
    }
}