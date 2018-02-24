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
    // Boje?
    // Optional: 
    // Enter nakon { da prijeđe u novi red sa tabom
    // Find funkciju

    public partial class Form1 : Form
    {        
        int velicinaTaba = 4;
        public Form1()
        {
            InitializeComponent();
            
        }
        

        void ProvjeraTabova()
        {
            // Provjerava ima li otvorenih tabova i ako nema onda stavlja na 'Disabled'
            // opcije koje ovise o tome da ima otvorenih tabova (tj. otvorenih file-ova)
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
            tabSizeToolStripMenuItem.Enabled    = EnableClick;

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Otvara novi tab sa novim txt file-om bez imena
            TabPage tp = new TabPage("(No name)");
            tabControl2.TabPages.Add(tp);
            tabSizeToolStripMenuItem.Enabled = true;
            RichTextBox tb = new RichTextBox();
            tb.KeyDown     += Tb_KeyDown;
            tb.KeyPress    += Tb_KeyPress;
            tb.MouseClick  += Tb_MouseClick;
            tb.KeyUp       += Tb_KeyUp;
            tb.AcceptsTab   = true;
            tb.TabStop      = false;
            tb.Dock         = DockStyle.Fill;
            tb.Multiline    = true;
            
            AutoComplete a = new AutoComplete();
            a.DoubleClick += Tb_DoubleClick;

            tp.Controls.Add(a);
            tp.Controls.Add(tb);

            tabControl2.SelectTab(tp);
            
           
            ProvjeraTabova();
            UpdateCurrentLine();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Otvara dijalog za otvaranje teksta i otvara taj tekst u novom tabu
            OpenFileDialog open = new OpenFileDialog();
            

            if (open.ShowDialog() == DialogResult.OK)
            {
                // Cita tekst iz otvorenog file-a, stavlja ga u RichTextBox i stavlja taj
                // RichTextBox u novootvoreni tab
                string filename = open.FileName;

                string[] filelines = File.ReadAllLines(filename);

                TabPage tp = new TabPage(filename.Split('\\').Last());
                tabControl2.TabPages.Add(tp);

                RichTextBox tb = new RichTextBox();
                tb.KeyDown      += Tb_KeyDown;
                tb.KeyPress     += Tb_KeyPress;
                tb.MouseClick   += Tb_MouseClick;
                tb.KeyUp        += Tb_KeyUp;
                tb.AcceptsTab    = true;
                tb.TabStop       = false;
                tb.Dock          = DockStyle.Fill;
                tb.Multiline     = true;
                foreach (string s in filelines)
                    tb.Text += s + "\n";

                AutoComplete a = new AutoComplete();
                a.DoubleClick += Tb_DoubleClick;

                tp.Controls.Add(a);
                tp.Controls.Add(tb);

                tabControl2.SelectTab(tp);

               
            }
            
            ProvjeraTabova();
            UpdateCurrentLine();
        }

        
        private void Tb_KeyUp(object sender, KeyEventArgs e)
        {
            // Na KeyUp provjeri na kojoj se liniji nalazi kursor
            UpdateCurrentLine();
        }

        
        private void Tb_MouseClick(object sender, MouseEventArgs e)
        {
            // Na MouseClick provjeri na kojoj se liniji nalazi kursor
            UpdateCurrentLine();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Zatvara odabrani tab
            tabControl2.TabPages.Remove(tabControl2.SelectedTab);
            ProvjeraTabova();
            UpdateCurrentLine();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Printa odabrani tab
            PrintPreviewDialog myPrintDialog = new PrintPreviewDialog();
            printDocument1.DocumentName      = tabControl2.SelectedTab.Text;
            myPrintDialog.Document           = printDocument1;

            if (myPrintDialog.ShowDialog() == DialogResult.OK)
                printDocument1.Print();
            
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Pomoćna funkcija za printanje
            RichTextBox tb = tabControl2.SelectedTab.Controls[1] as RichTextBox;
            string str     = tb.Text;
            int chars;
            int lines;
            Font f       = tb.Font;
            SolidBrush b = new SolidBrush(Color.Black);
            StringFormat strformat = new StringFormat();
            strformat.Trimming     = StringTrimming.Word;
            
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

            saveFileDialog1.Filter           = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex      = 1;
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
                Rtb.SelectedText = string.Empty; // Za paste-anje preko odabranog texta
                int i = Rtb.SelectionStart; // Da zapamti gdje je kursor bio
                string str = Clipboard.GetText(TextDataFormat.Text).ToString();
                i += str.Length; 
                Rtb.Text = Rtb.Text.Insert(Rtb.SelectionStart, str);
                Rtb.SelectionStart = i;
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cut-a text i stavlja na clipboard
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
            RichTextBox Rtb  = (RichTextBox)tabControl2.SelectedTab.Controls[1];
            FontDialog fd    = new FontDialog();
            fd.ShowColor     = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if(Rtb.SelectedText == string.Empty)
                {
                    Rtb.Font      = fd.Font;
                    Rtb.ForeColor = fd.Color;
                }
                else
                {
                    Rtb.SelectionFont  = fd.Font;
                    Rtb.SelectionColor = fd.Color;
                }
            }
        }

        private void webBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Otvara browser desno od text editora
            splitContainer1.Panel2Collapsed = false;
            splitContainer1.Panel2.Show();
            webBrowserToolStripMenuItem.Enabled = false;
            
            Label label      = new Label();
            label.Text       = "Adresa:";
            label.Size       = new Size(45, 20);
            label.Location   = new Point(5, 10);

            TextBox textBox  = new TextBox();
            textBox.Size     = new Size(Width / 2 - label.Width - 160, 20);
            textBox.Location = new Point(50, 10);
            textBox.KeyDown += traziEnter;

            Button btnTrazi   = new Button();
            btnTrazi.Location = new Point(textBox.Width + 50, 10);
            btnTrazi.Size     = new Size(50, 20);
            btnTrazi.Text     = "Traži";

            Button btnNazad   = new Button();
            btnNazad.Location = new Point(textBox.Width + 100, 10);
            btnNazad.Size     = new Size(50, 20);
            btnNazad.Text     = "Nazad";

            Button btnZatvori   = new Button();
            btnZatvori.Location = new Point(textBox.Width + 150, 10);
            btnZatvori.Size     = new Size(50, 20);
            btnZatvori.Text     = "Zatvori";

            WebBrowser browser = new WebBrowser();
            
            browser.Size     = new Size(splitContainer1.Panel2.Width-2, Height - 50);
            browser.Location = new Point(0, 40);
            browser.Anchor   = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            browser.ScriptErrorsSuppressed = true;

            splitContainer1.Panel2.Controls.Add(browser);
            splitContainer1.Panel2.Controls.Add(label);
            splitContainer1.Panel2.Controls.Add(textBox);
            splitContainer1.Panel2.Controls.Add(btnTrazi);
            splitContainer1.Panel2.Controls.Add(btnNazad);
            splitContainer1.Panel2.Controls.Add(btnZatvori);
            

            btnTrazi.Click   += new EventHandler(btnTrazi_Click);
            btnNazad.Click   += new EventHandler(btnNazad_Click);
            btnZatvori.Click += new EventHandler(btnZatvori_Click);
        }

        private void traziEnter(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                WebBrowser browser = (WebBrowser)splitContainer1.Panel2.Controls[0];
                TextBox tb = sender as TextBox;

                browser.Navigate(tb.Text);
            }
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
        void btnNazad_Click(object s, EventArgs e1)
        {
            splitContainer1.AutoScroll = true;
            WebBrowser browser = (WebBrowser)splitContainer1.Panel2.Controls[0];

            browser.GoBack();

        }

        void btnZatvori_Click(object s, EventArgs e1)
        {
            splitContainer1.Panel2Collapsed = true;
            splitContainer1.Panel2.Hide();
            webBrowserToolStripMenuItem.Enabled = true;
            splitContainer1.Panel2.Controls.Clear();

        }
        private void Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            RichTextBox tb = sender as RichTextBox;

            UpdateCurrentLine();

            // Ako nije ukljucen C# editor onda ne radi nista
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
            AutoComplete lista = (AutoComplete)tabControl2.SelectedTab.Controls[0];

            UpdateCurrentLine();

            // Korekcija velicine taba 
            if (e.KeyCode == Keys.Tab && lista.listShow == false)
            {
                e.SuppressKeyPress = true;

                int pozicija = tb.SelectionStart;
                string razmak = "";
                string prijeKursora = tb.Text.Substring(0, pozicija);
                string poslijeKursora = tb.Text.Substring(pozicija, tb.Text.Length - pozicija);

                for (int i = 0; i < velicinaTaba; ++i)
                    razmak += " ";

                tb.Text = prijeKursora + razmak + poslijeKursora;

                tb.SelectionStart = pozicija + velicinaTaba;
            }

            // Ako nije ukljucen C# editor onda ne radi nista
            if (!cEditorToolStripMenuItem.Checked)
            {
                return;
            }

            //brace completion
            if(e.KeyCode == Keys.B && e.Alt)
            {
                int pozicija = tb.SelectionStart;

                tb.Text = tb.Text.Insert(pozicija, "}");
                tb.SelectionStart = pozicija;
            }
            if(e.KeyCode == Keys.F && e.Alt)
            {
                int pozicija = tb.SelectionStart;

                tb.Text = tb.Text.Insert(pozicija, "]");
                tb.SelectionStart = pozicija;
            }
            if(e.KeyCode == Keys.D8 && e.Shift)
            {
                int pozicija = tb.SelectionStart;

                tb.Text = tb.Text.Insert(pozicija, ")");
                tb.SelectionStart = pozicija;
            }

            // Neke od kljucnih tipki ignoriraj
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu
                || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.RMenu)
            {
                return;
            }
            
            
            // Ako je upisano nesto osim slova, _ i - onda spremi trenutnu rijec i sakrij listu
            // Pazimo jos da ovdje iskljucimo tipke Up, Down i Tab jer one sluze za baratanje listom
            if (lista.listShow && 
                !( char.IsLetterOrDigit(Convert.ToChar(e.KeyCode)) || 
                e.KeyCode == Keys.OemMinus || 
                e.KeyCode == Keys.Up || 
                e.KeyCode == Keys.Down || 
                e.KeyCode == Keys.Tab ||
                e.KeyCode == Keys.Back )
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
                    e.SuppressKeyPress = true;
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
                    e.SuppressKeyPress = true;
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
                
                // Ako korisnik pritisne tab onda odabranu rijec stavi u tekst ili postavi tab i spremi rijec
                if (e.KeyCode == Keys.Tab)
                {
                    int pozicija;
                    string prijeKursora;
                    string poslijeKursora;
                    e.SuppressKeyPress = true;

                    // Ako je tab pritisnut nakon nove rijeci onda tu rijec dodaj u listu svih rijeci
                    // i nakon nje dodaj tabulator
                    if (lista.SelectedItem == null)
                    {
                        lista.DodajRijecIResetiraj();

                        // Dodaj tab nakon trenutne rijeci
                        pozicija = tb.SelectionStart;
                        string razmak = "";
                        prijeKursora = tb.Text.Substring(0, pozicija);
                        poslijeKursora = tb.Text.Substring(pozicija, tb.Text.Length - pozicija);

                        for (int i = 0; i < velicinaTaba; ++i)
                            razmak += " ";

                        tb.Text = prijeKursora + razmak + poslijeKursora;

                        tb.SelectionStart = pozicija + velicinaTaba;

                        return;
                    }

                    
                    // Odabranu rijec stavi u tekst i resetiraj podatke
                    string autoText = lista.SelectedItem.ToString();

                    int beginPlace = tb.SelectionStart - lista.count;
                    tb.Select(beginPlace, lista.count);
                    tb.SelectedText = "";
                    pozicija = tb.SelectionStart;
                    prijeKursora = tb.Text.Substring(0, pozicija);
                    poslijeKursora = tb.Text.Substring(pozicija, tb.Text.Length - pozicija);
                    tb.Text = prijeKursora + autoText + poslijeKursora;
                    tb.Focus();
                    lista.listShow = false;
                    lista.Hide();
                    tb.SelectionStart = autoText.Length + beginPlace;
                    lista.count = 0;
                    lista.keyword = "";

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
            int pozicija = tb.SelectionStart;
            string prijeKursora = tb.Text.Substring(0, pozicija);
            string poslijeKursora = tb.Text.Substring(pozicija, tb.Text.Length - pozicija);
            tb.Text = prijeKursora + autoText + poslijeKursora;
            tb.Focus();
            lista.listShow = false;
            lista.Hide();
            tb.SelectionStart = autoText.Length + beginPlace;
            lista.count = 0;
            lista.keyword = "";
        }

        private void tabSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Otvori NumericUpDown u kojem odabiremo velicinu taba
            NumericUpDown num = new NumericUpDown();
            num.Location = new Point(Width / 3, 30);
            num.Value = velicinaTaba;
            num.KeyDown += PostaviVelicinu;
            RichTextBox tb = tabControl2.SelectedTab.Controls[1] as RichTextBox;
            tb.Controls.Add(num);
            num.Show();
            num.Focus();
            
         }

        private void PostaviVelicinu(object sender, KeyEventArgs e)
        {
            // Provjerava jel u NumericUpDown pritisnuta tipka 'Enter' i sprema novu velicinu taba
            NumericUpDown num = sender as NumericUpDown;
            if(e.KeyCode == Keys.Enter)
            {
                velicinaTaba = (int)num.Value;
                num.Hide();
                tabControl2.SelectedTab.Controls[1].Focus(); // RichTextBox
            }

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ako se promijeni tab onda u status baru mijenjaj koja je linija
            UpdateCurrentLine();
        }

        private void UpdateCurrentLine()
        {
            // Provjerava koja je trenutna linija teksta i mijenja podatak na dnu aplikacije
            if (tabControl2.SelectedTab == null)
            {
                CurrentLine.Text = "Line: ";
                return;
            }

            RichTextBox tb = tabControl2.SelectedTab.Controls[1] as RichTextBox;
            int cursorPosition = tb.SelectionStart;
            int lineIndex = tb.GetLineFromCharIndex(cursorPosition);

            CurrentLine.Text = "Line: " + (lineIndex+1).ToString();
        }
    }
}
