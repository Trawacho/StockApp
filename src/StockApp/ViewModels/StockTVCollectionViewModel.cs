using StockApp.BaseClasses;
using StockApp.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    internal class StockTVCollectionViewModel : BaseViewModel
    {
        /// <summary>
        /// Reference to StockTVs
        /// </summary>
        private readonly StockTVs _stockTVs;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="stockTVs"></param>
        public StockTVCollectionViewModel(ref StockTVs stockTVs)
        {
            this.StockTVCollection = new ObservableCollection<StockTV>();
            this._stockTVs = stockTVs;
            this._stockTVs.StockTVCollectionAdded += StockTVs_StockTVCollectionAdded;
            this._stockTVs.StockTVCollectionRemoved += StockTVs_StockTVCollectionRemoved;

            AddStockTVsToCollection(_stockTVs);
        }

        #region Events

        #region _stockTVs - Based
        /// <summary>
        /// Rebuild the <see cref="StockTVCollection"/> after a StockTV removed from the internal <see cref="_stockTVs"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockTVs_StockTVCollectionRemoved(object sender, StockTVCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                RemoveStockTVsFromCollection();
                AddStockTVsToCollection(_stockTVs);
            });
        }

        /// <summary>
        /// Add the new StockTV to the <see cref="StockTVCollection"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">the new StockTV</param>
        private void StockTVs_StockTVCollectionAdded(object sender, StockTVCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                AddStockTVsToCollection(e.StockTV);
            });
        }

        #endregion

        #region StockTV - Based

        /// <summary>
        /// Set the changed value from <see cref="SyncDirectorTV"/> TVSettings to all <see cref="SyncPerformerTVs"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockTV_StockTVSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Sync && e.PropertyName != nameof(StockTVSettings.Bahn))
            {
                if (sender is StockTV s)
                {
                    if (s == SyncDirectorTV)
                    {
                        var newValue = typeof(StockTVSettings).GetProperty(e.PropertyName).GetValue(s.TVSettings);
                        foreach (var item in SyncPerformerTVs)
                        {
                            typeof(StockTVSettings).GetProperty(e.PropertyName).SetValue(item.TVSettings, newValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Only when  <see cref="Sync"/> is TRUE 
        /// <br>
        /// After the Settings are Sent to the <see cref="SyncDirectorTV"/> copy it to all <see cref="SyncPerformerTVs"/> and Sent it also 
        /// </br>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockTV_TVSettingsSent(object sender, EventArgs e)
        {
            if (Sync)
            {
                if (sender is StockTV s)
                {
                    if (s == SyncDirectorTV)
                    {
                        foreach (var tv in SyncPerformerTVs)
                        {
                            tv.TVSettings.CopyFrom(s.TVSettings);
                            tv.TVSettingsSend();
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Properties and Commands to Bind 

        public ObservableCollection<StockTV> StockTVCollection { get; private set; }

        public bool Sync { get; set; }

        private ICommand _ReOrderStockTVCollectionCommand;
        public ICommand ReOrderStockTVCollectionCommand
        {
            get
            {
                return _ReOrderStockTVCollectionCommand ??= new RelayCommand(
                    (p) =>
                    {
                        StockTVCollection.Sort(StockTVCollection.FirstOrDefault()?.TVSettings.NextBahnModus == NextBahnModis.Right);
                    },
                    (p) => true
                    );
            }
        }

        #endregion

        #region Private Functions and Properties

        private void RemoveStockTVsFromCollection()
        {
            foreach (StockTV item in _stockTVs)
            {
                item.StockTVSettingsChanged -= StockTV_StockTVSettingsChanged;
                item.TVSettingsSent -= StockTV_TVSettingsSent;
            }
            StockTVCollection.Clear();
        }

        private void AddStockTVsToCollection(StockTV tv)
        {
            tv.StockTVSettingsChanged += StockTV_StockTVSettingsChanged;
            tv.TVSettingsSent += StockTV_TVSettingsSent;
            this.StockTVCollection.Add(tv);
        }

        private void AddStockTVsToCollection(IEnumerable<StockTV> collection)
        {
            foreach (var tv in collection)
            {
                AddStockTVsToCollection(tv);
            }
        }

        private StockTV SyncDirectorTV
        {
            get => Sync ? StockTVCollection?.First() : null;
        }
        private IEnumerable<StockTV> SyncPerformerTVs
        {
            get => Sync ? StockTVCollection.Where(tv => tv != SyncDirectorTV) : null;
        }

        #endregion
    }

    internal class StockTVCollectionDesignViewModel
    {
        public StockTVCollectionDesignViewModel()
        {
            this.StockTVCollection = new ObservableCollection<StockTV>
            {
                new StockTV("StockTV-1", "10.10.10.1"),
                new StockTV("StockTV-2", "10.10.10.2"),
                new StockTV("StockTV-3", "10.10.10.3"),
                new StockTV("StockTV-4", "10.10.10.4")
            };
        }
        public ObservableCollection<StockTV> StockTVCollection { get; private set; }

        public ICommand ReOrderStockTVCollectoinCommand { get; }

        public bool Sync { get; set; }
    }
}