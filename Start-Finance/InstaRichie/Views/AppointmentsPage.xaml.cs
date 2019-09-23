using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using SQLite.Net;
using StartFinance.Models;
using System.Linq;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentsPage : Page {

        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentsPage(){
            
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Results();

        }

        public void Results(){
            conn.CreateTable<Models.Appointment>();
            var query1 = conn.Table<Models.Appointment>();
            //AppointmentsListView.ItemsSource = query1.ToList();
        }

        private async void AddAppointment_Click(object sender, RoutedEventArgs e){

            try{

                if (PersonName.Text.ToString() == ""){

                    MessageDialog dialog = new MessageDialog("No Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }

                else {

                    string CDay = ApptDate.Date.Value.Day.ToString();
                    string CMonth = ApptDate.Date.Value.Month.ToString();
                    string CYear = ApptDate.Date.Value.Year.ToString();
                    string FinalDate = CMonth + "/" + CDay + "/" + CYear;
                    string FinalTime = ApptTime.Time.ToString();
                   
                    conn.CreateTable<Models.Appointment>();
                    conn.Insert(new Appointment {

                        Name = PersonName.Text.ToString(),
                        DateOfAppt = FinalDate,
                        TimeOfAppt = FinalTime
                    });
                    
                    // Creating table
                    Results();

                    MessageDialog dialog = new MessageDialog("Appointment Added", "Sweet!");
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex) {

                if (ex is FormatException){

                    MessageDialog dialog = new MessageDialog("You forgot to enter the Name, Date or Time", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException){

                    MessageDialog dialog = new MessageDialog("Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }

                else{
                    
                    /// no idea
                }
            }
        }


        private async void UpdateAppointment_Click(object sender, RoutedEventArgs e){

            try{

                if (AppointmentsSelect.SelectedItem == null || UpdateApptDate.Date == null || UpdateApptTime.Time == null){

                    MessageDialog dialog1 = new MessageDialog("You forgot to Enter all required information", "Oops..!");
                    await dialog1.ShowAsync();

                }

                else{

                    string name = ((Appointment)AppointmentsSelect.SelectedItem).Name;
                    string CDay = UpdateApptDate.Date.Value.Day.ToString();
                    string CMonth = UpdateApptDate.Date.Value.Month.ToString();
                    string CYear = UpdateApptDate.Date.Value.Year.ToString();
                    string FinalDate = CMonth + "/" + CDay + "/" + CYear;
                    string FinalTime = UpdateApptTime.Time.ToString();

                    var query3 = conn.Query<Appointment>(query: "UPDATE Appointment SET DateOfAppt = '" + FinalDate + "', TimeOfAppt = '" + FinalTime + "' WHERE Name ='" + name + "'");

                    MessageDialog dialog = new MessageDialog("Appointment Updated for " + name + " ", "Sweet!");
                    await dialog.ShowAsync();

                }
                
            }
            catch (Exception ex) {

                if (ex is FormatException){

                    MessageDialog dialog = new MessageDialog("You forgot to enter the Date or Time", "Oops..!");
                    await dialog.ShowAsync();

                }
               
                else{

                    /// no idea
                }
            }
        }




        private async void DeleteAppointment_Click(object sender, RoutedEventArgs e){

            try {

                string ApptSelection = ((Appointment)AppointmentsList1.SelectedItem).Name;

                if (ApptSelection == "") {

                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }

                else{

                    conn.CreateTable<Appointment>();
                    var query1 = conn.Table<Appointment>();
                    var query3 = conn.Query<Appointment>("DELETE FROM Appointment WHERE Name ='" + ApptSelection + "'");
                    AppointmentsList1.ItemsSource = query1.ToList();
                }
            }

            catch (NullReferenceException){
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }


        private void AppointmentsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            int no = AppointmentsPivot.SelectedIndex;

            if (no == 0)
            {
                AddAppointmentsFooter.Visibility = Visibility.Visible;
                CancelAppointmentsFooter.Visibility = Visibility.Collapsed;
                UpdateAppointmentsFooter.Visibility = Visibility.Collapsed;
                conn.CreateTable<Appointment>();
                var query = conn.Table<Appointment>();
                
            }

            else if (no == 1) {

                CancelAppointmentsFooter.Visibility = Visibility.Visible;
                AddAppointmentsFooter.Visibility = Visibility.Collapsed;
                UpdateAppointmentsFooter.Visibility = Visibility.Collapsed;
                conn.CreateTable<Appointment>();
                var query = conn.Table<Appointment>();
                AppointmentsList1.ItemsSource = query.ToList();

            }


            else{

                UpdateAppointmentsFooter.Visibility = Visibility.Visible;
                CancelAppointmentsFooter.Visibility = Visibility.Collapsed;
                AddAppointmentsFooter.Visibility = Visibility.Collapsed;
                conn.CreateTable<Appointment>();
                var query = conn.Table<Appointment>();
                AppointmentsSelect.ItemsSource = query.ToList();
            }
        }
    }
}
