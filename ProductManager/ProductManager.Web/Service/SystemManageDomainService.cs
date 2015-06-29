
namespace ProductManager.Web.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using ProductManager.Web.Model;


    // 使用 productmanageEntities 上下文实现应用程序逻辑。
    // TODO: 将应用程序逻辑添加到这些方法中或其他方法中。
    // TODO: 连接身份验证(Windows/ASP.NET Forms)并取消注释以下内容，以禁用匿名访问
    //还可考虑添加角色，以根据需要限制访问。
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class SystemManageDomainService : LinqToEntitiesDomainService<productmanageEntities>
    {

//         protected override int Count<T>(IQueryable<T> query)
//         {
//             return query.Count();
//         }

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“action”查询添加顺序。
        public IQueryable<action> GetAction()
        {
            return this.ObjectContext.action;
        }

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“department”查询添加顺序。
        public IQueryable<department> GetDepartment()
        {
            return this.ObjectContext.department;
        }

        public void InsertDepartment(department department)
        {
            if ((department.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(department, EntityState.Added);
            }
            else
            {
                this.ObjectContext.department.AddObject(department);
            }
        }

        public void UpdateDepartment(department currentdepartment)
        {
            this.ObjectContext.department.AttachAsModified(currentdepartment, this.ChangeSet.GetOriginal(currentdepartment));
        }

        public void DeleteDepartment(department department)
        {
            if ((department.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(department, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.department.Attach(department);
                this.ObjectContext.department.DeleteObject(department);
            }
        }

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“role”查询添加顺序。
        public IQueryable<role> GetRole()
        {
            return this.ObjectContext.role;
        }

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“role_action”查询添加顺序。
        public IQueryable<role_action> GetRole_action()
        {
            return this.ObjectContext.role_action;
        }

        [Query]
        public IQueryable<role_action> GetRole_actionByRoleID(int aRoleID)
        {
            return this.ObjectContext.role_action.Where(c => c.role_id == aRoleID);
        }

        public void InsertRole_action(role_action role_action)
        {
            if ((role_action.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(role_action, EntityState.Added);
            }
            else
            {
                this.ObjectContext.role_action.AddObject(role_action);
            }
        }

        public void UpdateRole_action(role_action currentrole_action)
        {
            this.ObjectContext.role_action.AttachAsModified(currentrole_action, this.ChangeSet.GetOriginal(currentrole_action));
        }

        public void DeleteRole_action(role_action role_action)
        {
            if ((role_action.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(role_action, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.role_action.Attach(role_action);
                this.ObjectContext.role_action.DeleteObject(role_action);
            }
        }

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“user”查询添加顺序。
        [Invoke]
        public int GetUserTotalPages(int aPageSize)
        {
            return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(this.ObjectContext.user.Include("department").Count()) / Convert.ToDouble(aPageSize)));
        }

        [Query]
        public IQueryable<user> GetUserByPage(int aPageSize, int aCurrentPage)
        {
            return this.ObjectContext.user.Include("department").OrderByDescending(e => e.user_id).Skip(aPageSize * (aCurrentPage)).Take(aPageSize);
        }

        [Query]
        public IQueryable<user> GetUser()
        {
            return this.ObjectContext.user.Include("department").OrderBy(e => e.user_id);
        }

        [Insert]
        public void InsertUser(user user)
        {
            if ((user.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user, EntityState.Added);
            }
            else
            {
                this.ObjectContext.user.AddObject(user);
            }
        }

        [Update]
        public void UpdateUser(user currentuser)
        {
            this.ObjectContext.user.AttachAsModified(currentuser, this.ChangeSet.GetOriginal(currentuser));
        }

        [Delete]
        public void DeleteUser(user user)
        {
            if ((user.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.user.Attach(user);
                this.ObjectContext.user.DeleteObject(user);
            }
        }

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“user_action”查询添加顺序。
        public IQueryable<user_action> GetUser_action()
        {
            return this.ObjectContext.user_action;
        }

        [Query]
        public IQueryable<user_action> GetUser_actionByUserID(int aUserID)
        {
            return this.ObjectContext.user_action.Where(c => c.user_id == aUserID);
        }

        public void InsertUser_action(user_action user_action)
        {
            if ((user_action.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user_action, EntityState.Added);
            }
            else
            {
                this.ObjectContext.user_action.AddObject(user_action);
            }
        }

        public void UpdateUser_action(user_action currentuser_action)
        {
            this.ObjectContext.user_action.AttachAsModified(currentuser_action, this.ChangeSet.GetOriginal(currentuser_action));
        }

        public void DeleteUser_action(user_action user_action)
        {
            if ((user_action.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user_action, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.user_action.Attach(user_action);
                this.ObjectContext.user_action.DeleteObject(user_action);
            }
        }

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“user_role”查询添加顺序。
        public IQueryable<user_role> GetUser_role()
        {
            return this.ObjectContext.user_role;
        }

        public void InsertUser_role(user_role user_role)
        {
            if ((user_role.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user_role, EntityState.Added);
            }
            else
            {
                this.ObjectContext.user_role.AddObject(user_role);
            }
        }

        public void UpdateUser_role(user_role currentuser_role)
        {
            this.ObjectContext.user_role.AttachAsModified(currentuser_role, this.ChangeSet.GetOriginal(currentuser_role));
        }

        public void DeleteUser_role(user_role user_role)
        {
            if ((user_role.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user_role, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.user_role.Attach(user_role);
                this.ObjectContext.user_role.DeleteObject(user_role);
            }
        }

        public IQueryable<filetype> GetFiletype()
        {
            return this.ObjectContext.filetype;
        }

        public void InsertFiletype(filetype filetype)
        {
            if ((filetype.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(filetype, EntityState.Added);
            }
            else
            {
                this.ObjectContext.filetype.AddObject(filetype);
            }
        }

        public void UpdateFiletype(filetype currentfiletype)
        {
            this.ObjectContext.filetype.AttachAsModified(currentfiletype, this.ChangeSet.GetOriginal(currentfiletype));
        }

        public void DeleteFiletype(filetype filetype)
        {
            if ((filetype.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(filetype, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.filetype.Attach(filetype);
                this.ObjectContext.filetype.DeleteObject(filetype);
            }
        }

        public IQueryable<product_type> GetProduct_type()
        {
            return this.ObjectContext.product_type;
        }

        public void InsertProduct_type(product_type product_type)
        {
            if (product_type.EntityState != EntityState.Detached)
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(product_type, EntityState.Added);
            }
            else
            {
                this.ObjectContext.product_type.AddObject(product_type);
            }
        }

        public void UpdateProduct_type(product_type currentfiletype)
        {
            this.ObjectContext.product_type.AttachAsModified(currentfiletype, this.ChangeSet.GetOriginal(currentfiletype));
        }

        public void DeleteProduct_type(product_type product_type)
        {
            if ((product_type.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(product_type, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.product_type.Attach(product_type);
                this.ObjectContext.product_type.DeleteObject(product_type);
            }
        }

        public IQueryable<product_part_type> GetProduct_part_type()
        {
            return this.ObjectContext.product_part_type;
        }

        public void InsertProduct_part_type(product_part_type productPartType)
        {
            if (productPartType.EntityState != EntityState.Detached)
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(productPartType, EntityState.Added);
            } 
            else
            {
                this.ObjectContext.product_part_type.AddObject(productPartType);
            }
        }

        public void UpdateProduct_part_type(product_part_type productPartType)
        {
            this.ObjectContext.product_part_type.AttachAsModified(productPartType, this.ChangeSet.GetOriginal(productPartType));
        }

        public void DeleteProduct_part_type(product_part_type productPartType)
        {
            if ((productPartType.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(productPartType, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.product_part_type.Attach(productPartType);
                this.ObjectContext.product_part_type.DeleteObject(productPartType);
            }
        }

        static Dictionary<int, String> DictionaryLogonUser = new Dictionary<int, String>();

        [Query]
        public IQueryable<user> GetLogonUser()
        {
            using (productmanageEntities productmanageEntities = new productmanageEntities())
            {
                List<user> lUserList = new List<user>();
                foreach (KeyValuePair<int, String> pair in DictionaryLogonUser)
                {
                    var lRes = from r in productmanageEntities.user where r.user_id == pair.Key select r;
                    lUserList.AddRange(lRes.ToList());
                }
                return lUserList.AsQueryable<user>();
            }
        }

        public void LogonUser(int aUserId, String aUserName)
        {
            String lUserName;
            if (!DictionaryLogonUser.TryGetValue(aUserId, out lUserName))
            {
                DictionaryLogonUser.Add(aUserId, aUserName);
            }
        }

        public void LogoutUser(int aUserId, String aUserName)
        {
            String lUserName;
            if (DictionaryLogonUser.TryGetValue(aUserId, out lUserName))
            {
                DictionaryLogonUser.Remove(aUserId);
            }
        }
    }
}


