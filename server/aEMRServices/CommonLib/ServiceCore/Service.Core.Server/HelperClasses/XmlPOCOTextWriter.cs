using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace eHCMS.Services.Core
{
    public class XmlPOCOTextWriter : XmlTextWriter
    {
        public XmlPOCOTextWriter(TextWriter w) : base(w) { }
        public XmlPOCOTextWriter(Stream w, Encoding encoding) : base(w, encoding) { }
        public XmlPOCOTextWriter(string filename, Encoding encoding) :
            base(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None), encoding) { }

        bool _skip = false;

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            if (prefix == "xmlns" && (localName == "xsd" || localName == "xsi"))
            {
                _skip = true;
                return;
            }
            base.WriteStartAttribute(prefix, localName, ns);
        }
        public override void WriteString(string text)
        {
            if (_skip)
            {
                return;
            }
            base.WriteString(text);
        }
        public override void WriteEndAttribute()
        {
            if (_skip)
            {
                _skip = false;
                return;
            }
            base.WriteEndAttribute();
        }
        public override void WriteStartDocument()
        {
            //Khong ghi gi het
        }
        public override void WriteStartDocument(bool standalone)
        {
            //Khong ghi gi het
        }
    }
}
