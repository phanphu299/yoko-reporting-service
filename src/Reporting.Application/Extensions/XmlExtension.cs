using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace Reporting.Application.Extension
{
    public static class XmlExtension
    {
        public static XElement RemoveAllNamespaces(this XElement xml)
        {
            foreach (XElement XE in xml.DescendantsAndSelf())
            {
                // stripping the namespace by setting the name of the element to it's localname only
                XE.Name = XE.Name.LocalName;

                // replacing all attributes with attributes that are not namespaces and their names are set to only the localname
                XE.ReplaceAttributes((from xattrib in XE.Attributes().Where(xa => !xa.IsNamespaceDeclaration) select new XAttribute(xattrib.Name.LocalName, xattrib.Value)));
            }
            return xml;
        }
    }
}