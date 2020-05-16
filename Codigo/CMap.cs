using System;
using System.IO;

namespace Codigo
{
    public class CMap
    {
        double[,] cartLine;
        double[,] cartPoli;
        string[] Mlat = new string[] { "411749426N", "0020442410E" };//Coordenadas del mlat y SMR
        string[] SMR = new string[] { "411744226N", "0020542411E" };
        double[] cartMlat = new double[2];
        double[] cartSMR = new double[2];

        string nombreFichero;

        public CMap(string NombreFichero)
        {
            this.nombreFichero = NombreFichero;
        }

        public int[] EncontrarLongitud()//Encuentra la longitud de lineas y polilineas para la función de leer()
        {
            StreamReader leer = new StreamReader(nombreFichero + ".txt");
            string linea = leer.ReadLine();
            int contadorLinea = 0;
            int contadorPoli = 0;
            bool llegarPoli = false;
            while (linea != "")
            {
                if (llegarPoli == false)
                {
                    if (linea.Contains("Polilinea") == false)
                    {
                        contadorLinea = contadorLinea + 1;
                    }
                    else
                    {
                        llegarPoli = true;
                    }
                }
                else
                {
                    contadorPoli = contadorPoli + 1;
                }
                linea = leer.ReadLine();
            }

            leer.Close();
            int[] result = new int[2];
            result[0] = contadorLinea;
            result[1] = contadorPoli;
            return (result);
        }

        public void Leer()
        {
            StreamReader leer = new StreamReader(nombreFichero + ".txt");
            string linea = leer.ReadLine();
            int[] longitud = EncontrarLongitud();
            cartLine = new double[longitud[0], 4];
            cartPoli = new double[longitud[1], 2];

            bool llegarPoli = false;
            cartMlat[0] = ConvertWGStoRad(Mlat[0], 0);//Mlat en radianes
            cartMlat[1] = ConvertWGStoRad(Mlat[1], 1);
            int i = 0; //fila de la matriz
            while (linea != "")
            {
                if (llegarPoli == false)
                {
                    if (linea.Contains("Polilinea") == false)
                    {
                        string[] entradas = linea.Split(' ');
                        //entradas contiene ej:line 454545555N(lat,y) 0454545555E(long,x) 454545555N 0454545555E
                        cartLine[i, 0] = Convert.ToInt32(ConvertRadtoXY(cartMlat, ConvertWGStoRad(entradas[1], 0), ConvertWGStoRad(entradas[2], 1))[0]);
                        cartLine[i, 1] = Convert.ToInt32(ConvertRadtoXY(cartMlat, ConvertWGStoRad(entradas[1], 0), ConvertWGStoRad(entradas[2], 1))[1]);
                        cartLine[i, 2] = Convert.ToInt32(ConvertRadtoXY(cartMlat, ConvertWGStoRad(entradas[3], 0), ConvertWGStoRad(entradas[4], 1))[0]);
                        cartLine[i, 3] = Convert.ToInt32(ConvertRadtoXY(cartMlat, ConvertWGStoRad(entradas[3], 0), ConvertWGStoRad(entradas[4], 1))[1]);
                        i = i + 1;
                    }
                    else
                    {
                        llegarPoli = true;
                        i = 0;
                    }
                }
                else
                {
                    if (linea.Contains("Polilinea") == false)
                    {
                        string[] poliEntradas = linea.Split(' ');
                        cartPoli[i, 0] = Convert.ToInt32(ConvertRadtoXY(cartMlat, ConvertWGStoRad(poliEntradas[0], 0), ConvertWGStoRad(poliEntradas[1], 1))[0]);
                        cartPoli[i, 1] = Convert.ToInt32(ConvertRadtoXY(cartMlat, ConvertWGStoRad(poliEntradas[0], 0), ConvertWGStoRad(poliEntradas[1], 1))[1]);
                        i = i + 1;
                    }
                    else
                    {
                        cartPoli[i, 0] = Math.Pow(10, 8); //ponemos una linea con numero imposible para ver el salto entre polilineas, excepto la primera polilinea
                        cartPoli[i, 1] = 0; //con que la primera componente sea grande ya lo detectamos, asi ocupa menos. CREO
                        i = i + 1;
                    }
                }
                linea = leer.ReadLine();
            }
        }

        public double ConvertWGStoRad(string coordenada, int orientacion)//Si es lat, orientacion=0; Si es long, orientacion=1. El nombre expresa mal su funcion. Pasa de lat long a radianes
        {
            //Retorna una coordenada en radianes
            double grados = 0;
            int latSigno = 1; //por defecto es positivo para Norte
            int longSigno = 1; //por defecto es positivo para Este
            if (orientacion == 0)
            {
                if (Convert.ToString(coordenada[9]) == "S")
                {
                    latSigno = -1;
                }
                grados = Convert.ToDouble(Convert.ToString(coordenada[0]) + Convert.ToString(coordenada[1]));
                double minutos = Convert.ToDouble(Convert.ToString(coordenada[2]) + Convert.ToString(coordenada[3]));
                double segundos = Convert.ToDouble(Convert.ToString(coordenada[4]) + Convert.ToString(coordenada[5]));
                double decimas = Convert.ToDouble(Convert.ToString(coordenada[6]) + Convert.ToString(coordenada[7]) + Convert.ToString(coordenada[8])) / 1000;
                segundos = segundos + decimas;
                grados = grados + (minutos / 60) + (segundos / 3600);
                grados = latSigno * grados * Math.PI / 180;
            }
            if (orientacion == 1)
            {
                if ((Convert.ToString(coordenada[10]) == "W") || (Convert.ToString(coordenada[10]) == "O"))
                {
                    longSigno = -1;
                }
                grados = Convert.ToDouble(Convert.ToString(coordenada[0]) + Convert.ToString(coordenada[1]) + Convert.ToString(coordenada[2]));
                double minutos = Convert.ToDouble(Convert.ToString(coordenada[3]) + Convert.ToString(coordenada[4]));
                double segundos = Convert.ToDouble(Convert.ToString(coordenada[5]) + Convert.ToString(coordenada[6]));
                double decimas = Convert.ToDouble(Convert.ToString(coordenada[7]) + Convert.ToString(coordenada[8]) + Convert.ToString(coordenada[9])) / 1000;
                segundos = segundos + decimas;
                grados = grados + (minutos / 60) + (segundos / 3600);
                grados = longSigno * grados * Math.PI / 180;

            }
            return (grados);
        }

        public double[] ConvertRadtoXY(double[] llh0, double dphi, double dlam)//proyecta WGS84 sobre el plano. Lat long en radianes
        {
            double a = 6378137;
            double b = 6356752.3142;
            double e2;
            e2 = 1 - Math.Pow((b / a), 2);
            //Location of reference point in radians
            double phi = llh0[0];//lat
            double lam = llh0[1];//long
            //Some useful definitions
            double tmp1 = Math.Sqrt(1 - e2 * Math.Pow(Math.Sin(phi), 2));
            double cl = Math.Cos(lam);
            double sl = Math.Sin(lam);
            double cp = Math.Cos(phi);
            double sp = Math.Sin(phi);
            //Location of data points in radians
            double[] denu = new double[2];
            dphi = dphi - phi;
            dlam = dlam - lam;
            //Transformations
            denu[0] = (a / tmp1) * cp * dlam - (a * (1 - e2) / (Math.Pow(tmp1, 3))) * sp * dphi * dlam; //X
            denu[1] = (a * (1 - e2) / Math.Pow(tmp1, 3)) * dphi + 1.5 * cp * sp * a * e2 * Math.Pow(dphi, 2) + 0.5 * sp * cp * (a / tmp1) * Math.Pow(dlam, 2); //Y

            return (denu);
        }

        public double[,] GetDimensiones(double[,] Lvector, double[,] Pvector)//Obtiene los limites laterales del vector para plotearlo
        {
            //Compara el vector de polilineas con el vector de lineas
            double[,] dimensiones = new double[2, 2];

            int Prows = Pvector.GetLength(0);
            int Pcolumns = Pvector.GetLength(1);

            double Pxm = Pvector[0, 0];
            double PxM = Pvector[0, 0];
            double Pym = Pvector[0, 1];
            double PyM = Pvector[0, 1];

            double xm;
            double xM;
            double ym;
            double yM;

            int Lrows = Lvector.GetLength(0);
            int Lcolumns = Lvector.GetLength(1);
            double Lxm = Lvector[0, 0];
            double LxM = Lvector[0, 0];
            double Lym = Lvector[0, 1];
            double LyM = Lvector[0, 1];
            if (Lcolumns == 4)
            {
                int cont = 0;
                while (cont < Lrows)
                {
                    if (Lvector[cont, 1] < Lym)
                    {
                        Lym = Lvector[cont, 1];
                    }
                    if (Lvector[cont, 0] < Lxm)
                    {
                        Lxm = Lvector[cont, 0];
                    }
                    if (Lvector[cont, 3] < Lym)
                    {
                        Lym = Lvector[cont, 3];
                    }
                    if (Lvector[cont, 2] < Lxm)
                    {
                        Lxm = Lvector[cont, 2];
                    }
                    if (Lvector[cont, 1] > LyM)
                    {
                        LyM = Lvector[cont, 1];
                    }
                    if (Lvector[cont, 0] > LxM)
                    {
                        LxM = Lvector[cont, 0];
                    }
                    if (Lvector[cont, 3] > LyM)
                    {
                        LyM = Lvector[cont, 3];
                    }
                    if (Lvector[cont, 2] > LxM)
                    {
                        LxM = Lvector[cont, 2];
                    }
                    cont = cont + 1;
                }
            }
            if (Pcolumns == 2)
            {
                int cont = 0;
                while (cont < Prows)
                {
                    if (Pvector[cont, 0] != Math.Pow(10, 8))
                    {
                        if (Pvector[cont, 1] < Pym)
                        {
                            Pym = Pvector[cont, 1];
                        }
                        if (Pvector[cont, 0] < Pxm)
                        {
                            Pxm = Pvector[cont, 0];
                        }
                        if (Pvector[cont, 1] > PyM)
                        {
                            PyM = Pvector[cont, 1];
                        }
                        if (Pvector[cont, 0] > PxM)
                        {
                            PxM = Pvector[cont, 0];
                        }
                    }
                    cont = cont + 1;
                }
            }

            if (Lxm > Pxm)
            {
                xm = Pxm;
            }
            else xm = Lxm;
            if (Lym > Pym)
            {
                ym = Pym;
            }
            else ym = Lym;
            if (LxM < PxM)
            {
                xM = PxM;
            }
            else xM = LxM;
            if (LyM < PyM)
            {
                yM = PyM;
            }
            else yM = LyM;

            dimensiones[0, 0] = Convert.ToInt32(xm);
            dimensiones[0, 1] = Convert.ToInt32(xM);
            dimensiones[1, 0] = Convert.ToInt32(ym);
            dimensiones[1, 1] = Convert.ToInt32(yM);
            return (dimensiones);
        }

        public double GetProporcion(double[,] dimensiones, double panelDimensionesHeight, double panelDimensionesWidth)//Valor con el que proporcionalizar el mapa
        {
            double ProporcionY = Math.Abs((dimensiones[1, 1] - dimensiones[1, 0])) / panelDimensionesHeight;
            double ProporcionX = Math.Abs(dimensiones[0, 1] - dimensiones[0, 0]) / panelDimensionesWidth;
            double Proporcion;
            if (ProporcionX > ProporcionY)
            {
                Proporcion = ProporcionX;
            }
            else Proporcion = ProporcionY;

            return (Proporcion);
        }

        public double[,] GetLinea()
        {
            return cartLine;
        }

        public double[,] GetPoli()
        {
            return cartPoli;
        }
    }
}
