using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ClassLibrary1
{
    public class BookLib : AbstractItem
    {
        public BookLib(Guid ISBN, double price, string title, DateTime EditionDate, Categorie subject, int stock = 0, decimal discount = 0, BitmapImage image = null) 
            : base(ISBN, price, title, EditionDate, subject, stock, discount, image)
        {
        }

        // we override this function to Show that this item is a book
        public override string DescriptionString()
        {   
            return string.Format("Book - \n{0}",base.DescriptionString());
        }

    }
}
