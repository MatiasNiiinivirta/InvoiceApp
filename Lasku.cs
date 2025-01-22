using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LaskuApp
{
    public class Lasku : INotifyPropertyChanged
    {

        public ObservableCollection<Laskurivi> Laskurivit { get; set; }
        public int LaskunNumero { get; set; } // Laskun numero
     
        public string Address { get; set; } //Laskuttajan osoite

        private string customername; // Asiakkaan nimi
        public string CustomerName
        {
            get { return customername; }
            set 
            { 
                
                
                customername = value;
                OnPropertyChanged(nameof(CustomerName));
            
            
            }


        }

        public string PostalCode { get; set; }
        public DateTime datetime { get; set; } //Laskun päiväys

        public DateTime Duetime { get; set; } //Laskun eräpäivä
        public string AdditionalInfo { get; set; } //Lisätiedot

        private double totalprice;
        public double TotalPrice
        {
            get { return totalprice; }
            set
            {
                if (totalprice != value)
                {
                    totalprice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                    UpdateTotalPrice();

                }
            }
        }

        private double work;
        public double Work
        {
            get { return work; }
            set
            {
                if (work != value)
                {
                    work = value;
                    OnPropertyChanged(nameof(Work));
                    UpdateTotalPrice();
                }
            }
        }

        private double salary;
        public double Salary
        {
            get { return salary; }
            set
            {
                if (salary != value)
                {
                    salary = value;
                    OnPropertyChanged(nameof(Salary));
                    UpdateTotalPrice();
                }
            }
        }

        public void UpdateTotalPrice()
        {
            double totalPrice = 0;

            foreach (Laskurivi rivi in Laskurivit)
            {
                totalPrice += rivi.KokonaisHinta; 
            }

            TotalPrice = totalPrice + Work * Salary;
            Debug.WriteLine("Laskun Loppusumma on " + TotalPrice);

            OnPropertyChanged(nameof(TotalPrice));


        }

        // OnPropertyChanged metodia tarvitaan päivittämään tiettyjä arvoja realiajassa toisten arvojen perusteella. Esimerkiksi totalpricen päivittäminen

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Lasku()
        {


            Address = string.Empty;
            CustomerName = string.Empty;
            PostalCode = string.Empty;

            this.datetime = DateTime.Now;
            Duetime = DateTime.Now;
            AdditionalInfo = string.Empty;
            Laskurivit = new ObservableCollection<Laskurivi>();


        }




    }


    public class Laskurivi : INotifyPropertyChanged
        {
           
        public int RiviID { get; set; }

        private int laskuid;

        public int LaskuID
        {
            get { return laskuid; }
            set
            {
                if (laskuid != value)
                {
                    laskuid = value;
                    OnPropertyChanged(nameof(LaskuID));
          
                }
            }
        }

        public Tuote Product { get; set; }

        public ObservableCollection<Tuote> Tuotteet { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (value != name) 
                {
                    name = value;
                    OnPropertyChanged(nameof(name));

                }                       
            }
        }



    //Kokonaishinta ominaisuuten tallennetaan tuotteiden määrän ja hinnan tulo tai työn tuntihinta kertaa tehdyt tunnit
    private double hinta;
        private double amount;
        private double kokonaisHinta;

        public double Hinta
        {
            get { return hinta; }
            set
            {
                if (value != hinta)
                {
                    hinta = value;
                    OnPropertyChanged(nameof(Hinta));
                    UpdateKokonaisHinta();
                }
            }
        }

        public double Amount
        {
            get { return amount; }
            set
            {
                if (value != amount)
                {
                    amount = value;
                    OnPropertyChanged(nameof(Amount));
                    UpdateKokonaisHinta();
                }
            }
        }

        public double KokonaisHinta
        {
            get { return kokonaisHinta; }
            set
            {
                if (value != kokonaisHinta)
                {
                    kokonaisHinta = value;
                    OnPropertyChanged(nameof(KokonaisHinta));
                    UpdateKokonaisHinta();
                }
            }
        }

        public void UpdateKokonaisHinta()
        {
            KokonaisHinta = Hinta * Amount;
            OnPropertyChanged(nameof(KokonaisHinta));
        }

        public void UpdateLaskuID()
        {
            Lasku lasku = new Lasku();
            LaskuID = lasku.LaskunNumero;
            OnPropertyChanged(nameof(LaskuID));
        }


        // OnPropertyChanged metodia tarvitaan päivittämään tiettyjä arvoja realiajassa toisten arvojen perusteella

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }

   
}
