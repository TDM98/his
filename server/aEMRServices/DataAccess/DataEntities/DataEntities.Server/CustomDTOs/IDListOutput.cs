using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DataEntities.CustomDTOs
{
    /// <summary>
    /// Lớp này dùng để chứa danh sách các ID output ra khi insert 1 danh sách các record vào database
    /// </summary>
    [XmlRoot(ElementName = "Root", Namespace = "")]
    public class IDListOutput<T>
    {
        private List<long> _ids;
       
        [XmlArray("IDList"), XmlArrayItem("ID",IsNullable = false,Type = typeof(long))]
        public List<long> Ids
        {
            get { return _ids; }
            set { _ids = value; }
        }
    }
}
