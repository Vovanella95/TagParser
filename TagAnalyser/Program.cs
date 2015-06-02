using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tagparser;
using System.IO;
using System.Collections;

namespace TagAnalyser
{
    class Program
    {
        #region BadCode
        static Dictionary<string, Category> categoryDict = new Dictionary<string, Category>()
        {
            {"Sectioning",Category.Sectioning},
            {"Flow",Category.Flow},
            {"Embedded",Category.Embedded},
            {"Heading",Category.Heading},
            {"Interactiv",Category.Interactiv},
            {"Metadata",Category.Metadata},
            {"Palpable",Category.Palpable},
            {"Phrasing",Category.Phrasing}
        };

        static Dictionary<string, Attributes1> attributeDict = new Dictionary<string, Attributes1>()
        {
            {"abbr", Attributes1.Abbr},
            {"accept", Attributes1.Accept},
            {"acceptcharset", Attributes1.AcceptCharset},
            {"action", Attributes1.Action},
            {"allowfullscreen", Attributes1.Allowfullscreen},
            {"alt", Attributes1.Alt},
{"async", Attributes1.Async},
{"autocomplete", Attributes1.Autocomplete},
{"autofocus", Attributes1.Autofocus},
{"autoplay", Attributes1.Autoplay},
{"challenge", Attributes1.Challenge},
{"charset", Attributes1.Charset},
{"checked", Attributes1.Checked},
{"cite", Attributes1.Cite},
{"cols", Attributes1.Cols},
{"colspan", Attributes1.Colspan},
{"command", Attributes1.Command},
{"content", Attributes1.Content},
{"controls", Attributes1.Controls},
{"coords", Attributes1.Coords},
{"crossorigin", Attributes1.Crossorigin},
{"data", Attributes1.Data},
{"datetime", Attributes1.Datetime},
{"default", Attributes1.Default},
{"defer", Attributes1.Defer},
{"dirname", Attributes1.Dirname},
{"disabled", Attributes1.Disabled},
{"download", Attributes1.Download},
{"enctype", Attributes1.Enctype},
{"for", Attributes1.For},
{"form", Attributes1.Form},
{"formaction", Attributes1.Formaction},
{"formenctype", Attributes1.Formenctype},
{"formmethod", Attributes1.Formmethod},
{"formnovalidate", Attributes1.Formnovalidate},
{"formtarget", Attributes1.Formtarget},
{"headers", Attributes1.Headers},
{"height", Attributes1.Height},
{"high", Attributes1.High},
{"href", Attributes1.Href},
{"hreflang", Attributes1.Hreflang},
{"httpequiv", Attributes1.HttpEquiv},
{"icon", Attributes1.Icon},
{"inputmode", Attributes1.Inputmode},
{"ismap", Attributes1.Ismap},
{"keytype", Attributes1.Keytype},
{"kind", Attributes1.Kind},
{"label", Attributes1.Label},
{"list", Attributes1.List},
{"loop", Attributes1.Loop},
{"low", Attributes1.Low},
{"manifest", Attributes1.Manifest},
{"max", Attributes1.Max},
{"maxlength", Attributes1.Maxlength},
{"media", Attributes1.Media},
{"mediagroup", Attributes1.Mediagroup},
{"menu", Attributes1.Menu},
{"method", Attributes1.Method},
{"min", Attributes1.Min}
        };




        static Dictionary<string, Attributes12> attributeDict2 = new Dictionary<string, Attributes12>()
        {
{"minlength", Attributes12.Minlength},
{"multiple", Attributes12.Multiple},
{"muted", Attributes12.Muted},
{"mame", Attributes12.Mame},
{"novalidate", Attributes12.Novalidate},
{"open", Attributes12.Open},
{"optimum", Attributes12.Optimum},
{"pattern", Attributes12.Pattern},
{"placeholder", Attributes12.Placeholder},
{"poster", Attributes12.Poster},
{"preload", Attributes12.Preload},
{"radiogroup", Attributes12.Radiogroup},
{"readonly", Attributes12.Readonly},
{"rel", Attributes12.Rel},
{"required", Attributes12.Required},
{"reversed", Attributes12.Reversed},
{"rows", Attributes12.Rows},
{"rowspan", Attributes12.Rowspan},
{"sandbox", Attributes12.Sandbox},
{"scope", Attributes12.Scope},
{"scoped", Attributes12.Scoped},
{"seamless", Attributes12.Seamless},
{"selected", Attributes12.Selected},
{"shape", Attributes12.Shape},
{"size", Attributes12.Size},
{"sizes", Attributes12.Sizes},
{"sortable", Attributes12.Sortable},
{"sorted", Attributes12.Sorted},
{"span", Attributes12.Span},
{"src", Attributes12.Src},
{"srcdoc", Attributes12.Srcdoc},
{"srclang", Attributes12.Srclang},
{"srcset", Attributes12.Srcset},
{"start", Attributes12.Start},
{"step", Attributes12.Step},
{"target", Attributes12.Target},
{"type", Attributes12.Type},
{"typemustmatch", Attributes12.Typemustmatch},
{"usemap", Attributes12.Usemap},
{"value", Attributes12.Value},
{"width", Attributes12.Width},
{"wrap", Attributes12.Wrap},

        };

        #endregion
        static void GenerateTags(string csvPath)
        {
            StreamReader sr = new StreamReader(csvPath);
            while(!sr.EndOfStream)
            {
                var a = sr.ReadLine();
                var c = ParseString(a);
            }
        }

        static Tag ParseString(string str)
        {

            var arr = new int[101].Select((x, i) => x = i+1);

            var values = str.Split(',');
            Category category = Category.None;
            string temp;
            for (int i = 0; i < categoryDict.Count(); i++)
            {
                temp = categoryDict.ElementAt(i).Key;
                if (values[1].Contains(temp))
                {
                    category |= categoryDict[temp];
                }
            }
            Attributes1 attribute = Attributes1.None;
            for (int i = 0; i < attributeDict.Count(); i++)
            {
                temp = attributeDict.ElementAt(i).Key;
                if (values[5].Contains(temp))
                {
                    attribute |= attributeDict[temp];
                }
            }
            Attributes12 attribute2 = Attributes12.None;
            for (int i = 0; i < attributeDict2.Count(); i++)
            {
                temp = attributeDict2.ElementAt(i).Key;
                if (values[5].Contains(temp))
                {
                    attribute2 |= attributeDict2[temp];
                }
            }

            BitArray attribs = new BitArray(new int[]{1,2,3,4});
            
            

            bool isSingle = values[4].Contains("No end tag")? true:false;
            return new Tag()
            {
                TagName = values[0],
                Categories = category,
                Attributes1 = attribute,
                Attributes12 = attribute2,
                IsSingle = isSingle
            };
        }

        static void Main(string[] args)
        {
            GenerateTags("D:\\TagData.csv");
        }
    }
}
