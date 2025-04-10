using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for LuoTuote.xaml
    /// </summary>
    public partial class LuoTuote : Window
    {
        public LaskuRepo repo = new LaskuRepo();
        public LuoTuote()
        {
            InitializeComponent();

            this.DataContext = new Tuote();

            // Määrittää viewTuotteet searchbarin sisällön

            viewTuotteet.ItemsSource = repo.GetTuotteet();
        }
        private void SaveInvoice(object sender, RoutedEventArgs e)
        {
            // Tämä metodi päivittäää valitun tuotteen, jos sen nimi vastaa nimeä, joka on jo tietokannassa tai lisää uuden tuotteen, jos nimeä ei löydy

            var tuote = (Tuote)this.DataContext;

            // Listaa tietokannassa olevat tuotteet observablecollectioniin.

            ObservableCollection<Tuote> tuotteet = repo.GetTuotteet();


            // Määrittää tietyn tuotteen listasta

            Tuote haettutuote = tuotteet.FirstOrDefault(l => l.Nimi == tuote.Nimi);


            if (haettutuote != null)
            {
                // Jos saman niminen tuote löytyy tietokannasta, tuote päivitetään, jos painaa tallenna.
                repo.UpdateTuote(tuote);

                MessageBox.Show("Tuote päivitetty onnistuneesti.");
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(tuote.Nimi))
                {
                    // Jos saman nimistä tuotetta ei löydy tietokannasta lisätään uusi kunhan tuotteen nimeksi ei ole asetettu tyhjä tai null.
                    repo.AddOmaTuote(tuote);
                    MessageBox.Show("Tuote lisätty onnistuneesti.");
                }
                else
                {
                    // Jos tuotteen nimeksi on asetettu tyhjä tai null. Annetaan virheilmoitus
                    MessageBox.Show("Et voi lisätä tuotteita puuttellisilla tiedoilla");
                }
         
            }


        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Tässä määritellään searchtextboxin toiminnot, kuten sen, että hakutulokset rajataan kirjoitettujen merkkien mukaan popup ikkunassa.
            SearchResultsPopup.IsOpen = true;

            string searchText = SearchTextBox.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(searchText))
            {
                // Suodatetaan tuotteet hakutekstin perusteella
                viewTuotteet.ItemsSource = repo.GetTuotteet().Where(t => t.Nimi.ToLower().Contains(searchText));
                SearchResultsPopup.IsOpen = true;
            }
            else
            {
                // Tyhjennetään hakutulokset, jos hakuteksti on tyhjä
                viewTuotteet.ItemsSource = null;
                SearchResultsPopup.IsOpen = false;
            }
        }

        private void viewTuotteet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tuote = (Tuote)this.DataContext;

            // Tässä metodissa määritellään searchbarista tehty valinta aktiiviseksi valinnaksi, jotta seuraavaksi valitun laskun voi päivittää
            // Metodi päivittää nimen ja hinnan tehdyn valinnan mukaan ja sulkee popupin.

            string searchText = SearchTextBox.Text;
            if (viewTuotteet.SelectedItem != null)
            {
                Tuote selectedTuote = (Tuote)viewTuotteet.SelectedItem;
                string selectedTuoteNimi = selectedTuote.Nimi;
                double selectedTuoteHinta = selectedTuote.Price;
                SearchTextBox.Text = selectedTuoteNimi;
                SearchResultsPopup.IsOpen = false;
                NimiTextBox.Text = selectedTuoteNimi;
                tuote.Nimi = selectedTuoteNimi;
                tuote.Price = selectedTuoteHinta;
                PriceTextBox.Text = selectedTuoteHinta.ToString();

            }
            else
            {
                viewTuotteet.ItemsSource = repo.GetTuotteet();
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Tällä metodilla saa piilotettua searchbarin popupin, jos klikkaa mitä tahansa kohtaa kanvaalla.

            if (SearchResultsPopup.IsOpen = true)
            {
                SearchResultsPopup.IsOpen = false;

            }
        }

        private void DeleteTuote(object sender, RoutedEventArgs e)
        {

            // Poistaa valitun tuotteen tietokannasta

            var tuote = (Tuote)this.DataContext;

            ObservableCollection<Tuote> tuotteet = repo.GetTuotteet();

            Tuote haettutuote = tuotteet.FirstOrDefault(l => l.Nimi == tuote.Nimi);

            if (haettutuote != null)
            {
                repo.RemoveTuote(tuote);

                this.Close();

                LuoTuote luoTuote = new LuoTuote();

                var returnValue = luoTuote.ShowDialog();
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            //Sulkee ikkunan
            this.Close();
        }
    }
}
