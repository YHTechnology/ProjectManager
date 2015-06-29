using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ProductManager.ViewData.Entity;
using ProductManager.Web.Service;

namespace ProductManager.ViewModel.SystemManager
{
    [Export("OnLineUserList")]
    public class OnLineUserListViewModel : NotifyPropertyChanged
    {
        private SystemManageDomainContext systemManageDomainContext; //= new SystemManageDomainContext();

        public ObservableCollection<UserEntity> OnLineUserList {get;set;}

        public OnLineUserListViewModel()
        {
            OnLineUserList = new ObservableCollection<UserEntity>();
            OnRefash = new DelegateCommand(OnRefashCommand);
        }

        public void LoadData()
        {
            OnLineUserList.Clear();
            systemManageDomainContext = new SystemManageDomainContext();
            LoadOperation<ProductManager.Web.Model.user> loadOnLineUser =
                     systemManageDomainContext.Load<ProductManager.Web.Model.user>(systemManageDomainContext.GetLogonUserQuery());
                     loadOnLineUser.Completed += loadOperationloadOnLineUser_Completed;

        }

        private void loadOperationloadOnLineUser_Completed(object sender, EventArgs e)
        {
            LoadOperation loadOpseration = sender as LoadOperation;
            foreach (ProductManager.Web.Model.user user in loadOpseration.Entities)
            {
                UserEntity userEntity = new UserEntity();
                userEntity.User = user;
                //userEntity.UserNameDictionary = UserEntityDictionary;
                userEntity.Update();
                OnLineUserList.Add(userEntity);
            }
        }

        public ICommand OnRefash { get; private set; }

        public void OnRefashCommand()
        {
            LoadData();
        }
    }
}
