using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ClassLibrary1
{
    public class ItemCollection : IItemCollection
    {
        private List<AbstractItem> _absItemList;
        private static ItemCollection _instance; //singleton design pattern

        public int Count { get { return _absItemList.Count; } }

        //singleton design pattern
        static public ItemCollection GetInstance() 
        {
            if (_instance == null)
            {
                _instance = new ItemCollection();
            }
            return _instance;
        }

        private ItemCollection()
        {
            _absItemList = new List<AbstractItem>();
            //InitialiseFirstBooks();
        }

        //allow to add an Item to the collection and check if an other Item have the same ISBN
        public void Add(AbstractItem absItem)
        {
            if (this[absItem.ISBN] == default(AbstractItem))
            {
                _absItemList.Add(absItem);
            }
            else
            {
                throw new DoubleISBNException("An Item already exist with this ISBN");
            }
        }

        //allow you to Get an Item in the collection
        public AbstractItem Get(int index)
        {
            if (index >= _absItemList.Count) return default(AbstractItem);
            return _absItemList[index];
        }

        //allow you to Get all Item from a category in the collection
        public List<AbstractItem> Get(Categorie categorie)
        {
            List<AbstractItem> temp = new List<AbstractItem>();
            foreach (AbstractItem item in _absItemList)
            {
                if (item.Subject == categorie) temp.Add(item);
            }
            if (temp.Count == 0) return default(List<AbstractItem>);
            else return temp;

        }

        //allow you to Get all Item with a determined discount in the collection
        public List<AbstractItem> Get(decimal discount)
        {
            List<AbstractItem> temp = new List<AbstractItem>();
            foreach (AbstractItem item in _absItemList)
            {
                if (item.Discount == discount) temp.Add(item);
            }
            if (temp.Count == 0) return default(List<AbstractItem>);
            else return temp;

        }

        //allows you to remove elements thanks to the ISBN (unic) to the collection and know if it was successfull
        public bool Remove(AbstractItem absItem)
        {
            int index;
            try
            {
                index = _absItemList.FindIndex((c) => c.ISBN == absItem.ISBN);
            }
            catch (Exception)
            {
                return false;
            }
            if (index != default(int)) return Remove(index);
            else return false;
        }

        //allows you to remove elements by index to the collection and know if it was successfull
        public bool Remove(int index)
        {
            if (index >= _absItemList.Count) return false;
            _absItemList.RemoveAt(index);
            return true;
        }

        //Indexerthat allow to get an item in the collection with the ISBN
        public AbstractItem this[Guid ISBN]
        {
            get
            {
                foreach (AbstractItem item in _absItemList)
                {
                    if (item.ISBN == ISBN) return item;
                }
                return default(AbstractItem);
            }
        }

        //Indexerthat allow to get an item in the collection with the ISBN
        public AbstractItem this[string title]
        {
            get
            {
                foreach (AbstractItem item in _absItemList)
                {
                    if (item.Title == title) return item;
                }
                return default(AbstractItem);
            }
        }

        //allow to get all item that have in his title a certain string
        public List<AbstractItem> PartialTitleResearch(string searchedWord)
        {
            List<AbstractItem> temp = new List<AbstractItem>();
            foreach (AbstractItem item in _absItemList)
            {
                if (item.Title.ToLower().Contains(searchedWord.ToLower())) temp.Add(item);
            }
            if (temp.Count == 0) return default(List<AbstractItem>);
            return temp;
        }

        //allow to get all item that have a price between two prices 
        public List<AbstractItem> PriceResearch(double min, double max)
        {
            if (min >= max) return default(List<AbstractItem>);

            List<AbstractItem> temp = new List<AbstractItem>();
            foreach (AbstractItem item in _absItemList)
            {
                if (item.Price > min && item.Price < max) temp.Add(item);
            }
            if (temp.Count == 0) return default(List<AbstractItem>);
            return temp;
        }

        //allow to get all item that have a Date between two Dates 
        public List<AbstractItem> DateResearch(DateTime min, DateTime max)
        {
            if (min >= max) return default(List<AbstractItem>);

            List<AbstractItem> temp = new List<AbstractItem>();
            foreach (AbstractItem item in _absItemList)
            {
                if (item.EditionDate > min && item.EditionDate < max) temp.Add(item);
            }
            if (temp.Count == 0) return default(List<AbstractItem>);
            return temp;
        }

        //initialise thirst books to have an Exemple
        public void InitialiseFirstBooks()
        {


            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE0312577230"), 17.39, "The Great Alone: A Novel", new DateTime(2018, 2, 6), Categorie.DomesticLife, 100, 10m,
                new BitmapImage(new Uri("ms-appx:///Assets/The Great Alone A Novel.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE0439708184"), 24.91, "Harry Potter and the Sorcerer's Stone", new DateTime(2015, 12, 8), Categorie.ScienceFictionAndFantasy, 100, 11m,
                new BitmapImage(new Uri("ms-appx:///Assets/Harry.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE0307887448"), 12.19, "Ready Player One", new DateTime(2011, 8, 16), Categorie.ScienceFictionAndFantasy, 100, 12m,
                new BitmapImage(new Uri("ms-appx:///Assets/ReadyPlayer.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE3841730361"), 25.75, "Une Analyse Comparative de la Performance des Fonds", new DateTime(2014, 1, 14), Categorie.BusinessAndMoney, 2, 13m,
                new BitmapImage(new Uri("ms-appx:///Assets/MS.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE1616895888"), 27.19, "Botanical Sketchbooks", new DateTime(2017, 5, 17), Categorie.ArtsAndPhotography, 100, 14m,
                            new BitmapImage(new Uri("ms-appx:///Assets/botanical.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE0316270755"), 17.72, "You Don't Have to Say You Love Me", new DateTime(2017, 6, 13), Categorie.BiographiesAndMemoirs, 100, 15m,
                            new BitmapImage(new Uri("ms-appx:///Assets/you.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE1501124021"), 18.00, "Principles: Life and Work", new DateTime(2017, 9, 19), Categorie.BusinessAndMoney, 100, 10m,
                            new BitmapImage(new Uri("ms-appx:///Assets/principales.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE0316508888"), 12.35, "Nevermoor: The Trials of Morrigan Crow", new DateTime(2017, 10, 17), Categorie.ChildrenBooks, 100, 0m,
                            new BitmapImage(new Uri("ms-appx:///Assets/never.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE1401952852"), 26.99, "High Performance Habits", new DateTime(2017, 9, 19), Categorie.BusinessAndMoney, 100, 0m,
                            new BitmapImage(new Uri("ms-appx:///Assets/high.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE1524732680"), 29.95, "Option B", new DateTime(2017, 4, 24), Categorie.BusinessAndMoney, 100, 0m,
                            new BitmapImage(new Uri("ms-appx:///Assets/optionB.jpg"))));


            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE1101885939"), 27.00, "The Bear and the Nightingale", new DateTime(2017, 1, 10), Categorie.ScienceFictionAndFantasy, 100, 0m,
                            new BitmapImage(new Uri("ms-appx:///Assets/Bear.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE0393609097"), 25.95, "Norse Mythology", new DateTime(2017, 2, 7), Categorie.ScienceFictionAndFantasy, 100, 0m,
                            new BitmapImage(new Uri("ms-appx:///Assets/Norse.jpg"))));

            _absItemList.Add(new BookLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE0374285063"), 27.00, "Void Star: A Novel", new DateTime(2017, 4, 11), Categorie.ScienceFictionAndFantasy, 100, 0m,
                            new BitmapImage(new Uri("ms-appx:///Assets/Void Star A Novel.jpg"))));

            _absItemList.Add(new JournalLib(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EE1234567890"), 1.00, "The Journal", DateTime.Now, Categorie.EducationAndTeaching, 2, 0m));

        }

        //allow to get the total Stock of all Item
        public int TotalStock
        {
            get
            {
                int _total = 0;
                foreach (AbstractItem item in _absItemList)
                {
                    _total += item.Stock;
                }
                return _total;
            }
        }

        //allow to get the total Stock of all Book
        public int BookStock
        {
            get
            {
                int _total = 0;
                foreach (AbstractItem item in _absItemList)
                {
                    if (item is BookLib) _total += item.Stock;
                }
                return _total;
            }
        }

        //allow to get the total Stock of all Journals
        public int JournalStock
        {
            get
            {
                int _total = 0;
                foreach (AbstractItem item in _absItemList)
                {
                    if (item is JournalLib) _total += item.Stock;
                }
                return _total;
            }
        }

        //allow to get the number of different Book
        public int BookCount
        {
            get
            {
                int _total = 0;
                foreach (AbstractItem item in _absItemList)
                {
                    if (item is BookLib) _total++;
                }
                return _total;
            }
        }

        //allow to get the number of different Journal
        public int JournalCount
        {
            get
            {
                int _total = 0;
                foreach (AbstractItem item in _absItemList)
                {
                    if (item is JournalLib) _total++;
                }
                return _total;
            }
        }






    }

}
 