﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using System.Text.RegularExpressions;

namespace Tagparser
{

    public class Selector :Attribute
    {
        public string DataType;
        public Selector(string dataType)
        {
            DataType = dataType;
        }


        public IEnumerable<T> Deserialize<T>(string html) 
        {
            




            throw new Exception();
        }
    }



    [Selector("table.propstable tr")]
    class TagData
    {
        [Selector("TagName")]
        public string TagName;

        [Selector("Categories")]
        public string Categories;

        [Selector("ContextUsed")]
        public string ContextsUsed;

        [Selector("ContentModel")]
        public string ContentModel;

        [Selector("TagOmissionInText")]
        public string TagOmissionInText;

        [Selector("ContentAttributes")]
        public string ContentAttributes;
    }





    class Program
    {
        static void Main(string[] args)
        {
            var config = new AngleSharp.Configuration().WithDefaultLoader();
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
