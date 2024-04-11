﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SafeSkate.Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.listView.ItemsSource = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy.MapMarkerInfos;
        }
    }
}
