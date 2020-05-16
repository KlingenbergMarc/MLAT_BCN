using System;
using System.Drawing;
using System.Windows.Forms;

namespace ED_SMR_MLAT_Performance
{
    public partial class FormInformativaYesNo : Form
    {
        public FormInformativaYesNo()
        {
            InitializeComponent();
        }

        bool respuesta = false; //false = Cancel

        public bool ExportarRespuesta()
        {
            return respuesta;
        }

        public void ImportarInformacion(string Info1, string Info2, int formWidth, int xButton1, int xButton2, int xLabel1, int xLabel2)
        {
            this.label1.Text = Info1;
            this.label2.Text = Info2;
            this.Size = new Size(formWidth, this.Height);
            this.OkButton.Location = new Point(xButton1, OkButton.Location.Y);
            this.CancelarButton.Location = new Point(xButton2, CancelarButton.Location.Y);
            this.label1.Location = new Point(xLabel1, label1.Location.Y);
            this.label2.Location = new Point(xLabel2, label2.Location.Y);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            respuesta = true;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            respuesta = false;
            Close();
        }
    }
}
