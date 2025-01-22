using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace LaskuApp
{
    /// <summary>
    /// Interaction logic for UpdateLasku.xaml
    /// </summary>
    public partial class UpdateLasku : Window
    {
        // Suurin osa tämän näkymän koodista on samaa kuin LuoLasku-näkymässä. Siksi en näe tarvetta kommentoida sitä tarkasti, koska se on kommentoitu jo LuoLasku-näkymässä. Ainoana erotuksena on LoadLaskuFromDataBase-metodi
        private ObservableCollection<Laskurivi> rivis = new ObservableCollection<Laskurivi>();
        private Lasku valittuLasku;
        private LaskuRepo repo = new LaskuRepo();

        public UpdateLasku(Lasku lasku)
        {
            InitializeComponent();
            valittuLasku = lasku;

            // Asettaa DataContextin laskulle, jotta jatkossa voi määrittää sen tiedot valitun laskun perusteella.
            this.DataContext = valittuLasku;

            comTuotteet.ItemsSource = repo.GetTuotteet();
            rivis = valittuLasku.Laskurivit;
            YourDataGridName.ItemsSource = rivis;

            LoadLaskuFromDatabase();
        }

        private void LoadLaskuFromDatabase()
        {
            ObservableCollection<Lasku> laskut = repo.GetLaskut();
            // Haetaan valitun laskun tiedot tietokannasta
            Lasku haettuLasku = laskut.FirstOrDefault(l => l.LaskunNumero == valittuLasku.LaskunNumero);

            if (haettuLasku != null)
            {
                // Päivitä laskun tiedot 
                valittuLasku.CustomerName = haettuLasku.CustomerName;
                valittuLasku.Address = haettuLasku.Address;
                valittuLasku.PostalCode = haettuLasku.PostalCode;
                valittuLasku.datetime = haettuLasku.datetime;
                valittuLasku.Duetime = haettuLasku.Duetime;
                valittuLasku.AdditionalInfo = haettuLasku.AdditionalInfo;
                valittuLasku.Salary = haettuLasku.Salary;
                valittuLasku.Work = haettuLasku.Work;
                valittuLasku.TotalPrice = haettuLasku.TotalPrice;

                // Päivittää textboxien komponentit vastaamaan valittun laskun tietoja
                LaskuNumeroTextBox.Text = haettuLasku.LaskunNumero.ToString();
                CustomerNameTextBox.Text = haettuLasku.CustomerName;
                OsoiteTextBox.Text = haettuLasku.Address;
                PostalCodeTextBox.Text = haettuLasku.PostalCode;
                DateTimeTextBox.Text = haettuLasku.datetime.ToString();
                DueTimeTextBox.Text = haettuLasku.Duetime.ToString();
                AdditionalInfoTextBox.Text = haettuLasku.AdditionalInfo;
                //SalaryTextBox.Text = haettuLasku.Salary.ToString("F1");
                //WorkTextBox.Text = haettuLasku.Work.ToString("F1");
                TotalPriceTextBox.Text = haettuLasku.TotalPrice.ToString("F1");
            }
            else
            {
                MessageBox.Show("Laskun tiedot eivät löydy tietokannasta.");
            }

            // Päivitä laskurivit haetun laskun numeron perusteella. Laskurivin foreign key tietokannassa on liitetty laskun numeroon.
            ObservableCollection<Laskurivi> laskurivit = new ObservableCollection<Laskurivi>(
                repo.GetLaskurivit().Where(rivi => rivi.LaskuID == valittuLasku.LaskunNumero)
            );

            foreach (var rivi in laskurivit)
            {
                rivis.Add(rivi);
            }

            // Päivitä laskun kokonaishinta
            valittuLasku.UpdateTotalPrice();
        }




        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dataGrid = sender as DataGrid;
            var lasku = (Lasku)this.DataContext;


            var selectedRow = dataGrid.SelectedItem as Laskurivi;

            if (selectedRow != null)
            {
                var comboBoxColumn = YourDataGridName.Columns[1] as DataGridComboBoxColumn;
                var comboBox = comboBoxColumn.GetCellContent(selectedRow) as ComboBox;

                if (comboBox != null && comboBox.SelectedItem is Tuote selectedTuote)
                {
                    selectedRow.Name = selectedTuote.Nimi;
                    selectedRow.Hinta = selectedTuote.Price;

                    lasku.UpdateTotalPrice();
                }
            }

            lasku.UpdateTotalPrice();

        }

        private void SaveInvoice(object sender, RoutedEventArgs e)
        {

            var lasku = (Lasku)this.DataContext;

            DataGrid dataGrid = sender as DataGrid;

            ObservableCollection<Lasku> laskut = repo.GetLaskut();

            Lasku haettuLasku = laskut.FirstOrDefault(l => l.LaskunNumero == lasku.LaskunNumero);

            // Etsi valittu lasku laskukokoelmasta

            if (haettuLasku != null)
            {
                if (!string.IsNullOrWhiteSpace(lasku.CustomerName))
                {
                    repo.UpdateLasku(lasku);
                    MessageBox.Show("Lasku päivitetty onnistuneesti");

                }
                else
                {
                    MessageBox.Show("Laita Nimi");
                }

                foreach (Laskurivi item in YourDataGridName.Items)
                {
                    repo.RemoveLaskurivi(item);
                }

                foreach (var rivi in rivis)
                {
                    repo.AddLaskuRivi(rivi);
                }

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(lasku.CustomerName))
                {
                    repo.AddLasku(lasku);
                    MessageBox.Show("Lasku lisätty onnistuneesti");

                }
                else
                {
                    MessageBox.Show("Laita Nimi");
                }

                foreach (Laskurivi rivi in rivis)
                {

                    repo.AddLaskuRivi(rivi);


                }

            }
        }



        private void YourDataGridName_PreviewKeyDown(object sender, KeyEventArgs e)
        {


            var lasku = (Lasku)this.DataContext;

            lasku.UpdateTotalPrice();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            rivis.Add(new Laskurivi());

            Debug.WriteLine("Rivien määrä on " + rivis.Count);

            var lasku = (Lasku)this.DataContext;

            foreach (Laskurivi rivi in rivis)
            {
                rivi.LaskuID = lasku.LaskunNumero;
            }

        }

        private void RemoveLine(object sender, RoutedEventArgs e)
        {

            if (YourDataGridName.SelectedItem != null)
            {
                Laskurivi removedRow = (Laskurivi)YourDataGridName.SelectedItem;
                rivis.Remove(removedRow);

                repo.RemoveLaskurivi(removedRow);

                Debug.WriteLine("Rivien määrä on " + rivis.Count);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            //MainWindow mainWindow = new MainWindow();

            this.Close();

            //var returnValue = mainWindow.ShowDialog();

        }

        private void DatePicker_MouseEnter(object sender, MouseEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                datePicker.BorderBrush = Brushes.Blue;
            }
        }

        private void DatePicker_MouseLeave(object sender, MouseEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                datePicker.BorderBrush = Brushes.White;
            }
        }
    }
}
