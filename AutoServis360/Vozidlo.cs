using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoServis360
{
    public class Vozidlo
    {
        public int Id { get; set; }
        public int MajitelId { get; set; }
        public string Znacka { get; set; }
        public string Model { get; set; }
        public string SPZ { get; set; } // Nově přidaný sloupec odpovídající "LicensePlate" v DB
        public int RokVyroby { get; set; }

        public Vozidlo(int id, int majitelId, string znacka, string model, string spz, int rokVyroby)
        {
            Id = id;
            MajitelId = majitelId;
            Znacka = znacka;
            Model = model;
            SPZ = spz;
            RokVyroby = rokVyroby;
        }
    }

}
