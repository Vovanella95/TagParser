using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using System.Text.RegularExpressions;

namespace Tagparser
{

    class TagData
    {
        public string TagName;
        public string Categories;
        public string ContextsUsed;
        public string ContentModel;
        public string TagOmissionInText;
        public string ContentAttributes;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var config = new Configuration().WithDefaultLoader();
            var address = "http://www.w3.org/TR/html51/semantics.html";
            var document = BrowsingContext.New(config).OpenAsync(Url.Create(address)).Result;
            var cellSelector = "section";
            var cells = document.QuerySelectorAll(cellSelector);
            var titles = cells.Select(m => m).Where(w =>
            {
                Regex reg = new Regex(@"(^[0-9.]+ The \w+ element\n\n)|(^[0-9.]+ The [\w, and]+ elements)");
                return reg.IsMatch(w.TextContent);
            }).Select(w =>
                {
                    int i = -1;
                    string[] param = new string[5];
                    foreach (var item in w.Children[1].Children)
                    {
                        if (item.TagName == "DT") i++;
                        if (i == 5) break;
                        if(item.TagName=="DD")
                        {
                            param[i] += item.TextContent+'\n';
                        }
                    }
                    return new TagData()
                        {
                            TagName = w.QuerySelector("h4").TextContent,
                            Categories = param[0],
                            ContentAttributes = param[1],
                            ContentModel = param[2],
                            ContextsUsed = param[3],
                            TagOmissionInText = param[4]
                        };
                });
        }
    }
}
