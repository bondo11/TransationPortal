using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace translate_spa.Utilities
{
    public static class HtmlUtil
    {
        public static bool ContainsXHTML(this string input)
        {
            try
            {
                var x = XElement.Parse("<wrapper>" + input + "</wrapper>");
                return !(x.DescendantNodes().Count() == 1 && x.DescendantNodes().First().NodeType == XmlNodeType.Text);
            }
            catch (XmlException exception)
            {
                return true;
            }
        }
    }
}