using System.Collections.Generic;
using System.Linq;

namespace Codigo
{
    public class CListaMap
    {
        List<CMap> myList = new List<CMap>();

        public CListaMap()
        {
            myList.Add(new CMap("aeropuerto"));
            myList.Add(new CMap("Aeropuerto_Barcelona"));
            myList.Add(new CMap("Aeropuerto_Barcelonanue"));
            myList.Add(new CMap("BCN_Aparcamientos"));
            myList.Add(new CMap("BCN_CarreterasServicio"));
            myList.Add(new CMap("BCN_Edificios"));
            myList.Add(new CMap("BCN_Parterres"));
            myList.Add(new CMap("BCN_Pistas"));
            myList.Add(new CMap("BCN_ZonasMovimiento"));

            CargarMapas();
        }

        public CMap GetPlanI(int i)
        {
            return myList[i];
        }

        public void CargarMapas()
        {
            int i = 0;
            while (i < myList.Count)
            {
                myList[i].Leer();
                i++;
            }
        }

        public int GetNumList()
        {
            return myList.Count();
        }

        public double[,] GetDIMENSIONES()//Compara las dimensiones de todos los mapas
        {
            int i = 1;
            double[,] dimensiones = new double[2, 2];
            double[,] dimensionesi = new double[2, 2];
            dimensiones = myList[0].GetDimensiones(myList[0].GetLinea(), myList[0].GetPoli());
            while (i < myList.Count)
            {
                dimensionesi = myList[i].GetDimensiones(myList[i].GetLinea(), myList[i].GetPoli());
                if (dimensiones[0, 0] > dimensionesi[0, 0])
                {
                    dimensiones[0, 0] = dimensionesi[0, 0];
                }
                if (dimensiones[0, 1] < dimensionesi[0, 1])
                {
                    dimensiones[0, 1] = dimensionesi[0, 1];
                }
                if (dimensiones[1, 0] > dimensionesi[1, 0])
                {
                    dimensiones[1, 0] = dimensionesi[1, 0];
                }
                if (dimensiones[1, 1] < dimensionesi[1, 1])
                {
                    dimensiones[1, 1] = dimensionesi[1, 1];
                }
                i++;
            }

            return (dimensiones);
        }

        public double GetPROPORCION(double PH, double PW)//Valor mas grande de todos los mapas
        {
            int i = 1;
            double Proporcion;

            Proporcion = myList[0].GetProporcion(myList[0].GetDimensiones(myList[0].GetLinea(), myList[0].GetPoli()), PH, PW);
            while (i < myList.Count)
            {
                if (Proporcion < myList[i].GetProporcion(myList[i].GetDimensiones(myList[i].GetLinea(), myList[i].GetPoli()), PH, PW))
                {
                    Proporcion = myList[i].GetProporcion(myList[i].GetDimensiones(myList[i].GetLinea(), myList[i].GetPoli()), PH, PW);
                }
                i = i + 1;
            }
            return (Proporcion);
        }
    }
}