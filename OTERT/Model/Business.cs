using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTERT.Model {

    public class PlaceB {
        public int ID { get; set; }
        public CountryDTO Country { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
    }

    public class UserB {
        public int ID { get; set; }
        public int UserGroupID { get; set; }
        public string NameGR { get; set; }
        public string NameEN { get; set; }
        public string Telephone { get; set; }
        public string FAX { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserGroupDTO UserGroup { get; set; }
    }

}