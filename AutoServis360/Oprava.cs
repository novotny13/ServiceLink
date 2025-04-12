using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoServis360
{
    public class Oprava
    {
       private int id;
        private int vozidloId;
        private int mechanikId;
        private int zakanikId;
        private string popisProblemu;
        private string popisOpravy;
        private decimal cena;
        private string status;
        private DateTime? datumDokonceni;

        public int Id { get => id; set => id = value; }
        public int VozidloId { get => vozidloId; set => vozidloId = value; }
        public int ZakaznikId { get => zakanikId; set => zakanikId = value; }
        public int MechanikId { get => mechanikId; set => mechanikId = value; }
        public string PopisProblemu { get => popisProblemu; set => popisProblemu = value; }
        public string PopisOpravy { get => popisOpravy; set => popisOpravy = value; }
        public decimal Cena { get => cena; set => cena = value; }
        public string Status { get => status; set => status = value; }
        public DateTime? DatumDokonceni { get => datumDokonceni; set => datumDokonceni = value; }

        public Oprava(int id, int vozidloId, int mechanikId, int zakaznikId,string popisProblemu, string popisOpravy, decimal cena,  DateTime? datumDokonceni, string status)
        {
            this.id = id;
            this.vozidloId = vozidloId;
            this.mechanikId = mechanikId;
            this.zakanikId = zakaznikId;
            this.popisProblemu = popisProblemu;
            this.popisOpravy = popisOpravy;
            this.cena = cena;
           
            this.datumDokonceni = datumDokonceni;
            this.status = status;
        }

       
    }

}
