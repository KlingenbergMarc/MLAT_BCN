using System;
using System.Collections.Generic;
using System.Linq;

namespace Codigo
{
    public class Avaluador
    {
        LectorMensaje myList;
        LectorMensaje DGPSList;
        LectorMensaje listaPA;
        double[,] updateRate; //Cada fila de la matriz corresponde a una zona (siguiente orden): 
                              //maneouvering area; rwy 25L; rwy 02; rwy 25R; taxi; apron; apron T1; apron T2; stand; stand T1; stand T2; airborne; airborne STAR/SID; airborne Cruise 
                              //maneouvering area, apron, stand y airborne son la media de las zonas que la componen.
                              //Cada columna de la matriz corresponde a: Updates, Expected update; update rate; minimum UR
        double[,] updatesUnitarios; //Lo calculamos para poder decidir si descartar o no un vehiculo
        double[,] PosAcc; //Filas ideanticas a updateRate sin desglosar Airborne (no data)
                          //Cada columna de la matriz corresponde a: P95, P99 ; Max Value detected; max P95; max P99; Max value allowed; Mean; Std Dev
        List<double[]> diferencias; //3 columnas: [0]= error en x, [1]=error en y; [2]=zona.
        double[,] MLATDet; //Filas ideanticas a updateRate sin desglosar Apron
                           //Cada columna de la matriz corresponde a: Detected, Expected ; PD; minimum PD
        double[,] Identification; //Filas ideanticas a updateRate + Total
                                  //Cada columna de la matriz corresponde a: Correct; Incorrect; PID; minimum PID
        double[,] FalseDetection; //Filas ideanticas a updateRate + Total, Airborne solo tipo 4 y 5
                                  //Cada columna de la matriz corresponde a: Reports, False ; PFD; minimum PFD
        double[,] FalseIdentification;//Filas ideanticas a updateRate + Total
                                      //Cada columna de la matriz corresponde a: Total; False; PFI; minimum PFI

        List<List<int>> aircraftDetected;//Matriz con los aviones estructurados segun su ICAO Adress
        List<List<int>> aircraftTrackDetected;//Matriz con los aviones estructurados segun su Track Number

        public Avaluador(LectorMensaje listaActual)//Contructor en caso de analizar MLAT
        {
            this.myList = listaActual;
            aircraftDetected = myList.AircraftICAODetected();
            aircraftTrackDetected = myList.AircraftTrackNumberDetected();
            SetParameters("MLAT");
        }

        public Avaluador(LectorMensaje listaActual, LectorMensaje listaDGPS)//Constructor en caso de analizar MLAT + D-GPS
        {
            this.DGPSList = listaDGPS;//se usa en AdaptarListaActual
            this.listaPA = listaActual;
            this.myList = AdaptarListaActual(listaActual);//Hace remove de todos los paquetes que no son del vehiculo.
            aircraftDetected = myList.AircraftICAODetected();
            aircraftTrackDetected = myList.AircraftTrackNumberDetected();
            diferencias = SetDiferencias();
            SetParameters("DGPS");
        }

        public void SetParameters(string mode)
        {
            if (mode=="MLAT")
            {
                SetUpdateRate();
                SetProbOfMLATDetection();
                SetProbOfIdentification();
                SetProbOfFalseDetection();
                SetProbOfFalseIdentification();
            }
            else if (mode=="DGPS")
            {
                //Contiene PA, y ProbOfFalseDetDGPS
                SetUpdateRate();
                SetPositionAccuracy();
                SetProbOfMLATDetection();
                SetProbOfIdentification();
                SetProbOfFalseDetectionDGPS();
                SetProbOfFalseIdentification();
            }
        }

        public void SetUpdateRate()
        {
            updateRate = new double[15, 4];
            updatesUnitarios = new double[aircraftDetected.Count(), 3]; //0=real, 1=expected

            //Updates Reales
            bool tiempoSinSeñalSuperado = false;//Si tiempo actual es más grande que el anterior +1min
            for (int i = 0; i < aircraftDetected.Count(); i++)
            {
                int zonaActual = myList.GetPlanI(aircraftDetected[i][0]).GetZona();
                int indiceInicial = 0;
                updatesUnitarios[i, 2] = aircraftDetected[i][0];//Guardamos el indice para obtener icao Address
                for (int n = 1; n < aircraftDetected[i].Count(); n++)
                {
                    if (myList.GetPlanI(aircraftDetected[i][n]).GetUTC() < myList.GetPlanI(aircraftDetected[i][n - 1]).GetUTC())//Filtramos el caso de paso de 24h a 0h
                    {
                        if (myList.GetPlanI(aircraftDetected[i][n]).GetUTC() + 24 * 60 > myList.GetPlanI(aircraftDetected[i][n - 1]).GetUTC() + 1)//Si tiempo actual es más grande que el anterior +1min, dividimos en 2 el vuelo
                        {
                            tiempoSinSeñalSuperado = true;
                        }
                    }
                    else if (myList.GetPlanI(aircraftDetected[i][n]).GetUTC() > myList.GetPlanI(aircraftDetected[i][n - 1]).GetUTC() + 1)//Si tiempo actual es más grande que el anterior +1min, dividimos en 2 el vuelo
                    {
                        tiempoSinSeñalSuperado = true;
                    }

                    if ((myList.GetPlanI(aircraftDetected[i][n]).GetZona() != zonaActual) || (n == aircraftDetected[i].Count() - 1) || (tiempoSinSeñalSuperado))
                    {
                        tiempoSinSeñalSuperado = false;
                        int indiceFinal;
                        if (n != aircraftDetected[i].Count() - 1)
                            indiceFinal = n - 1;
                        else indiceFinal = n;

                        int updates = 1;
                        for (int m = indiceInicial + 1; m <= indiceFinal; m++)
                        {
                            double tiempoInicial = myList.GetPlanI(aircraftDetected[i][m - 1]).GetUTC() * 60;
                            double tiempoFinal = myList.GetPlanI(aircraftDetected[i][m]).GetUTC() * 60;
                            if (tiempoFinal < tiempoInicial)
                            {
                                tiempoFinal = tiempoFinal + 24 * 3600;
                            }
                            if (tiempoFinal <= tiempoInicial+1)
                            {
                                updates++;
                            }
                        }

                        if (zonaActual == 11)
                        {
                            updateRate[1, 0] = updateRate[1, 0] + updates;
                        }
                        else if (zonaActual == 12)
                        {
                            updateRate[2, 0] = updateRate[2, 0] + updates;
                        }
                        else if (zonaActual == 13)
                        {
                            updateRate[3, 0] = updateRate[3, 0] + updates;
                        }
                        else if (zonaActual == 4)
                        {
                            updateRate[4, 0] = updateRate[4, 0] + updates;
                        }
                        else if (zonaActual == 31)
                        {
                            updateRate[6, 0] = updateRate[6, 0] + updates;
                        }
                        else if (zonaActual == 32)
                        {
                            updateRate[7, 0] = updateRate[7, 0] + updates;
                        }
                        else if (zonaActual == 21)
                        {
                            updateRate[9, 0] = updateRate[9, 0] + updates;
                        }
                        else if (zonaActual == 22)
                        {
                            updateRate[10, 0] = updateRate[10, 0] + updates;
                        }
                        else if (zonaActual == 01)
                        {
                            updateRate[12, 0] = updateRate[12, 0] + updates;
                        }
                        else if (zonaActual == 02)
                        {
                            updateRate[13, 0] = updateRate[13, 0] + updates;
                        }
                        else if (zonaActual == 0)
                        {
                            updateRate[14, 0] = updateRate[14, 0] + updates;
                        }
                        updatesUnitarios[i, 0] = updatesUnitarios[i, 0] + updates;
                        zonaActual = myList.GetPlanI(aircraftDetected[i][n]).GetZona();
                        indiceInicial = n;
                    }
                }
            }

            //Expected
            tiempoSinSeñalSuperado = false;
            for (int i = 0; i < aircraftDetected.Count(); i++)
            {
                int zonaActual = myList.GetPlanI(aircraftDetected[i][0]).GetZona();
                double tiempoInicial = myList.GetPlanI(aircraftDetected[i][0]).GetUTCcorregido() * 60;
                for (int n = 1; n < aircraftDetected[i].Count(); n++)
                {
                    if (myList.GetPlanI(aircraftDetected[i][n]).GetUTCcorregido() > myList.GetPlanI(aircraftDetected[i][n - 1]).GetUTCcorregido() + 1)//Si tiempo actual es más grande que el anterior +1min, dividimos en 2 el vuelo
                    {
                        tiempoSinSeñalSuperado = true;
                    }

                    if ((myList.GetPlanI(aircraftDetected[i][n]).GetZona() != zonaActual) || (n == aircraftDetected[i].Count() - 1) || (tiempoSinSeñalSuperado))
                    {
                        tiempoSinSeñalSuperado = false;
                        double tiempoFinal;
                        if (n != aircraftDetected[i].Count() - 1)//Miramos si se trata del ultimo mensaje del avion
                            tiempoFinal = myList.GetPlanI(aircraftDetected[i][n - 1]).GetUTCcorregido() * 60;
                        else tiempoFinal = myList.GetPlanI(aircraftDetected[i][n]).GetUTCcorregido() * 60;

                        double updates = tiempoFinal - tiempoInicial + 1;
                        if (zonaActual == 11)
                        {
                            updateRate[1, 1] = updateRate[1, 1] + updates;
                        }
                        else if (zonaActual == 12)
                        {
                            updateRate[2, 1] = updateRate[2, 1] + updates;
                        }
                        else if (zonaActual == 13)
                        {
                            updateRate[3, 1] = updateRate[3, 1] + updates;
                        }
                        else if (zonaActual == 4)
                        {
                            updateRate[4, 1] = updateRate[4, 1] + updates;
                        }
                        else if (zonaActual == 31)
                        {
                            updateRate[6, 1] = updateRate[6, 1] + updates;
                        }
                        else if (zonaActual == 32)
                        {
                            updateRate[7, 1] = updateRate[7, 1] + updates;
                        }
                        else if (zonaActual == 21)
                        {
                            updateRate[9, 1] = updateRate[9, 1] + updates;
                        }
                        else if (zonaActual == 22)
                        {
                            updateRate[10, 1] = updateRate[10, 1] + updates;
                        }
                        else if (zonaActual == 01)
                        {
                            updateRate[12, 1] = updateRate[12, 1] + updates;
                        }
                        else if (zonaActual == 02)
                        {
                            updateRate[13, 1] = updateRate[13, 1] + updates;
                        }
                        else if (zonaActual == 0)
                        {
                            updateRate[14, 1] = updateRate[14, 1] + updates;
                        }
                        updatesUnitarios[i, 1] = updatesUnitarios[i, 1] + updates;
                        zonaActual = myList.GetPlanI(aircraftDetected[i][n]).GetZona();
                        tiempoInicial = myList.GetPlanI(aircraftDetected[i][n]).GetUTCcorregido() * 60;
                    }
                }
            }

            //Totales
            for (int i = 0; i < 2; i++)
            {
                updateRate[0, i] = updateRate[1, i] + updateRate[2, i] + updateRate[3, i] + updateRate[4, i];//Total maneouvering
                updateRate[5, i] = updateRate[6, i] + updateRate[7, i];//Total Apron
                updateRate[8, i] = updateRate[9, i] + updateRate[10, i];//Total Stand
                updateRate[11, i] = updateRate[12, i] + updateRate[13, i] + updateRate[14, i];//Airborne
            }

            //Update rates
            for (int i = 0; i < updateRate.GetLength(0); i++)
            {
                if (updateRate[i, 1] != 0)
                    updateRate[i, 2] = updateRate[i, 0]*100 / updateRate[i, 1];//%
                else updateRate[i, 2] = 0;
            }

            // Update rate minimo segun doc ED117
            updateRate[0, 3] = 95;//%
            updateRate[5, 3] = 70;
            updateRate[8, 3] = 50;
            updateRate[11, 3] = 95;
        }

        public void SetPositionAccuracy()
        {
            PosAcc = new double[12, 7];
            List<double[]> diferencia = new List<double[]>(2);//posición 1= diferencia; posicion 2=zona

            for (int i = 0; i < diferencias.Count(); i++)
            {
                double[] diferenciaActual = new double[2]; //[0]=Sqrt((X1-X2)^2+(Y1-Y2)^2); [1]=Zona
                diferenciaActual[0] = Math.Sqrt(Math.Pow(diferencias[i][0], 2) + Math.Pow(diferencias[i][1], 2));
                diferenciaActual[1] = diferencias[i][2];
                diferencia.Add(diferenciaActual);
            }
    
            List<int> zonas = new List<int>();

            for (int i = 0; i < PosAcc.GetLength(0); i++)//Inicialmente -1. Si no hay información de la zona se quitará el -1 por null
            {
                for (int j = 0; j < PosAcc.GetLength(1); j++)
                {
                    if ((j != 3) && (j != 4))
                    {
                        PosAcc[i, j] = -1;
                    }
                }
            }

            //P95, P99, Mean
            zonas.Add(11);zonas.Add(12); zonas.Add(13); zonas.Add(4); //Total Maneouvering
            SetPosAccValues(0, diferencia, zonas);
            zonas.Clear();

            zonas.Add(11);  //11
            SetPosAccValues(1, diferencia, zonas);
            zonas.Clear();

            zonas.Add(12);  //12
            SetPosAccValues(2, diferencia, zonas);
            zonas.Clear();

            zonas.Add(13);
            SetPosAccValues(3, diferencia, zonas);
            zonas.Clear();

            zonas.Add(4); //4
            SetPosAccValues(4, diferencia, zonas);
            zonas.Clear();

            zonas.Add(31); zonas.Add(32); //Total Apron
            SetPosAccValues(5, diferencia, zonas);
            zonas.Clear();

            zonas.Add(31);  //31
            SetPosAccValues(6, diferencia, zonas);
            zonas.Clear();

            zonas.Add(32); //32
            SetPosAccValues(7, diferencia, zonas);
            zonas.Clear();

            zonas.Add(21); zonas.Add(22); //Total Stand
            SetPosAccValues(8, diferencia, zonas);
            zonas.Clear();

            zonas.Add(21); //21
            SetPosAccValues(9, diferencia, zonas);
            zonas.Clear();

            zonas.Add(22); //22
            SetPosAccValues(10, diferencia, zonas);
            zonas.Clear();

            //Max Value Detected
            double promedioTemporal = 0;
            double zonaActual = diferencia[0][1];
            int recuento = 1; //mira si la ventana llega a 5
            for (int i = 1; i < diferencia.Count(); i++)
            {
                if (diferencia[i][1] == zonaActual)
                {
                    recuento++;
                }
                else
                {
                    recuento = 1;
                    zonaActual = diferencia[i][1];
                }

                if (recuento == 5)//Ventana de 5
                {
                    promedioTemporal = (diferencia[i][1] + diferencia[i - 1][1] + diferencia[i - 2][1] + diferencia[i - 3][1] + diferencia[i - 4][1]) / 5;
                    if (zonaActual == 11)
                    {
                        if (promedioTemporal> PosAcc[1, 2])
                            PosAcc[1, 2] = promedioTemporal;
                    }
                    else if (zonaActual == 12)
                    {
                        if (promedioTemporal > PosAcc[2, 2])
                            PosAcc[2, 2] = promedioTemporal;
                    }
                    else if (zonaActual == 13)
                    {
                        if (promedioTemporal > PosAcc[3, 2])
                            PosAcc[3, 2] = promedioTemporal;
                    }
                    else if (zonaActual == 4)
                    {
                        if (promedioTemporal > PosAcc[4, 2])
                            PosAcc[4, 2] = promedioTemporal;
                    }
                    else if (zonaActual == 31)
                    {
                        if (promedioTemporal > PosAcc[6, 2])
                            PosAcc[6, 2] = promedioTemporal;
                    }
                    else if (zonaActual == 32)
                    {
                        if (promedioTemporal > PosAcc[7, 2])
                            PosAcc[7, 2] = promedioTemporal;
                    }
                    else if (zonaActual == 21)
                    {
                        if (promedioTemporal > PosAcc[9, 2])
                            PosAcc[9, 2] = promedioTemporal;
                    }
                    else if (zonaActual == 22)
                    {
                        if (promedioTemporal > PosAcc[10, 2])
                            PosAcc[10, 2] = promedioTemporal;
                    }

                    promedioTemporal = 0;
                    recuento = 1;
                    i++;
                    if (i < diferencia.Count())
                    {
                        zonaActual = diferencia[i][1];
                    }
                }
            }

            double promedioSeccion = PosAcc[1, 2];//Promedio Seccion Maneouvering Area
            for (int i = 2; i <= 4; i++)
            {
                if (PosAcc[i, 2] > promedioSeccion)
                {
                    promedioSeccion = PosAcc[i, 2];
                }
            }
            PosAcc[0, 2] = promedioSeccion;

            if (PosAcc[7, 2] > PosAcc[6, 2]) //Promedio Seccion Apron
            {
                PosAcc[5, 2] = PosAcc[7, 2];
            }
            else PosAcc[5, 2] = PosAcc[6, 2];

            if (PosAcc[10, 2] > PosAcc[9, 2]) //Promedio Seccion Stand
            {
                PosAcc[8, 2] = PosAcc[10, 2];
            }
            else PosAcc[8, 2] = PosAcc[9, 2];

            PosAcc[0, 3] = 7.5; //Max Maneouvering P95
            PosAcc[0, 4] = 12; //Max Maneouvering P99
            PosAcc[5, 3] = 7.5; //Max Apron P95
            PosAcc[5, 4] = 12; //Max Apron P99
        }

        public void SetProbOfMLATDetection()
        {
            MLATDet = new double[11, 4];

            //Detecciones reales & Expected
            bool tiempoSinSeñalSuperado = false; //Si tiempo actual es más grande que el anterior +1min
            for (int i = 0; i < aircraftDetected.Count(); i++)
            {
                int zonaActual = myList.GetPlanI(aircraftDetected[i][0]).GetZona();
                int indiceInicial = 0;
                double tiempoInicial = myList.GetPlanI(aircraftDetected[i][0]).GetUTCcorregido() * 60;
                for (int n = 1; n < aircraftDetected[i].Count(); n++)
                {
                    if (myList.GetPlanI(aircraftDetected[i][n]).GetUTCcorregido() > myList.GetPlanI(aircraftDetected[i][n - 1]).GetUTCcorregido() + 1)//Si tiempo actual es más grande que el anterior +1min, dividimos en 2 el vuelo
                    {
                        tiempoSinSeñalSuperado = true;
                    }

                    if ((myList.GetPlanI(aircraftDetected[i][n]).GetZona() != zonaActual) || (n == aircraftDetected[i].Count() - 1) || (tiempoSinSeñalSuperado))
                    {
                        tiempoSinSeñalSuperado = false;
                        if ((zonaActual == 11) || (zonaActual == 12) || (zonaActual == 13) || (zonaActual == 4) || (zonaActual == 21) || (zonaActual == 22) || (zonaActual == 31)|| (zonaActual == 32))
                        {
                            int indiceFinal;
                            if (n != aircraftDetected[i].Count() - 1)
                                indiceFinal = n - 1;
                            else indiceFinal = n;

                            double ventanaTiempo = 2;//En segundos
                            if ((zonaActual == 21) || (zonaActual == 22))
                            {
                                ventanaTiempo = 5;
                            }

                            double tiempoFinal = myList.GetPlanI(aircraftDetected[i][indiceFinal]).GetUTCcorregido() * 60;
                            double tiempoFinalVentana = tiempoInicial + ventanaTiempo;
                            int ventanas = 0;
                            int ventanasDetectadas = 0;

                            while (tiempoFinalVentana <= tiempoFinal)
                            {
                                bool ventanaCumple = false;
                                int m = indiceInicial;
                                while ((m <= indiceFinal) && (!ventanaCumple))
                                {
                                    if (myList.GetPlanI(aircraftDetected[i][m]).GetUTCcorregido() * 60 >= tiempoInicial && myList.GetPlanI(aircraftDetected[i][m]).GetUTCcorregido() * 60 <= tiempoFinalVentana)
                                    {
                                        ventanaCumple = true;
                                        ventanasDetectadas++;
                                    }
                                    m++;
                                }
                                ventanas++;
                                tiempoInicial = tiempoInicial + 1;
                                tiempoFinalVentana = tiempoInicial + ventanaTiempo;
                            }

                            if (zonaActual == 11)
                            {
                                MLATDet[1, 0] = MLATDet[1, 0] + ventanasDetectadas;
                                MLATDet[1, 1] = MLATDet[1, 1] + ventanas;
                            }
                            else if (zonaActual == 12)
                            {
                                MLATDet[2, 0] = MLATDet[2, 0] + ventanasDetectadas;
                                MLATDet[2, 1] = MLATDet[2, 1] + ventanas;
                            }
                            else if (zonaActual == 13)
                            {
                                MLATDet[3, 0] = MLATDet[3, 0] + ventanasDetectadas;
                                MLATDet[3, 1] = MLATDet[3, 1] + ventanas;
                            }
                            else if (zonaActual == 4)
                            {
                                MLATDet[4, 0] = MLATDet[4, 0] + ventanasDetectadas;
                                MLATDet[4, 1] = MLATDet[4, 1] + ventanas;
                            }
                            else if (zonaActual == 31)
                            {
                                MLATDet[5, 0] = MLATDet[5, 0] + ventanasDetectadas;
                                MLATDet[5, 1] = MLATDet[5, 1] + ventanas;
                            }
                            else if (zonaActual == 32)
                            {
                                MLATDet[6, 0] = MLATDet[6, 0] + ventanasDetectadas;
                                MLATDet[6, 1] = MLATDet[6, 1] + ventanas;
                            }
                            else if (zonaActual == 21)
                            {
                                MLATDet[8, 0] = MLATDet[8, 0] + ventanasDetectadas;
                                MLATDet[8, 1] = MLATDet[8, 1] + ventanas;
                            }
                            else if (zonaActual == 22)
                            {
                                MLATDet[9, 0] = MLATDet[9, 0] + ventanasDetectadas;
                                MLATDet[9, 1] = MLATDet[9, 1] + ventanas;
                            }
                            //Airborne not required
                        }
                        zonaActual = myList.GetPlanI(aircraftDetected[i][n]).GetZona();
                        indiceInicial = n;
                        tiempoInicial = myList.GetPlanI(aircraftDetected[i][n]).GetUTCcorregido() * 60;
                    }
                }
            }

            //Totales
            for (int i = 0; i < 2; i++)
            {
                MLATDet[0, i] = MLATDet[1, i] + MLATDet[2, i] + MLATDet[3, i] + MLATDet[4, i] + MLATDet[5, i] + MLATDet[6, i]; //Total maneouvering + Apron
                MLATDet[7, i] = MLATDet[8, i] + MLATDet[9, i]; //Total Stand
            }

            //Prob. of detection
            for (int i = 0; i < MLATDet.GetLength(0); i++)
            {
                if (MLATDet[i, 1] != 0)
                    MLATDet[i, 2] = MLATDet[i, 0] * 100 / MLATDet[i, 1];
                else MLATDet[i, 2] = 0;
            }

            //Prob. of detection minimo segun doc ED117
            MLATDet[0, 3] = 99; //[%]
            MLATDet[7, 3] = 99;
        }

        public void SetProbOfIdentification()
        {
            Identification = new double[16, 4];

            //Incorrect & Correct
            List<string> ICAOdiferentes = new List<string>();
            List<int> ICAOdiferentesCount = new List<int>();
            for (int i = 0; i < aircraftTrackDetected.Count(); i++)
            {
                ICAOdiferentes.Clear();
                ICAOdiferentesCount.Clear();
                ICAOdiferentes.Add(myList.GetPlanI(aircraftTrackDetected[i][0]).GetICAOAdress());
                ICAOdiferentesCount.Add(1);
                for (int n = 1; n < aircraftTrackDetected[i].Count(); n++)
                {
                    for (int m = 0; m < ICAOdiferentes.Count(); m++)
                    {
                        if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetICAOAdress() == ICAOdiferentes[m])
                        {
                            List<int> ICAOdiferentesCountTemporal = new List<int>();
                            for (int j = 0; j < ICAOdiferentesCount.Count(); j++)
                            {
                                if (m == j)
                                {
                                    ICAOdiferentesCountTemporal.Add(ICAOdiferentesCount[j] + 1);
                                }
                                else ICAOdiferentesCountTemporal.Add(ICAOdiferentesCount[j]);
                            }
                            ICAOdiferentesCount.Clear();
                            ICAOdiferentesCount = ICAOdiferentesCountTemporal;
                        }
                        else
                        {
                            ICAOdiferentes.Add(myList.GetPlanI(aircraftTrackDetected[i][n]).GetICAOAdress());
                            ICAOdiferentesCount.Add(1);
                        }
                    }
                }
                int posicionGanadora = 0;
                if (ICAOdiferentesCount.Count() > 1)
                {
                    for (int m = 1; m < ICAOdiferentesCount.Count(); m++)
                    {
                        if (ICAOdiferentesCount[m] > ICAOdiferentesCount[posicionGanadora])
                        {
                            posicionGanadora = m;
                        }
                    }
                }

                for (int n = 0; n < aircraftTrackDetected[i].Count(); n++)
                {
                    int correcto = 0;
                    int incorrecto = 0;
                    if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetICAOAdress() != ICAOdiferentes[posicionGanadora])
                    {
                        incorrecto = 1;
                    }
                    else correcto = 1;

                    if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 11)
                    {
                        Identification[1, 0] = Identification[1, 0] + correcto;
                        Identification[1, 1] = Identification[1, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 12)
                    {
                        Identification[2, 0] = Identification[2, 0] + correcto;
                        Identification[2, 1] = Identification[2, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 13)
                    {
                        Identification[3, 0] = Identification[3, 0] + correcto;
                        Identification[3, 1] = Identification[3, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 4)
                    {
                        Identification[4, 0] = Identification[4, 0] + correcto;
                        Identification[4, 1] = Identification[4, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 31)
                    {
                        Identification[6, 0] = Identification[6, 0] + correcto;
                        Identification[6, 1] = Identification[6, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 32)
                    {
                        Identification[7, 0] = Identification[7, 0] + correcto;
                        Identification[7, 1] = Identification[7, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 21)
                    {
                        Identification[9, 0] = Identification[9, 0] + correcto;
                        Identification[9, 1] = Identification[9, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 22)
                    {
                        Identification[10, 0] = Identification[10, 0] + correcto;
                        Identification[10, 1] = Identification[10, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 01)
                    {
                        Identification[12, 0] = Identification[12, 0] + correcto;
                        Identification[12, 1] = Identification[12, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 02)
                    {
                        Identification[13, 0] = Identification[13, 0] + correcto;
                        Identification[13, 1] = Identification[13, 1] + incorrecto;
                    }
                    else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 0)
                    {
                        Identification[14, 0] = Identification[14, 0] + correcto;
                        Identification[14, 1] = Identification[14, 1] + incorrecto;
                    }
                }
            }

            //Totales
            for (int i = 0; i < 2; i++)
            {
                Identification[0, i] = Identification[1, i] + Identification[2, i] + Identification[3, i] + Identification[4, i];//Total maneouvering
                Identification[5, i] = Identification[6, i] + Identification[7, i];//Total Apron
                Identification[8, i] = Identification[9, i] + Identification[10, i];//Total Stand
                Identification[11, i] = Identification[12, i] + Identification[13, i] + Identification[14, i];//Airborne
                Identification[15, i] = Identification[0, i] + Identification[5, i] + Identification[8, i] + Identification[11, i]; //Suma de todos
            }

            //PID
            for (int i = 0; i < Identification.GetLength(0); i++)
            {
                if (Identification[i, 0] + Identification[i, 1] != 0)
                {
                    Identification[i, 2] = Identification[i, 0] * 100 / (Identification[i, 0] + Identification[i, 1]);
                }
            }

            // PID minimo segun doc ED117
            Identification[15, 3] = 99; //[%]
        }

        public void SetProbOfFalseDetection()
        {
            FalseDetection = new double[15, 4];

            //Reports
            for (int i = 0; i < aircraftDetected.Count(); i++)
            {
                for (int n = 0; n < aircraftDetected[i].Count(); n++)
                {
                    int zonaActual = myList.GetPlanI(aircraftDetected[i][n]).GetZona();
                    if (zonaActual == 11)
                    {
                        FalseDetection[1, 0] = FalseDetection[1, 0] + 1;
                    }
                    else if (zonaActual == 12)
                    {
                        FalseDetection[2, 0] = FalseDetection[2, 0] + 1;
                    }
                    else if (zonaActual == 13)
                    {
                        FalseDetection[3, 0] = FalseDetection[3, 0] + 1;
                    }
                    else if (zonaActual == 4)
                    {
                        FalseDetection[4, 0] = FalseDetection[4, 0] + 1;
                    }
                    else if (zonaActual == 31)
                    {
                        FalseDetection[6, 0] = FalseDetection[6, 0] + 1;
                    }
                    else if (zonaActual == 32)
                    {
                        FalseDetection[7, 0] = FalseDetection[7, 0] + 1;
                    }
                    else if (zonaActual == 21)
                    {
                        FalseDetection[9, 0] = FalseDetection[9, 0] + 1;
                    }
                    else if (zonaActual == 22)
                    {
                        FalseDetection[10, 0] = FalseDetection[10, 0] + 1;
                    }
                    else if (zonaActual == 01)
                    {
                        FalseDetection[12, 0] = FalseDetection[12, 0] + 1;
                    }
                    else if (zonaActual == 02)
                    {
                        FalseDetection[13, 0] = FalseDetection[13, 0] + 1;
                    }
                }
            }

            //False Reports
            for (int i = 0; i < aircraftDetected.Count(); i++)
            {
                int zonaInicial = myList.GetPlanI(aircraftDetected[i][0]).GetZona();
                double tiempoInicial = myList.GetPlanI(aircraftDetected[i][0]).GetUTCcorregido() * 60;
                double[] posicionInicial = GetPosicion(aircraftDetected[i][0]);
                for (int n = 1; n < aircraftDetected[i].Count(); n++)
                {
                    int zonaActual = myList.GetPlanI(aircraftDetected[i][n]).GetZona();
                    double tiempoActual = myList.GetPlanI(aircraftDetected[i][n]).GetUTCcorregido() * 60;
                    double[] posicionActual = GetPosicion(aircraftDetected[i][n]);
                    bool falseDetection = false;//Controla si ha habido false detection
                    if ((zonaInicial != 0) && (zonaInicial != -1))
                    {
                        if ((zonaActual != 0) && (zonaActual != -1))
                        {
                            bool mismaZona = false; //solo podemos comparar zonas iguales, ya que las distancia umbral cambia
                            int distUmbral = 50; //[m]. Para la zona aeroportuaria
                            if (zonaInicial == 01)
                            {
                                if (zonaActual == 01)
                                {
                                    mismaZona = true;
                                    distUmbral = 80; //[m]
                                }
                            }
                            else if (zonaInicial == 02)
                            {
                                if (zonaActual == 02)
                                {
                                    mismaZona = true;
                                    distUmbral = 160; //[m]
                                }
                            }
                            else//No es 0, -1, 01, 02; por lo que distUmbral será 50
                            {
                                if ((zonaActual != 01) && (zonaActual != 02))
                                {
                                    mismaZona = true;
                                }
                            }

                            if (mismaZona)//Solo comparamos zonas iguales: zona aeroportuaria, zona 4 o zona 5
                            {
                                if (tiempoActual <= tiempoInicial + 1)//Si la señal n llega 1s despues de la señal n-1
                                {
                                    if ((myList.GetPlanI(aircraftDetected[i][n]).GetCartTrackVelX() != Math.Pow(10, 8)) && (myList.GetPlanI(aircraftDetected[i][n]).GetCartTrackVelY() != Math.Pow(10, 8)))
                                    {
                                        posicionInicial[0] = posicionInicial[0]+ myList.GetPlanI(aircraftDetected[i][n]).GetCartTrackVelX(); //X=Xo+V*t (t=1s) ==> X=Xo+V
                                        posicionInicial[1] = posicionInicial[1] + myList.GetPlanI(aircraftDetected[i][n]).GetCartTrackVelY(); //Calculamos punto ficticio
                                        double distancia = Math.Sqrt(Math.Pow(posicionActual[0] - posicionInicial[0], 2) + Math.Pow(posicionActual[1] - posicionInicial[1], 2));
                                        if (distancia > distUmbral)
                                        {
                                            falseDetection = true;
                                            n++;//Saltamos un mensaje, ya que el mensaje falso seguramente esté a >50m del anterior y también del siguiente
                                        }
                                    }
                                }
                            }
                            if (falseDetection)
                            {
                                if (zonaActual == 11)
                                {
                                    FalseDetection[1, 1] = FalseDetection[1, 1] + 1;
                                }
                                else if (zonaActual == 12)
                                {
                                    FalseDetection[2, 1] = FalseDetection[2, 1] + 1;
                                }
                                else if (zonaActual == 13)
                                {
                                    FalseDetection[3, 1] = FalseDetection[3, 1] + 1;
                                }
                                else if (zonaActual == 4)
                                {
                                    FalseDetection[4, 1] = FalseDetection[4, 1] + 1;
                                }
                                else if (zonaActual == 31)
                                {
                                    FalseDetection[6, 1] = FalseDetection[6, 1] + 1;
                                }
                                else if (zonaActual == 32)
                                {
                                    FalseDetection[7, 1] = FalseDetection[7, 1] + 1;
                                }
                                else if (zonaActual == 21)
                                {
                                    FalseDetection[9, 1] = FalseDetection[9, 1] + 1;
                                }
                                else if (zonaActual == 22)
                                {
                                    FalseDetection[10, 1] = FalseDetection[10, 1] + 1;
                                }
                                else if (zonaActual == 01)
                                {
                                    FalseDetection[12, 1] = FalseDetection[12, 1] + 1;
                                }
                                else if (zonaActual == 02)
                                {
                                    FalseDetection[13, 1] = FalseDetection[13, 1] + 1;
                                }
                            }
                        }
                    }
                    if (falseDetection)
                    {
                        if (n < aircraftDetected[i].Count() - 1)//Para que no salte error con la ultima posicion del vector
                        {
                            zonaInicial = myList.GetPlanI(aircraftDetected[i][n]).GetZona();
                            tiempoInicial = myList.GetPlanI(aircraftDetected[i][n]).GetUTCcorregido() * 60;
                            posicionInicial = GetPosicion(aircraftDetected[i][n]);
                        }
                        //if not, el bucle acabará en este ciclo
                    }
                    else
                    {
                        zonaInicial = zonaActual;
                        tiempoInicial = tiempoActual;
                        posicionInicial = posicionActual;
                    }
                }
            }

            //Totales
            for (int i = 0; i < 2; i++)
            {
                FalseDetection[0, i] = FalseDetection[1, i] + FalseDetection[2, i] + FalseDetection[3, i] + FalseDetection[4, i];//Total maneouvering
                FalseDetection[5, i] = FalseDetection[6, i] + FalseDetection[7, i];//Total Apron
                FalseDetection[8, i] = FalseDetection[9, i] + FalseDetection[10, i];//Total Stand
                FalseDetection[11, i] = FalseDetection[12, i] + FalseDetection[13, i];//Airborne (solo tipo 4 y 5)
                FalseDetection[14, i] = FalseDetection[0, i] + FalseDetection[5, i] + FalseDetection[8, i] + FalseDetection[11, i]; //Suma de todos
            }

            //Prob. of detection
            for (int i = 0; i < FalseDetection.GetLength(0); i++)
            {
                if (FalseDetection[i, 0] != 0)
                    FalseDetection[i, 2] = FalseDetection[i, 1] * 100 / FalseDetection[i, 0]; // [%]
                else FalseDetection[i, 2] = 0;
            }

            //Prob. of detection minimo segun doc ED117
            FalseDetection[14, 3] = 0.01; // [%]
        }

        public void SetProbOfFalseDetectionDGPS()
        {
            FalseDetection = new double[15, 4];
            List<double[]> diferencia = new List<double[]>(2);//posición 1= diferencia; posicion 2=zona

            for (int i = 0; i < DGPSList.GetNumList(); i++)
            {
                if (DGPSList.GetPlanI(i).GetIndiceDGPS() != -1)
                {
                    double[] diferenciaActual = new double[2]; //[0]=Sqrt((X1-X2)^2+(Y1-Y2)^2); [1]=Zona
                    diferenciaActual[0] = Math.Sqrt(Math.Pow(DGPSList.GetPlanI(i).GetPosicion()[0] - listaPA.GetPlanI(DGPSList.GetPlanI(i).GetIndiceDGPS()).GetPosicion()[0], 2) + Math.Pow(DGPSList.GetPlanI(i).GetPosicion()[1] - listaPA.GetPlanI(DGPSList.GetPlanI(i).GetIndiceDGPS()).GetPosicion()[1], 2));
                    diferenciaActual[1] = listaPA.GetPlanI(DGPSList.GetPlanI(i).GetIndiceDGPS()).GetZona();
                    diferencia.Add(diferenciaActual);
                }
            }

            //Reports & False Reports
            for (int i = 0; i < diferencia.Count(); i++)
            {
                double zonaActual = diferencia[i][1];
                int falso = 0;
                if (diferencia[i][0] > 50)
                {
                    falso = 1;
                }
                if (zonaActual == 11)
                {
                    FalseDetection[1, 0] = FalseDetection[1, 0] + 1;
                    FalseDetection[1, 1] = FalseDetection[1, 1] + falso;
                }
                else if (zonaActual == 12)
                {
                    FalseDetection[2, 0] = FalseDetection[2, 0] + 1;
                    FalseDetection[2, 1] = FalseDetection[2, 1] + falso;
                }
                else if (zonaActual == 13)
                {
                    FalseDetection[3, 0] = FalseDetection[3, 0] + 1;
                    FalseDetection[3, 1] = FalseDetection[3, 1] + falso;
                }
                else if (zonaActual == 4)
                {
                    FalseDetection[4, 0] = FalseDetection[4, 0] + 1;
                    FalseDetection[4, 1] = FalseDetection[4, 1] + falso;
                }
                else if (zonaActual == 31)
                {
                    FalseDetection[6, 0] = FalseDetection[6, 0] + 1;
                    FalseDetection[6, 1] = FalseDetection[6, 1] + falso;
                }
                else if (zonaActual == 32)
                {
                    FalseDetection[7, 0] = FalseDetection[7, 0] + 1;
                    FalseDetection[7, 1] = FalseDetection[7, 1] + falso;
                }
                else if (zonaActual == 21)
                {
                    FalseDetection[9, 0] = FalseDetection[9, 0] + 1;
                    FalseDetection[9, 1] = FalseDetection[9, 1] + falso;
                }
                else if (zonaActual == 22)
                {
                    FalseDetection[10, 0] = FalseDetection[10, 0] + 1;
                    FalseDetection[10, 1] = FalseDetection[10, 1] + falso;
                }
                else if (zonaActual == 01)
                {
                    FalseDetection[12, 0] = FalseDetection[12, 0] + 1;
                    FalseDetection[12, 1] = FalseDetection[12, 1] + falso;
                }
                else if (zonaActual == 02)
                {
                    FalseDetection[13, 0] = FalseDetection[13, 0] + 1;
                    FalseDetection[13, 1] = FalseDetection[13, 1] + falso;
                }
            }

            //Totales
            for (int i = 0; i < 2; i++)
            {
                FalseDetection[0, i] = FalseDetection[1, i] + FalseDetection[2, i] + FalseDetection[3, i] + FalseDetection[4, i];//Total maneouvering
                FalseDetection[5, i] = FalseDetection[6, i] + FalseDetection[7, i];//Total Apron
                FalseDetection[8, i] = FalseDetection[9, i] + FalseDetection[10, i];//Total Stand
                FalseDetection[11, i] = FalseDetection[12, i] + FalseDetection[13, i];//Airborne (solo tipo 4 y 5)
                FalseDetection[14, i] = FalseDetection[0, i] + FalseDetection[5, i] + FalseDetection[8, i] + FalseDetection[11, i]; //Suma de todos
            }

            //Prob. of detection
            for (int i = 0; i < FalseDetection.GetLength(0); i++)
            {
                if (FalseDetection[i, 0] != 0)
                    FalseDetection[i, 2] = FalseDetection[i, 1] * 100 / FalseDetection[i, 0]; // [%]
                else FalseDetection[i, 2] = 0;
            }

            //Prob. of detection minimo segun doc ED117
            FalseDetection[14, 3] = 0.01; // [%]
        }

        public void SetProbOfFalseIdentification()
        {
            FalseIdentification = new double[16, 4];

            //Incorrect & Correct
            List<string> ICAOdiferentes = new List<string>();
            List<int> ICAOdiferentesCount = new List<int>();
            for (int i = 0; i < aircraftTrackDetected.Count(); i++)
            {
                ICAOdiferentes.Clear();
                ICAOdiferentesCount.Clear();
                ICAOdiferentes.Add(myList.GetPlanI(aircraftTrackDetected[i][0]).GetICAOAdress());
                ICAOdiferentesCount.Add(1);
                for (int n = 1; n < aircraftTrackDetected[i].Count(); n++)
                {
                    for (int m = 0; m < ICAOdiferentes.Count(); m++)
                    {
                        if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetICAOAdress() == ICAOdiferentes[m])
                        {
                            List<int> ICAOdiferentesCountTemporal = new List<int>();
                            for (int j = 0; j < ICAOdiferentesCount.Count(); j++)
                            {
                                if (m == j)
                                {
                                    ICAOdiferentesCountTemporal.Add(ICAOdiferentesCount[j] + 1);
                                }
                                else ICAOdiferentesCountTemporal.Add(ICAOdiferentesCount[j]);
                            }
                            ICAOdiferentesCount.Clear();
                            ICAOdiferentesCount = ICAOdiferentesCountTemporal;
                        }
                        else
                        {
                            ICAOdiferentes.Add(myList.GetPlanI(aircraftTrackDetected[i][n]).GetICAOAdress());
                            ICAOdiferentesCount.Add(1);
                        }
                    }
                }
                int posicionGanadora = 0;
                if (ICAOdiferentesCount.Count() > 1)
                {
                    for (int m = 1; m < ICAOdiferentesCount.Count(); m++)
                    {
                        if (ICAOdiferentesCount[m] > ICAOdiferentesCount[posicionGanadora])
                        {
                            posicionGanadora = m;
                        }
                    }
                }

                int zonaActual = myList.GetPlanI(aircraftTrackDetected[i][0]).GetZona();
                int indiceInicial = 0;
                double tiempoInicial = myList.GetPlanI(aircraftTrackDetected[i][0]).GetUTCcorregido() * 60;
                for (int n = 1; n < aircraftTrackDetected[i].Count(); n++)
                {
                    if ((myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() != zonaActual) || (n == aircraftTrackDetected[i].Count() - 1))
                    {
                        int indiceFinal;
                        if (n != aircraftTrackDetected[i].Count() - 1)
                            indiceFinal = n - 1;
                        else indiceFinal = n;

                        double ventanaTiempo = 5;//En segundos

                        double tiempoFinal = myList.GetPlanI(aircraftTrackDetected[i][indiceFinal]).GetUTCcorregido() * 60;
                        double tiempoFinalVentana = tiempoInicial + ventanaTiempo;
                        int ventanas = 0;
                        int incorrecto = 0;

                        while (tiempoFinalVentana <= tiempoFinal)
                        {
                            bool ventanaCumple = true;
                            int m = indiceInicial;
                            while ((m <= indiceFinal) && (ventanaCumple))
                            {
                                if (myList.GetPlanI(aircraftTrackDetected[i][m]).GetUTCcorregido() * 60 >= tiempoInicial && myList.GetPlanI(aircraftTrackDetected[i][m]).GetUTCcorregido() * 60 <= tiempoFinalVentana)
                                {
                                    if(myList.GetPlanI(aircraftTrackDetected[i][m]).GetICAOAdress()!= ICAOdiferentes[posicionGanadora])
                                    {
                                        ventanaCumple = false;
                                        incorrecto++;
                                    }
                                }
                                m++;
                            }
                            ventanas++;
                            tiempoInicial = tiempoInicial + 1;
                            tiempoFinalVentana = tiempoInicial + ventanaTiempo;
                        }                

                        if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 11)
                        {
                            FalseIdentification[1, 0] = FalseIdentification[1, 0] + ventanas;
                            FalseIdentification[1, 1] = FalseIdentification[1, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 12)
                        {
                            FalseIdentification[2, 0] = FalseIdentification[2, 0] + ventanas;
                            FalseIdentification[2, 1] = FalseIdentification[2, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 13)
                        {
                            FalseIdentification[3, 0] = FalseIdentification[3, 0] + ventanas;
                            FalseIdentification[3, 1] = FalseIdentification[3, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 4)
                        {
                            FalseIdentification[4, 0] = FalseIdentification[4, 0] + ventanas;
                            FalseIdentification[4, 1] = FalseIdentification[4, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 31)
                        {
                            FalseIdentification[6, 0] = FalseIdentification[6, 0] + ventanas;
                            FalseIdentification[6, 1] = FalseIdentification[6, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 32)
                        {
                            FalseIdentification[7, 0] = FalseIdentification[7, 0] + ventanas;
                            FalseIdentification[7, 1] = FalseIdentification[7, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 21)
                        {
                            FalseIdentification[9, 0] = FalseIdentification[9, 0] + ventanas;
                            FalseIdentification[9, 1] = FalseIdentification[9, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 22)
                        {
                            FalseIdentification[10, 0] = FalseIdentification[10, 0] + ventanas;
                            FalseIdentification[10, 1] = FalseIdentification[10, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 01)
                        {
                            FalseIdentification[12, 0] = FalseIdentification[12, 0] + ventanas;
                            FalseIdentification[12, 1] = FalseIdentification[12, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 02)
                        {
                            FalseIdentification[13, 0] = FalseIdentification[13, 0] + ventanas;
                            FalseIdentification[13, 1] = FalseIdentification[13, 1] + incorrecto;
                        }
                        else if (myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona() == 0)
                        {
                            FalseIdentification[14, 0] = FalseIdentification[14, 0] + ventanas;
                            FalseIdentification[14, 1] = FalseIdentification[14, 1] + incorrecto;
                        }
                        zonaActual = myList.GetPlanI(aircraftTrackDetected[i][n]).GetZona();
                        indiceInicial = n;
                        tiempoInicial = myList.GetPlanI(aircraftTrackDetected[i][n]).GetUTCcorregido() * 60;
                    }
                }
            }

            //Totales
            for (int i = 0; i < 2; i++)
            {
                FalseIdentification[0, i] = FalseIdentification[1, i] + FalseIdentification[2, i] + FalseIdentification[3, i] + FalseIdentification[4, i];//Total maneouvering
                FalseIdentification[5, i] = FalseIdentification[6, i] + FalseIdentification[7, i];//Total Apron
                FalseIdentification[8, i] = FalseIdentification[9, i] + FalseIdentification[10, i];//Total Stand
                FalseIdentification[11, i] = FalseIdentification[12, i] + FalseIdentification[13, i] + FalseIdentification[14, i];//Airborne
                FalseIdentification[15, i] = FalseIdentification[0, i] + FalseIdentification[5, i] + FalseIdentification[8, i] + FalseIdentification[11, i]; //Suma de todos
            }

            //PFI
            for (int i = 0; i < FalseIdentification.GetLength(0); i++)
            {
                if (FalseIdentification[i, 0] != 0)
                    FalseIdentification[i, 2] = FalseIdentification[i, 1] * 100 / FalseIdentification[i, 0];
                else FalseIdentification[i, 2] = 0;
            }

            // PID minimo segun doc ED117
            FalseIdentification[15, 3] = 0.0001; //[%]
        }

        public List<double[]> SetDiferencias()
        {
            List<double[]>  diferenciasFinales = new List<double[]>();
            for (int i = 0; i < DGPSList.GetNumList(); i++)
            {
                if (DGPSList.GetPlanI(i).GetIndiceDGPS() != -1)
                {
                    double[] diferenciaActual = new double[3];
                    diferenciaActual[0] = DGPSList.GetPlanI(i).GetPosicion()[0] - listaPA.GetPlanI(DGPSList.GetPlanI(i).GetIndiceDGPS()).GetPosicion()[0];
                    diferenciaActual[1] = DGPSList.GetPlanI(i).GetPosicion()[1] - listaPA.GetPlanI(DGPSList.GetPlanI(i).GetIndiceDGPS()).GetPosicion()[1];
                    diferenciaActual[2] = listaPA.GetPlanI(DGPSList.GetPlanI(i).GetIndiceDGPS()).GetZona();
                    diferenciasFinales.Add(diferenciaActual);
                }
            }
            return (diferenciasFinales);
        }

        public void OrdenarDiferencias(double[,] lista, string orden) //Te ordena de menor a mayor ó de mayor a menor (segun variable orden) las diferecias de un vector, ordena la primera columna, sirve para n columnas
        {
            int i, j, k;
            double[] temp = new double[lista.GetLength(1)];
            bool algoCambiado;//Si en algun momento para de cambiar, paramos el bucle
            bool parar = false;//para el bucle
            i = 1;
            while ((i < lista.GetLength(0))&&(!parar))//Algoritmo que ordena de menor a mayor (metodo burbuja, Bubble Sort)
            {
                algoCambiado = false;
                for (j = 0; j < lista.GetLength(0) - 1; j++) //De menor a mayor
                {
                    if (orden == "mM")
                    {
                        if (lista[j, 0] > lista[j + 1, 0])
                        {
                            for (k = 0; k < lista.GetLength(1); k++)
                            {
                                temp[k] = lista[j, k];
                                lista[j, k] = lista[j + 1, k];
                                lista[j + 1, k] = temp[k];
                                algoCambiado = true;
                            }
                        }
                    } 
                    else if (orden=="Mm") //De mayor a menor
                    {
                        if (lista[j, 0] < lista[j + 1, 0])
                        {
                            for (k = 0; k < lista.GetLength(1); k++)
                            {
                                temp[k] = lista[j, k];
                                lista[j, k] = lista[j + 1, k];
                                lista[j + 1, k] = temp[k];
                                algoCambiado = true;
                            }
                        }
                    }
                }
                if (!algoCambiado)
                {
                    parar = true;
                }
                i++;
            }
        }

        public List<double> OrdenarDiferenciasPA(List<double[]> lista, List<int> zonas) //Te ordena de menor a mayor las diferecias de una lista, teniendo en cuenta las zonas que comparar. Adaptado al formato de entrada de PosAcc
        {
            List<double> listaTemporal = new List<double>();
            int i, j;
            for (i = 0; i < lista.Count(); i++)//Seleccionamos los componentes de lista que tienen la zona de interes
            {
                for (j = 0; j < zonas.Count(); j++)
                {
                    if (lista[i][1] == zonas[j])
                    {
                        listaTemporal.Add(lista[i][0]);
                    }
                }
            }

            double[] listaTemporalVector = new double[listaTemporal.Count()];
            for (i = 0; i < listaTemporal.Count(); i++)//Pasamos de Lista a vector para poder trabajar con mas sencillez en el algoritmo que ordena las diferencias
            {
                listaTemporalVector[i] = listaTemporal[i];
            }        

            double temp;
            bool algoCambiado;//Si en algun momento para de cambiar, paramos el bucle
            bool parar = false;//para el bucle
            i = 1;
            while ((i < listaTemporalVector.GetLength(0))&&(!parar))
            {
                algoCambiado = false;
                for (j= 0; j < listaTemporalVector.GetLength(0) - 1; j++)
                {
                    if (listaTemporalVector[j] > listaTemporalVector[j+1])
                    {
                        temp= listaTemporalVector[j];
                        listaTemporalVector[j] = listaTemporalVector[j+1];
                        listaTemporalVector[j + 1]=temp;
                        algoCambiado = true;
                    }
                }
                if (!algoCambiado)
                {
                    parar = true;
                }
                i++;
            }

            listaTemporal.Clear();
            for (i = 0; i < listaTemporalVector.Count(); i++)//Pasamos de vector a lista para adaptar al output de la función
            {
                listaTemporal.Add(listaTemporalVector[i]);
            }

            return (listaTemporal);
        }

        public double[] Percentil(List<double> lista, double percentil1, double percentil2) //Devuelve el valor de la diferencia de la lista para 2 percentiles
        {
            double[] percentilesInput = new double[2];
            double[] percentilesOutput = new double[2];
            percentilesInput[0] = percentil1;
            percentilesInput[1] = percentil2;
            double condicion; //Dependiendo de si es entero o decimal el percentil se calcula de una manera o de otra
            int indice;
            for (int i=0;i< percentilesInput.Length;i++)
            {
                condicion = lista.Count() * percentilesInput[i] / 100;
                if (condicion-Math.Floor(condicion)!=0)
                {
                    indice = Convert.ToInt32(Math.Floor(condicion + 1)) - 1;
                    if (indice < lista.Count())
                    {
                        percentilesOutput[i] = lista[indice];
                    }
                    else percentilesOutput[i] = lista[lista.Count() - 1];
                }
                else
                {
                    indice = Convert.ToInt32(condicion) - 1;
                    if (indice<lista.Count()-1)
                    {
                        percentilesOutput[i] = (lista[indice] + lista[indice + 1]) / 2;
                    }
                    else percentilesOutput[i] = lista[lista.Count() - 1];
                }
            }
            return (percentilesOutput);
        }

        public double Mean(List<double> lista)//Devuelve la media de una lista
        {
            double media;
            double total=0;
            for(int i=0; i<lista.Count();i++)
            {
                total = total + lista[i];
            }
            if (lista.Count() != 0)
            {
                media = total / lista.Count();
            }
            else media = 0;
            return (media);
        }

        public double StdDev(List<double> lista, double mean)//Devuelve desviación estandar una lista
        {
            double std;
            double total = 0;
            for (int i = 0; i < lista.Count(); i++)
            {
                total = total + Math.Pow(lista[i] - mean, 2);
            }
            if (lista.Count() != 0)
            {
                std = Math.Sqrt(total / lista.Count());
            }
            else std = 0;
            return (std);
        }

        public void SetPosAccValues(int indice, List<double[]> diferencia, List<int> zonas)//Ahorra repetir codigo. Con un indice de entrada y su correpondiente lista + zonas establece el valor de Percentiles, media y STD
        {
            List<double> listaOrdenada = new List<double>();
            double percentil1 = 95;
            double percentil2 = 99;
            listaOrdenada = OrdenarDiferenciasPA(diferencia, zonas);
            if (listaOrdenada.Count() != 0)
            {
                PosAcc[indice, 0] = Percentil(listaOrdenada, percentil1, percentil2)[0];
                PosAcc[indice, 1] = Percentil(listaOrdenada, percentil1, percentil2)[1];
                PosAcc[indice, 5] = Mean(listaOrdenada);
                PosAcc[indice, 6] = StdDev(listaOrdenada, PosAcc[indice, 6]);
            }
        }

        public LectorMensaje AdaptarListaActual(LectorMensaje listaActual)//Elimin de myList todos los Icao Add que no son del vehiculo
        {
            string icao = DGPSList.GetPlanI(0).GetICAOAdress();
            LectorMensaje lista = new LectorMensaje();
            for (int i = 0; i < listaActual.GetNumList(); i++)
            {
                if(listaActual.GetPlanI(i).GetICAOAdress()== DGPSList.GetPlanI(0).GetICAOAdress())
                {
                    lista.AddPlanI(listaActual.GetPlanI(i));
                }
            }
            return (lista);
        }

        public double[] GetPosicion(int cont) //Miramos si los datos de posicion del paquete vienen en WGS, del SMR, o en coordenadas cartesianas del MLAT
        {
            double[] posicion = new double[2];//Indice 0=X; indice 1=Y
            if ((myList.GetPlanI(cont).GetCartFromWGS84()[0] != Math.Pow(10, 8)) && (myList.GetPlanI(cont).GetCartFromWGS84()[1] != Math.Pow(10, 8)))
            {
                posicion[0] = myList.GetPlanI(cont).GetCartFromWGS84()[0];
                posicion[1] = myList.GetPlanI(cont).GetCartFromWGS84()[1];
            }
            else if ((myList.GetPlanI(cont).GetMLATfromSMRX() != Math.Pow(10, 8)) && (myList.GetPlanI(cont).GetMLATfromSMRY() != Math.Pow(10, 8)))
            {
                posicion[0] = myList.GetPlanI(cont).GetMLATfromSMRX();
                posicion[1] = myList.GetPlanI(cont).GetMLATfromSMRY();
            }
            else if ((myList.GetPlanI(cont).GetCartX() != Math.Pow(10, 8)) && (myList.GetPlanI(cont).GetCartY() != Math.Pow(10, 8)))
            {
                posicion[0] = myList.GetPlanI(cont).GetCartX();
                posicion[1] = myList.GetPlanI(cont).GetCartY();
            }

            return (posicion);
        }

        public double[,] GetUpdateRate()
        {
            return this.updateRate;
        }
        public double[,] GetUpdatesUnitarios()
        {
            return this.updatesUnitarios;
        }
        public double[,] GetPositionAccuracy()
        {
            return this.PosAcc;
        }
        public double[,] GetProbOfMLATDetection()
        {
            return this.MLATDet;
        }
        public double[,] GetProbOfIdentification()
        {
            return this.Identification;
        }
        public double[,] GetProbOfFalseDetection()
        {
            return this.FalseDetection;
        }
        public double[,] GetProbOfFalseIdentification()
        {
            return this.FalseIdentification;
        }
        public List<double[]> GetDiferencias()
        {
            return this.diferencias;
        }
    }
}
