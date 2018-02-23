using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt_RP3
{
    //Paziti:
    //Za svaku opciju koja ovisi o tome da ima neki text otvoren nadodati u provjera tabova!!!!!
    // TODO:
    // Mijenjanje veličine taba (colision sa biranjem pomoću taba)
    // Napraviti listu ključnih riječi (45. slajd predavanja)
    // Boje?
    // Optional: 
    // Status bar koji pokazuje u kojem smo retku
    // Enter nakon { da prijeđe u novi red sa tabom
    // Find funkciju
    // Na dobule click ponuđene opcije se sprema nedovršena riječ (no -> notepad)

    public partial class Form1 : Form
    {
        private readonly object tabControl1;
        
        public Form1()
        {
            InitializeComponent();
            
        }
        

        void ProvjeraTabova()
        {
            // Mijenja mogućnost odabira opcija na osnovu toga ima li otvorenih tabova ili ne
            Boolean EnableClick = true;
            if (tabControl2.SelectedTab == null)
            {
                EnableClick = false;
            }
            saveToolStripMenuItem.Enabled       = EnableClick;
            printToolStripMenuItem.Enabled      = EnableClick;
            closeToolStripMenuItem.Enabled      = EnableClick;
            copyToolStripMenuItem.Enabled       = EnableClick;
            pasteToolStripMenuItem.Enabled      = EnableClick;
            cutToolStripMenuItem.Enabled        = EnableClick;
            selectAllToolStripMenuItem.Enabled  = EnableClick;
            deleteToolStripMenuItem.Enabled     = EnableClick;
            changeFontToolStripMenuItem.Enabled = EnableClick;

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Otvara novi tab sa novit txt file-om bez imena
            TabPage tp = new TabPage("(No name)");
            tabControl2.TabPages.Add(tp);

            RichTextBox tb = new RichTextBox();
            tb.KeyDown     += Tb_KeyDown;
            tb.KeyPress    += Tb_KeyPress;
            tb.AcceptsTab = true;
            tb.TabStop = false;
            tb.Dock = DockStyle.Fill;
            tb.Multiline = true;
            tabSizeToolStripMenuItem.Enabled = true;
            
            AutoComplete a = new AutoComplete();
            a.DoubleClick += Tb_DoubleClick;

            tp.Controls.Add(a);
            tp.Controls.Add(tb);

            tabControl2.SelectTab(tp);

            //veličine taba po defaultu 4 spacea(okoprilike)
            tabSizeMenuItem3.Checked = true;
            


            ProvjeriVeličinuTaba(tb);
            ProvjeraTabova();
        }
        //kad tb prihvaća tab
        private void ProvjeriVeličinuTaba(RichTextBox tb)
        {
            if (tabSizeMenuItem1.Checked)
            {
                tabSizeMenuItem2.Checked = false;
                tabSizeMenuItem3.Checked = false;
                tabSizeMenuItem4.Checked = false;

                tb.SelectionTabs = new int[] { 25, 50, 75, 100 };
            }
            if (tabSizeMenuItem2.Checked)
            {
                tabSizeMenuItem1.Checked = false;
                tabSizeMenuItem3.Checked = false;
                tabSizeMenuItem4.Checked = false;

                tb.SelectionTabs = new int[] { 50, 100, 150, 200 };
            }
            if (tabSizeMenuItem3.Checked)
            {
                tabSizeMenuItem1.Checked = false;
                tabSizeMenuItem2.Checked = false;
                tabSizeMenuItem4.Checked = false;

                tb.SelectionTabs = new int[] { 100, 200, 300, 400 };
            }
            if (tabSizeMenuItem4.Checked)
            {
                MessageBox.Show("tu");
                tabSizeMenuItem1.Checked = false;
                tabSizeMenuItem2.Checked = false;
                tabSizeMenuItem3.Checked = false;

                tb.SelectionTabs = new int[] { 200, 400, 600, 800 };
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Zatvara odabrani tab
            tabControl2.TabPages.Remove(tabControl2.SelectedTab);
            ProvjeraTabova();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Printa odabrani tab
            PrintPreviewDialog myPrintDialog = new PrintPreviewDialog();
            printDocument1.DocumentName = tabControl2.SelectedTab.Text;
            myPrintDialog.Document = printDocument1;
            if (myPrintDialog.ShowDialog() == DialogResult.OK)
                printDocument1.Print();
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Otvara dijalog za otvaranje teksta i otvara taj tekst u novom tabu
            OpenFileDialog open = new OpenFileDialog();

            if (open.ShowDialog() == DialogResult.OK)
            {
                string filename = open.FileName;

                string[] filelines = File.ReadAllLines(filename);

                TabPage tp = new TabPage(filename.Split('\\').Last());
                tabControl2.TabPages.Add(tp);

                RichTextBox tb = new RichTextBox();
                tb.KeyDown      += Tb_KeyDown;
                tb.KeyPress     += Tb_KeyPress;
                tb.AcceptsTab = true;
                tb.TabStop = false;
                tb.Dock = DockStyle.Fill;
                tb.Multiline = true;
                foreach (string s in filelines)
                    tb.Text += s + "\n";

                AutoComplete a = new AutoComplete();
                a.DoubleClick += Tb_DoubleClick;

                tp.Controls.Add(a);
                tp.Controls.Add(tb);

                tabControl2.SelectTab(tp);

                tabSizeToolStripMenuItem.Enabled = true;
                tabSizeMenuItem3.Checked = true;
                tb.SelectionTabs = new int[] { 100, 200, 300, 400 };
                ProvjeriVeličinuTaba(tb);
            }
            
            ProvjeraTabova();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Pomoćna funkcija za printanje
            string str = tabControl2.SelectedTab.Controls[1].Text;
            int chars;
            int lines;
            Font f = new Font("Arial", 12);
            SolidBrush b = new SolidBrush(Color.Black);
            StringFormat strformat = new StringFormat();
            strformat.Trimming = StringTrimming.Word;
            
            RectangleF myrect = new RectangleF(e.MarginBounds.Left,e.MarginBounds.Top, 
                                               e.MarginBounds.Width, e.MarginBounds.Height);
            
            SizeF sz = new SizeF(e.MarginBounds.Width, e.MarginBounds.Height - f.GetHeight(e.Graphics));
            e.Graphics.MeasureString(str, f, sz, strformat, out chars, out lines);
            string printstr = str.Substring(0, chars);
            
            e.Graphics.DrawString(printstr, f, b, myrect, strformat);
            

            if (str.Length > chars) 
            {
                str = str.Substring(chars);
                e.HasMorePages = true;
            }     
            else
                e.HasMorePages = false;

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Sprema tekstualnu datoteku (trenutno odabrani tab)
            RichTextBox Rtb = (RichTextBox) tabControl2.SelectedTab.Controls[1];
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.Create))
                    using (StreamWriter sw = new StreamWriter(s))
                    {
                        sw.Write(Rtb.Text);
                    }
                MessageBox.Show("Spremljeno " + saveFileDialog1.FileName);
                tabControl2.SelectedTab.Text = saveFileDialog1.FileName.Split('\\').Last();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Stavlja odabrani tekst na clipboard
            RichTextBox Rtb = (RichTextBox) tabControl2.SelectedTab.Controls[1];
            if (Rtb.SelectedText != string.Empty)
            {
                Clipboard.SetText(Rtb.SelectedText);
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Paste-anje teksta sa clipboarda na mjesto kursora
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[1];
            if (Clipboard.GetText(TextDataFormat.Text).ToString() != string.Empty)
            {
                Rtb.SelectedText = string.Empty; // Za pasteanje preko odabranog texta
                int i = Rtb.SelectionStart; // Da zapami gdje je kursor bio
                string str = Clipboard.GetText(TextDataFormat.Text).ToString();
                i += str.Length; 
                Rtb.Text = Rtb.Text.Insert(Rtb.SelectionStart, str);
                Rtb.SelectionStart = i;
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cut-a text i stavlja na cliboard
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[1];
            if (Rtb.SelectedText != string.Empty)
            {
                Clipboard.SetText(Rtb.SelectedText);
                Rtb.SelectedText = string.Empty;
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Odabire cijeli tekst u trenutnom otvorenom tabu
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[1];
            Rtb.SelectAll();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Briše trenutno odabrani tekst u trenutno otvorenom tabu
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[1];
            Rtb.SelectedText = string.Empty;
        }

        private void changeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Mijenja font teksta
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[1];
            FontDialog fd = new FontDialog();
            fd.ShowColor = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if(Rtb.SelectedText == string.Empty)
                {
                    Rtb.Font = fd.Font;
                    Rtb.ForeColor = fd.Color;
                }
                else
                {
                    Rtb.SelectionFont = fd.Font;
                    Rtb.SelectionColor = fd.Color;
                }
            }
        }

        private void webBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Otvara browser desno od text editora
            splitContainer1.Panel2Collapsed = false;
            splitContainer1.Panel2.Show();
            
            Label label = new Label();
            label.Text = "Adresa:";
            label.Size = new Size(45, 20);
            label.Location = new Point(5, 10);

            TextBox textBox = new TextBox();
            textBox.Size = new Size(Width / 2 - label.Width - 160, 20);
            textBox.Location = new Point(50, 10);

            Button btnTrazi = new Button();
            btnTrazi.Location = new Point(textBox.Width + 50, 10);
            btnTrazi.Size = new Size(50, 20);
            btnTrazi.Text = "Traži";


            Button btnZatvori = new Button();
            btnZatvori.Location = new Point(textBox.Width + 100, 10);
            btnZatvori.Size = new Size(60, 20);
            btnZatvori.Text = "Zatvori";

            WebBrowser browser = new WebBrowser();
            
            browser.Size = new Size(Width, Height - 50);
            browser.Location = new Point(0, 40);
            browser.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            browser.ScriptErrorsSuppressed = true;

            splitContainer1.Panel2.Controls.Add(browser);
            splitContainer1.Panel2.Controls.Add(label);
            splitContainer1.Panel2.Controls.Add(textBox);
            splitContainer1.Panel2.Controls.Add(btnTrazi);
            splitContainer1.Panel2.Controls.Add(btnZatvori);
            

            btnTrazi.Click += new EventHandler(btnTrazi_Click);
            btnZatvori.Click += new EventHandler(btnZatvori_Click);
        }

        void btnTrazi_Click(object s, EventArgs e1)
        {
            splitContainer1.AutoScroll = true;
            WebBrowser browser = new WebBrowser();
            browser = (WebBrowser)splitContainer1.Panel2.Controls[0];
            TextBox textBox = new TextBox();
            textBox = (TextBox)splitContainer1.Panel2.Controls[2];
            browser.Navigate(textBox.Text);

        }

        void btnZatvori_Click(object s, EventArgs e1)
        {
            splitContainer1.Panel2Collapsed = true;
            splitContainer1.Panel2.Hide();

        }
        private void Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            RichTextBox tb = sender as RichTextBox;

            
            if (!cEditorToolStripMenuItem.Checked)
            {
                return;
            }

           
            AutoComplete lista = (AutoComplete)tabControl2.SelectedTab.Controls[0];

            // Ako je lista već otvorena
            if (lista.listShow == true) 
            {
                if(e.KeyChar == 8) // 8 predstavlja BACKSPACE
                {
                    // Ako se pritisne BACKSPACE smanji keyword i ponovno mijenjaj ponudjenu rijec
                    lista.keyword = lista.keyword.Remove(lista.keyword.Length - 1);
                    lista.MijenjajListu();
                    lista.count--;
                    if (lista.count == 0) // Ako je prazna rijec sve resetiraj i odmakni listu
                    {
                        lista.count = 0;
                        lista.keyword = "";
                        lista.listShow = false;
                        lista.Hide();
                    }
                    else 
                        tb.Focus();
                    
                    
                }
                else
                {
                    lista.keyword += e.KeyChar;
                    lista.MijenjajListu();
                    lista.count++;
                    tb.Focus();
                }
                
            }
            else
            { 
                // Ako lista nije otvorena otvori je i pokazi preporucene rijeci
                // Rijeci koje preporucujemo se sastoje od brojeva, slova, obicne crte i donje crte
                if (char.IsLetterOrDigit( e.KeyChar ) || e.KeyChar == '_' || e.KeyChar == '-') 
                {
                    lista.keyword += e.KeyChar;
                    lista.MijenjajListu();
                    lista.listShow = true;
                    Point point = tb.GetPositionFromCharIndex(tb.SelectionStart);
                    point.Y += (int)Math.Ceiling(tb.Font.GetHeight()) + tb.Location.Y; 
                    point.X += tb.Location.Y; 
                    lista.Location = point;
                    lista.count++;
                    tb.Focus();

                }
            }
        }

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox tb = sender as RichTextBox;
            
            if (!cEditorToolStripMenuItem.Checked)
            {
                return;
            }
            
            if(e.KeyCode == Keys.B && e.Alt)
            {
                int pozicija = tb.SelectionStart;

                tb.Text = tb.Text.Insert(pozicija, "}");
                tb.SelectionStart = pozicija;
            }
            

            // Neke od kljucnih tipki ignoriraj
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu
                || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.RMenu)
            {
                return;
            }
            
            
            AutoComplete lista = (AutoComplete)tabControl2.SelectedTab.Controls[0];

            
            // Ako je upisano nesto osim slova, _ i - onda spremi trenutnu rijec i sakrij listu
            // Pazimo jos da ovdje iskljucimo tipke Up, Down i Tab jer one sluze za baratanje listom
            if (!( char.IsLetterOrDigit(Convert.ToChar(e.KeyCode)) || 
                e.KeyCode == Keys.OemMinus || 
                e.KeyCode == Keys.Up || 
                e.KeyCode == Keys.Down || 
                e.KeyCode == Keys.Tab ||
                e.KeyCode == Keys.Back)
                || // Ovo provjerava ako je neki specijalni znak za koji treba kombinacije dvije tipke (npr. '&')
                (char.IsLetterOrDigit(Convert.ToChar(e.KeyCode)) && (e.Alt || e.Control || e.Shift)) )
            {
                lista.DodajRijecIResetiraj();
                lista.Hide();

            }
            
            // Ako je lista pokazana
            if (lista.listShow == true)
            {
                // Tipka gore pomice oznaceni element na listi gore
                if (e.KeyCode == Keys.Up)
                {
                    lista.Focus();
                    if (lista.SelectedIndex != 0)
                    {
                        lista.SelectedIndex -= 1;
                    }
                    else
                    {
                        lista.SelectedIndex = 0;
                    }
                    tb.Focus();

                }// Tipka dolje pomice oznaceni element na listi dolje
                else if (e.KeyCode == Keys.Down)
                {
                    lista.Focus();
                    try
                    {
                        lista.SelectedIndex += 1;
                    }
                    catch
                    {
                    }
                    tb.Focus();
                }
                
                // Ako korisnik pritisne tab onda odabranu rijec stavi u tekst
                if (e.KeyCode == Keys.Tab)
                {

                    string autoText = lista.SelectedItem.ToString();

                    int beginPlace = tb.SelectionStart - lista.count;
                    tb.Select(beginPlace, lista.count);
                    tb.SelectedText = "";
                    tb.Text += autoText;
                    tb.Focus();
                    lista.listShow = false;
                    lista.Hide();
                    int endPlace = autoText.Length + beginPlace;
                    tb.SelectionStart = endPlace;
                    lista.count = 0;

                }
            }
        }

        private void Tb_DoubleClick(object sender, EventArgs e)
        {
            // Double click misom na neki tekst u ListBox-u odabire taj tekst
            // i stavlja ga u RichTextBox, resetira ostale podatke i sakriva listu
            RichTextBox tb = tabControl2.SelectedTab.Controls[1] as RichTextBox;
            AutoComplete lista = sender as AutoComplete;

            string autoText = lista.SelectedItem.ToString();
            int beginPlace = tb.SelectionStart - lista.count;
            tb.Select(beginPlace, lista.count);
            tb.SelectedText = "";
            tb.Text += autoText;
            tb.Focus();
            lista.listShow = false;
            lista.Hide();
            int endPlace = autoText.Length + beginPlace;
            tb.SelectionStart = endPlace;
            lista.count = 0;
        }

        private void tabSizeMenuItem_CheckedChanged(object sender, EventArgs e)
        {

            RichTextBox tb = tabControl2.SelectedTab.Controls[1] as RichTextBox;

            ProvjeriVeličinuTaba(tb);
        }
        //brace completion

    }

   
}
