﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MonoStockPortfolio.Core.PortfolioRepositories;
using MonoStockPortfolio.Core.Services;
using MonoStockPortfolio.Entities;

namespace MonoStockPortfolio
{
    [Activity(Label = "Stock Portfolio", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static string ClassName { get { return "monostockportfolio.MainActivity"; } }

        private IPortfolioService _svc;
        private IList<Portfolio> _portfolios;
        private string[] _longClickOptions;
        private IPortfolioRepository _repo;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _svc = new PortfolioService(this);
            _repo = new AndroidSqlitePortfolioRepository(this);

            SetContentView(Resource.layout.main);

            RefreshList();

            WireUpEvents();
        }

        private Button AddPortfolioButton { get { return FindViewById<Button>(Resource.id.btnAddPortfolio); } }
        private ListView PortfolioListView { get { return FindViewById<ListView>(Resource.id.portfolioList); } }

        private void RefreshList()
        {
            _portfolios = _svc.GetAllPortfolios();

            var listAdapter = new ArrayAdapter<string>(this, Android.R.Layout.SimpleListItem1, _portfolios.Select(p => p.Name).ToList());
            PortfolioListView.Adapter = listAdapter;
        }

        private void WireUpEvents()
        {
            AddPortfolioButton.Click += addPortfolioButton_Click;
            PortfolioListView.ItemLongClick += PortfolioListView_ItemLongClick;
            PortfolioListView.ItemClick += listView_ItemClick;
        }

        void PortfolioListView_ItemLongClick(object sender, ItemEventArgs e)
        {
            _longClickOptions = new[] {"Edit", "Delete"};
            var selectedPortfolio = ((TextView) e.View).Text.ToS();
            var dialogBuilder = new AlertDialog.Builder(this);
            dialogBuilder.SetTitle("Options");
            dialogBuilder.SetItems(_longClickOptions,
                                   (senderi, ei) => tr_LongClick_Options(senderi, ei, selectedPortfolio));
            dialogBuilder.Create().Show();
        }

        private void tr_LongClick_Options(object sender, DialogClickEventArgs e, string selectedPortfolio)
        {
            //Toast.MakeText(this, "Option: " + _longClickOptions[e.Which], ToastLength.Long).Show();
            if(_longClickOptions[e.Which] == "Edit")
            {
                // Edit
            }
            else if (_longClickOptions[e.Which] == "Delete")
            {
                // Delete
                _repo.DeletePortfolio(selectedPortfolio);
                RefreshList();
            }
        }

        private void listView_ItemClick(object sender, ItemEventArgs e)
        {
            var intent = new Intent();
            intent.SetClassName(this, PortfolioActivity.ClassName);
            intent.PutExtra(PortfolioActivity.Extra_PortfolioID, _portfolios[e.Position].ID ?? -1);
            StartActivityForResult(intent, 0);
        }

        private void addPortfolioButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            intent.SetClassName(this, AddPortfolioActivity.ClassName);
            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            RefreshList();
        }
    }
}

