using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ED_SMR_MLAT_Performance
{
    public partial class ICAOcode : Form
    {
        string Icao;

        public ICAOcode()
        {
            InitializeComponent();
        }

        public string ExportarIcao()
        {
            return Icao;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            Color myRgbColor = new Color();
            myRgbColor = Color.FromArgb(255, 63, 63);
            try
            {
                string temporalIcao = textBoxIcao.Text;
                if (temporalIcao.Count()==6)
                {
                    if (temporalIcao.Contains(" "))
                    {
                        textBoxIcao.BackColor = myRgbColor;
                        timer1.Interval = 800; // en milisegundos = 1segundo
                        timer1.Start();
                    }
                    else
                    {
                        Icao = temporalIcao;
                        Close();
                    }
                }
                else
                {
                    textBoxIcao.BackColor = myRgbColor;
                    timer1.Interval = 800; // en milisegundos = 1segundo
                    timer1.Start();
                }
            }
            catch (FormatException)
            {
                textBoxIcao.BackColor = myRgbColor;
                timer1.Interval = 800; // en milisegundos = 1segundo
                timer1.Start();
                MessageBox.Show("Incorrect data structure");
                return;
            }
        }

        private void ICAOcode_Load(object sender, EventArgs e)
        {
            textBoxIcao.Text = "344399"; //ICAO Address que se suele usar
            textBoxIcao.SelectAll();
            Icao = " ";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBoxIcao.BackColor = Color.White;//vuelve al color de la form
            timer1.Stop();
        }

        private void CancelarButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxIcao_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                SubmitButton.PerformClick();
            }
        }
    }
}
