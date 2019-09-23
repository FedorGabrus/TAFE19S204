// **************************************************************************
//Start Finance - An to manage your personal finances.
//Copyright(C) 2016  Jijo Bose

//Start Finance is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//Start Finance is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Start Finance.If not, see<http://www.gnu.org/licenses/>.
// ***************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactDetailsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ContactDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<Contacts>();

            /// Refresh Data
            var query = conn.Table<Contacts>();
            ContactListView.ItemsSource = query.ToList();
        }

        private async void AddContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ContactNameText.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No Contact Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new Contacts()
                    {
                        ContactName = ContactNameText.Text,
                        ContactEmail = ContactEmailText.Text,
                        ContactPhone = ContactPhoneText.Text
                    });
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Value or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("A Similar Contact Name already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            ContactNameText.Text = string.Empty;
            ContactEmailText.Text = string.Empty;
            ContactPhoneText.Text = string.Empty;

            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((Contacts)ContactListView.SelectedItem).ContactName;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Contacts>();
                    var query1 = conn.Table<Contacts>();
                    var query3 = conn.Query<Contacts>("DELETE FROM Contacts WHERE ContactName ='" + AccSelection + "'");
                    ContactListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private async void EditContact_Click(object sender, RoutedEventArgs e)
        {
            if (ContactListView.SelectedItem == null)
            {
                await new MessageDialog("Not the selected Item", "Oops..!").ShowAsync();
                return;
            }
            ContactListView.IsEnabled = false;
            AddContact.IsEnabled = false;
            DeleteContact.IsEnabled = false;
            Contacts editData = (Contacts)ContactListView.SelectedItem;
            ContactNameText.Text = editData.ContactName;
            ContactEmailText.Text = editData.ContactEmail;
            ContactPhoneText.Text = editData.ContactPhone;
            ButtonsStackPanel.Visibility = Visibility.Visible;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Contacts updatedData = (Contacts)ContactListView.SelectedItem;
            updatedData.ContactName = ContactNameText.Text;
            updatedData.ContactEmail = ContactEmailText.Text;
            updatedData.ContactPhone = ContactPhoneText.Text;
            conn.Update(updatedData);
            Results();
            ContactListView.SelectedItem = null;
            ContactListView.IsEnabled = true;
            AddContact.IsEnabled = true;
            DeleteContact.IsEnabled = true;
            ButtonsStackPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ContactListView.SelectedItem = null;
            ContactListView.IsEnabled = true;
            AddContact.IsEnabled = true;
            DeleteContact.IsEnabled = true;
            ButtonsStackPanel.Visibility = Visibility.Collapsed;
        }
    }
}
