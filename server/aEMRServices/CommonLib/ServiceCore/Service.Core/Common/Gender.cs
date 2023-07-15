using System;
using System.Net;
using System.Collections.Generic;

/*********************************
*
* Created by: Trinh Thai Tuyen
* Date Created: Friday, August 20, 2010
* Last Modified Date: 
*
*********************************/
using System.Runtime.Serialization;

namespace eHCMS.Services.Core
{
    [DataContract]
    public class Gender
    {
        public Gender(string sID, string sName)
        {
            ID = sID;
            Name = sName;
        }
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            Gender gen = obj as Gender;
            if (gen == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ID == gen.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        static Gender()
        {
            _allGenders = new List<Gender>() 
            {
                new Gender("M","Nam"),
                new Gender("F","Nữ"),
                new Gender("U","Chưa xác định")
            };
        }
        private static List<Gender> _allGenders;
        public static List<Gender> AllGenders
        {
            get
            {
                return _allGenders;
            }
        }
        public static Gender GetGender(string genderID)
        {
            Gender curGen = null;
            foreach (Gender g in _allGenders)
            {
                if(g.ID.ToUpper() == genderID.ToUpper())
                {
                    curGen = g;
                    break;
                }
            }
            return curGen;
        }
    }
}
