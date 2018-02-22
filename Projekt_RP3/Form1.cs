using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            tb.Dock = DockStyle.Fill;
            tb.Multiline = true;

            tp.Controls.Add(tb);


            tabControl2.SelectTab(tp);

            ProvjeraTabova();
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
                tb.Dock = DockStyle.Fill;
                tb.Multiline = true;
                foreach (string s in filelines)
                    tb.Text += s + "\n";

                tp.Controls.Add(tb);

                tabControl2.SelectTab(tp);
            }

            ProvjeraTabova();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Pomoćna funkcija za printanje
            string str = tabControl2.SelectedTab.Controls[0].Text;
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
            RichTextBox Rtb = (RichTextBox) tabControl2.SelectedTab.Controls[0];
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
            RichTextBox Rtb = (RichTextBox) tabControl2.SelectedTab.Controls[0];
            if (Rtb.SelectedText != string.Empty)
            {
                Clipboard.SetText(Rtb.SelectedText);
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Paste-anje teksta sa clipboarda na mjesto kursora
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[0];
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
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[0];
            if (Rtb.SelectedText != string.Empty)
            {
                Clipboard.SetText(Rtb.SelectedText);
                Rtb.SelectedText = string.Empty;
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Odabire cijeli tekst u trenutnom otvorenom tabu
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[0];
            Rtb.SelectAll();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Briše trenutno odabrani tekst u trenutno otvorenom tabu
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[0];
            Rtb.SelectedText = string.Empty;
        }

        private void changeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Mijenja font teksta
            RichTextBox Rtb = (RichTextBox)tabControl2.SelectedTab.Controls[0];
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
            //browser.Document.Window.Size = new Size(Width, Height - 50);
            browser.Size = new Size(Width, Height - 50);
            browser.Location = new Point(0, 40);
            browser.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;

            splitContainer1.Panel2.Controls.Add(browser);
            splitContainer1.Panel2.Controls.Add(label);
            splitContainer1.Panel2.Controls.Add(textBox);
            splitContainer1.Panel2.Controls.Add(btnTrazi);
            splitContainer1.Panel2.Controls.Add(btnZatvori);
            

            btnTrazi.Click += new EventHandler(btnTrazi_Click);
            btnZatvori.Click += new EventHandler(btnZatvori_Click);

            void btnTrazi_Click(object s, EventArgs e1)
            {
                splitContainer1.AutoScroll = true;
                browser.Navigate(textBox.Text);

            }

            void btnZatvori_Click(object s, EventArgs e1)
            {
                splitContainer1.Panel2Collapsed = true;
                splitContainer1.Panel2.Hide();

            }
        }

        
            
            

        
    }
}
