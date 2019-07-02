using Panacea.ContentControls;
using Panacea.Core;
using Panacea.Modules.WebBrowser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.WebBrowser
{

    public class LinkItemProvider : HospitalServerLazyItemProvider<Link>
    {
        public LinkItemProvider(PanaceaServices core)
            : base(core, "web/get_categories_only/", "web/get_category_limited/{0}/{1}/{2}/", "", 10)
        {

        }

    }

}
