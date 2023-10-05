using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
    public static class AuthorizeUtilities
    {
        internal static List<string> IGNORE_USERS { get; set; }
        private static IUnitOfWork UnitOfWork = new NHUnitOfWork();
        /// <summary>
        /// Kiểm tra xem user current có nắm trong list user full quyền hay không
        /// </summary>
        /// <returns></returns>
        public static bool CheckIgnoreUser(string userName)
        {
            if (IGNORE_USERS != null)
            {
                return IGNORE_USERS.Contains(userName);
            }
            return false;
        }

        public static bool CheckUserRight(string right)
        {
            if (ProfileUtilities.User.IS_IGNORE_USER)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(right))
            {
                return false;
            }

            var lstRight = right.Split(',');
            foreach (var item in lstRight)
            {
                if (ProfileUtilities.UserRight.Select(x => x.CODE).Contains(item))
                {
                    return true;
                }
            }
            
            return false;
        }

        public static bool CheckUserRightProject(string right, Guid projectId, string userName = "")
        {
            if (string.IsNullOrWhiteSpace(right))
            {
                return false;
            }

            var lstRight = right.Split(',');
            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = ProfileUtilities.User.USER_NAME;
            }

            if (userName == ProfileUtilities.User.USER_NAME && ProfileUtilities.User.IS_IGNORE_USER)
            {
                return true;
            }

            var result = false;
            try
            {
                var find = ProfileUtilities.UserProjectRight
                    .FirstOrDefault(x => x.PROJECT_ID == projectId && x.USER_NAME == userName && lstRight.Contains(x.RIGHT_CODE));
                if (find != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
    }
}