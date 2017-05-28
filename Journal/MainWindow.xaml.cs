using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.IO;
using System.Globalization;
using System.Threading;

namespace Journal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string DATE = DateTime.Now.ToString("yyyy-MM-dd");

        public MainWindow()
        {
            InitializeComponent();
            this.Title = DATE;

            // Set a fixed date format.
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name); ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = ci;

            datePicker.SelectedDate = DateTime.Today;

            string content = DB.QueryScalar("select content from journal where date ='" + DATE + "'").ToString();
            journalContentBox.Document.Blocks.Clear();
            journalContentBox.AppendText(content);
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var datepicker = sender as DatePicker;
            DateTime? date = datepicker.SelectedDate;
            if (date == null)
            {
                // ... A null object.
                this.Title = "No date";
            }
            else
            {
                // ... No need to display the time.
                this.Title = date.Value.ToShortDateString();
                DATE = this.Title;
            }

            string content = DB.QueryScalar("select content from journal where date ='" + DATE + "'").ToString();
            journalContentBox.Document.Blocks.Clear();
            if(content != "-1")
            {
                journalContentBox.AppendText(content);
            }
        }

        private void journalContentBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                string content = new TextRange(journalContentBox.Document.ContentStart, journalContentBox.Document.ContentEnd).Text;
                DB.Query("REPLACE INTO `journal` (date, content) values ('" + DATE + "', '" + content + "')");
            }
            if (e.Key == Key.Enter)
            {
                var newPointer = journalContentBox.Selection.Start.InsertLineBreak();
                journalContentBox.Selection.Select(newPointer, newPointer);
                e.Handled = true;
            }
        }

        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            datePicker.SelectedDate = DateTime.Today;
        }

        private void btnPreviousDay_Click(object sender, RoutedEventArgs e)
        {
            datePicker.SelectedDate = datePicker.SelectedDate.Value.AddDays(-1);
        }

        private void btnNextDay_Click(object sender, RoutedEventArgs e)
        {
            datePicker.SelectedDate = datePicker.SelectedDate.Value.AddDays(1);
        }
    }
}
