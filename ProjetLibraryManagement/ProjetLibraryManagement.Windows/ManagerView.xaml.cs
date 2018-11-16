using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ProjetLibraryManagement
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManagerUI : Page
    {
        private User _thisUser = InterfaceManager.ActualUser;
        private RentedItemCollection _rentCollec = RentedItemCollection.GetInstance();
        private ClassLibrary1.ItemCollection _itemCollec = ClassLibrary1.ItemCollection.GetInstance();
        private UserCollection _uCollect = UserCollection.GetInstance();

        public ManagerUI()
        {
            this.InitializeComponent();
            Initialisation();

            if (InterfaceManager.ActualUser.Level == AutorisationLevel.Manager)
            {
                buttonReport.Visibility = Visibility.Visible;
                buttonAddUser.Visibility = Visibility.Visible;
            }
            else
            {
                buttonReport.Visibility = Visibility.Collapsed;
                buttonAddUser.Visibility = Visibility.Collapsed;
            }

        }

        //allow user to return to last page
        private void btPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ItemView));
        }
        
        //show all rented item in listview
        private void Initialisation()
        {
            for (int i = 0; i < _rentCollec.Count; i++)
            {
                listView.Items.Add(_rentCollec[i]);
            }

        }

        //show late renter
        private void LateBt_Click(object sender, RoutedEventArgs e)
        {
            int num;
            if (int.TryParse(tBDay.Text, out num))
            {


                TimeSpan ts = new TimeSpan(num, 0, 0, 0);
                DateTime tempDate = DateTime.Now - ts;

                List<RentedItem> temp = new List<RentedItem>();
                foreach (RentedItem item in listView.Items)
                {
                    if (item._rentedDay < tempDate) temp.Add(item);
                }

                listView.Items.Clear();
                if (temp != null)
                    for (int i = 0; i < temp.Count; i++)
                    {
                        listView.Items.Add(temp[i]);
                    }
            }
        }

        //construct the selectioned item and show they information
        List<RentedItem> _rentedListSelection = new List<RentedItem>();
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            foreach (RentedItem item in e.AddedItems)
            {
                _rentedListSelection.Add(item);
            }
            foreach (RentedItem item in e.RemovedItems)
            {
                _rentedListSelection.Remove(item);
            }

            itemInfo.Text = "";
            for (int i = 0; i < _rentedListSelection.Count; i++)
            {
                itemInfo.Text += string.Format("- {0}\n", _rentedListSelection[i].ToString());

            }

        }

        //Delete rented item => they have been returned
        private void btReturned_Click(object sender, RoutedEventArgs e)
        {
            List<RentedItem> tmp = new List<RentedItem>();
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                if (listView.SelectedItems[i] is RentedItem)
                    tmp.Add(listView.SelectedItems[i] as RentedItem);
            }

            for (int i = 0; i < tmp.Count; i++)
            {
                tmp[i]._item.Stock++;
                listView.Items.Remove(tmp[i]);
                if (!_rentCollec.Remove(tmp[i])) ShowMessage("Remove preoblem", "Remove problem");
            }
        }

        private async void ShowMessage(string message, string title)
        {
            await new MessageDialog(message, title).ShowAsync();
        }

        //Change the date of selected item
        private void ChangeDate_Click(object sender, RoutedEventArgs e)
        {
            DateTime tmpDate = new DateTime(datePick.Date.Year, datePick.Date.Month, datePick.Date.Day);
            if (tmpDate > DateTime.Now) return;

            List<RentedItem> tmp = new List<RentedItem>();
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                if (listView.SelectedItems[i] is RentedItem)
                    tmp.Add(listView.SelectedItems[i] as RentedItem);
            }

            for (int i = 0; i < tmp.Count; i++)
            {
                int tempIndex = listView.Items.IndexOf(tmp[i]);
                tmp[i]._rentedDay = tmpDate;
                listView.Items.RemoveAt(tempIndex);
                listView.Items.Insert(tempIndex, tmp[i]);

            }

        }

        //Delete rented item => they have been returned & make the client pay for it
        private void btLost_Click(object sender, RoutedEventArgs e)
        {
            List<RentedItem> tmp = new List<RentedItem>();
            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                if (listView.SelectedItems[i] is RentedItem)
                    tmp.Add(listView.SelectedItems[i] as RentedItem);
            }

            for (int i = 0; i < tmp.Count; i++)
            {
                tmp[i]._user.NonReturnedBook += tmp[i]._item.Price;
                listView.Items.Remove(tmp[i]);
                if (!_rentCollec.Remove(tmp[i])) ShowMessage("Remove preoblem", "Remove problem");
            }
        }

        //search rented item for a determined client
        private void search_User_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            listView.Items.Clear();
            for (int i = 0; i < _rentCollec.Count; i++)
            {
                listView.Items.Add(_rentCollec[i]);
            }

            if (search_User.QueryText != string.Empty)
            {
                List<RentedItem> tempList = new List<RentedItem>();
                foreach (RentedItem item in listView.Items)
                {
                    if (item._user.Name == search_User.QueryText)
                        tempList.Add(item);
                }
                listView.Items.Clear();
                for (int i = 0; i < tempList.Count; i++)
                {
                    listView.Items.Add(tempList[i]);
                }

            }
        }

        //show the stackpanel for adding user
        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            sPAddUser.Visibility = Visibility.Visible;
            sPReport.Visibility = Visibility.Collapsed;
            PasswordValidation.Text = string.Empty;
        }

        //show a report of all data system
        private void showReport_Click(object sender, RoutedEventArgs e)
        {
            sPReport.Visibility = Visibility.Visible;
            sPAddUser.Visibility = Visibility.Collapsed;

            tbBookNum.Text = string.Format("{0} book{1} ", _itemCollec.BookCount,InterfaceManager.Plurial(_itemCollec.BookCount));
            tbBookStock.Text = string.Format("{0} book{1} ", _itemCollec.BookStock, InterfaceManager.Plurial(_itemCollec.BookStock));
            tbJournalNum.Text = string.Format("{0} journal{1} ", _itemCollec.JournalCount, InterfaceManager.Plurial(_itemCollec.JournalCount));
            tbJournalStock.Text = string.Format("{0} journal{1} ", _itemCollec.JournalStock, InterfaceManager.Plurial(_itemCollec.JournalStock));

            tbCliNumber.Text = string.Format("{0} client{1} ", _uCollect.UserPerLevel(AutorisationLevel.Client), InterfaceManager.Plurial(_uCollect.UserPerLevel(AutorisationLevel.Client)));
            tbEmpNumber.Text = string.Format("{0} employee{1} ", _uCollect.UserPerLevel(AutorisationLevel.Employee), InterfaceManager.Plurial(_uCollect.UserPerLevel(AutorisationLevel.Employee)));
            tbManNumber.Text = string.Format("{0} manager{1} ", _uCollect.UserPerLevel(AutorisationLevel.Manager), InterfaceManager.Plurial(_uCollect.UserPerLevel(AutorisationLevel.Manager)));

            tbRentedBook.Text = string.Format("{0} rented book{1} ", _rentCollec.Count, InterfaceManager.Plurial(_rentCollec.Count));
            tbTurnover.Text = string.Format("{0:$##########0.##}", _uCollect.TurnOver);

            tbCliAverage.Text = string.Format("{0:$##########0.##}/ per client", _uCollect.TurnOver / _uCollect.UserPerLevel(AutorisationLevel.Client));
        }

        //the user can only select one radioButton
        private void rBManager_Checked(object sender, RoutedEventArgs e)
        {
            rBEmployee.IsChecked = false;
        }

        private void rBEmployee_Checked(object sender, RoutedEventArgs e)
        {
            rBManager.IsChecked = false;
        }

        //Create a user
        private void createUser_Click(object sender, RoutedEventArgs e)
        {
            if (!InterfaceManager.ValidPassword(tBPasswordUser.Text))
            {
                PasswordValidation.Foreground = new SolidColorBrush(Colors.Red);
                PasswordValidation.Text = "PassWord have to contain 8 characteres, upper and lower case";
                return;
            }
            string password = tBPasswordUser.Text;

            if (_uCollect.IsUser(tBNameUser.Text))
            {
                PasswordValidation.Foreground = new SolidColorBrush(Colors.Red);
                PasswordValidation.Text = "Username already exist. Chose another one...";
                return;
            }
            string name = tBNameUser.Text;
            AutorisationLevel autoLevel;
            if (rBEmployee.IsChecked == true)
            {
                autoLevel = AutorisationLevel.Employee;
            }
            else if (rBManager.IsChecked == true)
            {
                autoLevel = AutorisationLevel.Manager;
            }
            else
            {
                PasswordValidation.Foreground = new SolidColorBrush(Colors.Red);
                PasswordValidation.Text = "Choose an autorisation Level";
                return;
            }

            User tmp = new User(autoLevel, name, password);
            _uCollect.AddUser(tmp);
            PasswordValidation.Foreground = new SolidColorBrush(Colors.Green);
            PasswordValidation.Text = "User Created";
            ClearUIData();

        }

        //show stackpanel to add user
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sPAddUser.Visibility = Visibility.Collapsed;
            ClearUIData();
        }

        private void ClearUIData()
        {
            tBNameUser.Text = string.Empty;
            tBPasswordUser.Text = string.Empty;
            rBEmployee.IsChecked = false;
            rBManager.IsChecked = false;
        }

        private void CloseReport_Click(object sender, RoutedEventArgs e)
        {
            sPReport.Visibility = Visibility.Collapsed;
        }
    }
}
