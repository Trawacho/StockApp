using StockApp.BaseClasses.Zielschiessen;
using StockApp.Interfaces;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace StockApp.BaseClasses
{
    public delegate void SpielgruppeChangedEventHandler(object sender, EventArgs e);

    public class Turnier : TBaseClass, ITurnier
    {

        public event SpielgruppeChangedEventHandler SpielgruppeChanged;
        protected virtual void RaiseSpielgruppeChanged()
        {
            SpielgruppeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Organisatorische Daten jedes Turniers
        /// </summary>
        public OrgaDaten OrgaDaten { get; set; }

        private readonly TeamBewerb teambewerb;
        private readonly Zielbewerb zielbewerb;


        //private int spielGruppe;

        ///// <summary>
        ///// Nummer der Gruppe, wenn mehrere Gruppen gleichzeitig auf der Spielfläche sind
        ///// 
        ///// Default: 0
        ///// </summary>
        //public int SpielGruppe
        //{
        //    get => this.spielGruppe;
        //    set
        //    {
        //        this.spielGruppe = value;
        //        RaisePropertyChanged();
        //    }
        //}
        //public string SpielGruppeString()
        //{
        //    switch (SpielGruppe)
        //    {
        //        case 1:
        //            return "A";
        //        case 2:
        //            return "B";
        //        case 3:
        //            return "C";
        //        case 4:
        //            return "D";
        //        case 5:
        //            return "E";
        //        case 6:
        //            return "F";
        //        case 7:
        //            return "G";
        //        case 8:
        //            return "H";
        //        case 9:
        //            return "I";
        //        case 10:
        //            return "J";
        //        case 0:
        //        default:
        //            return string.Empty;
        //    }
        //}


        private IBaseBewerb _wettbewerb;
        /// <summary>
        /// Kann ein <see cref="TeamBewerb"/> oder <see cref="Zielbewerb"/> sein
        /// </summary>
        public IBaseBewerb Wettbewerb
        {
            get
            {
                return _wettbewerb;
            }
            private set
            {
                if (value == _wettbewerb)
                    return;

                _wettbewerb = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Default Konstruktor
        /// </summary>
        public Turnier()
        {
            this.OrgaDaten = new OrgaDaten();

            teambewerb = new TeamBewerb();
            teambewerb.PropertyChanged += Teambewerb_PropertyChanged;

            zielbewerb = new Zielbewerb();
        }

        private void Teambewerb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TeamBewerb.SpielGruppe))
                RaiseSpielgruppeChanged();
        }

        public void SetBewerb(Wettbewerbsart art)
        {
            this.Wettbewerb = art == Wettbewerbsart.Team
                ? teambewerb
                : zielbewerb;

            RaiseSpielgruppeChanged();
        }



        public static void Save(Turnier turnier, string filePath)
        {
            var set = new SerializableTurnier();
            set.SetTurnier(turnier);
            var xmlString = "";

            using (var stringWriter = new StringWriter())
            using (var writer = XmlWriter.Create(stringWriter))
            {
                var serializer = new XmlSerializer(typeof(SerializableTurnier));
                serializer.Serialize(writer, set);
                xmlString = stringWriter.ToString();
            }

            File.WriteAllText(filePath, xmlString);
        }

        public static Turnier Load(string filePath)
        {
            using StreamReader reader = new StreamReader(filePath);
            var serializer = new XmlSerializer(typeof(SerializableTurnier));
            var set = serializer.Deserialize(reader) as SerializableTurnier;
            return set.GetTurnier();
        }
    }
}
