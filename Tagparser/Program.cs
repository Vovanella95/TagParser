using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Collections;


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
        public string ContentAttributes1;
    }


    [Flags]
    public enum Category
    {
        None = 0,
        Sectioning = 1 << 1,
        Flow = 1 << 2,
        Embedded = 1 << 3,
        Heading = 1 << 4,
        Interactiv = 1 << 5,
        Metadata = 1 << 6,
        Palpable = 1 << 7,
        Phrasing = 1 << 8
    }

    public enum GlobalAttributes1
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


    [Flags]
    public enum Attributes1 : ulong
    {
        None = 0,
        Abbr = 1ul << 1,
        Accept = 1ul << 2,
        AcceptCharset = 1ul << 3,
        Action = 1ul << 4,
        Allowfullscreen = 1ul << 5,
        Alt = 1ul << 6,
        Async = 1ul << 7,
        Autocomplete = 1ul << 8,
        Autofocus = 1ul << 9,
        Autoplay = 1ul << 10,
        Challenge = 1ul << 11,
        Charset = 1ul << 12,
        Checked = 1ul << 13,
        Cite = 1ul << 14,
        Cols = 1ul << 15,
        Colspan = 1ul << 16,
        Command = 1ul << 17,
        Content = 1ul << 18,
        Controls = 1ul << 19,
        Coords = 1ul << 20,
        Crossorigin = 1ul << 21,
        Data = 1ul << 22,
        Datetime = 1ul << 23,
        Default = 1ul << 24,
        Defer = 1ul << 25,
        Dirname = 1ul << 26,
        Disabled = 1ul << 27,
        Download = 1ul << 28,
        Enctype = 1ul << 29,
        For = 1ul << 30,
        Form = 1ul << 31,
        Formaction = 1ul << 32,
        Formenctype = 1ul << 33,
        Formmethod = 1ul << 34,
        Formnovalidate = 1ul << 35,
        Formtarget = 1ul << 36,
        Headers = 1ul << 37,
        Height = 1ul << 38,
        High = 1ul << 39,
        Href = 1ul << 40,
        Hreflang = 1ul << 41,
        HttpEquiv = 1ul << 42,
        Icon = 1ul << 43,
        Inputmode = 1ul << 44,
        Ismap = 1ul << 45,
        Keytype = 1ul << 46,
        Kind = 1ul << 47,
        Label = 1ul << 48,
        List = 1ul << 49,
        Loop = 1ul << 50,
        Low = 1ul << 51,
        Manifest = 1ul << 52,
        Max = 1ul << 53,
        Maxlength = 1ul << 54,
        Media = 1ul << 55,
        Mediagroup = 1ul << 56,
        Menu = 1ul << 57,
        Method = 1ul << 58,
        Min = 1ul << 59
    }

    [Flags]
    public enum Attributes12 : ulong
    {
        None = 0,
        Minlength = 1ul << 1,
        Multiple = 1ul << 2,
        Muted = 1ul << 3,
        Mame = 1ul << 4,
        Novalidate = 1ul << 5,
        Open = 1ul << 6,
        Optimum = 1ul << 7,
        Pattern = 1ul << 8,
        Placeholder = 1ul << 9,
        Poster = 1ul << 10,
        Preload = 1ul << 11,
        Radiogroup = 1ul << 12,
        Readonly = 1ul << 13,
        Rel = 1ul << 14,
        Required = 1ul << 15,
        Reversed = 1ul << 16,
        Rows = 1ul << 17,
        Rowspan = 1ul << 18,
        Sandbox = 1ul << 19,
        Scope = 1ul << 20,
        Scoped = 1ul << 21,
        Seamless = 1ul << 22,
        Selected = 1ul << 23,
        Shape = 1ul << 24,
        Size = 1ul << 25,
        Sizes = 1ul << 26,
        Sortable = 1ul << 27,
        Sorted = 1ul << 28,
        Span = 1ul << 29,
        Src = 1ul << 30,
        Srcdoc = 1ul << 31,
        Srclang = 1ul << 32,
        Srcset = 1ul << 33,
        Start = 1ul << 34,
        Step = 1ul << 35,
        Target = 1ul << 36,
        Type = 1ul << 37,
        Typemustmatch = 1ul << 38,
        Usemap = 1ul << 39,
        Value = 1ul << 40,
        Width = 1ul << 41,
        Wrap = 1ul << 42
    }


    public class Tag
    {
        public string TagName;
        public bool IsSingle;
        public Category Categories;
        public Attributes1 Attributes1;
        public Attributes12 Attributes12;
        public BitArray Attributes;
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

                string[] otherAttributes1 = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

                int i = -1;
                foreach (var item in cell.Children[1].Children)
                {
                    if (item.TagName == "DT") i++;
                    if (item.TagName == "DD")
                    {
                        otherAttributes1[i] += " | " + item.TextContent;
                    }
                    if (i == 5) break;
                }
                for (int j = 1; j <= 5; j++)
                {
                    typeof(T).GetField(fields[j].Name, BindingFlags.Public | BindingFlags.Instance).SetValue(a, otherAttributes1[j - 1].Replace('\n', '.').Replace(',', '.').Replace("—", "-"));
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
            sr.WriteLine("Tag Name,Categories,Context in which used,Content model,Tag omission,ContentAttributes1");


            foreach (var item in a)
            {
                sr.WriteLine(
                    item.TagName + "," +
                    item.Categories + "," +
                    item.ContextsUsed + "," +
                    item.ContentModel + "," +
                    item.TagOmissionInText + "," +
                    item.ContentAttributes1);
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
                            ContentAttributes1 = param[1],
                            ContentModel = param[2],
                            ContextsUsed = param[3],
                            TagOmissionInText = param[4]
                        };
                });*/
            #endregion
        }
    }
}
