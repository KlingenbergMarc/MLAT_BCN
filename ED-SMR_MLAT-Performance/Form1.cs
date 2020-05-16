using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Codigo;

namespace ED_SMR_MLAT_Performance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        LectorMensaje myList = new LectorMensaje();
        LectorMensaje myListDGPS = new LectorMensaje();
        LectorMensaje myListConDescartados = new LectorMensaje();
        string nombreFichero = "";//Guarda el nombre del fichero .ast leido
        bool nuevoFicheroMain = false; //Controla si se genera la form desde escritorio o desde Main Form
        bool cerrarMain = false;//boleano que cierra el Main si se carga bien el fichero (desde el main)
        Point labelInfoLoc; //Guarda la posicion incial de la label informativa (Label info de fichero despues de load)
        bool appInterrupida = false; //A veces la aplicacion se cierra sola, este boleano controla esa interrupción y la impide.

        private void Form1_Load(object sender, EventArgs e)
        {
            labelInfoLoc = labelInformativa.Location;
            if (!nuevoFicheroMain)
            {
                labelWelcome.Text = "Welcome to ED-MLAT Performance\n                    Evaluator";
                labelSubWelcome.Text = "This application calculates the value of the different parameters\nlisted in the document of EUROCAE “MOPS for Mode S MLAT\nSystems” (ED-117) using .ast files of opportunity traffic from LEBL,\nand determines if the MLAT meets these minimum requirements.\nIt also allows analyzing D-GPS recordings to calculate position\naccuracy data; and it provides an interface that allows you to\ndiscard certain vehicles from the analysis.";
                if (appInterrupida)
                {
                    appInterrupida = false;
                    CrearFormInformativa("An error has occurred. Something", "went wrong. Please try again.", 347, 116, 27, 46);
                }
            }
            else
            {
                LoadFileButton.PerformClick();
            }
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            string nombre;
            //try
            //{
                this.openFileDialog1.FileName = "";
                this.openFileDialog1.Filter = "File documents (.ast)|*.ast";
                Program.SetNoCerrar(true);
                this.openFileDialog1.ShowDialog();
                Program.SetNoCerrar(false);
                if (this.openFileDialog1.FileName.Equals("") == false)
                {
                    nombre = (this.openFileDialog1.FileName);
                    Cursor.Current = Cursors.WaitCursor;
                    LectorMensaje listTemporal = new LectorMensaje();//Si ya hemos cargado una lista, no queremos que se sobre escriba con una list que no se cargue correctamente
                    Program.SetNoCerrar(true);
                    listTemporal.CargarListaDeDirectorio(nombre);
                    Program.SetNoCerrar(false);
                    if (listTemporal.GetNumList() != 0)
                    {
                        if (!listTemporal.TodoSMR())
                        {
                            Program.SetNoCerrar(true);
                            if (nuevoFicheroMain)
                            {
                                cerrarMain = true;
                                nuevoFicheroMain = false;
                            }
                            myListConDescartados = listTemporal;

                            bool diferentesSIC = myListConDescartados.ComprobarDiferentesSIC();//Controla si hay SMR y MLAT en un mismo fichero
                            if (diferentesSIC)
                            {
                                CrearFormInformativa("This file contains both SMR and MLAT packages. Only MLAT", "can be evaluated. SMR packages will be discarded.", 518, 201, 17, 41);
                                Cursor.Current = Cursors.WaitCursor;
                                LectorMensaje listaTemporal = new LectorMensaje();
                                listaTemporal = myListConDescartados.SepararSMRyADSB();
                                myListConDescartados.ClearList();
                                myListConDescartados = listaTemporal;
                            }
                            myListConDescartados.OrdenarPaquetesPorTiempo();//Corrige algunos paquetes no ordenados
                            myListConDescartados.SetUTCcorregido(); //Coregimos la hora para cambio de dia. Se hace despues de separar MLAT y SMR, ya que
                                                                    //cuando estan juntos los ficheros no estan perfectamente ordenados en tiempo y la funcion no funciona
                            myList = myListConDescartados.DescartarVehiculosSquitter();//Tambien ordenamos en tiempo los vehiculos Squitter, puesto que despues se pueden 
                                                                                       //llegar a usar en MainForm

                            nombreFichero = this.openFileDialog1.SafeFileName;

                            labelWelcome.Visible = false;
                            labelSubWelcome.Visible = false;
                            ResultadosLoad();
                            AvaluateButton.Visible = true;
                            AddDGPSButton.BackColor = Color.FromArgb(255, 128, 0);
                            AddDGPSButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 128, 0);
                            myListDGPS.ClearList();
                            AddDGPSButton.Visible = true;
                            LoadFileButton.Visible = false;
                            Program.SetNoCerrar(false);
                        }
                        else
                        {
                            CrearFormInformativa("The file only contains SMR data. This program only evaluates MLAT.", "The program is also able to filter MLAT from (SMR + MLAT) files.", 575, 225, 22, 42);
                            if (nuevoFicheroMain)
                            {
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        CrearFormInformativa("Error while loading the file.", "Maybe wrong .ast category?", 280, 80, 22, 12);//Valores puestos a mano para cuadradrar el texto segun la longitud del propio texto
                        if (nuevoFicheroMain)
                        {
                            this.Close();
                        }
                    }
                }
                else
                {
                    CrearFormInformativa("Please, ", "select a file first.", 173, 28, 46, 15);
                    if (nuevoFicheroMain)
                    {
                        this.Close();
                    }
                }
            //}
            //catch (FormatException)
            //{
            //    CrearFormInformativa("Error while loading the file,", "incorrect data structure.", 261, 73, 17, 18);
            //    return;
            //}
            //catch (FileNotFoundException)
            //{
            //    CrearFormInformativa("File not", "found.", 155, 19, 40, 43);
            //    return;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return;
            //}
        }

        private void AddDGPSButton_Click(object sender, EventArgs e)
        {
            string nombre;
            string icao = "";
            ICAOcode address = new ICAOcode();
            address.ShowDialog();
            icao = address.ExportarIcao();
            if (icao != " ")
            {
                try
                {
                    List<string> descartados = myListConDescartados.LeerVehiculosSquitter();
                    int i = 0;
                    bool encontrado = false;
                    bool respuesta = false;
                    while ((i < descartados.Count()) && (!encontrado))
                    {
                        if (icao == descartados[i])
                        {
                            encontrado = true;
                            respuesta = CrearFormInformativaYesNo("This ICAO Address is in the list of discarded vehicles.", "Do you want to remove it from the list?", 476, 97, 241, 27, 71); //Si el ICAO Address D-GPS esta en la lista de descartados
                            if (respuesta)
                            {
                                Cursor.Current = Cursors.WaitCursor;
                                descartados.Remove(icao);
                                File.Delete("VehiculosAEliminar.txt");
                                File.WriteAllLines("VehiculosAEliminar.txt", descartados);
                                myList.ClearList();
                                myList = myListConDescartados.DescartarVehiculosSquitter();
                                ResultadosLoad();
                            }
                        }
                        i++;
                    }
                    if ((encontrado && respuesta) || (!encontrado))//Si esta en la lista de descartados y se elimina, o sino esta en la lista
                    {
                        this.openFileDialog1.FileName = "";
                        this.openFileDialog1.Filter = "File documents (.txt)|*.txt";
                        this.openFileDialog1.ShowDialog();
                        if (this.openFileDialog1.FileName.Equals("") == false)
                        {
                            nombre = (this.openFileDialog1.FileName);
                            Cursor.Current = Cursors.WaitCursor;
                            LectorMensaje listTemporal = new LectorMensaje();//Si ya hemos cargado una lista, no queremos que se sobre escriba con una list que no se cargue correctamente
                            listTemporal.CargarListaDeDirectorioDGPS(nombre, icao);
                            if (listTemporal.GetNumList() != 0)
                            {
                                int matches = listTemporal.SetIndicesDGPS(myList);//Devuelve matches y hace SetIndiceDGPS()
                                List<int> a = new List<int>();
                                for (int j = 0; j < listTemporal.GetNumList(); j++)
                                {
                                    if (listTemporal.GetPlanI(j).GetIndiceDGPS() != -1)
                                    {
                                        a.Add(j);
                                    }
                                }
                                Matches form = new Matches();
                                form.ImportarMatches(matches);
                                form.ShowDialog();
                                if (matches != 0)
                                {
                                    myListDGPS = listTemporal;
                                    AddDGPSButton.BackColor = Color.LimeGreen;
                                    AddDGPSButton.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
                                }
                            }
                            else
                            {
                                CrearFormInformativa("Error while loading the file.", "Maybe wrong format?", 261, 73, 17, 18);
                            }
                        }
                        else
                        {
                            CrearFormInformativa("Please, ", "select a file first.", 173, 28, 46, 15);
                        }
                    }
                }
                catch (FormatException)
                {
                    CrearFormInformativa("Error while loading the file,", "incorrect data structure.", 261, 73, 17, 18);
                    return;
                }
                catch (System.IndexOutOfRangeException)
                {
                    CrearFormInformativa("Error while loading the file,", "incorrect data structure.", 261, 73, 17, 18);
                    return;
                }
                catch (FileNotFoundException)
                {
                    CrearFormInformativa("File not", "found.", 155, 19, 40, 43);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void AvaluateButton_Click(object sender, EventArgs e)
        {
            try
            {
                bool respuesta = true;
                if (myListDGPS.GetNumList() == 0)
                {
                    respuesta = CrearFormInformativaYesNo("No D-GPS file has been added, so some features of the application", "won't be available (mainly position accuracy results).", 581, 166, 310, 27, 70);
                }
                if (respuesta)
                {
                    MainForm evaluar = new MainForm();
                    evaluar.SetMensaje(myList, myListDGPS, myListConDescartados);
                    string nuevoNombreFichero = "";
                    if (nombreFichero.Length >= 4)
                    {
                        for (int n = 0; n < nombreFichero.Length - 4; n++)//Recortamos el ".ast"
                            nuevoNombreFichero = nuevoNombreFichero + nombreFichero[n];
                    }
                    evaluar.SetNombreFichero(nuevoNombreFichero);
                    evaluar.StartPosition = FormStartPosition.CenterScreen;
                    evaluar.WindowState = FormWindowState.Maximized;
                    evaluar.Text = nuevoNombreFichero + " - ED-MLAT Permormance Evaluator";
                    evaluar.Show();
                    this.Close();
                }
            }
            catch (FormatException)
            {
                CrearFormInformativa("Incorrect data", "structure.", 193, 36, 27, 46);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        public void SetNuevoFichero(bool nuevo)
        {
            this.nuevoFicheroMain = nuevo;
        }

        public void SetAppInterrumpida(bool interrumpido)
        {
            this.appInterrupida = interrumpido;
        }

        public void ResultadosLoad() //establece el valor de las label de info despues de cargar un .ast
        {
            string nombreInformativo = this.openFileDialog1.SafeFileName;
            string nombreInformativo1 = "";
            string nombreInformativo2 = "";
            if (nombreInformativo.Count() > 34)//Controla que el titulo no sea demasiado largo y salga de la form
            {
                for (int i = 0; i < 34; i++)
                {
                    nombreInformativo1 = nombreInformativo1 + Convert.ToString(nombreInformativo[i]);
                }
                for (int i = 34; i < nombreInformativo.Count(); i++)
                {
                    nombreInformativo2 = nombreInformativo2 + Convert.ToString(nombreInformativo[i]);
                }
            }
            string avionesDetectados = "";
            int aircraftDetected = myList.AircraftDetected();
            if (aircraftDetected != 0)
                avionesDetectados = "\n            from " + aircraftDetected +
                " different transponders";
            if (nombreInformativo1 != "")
            {
                labelInformativa.Text = nombreInformativo1 + "\n" + nombreInformativo2 + " correctly\nloaded\n\n" +
                    myList.GetNumList() + " packages readed" + avionesDetectados + "\n\nbetween " + myList.GetPlanI(0).ConvertUTC("SinDecimas") +
                    " hours\n         and " + myList.GetPlanI(myList.GetNumList() - 1).ConvertUTC("SinDecimas") + " hours";
                labelInformativa.Location = new Point(labelInfoLoc.X - 20, labelInfoLoc.Y - 20);
            }
            else
            {
                labelInformativa.Text = this.openFileDialog1.SafeFileName + " correctly\nloaded\n\n" +
                    myList.GetNumList() + " packages readed" + avionesDetectados + "\n\nbetween " + myList.GetPlanI(0).ConvertUTC("SinDecimas") +
                    " hours\n         and " + myList.GetPlanI(myList.GetNumList() - 1).ConvertUTC("SinDecimas") + " hours";
                labelInformativa.Location = new Point(labelInfoLoc.X, labelInfoLoc.Y);
            }
        }

        public void CrearFormInformativa(string info1, string info2, int formWidth, int Xbutton, int Xlabel1, int Xlabel2)//Valores puestos a mano para cuadradrar el texto segun la longitud del propio texto
        {
            FormInformativa formI = new FormInformativa();
            formI.ImportarInformacion(info1, info2, formWidth, Xbutton, Xlabel1, Xlabel2);
            formI.ShowDialog();
        }

        public bool CrearFormInformativaYesNo(string info1, string info2, int formWidth, int Xbutton1, int Xbutton2, int Xlabel1, int Xlabel2)//Valores puestos a mano para cuadradrar el texto segun la longitud del propio texto
        {
            FormInformativaYesNo formI = new FormInformativaYesNo();
            formI.ImportarInformacion(info1, info2, formWidth, Xbutton1, Xbutton2, Xlabel1, Xlabel2);
            formI.ShowDialog();
            return (formI.ExportarRespuesta());
        }

        public bool CerrarMain()//Si se carga fichero desde el main, y se carga correctamente, cierra el main
        {
            return cerrarMain;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}