using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;


namespace Tagparser
{

    public class Selector : Attribute
    {
        public string DataType;
        public Selector(string dataType)
        {
            DataType = dataType;
        }
    }



    [Selector("body>section>section>section")]
    class TagData
    {
        [Selector(" > h4")]
        public string TagName;

        [Selector(" dd")]
        public string Categories;

        [Selector(" > dd")]
        public string ContextsUsed;

        [Selector(" > dd")]
        public string ContentModel;

        [Selector(" > dd")]
        public string TagOmissionInText;

        [Selector(" > dd")]
        public string ContentAttributes;
    }

    [Flags]
    public enum Category
    {
        None = 0,
        Metadata = 0x01,
        Flow = 0x02,
        Sectioning = 0x04,
        Heading = 0x08,
        Phrasing = 0x16,
        Embedded = 0x32,
        Interactive = 0x64,
        Palpable = 0x128
    }

    public enum GlobalAttributes
    {
        Accesskey,
        Class,
        Contenteditable,
        Contextmenu,
        Dir,
        Draggable,
        Dropzone,
        Hidden,
        Id,
        Itemid,
        Itemprop,
        Itemref,
        Itemscope,
        Itemtype,
        Lang,
        Spellcheck,
        Style,
        Tabindex,
        Title,
        Translate
    }

    public enum Attributes
    {
        None = 0,
        Abbr = 1,
        Accept = 2,
        AcceptCharset = 3,
        Action = 4,
        Allowfullscreen = 5,
        Alt = 6,
        Async = 7,
        Autocomplete = 8,
        Autofocus = 9,
        Autoplay = 10,
        Challenge = 11,
        Charset = 12,
        Checked = 13,
        Cite = 14,
        Cols = 15,
        Colspan = 16,
        Command = 17,
        Content = 18,
        Controls = 19,
        Coords = 20,
        Crossorigin = 21,
        Data = 22,
        Datetime = 23,
        Default = 24,
        Defer = 25,
        Dirname = 26,
        Disabled = 27,
        Download = 28,
        Enctype = 29,
        For = 30,
        Form = 31,
        Formaction = 32,
        Formenctype = 33,
        Formmethod = 34,
        Formnovalidate = 35,
        Formtarget = 36,
        Headers = 37,
        Height = 38,
        High = 39,
        Href = 40,
        Hreflang = 41,
        HttpEquiv = 42,
        Icon = 43,
        Inputmode = 44,
        Ismap = 45,
        Keytype = 46,
        Kind = 47,
        Label = 48,
        List = 49,
        Loop = 50,
        Low = 51,
        Manifest = 52,
        Max = 53,
        Maxlength = 54,
        Media,
        Mediagroup,
        Menu,
        Method,
        Min,
        Minlength,
        Multiple,
        Muted,
        Mame,
        Novalidate,
        Open,
        Optimum,
        Pattern,
        Placeholder,
        Poster,
        Preload,
        Radiogroup,
        Readonly,
        Rel,
        Required,
        Reversed,
        Rows,
        Rowspan,
        Sandbox,
        Scope,
        Scoped,
        Seamless,
        Selected,
        Shape,
        Size,
        Sizes,
        Sortable,
        Sorted,
        Span,
        Src,
        Srcdoc,
        Srclang,
        Srcset,
        Start,
        Step,
        Target,
        Type,
        Typemustmatch,
        Usemap,
        Value,
        Width,
        Wrap
    }

    public class Tag
    {
        public string TagName;
        public bool IsSingle;
        public Category Categories;
        public Attributes Attributes;
    }







    class Program
    {
        public static IEnumerable<T> Deserialize<T>(string address)
        {
            var config = new AngleSharp.Configuration().WithDefaultLoader();
            var document = BrowsingContext.New(config).OpenAsync(Url.Create(address)).Result;
            var fields = (typeof(T)).GetFields();
            var attrib = ((Selector)(System.Attribute.GetCustomAttributes(typeof(T)).First())).DataType;
            var attribs = fields.Select(w =>
                {
                    return ((Selector)(System.Attribute.GetCustomAttributes(typeof(T).GetField(w.Name)).First())).DataType;
                }).ToArray();

            T a = (T)Activator.CreateInstance(typeof(T));
            foreach (var cell in document.QuerySelectorAll(attrib))
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    var value = cell.QuerySelector(attribs[i]);
                    typeof(T).GetField(fields[i].Name, BindingFlags.Public | BindingFlags.Instance).SetValue(a, value != null ? value.TextContent : "Empty");
                }
                yield return a;
            }
        }

        public static IEnumerable<T> DeserializeTags<T>(string address)
        {
            var config = new AngleSharp.Configuration().WithDefaultLoader();
            var document = BrowsingContext.New(config).OpenAsync(Url.Create(address)).Result;
            var fields = (typeof(T)).GetFields();
            var attrib = ((Selector)(System.Attribute.GetCustomAttributes(typeof(T)).First())).DataType;
            var attribs = fields.Select(w =>
            {
                return ((Selector)(System.Attribute.GetCustomAttributes(typeof(T).GetField(w.Name)).First())).DataType;
            }).ToArray();

            T a = (T)Activator.CreateInstance(typeof(T));
            foreach (var cell in document.QuerySelectorAll(attrib))
            {
                var tagName = cell.QuerySelector(attribs[0]);
                if (tagName == null) continue;

                int i1 = tagName.TextContent.IndexOf("The");
                int i2 = tagName.TextContent.IndexOf("element");
                try
                {
                    typeof(T).GetField(fields[0].Name, BindingFlags.Public | BindingFlags.Instance).SetValue(a, tagName.TextContent.Substring(i1 + 4, i2 - i1 - 5));
                }
                catch (Exception ex)
                {
                    continue;
                }

                string[] otherAttributes = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

                int i = -1;
                foreach (var item in cell.Children[1].Children)
                {
                    if (item.TagName == "DT") i++;
                    if (item.TagName == "DD")
                    {
                        otherAttributes[i] += " | " + item.TextContent;
                    }
                    if (i == 5) break;
                }
                for (int j = 1; j <= 5; j++)
                {
                    typeof(T).GetField(fields[j].Name, BindingFlags.Public | BindingFlags.Instance).SetValue(a, otherAttributes[j - 1].Replace('\n', '.').Replace(',', '.').Replace("—", "-"));
                }

                Regex reg = new Regex(@"(^[0-9.]+ The \w+ element)|(^[0-9.]+ The [\w, and]+ elements)");
                if (reg.IsMatch(tagName.TextContent))
                {
                    yield return a;
                }

            }
        }

        static void Main(string[] args)
        {

            var a = DeserializeTags<TagData>("http://www.w3.org/TR/html51/semantics.html");

            StreamWriter sr = new StreamWriter("D:\\TagData.csv");

            sr.WriteLine("Tag Name,Categories,Context in which used,Content model,Tag omission,ContentAttributes");


            foreach (var item in a)
            {
                sr.WriteLine(
                    item.TagName + "," +
                    item.Categories + "," +
                    item.ContextsUsed + "," +
                    item.ContentModel + "," +
                    item.TagOmissionInText + "," +
                    item.ContentAttributes);
            }

            sr.Close();
            Console.Read();

            #region OldVersion
            /*
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
                });*/
            #endregion
        }
    }
}
