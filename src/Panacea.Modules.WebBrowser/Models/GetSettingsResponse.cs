using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.WebBrowser.Models
{
    [DataContract]
    public class GetSettingsResponse
    {
        [DataMember(Name = "tabLimit")]
        public int TabLimit { get; set; }

        [DataMember(Name = "browserEngine")]
        public string BrowserEngine { get; set; }

        [DataMember(Name = "urlBlackList")]
        public List<string> BlackList { get; set; }

        [DataMember(Name = "urlWhiteList")]
        public List<string> WhiteList { get; set; }
    }
}
