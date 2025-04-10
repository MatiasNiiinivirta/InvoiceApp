using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaskuApp
{
    public class Tuote : INotifyPropertyChanged
    {
        public int ID { get; set; } // Tuotteen ID

        private string name;

        public string Nimi //Tuotteen nimi
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Nimi));

                }

            }
        }

        private double price;
        public double Price // Tuotteen hinta
        {
            get { return price; }
            set
            {
                if (price != value)
                {
                    price = value;
                    OnPropertyChanged(nameof(Price));
                }

            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged metodia tarvitaan päivittämään tiettyjä arvoja realiajassa toisten arvojen perusteella.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
