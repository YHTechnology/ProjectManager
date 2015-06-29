using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;

namespace ChangeUserPassword
{
    class MainViewModel : INotifyPropertyChanged
    {
        private MySqlConnection mycon;

        public DelegateCommand ConnectCommand { get; set; }

        public MainViewModel()
        {
            this.ConnectCommand = new DelegateCommand();
            ConnectCommand.ActionExecute = new Action<object>(Connect);
            mycon = new MySqlConnection();
        }

        private string databaseName = "productmanage";

        public string DatabaseName
        {
            get { return databaseName; }
            set
            {
                databaseName = value;
                RaisePropertyChanged("DatabaseName");
            }
        }


        private string serverName = "localhost";

        public string ServerName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                RaisePropertyChanged("ServerName");
            }
        }

        private string loginName = "root";

        public string LoginName
        {
            get { return loginName; }
            set
            {
                loginName = value;
                RaisePropertyChanged("LoginName");
            }
        }


        private string loginPassword = "123456";

        public string LoginPassword
        {
            get { return loginPassword; }
            set
            {
                loginPassword = value;
                RaisePropertyChanged("LoginPassword");
            }
        }

        private int serverPort = 3306;

        public int ServerPort
        {
            get { return serverPort; }
            set
            {
                serverPort = value;
                RaisePropertyChanged("ServerPort");
            }
        }

        private ObservableCollection<User> users = new ObservableCollection<User>();

        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChanged("Users");
            }
        }

        private User selectedUser;
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                RaisePropertyChanged("SelectedUser");
            }
        }

        private void ChangePassword(User user)
        {
            if (mycon != null)
            {
                if (mycon.State != ConnectionState.Closed)
                {
                    mycon.Close();
                }

                try
                {
                    mycon.Open();
                    MySqlCommand queryDataBaseCommand = new MySqlCommand(string.Format("update user set user_password='{0}' where user_id={1}", MD5.GetMd5String(user.密码), user.序列号), mycon);
                    queryDataBaseCommand.ExecuteNonQuery();
                    mycon.Close();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Connect(object o)
        {
            if (mycon != null)
            {
                if (mycon.State != ConnectionState.Closed)
                {
                    try
                    {
                        mycon.Close();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }

                string connectionString = string.Format("server={0};port={1};uid={2};pwd={3};DataBase={4}", ServerName, ServerPort, LoginName, LoginPassword, DatabaseName);
                try
                {
                    mycon.ConnectionString = connectionString;
                    mycon.Open();

                    MySqlCommand queryDataBaseCommand = new MySqlCommand("select * from user", mycon);
                    DataSet ds = new DataSet();
                    MySqlDataReader reader = queryDataBaseCommand.ExecuteReader();

                    Users.Clear();
                    while (reader.Read())
                    {
                        int index = reader.GetInt32("user_id");
                        string strName = reader.GetString("user_name");
                        string strPassword = reader.GetString("user_password");
                        User user = new User() { 序列号 = index, 名字 = strName, 密码 = strPassword };
                        user.changePassword += ChangePassword;

                        Users.Add(user);
                    }

                    mycon.Close();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void selectOneItem(object o)
        {
            string str = o.GetType().FullName;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public delegate void UserPasswordChange(User user);

    public class User
    {
        private bool ischanged = false;
        private string password;

        public UserPasswordChange changePassword;

        public int 序列号 { get; set; }
        public string 名字 { get; set; }
        public string 密码
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                if (ischanged)
                {
                    changePassword(this);
                }

                ischanged = true;
            }
        }
    }
}
