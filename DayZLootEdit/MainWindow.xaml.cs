using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DayZLootEdit
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // VARIABLES

        private const string WINDOW_TITLE_DEFAULT = "DayZ Loot Editor";
        private const string WINDOW_TITLE_FILE = "DayZ Loot Editor - [%1]";

        private string LootFile = "";
        private LootTable LootTable;

        private List<string> categoryList = new List<string>();
        private List<string> tagList = new List<string>();

        // MAIN WINDOW

        public MainWindow()
        {

            InitializeComponent();
            InitializeControls();

        }

        private void InitializeControls()
        {

            this.Title = WINDOW_TITLE_DEFAULT;
            MnuFileSave.IsEnabled = false;
            MnuFileSaveAs.IsEnabled = false;
            MnuFileClose.IsEnabled = false;
            MnuFileReload.IsEnabled = false;
            MnuEditSelectAll.IsEnabled = false;
            MnuEditSelectNone.IsEnabled = false;
            MnuEditName.IsEnabled = false;
            MnuEditAmount.IsEnabled = false;
            MnuEditLifetime.IsEnabled = false;
            MnuEditRestock.IsEnabled = false;
            MnuEditQuantity.IsEnabled = false;
            MnuEditCost.IsEnabled = false;
            MnuEditCount.IsEnabled = false;
            MnuEditMark.IsEnabled = false;
            MnuEditClear.IsEnabled = false;
            MnuEditDelete.IsEnabled = false;

            ResetPercentControls();
            ResetFilterControls();

            txtFilterName.Text = "";
            cmbFilterCategory.SelectedIndex = 0;
            cmbFilterTag.SelectedIndex = 0;
            txtFilterUsage.Text = "";
            txtFilterValue.Text = "";

            grpPercent.IsEnabled = false;
            grpFilter.IsEnabled = false;

            lblStatusCount.Content = "No Data";
            prgLoading.Visibility = Visibility.Hidden;

        }

        private async void OpenLootFile(string lFile)
        {

            try
            {

                string windowTitle = "";
                StringBuilder builder = new StringBuilder(WINDOW_TITLE_FILE);
                builder.Replace("%1", lFile);
                windowTitle = builder.ToString();
                this.Title = windowTitle;
                this.IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;

                LootList.ItemsSource = null;
                ResetPercentControls();
                ResetFilterControls();

                lblStatusCount.Content = "loading...";

                LootTable = new LootTable(lFile);
                LootTable.LoadFile();

                prgLoading.Value = 0;
                prgLoading.Maximum = LootTable.Loot.Count;
                prgLoading.Visibility = Visibility.Visible;
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                RefreshFilterListing();

                prgLoading.Value = LootTable.Loot.Count;
                await Task.Delay(TimeSpan.FromSeconds(1));
                prgLoading.Visibility = Visibility.Hidden;

                this.IsEnabled = true;

                LootList.ItemsSource = LootTable.Loot;
                LootList.IsEnabled = true;

                MnuFileSave.IsEnabled = true;
                MnuFileSaveAs.IsEnabled = true;
                MnuFileClose.IsEnabled = true;
                MnuFileReload.IsEnabled = true;
                MnuEditSelectAll.IsEnabled = true;
                MnuEditSelectNone.IsEnabled = false;
                MnuEditAmount.IsEnabled = false;
                MnuEditLifetime.IsEnabled = false;
                MnuEditRestock.IsEnabled = false;
                MnuEditQuantity.IsEnabled = false;
                MnuEditCost.IsEnabled = false;
                MnuEditCount.IsEnabled = false;
                MnuEditMark.IsEnabled = false;
                MnuEditClear.IsEnabled = false;
                MnuEditDelete.IsEnabled = false;

                grpFilter.IsEnabled = true;

                Mouse.OverrideCursor = null;
                UpdateStatus();
                
            }
            catch (Exception err)
            {

                Mouse.OverrideCursor = null;

                string mb_caption = "Error";
                string mb_message = "An error occurred!\n\n%1";
                StringBuilder builder = new StringBuilder(mb_message);
                builder.Replace("%1", err.ToString());
                mb_message = builder.ToString();

                MessageBox.Show(
                    mb_message,
                    mb_caption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                this.IsEnabled = true;
                InitializeControls();

            }

        }

        private async void SaveLootFile(string lFile = "")
        {

            try
            {

                if (lFile == "")
                {
                    lFile = LootTable.FilePath;
                }

                string windowTitle = "";
                StringBuilder builder = new StringBuilder(WINDOW_TITLE_FILE);
                builder.Replace("%1", lFile);
                windowTitle = builder.ToString();
                this.Title = windowTitle;
                this.IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;

                lblStatusCount.Content = "saving...";

                prgLoading.Value = 0;
                prgLoading.Maximum = LootTable.Loot.Count;
                prgLoading.Visibility = Visibility.Visible;
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                LootTable.SaveFile(lFile);

                prgLoading.Value = LootTable.Loot.Count;
                await Task.Delay(TimeSpan.FromSeconds(1));
                prgLoading.Visibility = Visibility.Hidden;

                /*
                string mb_caption = "Success";
                string mb_message = "File '%1' saved.";
                StringBuilder mb_builder = new StringBuilder(mb_message);
                mb_builder.Replace("%1", LootTable.FilePath);
                mb_message = mb_builder.ToString();

                MessageBox.Show(
                    mb_message,
                    mb_caption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                */

                this.IsEnabled = true;

                Mouse.OverrideCursor = null;
                UpdateStatus();

            }
            catch (Exception err)
            {

                Mouse.OverrideCursor = null;

                string mb_caption = "Error";
                string mb_message = "An error occurred!\n\n%1";
                StringBuilder builder = new StringBuilder(mb_message);
                builder.Replace("%1", err.ToString());
                mb_message = builder.ToString();

                MessageBox.Show(
                    mb_message,
                    mb_caption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                this.IsEnabled = true;
                InitializeControls();

            }

        }

        // MENU FILE

        private void MnuFileOpen_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            dlg.Title = "Open DayZ Types-File";

            if (dlg.ShowDialog().Value)
            {
                LootFile = dlg.FileName;
                OpenLootFile(LootFile);
            }

        }

        private void MnuFileSave_Click(object sender, RoutedEventArgs e)
        {

            SaveLootFile();

        }

        private void MnuFileSaveAs_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            dlg.Title = "Save DayZ Types-File";

            if (dlg.ShowDialog().Value)
            {
                LootFile = dlg.FileName;
                SaveLootFile(LootFile);
            }

        }

        private void MnuFileReload_Click(object sender, RoutedEventArgs e)
        {

            OpenLootFile(LootFile);

        }

        private void MnuFileClose_Click(object sender, RoutedEventArgs e)
        {

            LootList.ItemsSource = null;
            InitializeControls();

        }

        private void MnuFileExit_Click(object sender, RoutedEventArgs e)
        {

            this.Close();

        }

        // MENU EDIT

        private void MnuEditSelectAll_Click(object sender, RoutedEventArgs e)
        {
            LootList.SelectAll();
            LootList.Focus();
        }

        private void MnuEditSelectNone_Click(object sender, RoutedEventArgs e)
        {
            LootList.SelectedIndex = -1;
            LootList.Focus();
        }

        private void MnuEditNameSearchDuckDuck_Click(object sender, RoutedEventArgs e)
        {
            string uriBase = "https://duckduckgo.com/?q=";
            string selectedClass = "";
            foreach (LootType loot in LootList.SelectedItems)
            {
                selectedClass = loot.Name;
                break;
            }
            string searchText = "DayZ " + selectedClass;
            string uriQuery = uriBase + HttpUtility.UrlEncode(searchText);
            Process.Start(uriQuery);
        }

        private void MnuEditNameSearchGoogle_Click(object sender, RoutedEventArgs e)
        {
            string uriBase = "https://www.google.com/search?q=";
            string selectedClass = "";
            foreach (LootType loot in LootList.SelectedItems)
            {
                selectedClass = loot.Name;
                break;
            }
            string searchText = "DayZ " + selectedClass;
            string uriQuery = uriBase + HttpUtility.UrlEncode(searchText);
            Process.Start(uriQuery);
        }

        private void MnuEditAmountNominalToMin_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                int value = loot.Nominal;
                loot.Min = value;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditAmountMinToNominal_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                int value = loot.Min;
                loot.Nominal = value;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditAmountZero_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.Nominal = 0;
                loot.Min = 0;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditLifetimeValue_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.Lifetime = int.Parse(item.Tag.ToString());
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditRestockValue_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.Restock = int.Parse(item.Tag.ToString());
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityNormal_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = -1;
                loot.QMin = -1;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityQMaxValue_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = int.Parse(item.Header.ToString());
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityQMinValue_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMin = int.Parse(item.Header.ToString());
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_100_1_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 100;
                loot.QMin = 1;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_100_10_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 100;
                loot.QMin = 10;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_100_20_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 100;
                loot.QMin = 20;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_100_30_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 100;
                loot.QMin = 30;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_90_10_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 90;
                loot.QMin = 10;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_80_20_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 80;
                loot.QMin = 20;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_70_30_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 70;
                loot.QMin = 30;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_60_10_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 60;
                loot.QMin = 10;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_50_10_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 50;
                loot.QMin = 10;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_40_10_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 40;
                loot.QMin = 10;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_30_10_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 30;
                loot.QMin = 10;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditQuantityPresets_0_0_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.QMax = 0;
                loot.QMin = 0;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditCostValue_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.Cost = int.Parse(item.Header.ToString());
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditCountInCargo_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = MnuEditCountInCargo.IsChecked;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.InCargo = isChecked;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditCountInHoarder_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = MnuEditCountInHoarder.IsChecked;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.InHoarder = isChecked;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditCountInMap_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = MnuEditCountInMap.IsChecked;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.InMap = isChecked;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditCountInPlayer_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = MnuEditCountInPlayer.IsChecked;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.InPlayer = isChecked;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditMarkCrafted_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = MnuEditMarkCrafted.IsChecked;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.Crafted = isChecked;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditMarkDeloot_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = MnuEditMarkDeloot.IsChecked;
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.Deloot = isChecked;
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditClearTag_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.Tag = "";
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditClearUsage_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.Usage = "";
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditClearValue_Click(object sender, RoutedEventArgs e)
        {
            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.Value = "";
            }
            LootList.Items.Refresh();
            LootList.Focus();
        }

        private void MnuEditDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        // MENU HELP

        private void MnuHelpWiki_Click(object sender, RoutedEventArgs e)
        {
            string uriQuery = "https://github.com/ojemineh/DayZLootEdit/wiki";
            Process.Start(uriQuery);
        }

        private void MnuHelpGitHub_Click(object sender, RoutedEventArgs e)
        {
            string uriQuery = "https://github.com/ojemineh/DayZLootEdit";
            Process.Start(uriQuery);
        }

        private void MnuHelpInfo_Click(object sender, RoutedEventArgs e)
        {

            InfoWindow iw = new InfoWindow();
            iw.ShowDialog();

        }

        // PERCENT FUNCTIONS

        private void UpdatePercentControls()
        {

            int newval = 0;
            bool ok = int.TryParse(txtPercent.Text.Replace("%", ""), out newval);

            if (ok)
            {
                sldPercent.Value = newval;
                txtPercent.Text = String.Format("{0}%", newval);
                btnPercent?.Focus();
            }

        }

        private void ResetPercentControls()
        {

            txtPercent.Text = "100%";
            sldPercent.Value = 100;
            btnPercent.IsEnabled = true;

        }

        private void txtPercent_GotFocus(object sender, RoutedEventArgs e)
        {

            txtPercent.SelectAll();

        }

        private void txtPercent_LostFocus(object sender, RoutedEventArgs e)
        {

            UpdatePercentControls();

        }

        private void txtPercent_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                UpdatePercentControls();
            }

        }

        private void sldPercent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            txtPercent.Text = Math.Round(sldPercent.Value).ToString();
            UpdatePercentControls();

        }

        private void btnPercent_Click(object sender, RoutedEventArgs e)
        {

            int percentage = 0;
            bool ok = int.TryParse(txtPercent.Text.Replace("%", ""), out percentage);
            if (!ok) return;

            foreach (LootType loot in LootList.SelectedItems)
            {
                loot.SetNominal(percentage);
                loot.SetMin(percentage);
            }

            UpdatePercentControls();
            LootList.Items.Refresh();
            LootList.Focus();

        }

        // FILTER FUNCTIONS

        private void RefreshFilterListing()
        {

            categoryList.Clear();
            categoryList.Add("");

            for (int i = 1; i < LootTable.Loot.Count; i++)
            {
                prgLoading.Value = i;
                if (LootTable.Loot[i].Category != null)
                {
                    bool exists = string.Concat(categoryList).Contains(LootTable.Loot[i].Category);
                    if (!exists)
                    {
                        categoryList.Add(LootTable.Loot[i].Category);
                    }
                }
            }

            cmbFilterCategory.ItemsSource = categoryList;
            cmbFilterCategory.SelectedIndex = 0;

            tagList.Clear();
            tagList.Add("");

            for (int i = 1; i < LootTable.Loot.Count; i++)
            {
                prgLoading.Value = i;
                if (LootTable.Loot[i].Tag != null)
                {
                    bool exists = string.Concat(tagList).Contains(LootTable.Loot[i].Tag);
                    if (!exists)
                    {
                        tagList.Add(LootTable.Loot[i].Tag);
                    }
                }
            }

            cmbFilterTag.ItemsSource = tagList;
            cmbFilterTag.SelectedIndex = 0;

        }

        private void ResetFilterControls()
        {

            chkFilterName.IsChecked = false;
            //txtFilterName.Text = "";

            chkFilterInCargo.IsChecked = false;
            chkFilterInHoarder.IsChecked = false;
            chkFilterInMap.IsChecked = false;
            chkFilterInPlayer.IsChecked = false;
            chkFilterCrafted.IsChecked = false;
            chkFilterDeloot.IsChecked = false;

            chkFilterCategory.IsChecked = false;
            //cmbFilterCategory.SelectedIndex = 0;

            chkFilterTag.IsChecked = false;
            //cmbFilterTag.SelectedIndex = 0;

            chkFilterUsage.IsChecked = false;
            //txtFilterUsage.Text = "";

            chkFilterValue.IsChecked = false;
            //txtFilterValue.Text = "";

            //btnFilterReset.IsEnabled = true;
            btnFilterExec.IsEnabled = false;

        }

        private void ResetFilter()
        {
            LootList.ItemsSource = LootTable.Loot;
            ResetFilterControls();
            UpdateStatus();

        }

        private void UpdateFilter()
        {
            ListCollectionView collectionView = new ListCollectionView(LootTable.Loot);
            List<object> filteredList = new List<object>();

            foreach (var item in collectionView)
            {
                LootType lt = item as LootType;
                if (chkFilterName.IsChecked == true)
                {
                    string nameItem = txtFilterName.Text.ToString();
                    if (!lt.Name.ToLower().Contains(nameItem.ToLower()))
                        continue;
                }
                if (chkFilterInCargo.IsChecked == true)
                {
                    if (!(lt.InCargo == true))
                        continue;
                }
                if (chkFilterInHoarder.IsChecked == true)
                {
                    if (!(lt.InHoarder == true))
                        continue;
                }
                if (chkFilterInMap.IsChecked == true)
                {
                    if (!(lt.InMap == true))
                        continue;
                }
                if (chkFilterInPlayer.IsChecked == true)
                {
                    if (!(lt.InPlayer == true))
                        continue;
                }
                if (chkFilterCrafted.IsChecked == true)
                {
                    if (!(lt.Crafted == true))
                        continue;
                }
                if (chkFilterDeloot.IsChecked == true)
                {
                    if (!(lt.Deloot == true))
                        continue;
                }
                if (chkFilterCategory.IsChecked == true)
                {
                    string categoryItem = cmbFilterCategory.Items[cmbFilterCategory.SelectedIndex].ToString();
                    if (categoryItem == "")
                    {
                        if (!(lt.Category == null))
                            continue;
                    }
                    else
                    {
                        if (!(lt.Category == categoryItem))
                            continue;
                    }
                }
                if (chkFilterTag.IsChecked == true)
                {
                    string tagItem = cmbFilterTag.Items[cmbFilterTag.SelectedIndex].ToString();
                    if (!(lt.Tag == tagItem))
                        continue;
                }
                if (chkFilterUsage.IsChecked == true)
                {
                    var usageItem = txtFilterUsage.Text.ToLower().ToString();
                    var filterStr = usageItem.Split(',').Select(x => x.Trim()).ToArray();
                    if (!filterStr.All(x => lt.Usage.ToLower().Contains(x)))
                        continue;
                }
                if (chkFilterValue.IsChecked == true)
                {
                    var valueItem = txtFilterValue.Text.ToLower().ToString();
                    var filterStr = valueItem.Split(',').Select(x => x.Trim()).ToArray();
                    if (!filterStr.All(x => lt.Value.ToLower().Contains(x)))
                        continue;
                }
                filteredList.Add(item);
            }
            UpdateStatus(filteredList.Count);
            LootList.ItemsSource = filteredList;
        }

        private void UpdateFilterExec()
        {
            
            if (
                (chkFilterName.IsChecked == false) &&
                (chkFilterInCargo.IsChecked == false) &&
                (chkFilterInHoarder.IsChecked == false) &&
                (chkFilterInMap.IsChecked == false) &&
                (chkFilterInPlayer.IsChecked == false) &&
                (chkFilterCrafted.IsChecked == false) &&
                (chkFilterDeloot.IsChecked == false) &&
                (chkFilterCategory.IsChecked == false) &&
                (chkFilterTag.IsChecked == false) &&
                (chkFilterUsage.IsChecked == false) &&
                (chkFilterValue.IsChecked == false)
            )
            {
                btnFilterExec.IsEnabled = false;
            }
            else
            {
                btnFilterExec.IsEnabled = true;
            }

        }

        private void btnFilterExec_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilter();
            LootList.Focus();
        }

        private void btnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            ResetFilter();
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                txtFilterName.Text = "";
                cmbFilterCategory.SelectedIndex = 0;
                cmbFilterTag.SelectedIndex = 0;
                txtFilterUsage.Text = "";
                txtFilterValue.Text = "";
            }
            LootList.Focus();
        }

        private void txtFilterName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtFilterName.SelectAll();
        }

        private void txtFilterUsage_GotFocus(object sender, RoutedEventArgs e)
        {
            txtFilterUsage.SelectAll();
        }

        private void txtFilterValue_GotFocus(object sender, RoutedEventArgs e)
        {
            txtFilterValue.SelectAll();
        }

        private void chkFilterName_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterInCargo_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterInHoarder_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterInMap_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterInPlayer_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterCrafted_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterDeloot_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterCategory_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterTag_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterUsage_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        private void chkFilterValue_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilterExec();
        }

        // LOOT LIST (DATA GRID)

        private void LootList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool IsLootSelected = LootList.SelectedItems.Count > 0;
            MnuEditSelectNone.IsEnabled = IsLootSelected;
            MnuEditName.IsEnabled = IsLootSelected;
            MnuEditAmount.IsEnabled = IsLootSelected;
            MnuEditLifetime.IsEnabled = IsLootSelected;
            MnuEditRestock.IsEnabled = IsLootSelected;
            MnuEditQuantity.IsEnabled = IsLootSelected;
            MnuEditCost.IsEnabled = IsLootSelected;
            MnuEditCount.IsEnabled = IsLootSelected;
            MnuEditMark.IsEnabled = IsLootSelected;
            MnuEditClear.IsEnabled = IsLootSelected;
            MnuEditDelete.IsEnabled = IsLootSelected;
            grpPercent.IsEnabled = IsLootSelected;
            if (LootList.SelectedItems.Count > 0)
            {
                int cargoChecked = 0;
                int cargoUnchecked = 0;
                foreach (LootType loot in LootList.SelectedItems)
                {
                    if (loot.InCargo == true)
                        cargoChecked++;
                    if (loot.InCargo == false)
                        cargoUnchecked++;
                }
                if (cargoChecked == LootList.SelectedItems.Count)
                {
                    MnuEditCountInCargo.IsChecked = true;
                }
                else if (cargoUnchecked == LootList.SelectedItems.Count)
                {
                    MnuEditCountInCargo.IsChecked = false;
                    MnuEditCountInCargo.Icon = null;
                }
                else
                {
                    MnuEditCountInCargo.IsChecked = false;
                    MnuEditCountInCargo.Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/DayZLootEdit;component/checkbox_mixed.png")) };
                }
                int hoarderChecked = 0;
                int hoarderUnchecked = 0;
                foreach (LootType loot in LootList.SelectedItems)
                {
                    if (loot.InHoarder == true)
                        hoarderChecked++;
                    if (loot.InHoarder == false)
                        hoarderUnchecked++;
                }
                if (hoarderChecked == LootList.SelectedItems.Count)
                {
                    MnuEditCountInHoarder.IsChecked = true;
                }
                else if (hoarderUnchecked == LootList.SelectedItems.Count)
                {
                    MnuEditCountInHoarder.IsChecked = false;
                    MnuEditCountInPlayer.Icon = null;
                }
                else
                {
                    MnuEditCountInHoarder.IsChecked = false;
                    MnuEditCountInHoarder.Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/DayZLootEdit;component/checkbox_mixed.png")) };
                }
                int mapChecked = 0;
                int mapUnchecked = 0;
                foreach (LootType loot in LootList.SelectedItems)
                {
                    if (loot.InMap == true)
                        mapChecked++;
                    if (loot.InMap == false)
                        mapUnchecked++;
                }
                if (mapChecked == LootList.SelectedItems.Count)
                {
                    MnuEditCountInMap.IsChecked = true;
                }
                else if (mapUnchecked == LootList.SelectedItems.Count)
                {
                    MnuEditCountInMap.IsChecked = false;
                    MnuEditCountInPlayer.Icon = null;
                }
                else
                {
                    MnuEditCountInMap.IsChecked = false;
                    MnuEditCountInMap.Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/DayZLootEdit;component/checkbox_mixed.png")) };
                }
                int playerChecked = 0;
                int playerUnchecked = 0;
                foreach (LootType loot in LootList.SelectedItems)
                {
                    if (loot.InPlayer == true)
                        playerChecked++;
                    if (loot.InPlayer == false)
                        playerUnchecked++;
                }
                if (playerChecked == LootList.SelectedItems.Count)
                {
                    MnuEditCountInPlayer.IsChecked = true;
                }
                else if (playerUnchecked == LootList.SelectedItems.Count)
                {
                    MnuEditCountInPlayer.IsChecked = false;
                    MnuEditCountInPlayer.Icon = null;
                }
                else
                {
                    MnuEditCountInPlayer.IsChecked = false;
                    MnuEditCountInPlayer.Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/DayZLootEdit;component/checkbox_mixed.png")) };
                }
                int craftedChecked = 0;
                int craftedUnchecked = 0;
                foreach (LootType loot in LootList.SelectedItems)
                {
                    if (loot.Crafted == true)
                        craftedChecked++;
                    if (loot.Crafted == false)
                        craftedUnchecked++;
                }
                if (craftedChecked == LootList.SelectedItems.Count)
                {
                    MnuEditMarkCrafted.IsChecked = true;
                }
                else if (craftedUnchecked == LootList.SelectedItems.Count)
                {
                    MnuEditMarkCrafted.IsChecked = false;
                    MnuEditMarkCrafted.Icon = null;
                }
                else
                {
                    MnuEditMarkCrafted.IsChecked = false;
                    MnuEditMarkCrafted.Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/DayZLootEdit;component/checkbox_mixed.png")) };
                }
                int delootChecked = 0;
                int delootUnchecked = 0;
                foreach (LootType loot in LootList.SelectedItems)
                {
                    if (loot.Deloot == true)
                        delootChecked++;
                    if (loot.Deloot == false)
                        delootUnchecked++;
                }
                if (delootChecked == LootList.SelectedItems.Count)
                {
                    MnuEditMarkDeloot.IsChecked = true;
                }
                else if (delootUnchecked == LootList.SelectedItems.Count)
                {
                    MnuEditMarkDeloot.IsChecked = false;
                    MnuEditMarkDeloot.Icon = null;
                }
                else
                {
                    MnuEditMarkDeloot.IsChecked = false;
                    MnuEditMarkDeloot.Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/DayZLootEdit;component/checkbox_mixed.png")) };
                }
            }
            LootList.Focus();
        }

        // STATUS

        private void UpdateStatus(int filteredClasses = 0, int lootClasses = 0)
        {

            if (filteredClasses == 0)
                filteredClasses = LootTable.Loot.Count;

            if (lootClasses == 0)
                lootClasses = LootTable.Loot.Count;

            lblStatusCount.Content = " Items: " + filteredClasses.ToString() + " / " + lootClasses.ToString();

        }

    }
}
