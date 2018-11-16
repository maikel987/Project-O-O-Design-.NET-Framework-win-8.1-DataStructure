using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ClassLibrary1
{
    public enum Categorie
    {
        ArtsAndPhotography,
        BiographiesAndMemoirs,
        BusinessAndMoney,
        Calendars,
        ChildrenBooks,
        ReligionBooksAndBibles,
        ComicsAndGraphicNovels,
        ComputersAndTechnology,
        Cookbooks,
        FoodAndWine,
        Crafts,
        HobbiesAndHome,
        EducationAndTeaching,
        EngineeringAndTransportation,
        GayAndLesbian,
        Health,
        FitnessAndDieting,
        History,
        HumorAndEntertainment,
        Law,
        LiteratureAndFiction,
        MedicalBooks,
        Mystery,
        ThrillerAndSuspense,
        ParentingAndRelationships,
        PoliticsAndSocialSciences,
        Reference,
        ReligionAndSpirituality,
        Romance,
        ScienceAndMath,
        ScienceFictionAndFantasy,
        SelfHelp,
        SportsAndOutdoors,
        TeenAndYoungAdult,
        TestPreparation,
        Travel,
        DomesticLife,
        Other
    }

    //Permit to Sort Item by Discount to Show Client best promotion
    public class AbstractItemCompareByDiscount : IComparer<AbstractItem>
    {
        public int Compare(AbstractItem x, AbstractItem y)
        {
            return (int)y.Discount - (int)x.Discount;
        }

    }

    //abstract class to be a base to Book Journal and other type of item.
    public abstract class AbstractItem : IComparable<AbstractItem> // Public   _private  
    {
        public Guid ISBN { get; private set; }
        public double Price { get; private set; }
        public string Title { get; private set; }
        public DateTime EditionDate { get; private set; }
        private int _stock;
 
        public Categorie Subject { get; private set; }
        public decimal Discount { get; private set; }
        public BitmapImage Image { get; private set; }

        public int Stock
        {
            get { return _stock; }
            set { if (value >= 0) _stock = value; }
        }



public AbstractItem(Guid ISBN, double price, string title, DateTime EditionDate, Categorie subject, int stock = 0, decimal discount = 0, BitmapImage image = default(BitmapImage))
        {
            this.Image = image;
            this.ISBN = ISBN;
            this.Price = price;
            this.Title = title;
            this.EditionDate = EditionDate;
            this._stock = stock;
            this.Subject = subject;
            this.Discount = discount;
        }
        // ToString shows little information to not overloaded UI Listview
        public override string ToString()
        {
            return string.Format("{0}, Price: {1:$#0.##}, \n Subject: {2}", Title, Price, Subject.ToString());
        }

        // This function is here to show more information about each Item when a client want more.
        public virtual string DescriptionString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine(string.Format("{0}, ", Title));
            _sb.AppendLine(string.Format("Price: \t{0:#0.##$} ", Price));
            if (Discount > 0m)
                _sb.AppendLine(string.Format("Discount: \t{0:#0.##%} ", Discount / 100));

            _sb.AppendLine(string.Format("Subject: \t{0} ", Subject));
            _sb.AppendLine(string.Format("EditionDate: \t{0:d} ", EditionDate));

            StringBuilder isbn = new StringBuilder(ISBN.ToString());
            isbn.Remove(0, isbn.Length - 10);

            _sb.AppendLine(string.Format("ISBN: \t{0} ", isbn.ToString()));
            _sb.AppendLine(string.Format("Stock: \t{0} ", _stock));

            return _sb.ToString();
        }
        //is used to reduce stock when a client want to rent or buy an item, and check if there are enought stock
        public void PickStock()
        {
            if (_stock > 0)
                _stock--;
            else throw new RunOutException(string.Format("Sorry we runout of {0}...", Title));
        }

        public int CompareTo(AbstractItem other)
        {
            return Title.CompareTo(other.Title);
        }
    }
}
