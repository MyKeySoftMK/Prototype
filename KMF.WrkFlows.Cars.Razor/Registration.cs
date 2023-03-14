using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MYKEY.FxCore.Razor;
using MYKEY.FxCore.Web.Razor;

namespace KMF.WrkFlows.Cars.Razor
{
    class Registration : IModule
    {
        public string Name => "Cars";
        public List<NavItem> NavItems => new List<NavItem> {
            new NavItem { Name = "Fahrzeuge", Url = "/carmanagement/init" }
        };
    }
}
