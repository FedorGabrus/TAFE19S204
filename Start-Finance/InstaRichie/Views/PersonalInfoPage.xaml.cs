using SQLite.Net;
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
using StartFinance.Models;
using Windows.UI;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    public sealed partial class PersonalInfoPage : Page
    {
        private SQLiteConnection connection;

        public PersonalInfoPage()
        {
            this.InitializeComponent();

            // Initialize db connection.
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
            connection = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Try to create new table.
            connection.CreateTable<PersonalInfo>();

            PopulateUIandDB();

            // Setup date picker.
            DobDatePicker.MaxYear = DateTime.Now;
        }

        private void AddPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
            {
                return;
            }
            connection.Insert(new PersonalInfo
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                DOB = DobDatePicker.Date.Date,
                Gender = GenderComboBox.SelectedValue.ToString(),
                EmailAddress = EmailTextBox.Text,
                MobilePhone = PhoneTextBox.Text
            });
            PopulateUIandDB();
            ClearForm();
        }

        private async void EditPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            // Clear form.
            ClearForm();
            // Check selection.
            if (PersonalInfoListView.SelectedItem == null)
            {
                await new MessageDialog("Not selected the Item", "Oops..!").ShowAsync();
                return;
            }
            // Freeze ListView. Need to preserve selection state to keep track of selected object.
            PersonalInfoListView.IsEnabled = false;
            // Disable add new and delete buttons to prevent unexpected behaviour.
            AddPersonalInfo.IsEnabled = false;
            DeleteItem.IsEnabled = false;
            // Populate form with data from selection.
            PersonalInfo editData = (PersonalInfo) PersonalInfoListView.SelectedItem;
            FirstNameTextBox.Text = editData.FirstName;
            LastNameTextBox.Text = editData.LastName;
            DobDatePicker.SelectedDate = editData.DOB;
            GenderComboBox.SelectedItem = editData.Gender;
            EmailTextBox.Text = editData.EmailAddress;
            PhoneTextBox.Text = editData.MobilePhone;
            // Show edit buttons.
            ButtonsStackPanel.Visibility = Visibility.Visible;
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            // Clear form.
            ClearForm();
            // Check selection.
            if (PersonalInfoListView.SelectedItem == null)
            {
                await new MessageDialog("Not selected the Item", "Oops..!").ShowAsync();
                return;
            }
            PersonalInfo objectToDelete = (PersonalInfo)PersonalInfoListView.SelectedItem;
            connection.Delete(objectToDelete);
            PopulateUIandDB();
            ClearForm();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FirstNameTextBox.Focus(FocusState.Programmatic);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Check form fields.
            if (!CheckFields())
            {
                return;
            }
            // Update object.
            PersonalInfo updatedData = (PersonalInfo)PersonalInfoListView.SelectedItem;
            updatedData.FirstName = FirstNameTextBox.Text;
            updatedData.LastName = LastNameTextBox.Text;
            updatedData.DOB = DobDatePicker.Date.Date;
            updatedData.Gender = GenderComboBox.SelectedValue.ToString();
            updatedData.EmailAddress = EmailTextBox.Text;
            updatedData.MobilePhone = PhoneTextBox.Text;
            // Update DB.
            connection.Update(updatedData);
            // Update UI.
            PopulateUIandDB();
            // Free ListView and buttons.
            PersonalInfoListView.SelectedItem = null;
            PersonalInfoListView.IsEnabled = true;
            AddPersonalInfo.IsEnabled = true;
            DeleteItem.IsEnabled = true;
            ClearForm();
        }

        // Checks imput fields.
        private bool CheckFields()
        {
            // First name.
            if (String.IsNullOrEmpty(FirstNameTextBox.Text)) {
                ErrorFirstNameTextBlock.Visibility = Visibility.Visible;
                FirstNameTextBox.Focus(FocusState.Programmatic);
                return false;
            }
            else
            {
                ErrorFirstNameTextBlock.Visibility = Visibility.Collapsed;
            }

            // Last name.
            if (String.IsNullOrEmpty(LastNameTextBox.Text))
            {
                ErrorLastNameTextBlock.Visibility = Visibility.Visible;
                LastNameTextBox.Focus(FocusState.Programmatic);
                return false;
            }
            else
            {
                ErrorLastNameTextBlock.Visibility = Visibility.Collapsed;
            }

            // DOB.
            if (DobDatePicker.SelectedDate == null)
            {
                ErrorDobDateTextBlock.Visibility = Visibility.Visible;
                DobDatePicker.Focus(FocusState.Programmatic);
                return false;
            }
            else
            {
                ErrorDobDateTextBlock.Visibility = Visibility.Collapsed;
            }

            // Gender.
            if (GenderComboBox.SelectedItem == null)
            {
                ErrorGenderTextBlock.Visibility = Visibility.Visible;
                GenderComboBox.Focus(FocusState.Programmatic);
                return false;
            }
            else
            {
                ErrorGenderTextBlock.Visibility = Visibility.Collapsed;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            PersonalInfoListView.SelectedItem = null;
            PersonalInfoListView.IsEnabled = true;
            AddPersonalInfo.IsEnabled = true;
            DeleteItem.IsEnabled = true;
            ClearForm();
        }

        private void PopulateUIandDB()
        {
            var data = connection.Table<PersonalInfo>();
            if (data.Count() > 0)
            {

                PersonalInfoListHeader.Visibility = Visibility.Visible;
                PersonalInfoListView.Visibility = Visibility.Visible;
                PersonalInfoListScrollViewer.BorderBrush = new SolidColorBrush(Colors.Gray);
                PersonalInfoListScrollViewer.BorderThickness = new Thickness(0, 1, 0, 0);
                PersonalInfoListView.ItemsSource = data.ToList<PersonalInfo>();
            }
            else
            {
                PersonalInfoListHeader.Visibility = Visibility.Collapsed;
                PersonalInfoListView.Visibility = Visibility.Collapsed;
                PersonalInfoListScrollViewer.BorderBrush = null;
            }
        }

        private void ClearForm()
        {
            // Clear input fields.
            FirstNameTextBox.Text = String.Empty;
            LastNameTextBox.Text = String.Empty;
            DobDatePicker.SelectedDate = null;
            GenderComboBox.SelectedItem = null;
            EmailTextBox.Text = String.Empty;
            PhoneTextBox.Text = String.Empty;
            // Clear error fields.
            ErrorFirstNameTextBlock.Visibility = Visibility.Collapsed;
            ErrorLastNameTextBlock.Visibility = Visibility.Collapsed;
            ErrorDobDateTextBlock.Visibility = Visibility.Collapsed;
            ErrorGenderTextBlock.Visibility = Visibility.Collapsed;
            // Clear edit buttons.
            ButtonsStackPanel.Visibility = Visibility.Collapsed;
            // Set focus to the first input.
            FirstNameTextBox.Focus(FocusState.Programmatic);
        }
    }
}
