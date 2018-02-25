using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace OTERT_Telerik.Model {

    public class JSON2WS {
        [JsonProperty("skip")]
        public int Skip { get; set; }
        [JsonProperty("take")]
        public int Take { get; set; }
        [JsonProperty("order")]
        public string Order { get; set; }
        [JsonProperty("search")]
        public List<string> SearchFilters { get; set; }
    }

    public class Login {
        [JsonProperty("name")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }

}