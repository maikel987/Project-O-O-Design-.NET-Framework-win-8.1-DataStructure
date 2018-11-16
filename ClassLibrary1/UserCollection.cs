using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class UserCollection
    {
        private List<User> _customerList;
        public int Count { get { return _customerList.Count; } }
        private static UserCollection _instance;   //singleton design pattern

        static public UserCollection GetInstance()    //singleton design pattern
        {
            if (_instance == null)
            {
                _instance = new UserCollection();
            }
            return _instance;
        }

        private UserCollection() //ctor
        {
            _customerList = new List<User>();
        }

        //initialise thirst books to have an Exemple
        public void SetUser()
        {
            _customerList.Add(new User(AutorisationLevel.Client, "David", "123456"));
            _customerList.Add(new User(AutorisationLevel.Client, "2", "2"));
            _customerList.Add(new User(AutorisationLevel.Client, "Benoit", "qwerty"));


            _customerList.Add(new User(AutorisationLevel.Manager, "Sela", "944"));
            _customerList.Add(new User(AutorisationLevel.Manager, "1", "1"));

            _customerList.Add(new User(AutorisationLevel.Employee, "George", "azerty"));
            _customerList.Add(new User(AutorisationLevel.Employee, "3", "3"));
            _customerList.Add(new User(AutorisationLevel.Employee, "Babar", "azerty2"));
        }

        //allow you to get a user thanks to user name and password usefull at the connection to know who connect
        public User GetUser(string name, string password)
        {
            if (name == null || password == null) return default(User);
            for (int i = 0; i < _customerList.Count; i++)
            {
                if (_customerList[i].Name == name && _customerList[i]._password == password) return _customerList[i];
            }
            return default(User);

        }

        //to know if a user already exist
        public bool IsUser(string name, string password)
        {
            if (name == null || password == null) return false;
            for (int i = 0; i < _customerList.Count; i++)
            {
                if (_customerList[i].Name == name && _customerList[i]._password == password) return true;
            }
            return false;

        }

        //to know if a username have been taken before
        public bool IsUser(string name)
        {
            if (name == null) return false;
            for (int i = 0; i < _customerList.Count; i++)
            {
                if (_customerList[i].Name == name) return true;
            }
            return false;

        }
        
        //allow you to add a user to the collection
        public void AddUser(User user)
        {
            if (!IsUser(user.Name))_customerList.Add(user);
            else throw new DoubleUserException();
        }

        //allow you to know how much user are in the collection for a determined Level
        public int UserPerLevel(AutorisationLevel autoLevel)
        {
            int _userNum = 0;
            foreach (User item in _customerList)
            {
                if (item.Level == autoLevel) _userNum++;
            }
            return _userNum;
        }

        //allow us to get the total turnover
        public double TurnOver
        {
            get
            {
                double _turnOver = 0;
                foreach (User item in _customerList)
                {
                    _turnOver += item.Turnover;
                }
                return _turnOver;
            }
        }

    }
    public class User
    {
        public AutorisationLevel Level { get; private set; }
        public string Name { get; private set; }
        internal string _password { get; private set; }
        public double Total { get; set; }                   //gives the total amount of the current shopping cart, before purchase
        public double Turnover { get; set; }                //gives the total amount of the user's purchased item
        public double NonReturnedBook { get; set; }         //gives the total due amount of the user for non returned item

        //ctor
        public User(AutorisationLevel level, string name, string password, double total = 0, double turnover = 0)
        {
            NonReturnedBook = 0;
            Turnover = turnover;
            Total = total;
            Level = level;
            Name = name;
            _password = password;
        }

    }

    public enum AutorisationLevel
    {
        Client, //can see item and there description, purchase and rent it
        Employee, //can see item and there description, can add and remove item, add/remove stock, and consult the rented item, and mark them as returned or lost
        Manager //same than Employee, plus, access to the report, and add Employee and manager.

    }
}
