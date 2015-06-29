
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


    // Implements application logic using the productmanageEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class PlanManagerDomainService : LinqToEntitiesDomainService<productmanageEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'action' query.
        [Query]
        public IQueryable<action> GetAction()
        {
            return this.ObjectContext.action;
        }

        [Insert]
        public void InsertAction(action action)
        {
            if ((action.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(action, EntityState.Added);
            }
            else
            {
                this.ObjectContext.action.AddObject(action);
            }
        }

        [Update]
        public void UpdateAction(action currentaction)
        {
            this.ObjectContext.action.AttachAsModified(currentaction, this.ChangeSet.GetOriginal(currentaction));
        }

        [Delete]
        public void DeleteAction(action action)
        {
            if ((action.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(action, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.action.Attach(action);
                this.ObjectContext.action.DeleteObject(action);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'department' query.
        [Query]
        public IQueryable<department> GetDepartment()
        {
            return this.ObjectContext.department;
        }

        [Insert]
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

        [Update]
        public void UpdateDepartment(department currentdepartment)
        {
            this.ObjectContext.department.AttachAsModified(currentdepartment, this.ChangeSet.GetOriginal(currentdepartment));
        }
        
        [Delete]
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
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'plan' query.
        [Query]
        public IQueryable<plan> GetPlan()
        {
            return this.ObjectContext.plan;
        }

        [Query]
        public IQueryable<plan> GetPlanNew()
        {
            var result = from c in this.ObjectContext.plan
                         from o in this.ObjectContext.project
                         where c.manufacture_number == o.manufacture_number && c.version_id == o.plan_version_id
                         select c;
            return result;
        }

        [Insert]
        public void InsertPlan(plan plan)
        {
            if ((plan.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(plan, EntityState.Added);
            }
            else
            {
                this.ObjectContext.plan.AddObject(plan);
            }
        }

        [Update]
        public void UpdatePlan(plan currentplan)
        {
            this.ObjectContext.plan.AttachAsModified(currentplan, this.ChangeSet.GetOriginal(currentplan));
        }

        [Delete]
        public void DeletePlan(plan plan)
        {
            if ((plan.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(plan, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.plan.Attach(plan);
                this.ObjectContext.plan.DeleteObject(plan);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'project' query.
        [Query]
        public IQueryable<project> GetProject()
        {
            return this.ObjectContext.project.OrderByDescending(e => e.record_date);
        }

        [Insert]
        public void InsertProject(project project)
        {
            if ((project.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(project, EntityState.Added);
            }
            else
            {
                this.ObjectContext.project.AddObject(project);
            }
        }

        [Update]
        public void UpdateProject(project currentproject)
        {
            this.ObjectContext.project.AttachAsModified(currentproject, this.ChangeSet.GetOriginal(currentproject));
        }

        [Delete]
        public void DeleteProject(project project)
        {
            if ((project.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(project, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.project.Attach(project);
                this.ObjectContext.project.DeleteObject(project);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'role' query.
        [Query]
        public IQueryable<role> GetRole()
        {
            return this.ObjectContext.role;
        }

        [Insert]
        public void InsertRole(role role)
        {
            if ((role.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(role, EntityState.Added);
            }
            else
            {
                this.ObjectContext.role.AddObject(role);
            }
        }

        [Update]
        public void UpdateRole(role currentrole)
        {
            this.ObjectContext.role.AttachAsModified(currentrole, this.ChangeSet.GetOriginal(currentrole));
        }

        [Delete]
        public void DeleteRole(role role)
        {
            if ((role.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(role, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.role.Attach(role);
                this.ObjectContext.role.DeleteObject(role);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'role_action' query.
        [Query]
        public IQueryable<role_action> GetRole_action()
        {
            return this.ObjectContext.role_action;
        }

        [Insert]
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

        [Update]
        public void UpdateRole_action(role_action currentrole_action)
        {
            this.ObjectContext.role_action.AttachAsModified(currentrole_action, this.ChangeSet.GetOriginal(currentrole_action));
        }

        [Delete]
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
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'user' query.
        [Query]
        public IQueryable<user> GetUser()
        {
            return this.ObjectContext.user;
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
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'user_action' query.
        [Query]
        public IQueryable<user_action> GetUser_action()
        {
            return this.ObjectContext.user_action;
        }

        [Insert]
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

        [Update]
        public void UpdateUser_action(user_action currentuser_action)
        {
            this.ObjectContext.user_action.AttachAsModified(currentuser_action, this.ChangeSet.GetOriginal(currentuser_action));
        }

        [Delete]
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
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'user_role' query.
        [Query]
        public IQueryable<user_role> GetUser_role()
        {
            return this.ObjectContext.user_role;
        }

        [Insert]
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

        [Update]
        public void UpdateUser_role(user_role currentuser_role)
        {
            this.ObjectContext.user_role.AttachAsModified(currentuser_role, this.ChangeSet.GetOriginal(currentuser_role));
        }

        [Delete]
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

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'plan_extra' query.
        [Query]
        public IQueryable<plan_extra> GetPlan_extra()
        {
            return this.ObjectContext.plan_extra;
        }

        [Insert]
        public void InsertPlan_extra(plan_extra plan_extra)
        {
            if ((plan_extra.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(plan_extra, EntityState.Added);
            }
            else
            {
                this.ObjectContext.plan_extra.AddObject(plan_extra);
            }
        }

        [Update]
        public void UpdatePlan_extra(plan_extra currentuser_role)
        {
            this.ObjectContext.plan_extra.AttachAsModified(currentuser_role, this.ChangeSet.GetOriginal(currentuser_role));
        }

        [Delete]
        public void DeletePlan_extra(plan_extra plan_extra)
        {
            if ((plan_extra.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(plan_extra, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.plan_extra.Attach(plan_extra);
                this.ObjectContext.plan_extra.DeleteObject(plan_extra);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'plan_outline_files' query.
        [Query]
        public IQueryable<plan_outline_files> Getplan_outline_files()
        {
            return this.ObjectContext.plan_outline_files.OrderByDescending(e => e.file_uploadtime);
        }

        [Insert]
        public void Insertplan_outline_files(plan_outline_files plan_outline_files)
        {
            if ((plan_outline_files.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(plan_outline_files, EntityState.Added);
            }
            else
            {
                this.ObjectContext.plan_outline_files.AddObject(plan_outline_files);
            }
        }

        [Update]
        public void Updateplan_outline_files(plan_outline_files currentuser_role)
        {
            this.ObjectContext.plan_outline_files.AttachAsModified(currentuser_role, this.ChangeSet.GetOriginal(currentuser_role));
        }

        [Delete]
        public void Deleteplan_outline_files(plan_outline_files plan_outline_files)
        {
            if ((plan_outline_files.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(plan_outline_files, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.plan_outline_files.Attach(plan_outline_files);
                this.ObjectContext.plan_outline_files.DeleteObject(plan_outline_files);
            }
        }

        [Query]
        public IQueryable<user_remind> GetUserRemind(int aUserID)
        {
            return this.ObjectContext.user_remind.Where(e => e.user_id == aUserID);
        }

        [Insert]
        public void InsertUserRemind(user_remind user_remind)
        {
            if (user_remind.EntityState != EntityState.Detached)
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user_remind, EntityState.Added);
            }
            else
            {
                this.ObjectContext.user_remind.AddObject(user_remind);
            }
        }

        [Update]
        public void UpdateUserRemind(user_remind user_remind)
        {
            this.ObjectContext.user_remind.AttachAsModified(user_remind, this.ChangeSet.GetOriginal(user_remind));
        }

        [Delete]
        public void DeleteUserRemind(user_remind user_remind)
        {
            if (user_remind.EntityState != EntityState.Detached)
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user_remind, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.user_remind.Attach(user_remind);
                this.ObjectContext.user_remind.DeleteObject(user_remind);
            }
        }

    }
}


