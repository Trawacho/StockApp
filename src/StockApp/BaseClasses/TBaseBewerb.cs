using StockApp.Interfaces;
using System;

namespace StockApp.BaseClasses
{
    /// <summary>
    /// Basisklasse für einen Bewerb
    /// <para>Erbt von <see cref="TBaseClass"/> und implementiert <see cref="IBaseBewerb"/></para>
    /// </summary>
    public abstract class TBaseBewerb : TBaseClass, IBaseBewerb
    {
        public TBaseBewerb()
        {

        }

        public abstract void SetBroadcastData(byte[] data);
    }

   
}

