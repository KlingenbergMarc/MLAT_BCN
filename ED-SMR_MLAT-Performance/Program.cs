using System;
using System.Windows.Forms;

namespace ED_SMR_MLAT_Performance
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            form1.FormClosed += AllForms_Closed;
            form1.Show();
            Application.Run();
        }

        static bool NoCerrar = false; 

        private static void AllForms_Closed(object sender, FormClosedEventArgs e) //La aplicacion se cierra cuando todas las ventanas se cierran
        {
            ((Form)sender).FormClosed -= AllForms_Closed;

            if (Application.OpenForms.Count == 0)
            {
                if (!NoCerrar)
                {
                    Application.ExitThread();
                }
                else
                {
                    NoCerrar = false;
                    Form1 form1 = new Form1();
                    form1.FormClosed += AllForms_Closed;
                    form1.SetAppInterrumpida(true);
                    form1.Show();
                }
            }
            else
            {
                Application.OpenForms[0].FormClosed += AllForms_Closed;
            }
        }

        public static void SetNoCerrar(bool loading)
        {
            NoCerrar = loading;
        }
    }
}
