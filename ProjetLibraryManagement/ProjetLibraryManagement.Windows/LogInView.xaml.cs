using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ClassLibrary1;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ProjetLibraryManagement
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogInView : Page
    {
        UserCollection uCollect = UserCollection.GetInstance();
        public LogInView()
        {
            this.InitializeComponent();
            if (InterfaceManager.ConnectNum == 0)
                uCollect.SetUser();


        }

        //allow to the user to connect and send exception if user isn't registered
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (userName.Text != string.Empty && password.Text != string.Empty)
            {
                User temp = uCollect.GetUser(userName.Text, password.Text);
                if (temp == default(User))
                {
                    tBValidation.Foreground = new SolidColorBrush(Colors.Red);
                    tBValidation.Text = "Username & passwords are not matching";
                    ClearUI();
                }
                else
                {
                    InterfaceManager.ActualUser = temp;
                    this.Frame.Navigate(typeof(ItemView));
                }
            }
            else
            {
                tBValidation.Foreground = new SolidColorBrush(Colors.Red);
                tBValidation.Text = "Username and/or passwords are empty";

            }
        }

        bool wantToRegister = false;//allow user to create a client account and check if all data are valid
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            LogIn.Visibility = Visibility.Collapsed;

            if (!wantToRegister)
            {
                ConfirmPassword.Visibility = Visibility.Visible;
                wantToRegister = true;
                tBValidation.Text = "Enter your data..";
                tBValidation.Foreground = new SolidColorBrush(Colors.LightGreen);
            }
            else if (password.Text == tBConfirmPassword.Text)
            {
                if (!InterfaceManager.ValidPassword(password.Text))
                {
                    tBValidation.Foreground = new SolidColorBrush(Colors.Red);
                    tBValidation.Text = "PassWord have to contain 8 characteres, upper and lower case";
                }
                else if (!uCollect.IsUser(userName.Text))
                {
                    User temp = new User(AutorisationLevel.Client, userName.Text, tBConfirmPassword.Text);
                    uCollect.AddUser(temp);
                    tBValidation.Text = "Your account have been created.. you can login..";
                    tBValidation.Foreground = new SolidColorBrush(Colors.LightGreen);
                    ConfirmPassword.Visibility = Visibility.Collapsed;
                    ClearUI();
                    LogIn.Visibility = Visibility.Visible;
                    wantToRegister = false;
                }
                else
                {
                    tBValidation.Foreground = new SolidColorBrush(Colors.Red);
                    tBValidation.Text = "An account already Exist with this username";
                }

            }
            else
            {
                tBValidation.Foreground = new SolidColorBrush(Colors.Red);
                tBValidation.Text = "Your passwords are different";

            }


        }

        //Clear UI Element
        private void ClearUI()
        {
            password.Text = "";
            userName.Text = "";
            tBConfirmPassword.Text = "";
        }

    }
}
