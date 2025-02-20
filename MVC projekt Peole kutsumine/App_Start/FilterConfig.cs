using System.Web;
using System.Web.Mvc;

namespace MVC_projekt_Peole_kutsumine
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
