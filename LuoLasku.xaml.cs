using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
    /// Interaction logic for LuoLasku.xaml
    /// </summary>
    public partial class LuoLasku : Window
    {
        //Luodaan observable collection laskuun liitettäville laskuriveille, jotta niitä on helpompi poistaa ja lisätä halutessaan.

        private ObservableCollection<Laskurivi> rivis = new ObservableCollection<Laskurivi>();

        public LaskuRepo repo = new LaskuRepo();
        public LuoLasku()
        {
     
            InitializeComponent();

            //Määrittää laskurivien comboboxin sisällön
            comTuotteet.ItemsSource = repo.GetTuotteet();

            this.DataContext = new Lasku();

            var lasku = (Lasku)this.DataContext;

            rivis = lasku.Laskurivit;

            YourDataGridName.ItemsSource = rivis;

            GenerateLaskunNumero();


        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Tämä metodi määrittää laskurivillä valitun tuotteen nimen ja hinnan laskuriville.

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

                    // Metodi myös varmistaa, että samaan aikaan kun tuote valitaan päivittyy laskun kokonaishinta

                    lasku.UpdateTotalPrice();
                }
            }

            // Kokonaishinnan päivittyminen varmistetaan kaksi kertaa, koska tapahtui tilanteita missä totalprice ei päivittynyt
            lasku.UpdateTotalPrice();

        }

        private void SaveInvoice(object sender, RoutedEventArgs e)
        {
            // Tämä metodi lisää uuden laskun tietokantaan rivien kera. Metodi myös päivittää juuri luodun laskun, jos käyttäjä sitä vaatii.

            var lasku = (Lasku)this.DataContext;

            DataGrid dataGrid = sender as DataGrid;

            // Luo observable collectionin tietokannan laskuista

            ObservableCollection<Lasku> laskut = repo.GetLaskut();

            Lasku haettuLasku = laskut.FirstOrDefault(l => l.LaskunNumero == lasku.LaskunNumero);

            // Etsii kokoelmasta laskun, joka vastaa tämän laskun numeroa

            if (haettuLasku != null)
            {
                // jos lasku löytyy laskun voi päivittäää

                if (!string.IsNullOrWhiteSpace(lasku.CustomerName))
                {
                    repo.UpdateLasku(lasku);
                    MessageBox.Show("Lasku päivitetty onnistuneesti");

                }
                else
                {
                    MessageBox.Show("Laita Nimi");
                }
                //Laskurivit poistetaan ja lisätään uudestaan, jos lasku on päivitetty. Tämä oli yksinkertaisin tapa päivittää myös laskurivit tietokannassa

                foreach (var rivi in rivis)
                {
                    
                    repo.RemoveLaskurivi(rivi);

                }

                foreach (var rivi in rivis)
                {

                    repo.AddLaskuRivi(rivi);

                }
            }
            else
            {
                //Jos laskua samalla numerolla ei löydy, uusi lasku lisätään lasku rivien kera.
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
            // Tämä metodi päivittää laskun kokonaishinnan realiaikaisesti kirjoittaessa
            var lasku = (Lasku)this.DataContext;

            lasku.UpdateTotalPrice();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Tämä metodi lisää laskurivin observable collectioniin ja määrittää niiden piilotetun ID:n vastaavan laskun numeroa. Tällä tavoin laskurivit liitetään laskuun tietokannassa foreign keyn avulla.
            rivis.Add(new Laskurivi());

            var lasku = (Lasku)this.DataContext;
      
            foreach (Laskurivi rivi in rivis)
            {
                rivi.LaskuID = lasku.LaskunNumero;

            }

        }

        private void RemoveLine(object sender, RoutedEventArgs e)
        {
            // Tämä metodi poistaa laskurivit realiaikaisesti painamalla miinusta riviltä. Päädyin tähän ratkaisuun, koska tuli bugi, jossa laskurivit eivät poistuneet tietokannasta, jos ne poisti kaikki kerralla.
            // Tässä metodissa on myöös miinus puolia, jotka ovat, että rivit on poistettu tietokannasta ennen kuin painaa tallenna, mutta näin päin sovellus toimii parhaiten
            if (YourDataGridName.SelectedItem != null)
            {
                // Poistetaan valittu rivi rivis-listalta
                Laskurivi removedRow = (Laskurivi)YourDataGridName.SelectedItem;
                rivis.Remove(removedRow);

                // Poista rivin tietokannasta
                repo.RemoveLaskurivi(removedRow);

                Debug.WriteLine("Rivien määrä on " + rivis.Count);
            }
        }


        private void GenerateLaskunNumero()
        {
            // Tämä generoi laskun numeron automaattisesti
            Random rand = new Random();

            var lasku = (Lasku)this.DataContext;

            for (int i = 0; i < 8; i++)
            {
                lasku.LaskunNumero = lasku.LaskunNumero * 10 + rand.Next(8);
            }

            LaskunNumeroTextBox.Text = lasku.LaskunNumero.ToString();

        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            // Aikaisemmin olin rakentanut sovelluksen niin, että se loi tietokannan avatessaan, mutta ei luonut uudestaan jos se oli jo luotu

            //MainWindow mainWindow = new MainWindow();

            this.Close(); 

            //var returnValue = mainWindow.ShowDialog();

        }

        private void DatePicker_MouseEnter(object sender, MouseEventArgs e)
        {
            // Tämä muuttaa datepickerin reunat siniseksi, jos hiirtä pitää sen päällä.
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                datePicker.BorderBrush = Brushes.Blue;
            }
        }

        private void DatePicker_MouseLeave(object sender, MouseEventArgs e)
        {
            // Tämä muuttaa datepickerin reunat taksisin, jos hiiren poistaa sen päältä.
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                datePicker.BorderBrush = Brushes.White;
            }
        }
    }
}

