using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaskuApp
{
    public class LaskuRepo
    {

        private const string local = "Server=xxx.x.x.x; Port=xxxx; User ID=xxxxxxx; Pwd=xxxxxx;";

        private const string localWithDb = "Server=xxx.x.x.x; Port=xxxx; User ID=xxxxxxxxx; Pwd=xxxxxxxx; Database=xxxxxx;";
        public void CreateLaskuDb()
        {
            // Luo tietokannan
            using (MySqlConnection conn = new MySqlConnection(local))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("DROP DATABASE IF EXISTS LaskuDb", conn);

                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS LaskuDb", conn);

                cmd.ExecuteNonQuery();


            }
        }

        public void CreateLaskuTable()
        {
            // Luo lasku-taulun
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS Lasku (Numero INT NOT NULL PRIMARY KEY,Nimi VARCHAR(50),Osoite VARCHAR(50),PostiNumero VARCHAR(20),Paivays DATE, EraPaiva DATE,LisaTiedot VARCHAR(100), TyoAika_h DOUBLE, Palkka_h DOUBLE, LopullinenHinta DOUBLE)", conn);
                cmd.ExecuteNonQuery();


            }


        }

        public void CreateTuoteTable()
        {
            // Luo tuote-taulun
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS Tuote (ID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,Nimi VARCHAR(50),Price DOUBLE)", conn);
                cmd.ExecuteNonQuery();


            }


        }

        public void CreateLaskuRiviTable()
        {
            // Luo laskurivi_taulun
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("CREATE TABLE IF NOT EXISTS LaskuRivi (RiviID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,Laskun_Numero INT(20),Tuote VARCHAR(50),Hinta DOUBLE,Maara DOUBLE,KokonaisHinta DOUBLE,FOREIGN KEY (Laskun_Numero) REFERENCES lasku(Numero) ON DELETE CASCADE)", conn);
                cmd.ExecuteNonQuery();


            }


        }

        public void AddLaskuRivi(Laskurivi laskurivi)
        {
            // luo uuden tietueen laskurivi tauluun
            // AddLaskuRivi-metodia käytetään myös laskurivien muokkaamiseen ja päivittämiseen yhdessä RemoveLaskurivi-metodin kanssa. Koska laskurivejä voi olla useita yhdellä laskulla. Päivitäessä laskua laaskurivejä voi poistua tai tulla uusia lisää, joten on parempi poistaa kaikki vanhat laskurivit koskien laskua ja listätä uudet.
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {

                conn.Open();

                MySqlCommand cmd = new MySqlCommand("INSERT INTO LaskuRivi(RiviID, Laskun_Numero, Tuote, Hinta, Maara, KokonaisHinta) VALUES(@RiviID,@Laskun_Numero,@Tuote,@Hinta,@Maara,@KokonaisHinta)", conn);
                cmd.Parameters.AddWithValue("@RiviID", laskurivi.RiviID);
                cmd.Parameters.AddWithValue("@Laskun_Numero", laskurivi.LaskuID);
                cmd.Parameters.AddWithValue("@Tuote", laskurivi.Name);
                cmd.Parameters.AddWithValue("@Hinta", laskurivi.Hinta);
                cmd.Parameters.AddWithValue("@Maara", laskurivi.Amount);
                cmd.Parameters.AddWithValue("@KokonaisHinta", laskurivi.KokonaisHinta);

                cmd.ExecuteNonQuery();

            }
        }

        public ObservableCollection<Laskurivi> GetLaskurivit()
        {
            // Hakee laskurivit tietokannasta
            var laskurivit = new ObservableCollection<Laskurivi>();

            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                string query = "SELECT * FROM LaskuRivi";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Tämä estää sen, ettei koko ohjelma mene rikki, jos Syöttää Null Arvon tuotteen nimeksi
                    string tuote = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("Tuote")))
                    {
                        tuote = reader.GetString("Tuote");
                    }


                    laskurivit.Add(new Laskurivi
                    {
                        RiviID = reader.GetInt32("RiviID"),
                        LaskuID = reader.GetInt32("Laskun_Numero"),
                        //Name = reader.GetString("Tuote"),
                        Name = tuote,
                        Hinta = reader.GetDouble("Hinta"),
                        Amount = reader.GetDouble("Maara"),
                        KokonaisHinta = reader.GetDouble("KokonaisHinta")


                    });

                }

            }

            return laskurivit;
        }


        public void RemoveLaskurivi(Laskurivi laskurivi)
        {

            // luo uuden tietueen laskurivi tauluun
            // RemoveLaskurivi-metodia käytetään myös laskurivien muokkaamiseen ja päivittämiseen yhdessä AddLaskurivi-metodin kanssa. Koska laskurivejä voi olla useita yhdellä laskulla. Päivitäessä laskua laaskurivejä voi poistua tai tulla uusia lisää, joten on parempi poistaa kaikki vanhat laskurivit koskien laskua ja listätä uudet.

            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("DELETE FROM LaskuRivi WHERE Laskun_Numero=@Laskun_Numero", conn);
                cmd.Parameters.AddWithValue("@RiviID", laskurivi.RiviID);
                cmd.Parameters.AddWithValue("@Laskun_Numero", laskurivi.LaskuID);
                cmd.Parameters.AddWithValue("@Tuote", laskurivi.Name);
                cmd.Parameters.AddWithValue("@Hinta", laskurivi.Hinta);
                cmd.Parameters.AddWithValue("@Maara", laskurivi.Amount);
                cmd.Parameters.AddWithValue("@KokonaisHinta", laskurivi.KokonaisHinta);

                cmd.ExecuteNonQuery();

                Debug.WriteLine("Poistettavan rivin RiviID: " + laskurivi.RiviID);

            }


        }


        public void UpdateLasku(Lasku lasku)
        {
            //Päivittää laskutietuee numeron perusteella
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("UPDATE Lasku SET Nimi=@Nimi, Osoite=@Osoite, PostiNumero=@PostiNumero, Paivays=@Paivays, EraPaiva=@EraPaiva, LisaTiedot=@LisaTiedot, TyoAika_h=@TyoAika_h, Palkka_h=@Palkka_h, LopullinenHinta=@LopullinenHinta WHERE Numero=@Numero", conn);
                cmd.Parameters.AddWithValue("@Numero", lasku.LaskunNumero);

                cmd.Parameters.AddWithValue("@Nimi", lasku.CustomerName);

                cmd.Parameters.AddWithValue("@Osoite", lasku.Address);

                cmd.Parameters.AddWithValue("@PostiNumero", lasku.PostalCode);

                cmd.Parameters.AddWithValue("@Paivays", lasku.datetime);

                cmd.Parameters.AddWithValue("@EraPaiva", lasku.Duetime);

                cmd.Parameters.AddWithValue("@LisaTiedot", lasku.AdditionalInfo);

                cmd.Parameters.AddWithValue("@TyoAika_h", lasku.Work);

                cmd.Parameters.AddWithValue("@Palkka_h", lasku.Salary);

                cmd.Parameters.AddWithValue("@LopullinenHinta", lasku.TotalPrice);

                cmd.ExecuteNonQuery();

            }
        }

        public ObservableCollection<Lasku> GetLaskut()
        {
            // Noutaa laskut tietokannasta
            var laskut = new ObservableCollection<Lasku>();

            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                string query = "SELECT * FROM Lasku";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //int id = reader.GetInt32("ID");
                    //string nimi = reader.GetString("Nimi");
                    //decimal price = reader.GetDecimal("Price");

                    laskut.Add(new Lasku
                    {
                        LaskunNumero = reader.GetInt32("Numero"),
                        CustomerName = reader.GetString("Nimi"),
                        Address = reader.GetString("Osoite"),
                        PostalCode = reader.GetString("PostiNumero"),
                        datetime = reader.GetDateTime("Paivays"),
                        Duetime = reader.GetDateTime("EraPaiva"),
                        AdditionalInfo = reader.GetString("LisaTiedot"),
                        Work = reader.GetDouble("TyoAika_h"),
                        Salary = reader.GetDouble("Palkka_h"),
                        TotalPrice = reader.GetDouble("LopullinenHinta")


                    });

                }

            }

            return laskut;
        }



        public void AddTuotteet()
        {
            // Lisää default tuotteet tietokantaan
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                var tr = conn.BeginTransaction();

                string tuote0 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(1, 'ESISUODATIN', 45.00)";
                string tuote1 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(2, 'SILIKONIPURISTIN', 39.00)";
                string tuote3 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(3, 'KOKONAAMARI', 314.00)";
                string tuote4 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(4, 'HIONTAKIRAHVI', 1400.00)";
                string tuote5 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(5, 'Kovametallilaikka', 100.00)";
                string tuote6 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(6, 'Lattialakka', 0.00)";
                string tuote7 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(7, 'KIRVES', 20.00)";
                string tuote8 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(8, 'KATUPORAKONE', 335.00)";
                string tuote9 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(9, 'ASTALO', 23.00)";
                string tuote10 = "INSERT IGNORE INTO tuote(ID, Nimi, Price) VALUES(10, 'PEKKELE', 15.00)";

                MySqlCommand cmd = new MySqlCommand(tuote0, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(tuote1, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(tuote3, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(tuote4, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(tuote5, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(tuote6, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(tuote7, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(tuote8, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(tuote9, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(tuote10, conn, tr);
                cmd.ExecuteNonQuery();
                tr.Commit();

            }
        }

        public void AddDefaultLaskut()
        {
            // Lisää default-laskut tietokantaan
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                var tr = conn.BeginTransaction();

                //---Default laskurivien luonti---

                string lasku0 = "INSERT INTO Lasku(Numero, Nimi, Osoite, PostiNumero, Paivays, EraPaiva, LisaTiedot, TyoAika_h, Palkka_h, LopullinenHinta) VALUES(1,'Kauppisen Pete','Tyä maa','1290','2024-3-26','2024-12-8','Ei lisätietoja',8,11,0)";

                string lasku1 = "INSERT INTO Lasku(Numero, Nimi, Osoite, PostiNumero, Paivays, EraPaiva, LisaTiedot, TyoAika_h, Palkka_h, LopullinenHinta) VALUES(2,'Töi mies','Töi maalla asun','1290','2024-3-26','2024-6-7','Menkää TÖIHIN',35,9,0)";

                string lasku2 = "INSERT INTO Lasku(Numero, Nimi, Osoite, PostiNumero, Paivays, EraPaiva, LisaTiedot, TyoAika_h, Palkka_h, LopullinenHinta) VALUES(3,'Viru valge mies','Eesti maa','372','2024-3-26','2024-4-1','saab maksta kolme pudeli viruvalge eest',25,7,0)";

                //---Default laskurivien luonti---

                string laskurivi0_0 = "INSERT INTO LaskuRivi(RiviID, Laskun_Numero, Tuote, Hinta, Maara, KokonaisHinta) VALUES(1,1,'ESISUODATIN',45,2,0)";

                string laskurivi0_1 = "INSERT INTO LaskuRivi(RiviID, Laskun_Numero, Tuote, Hinta, Maara, KokonaisHinta) VALUES(2,1,'KATUPORAKONE',335,1,0)";

                string laskurivi0_2 = "INSERT INTO LaskuRivi(RiviID, Laskun_Numero, Tuote, Hinta, Maara, KokonaisHinta) VALUES(3,1,'PEKKELE',15,2,0)";

                string laskurivi1_0 = "INSERT INTO LaskuRivi(RiviID, Laskun_Numero, Tuote, Hinta, Maara, KokonaisHinta) VALUES(4,2,'Lattialakka',0,5,0)";

                string laskurivi1_1 = "INSERT INTO LaskuRivi(RiviID, Laskun_Numero, Tuote, Hinta, Maara, KokonaisHinta) VALUES(5,2,'HIONTAKIRAHVI',1400,1,0)";

                string laskurivi2_0 = "INSERT INTO LaskuRivi(RiviID, Laskun_Numero, Tuote, Hinta, Maara, KokonaisHinta) VALUES(6,3,'KIRVES',20,1,0)";

                MySqlCommand cmd = new MySqlCommand(lasku0, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(lasku1, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(lasku2, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(laskurivi0_0, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(laskurivi0_1, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(laskurivi0_2, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(laskurivi1_0, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(laskurivi1_1, conn, tr);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(laskurivi2_0, conn, tr);
                cmd.ExecuteNonQuery();
                tr.Commit();

            }
        }

        public ObservableCollection<Tuote> GetTuotteet()
        {
            // Hakee tuotteet tietokannasta
            var tuotteet = new ObservableCollection<Tuote>();

            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                string query = "SELECT ID, Nimi, Price FROM tuote";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string nimi = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("Nimi")))
                    {
                        nimi = reader.GetString("Nimi");
                    }

                    tuotteet.Add(new Tuote
                    {
                        ID = reader.GetInt32("ID"),
                        Nimi = nimi,
                        Price = reader.GetDouble("Price")



                    });

                }

            }

            return tuotteet;
        }

        public void AddOmaTuote(Tuote tuote)
        {
            // Lisää tuotteen tietokantaan
            using (MySqlConnection conn = new MySqlConnection(localWithDb))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("INSERT INTO tuote(Nimi, Price) VALUES(@Nimi, @Price)", conn);

                cmd.Parameters.AddWithValue("@Nimi", tuote.Nimi);
                cmd.Parameters.AddWithValue("@Price", tuote.Price);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateTuote(Tuote tuote)
        {
            // Päivittää tuotteen tietokannassa nimen perusteella
            using (MySqlConnection conn = new MySqlConnection(localWithDb))

            {

                conn.Open();



                MySqlCommand cmd = new MySqlCommand("UPDATE tuote SET Nimi=@Nimi, Price=@Price WHERE Nimi=@Nimi", conn);

                cmd.Parameters.AddWithValue("@Nimi", tuote.Nimi);

                cmd.Parameters.AddWithValue("@Price", tuote.Price);

                cmd.ExecuteNonQuery();

            }
        }

        public void RemoveTuote(Tuote tuote)
        {
            // Poistaa tuotteen tietokannasta
            using (MySqlConnection conn = new MySqlConnection(localWithDb))

            {

                conn.Open();

                MySqlCommand cmd = new MySqlCommand("DELETE FROM tuote WHERE Nimi=@Nimi", conn);

                cmd.Parameters.AddWithValue("@Nimi", tuote.Nimi);

                cmd.Parameters.AddWithValue("@Price", tuote.Price);

                cmd.ExecuteNonQuery();

            }
        }


        public void AddLasku(Lasku lasku)
        {
            // Lisää laskutietueen tietokantaan
            using (MySqlConnection conn = new MySqlConnection(localWithDb))

            {

                conn.Open();



                MySqlCommand cmd = new MySqlCommand("INSERT INTO Lasku(Numero, Nimi, Osoite, PostiNumero, Paivays, EraPaiva, LisaTiedot, TyoAika_h, Palkka_h, LopullinenHinta) VALUES(@Numero,@Nimi,@Osoite,@PostiNumero,@Paivays,@EraPaiva,@LisaTiedot,@TyoAika_h,@Palkka_h,@LopullinenHinta)", conn);

                cmd.Parameters.AddWithValue("@Numero", lasku.LaskunNumero);

                cmd.Parameters.AddWithValue("@Nimi", lasku.CustomerName);

                cmd.Parameters.AddWithValue("@Osoite", lasku.Address);

                cmd.Parameters.AddWithValue("@PostiNumero", lasku.PostalCode);

                cmd.Parameters.AddWithValue("@Paivays", lasku.datetime);

                cmd.Parameters.AddWithValue("@EraPaiva", lasku.Duetime);

                cmd.Parameters.AddWithValue("@LisaTiedot", lasku.AdditionalInfo);

                cmd.Parameters.AddWithValue("@TyoAika_h", lasku.Work);

                cmd.Parameters.AddWithValue("@Palkka_h", lasku.Salary);

                cmd.Parameters.AddWithValue("@LopullinenHinta", lasku.TotalPrice);

                cmd.ExecuteNonQuery();



            }


        }

        public void RemoveLasku(Lasku lasku)
        {
            // Poistaa laskun tietokannasta
            using (MySqlConnection conn = new MySqlConnection(localWithDb))

            {

                conn.Open();



                MySqlCommand cmd = new MySqlCommand("DELETE FROM Lasku WHERE Numero=@Numero", conn);

                cmd.Parameters.AddWithValue("@Numero", lasku.LaskunNumero);

                cmd.Parameters.AddWithValue("@Nimi", lasku.CustomerName);

                cmd.Parameters.AddWithValue("@Osoite", lasku.Address);

                cmd.Parameters.AddWithValue("@PostiNumero", lasku.PostalCode);

                cmd.Parameters.AddWithValue("@Paivays", lasku.datetime);

                cmd.Parameters.AddWithValue("@EraPaiva", lasku.Duetime);

                cmd.Parameters.AddWithValue("@LisaTiedot", lasku.AdditionalInfo);

                cmd.Parameters.AddWithValue("@TyoAika_h", lasku.Work);

                cmd.Parameters.AddWithValue("@Palkka_h", lasku.Salary);

                cmd.Parameters.AddWithValue("@LopullinenHinta", lasku.TotalPrice);

                cmd.ExecuteNonQuery();



            }


        }

    }
}
