using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ProjetLibraryManagement
{
    class InterfaceManager
    {
        public static User ActualUser;
        static public AbstractItem SelectedItem;
        //static public RentedItem SelectedRentedItem;
        private ItemView _mainpage;
        public ClassLibrary1.ItemCollection itCollec = ClassLibrary1.ItemCollection.GetInstance();
        public static int ConnectNum = 0;

        //ctor
        public InterfaceManager(ItemView mainpage)
        {

            _mainpage = mainpage;
        }

        //add an "s" when the data is su^perior to 1
        static public string Plurial(int data)
        {
            if (data > 1) return "s";
            else return string.Empty;
        }

        //test password
        static public bool ValidPassword(string password)
        {
            return (password.Length > 8 && password.ToLower() != password && password.ToUpper() != password);
        }

        //show a message to selectionned txtblock
        static public void ShowValidationOnTextBox(string message, Color color, TextBlock tBDestination)
        {
            tBDestination.Text = message;
            tBDestination.Foreground = new SolidColorBrush(color);
        }

        //Show Item on selectione listview
        static public void ShowElementInListView(List<AbstractItem> absList, ListView listV, TextBlock tBDestination)
        {
            if (absList == null) { ShowValidationOnTextBox("no book corresponding to your research", Colors.Red, tBDestination); return; }
            listV.Items.Clear();
            foreach (AbstractItem item in absList)
            {
                listV.Items.Add(item);
            }

        }

        //Show Rented on selectione listview
        static public void ShowElementInListView(List<RentedItem> absList, ListView listV, TextBlock tBDestination)
        {
            if (absList == null) { ShowValidationOnTextBox("no book corresponding to your research", Colors.Red, tBDestination); return; }
            listV.Items.Clear();
            foreach (RentedItem item in absList)
            {
                listV.Items.Add(item);
            }

        }

        //Show element depending on user Autorisation level
        public void ShowDependingLevel()
        {
            _mainpage.sPManageStock.Visibility = _mainpage.sPEmployee.Visibility = Visibility.Collapsed;
            _mainpage.sPClient.Visibility = _mainpage.sPanelClientBt.Visibility = _mainpage.sPanelMangerBt.Visibility = _mainpage.btManager.Visibility = Visibility.Visible;

            switch (ActualUser.Level)
            {
                case (AutorisationLevel.Client):
                    _mainpage.btManager.Visibility = _mainpage.sPanelMangerBt.Visibility = Visibility.Collapsed;

                    break;
                case (AutorisationLevel.Employee):
                    _mainpage.sPClient.Visibility = _mainpage.sPanelClientBt.Visibility = Visibility.Collapsed;

                    break;
                case (AutorisationLevel.Manager):
                    _mainpage.sPClient.Visibility = _mainpage.sPanelClientBt.Visibility = Visibility.Collapsed;
                    break;

            }
        }

        //Show the itemthat have a specific ISBN in listview
        public void RequestISBN()
        {
            UInt64 value;
            if (_mainpage.SearchBox1.QueryText.Length == 10 && UInt64.TryParse(_mainpage.SearchBox1.QueryText, out value))
            {
                Guid tmp = Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE" + _mainpage.SearchBox1.QueryText);
                _mainpage.listView.Items.Clear();
                if (_mainpage.ItCollec[tmp] == null)
                {
                    ShowValidationOnTextBox("No Book Founded with this ISBN...", Colors.Red, _mainpage.textBlock);
                }
                else
                {
                    ShowValidationOnTextBox("A book Founded with this ISBN...", Colors.Green, _mainpage.textBlock);
                    _mainpage.listView.Items.Add(_mainpage.ItCollec[tmp]);
                }
            }
            else
            {
                ShowValidationOnTextBox("The ISBN you enter isn't valid...", Colors.Red, _mainpage.textBlock);
            }
        }

        public void RequestName()
        {
            if (_mainpage.ItCollec.PartialTitleResearch(_mainpage.SearchBox1.QueryText) != null)
            {
                ShowElementInListView(_mainpage.ItCollec.PartialTitleResearch(_mainpage.SearchBox1.QueryText), _mainpage.listView, _mainpage.textBlock);
                ShowValidationOnTextBox("this is the books corresponding to your research", Colors.Green, _mainpage.textBlock);
            }
            else ShowValidationOnTextBox("no book corresponding to your research", Colors.Red, _mainpage.textBlock);

        }

        //Show the items that have a price between two price in listview
        public void RequestPrice()
        {
            double maxPrice;
            double minPrice;
            if (_mainpage.SearchBox1.QueryText.Length == 0) minPrice = 0;
            else if (!double.TryParse(_mainpage.SearchBox1.QueryText, out minPrice))
            {
                ShowValidationOnTextBox("Invalide minimum Price entered...", Colors.Red, _mainpage.textBlock);
                return;
            }

            if (_mainpage.SearchBox2.QueryText.Length == 0) maxPrice = double.MaxValue;
            else if (!double.TryParse(_mainpage.SearchBox2.QueryText, out maxPrice))
            {
                ShowValidationOnTextBox("Invalide maximum Price entered...", Colors.Red, _mainpage.textBlock);
                return;
            }


            if (minPrice < maxPrice)
            {
                ShowElementInListView(_mainpage.ItCollec.PriceResearch(minPrice, maxPrice), _mainpage.listView, _mainpage.textBlock);
                ShowValidationOnTextBox("this is the books corresponding to your research", Colors.Green, _mainpage.textBlock);
            }
            else
                ShowValidationOnTextBox("maximum price inferior to minimum price ...", Colors.Red, _mainpage.textBlock);

        }

        //Show the items that have a date between two dates in listview
        public void RequestDate()
        {
            DateTime dTimeMin = new DateTime(_mainpage.DateSelect1.Date.Year, _mainpage.DateSelect1.Date.Month, _mainpage.DateSelect1.Date.Day);
            DateTime dTimeMax = new DateTime(_mainpage.DateSelect2.Date.Year, _mainpage.DateSelect1.Date.Month, _mainpage.DateSelect1.Date.Day);
            if (dTimeMin <= dTimeMax)
            {
                ShowValidationOnTextBox("this is the books corresponding to your research", Colors.Green, _mainpage.textBlock);
                ShowElementInListView(_mainpage.ItCollec.DateResearch(dTimeMin, dTimeMax), _mainpage.listView, _mainpage.textBlock);
            }
            else
                ShowValidationOnTextBox("maximum date inferior to minimum date ...", Colors.Red, _mainpage.textBlock);

        }

        //select wich action will done depending on user choice
        public void ActionAfterResearchTypeSelected()
        {
            switch (_mainpage.ActionSelector)
            {
                case ("ISBN"):
                    RequestISBN();
                    break;

                case ("Name"):
                    RequestName();
                    break;

                case ("Price"):
                    RequestPrice();
                    break;

                case ("Date"):
                    RequestDate();
                    break;


                default: return;
            }
        }

        //Sort item and show it in listview
        public void InitialiseListeView(IComparer<AbstractItem> alternativeComparer = null)
        {
            _mainpage.listView.Items.Clear();
            if (alternativeComparer != null)
            {
                List<AbstractItem> absList = new List<AbstractItem>();
                for (int i = 0; i < itCollec.Count; i++)
                {
                    absList.Add(itCollec.Get(i));
                }

                absList.Sort(alternativeComparer);

                for (int i = 0; i < absList.Count; i++)
                {
                    _mainpage.listView.Items.Add(absList[i]);
                }
            }
            else
            {
                for (int i = 0; i < itCollec.Count; i++)
                {
                    _mainpage.listView.Items.Add(itCollec.Get(i));
                }
            }

        }

        //Show Item selected in the textbox and in image
        public void LviewAction(AbstractItem absItem)
        {
            if (absItem != null)
            {
                _mainpage.image.Source = absItem.Image;
                _mainpage.tBDescription.Text = absItem.DescriptionString();
                SelectedItem = absItem;
                if (absItem as JournalLib != null) _mainpage.bTRentIt.Visibility = Visibility.Collapsed;
                else _mainpage.bTRentIt.Visibility = Visibility.Visible;
            }

        }

        
    }
}
