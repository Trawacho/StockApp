using StockApp.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockApp.UserControls.StockTV
{
    /// <summary>
    /// Interaction logic for UserControlStockTV.xaml
    /// </summary>
    public partial class UserControlStockTV : UserControl, INotifyPropertyChanged, IEquatable<UserControlStockTV>, IComparable<UserControlStockTV>
    {

        #region IEquatable- and ICompareable Implementation

        /// <summary>
        /// True if Hostname is equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(UserControlStockTV other)
        {
            return this.StockTV.HostName.Equals(other.StockTV.HostName) &&
                this.StockTV.IPAddress.Equals(other.StockTV.IPAddress) &&
                this.StockTV.TVSettings.Bahn.Equals(other.StockTV.TVSettings.Bahn);
        }

        public int CompareTo(UserControlStockTV other)
        {
            var equalBahn = StockTV.TVSettings.Bahn.CompareTo(other.StockTV.TVSettings.Bahn);
            if (equalBahn != 0)
                return equalBahn;

            var equalHostName = this.StockTV.HostName.CompareTo(other.StockTV.HostName);
            if (equalHostName != 0)
                return equalHostName;

            return this.StockTV.IPAddress.CompareTo(other.StockTV.IPAddress);

        }

        #endregion


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void RaiseAllPropertiesChanged()
        {
            foreach (var p in this.GetType().GetProperties())
            {
                RaisePropertyChanged(p.Name);
            }
        }

        #endregion

        #region Konstruktor
        public UserControlStockTV()
        {
            InitializeComponent();
        }
        #endregion

        #region DependencyProperty StockTV

        public static readonly DependencyProperty StockTVProperty = DependencyProperty.Register(
          "StockTV", typeof(BaseClasses.StockTV), typeof(UserControlStockTV), new PropertyMetadata(null, StockTVChanged));
        public BaseClasses.StockTV StockTV
        {
            get { return (BaseClasses.StockTV)GetValue(StockTVProperty); }
            set { SetValue(StockTVProperty, value); }
        }

        #endregion

        #region Source-Lists for comboBoxes
        public List<BaseClasses.ColorModis> ColorModis
        {
            get => Enum.GetValues(typeof(BaseClasses.ColorModis)).Cast<BaseClasses.ColorModis>().ToList();
        }
        public List<string> Spielgruppen
        {
            get => new List<string>() { "--", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        }

        public List<BaseClasses.GameModis> GameModis
        {
            get => Enum.GetValues(typeof(BaseClasses.GameModis)).Cast<BaseClasses.GameModis>().ToList();
        }

        public List<BaseClasses.NextBahnModis> NextBahnModis
        {
            get => Enum.GetValues(typeof(BaseClasses.NextBahnModis)).Cast<BaseClasses.NextBahnModis>().ToList();
        }

        #endregion

        #region Properties

        public int BahnNummer
        {
            get => StockTV?.TVSettings.Bahn ?? -2;
            set
            {
                StockTV.TVSettings.Bahn = value;
                RaisePropertyChanged();
            }
        }

        private int Spielgruppe
        {
            get => StockTV?.TVSettings.Spielgruppe ?? 0;
            set
            {
                StockTV.TVSettings.Spielgruppe = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(SpielgruppeString));
            }
        }
        public string SpielgruppeString
        {
            get => Spielgruppe switch { 1 => "A", 2 => "B", 3 => "C", 4 => "D", 5 => "E", 6 => "F", 7 => "G", 8 => "H", 9 => "I", 10 => "J", 0 or _ => "--" };
            set => Spielgruppe = value switch { "A" => 1, "B" => 2, "C" => 3, "D" => 4, "E" => 5, "F" => 6, "G" => 7, "H" => 8, "I" => 9, "J" => 10, _ => 0 };
        }

        public int PunkteProKehre
        {
            get => StockTV?.TVSettings.PointsPerTurn ?? -2;
            set
            {
                StockTV.TVSettings.PointsPerTurn = value;
                RaisePropertyChanged();
            }
        }

        public int KehrenProSpiel
        {
            get => StockTV?.TVSettings.TurnsPerGame ?? -2;
            set
            {
                StockTV.TVSettings.TurnsPerGame = value;
                RaisePropertyChanged();
            }
        }

        public BaseClasses.GameModis GameModus
        {
            get => StockTV?.TVSettings.GameModus ?? BaseClasses.GameModis.Training;
            set
            {
                StockTV.TVSettings.GameModus = value;
                RaisePropertyChanged();
            }
        }

        public BaseClasses.ColorModis ColorModus
        {
            get => StockTV?.TVSettings.ColorModus ?? BaseClasses.ColorModis.Normal;
            set
            {
                StockTV.TVSettings.ColorModus = value;
                RaisePropertyChanged();
            }
        }

        public BaseClasses.NextBahnModis NextBahnModus
        {
            get => StockTV?.TVSettings.NextBahnModus ?? BaseClasses.NextBahnModis.Left;
            set
            {
                StockTV.TVSettings.NextBahnModus = value;
                RaisePropertyChanged();
            }
        }

        public bool IsOnline { get => StockTV?.IsOnline ?? false; }

        public string Identifier { get { return $"{StockTV?.HostName}" + Environment.NewLine + $"IP: {StockTV?.IPAddress}" + Environment.NewLine + $"{FirmwareVersion}"; } }
        public string FirmwareVersion
        {
            get
            {
                return $"FW: {StockTV?.FW ?? ""}";
            }
        }
        #endregion

        #region Events

        private void StockTV_StockTVSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseAllPropertiesChanged();
        }

        private static void StockTVChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is BaseClasses.StockTV tv &&
                o is UserControlStockTV ucTv)
            {
                tv.StockTVSettingsChanged += ucTv.StockTV_StockTVSettingsChanged;
                tv.StockTVOnlineChanged += ucTv.StockTV_StockTVOnlineChanged;
                ucTv.RaiseAllPropertiesChanged();
            }
        }



        private void StockTV_StockTVOnlineChanged(object sender, bool IsOnline)
        {
            RaisePropertyChanged(nameof(this.IsOnline));
        }

        #endregion

        #region Commands

        private ICommand _sendTVSettingsCommand; public ICommand SendTVSettingsCommand
        {
            get
            {
                return _sendTVSettingsCommand ??= new RelayCommand(
                    (p) =>
                    {
                        StockTV.TVSettingsSend();
                    },
                    (p) =>
                    {
                        return true;
                    });
            }
        }

        private ICommand _getTVSettingsCommand; public ICommand GetTVSettingsCommand
        {
            get
            {
                return _getTVSettingsCommand ??= new RelayCommand(
                    (p) =>
                    {
                        StockTV.TVSettingsGet();
                    },
                    (p) =>
                    {
                        return true;
                    });
            }
        }

        private ICommand _sendResetCommand; public ICommand SendResetCommand
        {
            get
            {
                return _sendResetCommand ??= new RelayCommand(
                    (p) =>
                    {
                        StockTV.TVResultReset();
                    },
                    (p) => true);
            }
        }

        private ICommand _sendGetResultCommand; public ICommand SendGetResultCommand
        {
            get
            {
                return _sendGetResultCommand ??= new RelayCommand(
                    (p) =>
                    {
                        StockTV.TVResultGet();
                    },
                    (p) => true);
            }
        }


        #endregion

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //http://192.168.100.136:8080/#Apps%20manager
            if (e.ClickCount == 2)
                System.Diagnostics.Process.Start($"http://{StockTV.IPAddress}:8080/#Apps%20manager");

        }


    }

}
