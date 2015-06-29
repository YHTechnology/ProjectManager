
namespace ProductManager.Web.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using ProductManager.Web.Model;


    // TODO: 创建包含应用程序逻辑的方法。
    [EnableClientAccess()]
    public class OperationDomainService : DomainService
    {
        //static Dictionary<int, String> DictionaryLogonUser = new Dictionary<int, String>();

        public int GetMaxUserID()
        {
            using (productmanageEntities productmanageEntities = new productmanageEntities())
            {
                int maxID = productmanageEntities.user.Max(w => w.user_id);
                return maxID;
            }
        }

        public bool ModifyPassword(int aUserID, String aNewPassword)
        {
            using (productmanageEntities productmanageEntities = new productmanageEntities())
            {
                var lUserList = from r in productmanageEntities.user where r.user_id == aUserID select r;
                if (lUserList.Count() > 0)
                {
                    user lUser = lUserList.First();
                    lUser.user_password = aNewPassword;
                    productmanageEntities.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

//         public IQueryable<user> GetLogonUser()
//         {
//             using (productmanageEntities productmanageEntities = new productmanageEntities())
//             {
//                 List<user> lUserList = new List<user>();
//                 foreach (KeyValuePair<int, String> pair in DictionaryLogonUser)
//                 {
//                     var lRes = from r in productmanageEntities.user where r.user_id == pair.Key select r;
//                     lUserList.AddRange(lRes.ToList());
//                 }
//                 return lUserList.AsQueryable<user>();
//             }
//         }
// 
//         public void LogonUser(int aUserId, String aUserName)
//         {
//             String lUserName;
//             if (!DictionaryLogonUser.TryGetValue(aUserId, out lUserName))
//             {
//                 DictionaryLogonUser.Add(aUserId, aUserName);
//             }
//         }
// 
//         public void LogoutUser(int aUserId, String aUserName)
//         {
//             String lUserName;
//             if (DictionaryLogonUser.TryGetValue(aUserId, out lUserName))
//             {
//                 DictionaryLogonUser.Remove(aUserId);
//             }
//         }

    }
}


