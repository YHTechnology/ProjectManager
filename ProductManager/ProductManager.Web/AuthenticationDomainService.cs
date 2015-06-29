using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.ServiceModel.DomainServices.Server.ApplicationServices;
using ProductManager.Web.Model;

namespace ProductManager.Web
{
    [EnableClientAccess]
    public class AuthenticationDomainService : AuthenticationBase<User>
    {
        protected override User GetAuthenticatedUser(IPrincipal principal)
        {
            using (productmanageEntities productmanageEntities = new productmanageEntities())
            {
                User user = new User();
                try
                {
                    var result = from r in productmanageEntities.user where r.user_name == principal.Identity.Name select r;
                    if (result.Count() > 0)
                    {
                        ProductManager.Web.Model.user lUser = result.First();
                        user.Name = lUser.user_name;
                        user.UserName = lUser.user_cname;
                        
                        user.RightDictionary = new Dictionary<int,bool>();
                        user.IsFreeze = lUser.user_is_freeze.GetValueOrDefault(false);
                        user.DepartmentID = lUser.user_department_id.GetValueOrDefault(0);
                        user.UserID = lUser.user_id;
                        user.Password = lUser.user_password;
                        user.IsManager = lUser.isManager.GetValueOrDefault(false);

                        if (lUser.user_department_id.HasValue)
                        {
                            user.Department = lUser.department.department_name;
                        }

                        if (lUser.user_name == "admin")
                        {
                            {
                                var actionresult = from rs in productmanageEntities.action select rs;
                                foreach (ProductManager.Web.Model.action action in actionresult)
                                {
                                    user.RightDictionary.Add(action.action_id, true);
                                }
                                
                            }
                        }
                        else
                        {
                            //Role action 
                            {
                                var roleactionresult = from rs in productmanageEntities.role_action where rs.role_id == lUser.user_department_id select rs;
                                //var roleactionresult = from rs in productmanageEntities.role_action where rs.role_id == userrole.role_id select rs;
                                foreach (ProductManager.Web.Model.role_action roleaction in roleactionresult)
                                {
                                    bool lIsPermit;
                                    if (user.RightDictionary.TryGetValue(roleaction.action_id.Value, out lIsPermit))
                                    {
                                        user.RightDictionary.Remove(roleaction.action_id.Value);
                                    }
                                    user.RightDictionary.Add(roleaction.action_id.Value, roleaction.isPermit.Value);
                                }
                            }

                            {
                                var useractionresult = from r in productmanageEntities.user_action where r.user_id == lUser.user_id select r;
                                foreach (ProductManager.Web.Model.user_action useraction in useractionresult)
                                {
                                    bool lIsPermit;
                                    if (user.RightDictionary.TryGetValue(useraction.action_id.Value, out lIsPermit))
                                    {
                                        user.RightDictionary.Remove(useraction.action_id.Value);
                                    }
                                    user.RightDictionary.Add(useraction.action_id.Value, useraction.isPermit.Value);
                                }
                            }
                        }
                        
                    }
                }
                catch (System.Exception ex)
                {
                }
                return user;
            }
        }
    }

    public class User : UserBase
    {
        public int UserID { get; set; }
        public int DepartmentID { get; set; }
        public string UserName { get; set; }
        public bool IsFreeze { get; set; }
        public bool IsManager { get; set; }
        public string Department { get; set; }
        public string Password { get; set; }
        public Dictionary<int, bool> RightDictionary { get; set; }
    }
}