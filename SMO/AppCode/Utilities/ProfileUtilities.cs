using SMO.Core.Entities;
using SMO.Core.Entities.PS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
    public class ProfileUtilities
    {
        public static T_AD_USER User
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["Profile"] != null)
                {
                    return HttpContext.Current.Session["Profile"] as T_AD_USER;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["Profile"] = value;
            }
        }

        public static List<T_AD_RIGHT> UserRight
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["Profile"] != null)
                {
                    return HttpContext.Current.Session["UserRight"] as List<T_AD_RIGHT>;
                }
                else
                {
                    return new List<T_AD_RIGHT>();
                }
            }
            set
            {
                HttpContext.Current.Session["UserRight"] = value;
            }
        }

        public static List<T_PS_PROJECT_USER_RIGHT> UserProjectRight
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["Profile"] != null)
                {
                    return HttpContext.Current.Session["UserProjectRight"] as List<T_PS_PROJECT_USER_RIGHT>;
                }
                else
                {
                    return new List<T_PS_PROJECT_USER_RIGHT>();
                }
            }
            set
            {
                HttpContext.Current.Session["UserProjectRight"] = value;
            }
        }
    }
}