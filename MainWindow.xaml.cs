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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaskuApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LaskuRepo repo = new LaskuRepo();

        public MainWindow()
        {
            InitializeComponent();

            // Tässä luodaaan tietokanta ja siihen tarvittavat taulut
            repo.CreateLaskuDb();
            repo.CreateLaskuTable();
            repo.CreateTuoteTable();
            repo.CreateLaskuRiviTable();
            repo.AddTuotteet();
            repo.AddDefaultLaskut();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // Tässä viitataan nappiin, jota kautta voi avata LuoLasku näkymän

            LuoLasku addNew = new LuoLasku();

            // Nappi sulkee myös searchbarin popupin, jotta se ei jää päälle

            if (SearchResultsPopup.IsOpen = true)
            {
                SearchResultsPopup.IsOpen = false;
            }

            var returnValue = addNew.ShowDialog();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Tässä viitataan nappiin, jota kautta voi avata LuoTuotenäkymän

            LuoTuote addNew = new LuoTuote();

            // Nappi sulkee myös searchbarin popupin, jotta se ei jää päälle

            if (SearchResultsPopup.IsOpen = true)
            {
                SearchResultsPopup.IsOpen = false;
            }

            var returnValue = addNew.ShowDialog();
        }


        private void SearchTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Tämä metodi määrittää searchboxissa tulleen popupin sisällön ja sen mikä tietojen perusteella tietoja voi hakea.

            SearchResultsPopup.IsOpen = true;

            string searchText = SearchTextBox.Text;
            if (!string.IsNullOrWhiteSpace(searchText))
            {

                // Luo ensin tyhjän listan hakutuloksille
                List<Lasku> searchResults = new List<Lasku>();

                // Etsi laskunumeron perusteella
                var laskuByNumber = repo.GetLaskut().Where(t => t.LaskunNumero.ToString().Contains(searchText));
                searchResults.AddRange(laskuByNumber);

                // Etsi asiakkaan nimen perusteella
                var laskuByCustomerName = repo.GetLaskut().Where(t => t.CustomerName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);
                searchResults.AddRange(laskuByCustomerName);

                // Aseta hakutulokset ListViewn ItemsSourceen
                viewLaskut.ItemsSource = searchResults;
            }
            else
            {
                viewLaskut.ItemsSource = repo.GetLaskut();
            }
        }

        private void viewLaskut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Tässä metodissa määritellään searchbarista tehty valinta aktiiviseksi valinnaksi, jotta seuraavaksi valitun laskun voi päivittää

            if (viewLaskut.SelectedItem != null)
            {
                Lasku selectedLasku = (Lasku)viewLaskut.SelectedItem;
                string selectedLaskuNimi = selectedLasku.CustomerName;
                string selectedLaskunNumero = selectedLasku.LaskunNumero.ToString();

                if (string.IsNullOrEmpty(selectedLaskuNimi)) // Tarkista, onko nimi tyhjä tai null
                {
                    SearchTextBox.Text = selectedLaskunNumero;
                }
                else
                {
                    SearchTextBox.Text = selectedLaskuNimi;
                }

                SearchResultsPopup.IsOpen = false;
            }
        }
        private void UpdateInvoiceButton(object sender, RoutedEventArgs e)
        {
            // Tämä metodi avaa UpdateLasku-näkymän haetun valinnan perusteella


            string searchText = SearchTextBox.Text;
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                // Hae laskut, jotka vastaavat hakutulosta
                var matchingInvoices = repo.GetLaskut()
                    .Where(t => t.LaskunNumero.ToString().Contains(searchText) || t.CustomerName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();

                if (matchingInvoices.Any())
                {
                    // Jos löytyi vastaavia laskuja, valitaan ensimmäinen ja päivitetään SearchTextBox
                    var selectedInvoice = matchingInvoices.First();
                    SearchTextBox.Text = string.Empty;
                    SearchResultsPopup.IsOpen = false;
                    Debug.WriteLine(selectedInvoice.CustomerName);

                    // Avataan update-lomake valitulle laskulle
                    UpdateLasku updateLasku = new UpdateLasku(selectedInvoice);
                    var returnValue = updateLasku.ShowDialog();

                    if (returnValue == true)
                    {
                        // Päivitämme viewLaskut näyttämään uudet laskut
                        viewLaskut.ItemsSource = repo.GetLaskut();
                    }
                }
                else
                {
                    // Jos vastaavia laskuja ei löytynyt, ilmoitetaan käyttäjälle
                    MessageBox.Show("Laskuja ei löytynyt.");
                }
            }
            else
            {
                // Jos SearchTextBox on tyhjä, ilmoitetaan käyttäjälle
                MessageBox.Show("Ole hyvä ja syötä teksti.");
            }


        }

        private void RemoveInvoiceButton(object sender, RoutedEventArgs e)
        {
            // Tämä metodi poistaa laskun tietokannasta haetun valinnan perusteella

            if (SearchResultsPopup.IsOpen = true)
            {
                SearchResultsPopup.IsOpen = false;
            }

            string searchText = SearchTextBox.Text;
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                // Hae laskut, jotka vastaavat hakutulosta
                var matchingInvoices = repo.GetLaskut()
                    .Where(t => t.LaskunNumero.ToString().Contains(searchText) || t.CustomerName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();

                if (matchingInvoices.Any())
                {
                    // Jos löytyi vastaavia laskuja, valitaan ensimmäinen ja päivitetään SearchTextBox tyhjäksi ja laitetaan popup kiinni
                    var selectedInvoice = matchingInvoices.First();

                    SearchTextBox.Text = string.Empty;
                    SearchResultsPopup.IsOpen = false;

                    // Jos lasku on poistettu, ilmoitetaan käyttäjälle
                    MessageBox.Show("Lasku poistettiin onnistuneesti.");
                    repo.RemoveLasku(selectedInvoice);

                }
                else
                {
                    // Jos vastaavia laskuja ei löytynyt, ilmoitetaan käyttäjälle
                    MessageBox.Show("No matching invoices found.");
                }
            }
            else
            {
                // Jos SearchTextBox on tyhjä, ilmoitetaan käyttäjälle
                MessageBox.Show("Please enter search text.");
            }
        }


        private void ExitButton(object sender, RoutedEventArgs e)
        {
            // Poistutaan sovelluksesta
            this.Close();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            // tämä muutta "Poistu" näppäimen punaiseksi, jos hiiri on sen päällä
            Button button = sender as Button;

            if (button != null)
            {
                button.Foreground = Brushes.Red;
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            // tämä muutta "Poistu" näppäimen takaisin mustaksi, jos hiiri poistuu sen päältä
            Button button = sender as Button;

            if (button != null)
            {
                button.Foreground = Brushes.Black;
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Tällä metodilla saa piilotettua searchbarin popupin, jos klikkaa mitä tahansa kohtaa kanvaalla.
            SearchResultsPopup.IsOpen = false;
        }
    }
}
