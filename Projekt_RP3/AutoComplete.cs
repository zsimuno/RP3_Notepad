using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt_RP3
{
    public partial class AutoComplete : System.Windows.Forms.ListBox
    {
        public List<string> SveRijeci; // Lista u koju spremamo sve riječi za preporuku

        public List<string> preporuka; // Lista koja je povezana sa autoComplete ListBox-om 
                                       // i u nju spremamo riječi koje ćemo preporučiti korisniku
        public BindingSource bs;

        public bool listShow;
        public string keyword;
        public int count;
        public AutoComplete()
        {
            InitializeComponent();

            this.TabStop = false;
            this.Visible = false;

            listShow = false;
            keyword = "";
            count = 0;

            SveRijeci = new List<string>();
            preporuka = new List<string>();

            bs = new BindingSource();
            bs.DataSource = preporuka;
            this.DataSource = bs;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void DodajRijecIResetiraj() // Dodaje rijec u glavnu listu i resetira listu
        {
            if(!SveRijeci.Contains(keyword))
            {
                SveRijeci.Add(keyword);
                SveRijeci.Sort();
            }
                
            count = 0;
            keyword = "";
            listShow = false;
        }

        public void MijenjajListu() // Stavlja u preporuke samo rijeci koje pocinju sa onim sto se upisuje
        {
            preporuka.Clear();
            foreach(string s in SveRijeci)
            {
                if(s.StartsWith(keyword))
                {
                    preporuka.Add(s);
                }
            }
            Debug.WriteLine(preporuka.Count.ToString());
            if(preporuka.Count == 0)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
            bs.ResetBindings(false);
        }
    }
}
