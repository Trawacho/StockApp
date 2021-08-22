using StockApp.Interfaces;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace StockApp.BaseClasses
{
    public class Turnier : TBaseClass, ITurnier
    {
        /// <summary>
        /// Organisatorische Daten jedes Turniers
        /// </summary>
        public OrgaDaten OrgaDaten { get; set; }

        private int spielGruppe;

        /// <summary>
        /// Nummer der Gruppe, wenn mehrere Gruppen gleichzeitig auf der Spielfläche sind
        /// 
        /// Default: 0
        /// </summary>
        public int SpielGruppe
        {
            get => this.spielGruppe;
            set
            {
                this.spielGruppe = value;
                RaisePropertyChanged();
            }
        }
        public string SpielGruppeString()
        {
            switch (SpielGruppe)
            {
                case 1:
                    return "A";
                case 2:
                    return "B";
                case 3:
                    return "C";
                case 4:
                    return "D";
                case 5:
                    return "E";
                case 6:
                    return "F";
                case 7:
                    return "G";
                case 8:
                    return "H";
                case 9:
                    return "I";
                case 10:
                    return "J";
                case 0:
                default:
                    return string.Empty;
            }
        }


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
            set
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
        }




        public static void Save(Turnier turnier, string filePath)
        {
            if (!(turnier.Wettbewerb is TeamBewerb))
            {
                throw new InvalidCastException("Es kann nur ein Teambewerb gespeicert werden");
            }

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
