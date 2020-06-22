using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Codigo;
using System.IO;
using SpreadsheetLight;

namespace ED_SMR_MLAT_Performance
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        LectorMensaje myList;
        LectorMensaje myListDGPS;
        LectorMensaje myListConDescartados;
        CListaMap myLmap;
        Bitmap myBitmap;
        Bitmap myBitmapDescartados;
        Avaluador myAvaluador;
        Avaluador myAvaluadorDGPS;
        List<List<int>> aircraftDetected;//Matriz con los aviones estructurados segun su ICAO Adress
        string nombreFichero = "";//nombre del .ast que se esta leyendo
        bool saveAllCorrecto = false; //Controla si se ha guardado todo correctamente
        bool[] saveAllCorrectoPorSeparado = new bool[2];//Controla si se ha guardado la foto y todos los excels, por lo que ya no haria falta clicar en SaveAll "general"
        bool segmentation = false;//Controla si se ha clicado en segmentation
        bool zoom = false; //En el mapa esta false cuando esta en modo zoom in (predeterminado) 
        bool HideBack = false; //En el mapa controla si se esconde o no el mapa. False si se muestra el mapa
        bool MLATandDGPS = false; //En el mapa controla si se quiere ver o no DGPS + MLAT
        bool DGPS = false; //En el mapa controla si se quiere ver o no DGPS
        int grosor = 1; //Grosor del trazo en SaveBitmap
        bool DGPSresults = false; //False cuando se ve el summary de MLAT, true con summary D-GPS
        bool mapaDescartados = false; //Si estamos viendo el mapa de descartados no hay opciones de zoom, segmentacion, etc.
        double[,] dimensionesDescartado; //Cambia la dimensiones del mapa de descartados en caso de que clique "View Whole"
        double proporcionDescartados; //Cambia la proporcion del mapa de descartados en caso de que clique "View Whole"
        //bool seccionInicializada = true; //Regula el evento clic sobre panelMap, inicia y finaliza la creación de una sección
        //List<double[]> puntosSecciones = new List<double[]>();
        double Proporcion = 5; //Proporcion del mapa que se enseña
        double OffsetX = 10000; //Offset del origen de coordenadas del mapa (es igual al origen de coordenadas de la MLAT)
        double OffsetY = 7000;

        private void MainForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            panelContenedorMapa.Visible = false;
            panelContenedorResultados.Visible = true;
            panelContenedorDescartados.Visible = false;
            saveAllCorrecto = false;
            myAvaluador = new Avaluador(myList);
            aircraftDetected = myList.AircraftICAODetected();
            myLmap = new CListaMap();
            if (myListDGPS.GetNumList()!=0)//Si no hay DGPS se ocultan algunas prestaciones
            {
                myAvaluadorDGPS = new Avaluador(myList, myListDGPS);
            }
            else
            {
                DGPSresultsButton.Visible = false;
                SaveScatterButton.Visible = false;
                ButtonDGPS.Visible = false;
                ButtonMLAT_DGPS.Visible = false;
            }
            InicializarBotones();
            InicializarTablas();
            InicializarMapas();
        }

        public void RecargarMainForm()//Recarga los valores de la form despues de añadir / descartar un vehiculo
        {
            Cursor.Current = Cursors.WaitCursor;
            myList.ClearList();
            myList = myListConDescartados.DescartarVehiculosSquitter();
            if (myListDGPS.GetNumList() != 0)
            {
                myListDGPS.SetIndicesDGPS(myList);
            }

            saveAllCorrecto = false;
            saveAllCorrectoPorSeparado = new bool[2];
            myAvaluador = new Avaluador(myList);
            aircraftDetected = myList.AircraftICAODetected();
            if (myListDGPS.GetNumList() != 0)
            {
                myAvaluadorDGPS = new Avaluador(myList, myListDGPS);
            }
            string icao="";
            string textoBoton="";
            if (AddRemoveDiscardButton.Text=="Add")
            {
                icao = Convert.ToString(tablaGridNoDescartados[0, tablaGridNoDescartados.CurrentRow.Index].Value);
                textoBoton = "added";
            }
            else if(AddRemoveDiscardButton.Text == "Remove")
            {
                icao = Convert.ToString(tablaGridDescartados[0, tablaGridDescartados.CurrentRow.Index].Value);
                textoBoton = "removed";
            }
            InicializarBotones();
            InicializarTablas();
            InicializarMapas();
            panelMapDescartados.Invalidate();

            FormInformativa formI = new FormInformativa();
            formI.ImportarInformacion("Vehicle with ICAO Address", icao + " correctly " + textoBoton + ".", 276, 74, 20, 24);
            formI.ShowDialog();
        }

        public void ResetearBoleanos()//Pone los boleanos en su estado original
        {
            MLATandDGPS = false;
            DGPS = false;
            segmentation = false;
            zoom = false;
            HideBack = false;
            DGPSresults = false;
        }

        public void SetMensaje(LectorMensaje lista, LectorMensaje listaDGPS, LectorMensaje listaSinDescartados) 
        {
            this.myList = lista; //Carga la lista MLAT de la Form1 a MainForm
            this.myListDGPS = listaDGPS; //Carga la lista DGPS de la Form1 a MainForm
            this.myListConDescartados = listaSinDescartados; //Carga la lista MLAT sin vehiculos descartados de la Form1 a MainForm
        }

        public void SetNombreFichero(string nombre)//nombre .ast que se esta leyendo
        {
            this.nombreFichero = nombre;
        }

        public void InicializarBotones()//Pone el texto y booleanos en el estado inicial
        {
            buttonHideBackground.Text = "Hide Background";
            buttonViewSegmentation.Text = "View Segmentation";
            ButtonZoom.Text = "Zoom Out";
            ButtonDGPS.Text = "View only D-GPS";
            segmentation = false;//Controla si se ha clicado en segmentation
            zoom = false; //En el mapa esta false cuando esta en modo zoom in (predeterminado)
            MLATandDGPS = false; //En el mapa controla si se quiere ver o no DGPS mas MLAT
            DGPS = false; //En el mapa controla si se quiere ver o no DGPS
            HideBack = false;
            grosor = 1; //Grosor del trazo en SaveBitmap
            DGPSresultsButton.Text = "View D-GPS Results";
            DGPSresults = false; //False cuando se ve el summary de MLAT, true con summary D-GPS
            AddRemoveDiscardButton.Visible = false;
            ViewWholeButton.Visible = false;
        }

        public void InicializarTablas()//Crea el contenido de las tablas
        {
            CrearTablaParameters();
            CrearTablaSummary(this.SummaryGrid, myAvaluador);
            CrearTablaDescartados(this.tablaGridDescartados);
            tablaGridDescartados.ClearSelection();
            CrearTablaNoDescartados(this.tablaGridNoDescartados);
            tablaGridNoDescartados.ClearSelection();
        }

        public void InicializarMapas()//Crea el contenido de los mapas
        {
            myBitmap = new Bitmap(this.panelMap.Width, this.panelMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            PintarFondoPantalla(myBitmap, this.panelMap.Width, this.panelMap.Height); //Genera el bmp con el mapa de LEBL
            PintarAll(myBitmap, this.panelMap.Width, this.panelMap.Height); //Pinta los paquetes sobre el mapa
            myBitmapDescartados = new Bitmap(this.panelMapDescartados.Width, this.panelMapDescartados.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            dimensionesDescartado = myLmap.GetDIMENSIONES();
            proporcionDescartados = myLmap.GetPROPORCION(this.panelMapDescartados.Height, this.panelMapDescartados.Width);
            PintarFondoPantalla(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height); //Genera el bmp con el mapa de LEBL        
        }

        private void panelMap_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;
                graphics.DrawImage(myBitmap, 0, 0, myBitmap.Width, myBitmap.Height);
                graphics.Dispose();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void PintarFondoPantalla(Bitmap Bit, int width, int height)  //Dibuja el fondo de pantalla con el mapa y lo guarda en bmp
        {
            Graphics graphicsObj;
            graphicsObj = Graphics.FromImage(Bit);
            graphicsObj.Clear(Color.White); // Set Bitmap background color. Black by default
            Pen myPen = new Pen(Color.Gray, 1 / 2);
            if (mapaDescartados)
            {
                PintarMapa(graphicsObj, myPen, width, height);
            }
            else if (!HideBack)
            { 
                PintarMapa(graphicsObj, myPen, width, height);
            }
            graphicsObj.Dispose();
        }

        private void PintarMapa(Graphics graphics, Pen myPen, int width, int height)//Pinta el mapa de LEBL, se usa dentro de PintarFondoPantalla
        {
            double[,] dimensiones = myLmap.GetDIMENSIONES();
            double proporcion;

            if (mapaDescartados)
            {
                dimensiones = dimensionesDescartado;
                proporcion = proporcionDescartados;
            }
            else if (zoom)
            {
                dimensiones[0, 0] = dimensiones[0, 0] - OffsetX;
                dimensiones[1, 0] = dimensiones[1, 0] - OffsetY;
                proporcion= myLmap.GetPROPORCION(height /Proporcion, width / Proporcion);
            }
            else
            {
                proporcion = myLmap.GetPROPORCION(height, width);
            }

            try
            {
                int indiceFichero = 0;
                while (indiceFichero < myLmap.GetNumList())//Pinta el mapa con lineas y polilineas
                {
                    int i = 0;
                    Point[] lineVector = new Point[2];
                    while (i < myLmap.GetPlanI(indiceFichero).GetLinea().GetLength(0))
                    {
                        lineVector[0] = new Point(Convert.ToInt32((myLmap.GetPlanI(indiceFichero).GetLinea()[i, 0] - dimensiones[0, 0]) / proporcion), Convert.ToInt32(((-(myLmap.GetPlanI(indiceFichero).GetLinea()[i, 1] - dimensiones[1, 0]) / proporcion)) + height));
                        //(x - dimensionX) / proporcion; (-(y - dimensionY) / proporcion) + panelAe.Size.Height
                        lineVector[1] = new Point(Convert.ToInt32((myLmap.GetPlanI(indiceFichero).GetLinea()[i, 2] - dimensiones[0, 0]) / proporcion), Convert.ToInt32(((-(myLmap.GetPlanI(indiceFichero).GetLinea()[i, 3] - dimensiones[1, 0]) / proporcion)) + height));
                        graphics.DrawPolygon(myPen, lineVector); //Dibujamos la linea del vector
                        i = i + 1;
                    }

                    i = 0;
                    int longPoli = 0;
                    int iPoli = 0;
                    bool vectorNOAcabado = true;
                    while (i < myLmap.GetPlanI(indiceFichero).GetPoli().GetLength(0))
                    {
                        while ((vectorNOAcabado) && (myLmap.GetPlanI(indiceFichero).GetPoli()[i, 0] != Math.Pow(10, 8)))
                        {
                            longPoli = longPoli + 1;
                            i = i + 1;
                            if (i == myLmap.GetPlanI(indiceFichero).GetPoli().GetLength(0))
                            {
                                vectorNOAcabado = false;
                                i = i - 1;
                            }
                        }
                        if (vectorNOAcabado)
                        {
                            i = i - longPoli;
                        }
                        else i = i - longPoli + 1;

                        Point[] poliVector = new Point[longPoli];
                        while (iPoli < longPoli)
                        {
                            poliVector[iPoli] = new Point(Convert.ToInt32((myLmap.GetPlanI(indiceFichero).GetPoli()[i, 0] - dimensiones[0, 0]) / proporcion), Convert.ToInt32((-(myLmap.GetPlanI(indiceFichero).GetPoli()[i, 1] - dimensiones[1, 0]) / proporcion) + height));
                            iPoli = iPoli + 1;
                            i = i + 1;
                        }
                        i = i + 1;
                        iPoli = 0;
                        longPoli = 0;
                        graphics.DrawLines(myPen, poliVector); //Dibujamos la linea del vector
                    }
                    indiceFichero = indiceFichero + 1;
                }
                myPen.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void PintarAll(Bitmap Bit, int width, int height)//Pinta todos los paquetes de myList
        {
            Graphics graphics;
            graphics = Graphics.FromImage(Bit);
            double[,] dimensiones = myLmap.GetDIMENSIONES();
            double proporcion;

            if (zoom)
            {
                dimensiones[0, 0] = dimensiones[0, 0] - OffsetX;
                dimensiones[1, 0] = dimensiones[1, 0] - OffsetY;
                proporcion = myLmap.GetPROPORCION(height / Proporcion, width / Proporcion);
            }
            else
            {
                proporcion = myLmap.GetPROPORCION(height, width);
            }

            try
            {
                int cont = 0;
                Pen myPen;
                while (cont < myList.GetNumList())
                {
                    if (segmentation)
                    {
                        if ((myList.GetPlanI(cont).GetZona() == 01) || (myList.GetPlanI(cont).GetZona() == 02))
                        {
                            myPen = new Pen(Color.BlueViolet, grosor);
                        }
                        else if ((myList.GetPlanI(cont).GetZona() == 11) || (myList.GetPlanI(cont).GetZona() == 12) || (myList.GetPlanI(cont).GetZona() == 13))
                        {
                            myPen = new Pen(Color.Yellow, grosor);
                        }
                        else if ((myList.GetPlanI(cont).GetZona() == 21) || (myList.GetPlanI(cont).GetZona() == 22))
                        {
                            myPen = new Pen(Color.Black, grosor);
                        }
                        else if ((myList.GetPlanI(cont).GetZona() == 31) || (myList.GetPlanI(cont).GetZona() == 32))
                        {
                            myPen = new Pen(Color.Red, grosor);
                        }
                        else if (myList.GetPlanI(cont).GetZona() == 4)
                        {
                            myPen = new Pen(Color.Green, grosor);
                        }
                        else
                        {
                            myPen = new Pen(Color.DeepPink, grosor);
                        }
                    }
                    else myPen = new Pen(Color.Blue, grosor);

                    if ((myList.GetPlanI(cont).GetPosicion()[0] != Math.Pow(10, 8)) && (myList.GetPlanI(cont).GetPosicion()[1] != Math.Pow(10, 8)))
                    {
                        graphics.DrawRectangle(myPen, Convert.ToInt32((myList.GetPlanI(cont).GetPosicion()[0] - dimensiones[0, 0]) / proporcion), Convert.ToInt32((-(myList.GetPlanI(cont).GetPosicion()[1] - dimensiones[1, 0]) / proporcion) + height), 1, 1);
                    }
                    cont = cont + 1;
                }
                graphics.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void PintarAllDGPS(Bitmap Bit, int width, int height)//Pinta todos los paquetes de myListDGPS
        {
            Graphics graphics;
            graphics = Graphics.FromImage(Bit);
            double[,] dimensiones = myLmap.GetDIMENSIONES();
            double proporcion;

            if (zoom)
            {
                dimensiones[0, 0] = dimensiones[0, 0] - OffsetX;
                dimensiones[1, 0] = dimensiones[1, 0] - OffsetY;
                proporcion = myLmap.GetPROPORCION(height / Proporcion, width / Proporcion);
            }
            else
            {
                proporcion = myLmap.GetPROPORCION(height, width);
            }

            try
            {
                int cont = 0;
                Pen myPen;
                Pen myPenMLAT;
                Pen myPenUnion;
                double diferencia;
                while (cont < myListDGPS.GetNumList())
                {
                    myPen = new Pen(Color.LimeGreen, grosor);
                    myPenMLAT = new Pen(Color.Blue, grosor);
                    if (MLATandDGPS)
                    {
                        if (myListDGPS.GetPlanI(cont).GetIndiceDGPS() != -1)
                        {
                            diferencia = Math.Sqrt(Math.Pow(myListDGPS.GetPlanI(cont).GetPosicion()[0] - myList.GetPlanI(myListDGPS.GetPlanI(cont).GetIndiceDGPS()).GetPosicion()[0], 2) + Math.Pow(myListDGPS.GetPlanI(cont).GetPosicion()[1] - myList.GetPlanI(myListDGPS.GetPlanI(cont).GetIndiceDGPS()).GetPosicion()[1], 2));
                            if (diferencia < 5) //El color de la union cambia segun el error en posición
                                myPenUnion = new Pen(Color.LawnGreen, grosor);
                            else if (diferencia < 7.5)
                                myPenUnion = new Pen(Color.LightSkyBlue, grosor);
                            else if (diferencia < 12)
                                myPenUnion = new Pen(Color.Orange, grosor);
                            else myPenUnion = new Pen(Color.Red, grosor);
                            graphics.DrawRectangle(myPen, Convert.ToInt32((myListDGPS.GetPlanI(cont).GetPosicion()[0] - dimensiones[0, 0]) / proporcion), Convert.ToInt32((-(myListDGPS.GetPlanI(cont).GetPosicion()[1] - dimensiones[1, 0]) / proporcion) + height), 1, 1);
                            graphics.DrawRectangle(myPenMLAT, Convert.ToInt32((myList.GetPlanI(myListDGPS.GetPlanI(cont).GetIndiceDGPS()).GetPosicion()[0] - dimensiones[0, 0]) / proporcion), Convert.ToInt32((-(myList.GetPlanI(myListDGPS.GetPlanI(cont).GetIndiceDGPS()).GetPosicion()[1] - dimensiones[1, 0]) / proporcion) + height), 1, 1);
                            graphics.DrawLine(myPenUnion, Convert.ToInt32((myListDGPS.GetPlanI(cont).GetPosicion()[0] - dimensiones[0, 0]) / proporcion), Convert.ToInt32((-(myListDGPS.GetPlanI(cont).GetPosicion()[1] - dimensiones[1, 0]) / proporcion) + height), Convert.ToInt32((myList.GetPlanI(myListDGPS.GetPlanI(cont).GetIndiceDGPS()).GetPosicion()[0] - dimensiones[0, 0]) / proporcion), Convert.ToInt32((-(myList.GetPlanI(myListDGPS.GetPlanI(cont).GetIndiceDGPS()).GetPosicion()[1] - dimensiones[1, 0]) / proporcion) + height)); //Linea que une DGPS con MLAT
                        }
                    }
                    else
                    {
                        if (segmentation)
                        {
                            if ((myListDGPS.GetPlanI(cont).GetZona() == 01) || (myListDGPS.GetPlanI(cont).GetZona() == 02))
                            {
                                myPen = new Pen(Color.BlueViolet, grosor);
                            }
                            else if ((myListDGPS.GetPlanI(cont).GetZona() == 11) || (myListDGPS.GetPlanI(cont).GetZona() == 12) || (myListDGPS.GetPlanI(cont).GetZona() == 13))
                            {
                                myPen = new Pen(Color.Yellow, grosor);
                            }
                            else if ((myListDGPS.GetPlanI(cont).GetZona() == 21) || (myListDGPS.GetPlanI(cont).GetZona() == 22))
                            {
                                myPen = new Pen(Color.Black, grosor);
                            }
                            else if ((myListDGPS.GetPlanI(cont).GetZona() == 31) || (myListDGPS.GetPlanI(cont).GetZona() == 32))
                            {
                                myPen = new Pen(Color.Red, grosor);
                            }
                            else if (myListDGPS.GetPlanI(cont).GetZona() == 4)
                            {
                                myPen = new Pen(Color.Green, grosor);
                            }
                            else
                            {
                                myPen = new Pen(Color.DeepPink, grosor);
                            }
                        }
                        graphics.DrawRectangle(myPen, Convert.ToInt32((myListDGPS.GetPlanI(cont).GetPosicion()[0] - dimensiones[0, 0]) / proporcion), Convert.ToInt32((-(myListDGPS.GetPlanI(cont).GetPosicion()[1] - dimensiones[1, 0]) / proporcion) + height), 1, 1);
                    }
                    cont = cont + 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void PintarAllDescartados(Bitmap Bit, int width, int height, string filtro)//Pinta todos los paquetes de myListConDescartados
        {
            Graphics graphics;
            graphics = Graphics.FromImage(Bit);
            double[,] dimensiones = dimensionesDescartado;
            double proporcion = proporcionDescartados;

            try
            {
                int cont = 0;
                Pen myPen = new Pen(Color.Red, grosor);
                while (cont < myListConDescartados.GetNumList())
                {
                    if (myListConDescartados.GetPlanI(cont).GetICAOAdress() == filtro)
                    {
                        if ((myListConDescartados.GetPlanI(cont).GetPosicion()[0] != Math.Pow(10, 8)) && (myListConDescartados.GetPlanI(cont).GetPosicion()[1] != Math.Pow(10, 8)))
                        {
                            graphics.DrawRectangle(myPen, Convert.ToInt32((myListConDescartados.GetPlanI(cont).GetPosicion()[0] - dimensiones[0, 0]) / proporcion), Convert.ToInt32((-(myListConDescartados.GetPlanI(cont).GetPosicion()[1] - dimensiones[1, 0]) / proporcion) + height), 1, 1);
                        }
                    }
                    cont = cont + 1;
                }
                graphics.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        public void PintarSecciones(Bitmap Bit, int width, int height) //Leyendo el .txt de secciones lo pinta sobre el mapa
        {
            Graphics graphics;
            graphics = Graphics.FromImage(Bit);
            double[,] dimensiones = myLmap.GetDIMENSIONES();
            double proporcion;
            if (zoom)
            {
                dimensiones[0, 0] = dimensiones[0, 0] - OffsetX;
                dimensiones[1, 0] = dimensiones[1, 0] - OffsetY;
                proporcion = myLmap.GetPROPORCION(height / Proporcion, width / Proporcion);
            }
            else
            {
                proporcion = myLmap.GetPROPORCION(height, width);
            }

            string nombreFichero = "Secciones";
            StreamReader leer = new StreamReader(nombreFichero + ".txt");
            string linea = leer.ReadLine();
            int numZona = 1;
            while (linea != null)
            {
                List<Point> points = new List<Point>();
                while (linea != "/")
                {
                    string[] coordenadas = linea.Split(' ');
                    int X = Convert.ToInt32((Convert.ToDouble(coordenadas[0]) - dimensiones[0, 0]) / proporcion);
                    int Y = Convert.ToInt32((-(Convert.ToDouble(coordenadas[1]) - dimensiones[1, 0]) / proporcion) + height);
                    points.Add(new Point(X, Y));
                    linea = leer.ReadLine();
                }
                Point[] Point = new Point[points.Count()];
                for (int i = 0; i < points.Count(); i++)
                    Point[i] = points[i];

                int opacidad = 50;//De 0 a 255
                Color myColor = new Color();
                if ((numZona >= 1) && (numZona <= 5))//RWY
                    myColor = Color.FromArgb(opacidad, Color.Yellow);
                else if ((numZona >= 6) && (numZona <= 16))//2 stand
                    myColor = Color.FromArgb(opacidad, Color.Black);
                else if ((numZona >= 17) && (numZona <= 25))//3 apron
                    myColor = Color.FromArgb(opacidad, Color.Red);
                else if ((numZona >= 26) && (numZona <= 36))//4 taxi
                    myColor = Color.FromArgb(opacidad, Color.Green);
                else if ((numZona >= 37) && (numZona <= 48))//01,02 areas tipo 4,5
                    myColor = Color.FromArgb(opacidad, Color.BlueViolet);

                graphics.FillPolygon(new SolidBrush(myColor), Point);
                points.Clear();
                linea = leer.ReadLine();
                numZona++;
            }
            graphics.Dispose();
            leer.Close();
        }

        private Bitmap SaveMap()//Prepara el Bitmap para ser exportado
        {
            int[] calidad;
            if (zoom)
            {
                calidad = new int[2] { 6000, 4400 }; //en PintarAll el grosor se pone en 2 si calidad[0]=6000 
                grosor = 2;
            }
            else
            {
                calidad = new int[2] { 2500, 2200 };
                grosor = 3;
            }
            Bitmap saveBitmap = new Bitmap(calidad[0], calidad[1], System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            PintarFondoPantalla(saveBitmap, calidad[0], calidad[1]);
            if (DGPS)
            {
                if (!MLATandDGPS)
                {
                    PintarAllDGPS(saveBitmap, calidad[0], calidad[1]);
                }
                else PintarAllDGPS(saveBitmap, calidad[0], calidad[1]);
            }
            else if (MLATandDGPS)
            {
                PintarAllDGPS(saveBitmap, calidad[0], calidad[1]); //Pinta los paquetes DGPS sobre el mapa
            }
            else PintarAll(saveBitmap, calidad[0], calidad[1]);

            if (segmentation)
            {
                PintarSecciones(saveBitmap, calidad[0], calidad[1]);
            }    
            return (saveBitmap);
        }

        private void ButtonSaveMap_Click(object sender, EventArgs e)//Boton exportar el mapa LEBL a .png
        {
            Bitmap saveBitmap = SaveMap();

            string saveName = nombreFichero;//Añadimos caracteristicas actuales al nombre del fichero a guardar
            if (MLATandDGPS)
                saveName = saveName + "_MLATandDGPS";
            else if (!DGPS)
                saveName = saveName + "_MLAT";
            else if (DGPS)
                saveName = saveName + "_DGPS";
                  
            if (segmentation)
                saveName = saveName + "_Segmented";
            if (zoom)
                saveName = saveName + "_ZoomOut";
            if (HideBack)
                saveName = saveName + "_WithOutBackground";

            saveFileDialog1.FileName = saveName + "_Map.png";
            saveFileDialog1.Filter = "File documents (.png)|*.png";
            saveFileDialog1.ShowDialog();
            try
            {
                if (this.saveFileDialog1.FileName.Equals("") == false)
                {
                    if (this.saveFileDialog1.FileName.Contains(@"\"))//Controla si se ha escogido un directorio o se ha cancelado/cerrado el save dialog
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        saveBitmap.Save(saveFileDialog1.FileName);
                        saveAllCorrectoPorSeparado[0] = true;
                        if (saveAllCorrectoPorSeparado[1])//Si el del excel tambien es true, el general tambien pasa a ser true
                            saveAllCorrecto = true;
                    }
                }
                else CrearFormInformativa("Please write a", "name first.", 187, 30, 27, 41);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void panelMap_MouseClick(object sender, MouseEventArgs e) //Devuelve la posicion respecto MLAT al clicar en pantalla
        {
            int xp = e.X;
            int yp = e.Y;
            double[,] dimensiones = myLmap.GetDIMENSIONES();
            double proporcion;
            if (zoom)
            {
                dimensiones[0, 0] = dimensiones[0, 0] - OffsetX;
                dimensiones[1, 0] = dimensiones[1, 0] - OffsetY;
                proporcion = myLmap.GetPROPORCION(this.panelMap.Height / Proporcion, this.panelMap.Width / Proporcion);
            }
            else
            {
                proporcion = myLmap.GetPROPORCION(this.panelMap.Height, this.panelMap.Width);
            }

            double Xr = Math.Round(xp * proporcion + dimensiones[0, 0], 2);
            double Yr = Math.Round(dimensiones[1, 0] + (this.panelMap.Height - yp) * proporcion, 2);
            labelCursor.Text = "Cursor: " + Xr + "m, " + Yr + "m (X,Y)";
            
            //SE PUEDE BORRAR EL EVENTO AL FINAL SI NO SE IMPLEMENTA UNA UTILIDAD NO TEMPORAL//
            //double[] result = { Xr, Yr };
            //puntosSecciones.Add(result);         
        }

        private void panelMap_MouseMove(object sender, MouseEventArgs e)//Estblece los valores X, Y del raton sobre el mapa
        {
            int xp = e.X;
            int yp = e.Y;
            double[,] dimensiones = myLmap.GetDIMENSIONES();
            double proporcion;
            if (zoom)
            {
                dimensiones[0, 0] = dimensiones[0, 0] - OffsetX;
                dimensiones[1, 0] = dimensiones[1, 0] - OffsetY;
                proporcion = myLmap.GetPROPORCION(this.panelMap.Height / Proporcion, this.panelMap.Width / Proporcion);
            }
            else
            {
                proporcion = myLmap.GetPROPORCION(this.panelMap.Height, this.panelMap.Width);
            }

            double Xr = Math.Round(xp * proporcion + dimensiones[0, 0], 2);
            double Yr = Math.Round(dimensiones[1, 0] + (this.panelMap.Height - yp) * proporcion, 2);
            labelCursor.Text = "Cursor: " + Xr + "m, " + Yr + "m (X,Y)";
        }

        private void panelMap_MouseLeave(object sender, EventArgs e)//Pone el cursor a 0 cuando se abandona el panel
        {
            labelCursor.Text = "Cursor: --.--m, --.--m (X,Y)";
        }

        private void buttonViewSegmentation_Click(object sender, EventArgs e)//Mostrar las zonas del mapa
        {
            grosor = 1;
            if (!segmentation)
            {
                segmentation = true;
                buttonViewSegmentation.Text = "Hide Segmentation";
                myBitmap = new Bitmap(this.panelMap.Width, this.panelMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                PintarFondoPantalla(myBitmap, this.panelMap.Width, this.panelMap.Height);
                PintarSecciones(myBitmap, this.panelMap.Width, this.panelMap.Height);//Pinta encima del mapa las diferentes secciones
                if (DGPS)
                {
                    if (!MLATandDGPS)
                    {
                        PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                    }
                    else PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                }
                else if (MLATandDGPS)
                {
                    PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height); //Pinta los paquetes DGPS sobre el mapa
                }
                else PintarAll(myBitmap, this.panelMap.Width, this.panelMap.Height);

                panelMap.Invalidate();     
            }
            else
            {
                buttonViewSegmentation.Text = "View Segmentation";
                segmentation = false;
                myBitmap = new Bitmap(this.panelMap.Width, this.panelMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                PintarFondoPantalla(myBitmap, this.panelMap.Width, this.panelMap.Height);
                if (DGPS)
                {
                    if (!MLATandDGPS)
                    {
                        PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                    }
                    else PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                }
                else if (MLATandDGPS)
                {
                    PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height); //Pinta los paquetes DGPS sobre el mapa
                }
                else PintarAll(myBitmap, this.panelMap.Width, this.panelMap.Height);

                panelMap.Invalidate();
            }
        }

        private void ButtonZoom_Click(object sender, EventArgs e) //Hace zoom in y zoom out en el mapa
        {
            grosor = 1;
            if (!zoom)
            {
                zoom = true;
                ButtonZoom.Text = "Zoom In";
                myBitmap = new Bitmap(this.panelMap.Width, this.panelMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                PintarFondoPantalla(myBitmap, this.panelMap.Width, this.panelMap.Height);
                if (DGPS)
                {
                    if (!MLATandDGPS)
                    {
                        PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                    }
                    else PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                }
                else if (MLATandDGPS)
                {
                    PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height); //Pinta los paquetes DGPS sobre el mapa
                }
                else PintarAll(myBitmap, this.panelMap.Width, this.panelMap.Height);

                if  (segmentation)
                {
                    PintarSecciones(myBitmap, this.panelMap.Width, this.panelMap.Height);//Pinta encima del mapa las diferentes secciones
                }
                panelMap.Invalidate();
            }
            else
            {
                zoom = false;
                ButtonZoom.Text = "Zoom Out";
                myBitmap = new Bitmap(this.panelMap.Width, this.panelMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                PintarFondoPantalla(myBitmap, this.panelMap.Width, this.panelMap.Height);
                if (DGPS)
                {
                    if (!MLATandDGPS)
                    {
                        PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                    }
                    else PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                }
                else if (MLATandDGPS)
                {
                    PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height); //Pinta los paquetes DGPS sobre el mapa
                }
                else PintarAll(myBitmap, this.panelMap.Width, this.panelMap.Height);

                if (segmentation)
                {
                    PintarSecciones(myBitmap, this.panelMap.Width, this.panelMap.Height);//Pinta encima del mapa las diferentes secciones
                }
                panelMap.Invalidate();
            }
        }

        private void ButtonDGPS_Click(object sender, EventArgs e) //Muestra solo el mensaje DGPS
        {
            MLATandDGPS = false;
            grosor = 1;

            if (!DGPS)
            {
                DGPS = true;
                myBitmap = new Bitmap(this.panelMap.Width, this.panelMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                PintarFondoPantalla(myBitmap, this.panelMap.Width, this.panelMap.Height);
                PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                if (segmentation)
                {
                    PintarSecciones(myBitmap, this.panelMap.Width, this.panelMap.Height);//Pinta encima del mapa las diferentes secciones
                }
                panelMap.Invalidate();
                ButtonDGPS.Text = "View only MLAT";
            }
            else
            {
                DGPS = false;
                myBitmap = new Bitmap(this.panelMap.Width, this.panelMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                PintarFondoPantalla(myBitmap, this.panelMap.Width, this.panelMap.Height);
                PintarAll(myBitmap, this.panelMap.Width, this.panelMap.Height); //Pinta los paquetes sobre el mapa
                if (segmentation)
                {
                    PintarSecciones(myBitmap, this.panelMap.Width, this.panelMap.Height);//Pinta encima del mapa las diferentes secciones
                }
                panelMap.Invalidate();
                ButtonDGPS.Text = "View only D-GPS";
            }
        }

        private void ButtonMLAT_DGPS_Click(object sender, EventArgs e) //Muestra el mensaje DGPS y el trafico MLAT correspondiente.
        {
            grosor = 1;
            if (!MLATandDGPS)
            {
                MLATandDGPS = true;
                DGPS = true;
                ButtonDGPS.Text = "View only MLAT";
                myBitmap = new Bitmap(this.panelMap.Width, this.panelMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                PintarFondoPantalla(myBitmap, this.panelMap.Width, this.panelMap.Height);
                PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height); //Pinta los paquetes DGPS sobre el mapa
                if (segmentation)
                {
                    PintarSecciones(myBitmap, this.panelMap.Width, this.panelMap.Height);//Pinta encima del mapa las diferentes secciones
                }
                panelMap.Invalidate();
            }
        }

        private void buttonHideBackground_Click_1(object sender, EventArgs e)//Esconde el mapa de LEBL
        {
            grosor = 1;
            if (!HideBack)
            {
                HideBack = true;
                buttonHideBackground.Text = "Show Background";
            }
            else
            {
                HideBack = false;
                buttonHideBackground.Text = "Hide Background";
            }

            myBitmap = new Bitmap(this.panelMap.Width, this.panelMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            PintarFondoPantalla(myBitmap, this.panelMap.Width, this.panelMap.Height);
            if (segmentation)
            {
                PintarSecciones(myBitmap, this.panelMap.Width, this.panelMap.Height);//Pinta encima del mapa las diferentes secciones
            }
            if (DGPS)
            {
                if (!MLATandDGPS)
                {
                    PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
                }
                else PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height);
            }
            else if (MLATandDGPS)
            {
                PintarAllDGPS(myBitmap, this.panelMap.Width, this.panelMap.Height); //Pinta los paquetes DGPS sobre el mapa
            }
            else PintarAll(myBitmap, this.panelMap.Width, this.panelMap.Height);

            panelMap.Invalidate();
        }

        private void CrearTablaSummary(DataGridView grid, Avaluador avaluador)  //Update Rate
        {
            grid.RowCount = avaluador.GetUpdateRate().GetLength(0);
            grid.ColumnCount = avaluador.GetUpdateRate().GetLength(1);
            grid.ColumnHeadersVisible = true;
            grid.RowHeadersVisible = true;
            grid.ClearSelection();
            for (int i = 0; i < grid.ColumnCount; i++)//Desactivamos la funcion de SortMode
            {
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            Padding paddingZonasPrincipales = new Padding(20, 0, 0, 0);
            grid.Columns[0].Name = "Updates";
            grid.Columns[1].Name = "Expected";
            grid.Columns[2].Name = "UR (%) (updates/s)";
            grid.Columns[3].Name = "Minimum UR (%)";
            grid.Rows[0].HeaderCell.Value = "Maneouvering Area";
            grid.Rows[1].HeaderCell.Value = "RWY 25L";
            grid.Rows[2].HeaderCell.Value = "RWY 02";
            grid.Rows[3].HeaderCell.Value = "RWY 25R";
            grid.Rows[4].HeaderCell.Value = "Taxi";
            grid.Rows[5].HeaderCell.Value = "Apron";
            grid.Rows[6].HeaderCell.Value = "Apron T1";
            grid.Rows[7].HeaderCell.Value = "Apron T2";
            grid.Rows[8].HeaderCell.Value = "Stands";
            grid.Rows[9].HeaderCell.Value = "Stands T1";
            grid.Rows[10].HeaderCell.Value = "Stands T2";
            grid.Rows[11].HeaderCell.Value = "Airborne";
            grid.Rows[12].HeaderCell.Value = "Type 4";
            grid.Rows[13].HeaderCell.Value = "Type 5";
            grid.Rows[14].HeaderCell.Value = "Rest";
            grid.Rows[1].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[2].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[3].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[4].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[6].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[7].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[9].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[10].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[12].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[13].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[14].HeaderCell.Style.Padding = paddingZonasPrincipales;
            for (int i = 0; i < avaluador.GetUpdateRate().GetLength(0); i++)
            {
                for (int j = 0; j < avaluador.GetUpdateRate().GetLength(1); j++)
                {
                    if ((j == 2) || (j == 3))//Redondea las columnas UR y minimum UR
                    {
                        grid[j, i].Value = Math.Round(avaluador.GetUpdateRate()[i, j], 3);
                    }
                    else grid[j, i].Value = Math.Floor(avaluador.GetUpdateRate()[i, j]);//Redondea a la baja los expected updates

                    if (j == 2)//Si UR = 0, no hay UR
                    {
                        if (avaluador.GetUpdateRate()[i, j] == 0)
                        {
                            grid[j, i].Value = null;
                        }
                    }

                    if ((i == 0) || (i == 5) || (i == 8) || (i == 11))//Controla si el fondo es verde o rojo segun cumplan o no con el minimo
                    {
                        if (j == 2)
                        {
                            if (avaluador.GetUpdateRate()[i, j] != 0)
                            {
                                if (avaluador.GetUpdateRate()[i, j] >= avaluador.GetUpdateRate()[i, j + 1])
                                {
                                    grid[j, i].Style.BackColor = Color.Green;
                                }
                                else grid[j, i].Style.BackColor = Color.Red;
                            }
                        }
                    }

                    Font fuentePrincipal = new Font("Century Gothic", 12, FontStyle.Bold);//Ponemos en negrita las filas principales
                    if ((i == 0) || (i == 5) || (i == 8) || (i == 11))
                    {
                        grid[j, i].Style.Font = fuentePrincipal;
                    }
                    else
                    {
                        if (j == 3)//Quitamos el 0 de la columna minimum value para las filas no principles;
                        {
                            grid[j, i].Value = null;
                        }
                    }
                }
            }
        }

        private void CrearTablaPositionAccuracy(DataGridView grid, Avaluador avaluador)  //Position Accuracy
        {
            grid.RowCount = avaluador.GetPositionAccuracy().GetLength(0);
            grid.ColumnCount = avaluador.GetPositionAccuracy().GetLength(1);
            grid.ColumnHeadersVisible = true;
            grid.RowHeadersVisible = true;
            grid.ClearSelection();
            for (int i = 0; i < grid.ColumnCount; i++)//Desactivamos la funcion de SortMode
            {
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            Padding paddingZonasPrincipales = new Padding(20, 0, 0, 0);
            grid.Columns[0].Name = "P95";
            grid.Columns[1].Name = "P99";
            grid.Columns[2].Name = "Max Detected";
            grid.Columns[3].Name = "max P95";
            grid.Columns[4].Name = "max P99";
            grid.Columns[5].Name = "Mean";
            grid.Columns[6].Name = "STD";
            grid.Rows[0].HeaderCell.Value = "Maneouvering Area";
            grid.Rows[1].HeaderCell.Value = "RWY 25L";
            grid.Rows[2].HeaderCell.Value = "RWY 02";
            grid.Rows[3].HeaderCell.Value = "RWY 25R";
            grid.Rows[4].HeaderCell.Value = "Taxi";
            grid.Rows[5].HeaderCell.Value = "Apron";
            grid.Rows[6].HeaderCell.Value = "Apron T1";
            grid.Rows[7].HeaderCell.Value = "Apron T2";
            grid.Rows[8].HeaderCell.Value = "Stands";
            grid.Rows[9].HeaderCell.Value = "Stands T1";
            grid.Rows[10].HeaderCell.Value = "Stands T2";
            grid.Rows[11].HeaderCell.Value = "Airborne (No Data)";
            grid.Rows[1].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[2].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[3].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[4].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[6].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[7].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[9].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[10].HeaderCell.Style.Padding = paddingZonasPrincipales;
            for (int i = 0; i < avaluador.GetPositionAccuracy().GetLength(0); i++)
            {
                for (int j = 0; j < avaluador.GetPositionAccuracy().GetLength(1); j++)
                {
                    if ((j == 0) || (j == 1) || (j == 2) || (j == 5) || (j == 6))//Redondea las columnas P95 y P99, max value, mean y STD
                    {
                        grid[j, i].Value = Math.Round(avaluador.GetPositionAccuracy()[i, j], 2);
                    }
                    else grid[j, i].Value = avaluador.GetPositionAccuracy()[i, j];

                    if ((i == 0) || (i == 5))//Controla si el fondo es verde o rojo segun cumplan o no con el minimo
                    {
                        if ((j == 0)||(j==1))
                        {
                            if (avaluador.GetPositionAccuracy()[i, j] !=-1)
                            {
                                if (avaluador.GetPositionAccuracy()[i, j] <= avaluador.GetPositionAccuracy()[i, j + 3])
                                {
                                    grid[j, i].Style.BackColor = Color.Green;
                                }
                                else grid[j, i].Style.BackColor = Color.Red;
                            }
                        }
                    }
                    else
                    {
                        if ((j == 3) || (j == 4))//Quitamos el 0 de la columna minimum value para las filas no principles
                        {
                            grid[j, i].Value = null;
                        }
                    }

                    if (i == 8)//Controla si el fondo es verde o rojo segun cumplan o no con el minimo
                    {
                        if (j == 2)
                        {
                            if (avaluador.GetPositionAccuracy()[i, j] != -1)
                            {
                                if (avaluador.GetPositionAccuracy()[i, j] <= 20)//max 20m en Stand PosAcc
                                {
                                    grid[j, i].Style.BackColor = Color.Green;
                                }
                                else grid[j, i].Style.BackColor = Color.Red;
                            }
                        }
                    }

                    Font fuentePrincipal = new Font("Century Gothic", 12, FontStyle.Bold);//Ponemos en negrita las filas principales
                    if ((i == 0) || (i == 5) || (i == 8) || (i == 11))
                    {
                        grid[j, i].Style.Font = fuentePrincipal;
                    }

                    if(avaluador.GetPositionAccuracy()[i, j] == -1)//Quitamos el -1 de las zonas que no tenemos informacion
                    {
                        grid[j, i].Value = null;
                    }
                }
            }
        }

        private void CrearTablaProbOfMLATDetection(DataGridView grid, Avaluador avaluador)
        {
            grid.RowCount = avaluador.GetProbOfMLATDetection().GetLength(0);
            grid.ColumnCount = avaluador.GetProbOfMLATDetection().GetLength(1);
            grid.ColumnHeadersVisible = true;
            grid.RowHeadersVisible = true;
            grid.ClearSelection();
            for (int i = 0; i < grid.ColumnCount; i++)//Desactivamos la funcion de SortMode
            {
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            Padding paddingZonasPrincipales = new Padding(20, 0, 0, 0);
            grid.Columns[0].Name = "Detected";
            grid.Columns[1].Name = "Expected";
            grid.Columns[2].Name = "PD (%) (Prob. of Detection)";
            grid.Columns[3].Name = "Minimum PD (%)";
            grid.Rows[0].HeaderCell.Value = "Maneouvering Area";
            grid.Rows[1].HeaderCell.Value = "RWY 25L";
            grid.Rows[2].HeaderCell.Value = "RWY 02";
            grid.Rows[3].HeaderCell.Value = "RWY 25R";
            grid.Rows[4].HeaderCell.Value = "Taxi";
            grid.Rows[5].HeaderCell.Value = "Apron T1";
            grid.Rows[6].HeaderCell.Value = "Apron T2";
            grid.Rows[7].HeaderCell.Value = "Stands";
            grid.Rows[8].HeaderCell.Value = "Stands T1";
            grid.Rows[9].HeaderCell.Value = "Stands T2";
            grid.Rows[10].HeaderCell.Value = "Airborne (Not Required)";
            grid.Rows[1].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[2].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[3].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[4].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[5].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[6].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[8].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[9].HeaderCell.Style.Padding = paddingZonasPrincipales;
            for (int i = 0; i < avaluador.GetProbOfMLATDetection().GetLength(0); i++)
            {
                for (int j = 0; j < avaluador.GetProbOfMLATDetection().GetLength(1); j++)
                {
                    if ((j == 2) || (j == 3))//Redondea las columnas UR y minimum UR
                    {
                        grid[j, i].Value = Math.Round(avaluador.GetProbOfMLATDetection()[i, j], 3);
                    }
                    else grid[j, i].Value = avaluador.GetProbOfMLATDetection()[i, j];

                    if (j == 2)//Si PD = 0, no hay PD
                    {
                        if (avaluador.GetProbOfMLATDetection()[i, j] == 0)
                        {
                            grid[j, i].Value = null;
                        }
                    }

                    Font fuentePrincipal = new Font("Century Gothic", 12, FontStyle.Bold);//Ponemos en negrita las filas principales
                    if ((i == 0) || (i == 7))//Controla si el fondo es verde o rojo segun cumplan o no con el minimo
                    {
                        grid[j, i].Style.Font = fuentePrincipal;
                        if (j == 2)
                        {
                            if (avaluador.GetProbOfMLATDetection()[i, j] != 0)
                            {
                                if (avaluador.GetProbOfMLATDetection()[i, j] >= avaluador.GetProbOfMLATDetection()[i, j + 1])
                                {
                                    grid[j, i].Style.BackColor = Color.Green;
                                }
                                else grid[j, i].Style.BackColor = Color.Red;
                            }
                        }
                    }
                    else if (j == 3)//Quitamos el 0 de la columna minimum value para las filas no principles;
                    {
                        grid[j, i].Value = null;
                    }

                    if (i == 10) //Quitamos el 0 de Apron y Airborne
                    {
                        grid[j, i].Value = null;
                    }
                }
            }
        }

        private void CrearTablaProbOfIdentification(DataGridView grid, Avaluador avaluador)
        {
            grid.RowCount = avaluador.GetProbOfIdentification().GetLength(0);
            grid.ColumnCount = avaluador.GetProbOfIdentification().GetLength(1);
            grid.ColumnHeadersVisible = true;
            grid.RowHeadersVisible = true;
            grid.ClearSelection();
            for (int i = 0; i < grid.ColumnCount; i++)//Desactivamos la funcion de SortMode
            {
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            Padding paddingZonasPrincipales = new Padding(20, 0, 0, 0);
            grid.Columns[0].Name = "Correct";
            grid.Columns[1].Name = "Inorrect";
            grid.Columns[2].Name = "PID (%) (correct/total)";
            grid.Columns[3].Name = "Minimum PID (%)";
            grid.Rows[0].HeaderCell.Value = "Maneouvering Area";
            grid.Rows[1].HeaderCell.Value = "RWY 25L";
            grid.Rows[2].HeaderCell.Value = "RWY 02";
            grid.Rows[3].HeaderCell.Value = "RWY 25R";
            grid.Rows[4].HeaderCell.Value = "Taxi";
            grid.Rows[5].HeaderCell.Value = "Apron";
            grid.Rows[6].HeaderCell.Value = "Apron T1";
            grid.Rows[7].HeaderCell.Value = "Apron T2";
            grid.Rows[8].HeaderCell.Value = "Stands";
            grid.Rows[9].HeaderCell.Value = "Stands T1";
            grid.Rows[10].HeaderCell.Value = "Stands T2";
            grid.Rows[11].HeaderCell.Value = "Airborne";
            grid.Rows[12].HeaderCell.Value = "Type 4";
            grid.Rows[13].HeaderCell.Value = "Type 5";
            grid.Rows[14].HeaderCell.Value = "Rest";
            grid.Rows[15].HeaderCell.Value = "Total";
            grid.Rows[1].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[2].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[3].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[4].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[6].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[7].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[9].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[10].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[12].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[13].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[14].HeaderCell.Style.Padding = paddingZonasPrincipales;
            for (int i = 0; i < avaluador.GetProbOfIdentification().GetLength(0); i++)
            {
                for (int j = 0; j < avaluador.GetProbOfIdentification().GetLength(1); j++)
                {
                    grid[j, i].Value = avaluador.GetProbOfIdentification()[i, j];

                    if (i == 15)//Controla si el fondo es verde o rojo segun cumplan o no con el minimo
                    {
                        if (j == 2)
                        {
                            if (avaluador.GetProbOfIdentification()[i, j - 2] != 0)
                            {
                                if (avaluador.GetProbOfIdentification()[i, j] >= avaluador.GetProbOfIdentification()[i, j + 1])
                                {
                                    grid[j, i].Style.BackColor = Color.Green;
                                }
                                else grid[j, i].Style.BackColor = Color.Red;
                            }
                        }
                    }

                    if (j == 2)//Redondeamos columna PID
                    {
                        grid[j, i].Value = Math.Round(avaluador.GetProbOfIdentification()[i, j], 3);
                    }

                    Font fuentePrincipal = new Font("Century Gothic", 12, FontStyle.Bold);//Ponemos en negrita las filas principales
                    if ((i == 0) || (i == 5) || (i == 8) || (i == 11) || (i == 15))
                    {
                        grid[j, i].Style.Font = fuentePrincipal;
                    }

                    if (i != 15)
                    {
                        if (j == 3)//Quitamos el 0 de la columna minimum value para todas las filas menos total;
                        {
                            grid[j, i].Value = null;
                        }
                    }
                }
            }
            for (int i = 0; i < avaluador.GetProbOfIdentification().GetLength(0); i++)//Si correcto = 0, no hay PID
            {
                if (avaluador.GetProbOfIdentification()[i, 0] == 0)
                {
                    grid[2, i].Value = null;
                }
            }
        }

        private void CrearTablaProbOfFalseDetection(DataGridView grid, Avaluador avaluador)
        {
            grid.RowCount = avaluador.GetProbOfFalseDetection().GetLength(0);
            grid.ColumnCount = avaluador.GetProbOfFalseDetection().GetLength(1);
            grid.ColumnHeadersVisible = true;
            grid.RowHeadersVisible = true;
            grid.ClearSelection();
            for (int i = 0; i < grid.ColumnCount; i++)//Desactivamos la funcion de SortMode
            {
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            Padding paddingZonasPrincipales = new Padding(20, 0, 0, 0);
            grid.Columns[0].Name = "Reports";
            grid.Columns[1].Name = "False Reports";
            grid.Columns[2].Name = "PFD (%) (False/Reports)";
            grid.Columns[3].Name = "Minimum PFD (%)";
            grid.Rows[0].HeaderCell.Value = "Maneouvering Area";
            grid.Rows[1].HeaderCell.Value = "RWY 25L";
            grid.Rows[2].HeaderCell.Value = "RWY 02";
            grid.Rows[3].HeaderCell.Value = "RWY 25R";
            grid.Rows[4].HeaderCell.Value = "Taxi";
            grid.Rows[5].HeaderCell.Value = "Apron";
            grid.Rows[6].HeaderCell.Value = "Apron T1";
            grid.Rows[7].HeaderCell.Value = "Apron T2";
            grid.Rows[8].HeaderCell.Value = "Stands";
            grid.Rows[9].HeaderCell.Value = "Stands T1";
            grid.Rows[10].HeaderCell.Value = "Stands T2";
            grid.Rows[11].HeaderCell.Value = "Airborne";
            grid.Rows[12].HeaderCell.Value = "Type 4";
            grid.Rows[13].HeaderCell.Value = "Type 5";
            grid.Rows[14].HeaderCell.Value = "Total";
            grid.Rows[1].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[2].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[3].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[4].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[6].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[7].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[9].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[10].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[12].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[13].HeaderCell.Style.Padding = paddingZonasPrincipales;
            for (int i = 0; i < avaluador.GetProbOfFalseDetection().GetLength(0); i++)
            {
                for (int j = 0; j < avaluador.GetProbOfFalseDetection().GetLength(1); j++)
                {
                    grid[j, i].Value = avaluador.GetProbOfFalseDetection()[i, j];

                    if (i == 14)//Controla si el fondo es verde o rojo segun cumplan o no con el minimo
                    {
                        if (j == 2)
                        {
                            if (avaluador.GetProbOfFalseDetection()[i, j - 2] != 0)
                            {
                                if (avaluador.GetProbOfFalseDetection()[i, j] <= avaluador.GetProbOfFalseDetection()[i, j + 1])
                                {
                                    grid[j, i].Style.BackColor = Color.Green;
                                }
                                else grid[j, i].Style.BackColor = Color.Red;
                            }
                        }
                    }

                    if (j == 2)//Redondeamos columna PFD, añadimos simbolo %
                    {
                        grid[j, i].Value = Math.Round(avaluador.GetProbOfFalseDetection()[i, j], 3);
                    }

                    Font fuentePrincipal = new Font("Century Gothic", 12, FontStyle.Bold);//Ponemos en negrita las filas principales
                    if ((i == 0) || (i == 5) || (i == 8) || (i == 11) || (i == 14))
                    {
                        grid[j, i].Style.Font = fuentePrincipal;
                    }

                    if (j == 3)//Quitamos el 0 de la columna minimum value para todas las filas menos total;
                    {
                        if (i != 14)
                        {
                            grid[j, i].Value = null;
                        }
                        else grid[j, i].Value = grid[j, i].Value;
                    }
                }
            }
            for (int i = 0; i < avaluador.GetProbOfFalseDetection().GetLength(0); i++)//Si reports = 0, no hay ProbFalseDet
            {
                if (avaluador.GetProbOfFalseDetection()[i, 0] == 0)
                {
                    grid[2, i].Value = null;
                }
            }
        }

        private void CrearTablaProbOfFalseIdentification(DataGridView grid, Avaluador avaluador)
        {
            grid.RowCount = avaluador.GetProbOfFalseIdentification().GetLength(0);
            grid.ColumnCount = avaluador.GetProbOfFalseIdentification().GetLength(1);
            grid.ColumnHeadersVisible = true;
            grid.RowHeadersVisible = true;
            grid.ClearSelection();
            for (int i = 0; i < grid.ColumnCount; i++)//Desactivamos la funcion de SortMode
            {
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            Padding paddingZonasPrincipales = new Padding(20, 0, 0, 0);
            grid.Columns[0].Name = "Total";
            grid.Columns[1].Name = "False";
            grid.Columns[2].Name = "Prob. False ID (%) (false/total)";
            grid.Columns[3].Name = "Minimum Prob. False ID (%)";
            grid.Rows[0].HeaderCell.Value = "Maneouvering Area";
            grid.Rows[1].HeaderCell.Value = "RWY 25L";
            grid.Rows[2].HeaderCell.Value = "RWY 02";
            grid.Rows[3].HeaderCell.Value = "RWY 25R";
            grid.Rows[4].HeaderCell.Value = "Taxi";
            grid.Rows[5].HeaderCell.Value = "Apron";
            grid.Rows[6].HeaderCell.Value = "Apron T1";
            grid.Rows[7].HeaderCell.Value = "Apron T2";
            grid.Rows[8].HeaderCell.Value = "Stands";
            grid.Rows[9].HeaderCell.Value = "Stands T1";
            grid.Rows[10].HeaderCell.Value = "Stands T2";
            grid.Rows[11].HeaderCell.Value = "Airborne";
            grid.Rows[12].HeaderCell.Value = "Type 4";
            grid.Rows[13].HeaderCell.Value = "Type 5";
            grid.Rows[14].HeaderCell.Value = "Rest";
            grid.Rows[15].HeaderCell.Value = "Total";
            grid.Rows[1].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[2].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[3].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[4].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[6].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[7].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[9].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[10].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[12].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[13].HeaderCell.Style.Padding = paddingZonasPrincipales;
            grid.Rows[14].HeaderCell.Style.Padding = paddingZonasPrincipales;
            for (int i = 0; i < avaluador.GetProbOfFalseIdentification().GetLength(0); i++)
            {
                for (int j = 0; j < avaluador.GetProbOfFalseIdentification().GetLength(1); j++)
                {
                    grid[j, i].Value = avaluador.GetProbOfFalseIdentification()[i, j];

                    if (i == 15)//Controla si el fondo es verde o rojo segun cumplan o no con el minimo
                    {
                        if (j == 2)
                        {
                            if (avaluador.GetProbOfFalseIdentification()[i, j - 2] != 0)
                            {
                                if (avaluador.GetProbOfFalseIdentification()[i, j] <= avaluador.GetProbOfFalseIdentification()[i, j + 1])
                                {
                                    grid[j, i].Style.BackColor = Color.Green;
                                }
                                else grid[j, i].Style.BackColor = Color.Red;
                            }
                        }
                    }

                    if (j == 2)//Redondeamos columna PFI
                    {
                        grid[j, i].Value = Math.Round(avaluador.GetProbOfFalseIdentification()[i, j], 3);
                    }

                    Font fuentePrincipal = new Font("Century Gothic", 12, FontStyle.Bold);//Ponemos en negrita las filas principales
                    if ((i == 0) || (i == 5) || (i == 8) || (i == 11) || (i == 15))
                    {
                        grid[j, i].Style.Font = fuentePrincipal;
                    }

                    if (i != 15)
                    {
                        if (j == 3)//Quitamos el 0 de la columna minimum value para todas las filas menos total;
                        {
                            grid[j, i].Value = null;
                        }
                    }
                }
            }
            for (int i = 0; i < avaluador.GetProbOfFalseIdentification().GetLength(0); i++)//Si reports = 0, no hay ProbFalseDet
            {
                if (avaluador.GetProbOfFalseIdentification()[i, 0] == 0)
                {
                    grid[2, i].Value = null;
                }
            }
        }

        private void CrearTablaParameters()
        {
            if (!DGPSresults)
            {
                ParametersGrid.RowCount = 5;
                ParametersGrid.ColumnCount = 1;
                ParametersGrid.ClearSelection();
                ParametersGrid[0, 0].Selected = true;
                ParametersGrid[0, 0].Value = "Update Rate";
                ParametersGrid[0, 1].Value = "Probability of MLAT detection";
                ParametersGrid[0, 2].Value = "Probability of Identification";
                ParametersGrid[0, 3].Value = "Probability of False Detection";
                ParametersGrid[0, 4].Value = "Probability of False Identification";
            }
            else
            {
                ParametersGrid.RowCount = 6;
                ParametersGrid.ColumnCount = 1;
                ParametersGrid.ClearSelection();
                ParametersGrid[0, 0].Selected = true;
                ParametersGrid[0, 0].Value = "Update Rate";
                ParametersGrid[0, 1].Value = "Position Accuracy";
                ParametersGrid[0, 2].Value = "Probability of MLAT detection";
                ParametersGrid[0, 3].Value = "Probability of Identification";
                ParametersGrid[0, 4].Value = "Probability of False Detection";
                ParametersGrid[0, 5].Value = "Probability of False Identification";
            }

        }

        private void ParametersGrid_CellClick(object sender, DataGridViewCellEventArgs e)//Cambia la summary grid cuando se clica otra en parameters grid
        {
            int i = e.RowIndex;
            if (i != -1)
            {
                ParametersGrid.ClearSelection();
                ParametersGrid[0, i].Selected = true;
                LabelTituloSummary.Text = Convert.ToString(ParametersGrid[0, i].Value) + ":";
                SummaryGrid.Rows.Clear();
                SummaryGrid.Columns.Clear();
                if (DGPSresults)
                {
                    if (i == 0)
                        CrearTablaSummary(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 1)
                        CrearTablaPositionAccuracy(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 2)
                        CrearTablaProbOfMLATDetection(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 3)
                        CrearTablaProbOfIdentification(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 4)
                        CrearTablaProbOfFalseDetection(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 5)
                        CrearTablaProbOfFalseIdentification(this.SummaryGrid, myAvaluadorDGPS);
                }
                else
                {
                    if (i == 0)
                        CrearTablaSummary(this.SummaryGrid, myAvaluador);
                    else if (i == 1)
                        CrearTablaProbOfMLATDetection(this.SummaryGrid, myAvaluador);
                    else if (i == 2)
                        CrearTablaProbOfIdentification(this.SummaryGrid, myAvaluador);
                    else if (i == 3)
                        CrearTablaProbOfFalseDetection(this.SummaryGrid, myAvaluador);
                    else if (i == 4)
                        CrearTablaProbOfFalseIdentification(this.SummaryGrid, myAvaluador);
                }
            }
        }

        private void panelContenedorResultados_MouseClick(object sender, MouseEventArgs e)//Quita la seleccion de summary grid si se clica fuera de ella
        {
            //Hay enlace de panelContenedor click a este evento
            SummaryGrid.ClearSelection();
        }

        private void SummaryGrid_Click(object sender, EventArgs e)//Quita la seleccion de summary grid si se clica en la tabla
        {
            //Si se clica en una celda, primero se activa este evento y despues el evento de clicar la celda
            SummaryGrid.ClearSelection();
        }

        private void SummaryGrid_CellClick(object sender, DataGridViewCellEventArgs e)//Si se clica en una celda de summary grid, la selcciona 
        {
            SummaryGrid[e.ColumnIndex, e.RowIndex].Selected = true;
        }

        private void DGPSresultsButton_Click(object sender, EventArgs e)//Controla si se muestran los resultados MLAT o D-GPS
        {
            if(!DGPSresults)
            {
                DGPSresults = true;
                DGPSresultsButton.Text = "View MLAT Results";
                SummaryGrid.Rows.Clear();
                SummaryGrid.Columns.Clear();
                CrearTablaSummary(this.SummaryGrid, myAvaluadorDGPS);
                ParametersGrid.Rows.Clear();
                ParametersGrid.Columns.Clear();
                CrearTablaParameters();
                LabelTituloSummary.Text = "Update Rate:";
            }
            else
            {
                DGPSresults = false;
                DGPSresultsButton.Text = "View D-GPS Results";
                SummaryGrid.Rows.Clear();
                SummaryGrid.Columns.Clear();
                CrearTablaSummary(this.SummaryGrid, myAvaluador);
                ParametersGrid.Rows.Clear();
                ParametersGrid.Columns.Clear();
                CrearTablaParameters();
                LabelTituloSummary.Text = "Update Rate:";
            }
        }

        private SLDocument SaveScatter()//prepara el excel para ser exportado; Exporta solo una tabla (un parametro)
        {
            SLDocument myExcel = new SLDocument();
            myExcel.SetCellValue("A1", "File: " + nombreFichero);//Nombre fichero

            string avionesDetectados = "";
            if (myList.AircraftDetected() != 0)
                avionesDetectados = " from " + myList.AircraftDetected() + " different transponders ";//Esta frase solo aparece si hay MLAT
            myExcel.SetCellValue("A2", myList.GetNumList() + " packages avaluated" + avionesDetectados + "between " + myList.GetPlanI(0).ConvertUTC("SinDecimas") +
                            " hours and " + myList.GetPlanI(myList.GetNumList() - 1).ConvertUTC("SinDecimas") + " hours");//Información del fichero

            myExcel.SetCellValue("A3", "Scatter:");//Titulo de la grid

            DataTable data = new DataTable();     
            data.Columns.Add("X error", typeof(double));//La informacion que contiene esta columna es de tipo double
            data.Columns.Add("Y error", typeof(double));
            List<double[]> diferencias = myAvaluadorDGPS.GetDiferencias();
            int i;
            int columnaActual = 1;

            myExcel.SetCellValue(4, columnaActual, "RWY 25L");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 11)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);//Inserta la tabla en la posicion i,j=5,1 (la primera celda en excel es la 1,1 ó A1
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "RWY 02");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 12)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "RWY 25R");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 13)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "Taxi");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 4)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "Apron T1");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 31)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "Apron T2");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 32)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "Stands T1");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 21)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "Stands T2");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 22)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "Type 4");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 01)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "Type 5");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 02)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.SetCellValue(4, columnaActual, "Rest");
            for (i = 0; i < diferencias.Count(); i++)
            {
                if (diferencias[i][2] == 0)
                {
                    data.Rows.Add(diferencias[i][0], diferencias[i][1]);
                }
            }
            myExcel.ImportDataTable(5, columnaActual, data, true);
            data.Rows.Clear();
            columnaActual = columnaActual + 2;

            myExcel.AutoFitColumn(1, columnaActual - 1, 20); //Adapta el tamaño de las celdas (columnas de 1 a 2, maximo grosor de 20)
            return myExcel;
        }

        private void SaveScatterButton_Click(object sender, EventArgs e) //Guarda la tabla de diferencias a excel para poder hacer el Scatter
        {
            string saveName = nombreFichero;//Añadimos caracteristicas actuales al nombre del fichero a guardar
            saveFileDialogExcel.FileName = nombreFichero + "_Scatter";

            saveFileDialogExcel.Filter = "File documents (.xlsx)|*.xlsx";
            saveFileDialogExcel.ShowDialog();
            try
            {
                if (this.saveFileDialogExcel.FileName.Equals("") == false)
                {
                    if (this.saveFileDialogExcel.FileName.Contains(@"\"))//Controla si se ha escogido un directorio o se ha cancelado/cerrado el save dialog
                    {
                        SLDocument myExcel = SaveScatter();
                        Cursor.Current = Cursors.WaitCursor;
                        myExcel.SaveAs(saveFileDialogExcel.FileName);
                    }
                }
                else CrearFormInformativa("Please write a", "name first.", 187, 30, 27, 41);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private SLDocument SaveSummaryParameter()//prepara el excel para ser exportado; Exporta solo una tabla (un parametro)
        {
            SLDocument myExcel = new SLDocument();
            myExcel.SetCellValue("A1", "File: " + nombreFichero);//Nombre fichero

            string avionesDetectados = "";
            if (myList.AircraftDetected() != 0)
                avionesDetectados = " from " + myList.AircraftDetected() + " different transponders ";//Esta frase solo aparece si hay MLAT
            myExcel.SetCellValue("A2", myList.GetNumList() + " packages avaluated" + avionesDetectados + "between " + myList.GetPlanI(0).ConvertUTC("SinDecimas") +
                            " hours and " + myList.GetPlanI(myList.GetNumList() - 1).ConvertUTC("SinDecimas") + " hours");//Información del fichero

            myExcel.SetCellValue("A3", Convert.ToString(ParametersGrid[0, ParametersGrid.CurrentRow.Index].Value) + ":");//Titulo de la grid

            DataTable data = new DataTable();
            data.Columns.Add(" ", typeof(string)); //Columna de zonas de movimiento (sin titulo)
            for (int j = 0; j < SummaryGrid.ColumnCount; j++)//Copiamos filas y columnas de Summary Grid a data table
            {
                data.Columns.Add(SummaryGrid.Columns[j].Name, typeof(double));//La informacion que contienen estas columnas es de tipo double
            }

            int parameter = ParametersGrid.CurrentRow.Index; //A partir de aqui el codigo diferencia entre cada parametro, lo anterior es generico
            if (!DGPSresults)
            {
                if (parameter >= 1)
                {
                    parameter++;//saltamos PA en caso de que no sea tabla DGPS
                }
                for (int i = 0; i < SummaryGrid.RowCount; i++)
                {
                    data.Rows.Add(SummaryGrid.Rows[i].HeaderCell.Value, SummaryGrid[0, i].Value, SummaryGrid[1, i].Value, SummaryGrid[2, i].Value, SummaryGrid[3, i].Value);
                }
                myExcel.ImportDataTable(4, 1, data, true);//Inserta la tabla en la posicion i,j=4,1 (la primera celda en excel es la 1,1 ó A1
            }
            else
            {
                if (parameter == 1)
                {
                    for (int i = 0; i < SummaryGrid.RowCount; i++)
                    {
                        data.Rows.Add(SummaryGrid.Rows[i].HeaderCell.Value, SummaryGrid[0, i].Value, SummaryGrid[1, i].Value, SummaryGrid[2, i].Value, SummaryGrid[3, i].Value, SummaryGrid[4, i].Value, SummaryGrid[5, i].Value, SummaryGrid[6, i].Value);
                    }
                    myExcel.ImportDataTable(4, 1, data, true);//Inserta la tabla en la posicion i,j=4,1 (la primera celda en excel es la 1,1 ó A17
                }
                else
                {
                    for (int i = 0; i < SummaryGrid.RowCount; i++)
                    {
                        data.Rows.Add(SummaryGrid.Rows[i].HeaderCell.Value, SummaryGrid[0, i].Value, SummaryGrid[1, i].Value, SummaryGrid[2, i].Value, SummaryGrid[3, i].Value);
                    }
                    myExcel.ImportDataTable(4, 1, data, true);//Inserta la tabla en la posicion i,j=4,1 (la primera celda en excel es la 1,1 ó A1
                }
            }

            SLStyle style = myExcel.CreateStyle();//Pintamos el fondo verde o rojo
            int posInicial = 5;//5 por posicion del nombre de fichero, titulo parametro, informacion .ast, ademas de que excel empieza a enumerar en 1  
            for (int i = posInicial; i < SummaryGrid.RowCount + posInicial; i++)
            {
                if (parameter == 0) //UR
                {
                    if ((i == 0 + posInicial) || (i == 5 + posInicial) || (i == 8 + posInicial) || (i == 11 + posInicial))
                    {
                        if (myExcel.GetCellValueAsString(i, 4) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 4) >= myExcel.GetCellValueAsDouble(i, 5))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);

                            }
                            myExcel.SetCellStyle(i, 4, style);
                        }
                    }
                }
                if (parameter == 1) //Pos Acc
                {
                    if (i == 0 + posInicial)
                    {
                        if (myExcel.GetCellValueAsString(i, 2) != "")//AsDouble no acepta null
                        {
                            if (myExcel.GetCellValueAsDouble(i, 2) <= myExcel.GetCellValueAsDouble(i, 5))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 2, style);
                        }
                        if (myExcel.GetCellValueAsString(i, 3) != "")//AsDouble no acepta null
                        {
                            if (myExcel.GetCellValueAsDouble(i, 3) <= myExcel.GetCellValueAsDouble(i, 6))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 3, style);
                        }
                    }
                    else if (i == 5 + posInicial)
                    {
                        if (myExcel.GetCellValueAsString(i, 2) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 2) <= myExcel.GetCellValueAsDouble(i, 5))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 2, style);
                        }
                        if (myExcel.GetCellValueAsString(i, 3) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 3) <= myExcel.GetCellValueAsDouble(i, 6))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 3, style);
                        }
                    }
                    else if (i == 8 + posInicial)
                    {
                        if (myExcel.GetCellValueAsString(i, 4) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 4) <= 20)//max 20m en Stand PosAcc
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 4, style);
                        }
                    }
                }
                else if (parameter == 2) //ProbMLATdetection
                {
                    if ((i == 0 + posInicial) || (i == 7 + posInicial))
                    {
                        if (myExcel.GetCellValueAsString(i, 4) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 4) >= myExcel.GetCellValueAsDouble(i, 5))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 4, style);
                        }
                    }
                }
                else if (parameter == 3) //PID
                {
                    if (i == 15 + posInicial)
                    {
                        if (myExcel.GetCellValueAsString(i, 4) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 4) >= myExcel.GetCellValueAsDouble(i, 5))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 4, style);
                        }
                    }
                }
                else if (parameter == 4) //PFD
                {
                    if (i == 14 + posInicial)
                    {
                        if (myExcel.GetCellValueAsString(i, 4) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 4) <= myExcel.GetCellValueAsDouble(i, 5))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 4, style);
                        }
                    }
                }
                else if (parameter == 5) //PFI
                {
                    if (i == 15 + posInicial)
                    {
                        if (myExcel.GetCellValueAsString(i, 4) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 4) <= myExcel.GetCellValueAsDouble(i, 5))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 4, style);
                        }
                    }
                }
            }

            myExcel.AutoFitColumn(1, data.Columns.Count, 20);//Adapta el tamaño de las celdas (columnas de 1 a 5, maximo grosor de 20)
            return myExcel;
        }

        private void buttonSaveSummary_Click(object sender, EventArgs e)//Exporta el parameter grid actual a excel
        {
            string saveName = nombreFichero;//Añadimos caracteristicas actuales al nombre del fichero a guardar
            if (DGPSresults)
                saveName = saveName + "_DGPS";
            else saveName = saveName + "_MLAT";
            saveFileDialogExcel.FileName = saveName + "_" + Convert.ToString(ParametersGrid[0, ParametersGrid.CurrentRow.Index].Value);

            saveFileDialogExcel.Filter = "File documents (.xlsx)|*.xlsx";
            saveFileDialogExcel.ShowDialog();
            try
            {
                if (this.saveFileDialogExcel.FileName.Equals("") == false)
                {
                    if (this.saveFileDialogExcel.FileName.Contains(@"\"))//Controla si se ha escogido un directorio o se ha cancelado/cerrado el save dialog
                    {
                        SLDocument myExcel = SaveSummaryParameter();
                        Cursor.Current = Cursors.WaitCursor;
                        myExcel.SaveAs(saveFileDialogExcel.FileName);
                    }
                }
                else CrearFormInformativa("Please write a", "name first.", 187, 30, 27, 41);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private SLDocument SaveSummaryAll(Avaluador avaluador)//Prepara el excel para ser exportado; Exporta todos los parametros juntos
        {
            SLDocument myExcel = new SLDocument();
            myExcel.SetCellValue("A1", "File: " + nombreFichero);//Nombre fichero
            string avionesDetectados = "";
            if (myList.AircraftDetected() != 0)
                avionesDetectados = " from " + myList.AircraftDetected() + " different transponders ";//Esta frase solo aparece si hay MLAT
            myExcel.SetCellValue("A2", myList.GetNumList() + " packages avaluated" + avionesDetectados + "between " + myList.GetPlanI(0).ConvertUTC("SinDecimas") +
                            " hours and " + myList.GetPlanI(myList.GetNumList() - 1).ConvertUTC("SinDecimas") + " hours");//Información del fichero
            myExcel.SetCellValue("A3", "Summary:");

            int lineaActual = 5;//Guarda el valor de la linea en la que estamos añadiendo información (3 + 1(espacio))
            int parametroActual = 0;

            //////////////////UR//////////////////////
            myExcel.SetCellValue(lineaActual, 1, Convert.ToString(ParametersGrid[0, parametroActual].Value) + ":");//UR
            lineaActual++;

            DataTable data = new DataTable();
            DataGridView grid = new DataGridView();
            CrearTablaSummary(grid, avaluador);//UR
            data.Columns.Add(" ", typeof(string)); //Columna de zonas de movimiento (sin titulo)
            for (int j = 0; j < grid.ColumnCount; j++)//Copiamos filas y columnas de Summary Grid a data table
            {
                data.Columns.Add(grid.Columns[j].Name, typeof(double));//La informacion que contienen estas columnas es de tipo double
            }
            for (int i = 0; i < grid.RowCount; i++)
            {
                data.Rows.Add(grid.Rows[i].HeaderCell.Value, grid[0, i].Value, grid[1, i].Value, grid[2, i].Value, grid[3, i].Value);
            }
            myExcel.ImportDataTable(lineaActual, 1, data, true);//Inserta la tabla en la posicion i,j=5,1 (la primera celda en excel es la 1,1 ó A1
            lineaActual++;//sumamos 1 para usarlo en syle; despues sumamos el tamaño de la tabla(-1 que ya sumamos ahora) + 2 espacios entre tabla y tabla

            SLStyle style = myExcel.CreateStyle();//Pintamos el fondo verde o rojo
            for (int i = lineaActual; i < grid.RowCount + lineaActual; i++)
            {
                if ((i == 0 + lineaActual) || (i == 5 + lineaActual) || (i == 8 + lineaActual) || (i == 11 + lineaActual))
                {
                    if (myExcel.GetCellValueAsString(i, 4) != "")
                    {
                        if (myExcel.GetCellValueAsDouble(i, 4) >= myExcel.GetCellValueAsDouble(i, 5))
                        {
                            style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                        }
                        else
                        {
                            style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                        }
                        myExcel.SetCellStyle(i, 4, style);
                    }
                }
            }
            lineaActual = lineaActual + grid.RowCount + 2; //dejamos 2 espacios entre tabla y tabla
            int columnasMaximas = data.Columns.Count; //Compara el numero de columnas de cada tabla y se queda con el mayor


            //////////////////PositionAccuracy//////////////////////
            if (DGPSresults)
            {
                parametroActual++;
                myExcel.SetCellValue(lineaActual, 1, Convert.ToString(ParametersGrid[0, parametroActual].Value) + ":");//PositionAccuracy
                lineaActual++;

                data = new DataTable();
                grid = new DataGridView();
                CrearTablaPositionAccuracy(grid, avaluador);//PosAcc
                data.Columns.Add(" ", typeof(string)); //Columna de zonas de movimiento (sin titulo)
                for (int j = 0; j < grid.ColumnCount; j++)//Copiamos filas y columnas de Summary Grid a data table
                {
                    data.Columns.Add(grid.Columns[j].Name, typeof(double));//La informacion que contienen estas columnas es de tipo double
                }
                for (int i = 0; i < grid.RowCount; i++)
                {
                    data.Rows.Add(grid.Rows[i].HeaderCell.Value, grid[0, i].Value, grid[1, i].Value, grid[2, i].Value, grid[3, i].Value, grid[4, i].Value, grid[5, i].Value, grid[6, i].Value);
                }
                myExcel.ImportDataTable(lineaActual, 1, data, true);//Inserta la tabla en la posicion i,j=lineaActual,1 (la primera celda en excel es la 1,1 ó A1
                lineaActual++;//sumamos 1 para usarlo en syle; despues sumamos el tamaño de la tabla(-1 que ya sumamos ahora) + 2 espacios entre tabla y tabla

                style = myExcel.CreateStyle();//Pintamos el fondo verde o rojo
                for (int i = lineaActual; i < grid.RowCount + lineaActual; i++)
                {
                    if (i == 0 + lineaActual)
                    {
                        if (myExcel.GetCellValueAsString(i, 2) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 2) <= myExcel.GetCellValueAsDouble(i, 5))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 2, style);
                        }
                        if (myExcel.GetCellValueAsString(i, 3) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 3) <= myExcel.GetCellValueAsDouble(i, 6))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 3, style);
                        }
                    }
                    else if (i == 5 + lineaActual)
                    {
                        if (myExcel.GetCellValueAsString(i, 2) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 2) <= myExcel.GetCellValueAsDouble(i, 5))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 2, style);
                        }
                        if (myExcel.GetCellValueAsString(i, 3) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 3) <= myExcel.GetCellValueAsDouble(i, 6))
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 3, style);
                        }
                    }
                    else if (i == 8 + lineaActual)
                    {
                        if (myExcel.GetCellValueAsString(i, 4) != "")
                        {
                            if (myExcel.GetCellValueAsDouble(i, 4) <= 20) //max 20m en Stand PosAcc
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                            }
                            else
                            {
                                style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                            }
                            myExcel.SetCellStyle(i, 4, style);
                        }
                    }
                }
                lineaActual = lineaActual + grid.RowCount + 2; //dejamos 2 espacios entre tabla y tabla

                if (data.Columns.Count > columnasMaximas)//Si las columnas son mayores que las anteriores, reasigna valor
                {
                    columnasMaximas = data.Columns.Count;
                }
            }
            //////////////////ProbOfMLATDetection//////////////////////
            parametroActual++;
            myExcel.SetCellValue(lineaActual, 1, Convert.ToString(ParametersGrid[0, parametroActual].Value) + ":");//ProbOfMlatDetection
            lineaActual++;

            data = new DataTable();
            grid = new DataGridView();
            CrearTablaProbOfMLATDetection(grid, avaluador);//PD
            data.Columns.Add(" ", typeof(string)); //Columna de zonas de movimiento (sin titulo)
            for (int j = 0; j < grid.ColumnCount; j++)//Copiamos filas y columnas de Summary Grid a data table
            {
                data.Columns.Add(grid.Columns[j].Name, typeof(double));//La informacion que contienen estas columnas es de tipo double
            }
            for (int i = 0; i < grid.RowCount; i++)
            {
                data.Rows.Add(grid.Rows[i].HeaderCell.Value, grid[0, i].Value, grid[1, i].Value, grid[2, i].Value, grid[3, i].Value);
            }
            myExcel.ImportDataTable(lineaActual, 1, data, true);//Inserta la tabla en la posicion i,j=lineaActual,1 (la primera celda en excel es la 1,1 ó A1
            lineaActual++;//sumamos 1 para usarlo en syle; despues sumamos el tamaño de la tabla(-1 que ya sumamos ahora) + 2 espacios entre tabla y tabla

            style = myExcel.CreateStyle();//Pintamos el fondo verde o rojo
            for (int i = lineaActual; i < grid.RowCount + lineaActual; i++)
            {
                if ((i == 0 + lineaActual) || (i == 7 + lineaActual))
                {
                    if (myExcel.GetCellValueAsString(i, 4) != "")
                    {
                        if (myExcel.GetCellValueAsDouble(i, 4) >= myExcel.GetCellValueAsDouble(i, 5))
                        {
                            style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                        }
                        else
                        {
                            style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                        }
                        myExcel.SetCellStyle(i, 4, style);
                    }
                }
            }
            lineaActual = lineaActual + grid.RowCount + 2; //dejamos 2 espacios entre tabla y tabla

            if (data.Columns.Count > columnasMaximas)//Si las columnas son mayores que las anteriores, reasigna valor
            {
                columnasMaximas = data.Columns.Count;
            }

            //////////////////ProbOfIdentification/////////////////////
            parametroActual++;
            myExcel.SetCellValue(lineaActual, 1, Convert.ToString(ParametersGrid[0, parametroActual].Value) + ":");//PID
            lineaActual++;

            data = new DataTable();
            grid = new DataGridView();
            CrearTablaProbOfIdentification(grid, avaluador);//PID
            data.Columns.Add(" ", typeof(string)); //Columna de zonas de movimiento (sin titulo)
            for (int j = 0; j < grid.ColumnCount; j++)//Copiamos filas y columnas de Summary Grid a data table
            {
                data.Columns.Add(grid.Columns[j].Name, typeof(double));//La informacion que contienen estas columnas es de tipo double
            }
            for (int i = 0; i < grid.RowCount; i++)
            {
                data.Rows.Add(grid.Rows[i].HeaderCell.Value, grid[0, i].Value, grid[1, i].Value, grid[2, i].Value, grid[3, i].Value);
            }
            myExcel.ImportDataTable(lineaActual, 1, data, true);//Inserta la tabla en la posicion i,j=lineaActual,1 (la primera celda en excel es la 1,1 ó A1
            lineaActual++;//sumamos 1 para usarlo en syle; despues sumamos el tamaño de la tabla(-1 que ya sumamos ahora) + 2 espacios entre tabla y tabla

            if (myExcel.GetCellValueAsString(grid.RowCount + lineaActual - 1, 4) != "")
            {
                style = myExcel.CreateStyle();//Pintamos el fondo verde o rojo
                if (myExcel.GetCellValueAsDouble(grid.RowCount + lineaActual - 1, 4) >= myExcel.GetCellValueAsDouble(grid.RowCount + lineaActual - 1, 5))//-1 porque grid empieza a contar en 0
                {
                    style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                }
                else
                {
                    style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                }
                myExcel.SetCellStyle(grid.RowCount + lineaActual - 1, 4, style);
            }

            lineaActual = lineaActual + grid.RowCount + 2; //dejamos 2 espacios entre tabla y tabla

            if (data.Columns.Count > columnasMaximas)//Si las columnas son mayores que las anteriores, reasigna valor
            {
                columnasMaximas = data.Columns.Count;
            }

            //////////////////ProbOfFalseDetection/////////////////////
            parametroActual++;
            myExcel.SetCellValue(lineaActual, 1, Convert.ToString(ParametersGrid[0, parametroActual].Value) + ":");//PFD
            lineaActual++;

            data = new DataTable();
            grid = new DataGridView();
            CrearTablaProbOfFalseDetection(grid, avaluador);//PFD
            data.Columns.Add(" ", typeof(string)); //Columna de zonas de movimiento (sin titulo)
            for (int j = 0; j < grid.ColumnCount; j++)//Copiamos filas y columnas de Summary Grid a data table
            {
                data.Columns.Add(grid.Columns[j].Name, typeof(double));//La informacion que contienen estas columnas es de tipo double
            }
            for (int i = 0; i < grid.RowCount; i++)
            {
                data.Rows.Add(grid.Rows[i].HeaderCell.Value, grid[0, i].Value, grid[1, i].Value, grid[2, i].Value, grid[3, i].Value);
            }
            myExcel.ImportDataTable(lineaActual, 1, data, true);//Inserta la tabla en la posicion i,j=lineaActual,1 (la primera celda en excel es la 1,1 ó A1
            lineaActual++;//sumamos 1 para usarlo en syle; despues sumamos el tamaño de la tabla(-1 que ya sumamos ahora) + 2 espacios entre tabla y tabla

            if (myExcel.GetCellValueAsString(grid.RowCount + lineaActual - 1, 4) != "")
            {
                style = myExcel.CreateStyle();//Pintamos el fondo verde o rojo
                if (myExcel.GetCellValueAsDouble(grid.RowCount + lineaActual - 1, 4) <= myExcel.GetCellValueAsDouble(grid.RowCount + lineaActual - 1, 5))//-1 porque grid empieza a contar en 0
                {
                    style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                }
                else
                {
                    style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                }
                myExcel.SetCellStyle(grid.RowCount + lineaActual - 1, 4, style);
            }

            lineaActual = lineaActual + grid.RowCount + 2; //dejamos 2 espacios entre tabla y tabla

            if (data.Columns.Count > columnasMaximas)//Si las columnas son mayores que las anteriores, reasigna valor
            {
                columnasMaximas = data.Columns.Count;
            }

            //////////////////ProbOfFalseIdentification/////////////////////
            parametroActual++;
            myExcel.SetCellValue(lineaActual, 1, Convert.ToString(ParametersGrid[0, parametroActual].Value) + ":");//PFI
            lineaActual++;

            data = new DataTable();
            grid = new DataGridView();
            CrearTablaProbOfFalseIdentification(grid, avaluador);//PFI
            data.Columns.Add(" ", typeof(string)); //Columna de zonas de movimiento (sin titulo)
            for (int j = 0; j < grid.ColumnCount; j++)//Copiamos filas y columnas de Summary Grid a data table
            {
                data.Columns.Add(grid.Columns[j].Name, typeof(double));//La informacion que contienen estas columnas es de tipo double
            }
            for (int i = 0; i < grid.RowCount; i++)
            {
                data.Rows.Add(grid.Rows[i].HeaderCell.Value, grid[0, i].Value, grid[1, i].Value, grid[2, i].Value, grid[3, i].Value);
            }
            myExcel.ImportDataTable(lineaActual, 1, data, true);//Inserta la tabla en la posicion i,j=lineaActual,1 (la primera celda en excel es la 1,1 ó A1
            lineaActual++;//sumamos 1 para usarlo en syle; despues sumamos el tamaño de la tabla(-1 que ya sumamos ahora) + 2 espacios entre tabla y tabla

            if (myExcel.GetCellValueAsString(grid.RowCount + lineaActual - 1, 4) != "")
            {
                style = myExcel.CreateStyle();//Pintamos el fondo verde o rojo
                if (myExcel.GetCellValueAsDouble(grid.RowCount + lineaActual - 1, 4) <= myExcel.GetCellValueAsDouble(grid.RowCount + lineaActual - 1, 5))//-1 porque grid empieza a contar en 0
                {
                    style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Green, Color.Transparent);
                }
                else
                {
                    style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, Color.Red, Color.Transparent);
                }
                myExcel.SetCellStyle(grid.RowCount + lineaActual - 1, 4, style);
            }

            lineaActual = lineaActual + grid.RowCount + 2; //dejamos 2 espacios entre tabla y tabla

            if (data.Columns.Count > columnasMaximas)//Si las columnas son mayores que las anteriores, reasigna valor
            {
                columnasMaximas = data.Columns.Count;
            }

            ////////////////////////////////////
            myExcel.AutoFitColumn(1, columnasMaximas, 20);//Adapta el tamaño de las celdas (columnas de 1 a 5, maximo grosor de 20)
            return myExcel;
        }

        private void buttonSaveAllSummary_Click(object sender, EventArgs e) //Boton de guardar todos los parametros a excel
        {
            string saveName = nombreFichero;//Añadimos caracteristicas actuales al nombre del fichero a guardar
            if (DGPSresults)
                saveName = saveName + "_DGPS";
            else saveName = saveName + "_MLAT";

            saveFileDialogExcel.FileName = saveName + "_Summary";
            saveFileDialogExcel.Filter = "File documents (.xlsx)|*.xlsx";
            saveFileDialogExcel.ShowDialog();
            try
            {
                if (this.saveFileDialogExcel.FileName.Equals("") == false)
                {
                    if (this.saveFileDialogExcel.FileName.Contains(@"\"))//Controla si se ha escogido un directorio o se ha cancelado/cerrado el save dialog
                    {
                        SLDocument myExcel;
                        if (DGPSresults)
                        {
                             myExcel = SaveSummaryAll(myAvaluadorDGPS);
                        }
                        else  myExcel = SaveSummaryAll(myAvaluador);
                        Cursor.Current = Cursors.WaitCursor;
                        myExcel.SaveAs(saveFileDialogExcel.FileName);
                        saveAllCorrectoPorSeparado[1] = true;
                        if (saveAllCorrectoPorSeparado[0])//Si el del mapa tambien es true, el general tambien pasa a ser true
                            saveAllCorrecto = true;
                    }
                }
                else CrearFormInformativa("Please write a", "name first.", 187, 30, 27, 41);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        
        private void ButtonDerechaSummay_Click(object sender, EventArgs e)//Botones para pasar de parametro en la summay grid (derecha)
        {
            int i = ParametersGrid.CurrentRow.Index;
            ParametersGrid[0, i].Selected = false;
            if (i == ParametersGrid.RowCount - 1)//De la ultima a la primera
            {
                i = 0;
            }
            else i++;
            ParametersGrid[0, i].Selected = true;
            LabelTituloSummary.Text = Convert.ToString(ParametersGrid[0, i].Value) + ":";
            SummaryGrid.Rows.Clear();
            SummaryGrid.Columns.Clear();
            if (DGPSresults)
            {
                if (i == 0)
                    CrearTablaSummary(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 1)
                    CrearTablaPositionAccuracy(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 2)
                    CrearTablaProbOfMLATDetection(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 3)
                    CrearTablaProbOfIdentification(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 4)
                    CrearTablaProbOfFalseDetection(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 5)
                    CrearTablaProbOfFalseIdentification(this.SummaryGrid, myAvaluadorDGPS);
            }
            else
            {
                if (i == 0)
                    CrearTablaSummary(this.SummaryGrid, myAvaluador);
                else if (i == 1)
                    CrearTablaProbOfMLATDetection(this.SummaryGrid, myAvaluador);
                else if (i == 2)
                    CrearTablaProbOfIdentification(this.SummaryGrid, myAvaluador);
                else if (i == 3)
                    CrearTablaProbOfFalseDetection(this.SummaryGrid, myAvaluador);
                else if (i == 4)
                    CrearTablaProbOfFalseIdentification(this.SummaryGrid, myAvaluador);
            }
        } 

        private void ButtonIzquierdaSummary_Click(object sender, EventArgs e)//Botones para pasar de parametro en la summay grid (izquierda)
        {
            int i = ParametersGrid.CurrentRow.Index;
            ParametersGrid[0, i].Selected = false;
            if (i == 0)//De la primera a la ultima
            {
                i = ParametersGrid.RowCount - 1;
            }
            else i--;
            ParametersGrid[0, i].Selected = true;
            LabelTituloSummary.Text = Convert.ToString(ParametersGrid[0, i].Value) + ":";
            SummaryGrid.Rows.Clear();
            SummaryGrid.Columns.Clear();

            if (DGPSresults)
            {
                if (i == 0)
                    CrearTablaSummary(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 1)
                    CrearTablaPositionAccuracy(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 2)
                    CrearTablaProbOfMLATDetection(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 3)
                    CrearTablaProbOfIdentification(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 4)
                    CrearTablaProbOfFalseDetection(this.SummaryGrid, myAvaluadorDGPS);
                else if (i == 5)
                    CrearTablaProbOfFalseIdentification(this.SummaryGrid, myAvaluadorDGPS);
            }
            else
            {
                if (i == 0)
                    CrearTablaSummary(this.SummaryGrid, myAvaluador);
                else if (i == 1)
                    CrearTablaProbOfMLATDetection(this.SummaryGrid, myAvaluador);
                else if (i == 2)
                    CrearTablaProbOfIdentification(this.SummaryGrid, myAvaluador);
                else if (i == 3)
                    CrearTablaProbOfFalseDetection(this.SummaryGrid, myAvaluador);
                else if (i == 4)
                    CrearTablaProbOfFalseIdentification(this.SummaryGrid, myAvaluador);
            }
        }

        private void ParametersGrid_SelectionChanged(object sender, EventArgs e)//Si bajas, subes con teclado
        {
            if (ParametersGrid.SelectedRows.Count != 0)
            {
                int i = ParametersGrid.SelectedRows[0].Index;
                if (ParametersGrid[0, 0].Value == null)//En el inicio entra aqui antes de asignar el nombre a cada row
                {
                    ParametersGrid[0, 0].Value = "Update Rate";
                }
                LabelTituloSummary.Text = Convert.ToString(ParametersGrid[0, i].Value) + ":";
                SummaryGrid.Rows.Clear();
                SummaryGrid.Columns.Clear();
                if (DGPSresults)
                {
                    if (i == 0)
                        CrearTablaSummary(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 1)
                        CrearTablaPositionAccuracy(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 2)
                        CrearTablaProbOfMLATDetection(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 3)
                        CrearTablaProbOfIdentification(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 4)
                        CrearTablaProbOfFalseDetection(this.SummaryGrid, myAvaluadorDGPS);
                    else if (i == 5)
                        CrearTablaProbOfFalseIdentification(this.SummaryGrid, myAvaluadorDGPS);
                }
                else
                {
                    if (i == 0)
                        CrearTablaSummary(this.SummaryGrid, myAvaluador);
                    else if (i == 1)
                        CrearTablaProbOfMLATDetection(this.SummaryGrid, myAvaluador);
                    else if (i == 2)
                        CrearTablaProbOfIdentification(this.SummaryGrid, myAvaluador);
                    else if (i == 3)
                        CrearTablaProbOfFalseDetection(this.SummaryGrid, myAvaluador);
                    else if (i == 4)
                        CrearTablaProbOfFalseIdentification(this.SummaryGrid, myAvaluador);
                }
            }
        }

        private void CrearTablaDescartados(DataGridView grid)  //tabla de vehiculos descartados
        {
            List<string> descartados = myListConDescartados.LeerVehiculosSquitter();
            grid.RowCount = descartados.Count(); 
            grid.ColumnCount = 1;
            grid.ColumnHeadersVisible = true;
            grid.RowHeadersVisible = false;
            grid.Columns[0].SortMode = DataGridViewColumnSortMode.Programmatic;
            grid.Columns[0].Name = "Discarded";
            for (int n = 0; n < grid.RowCount; n++)
            {
                grid[0, n].Style.BackColor = Color.White;
            }
            int j;
            bool encontrado;
            if (descartados[0] != "Sin Vehiculos")
            {
                for (int i = 0; i < descartados.Count(); i++)
                {
                    grid[0, i].Value = descartados[i];
                    j = 0;
                    encontrado = false;
                    while ((j < myListConDescartados.GetNumList()) && (!encontrado))
                    {
                        if (myListConDescartados.GetPlanI(j).GetICAOAdress() == descartados[i])
                        {
                            encontrado = true;
                            grid[0, i].Style.BackColor = Color.Green;
                        }
                        j++;
                    }
                }
            }
            else
            {
                grid[0, 0].Value = "No Vehicles";
            }
        }

        private void CrearTablaNoDescartados(DataGridView grid)  //tabla de vehiculos no descartados
        {
            double[,] updateRateU = myAvaluador.GetUpdatesUnitarios();
            double[,] diferencia = new double[aircraftDetected.Count(), 3]; //Contiene diferencia (expected-updates reales); UR; indice para Icao
            for (int i = 0; i < updateRateU.GetLength(0); i++)
            {
                diferencia[i, 0] = updateRateU[i, 1] - updateRateU[i, 0];
                if (updateRateU[i, 1] != 0)
                {
                    diferencia[i, 1] = updateRateU[i, 0] / updateRateU[i, 1];
                }
                diferencia[i, 2] = updateRateU[i, 2];
            }
            myAvaluador.OrdenarDiferencias(diferencia, "Mm");

            grid.RowCount = diferencia.GetLength(0);
            grid.ColumnCount = 3;
            grid.ColumnHeadersVisible = true;
            grid.RowHeadersVisible = false;
            grid.Columns[0].SortMode = DataGridViewColumnSortMode.Programmatic;
            grid.Columns[0].Name = "Not Discarded";
            grid.Columns[1].Name = "Expected-Real";
            grid.Columns[2].Name = "UR (%)";
            grid.ClearSelection();
            for (int i = 0; i < grid.RowCount; i++)
            {
                grid[0, i].Value = myList.GetPlanI(Convert.ToInt32(diferencia[i, 2])).GetICAOAdress();
                grid[1, i].Value = Math.Round(diferencia[i, 0]);//Diferencia
                grid[2, i].Value = Math.Round(diferencia[i, 1], 3) * 100;//UR
            }
        }

        private void panelMapDescartados_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;
                graphics.DrawImage(myBitmapDescartados, 0, 0, myBitmapDescartados.Width, myBitmapDescartados.Height);
                graphics.Dispose();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void tablaGridDescartados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i != -1)
            {
                if (Convert.ToString(tablaGridDescartados[0, i].Value) != "No Vehicles")
                {
                    AddRemoveDiscardButton.Visible = true;
                    ViewWholeButton.Visible = true;
                    AddRemoveDiscardButton.Text = "Remove";
                    tablaGridNoDescartados.ClearSelection();
                    tablaGridDescartados.ClearSelection();
                    tablaGridDescartados[0, i].Selected = true;
                    string icao = Convert.ToString(tablaGridDescartados[0, i].Value);
                    myBitmapDescartados = new Bitmap(this.panelMapDescartados.Width, this.panelMapDescartados.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    dimensionesDescartado = myLmap.GetDIMENSIONES();
                    proporcionDescartados = myLmap.GetPROPORCION(this.panelMapDescartados.Height, this.panelMapDescartados.Width);
                    ViewWholeButton.Text = "View Whole";
                    PintarFondoPantalla(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height);
                    PintarAllDescartados(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height, icao);
                    panelMapDescartados.Invalidate();
                }
            }
        }

        private void tablaGridDescartados_SelectionChanged(object sender, EventArgs e)//Si bajas con el teclado por la tabla, actualiza el mapa
        {
            if (tablaGridDescartados.SelectedRows.Count != 0)
            {
                if (Convert.ToString(tablaGridDescartados[0, tablaGridDescartados.SelectedRows[0].Index].Value) != "No Vehicles")
                {
                    string icao = Convert.ToString(tablaGridDescartados[0, tablaGridDescartados.SelectedRows[0].Index].Value);
                    myBitmapDescartados = new Bitmap(this.panelMapDescartados.Width, this.panelMapDescartados.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    dimensionesDescartado = myLmap.GetDIMENSIONES();
                    proporcionDescartados = myLmap.GetPROPORCION(this.panelMapDescartados.Height, this.panelMapDescartados.Width);
                    ViewWholeButton.Text = "View Whole";
                    PintarFondoPantalla(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height);
                    PintarAllDescartados(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height, icao);
                    panelMapDescartados.Invalidate();
                }
            }
        }

        private void tablaGridNoDescartados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i != -1)
            {
                AddRemoveDiscardButton.Visible = true;
                ViewWholeButton.Visible = true;
                AddRemoveDiscardButton.Text = "Add";
                tablaGridDescartados.ClearSelection();
                tablaGridNoDescartados.ClearSelection();
                tablaGridNoDescartados[0, i].Selected = true;
                string icao = Convert.ToString(tablaGridNoDescartados[0, i].Value);
                myBitmapDescartados = new Bitmap(this.panelMapDescartados.Width, this.panelMapDescartados.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                dimensionesDescartado = myLmap.GetDIMENSIONES();
                proporcionDescartados = myLmap.GetPROPORCION(this.panelMapDescartados.Height, this.panelMapDescartados.Width);
                ViewWholeButton.Text = "View Whole";
                PintarFondoPantalla(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height);
                PintarAllDescartados(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height, icao);
                panelMapDescartados.Invalidate();
            }
        }

        private void tablaGridNoDescartados_SelectionChanged(object sender, EventArgs e)//Si bajas con el teclado por la tabla, actualiza el mapa
        {
            if (tablaGridNoDescartados.SelectedRows.Count != 0)
            {
                string icao = Convert.ToString(tablaGridNoDescartados[0, tablaGridNoDescartados.SelectedRows[0].Index].Value);
                myBitmapDescartados = new Bitmap(this.panelMapDescartados.Width, this.panelMapDescartados.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                dimensionesDescartado = myLmap.GetDIMENSIONES();
                proporcionDescartados = myLmap.GetPROPORCION(this.panelMapDescartados.Height, this.panelMapDescartados.Width);
                ViewWholeButton.Text = "View Whole";
                PintarFondoPantalla(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height);
                PintarAllDescartados(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height, icao);
                panelMapDescartados.Invalidate();
            }
        }

        private void AddRemoveDiscardButton_Click(object sender, EventArgs e) //Añade o descarta un vehiculo de la lista de descartados
        {
            List<string> descartados = myListConDescartados.LeerVehiculosSquitter();
            bool descartado = false; //Hay caminos que no descartan/ añaden un vehiculo, se evita asi el Reinicio de la form
            if (AddRemoveDiscardButton.Text=="Add")
            {
                if (myListDGPS.GetNumList() != 0)
                {
                    if (myListDGPS.GetPlanI(0).GetICAOAdress() != Convert.ToString(tablaGridNoDescartados[0, tablaGridNoDescartados.CurrentRow.Index].Value)) //Si hay DGPS no se puede hacer Add de ese ICAO Address. 
                    {
                        descartados.Add(Convert.ToString(tablaGridNoDescartados[0, tablaGridNoDescartados.CurrentRow.Index].Value));
                        descartados.Remove("Sin Vehiculos");
                        File.Delete("VehiculosAEliminar.txt");
                        File.WriteAllLines("VehiculosAEliminar.txt", descartados);
                        descartado = true;
                    }
                    else
                    {
                        CrearFormInformativa("This vehicle can't be added to the discarded list because", "it's being used to compare MLAT with D-GPS data.", 508, 190, 27, 51);
                    }
                }
                else
                {
                    descartados.Add(Convert.ToString(tablaGridNoDescartados[0, tablaGridNoDescartados.CurrentRow.Index].Value));
                    descartados.Remove("Sin Vehiculos");
                    File.Delete("VehiculosAEliminar.txt");
                    File.WriteAllLines("VehiculosAEliminar.txt", descartados);
                    descartado = true;
                }
            }
            else if (AddRemoveDiscardButton.Text == "Remove")
            {
                if (Convert.ToString(tablaGridDescartados[0, tablaGridDescartados.CurrentRow.Index].Value) != "No Vehicles")
                {
                    descartados.Remove(Convert.ToString(tablaGridDescartados[0, tablaGridDescartados.CurrentRow.Index].Value));
                }
                if (descartados.Count()==0)
                {
                    descartados.Add("Sin Vehiculos");
                }
                File.Delete("VehiculosAEliminar.txt");
                File.WriteAllLines("VehiculosAEliminar.txt",descartados);
                descartado = true;
            }
            if (descartado)
            {
                RecargarMainForm();
            }    
        }

        private void ViewWholeButton_Click(object sender, EventArgs e) //A veces no se ve una trayectoria en el mapa porque esta en airborne y no pasa por LEBL. Este boton reajusta el tamaño del mapa
        {
            string icao = "";
            if (AddRemoveDiscardButton.Text == "Add")
            {
                icao = Convert.ToString(tablaGridNoDescartados[0, tablaGridNoDescartados.CurrentRow.Index].Value);
            }
            else if (AddRemoveDiscardButton.Text == "Remove")
            {
                icao = Convert.ToString(tablaGridDescartados[0, tablaGridDescartados.CurrentRow.Index].Value);
            }
            if (ViewWholeButton.Text == "View Whole")
            {
                ViewWholeButton.Text = "View Normal";
                double[,] dimensionesVehiculo = myList.GetDimensiones(icao);
                double[,] dimensionesMapa = myLmap.GetDIMENSIONES();
                double[,] dimensionesFinales = dimensionesVehiculo;
                if (dimensionesVehiculo[0, 0] > dimensionesMapa[0, 0])
                {
                    dimensionesFinales[0, 0] = dimensionesMapa[0, 0];
                }
                if (dimensionesVehiculo[0, 1] < dimensionesMapa[0, 1])
                {
                    dimensionesFinales[0, 1] = dimensionesMapa[0, 1];
                }
                if (dimensionesVehiculo[1, 0] > dimensionesMapa[1, 0])
                {
                    dimensionesFinales[1, 0] = dimensionesMapa[1, 0];
                }
                if (dimensionesVehiculo[1, 1] < dimensionesMapa[1, 1])
                {
                    dimensionesFinales[1, 1] = dimensionesMapa[1, 1];
                }
                myBitmapDescartados = new Bitmap(this.panelMapDescartados.Width, this.panelMapDescartados.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                dimensionesDescartado = dimensionesFinales;
                proporcionDescartados = myList.GetProporcion(dimensionesFinales,this.panelMapDescartados.Height, this.panelMapDescartados.Width);
                PintarFondoPantalla(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height);
                PintarAllDescartados(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height, icao);
                panelMapDescartados.Invalidate();
            }
            else
            {
                ViewWholeButton.Text = "View Whole";
                dimensionesDescartado = myLmap.GetDIMENSIONES();
                proporcionDescartados = myLmap.GetPROPORCION(this.panelMapDescartados.Height, this.panelMapDescartados.Width);
                PintarFondoPantalla(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height);
                PintarAllDescartados(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height, icao);
                panelMapDescartados.Invalidate();
            }
            
        }

        //Menu tool strips
        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveAllCorrecto)
            {
                Form1 nuevoFichero = new Form1();
                nuevoFichero.StartPosition = FormStartPosition.CenterScreen;
                nuevoFichero.SetNuevoFichero(true);
                nuevoFichero.Show();
                if (nuevoFichero.CerrarMain())
                {
                    this.Close();
                }
            }
            else
            {
                bool exit = false; //Controla si se quiere salir en la Form Close
                bool save = false; //Controla si se quiere guardar el file
                FormClose close = new FormClose();
                close.StartPosition = FormStartPosition.CenterScreen;
                close.ShowDialog();
                exit = close.ExportarExit();
                save = close.ExportarSave();
                if (exit)
                {
                    if (save) //Guarda todos los documentos en una carpeta
                    {
                        saveAllToolStripMenuItem.PerformClick();
                        if (saveAllCorrecto)
                        {
                            CrearFormInformativa("File correctly saved,", "choose a new file.", 231, 54, 27, 34);
                            loadFileToolStripMenuItem.PerformClick();
                        }
                    }
                    else
                    {
                        saveAllCorrecto = true;
                        loadFileToolStripMenuItem.PerformClick();
                    }
                }
            }
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool origenDescartados = false;
            if (mapaDescartados)//Miramos si esta en la ventana de descartados
            {
                origenDescartados = true;
            }
            mapaDescartados = false;//Lo ponemos false para que no guarde el formato basico, despues, segun el origen, volverá a ser true

            Bitmap saveBitmap;
            SLDocument myExcel;
            saveFileDialogAll.FileName = nombreFichero + "_Results";
            saveFileDialogAll.ShowDialog();
            try
            {
                if (this.saveFileDialogAll.FileName.Equals("") == false)
                {
                    if (this.saveFileDialogAll.FileName.Contains(@"\"))//Controla si se ha escogido un directorio o se ha cancelado/cerrado el save dialog
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        Directory.CreateDirectory(saveFileDialogAll.FileName);

                        bool saveDGPS = DGPS;//Guardamos el valor actual de los boleanos
                        bool saveMLATandDGPS = MLATandDGPS;
                        bool saveSegmentation = segmentation;
                        bool saveZoom = zoom;
                        bool saveHideBack = HideBack;
                        bool saveDGPSresults = DGPSresults;
                        int indiceParametro = ParametersGrid.CurrentRow.Index;

                        ResetearBoleanos();//Lo pone todo en false
                        saveBitmap = SaveMap();
                        saveBitmap.Save(Path.Combine(saveFileDialogAll.FileName, nombreFichero + "_MLAT_Map.png"));
                        segmentation = true;
                        saveBitmap = SaveMap();
                        saveBitmap.Save(Path.Combine(saveFileDialogAll.FileName, nombreFichero + "_MLAT_Segmented_Map.png"));
                        ResetearBoleanos();
                        HideBack = true;
                        zoom = true;
                        saveBitmap = SaveMap();
                        saveBitmap.Save(Path.Combine(saveFileDialogAll.FileName, nombreFichero + "_MLAT_ZoomOut_WithOutBackground_Map.png"));
                        if (myListDGPS.GetNumList() != 0)
                        {
                            ResetearBoleanos();
                            DGPS = true;
                            saveBitmap = SaveMap();
                            saveBitmap.Save(Path.Combine(saveFileDialogAll.FileName, nombreFichero + "_DGPS_Map.png"));
                            ResetearBoleanos();
                            MLATandDGPS = true;
                            HideBack = true;
                            saveBitmap = SaveMap();
                            saveBitmap.Save(Path.Combine(saveFileDialogAll.FileName, nombreFichero + "_MLATandDGPS_WithOutBackground_Map.png"));

                            ResetearBoleanos();
                            DGPSresults = true;
                            CrearTablaParameters();
                            myExcel = SaveSummaryAll(myAvaluadorDGPS);
                            myExcel.SaveAs(Path.Combine(saveFileDialogAll.FileName, nombreFichero + "_DGPS_Summary.xlsx"));
                        }

                        ResetearBoleanos();
                        CrearTablaParameters();
                        myExcel = SaveSummaryAll(myAvaluador);
                        myExcel.SaveAs(Path.Combine(saveFileDialogAll.FileName, nombreFichero + "_MLAT_Summary.xlsx"));

                        saveAllCorrecto = true;

                        DGPS = saveDGPS;//Volvemos a asignar los valores originales de los boleanos
                        MLATandDGPS = saveMLATandDGPS;
                        segmentation = saveSegmentation;
                        zoom = saveZoom;
                        HideBack = saveHideBack;
                        DGPSresults = saveDGPSresults;
                        CrearTablaParameters();
                        ParametersGrid.ClearSelection();
                        ParametersGrid[0, indiceParametro].Selected = true;
                        LabelTituloSummary.Text = Convert.ToString(ParametersGrid[0, indiceParametro].Value) + ":";
                    }
                    else
                    {
                        saveAllCorrecto = false;
                    }

                }
                else
                {
                    CrearFormInformativa("Please write a", "name first.", 187, 30, 27, 41);
                    saveAllCorrecto = false;
                }

                if (origenDescartados)//Miramos si esta en la ventana de descartados y lo devolvemos a ahi
                {
                    mapaDescartados = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                saveAllCorrecto = false;
                if (origenDescartados)//Miramos si esta en la ventana de descartados y lo devolvemos a ahi
                {
                    mapaDescartados = true;
                }
                return;
            }
        }

        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelContenedorMapa.Visible = true;
            panelContenedorResultados.Visible = false;
            panelContenedorDescartados.Visible = false;
            mapaDescartados = false;
            labelCursor.Text = "Cursor: --.--m, --.--m (X,Y)";
        }

        private void summaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelContenedorMapa.Visible = false;
            panelContenedorResultados.Visible = true;
            panelContenedorDescartados.Visible = false;
            mapaDescartados = false;
        }

        private void discardedVehiclesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelContenedorMapa.Visible = false;
            panelContenedorResultados.Visible = false;
            panelContenedorDescartados.Visible = true;
            mapaDescartados = true;
            myBitmapDescartados = new Bitmap(this.panelMapDescartados.Width, this.panelMapDescartados.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            PintarFondoPantalla(myBitmapDescartados, this.panelMapDescartados.Width, this.panelMapDescartados.Height); //Genera el bmp con el mapa de LEBL
            tablaGridDescartados.ClearSelection();
            tablaGridNoDescartados.ClearSelection();
            AddRemoveDiscardButton.Visible = false;
            ViewWholeButton.Visible = false;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)//Evento que se activa cuando se cierra la Mainform
        {
            if (!saveAllCorrecto)
            {
                e.Cancel = true;
                bool exit = false; //Controla si se quiere salir en la Form Close
                bool save = false; //Controla si se quiere guardar el file
                FormClose close = new FormClose();
                close.StartPosition = FormStartPosition.CenterScreen;
                close.ShowDialog();
                exit = close.ExportarExit();
                save = close.ExportarSave();
                if (exit)
                {
                    if (save) //Guarda todos los documentos en una carpeta
                    {
                        saveAllToolStripMenuItem.PerformClick();
                        if (saveAllCorrecto)
                            e.Cancel = false;
                    }
                    else e.Cancel = false;
                }
                //Si exit=false e.cancel se mantiene true y todo se queda como estaba
            }
        }

        //Subfunciones
        public void CrearFormInformativa(string info1, string info2, int formWidth, int Xbutton, int Xlabel1, int Xlabel2)//Valores puestos a mano para cuadradrar el texto segun la longitud del propio texto
        {
            FormInformativa formI = new FormInformativa();
            formI.ImportarInformacion(info1, info2, formWidth, Xbutton, Xlabel1, Xlabel2);
            formI.ShowDialog();
        }

        //private void seccionesButton_Click(object sender, EventArgs e)//Boton para iniciar/finalizar una seccion
        //{
        //    if (seccionInicializada)
        //    {
        //        puntosSecciones.Clear();
        //        seccionInicializada = false;
        //        seccionesButton.Text = "Finalizar seccion";
        //    }
        //    else
        //    {
        //        seccionInicializada = true;
        //        seccionesButton.Text = "Iniciar seccion";
        //        StreamWriter W = File.AppendText("Secciones.txt");
        //        for (int i = 0; i < puntosSecciones.Count(); i++)
        //            W.WriteLine(puntosSecciones[i][0] + " " + puntosSecciones[i][1]);
        //        W.WriteLine("/");
        //        W.Close();
        //        panelMap.Invalidate();
        //    }
        //}
    }
}
