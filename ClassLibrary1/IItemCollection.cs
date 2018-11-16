using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    interface IItemCollection
    {
        //allows you to add elements to the collection
        void Add(AbstractItem absItem);

        //allow you to Get an Item in the collection
        AbstractItem Get(int index);

        //allow you to Get all Item from a category in the collection
        List<AbstractItem> Get(Categorie categorie);

        //allow you to Get all Item with a determined discount in the collection
        List<AbstractItem> Get(decimal discount);

        //allows you to remove elements to the collection and know if it was successfull
        bool Remove(int index);


    }

}

