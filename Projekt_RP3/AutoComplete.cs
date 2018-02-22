using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt_RP3
{
    public partial class AutoComplete : System.Windows.Forms.ListBox
    {
        public List<string> SveRiječi; // Lista u koju spremamo sve riječi za preporuku

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

            SveRiječi = new List<string>();
            preporuka = new List<string>();

            preporuka.Add("int");
            preporuka.Add("double");
            preporuka.Add("string");
            preporuka.Add("char");
            preporuka.Add("List");
            




            bs = new BindingSource();
            bs.DataSource = preporuka;
            this.DataSource = bs;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
