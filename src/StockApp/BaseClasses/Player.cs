namespace StockApp.BaseClasses
{
    /// <summary>
    /// Spieler im TeamBewerb
    /// </summary>
    public class Player : TPlayer
    {

        #region Constructors

        public Player()
        {
            this.LicenseNumber = string.Empty;
        }

        public Player(string LastName = "lastname", string FirstName = "firstname") : this()
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
        }


        #endregion
    }

    public abstract class TPlayer : TBaseClass
    {
        private string firstname;

        public string FirstName
        {
            get { return firstname; }
            set
            {
                if (firstname == value)
                    return;

                firstname = value.Trim();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string lastname;

        public string LastName
        {
            get { return lastname; }
            set
            {
                if (lastname == value)
                    return;

                lastname = value.Trim();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string licensenumber;

        public string LicenseNumber
        {
            get { return licensenumber; }
            set
            {
                if (licensenumber == value)
                    return;
                licensenumber = value.Trim();
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return $"{(string.IsNullOrEmpty(LastName) ? "NACHNAME" : LastName.ToUpper())}, {(string.IsNullOrEmpty(FirstName) ? "Vorname" : FirstName)}";
            }
        }

    }
}
