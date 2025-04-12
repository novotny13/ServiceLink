using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoServis360
{
    public class Zakaznik
    {
        int id;
        string jmeno;
        string prijmeni;
        string email;
        string telefon;
        string adresa;

        public int Id { get => id; set => id = value; }
        public string Jmeno { get => jmeno; set => jmeno = value; }
        public string Prijmeni { get => prijmeni; set => prijmeni = value; }
        public string Email { get => email; set => email = value; }
        public string Telefon { get => telefon; set => telefon = value; }
        public string Adresa { get => adresa; set => adresa = value; }

        public Zakaznik(int id, string jmeno, string prijmeni, string email, string telefon, string adresa)
        {
            this.id = id;
            this.jmeno = jmeno;
            this.prijmeni = prijmeni;
            this.email = email;
            this.telefon = telefon;
            this.adresa = adresa;
        }
    }

}
