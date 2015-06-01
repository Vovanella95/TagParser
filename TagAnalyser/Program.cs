using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tagparser;
using System.IO;

namespace TagAnalyser
{
    class Program
    {

        static IEnumerable<Tag> GenerateTags(string scvPath)
        {
            throw new NotImplementedException();
        }




        static Tag ParseString(string str)
        {
            var values = str.Split(',');

            var categs = ((IEnumerable<Category>)Enum.GetValues(typeof(Category))).Select(w => w.ToString());
            Category category = Category.None;
            for (int i = 0; i < categs.Count(); i++)
            {
                if (values[1].Contains(categs.ElementAt(i)))
                {
                    category = category | (Category)(i);
                }
            }


            var attrs = ((IEnumerable<Attributes>)Enum.GetValues(typeof(Attributes))).Select(w => w.ToString().ToLower());
            Attributes attribute = Attributes.None;
            for (int i = 0; i < attrs.Count(); i++)
            {
                if (values[5].Contains(attrs.ElementAt(i)))
                {
                    attribute = attribute | (Attributes)i;
                }
            }
            

            return new Tag()
            {
                TagName = values[0],
                Categories = category,
                Attributes = attribute
            };
        }



        static void Main(string[] args)
        {
            Category c = Category.Embedded | Category.Metadata | Category.Heading;


            var a = ParseString("input, | Flow content. | Phrasing content. | If the type attribute is not in the Hidden state: Interactive content. | If the type attribute is not in the Hidden state: Listed. labelable. submittable. resettable. and reassociateable form-associated element. | If the type attribute is in the Hidden state: Listed. submittable. resettable. and reassociateable form-associated element. | If the type attribute is not in the Hidden state: Palpable content., | Where phrasing content is expected., | Nothing., | No end tag., | Global attributes | accept - Hint for expected file type in file upload controls | alt - Replacement text for use when images are not available | autocomplete - Hint for form autofill feature | autofocus - Automatically focus the form control when the page is loaded | checked - Whether the command or control is checked | dirname - Name of form field to use for sending the element's directionality in form submission | disabled - Whether the form control is disabled | form - Associates the control w");
        }
    }
}
