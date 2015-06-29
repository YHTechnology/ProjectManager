
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
    public class ProductDomainService : LinqToEntitiesDomainService<productmanageEntities>
    {

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“product”查询添加顺序。
        public IQueryable<product> GetProduct()
        {
            return this.ObjectContext.product.OrderBy(e => e.product_id);
        }

        public void InsertProduct(product product)
        {
            if ((product.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(product, EntityState.Added);
            }
            else
            {
                this.ObjectContext.product.AddObject(product);
            }
        }

        public void UpdateProduct(product currentproduct)
        {
            this.ObjectContext.product.AttachAsModified(currentproduct, this.ChangeSet.GetOriginal(currentproduct));
        }

        public void DeleteProduct(product product)
        {
            if ((product.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(product, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.product.Attach(product);
                this.ObjectContext.product.DeleteObject(product);
            }
        }

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“project”查询添加顺序。
        public IQueryable<project> GetProject()
        {
            return this.ObjectContext.project.OrderByDescending(e => e.record_date);
        }

        public IQueryable<project> GetProjectByRespPersonName(String aPersonName)
        {
            if (String.IsNullOrEmpty(aPersonName))
            {
                var lRes = (from n in this.ObjectContext.project
                       from p in this.ObjectContext.project_responsible
                       where n.manufacture_number == p.manufacture_number
                       select n).Distinct();
                return lRes;
            }
            else
            {
                var lRes = (from n in this.ObjectContext.project
                       from p in this.ObjectContext.project_responsible
                       where n.manufacture_number == p.manufacture_number && p.responsible_persionName.Contains(aPersonName)
                       select n).Distinct();
                return lRes;
            }
        }

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

        public void UpdateProject(project currentproject)
        {
            this.ObjectContext.project.AttachAsModified(currentproject, this.ChangeSet.GetOriginal(currentproject));
        }

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
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“project_files”查询添加顺序。
        public IQueryable<project_files> GetProject_files()
        {
            return this.ObjectContext.project_files;
        }

        public IQueryable<project_files> GetProject_filesByID(String aID)
        {
            return this.ObjectContext.project_files.Where(e => e.manufacture_number == aID);
        }

        public void InsertProject_files(project_files project_files)
        {
            if ((project_files.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(project_files, EntityState.Added);
            }
            else
            {
                this.ObjectContext.project_files.AddObject(project_files);
            }
        }

        public void UpdateProject_files(project_files currentproject_files)
        {
            this.ObjectContext.project_files.AttachAsModified(currentproject_files, this.ChangeSet.GetOriginal(currentproject_files));
        }

        public void DeleteProject_files(project_files project_files)
        {
            if ((project_files.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(project_files, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.project_files.Attach(project_files);
                this.ObjectContext.project_files.DeleteObject(project_files);
            }
        }

        // TODO:
        // 考虑约束查询方法的结果。如果需要其他输入，
        //可向此方法添加参数或创建具有不同名称的其他查询方法。
        // 为支持分页，需要向“project_responsible”查询添加顺序。
        public IQueryable<project_responsible> GetProject_responsible()
        {
            return this.ObjectContext.project_responsible.OrderBy(e => e.department_id);
        }

        public void InsertProject_responsible(project_responsible project_responsible)
        {
            if ((project_responsible.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(project_responsible, EntityState.Added);
            }
            else
            {
                this.ObjectContext.project_responsible.AddObject(project_responsible);
            }
        }

        public void UpdateProject_responsible(project_responsible currentproject_responsible)
        {
            this.ObjectContext.project_responsible.AttachAsModified(currentproject_responsible, this.ChangeSet.GetOriginal(currentproject_responsible));
        }

        public void DeleteProject_responsible(project_responsible project_responsible)
        {
            if ((project_responsible.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(project_responsible, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.project_responsible.Attach(project_responsible);
                this.ObjectContext.project_responsible.DeleteObject(project_responsible);
            }
        }

        public IQueryable<questiontrace> GetQuestiontrace()
        {
            return this.ObjectContext.questiontrace.OrderByDescending(e => e.question_starttime);
        }

        public void InsertQuestiontrace(questiontrace questiontrace)
        {
            if ((questiontrace.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(questiontrace, EntityState.Added);
            }
            else
            {
                this.ObjectContext.questiontrace.AddObject(questiontrace);
            }
        }

        public void UpdateQuestiontrace(questiontrace currentquestiontrace)
        {
            this.ObjectContext.questiontrace.AttachAsModified(currentquestiontrace, this.ChangeSet.GetOriginal(currentquestiontrace));
        }

        public void DeleteQuestiontrace(questiontrace questiontrace)
        {
            if ((questiontrace.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(questiontrace, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.questiontrace.Attach(questiontrace);
                this.ObjectContext.questiontrace.DeleteObject(questiontrace);
            }
        }

        public IQueryable<product_part_time> GetProduct_part_time()
        {
            return this.ObjectContext.product_part_time.OrderBy(e => e.product_part_id);
        }

        public void InsertProduct_part_time(product_part_time product_part_time)
        {
            if ((product_part_time.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(product_part_time, EntityState.Added);
            }
            else
            {
                this.ObjectContext.product_part_time.AddObject(product_part_time);
            }
        }

        public void UpdateProduct_part_time(product_part_time currentproduct_part_time)
        {
            this.ObjectContext.product_part_time.AttachAsModified(currentproduct_part_time, this.ChangeSet.GetOriginal(currentproduct_part_time));
        }

        public void DeleteProduct_part_time(product_part_time product_part_time)
        {
            if ((product_part_time.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(product_part_time, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.product_part_time.Attach(product_part_time);
                this.ObjectContext.product_part_time.DeleteObject(product_part_time);
            }
        }

        public IQueryable<important_part> GetImportant_part()
        {
            return this.ObjectContext.important_part.OrderBy(e => e.arive_time);
        }

        public void InsertImportant_part(important_part important_part)
        {
            if ((important_part.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(important_part, EntityState.Added);
            }
            else
            {
                this.ObjectContext.important_part.AddObject(important_part);
            }
        }

        public void UpdateImportant_part(important_part currentimportant_part)
        {
            this.ObjectContext.important_part.AttachAsModified(currentimportant_part, this.ChangeSet.GetOriginal(currentimportant_part));
        }

        public void DeleteImportant_part(important_part important_part)
        {
            if ((important_part.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(important_part, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.important_part.Attach(important_part);
                this.ObjectContext.important_part.DeleteObject(important_part);
            }
        }

        public IQueryable<user_project> GetUserProject(int aUserId)
        {
            return this.ObjectContext.user_project.Where(e => e.user_id == aUserId);
        }

        public void InsertUserProject(user_project user_project)
        {
            if ((user_project.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user_project, EntityState.Added);
            }
            else
            {
                this.ObjectContext.user_project.AddObject(user_project);
            }
        }

        public void UpdateUserProject(user_project currentuser_project)
        {
            this.ObjectContext.user_project.AttachAsModified(currentuser_project, this.ChangeSet.GetOriginal(currentuser_project));
        }

        public void DeleteUserProject(user_project user_project)
        {
            if ((user_project.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(user_project, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.user_project.Attach(user_project);
                this.ObjectContext.user_project.DeleteObject(user_project);
            }
        }

        public IQueryable<important_part_rejester> GetImportantPartRejester()
        {
            return this.ObjectContext.important_part_rejester;
        }

        public void InsertImportantPartRejester(important_part_rejester important_part_rejester)
        {
            if ((important_part_rejester.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(important_part_rejester, EntityState.Added);
            }
            else
            {
                this.ObjectContext.important_part_rejester.AddObject(important_part_rejester);
            }
        }

        public void UpdateImportantPartRejester(important_part_rejester currentimportant_part_rejester)
        {
            this.ObjectContext.important_part_rejester.AttachAsModified(currentimportant_part_rejester, this.ChangeSet.GetOriginal(currentimportant_part_rejester));
        }

        public void DeleteImportantPartRejester(important_part_rejester important_part_rejester)
        {
            if ((important_part_rejester.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(important_part_rejester, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.important_part_rejester.Attach(important_part_rejester);
                this.ObjectContext.important_part_rejester.DeleteObject(important_part_rejester);
            }
        }
    }
}


