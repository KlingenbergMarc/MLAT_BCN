using System;
using System.Windows.Forms;

namespace ED_SMR_MLAT_Performance
{
    public partial class FormClose : Form
    {
        bool exit; //Controla si se quiere salir de la app o no
        bool save; //Controla si se quiere guardar el file

        public FormClose()
        {
            InitializeComponent();
        }

        public bool ExportarExit()
        {
            return exit;
        }

        public bool ExportarSave()
        {
            return save;
        }

        private void DontSaveButton_Click(object sender, EventArgs e)
        {
            exit = true;
            save = false;
            Close();
        }

        private void CancelarButton_Click(object sender, EventArgs e)
        {
            exit = false;
            save = false;
            Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            exit = true;
            save = true;
            Close();
        }
    }
}
