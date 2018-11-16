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
    public sealed partial class ItemView : Page
    {
        public ClassLibrary1.ItemCollection ItCollec = ClassLibrary1.ItemCollection.GetInstance();
        private ClassLibrary1.RentedItemCollection _rentCollec = ClassLibrary1.RentedItemCollection.GetInstance();
        private AbstractItemCompareByDiscount _discountComp = new AbstractItemCompareByDiscount();
        private InterfaceManager _manager;

        public static double CostForRent = 0.1; //to rent a book, a client have to pay 0.1*Book price
        
        public ItemView()
        {
            _manager = new InterfaceManager(this);
            this.InitializeComponent();

            //initialise books for exemple (once, at the first connection of any user)
            if (InterfaceManager.ConnectNum == 0)
            {
                ItCollec.InitialiseFirstBooks();
            }
            InterfaceManager.ConnectNum++;

            //welcome message
            tBWelcome.Text = string.Format("Welocome {0}, have a great visit...", InterfaceManager.ActualUser.Name);
            _manager.InitialiseListeView(); //Show the item in list manager
            _manager.ShowDependingLevel();  //show StackPanel and button dependind of user level
            NonReturnBooks();               //check if the user didn't returned a book

        }

        //send to the user a paiement message if he didn't returned a book
        private void NonReturnBooks()
        {
            if (InterfaceManager.ActualUser.NonReturnedBook > 0)
            {
                ShowMessage(string.Format("you didn't returned Book. \n We took from you credit card {0:$#0.##}", InterfaceManager.ActualUser.NonReturnedBook.ToString()), "Non Returned Book.. ");
                InterfaceManager.ActualUser.NonReturnedBook = 0;
            }
        }

        // allow to Employee and Manager to go to ManagerUI window
        private void btManager_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ManagerUI));
        }



        #region Message
        //Show the client a message when he pay
        private async void PaiementMessage(double amount)
        {
            string mess = string.Format("Your Paiement of {0:$#0.##} have been processed...", amount);
            await new MessageDialog(mess, "Paiement").ShowAsync();
        }

        //Generic founction to Show messages
        private async void ShowMessage(string message, string title)
        {
            await new MessageDialog(message, title).ShowAsync();
        }

        #endregion

        #region Button
        //Active when Employee or Manager remove a book
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            int index = listView.Items.IndexOf(InterfaceManager.SelectedItem);
            if (index > -1)
            {
                listView.Items.RemoveAt(index);
                ItCollec.Remove(InterfaceManager.SelectedItem);
            }
            InterfaceManager.SelectedItem = default(AbstractItem);
            image.Source = default(ImageSource);
            tBDescription.Text = string.Empty;
        }

        //Create a Book or Journal
        private void Validation_Click(object sender, RoutedEventArgs e)
        {
            string title = string.Empty;
            if (boxTitle.Text != string.Empty)
            {
                title = boxTitle.Text;
            }
            else
            {
                ShowMessage("Invalide Title", "Invalide Data");
                return;
            }

            Guid ISBN = Guid.Empty;
            UInt64 ISBNValue;
            if (boxISBN.Text != string.Empty && boxISBN.Text.Length == 10 && UInt64.TryParse(boxISBN.Text, out ISBNValue))
            {

                ISBN = Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE" + boxISBN.Text);
                if (ItCollec[ISBN] != default(AbstractItem))
                {
                    ShowMessage("A book with this ISBN Already Exist...", "Invalide Data");
                    return;
                }
            }
            else
            {
                ShowMessage("Invalide ISBN", "Invalide Data");
                return;
            }

            double price = default(double);
            if (!double.TryParse(boxPrice.Text, out price))
            {
                ShowMessage("Invalide Price", "Invalide Data");
                return;
            }

            DateTime EditionDate = new DateTime(boxDate.Date.Year, boxDate.Date.Month, boxDate.Date.Day);
            if (EditionDate > DateTime.Now)
            {
                ShowMessage("Invalide Date", "Invalide Data");
                return;
            }

            Categorie subject = Categorie.Other;
            if (!Enum.TryParse(boxCategorie.SelectedItem.ToString(), out subject))
            {
                ShowMessage("Invalide Categorie", "Invalide Data");
                return;
            }

            int stock = default(int);
            if (boxStock.Text == string.Empty) stock = 0;
            else if (!int.TryParse(boxStock.Text, out stock))
            {
                ShowMessage("Invalide stock", "Invalide Data");
                return;
            }

            decimal discount = default(decimal);
            if (boxDiscount.Text == string.Empty) discount = 0;
            else if (!decimal.TryParse(boxDiscount.Text, out discount))
            {
                ShowMessage("Invalide Discount", "Invalide Data");
                return;
            }

            AbstractItem newBook;
            if (selectedType is BookLib)
            {
                newBook = new BookLib(ISBN, price, title, EditionDate, subject, stock, discount, null);
            }
            else
            {
                newBook = new JournalLib(ISBN, price, title, EditionDate, subject, stock, discount, null);
            }
            ItCollec.Add(newBook);
            listView.Items.Add(newBook);
            boxTitle.Text = string.Empty;
            boxISBN.Text = string.Empty;
            boxPrice.Text = string.Empty;
            boxStock.Text = string.Empty;
            boxDiscount.Text = string.Empty;
            sPEmployee.Visibility = Visibility.Collapsed;
        }

        public AbstractItem selectedType; //Initialise the stackPanel For Adding book
        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            sPManageStock.Visibility = Visibility.Collapsed;
            sPEmployee.Visibility = Visibility.Visible;
            addType.Text = "Add a Book..";
            btCreateItem.Content = "Click to add the book...";

            string[] categorieString = Enum.GetNames(typeof(Categorie));
            for (int i = 0; i < categorieString.Length; i++)
            {
                boxCategorie.Items.Add(categorieString[i]);
            }
            selectedType = selectedType as BookLib;
        }

        //Initialise the stackPanel For Adding book
        private void AddJournal_Click(object sender, RoutedEventArgs e)
        {
            sPManageStock.Visibility = Visibility.Collapsed;
            sPEmployee.Visibility = Visibility.Visible;
            addType.Text = "Add a Journal..";
            btCreateItem.Content = "Click to add the journal...";

            string[] categorieString = Enum.GetNames(typeof(Categorie));
            for (int i = 0; i < categorieString.Length; i++)
            {
                boxCategorie.Items.Add(categorieString[i]);
            }
            selectedType = selectedType as JournalLib;
        }

        //Initialise the stackPanel For Adding Stock to item
        private void AddStockBook_Click(object sender, RoutedEventArgs e)
        {
            sPManageStock.Visibility = Visibility.Visible;
            sPEmployee.Visibility = Visibility.Collapsed;
            if (InterfaceManager.SelectedItem != null)
                tBStock.Text = InterfaceManager.SelectedItem.Stock.ToString();
        }
        //Reinitialise the stock button
        private void ReiniStock_Click(object sender, RoutedEventArgs e)
        {
            int stock;
            if (int.TryParse(tBStock.Text, out stock))
            {
                if (stock < 0)
                {
                    ShowMessage("Invalide Stock, it should be superior to 0..", "Invalide Data");
                }

                else if (InterfaceManager.SelectedItem != null) InterfaceManager.SelectedItem.Stock = stock;
            }
            else ShowMessage("Invalide Stock, error in the data input", "Invalide Data");


            sPManageStock.Visibility = Visibility.Collapsed;
            StockAdd.Text = string.Empty;
            tBStock.Text = string.Empty;
            tBDescription.Text = InterfaceManager.SelectedItem.DescriptionString();
        }
        //add to the stock button
        private void AddStock_Click(object sender, RoutedEventArgs e)
        {
            int stock;
            if (int.TryParse(StockAdd.Text, out stock))
            {
                if (stock < 0)
                {
                    ShowMessage("Invalide Stock, it should be superior to 0..", "Invalide Data");
                }
                else if (InterfaceManager.SelectedItem != null) InterfaceManager.SelectedItem.Stock += stock;
            }
            else ShowMessage("Invalide Stock, error in the data input", "Invalide Data");



            sPManageStock.Visibility = Visibility.Collapsed;
            StockAdd.Text = string.Empty;
            tBStock.Text = string.Empty;
            tBDescription.Text = InterfaceManager.SelectedItem.DescriptionString();

        }

        //Select the type of research depending of the critera selectioned by user
        private void button_Click(object sender, RoutedEventArgs e)
        {
            _manager.ActionAfterResearchTypeSelected();
        }

        private void LView_ItemClick(object sender, ItemClickEventArgs e)
        {
            AbstractItem tmp = e.ClickedItem as AbstractItem;
            _manager.LviewAction(tmp);
        }

        //Show item when selectioned in listview
        private void LVCategorie_ItemClick(object sender, ItemClickEventArgs e)
        {
            string tmp = e.ClickedItem as string;
            if (tmp != null)
            {
                listView.Items.Clear();
                for (int i = 0; i < ItCollec.Count; i++)
                {
                    if (Enum.GetName(typeof(Categorie), ItCollec.Get(i).Subject) == tmp)
                        listView.Items.Add(ItCollec.Get(i));
                }
            }
        }

        //Return to login page
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.ActualUser = default(User);
            this.Frame.Navigate(typeof(LogInView));
        }



        public string ActionSelector = null; //Show different type of UI depending the critera selectioned by user
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bTEnter.Visibility = SearchBox1.Visibility = SearchBox2.Visibility = DateSelect1.Visibility = DateSelect2.Visibility = listViewCategorie.Visibility = Visibility.Visible;

            ComboBoxItem temp = e.AddedItems[0] as ComboBoxItem;
            if (temp == null) return;
            switch (temp.Content.ToString())
            {
                case ("All"):
                    bTEnter.Visibility = SearchBox1.Visibility = SearchBox2.Visibility = DateSelect1.Visibility = DateSelect2.Visibility = listViewCategorie.Visibility = Visibility.Collapsed;

                    _manager.InitialiseListeView();
                    break;
                case ("ISBN"):
                    bTEnter.Visibility = SearchBox2.Visibility = DateSelect1.Visibility = DateSelect2.Visibility = listViewCategorie.Visibility = Visibility.Collapsed;

                    textBlock.Text = ActionSelector = "ISBN";
                    SearchBox1.PlaceholderText = "Enter ISBN...";


                    break;
                case ("Name"):
                    bTEnter.Visibility = SearchBox2.Visibility = DateSelect1.Visibility = DateSelect2.Visibility = listViewCategorie.Visibility = Visibility.Collapsed;

                    textBlock.Text = ActionSelector = "Name";
                    SearchBox1.PlaceholderText = "Enter Name...";

                    break;
                case ("Price"):
                    bTEnter.Visibility = DateSelect1.Visibility = DateSelect2.Visibility = listViewCategorie.Visibility = Visibility.Collapsed;

                    textBlock.Text = ActionSelector = "Price";
                    SearchBox1.PlaceholderText = "Enter minimum price...";
                    SearchBox2.PlaceholderText = "Enter maximum price...";
                    break;
                case ("Date"):
                    SearchBox1.Visibility = SearchBox2.Visibility = listViewCategorie.Visibility = Visibility.Collapsed;

                    textBlock.Text = ActionSelector = "Date";
                    break;
                case ("Categorie"):
                    bTEnter.Visibility = SearchBox1.Visibility = SearchBox2.Visibility = DateSelect1.Visibility = DateSelect2.Visibility = bTEnter.Visibility = Visibility.Collapsed;

                    string[] categorieString = Enum.GetNames(typeof(Categorie));
                    for (int i = 0; i < categorieString.Length; i++)
                    {
                        listViewCategorie.Items.Add(categorieString[i]);
                    }

                    break;
                case ("Best Promo"):
                    bTEnter.Visibility = SearchBox1.Visibility = SearchBox2.Visibility = DateSelect1.Visibility = DateSelect2.Visibility = listViewCategorie.Visibility = Visibility.Collapsed;
                    _manager.InitialiseListeView(_discountComp);
                    textBlock.Text = ActionSelector = "Best Promo";
                    break;
                default: return;
            }

        }

        //Change list after a research
        private void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            _manager.ActionAfterResearchTypeSelected();
        }

        //buy button, add item to cart, and the price to the total
        private void btBuyIt_Click(object sender, RoutedEventArgs e)
        {
            if (InterfaceManager.SelectedItem != default(AbstractItem))
            {
                buyedList.Items.Add(InterfaceManager.SelectedItem);
                InterfaceManager.ActualUser.Total += InterfaceManager.SelectedItem.Price * (100 - (double)InterfaceManager.SelectedItem.Discount) / 100;
                tBTotal.Text = string.Format("{0:$#0.##}", InterfaceManager.ActualUser.Total);
            }
        }

        //rent button, add item to cart, and the 10% of the price to the total
        private void bTRentIt_Click(object sender, RoutedEventArgs e)
        {
            if (InterfaceManager.SelectedItem != default(AbstractItem))
            {
                rentedList.Items.Add(InterfaceManager.SelectedItem);
                InterfaceManager.ActualUser.Total += (InterfaceManager.SelectedItem.Price * (100 - (double)InterfaceManager.SelectedItem.Discount) / 100) * CostForRent;
                tBTotal.Text = string.Format("{0:$#0.##}", InterfaceManager.ActualUser.Total);
            }
        }

        //Remove item from cart
        private void removeItem_Click(object sender, RoutedEventArgs e)
        {
            if (rentedList.SelectedIndex > -1)
            {
                AbstractItem temp = rentedList.Items.ElementAt(rentedList.SelectedIndex) as AbstractItem;
                InterfaceManager.ActualUser.Total -= (temp.Price * (100 - (double)temp.Discount) / 100) * CostForRent;

                rentedList.Items.RemoveAt(rentedList.SelectedIndex);
            }

            if (buyedList.SelectedIndex > -1)
            {
                AbstractItem temp = buyedList.Items.ElementAt(buyedList.SelectedIndex) as AbstractItem;
                InterfaceManager.ActualUser.Total -= (temp.Price * (100 - (double)temp.Discount) / 100);

                buyedList.Items.RemoveAt(buyedList.SelectedIndex);
            }
            tBTotal.Text = string.Format("{0:$#0.##}", InterfaceManager.ActualUser.Total);


        }

        //button to finalise the purchase, clean the cart, make the paiement, and add the sum to client Turnover
        private void btCheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (InterfaceManager.ActualUser.Total > 0)
            {
                List<AbstractItem> tmpList = new List<AbstractItem>();
                foreach (AbstractItem item in buyedList.Items)
                    tmpList.Add(item);

                foreach (AbstractItem item in tmpList)
                {
                    buyedList.Items.Remove(item);
                    try
                    {
                        item.PickStock();
                    }
                    catch (RunOutException Ex)
                    {
                        ShowMessage(Ex.Message, "Run Out Exception");
                        InterfaceManager.ActualUser.Total -= (item.Price * (100 - (double)item.Discount) / 100);
                    }
                }

                tmpList.Clear();

                foreach (AbstractItem item in rentedList.Items)
                    tmpList.Add(item);

                foreach (AbstractItem item in tmpList)
                {
                    rentedList.Items.Remove(item);
                    try
                    {
                        item.PickStock();
                        _rentCollec.Add(item, InterfaceManager.ActualUser);
                    }
                    catch (RunOutException Ex)
                    {
                        ShowMessage(Ex.Message, "Run Out Exception");
                        InterfaceManager.ActualUser.Total -= (item.Price * (100 - (double)item.Discount) / 100) * CostForRent;
                    }
                }



                InterfaceManager.ActualUser.Turnover += InterfaceManager.ActualUser.Total;   //return total to 0, and affect the amount to the client turnover                
                tBTotal.Text = string.Empty;
                PaiementMessage(InterfaceManager.ActualUser.Total);       //Message
                InterfaceManager.ActualUser.Total = 0;
            }
            #endregion

        }

    }

}
