using StockApp.Interfaces;
using System;

namespace StockApp.BaseClasses
{
    /// <summary>
    /// Basisklasse für einen Bewerb
    /// </summary>
    public abstract class TBaseBewerb : TBaseClass, IBaseBewerb
    {
        public TBaseBewerb()
        {

        }

        public abstract void SetBroadcastData(byte[] data);
    }

   
}

