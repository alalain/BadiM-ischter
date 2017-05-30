﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using BadiMeischter.Model;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace BadiMeischter.Pages
{
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private const string Url = "https://data.stadt-zuerich.ch/dataset/freibad/resource/c9d56476-344e-4081-af86-0b38a3cc8ccd/download/freibad.json";
        private ObservableCollection<Badi> _badiList;

        public HomePage()
        {
            InitializeComponent();
            BindingContext = this;
            BadiList = new System.Collections.ObjectModel.ObservableCollection<Badi>();
            BadiListView.Footer = string.Empty;
        }

		public ObservableCollection<Badi> BadiList
		{
			get { return _badiList; }
			set
			{
				_badiList = value;
				RaisePropertyChanged();
			}
		}

        #region Overrides of Page

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var result = "";


            //remove ! if not behind proxy
            if (CrossConnectivity.Current.IsConnected)
            {
                // Uncomment when not behind proxy              
                var client = new HttpClient();
                result = await client.GetStringAsync(Url);

				if (result == "")
					result = "[{\"Date\": \"Keine Daten vorhanden\"}]";
            } else {
				if (result == "")
				{
					await DisplayAlert("Keine Netzwerkverbindung", "Die Daten konnten nicht geladen werden", "Ok");
					result = "[{\"Date\": \"Die Daten konten nicht geladen werden\"}]";
				}
            }

            BadiList = new ObservableCollection<Badi>(JsonConvert.DeserializeObject<IEnumerable<Badi>>(result));
        }

		#endregion

		private void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			((ListView)sender).SelectedItem = null;
		}

		#region INotifyPropertyChanged

		public new event PropertyChangedEventHandler PropertyChanged = delegate { };

		private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
