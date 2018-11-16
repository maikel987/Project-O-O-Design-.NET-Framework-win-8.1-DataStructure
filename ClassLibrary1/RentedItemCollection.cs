using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class RentedItemCollection
    {
        private List<RentedItem> _rentItemList;
        private static RentedItemCollection _instance;         //singleton design pattern

        public int Count { get { return _rentItemList.Count; } }

        static public RentedItemCollection GetInstance()         //singleton design pattern
        {
            if (_instance == null)
            {
                _instance = new RentedItemCollection();
            }
            return _instance;
        }

        //ctor
        private RentedItemCollection() 
        {
            _rentItemList = new List<RentedItem>();
        }

        //Allow to add directly an item and a user as a Rented Item.
        public void Add(AbstractItem absItem, User user)
        {
            RentedItem _tmp = new RentedItem(absItem,user);
            _rentItemList.Add(_tmp);
        }

        //allows you to remove a rented item from the collection
        public bool Remove(RentedItem item)
        {
            for (int i = 0; i < _rentItemList.Count; i++)
            {
                if (_rentItemList[i] == item)
                {
                    _rentItemList.Remove(item);
                    return true;
                }
            }
            return false;
        }

        // allow to find late renters. item that have been rented from more than x days
        public List<RentedItem> DateResearch(int dayNumber) 
        {
            //DateTime tempDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - dayNumber);
            TimeSpan _ts = new TimeSpan(dayNumber, 0, 0, 0);
            DateTime _tempDate = DateTime.Now - _ts;
            return DateResearch(_tempDate);
        }

        // allow to find late renters. item that have been rented before a date
        public List<RentedItem> DateResearch(DateTime date) // to find late renter
        {
            //if (date >= DateTime.Now) return default(List<RentedItem>);

            List<RentedItem> temp = new List<RentedItem>();
            foreach (RentedItem item in _rentItemList)
            {
                if (item._rentedDay < date) temp.Add(item);
            }
            if (temp.Count == 0) return default(List<RentedItem>);
            return temp;
        }

        //find an item by index
        public RentedItem this[int index]
        {
            get
            {
                if (index >= _rentItemList.Count) return default(RentedItem);
                return _rentItemList[index];
            }   
        }
    }

    //allow to associate an item, an user and a Date.
    public class RentedItem
    {
        public AbstractItem _item { get; set; }
        public DateTime _rentedDay { get; set; }
        public User _user;

        //mostly used in the code, allow to fix the rented Date directly as today
        public RentedItem(AbstractItem item,User user)
        {
            _user = user;
            _item = item;
            _rentedDay = DateTime.Now;
        }

        //mostly used in testing
        internal RentedItem(AbstractItem item, User user,DateTime date)
        {
            _user = user;
            _item = item;
            _rentedDay = date;
        }
        public override string ToString()
        {
            return string.Format("{0} \n rented the {1:d} \n by {2}", _item.Title, _rentedDay,_user.Name);//base.ToString();
        }
    }
}
