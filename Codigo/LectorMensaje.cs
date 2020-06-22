using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
 
namespace Codigo
{
    public class LectorMensaje
    {
        List<DecodificadorMensaje> myList = new List<DecodificadorMensaje>();

        public int GetNumList()
        {
            return myList.Count();
        }

        public DecodificadorMensaje GetPlanI(int i)
        {
            return myList[i];
        }

        public void RemovePlanI(int i)
        {
            myList.RemoveAt(i);
        }

        public void AddPlanI(DecodificadorMensaje plan)
        {
            myList.Add(plan);
        }

        public void ClearList()
        {
            myList.Clear();
        }

        public int AircraftDetected() //Retorna el recuento de aviones (compara ICAO Adress). Retorna 0 si el item no llega
        {
            int indice = 0;
            List<string> listaAviones = new List<string>();

            while ((myList[indice].GetICAOAdress() == "No Data") && (indice < myList.Count() - 1))
            {
                indice++;
            }

            if (indice < myList.Count() - 1)
            {
                listaAviones.Add(myList[indice].GetICAOAdress());
                for (int i = indice + 1; i < myList.Count(); i++)
                {
                    if (myList[i].GetICAOAdress() != "No Data")
                    {
                        bool yaExiste = false;
                        int n = 0;
                        while ((n < listaAviones.Count()) && (!yaExiste))
                        {
                            if (myList[i].GetICAOAdress() == listaAviones[n])
                            {
                                yaExiste = true;
                            }
                            else n++;
                        }
                        if (!yaExiste)
                            listaAviones.Add(myList[i].GetICAOAdress());
                    }
                }
                return (listaAviones.Count());
            }
            else return (0);
        }

        public void CargarListaDeDirectorio(string nombreDirectorio)
        //Rellena la lista de mensajes desde un fichero cualquiera
        {
            FileStream fs = new FileStream(nombreDirectorio, FileMode.Open, FileAccess.Read);
            myList.Clear();
            bool yaEsta = false;
            DecodificadorMensaje mensaje;

            //Leemos mensajes hasta que encontremos un mensaje vacío
            int CatInicial = 0;
            while (!yaEsta)
            {
                mensaje = new DecodificadorMensaje();
                mensaje.LeerSiguienteMensaje(fs);
                //Si el mensaje está vacío, se descarta y se deja de leer
                if ((CatInicial != 10)&&(CatInicial != 20))
                    CatInicial = mensaje.GetCAT();//Solo podremos leer un tipo de CAT a la vez (10 ó 20)
                if (mensaje.GetCAT() == -1)
                    yaEsta = true;
                else if ((mensaje.GetCAT() == 10) && (CatInicial == 10))
                {
                    myList.Add(mensaje);
                }
                else if ((mensaje.GetCAT() == 20) && (CatInicial == 20))
                {
                    myList.Add(mensaje);
                }
            }
            fs.Close();
            SetZonas();//Asignamos la zona en funcion de la posicion en el mapa
            //Antes de OrdenarPaquetesPorTiempo, descartamos SMR. Antes de asignarUTCcorregido ordenamos por tiempo.
            //Por lo tanto ambas funciones se ejecutan en Form1, despues de descartar los vehiculos SMR.
        }

        public void CargarListaDeDirectorioDGPS(string nombreDirectorio, string Icao)
        //Rellena la lista de mensajes desde un fichero cualquiera
        {
            StreamReader leer = new StreamReader(nombreDirectorio);
            myList.Clear();
            DecodificadorMensaje mensaje;

            //Leemos mensajes hasta que encontremos un mensaje vacío
            string linea = leer.ReadLine();

            bool datos = false;
            while ((linea != null) && (!datos))
            {
                string a = linea.Split('\t')[1];
                double numero = 0;
                if (!double.TryParse(a, out numero))//A veces la primera linea no contiene datos sino info de la estructura de los datos, la saltamos
                {
                    //TryParse() permite convertir y trabajar con la respuesta
                    //si se puede realizar correctamente el resultado se asigna en la variable definida en el out
                    linea = leer.ReadLine();
                }
                else datos = true;
            }

            while (linea != null)
            {
                mensaje = new DecodificadorMensaje();
                mensaje.DecodificarDGPS(linea, Icao);
                myList.Add(mensaje);
                linea = leer.ReadLine();
            }
            leer.Close();
            SetUTCcorregido(); //Coregimos la hora para cambio de dia
            SetVelocidadDGPS();//Asignamos velocidades a los mensajes de DGPS
            SetZonas();//Asignamos la zona en funcion de la posicion en el mapa
        }

        public void OrdenarPaquetesPorTiempo() //Te ordena de menor a mayor por tiempo. A veces algun paquete tiene un tiempo anterior al anterior paquete
        {
            int i, j, k;
            DecodificadorMensaje temp;
            int indiceFinal = 0;
            int indiceInicial = 0;
            k = 1;
            bool encontrado = false;
            while ((k < myList.Count()) && (!encontrado))//Buscamos el cambio de dia
            {
                if (myList[k - 1].GetUTC() > myList[k].GetUTC())
                {
                    //No podemos aplicar solo el criterio de mirar si el tiempo de un paquete es menor al siguiente para encontrar el cambio de dia
                    //debido a que precisamente el problema es que algunos paquetes no estan oredenados en tiempo.
                    encontrado = true;
                    int mMax = k - 1 + 100; //Comparamos el paquete con los siguientes 100 paquetes. Asumimos que solo hay pequeños desordenes en la lista, menores a 100 paquetes seguidos.
                    if (mMax >= myList.Count())
                    {
                        mMax = myList.Count() - 1;
                    }
                    for (int m = k + 1; m <= mMax; m++)
                    {
                        if (myList[k - 1].GetUTC() < myList[m].GetUTC())
                        {
                            encontrado = false;
                        }
                    }
                    if (encontrado)
                    {
                        indiceFinal = k - 1;
                    }
                }
                k++;
            }
            if ((!encontrado) || (indiceFinal == 0))
            {
                indiceFinal = myList.Count();
            }
            bool algoCambiado;//Si en algun momento para de cambiar, paramos el bucle
            bool parar = false;//para el bucle
            if (indiceFinal != myList.Count())
            {
                i = indiceInicial + 1;
                while ((i <= indiceFinal) && (!parar))//Algoritmo que ordena de menor a mayor (metodo burbuja, Bubble Sort)
                {
                    algoCambiado = false;
                    for (j = 0; j <= indiceFinal - 1; j++) //De menor a mayor
                    {
                        if (myList[j].GetUTC() > myList[j + 1].GetUTC())//Ordenar de menor a mayor teniendo en cuenta el cambio de dia
                        {
                            temp = myList[j];
                            myList[j] = myList[j + 1];
                            myList[j + 1] = temp;
                            algoCambiado = true;
                        }
                    }
                    if (!algoCambiado)
                    {
                        parar = true;
                    }
                    i++;
                }
                indiceInicial = indiceFinal + 1;
                indiceFinal = myList.Count();
            }
            i = indiceInicial + 1; //Bubble empieza en 1. Ó indiceFinal +1 +1
            while ((i <= indiceFinal) && (!parar))//De menor a mayor
            {
                algoCambiado = false;
                for (j = 0; j < indiceFinal - 1; j++) //De menor a mayor
                {
                    if (myList[j].GetUTC() > myList[j + 1].GetUTC())
                    {
                        temp = myList[j];
                        myList[j] = myList[j + 1];
                        myList[j + 1] = temp;
                        algoCambiado = true;
                    }
                }
                if (!algoCambiado)
                {
                    parar = true;
                }
                i++;
            }
        }

        public bool ComprobarDiferentesSIC() //Si devuelve true significa que hay SIC=007(SMR) y =107(MLAT)
        {
            bool haySicDiferentes = false;
            int indice = 0;
            while ((myList[indice].GetSIC() == "No Data") && (indice < myList.Count()))
            {
                indice++;
            }//Nos saltamos los NO Data
            if (indice < myList.Count())
            {
                int SIC = Convert.ToInt32(myList[indice].GetSIC());
                int i = indice + 1;
                while ((i < myList.Count()) && (!haySicDiferentes))
                {
                    if (SIC == 007)//Guardamos el primero y lo comparamos con el resto
                    {
                        if (Convert.ToInt32(myList[i].GetSIC()) == 107)
                        {
                            haySicDiferentes = true;
                        }
                    }
                    if (SIC == 107)
                    {
                        if (Convert.ToInt32(myList[i].GetSIC()) == 007)
                        {
                            haySicDiferentes = true;
                        }
                    }
                    i++;
                }
            }
            return (haySicDiferentes);
        }

        public bool TodoSMR()//Comprueba si solo hay SMR en el fichero
        {
            bool todoSMR = true;
            int indice = 0;
            while ((myList[indice].GetSIC() == "No Data") && (indice < myList.Count()))
            {
                indice++;
            }//Nos saltamos los NO Data
            if (indice < myList.Count())
            {
                while ((indice < myList.Count()) && (todoSMR))
                {
                    if (Convert.ToInt32(myList[indice].GetSIC()) != 007)
                    {
                        todoSMR = false;
                    }
                    indice++;
                }
            }
            return (todoSMR);
        }

        public LectorMensaje SepararSMRyADSB() //Si hay diferentes SIC solo se queda con uno de ellos (MLAT SIC 107)
        {
            LectorMensaje listaFinal = new LectorMensaje();
            for (int i = 0; i < myList.Count(); i++)
            {
                if (Convert.ToInt32(myList[i].GetSIC()) == 107)
                {
                    listaFinal.AddPlanI(myList[i]);
                }
            }
            return (listaFinal);
        }

        public List<string> LeerVehiculosSquitter()//Lista con los ICAO Address de los vehiculos descartados
        {
            List<string> descartados = new List<string>();
            StreamReader leer = new StreamReader("VehiculosAEliminar.txt");
            string linea = leer.ReadLine();
            while (linea != null)
            {
                descartados.Add(linea);
                linea = leer.ReadLine();
            }
            leer.Close();
            return (descartados);
        }

        public LectorMensaje DescartarVehiculosSquitter() //Elimina los vehiculos que no envian señal de una manera normal
        {
            List<string> descartados = LeerVehiculosSquitter(); //Lista que contiene ICAOAdress de los vehiculos decartados
            LectorMensaje listaFinal = new LectorMensaje();
            bool descartar; //Devuelve true si el mensaje se debe descartar
            int i;
            for (int j = 0; j < myList.Count; j++)
            {
                descartar = false;
                i = 0;
                while ((i < descartados.Count()) && (!descartar))
                {
                    if (myList[j].GetICAOAdress() != "No Data") //Evitamos que compare con los No Data para reducir tiempo de procesado
                    {
                        if (myList[j].GetICAOAdress() == descartados[i])
                        {
                            descartar = true;
                        }
                    }
                    i++;
                }
                if (!descartar)
                {
                    listaFinal.AddPlanI(myList[j]);
                }
            }
            return (listaFinal);
        }

        public int SetIndicesDGPS(LectorMensaje lista)
        {
            //Al aplicar esta funcion a una lista de DGPS, la compara con la lista introducida como parametro, y rellena el campo indiceDGPS de la lista de DGPS
            //El campo indiceDGPS asigna la posicion en el vector myList en la que se encuentra el punto con el que se va a comparar
            //Para escoger estos indices. Se busca en MyList los paquetes que tienen el mismo ICAO Address que el D-GPS
            //Despues se mira que franja de tiempo que comparten y se fija un tiempo inicial y final
            //Finalmente se escoge que puntos de la lista D-GPS estan mas cerca (en tiempo) de la lista principal. En principio se tendra 1 dato/s de MLAT y varios datos/s de D-GPS
            //Tambien retorna el numero de puntos emparejados

            for (int m = 0; m < myList.Count(); m++)
            {
                myList[m].SetIndiceDGPS(-1);//Puede que la lista se este recargando (reasignando indices), por lo que reseteamos todo a -1
            }

            int match = 0;
            List<int> icaoList = ICAODetectedDGPS(lista);
            int indiceInicialMLAT = -1;
            int indiceFinalMLAT = -1;
            bool yaEsta = false;
            int i = 0;
            if (icaoList.Count() != 0)
            {
                if (myList[0].GetUTCcorregido() >= lista.GetPlanI(icaoList[0]).GetUTCcorregido())
                {
                    if (myList[myList.Count() - 1].GetUTCcorregido() >= lista.GetPlanI(icaoList[icaoList.Count() - 1]).GetUTCcorregido())
                    {
                        //Caso 1: D-GPS marca el inicio y MLAT el final (D-GPS empieza despues de MLAT, D-GPS acaba despues de MLAT)
                        indiceFinalMLAT = icaoList.Count() - 1;
                        while ((i < icaoList.Count()) && (!yaEsta))
                        {
                            if (lista.GetPlanI(icaoList[i]).GetUTCcorregido() >= myList[0].GetUTCcorregido())
                            {
                                indiceInicialMLAT = i;
                                yaEsta = true;
                            }
                            i++;
                        }
                    }
                    else
                    {
                        //Caso 2: D-GPS marca el inicio y el final
                        while ((i < icaoList.Count()) && (!yaEsta))
                        {
                            if (lista.GetPlanI(icaoList[i]).GetUTCcorregido() >= myList[0].GetUTCcorregido())
                            {
                                indiceInicialMLAT = i;
                                yaEsta = true;
                            }
                            i++;
                        }
                        yaEsta = false;
                        i = 0;
                        while ((i < icaoList.Count()) && (!yaEsta))
                        {
                            if (lista.GetPlanI(icaoList[i]).GetUTCcorregido() > myList[myList.Count() - 1].GetUTCcorregido())
                            {
                                indiceFinalMLAT = i - 1;//Nos pasamos con el >, compensamos con -1
                                yaEsta = true;
                            }
                            i++;
                        }

                    }
                }
                else if (myList[0].GetUTCcorregido() < lista.GetPlanI(icaoList[0]).GetUTCcorregido())
                {
                    if (myList[myList.Count() - 1].GetUTCcorregido() <= lista.GetPlanI(icaoList[icaoList.Count() - 1]).GetUTCcorregido())
                    {
                        //Caso 3: MLAT marca el inicio y D-GPS el final
                        indiceInicialMLAT = 0;
                        while ((i < icaoList.Count()) && (!yaEsta))
                        {
                            if (lista.GetPlanI(icaoList[i]).GetUTCcorregido() > myList[myList.Count() - 1].GetUTCcorregido())
                            {
                                indiceFinalMLAT = i - 1;//Nos pasamos con el >, compensamos con -1
                                yaEsta = true;
                            }
                            i++;
                        }
                    }
                    else
                    {
                        //Caso 4: MLAT marca el inicio y el final
                        indiceInicialMLAT = 0;
                        indiceFinalMLAT = icaoList.Count() - 1;
                    }
                }
                else
                {
                    //Caso 5: No se comparte franja de tiempo
                    indiceInicialMLAT = -1;
                    indiceFinalMLAT = -1;
                }

                int indiceI = Convert.ToInt32(myList.Count()/2); //En algunos ficheros de D-GPS los primeros paquetes llegan muy separados en tiempo. Buscamos la tasa de refresco en la mitad del fichero
                double tasaRefresco = Math.Round((myList[indiceI+1].GetUTCcorregido() - myList[indiceI].GetUTCcorregido()) * 60, 1);//solo se comparan puntos a +/- la tasa de refresco
                if ((indiceInicialMLAT != -1) && (indiceFinalMLAT != -1))//Si hay franja compartida
                {
                    double diferenciaTiempoGanador;
                    int indiceGanador;
                    bool encontrado;
                    int m = 0;//Esta fuera del for para que no se reinicie para cada punto
                    int guardarIndice;//Si un punto no se encuentra, el siguiente empieza a analizar desde guardarIndice
                    for (int n = indiceInicialMLAT; n <= indiceFinalMLAT; n++)
                    {
                        diferenciaTiempoGanador = Math.Pow(10, 8);
                        indiceGanador = -1;
                        encontrado = false;
                        guardarIndice = m;
                        while ((m < myList.Count()) && (!encontrado))
                        {
                            double diferenciaTemporalTiempo = Math.Abs(myList[m].GetUTCcorregido() - lista.GetPlanI(icaoList[n]).GetUTCcorregido()) * 60;
                            if (diferenciaTemporalTiempo <= tasaRefresco)
                            {
                                if (diferenciaTemporalTiempo < diferenciaTiempoGanador)
                                {
                                    diferenciaTiempoGanador = Math.Abs(myList[m].GetUTCcorregido() - lista.GetPlanI(icaoList[n]).GetUTCcorregido()) * 60;
                                    indiceGanador = m;
                                }
                            }
                            else
                            {
                                if (diferenciaTiempoGanador != Math.Pow(10, 8))
                                {
                                    encontrado = true;//Si ya ha encontrado un punto ganador, paramos de analizar. En el siguiente punto seguimos desde este indice
                                    //Si no se hace asi, el tiempo de procesado augmenta considerablemente
                                    m=m-1;
                                }
                            }

                            m++;

                            if (m == myList.Count())
                            {
                                m = guardarIndice;
                                encontrado = true;//No se ha encontrado, pero asi se sale del bucle
                            }
                        }
                        if (indiceGanador != -1)
                        {
                            myList[indiceGanador].SetIndiceDGPS(icaoList[n]);
                            match++;
                        }
                    }
                }
            }
            return (match);
        }

        public List<int> ICAODetectedDGPS(LectorMensaje lista) //Retorna un vector que en cada posicion tiene los indices de myList que corresponden a un unico avion (compara ICAO Adress)
        {
            //Funciona para listas de tipo D-GPS, necesita como parametro de entrada la lista general y usa Icao Adress del D-GPS

            int indice = 0;
            List<int> listaIcao = new List<int>();

            while ((lista.GetPlanI(indice).GetICAOAdress() == "No Data") && (indice < lista.GetNumList()))
            {
                indice++;
            }

            if (indice < lista.GetNumList())
            {
                for (int i = indice; i < lista.GetNumList(); i++)
                {
                    if (lista.GetPlanI(i).GetICAOAdress() == myList[0].GetICAOAdress())
                    {
                        listaIcao.Add(i);
                    }
                }
            }
            return (listaIcao);
        }

        public void SetZonas()
        {
            //Determina a que zona pertenece cada punto recibido
            //Las zonas son una composicion de puntos que forman una zona cerrada.
            //El contorno de esta zona cerrada debe tener sentido antihorario.
            //Las zonas se comprueban en el codigo con el siguiente orden de prioridad: runway, stand, apron, taxi y airborne(el resto).
            //Esto permite, por ej., que apron tenga zonas superpuestas a stand, para reducir numero y complejidad de cada zona
            //Cada punto se somete por orden a cada zona a traves de un metodo de optimización lineal
            //Si un punto cumple en una zona, ya nos comprueba en la siguiente
            //Las zonas asignadas son las siguientes: -1/Sin zona (no hay datos de posicion); 0/airborne; 01/type 4; 02/type 5
            // 11/pista 25L; 12/pista 02; 13/pista 25R;
            // 21/stand T1; 22/stand T2;  31/apron T1; 32/apron T2; 4/taxi

            bool[] puntoEncontrado = new bool[myList.Count()]; //false by default. Si el punto se encuentra en una zona, ya no se compara en las siguientes zonas
            bool[] puntoNoCumple; //Si en una inecuacion se encuentra que no cumple, ya no se compara con las siguiente inecuaciones de esa seccion
            double[,] posicion = new double[myList.Count(), 2];
            double[] velocidad = new double[myList.Count()]; //Velocidad absoluta [m/s]

            for (int i = 0; i < myList.Count(); i++)
            {
                //Miramos si los datos de posicion del paquete vienen en WGS, del SMR, o en coordenadas cartesianas del MLAT
                posicion[i, 0] = myList[i].GetPosicion()[0];
                posicion[i, 1] = myList[i].GetPosicion()[1];
                if ((posicion[i, 0] == Math.Pow(10, 8)) && (posicion[i, 1] == Math.Pow(10, 8)))
                {
                    puntoEncontrado[i] = true; //Ya no entra en el codigo, zona se queda en -1
                }

                if ((myList[i].GetCartTrackVelX() != Math.Pow(10, 8)) && (myList[i].GetCartTrackVelY() != Math.Pow(10, 8)))
                {
                    velocidad[i] = Math.Sqrt(Math.Pow(myList[i].GetCartTrackVelX(), 2) + Math.Pow(myList[i].GetCartTrackVelY(), 2));
                }
                else if ((myList[i].GetPolarTrackVelV() != Math.Pow(10, 8)) && (myList[i].GetPolarTrackVelAngle() != Math.Pow(10, 8)))
                {
                    velocidad[i] = myList[i].GetPolarTrackVelV();
                }
                else velocidad[i] = Math.Pow(10, 8);
            }

            StreamReader leer = new StreamReader("Secciones.txt");
            string linea1 = leer.ReadLine();
            string lineaInicial = linea1; //Guardamos el primer punto para poder cerrar el controno con el ultimo punto
            string linea2 = leer.ReadLine();
            int numZona = 1;//Numero arbitrario asignado de manera ascendente a cada zona de secciones.txt. 
            bool finSeccion;//Controla si es seccion ha acabado
            bool ultimaLinea;//Se activa para comparar la ultima linea con la primera
            int velocidadLimitePista = 120; //[m/s]Las tres primeras zonas son las pistas. Las siguientes tendran velocidad limite de 35m/s
            int velocidadLimiteRodadura = 35; //Velocidad limite para filtrar aviones que sobrevuelan el aeropuerto.
            while (linea2 != null)
            {
                ultimaLinea = false;
                finSeccion = false;
                puntoNoCumple = new bool[myList.Count()];
                while (!finSeccion)
                {
                    string[] punto = linea1.Split(' ');
                    double[] coordenadaInicial = { Convert.ToDouble(punto[0]), Convert.ToDouble(punto[1]) };
                    punto = linea2.Split(' ');
                    double[] coordenadaFinal = { Convert.ToDouble(punto[0]), Convert.ToDouble(punto[1]) };
                    double Ax = coordenadaFinal[0] - coordenadaInicial[0];
                    double Ay = coordenadaFinal[1] - coordenadaInicial[1];
                    string operador = "<"; //Determina el sentido de la inecuacion. Premisa: contorno de la seccion antihorario
                    if (((Ax > 0) && (Ay >= 0)) || ((Ax >= 0) && (Ay < 0)))
                        operador = ">";
                    //Sino (((Ax <= 0) && (Ay > 0)) || ((Ax < 0) && (Ay <= 0))) por lo que operador permanece = "<";

                    if (operador == ">") //Si el punto no cumple alguna de las inecuaciones de la seccion, no cumple
                    {
                        for (int i = 0; i < myList.Count(); i++)
                        {
                            if ((!puntoEncontrado[i]) && (!puntoNoCumple[i]))
                            {
                                if (posicion[i, 1] - (Ay / Ax) * posicion[i, 0] < coordenadaInicial[1] - (Ay / Ax) * coordenadaInicial[0])
                                    puntoNoCumple[i] = true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < myList.Count(); i++)
                        {
                            if ((!puntoEncontrado[i]) && (!puntoNoCumple[i]))
                            {
                                if (posicion[i, 1] - (Ay / Ax) * posicion[i, 0] > coordenadaInicial[1] - (Ay / Ax) * coordenadaInicial[0])
                                    puntoNoCumple[i] = true;
                            }
                        }
                    }

                    linea1 = linea2;
                    linea2 = leer.ReadLine();
                    if (ultimaLinea)
                        finSeccion = true;
                    if (linea2 == "/")
                    {
                        ultimaLinea = true;
                        linea2 = lineaInicial;
                    }
                }
                for (int i = 0; i < myList.Count(); i++)
                {
                    if ((!puntoNoCumple[i]) && (!puntoEncontrado[i]))
                    {
                        puntoEncontrado[i] = true;
                        if (numZona <= 5)//Las 5 primeras zonas son las 3 pistas y los dos cruces
                        {
                            if (velocidad[i] <= velocidadLimitePista)
                            {
                                if ((numZona == 1) || (numZona == 3))// cruce de pista 25L con aproximacion a 02 o 25R con 02
                                {
                                    double anguloVelocidad; //Angulo de movimiento [º]
                                    if ((myList[i].GetCartTrackVelX() != Math.Pow(10, 8)) && (myList[i].GetCartTrackVelY() != Math.Pow(10, 8)))
                                    {
                                        if ((myList[i].GetCartTrackVelX() == 0) && (myList[i].GetCartTrackVelY() > 0))
                                            anguloVelocidad = 90;
                                        else if ((myList[i].GetCartTrackVelX() == 0) && (myList[i].GetCartTrackVelY() < 0))
                                            anguloVelocidad = 270;
                                        else if ((myList[i].GetCartTrackVelX() > 0) && (myList[i].GetCartTrackVelY() == 0))
                                            anguloVelocidad = 0;
                                        else if ((myList[i].GetCartTrackVelX() < 0) && (myList[i].GetCartTrackVelY() == 0))
                                            anguloVelocidad = 180;
                                        else if (myList[i].GetCartTrackVelX() < 0)
                                            anguloVelocidad = Math.Atan(myList[i].GetCartTrackVelY() / myList[i].GetCartTrackVelX()) * (180 / Math.PI) + 180;
                                        else if ((myList[i].GetCartTrackVelX() > 0) && (myList[i].GetCartTrackVelY() < 0))
                                            anguloVelocidad = Math.Atan(myList[i].GetCartTrackVelY() / myList[i].GetCartTrackVelX()) * (180 / Math.PI) + 360;
                                        else anguloVelocidad = Math.Atan(myList[i].GetCartTrackVelY() / myList[i].GetCartTrackVelX()) * (180 / Math.PI);

                                        if (((anguloVelocidad >= 47.75) && (anguloVelocidad <= 94.25)) || ((anguloVelocidad >= 227.25) && (anguloVelocidad <= 274.25)))
                                        {
                                            if (numZona == 1)
                                                myList[i].SetZona(01);//Sigue en airborne, haciendo SID/STAR, Area tipo 4
                                        }
                                        else
                                        {
                                            if (numZona == 3)
                                                myList[i].SetZona(13);
                                        }
                                    }
                                    else if ((myList[i].GetPolarTrackVelV() != Math.Pow(10, 8)) && (myList[i].GetPolarTrackVelAngle() != Math.Pow(10, 8)))
                                    {
                                        anguloVelocidad = myList[i].GetPolarTrackVelAngle();
                                        if ((anguloVelocidad <= 42.25) || (anguloVelocidad >= 355.75) || ((anguloVelocidad >= 222.25) && (anguloVelocidad <= 175.75)))
                                        {
                                            if (numZona == 1)
                                                myList[i].SetZona(01);
                                        }
                                        else
                                        {
                                            if (numZona == 3)
                                                myList[i].SetZona(13);
                                        }
                                    }
                                    if (numZona == 1)
                                    {
                                        if (myList[i].GetZona() != 01)
                                            puntoEncontrado[i] = false;
                                    }
                                    if (numZona == 3)
                                    {
                                        if (myList[i].GetZona() != 13)
                                            puntoEncontrado[i] = false;
                                    }
                                }
                                else if (numZona == 2)//11 pista 25L
                                    myList[i].SetZona(11);
                                else if (numZona == 4)//12 pista 02
                                    myList[i].SetZona(12);
                                else if (numZona == 5)//13 pista 25R
                                    myList[i].SetZona(13);
                            }
                            else myList[i].SetZona(0); //Zona Airborne
                        }
                        else
                        {
                            if (velocidad[i] <= velocidadLimiteRodadura)
                            {
                                if ((numZona >= 6) && (numZona <= 9))//21 stand T1
                                    myList[i].SetZona(21);
                                else if ((numZona >= 10) && (numZona <= 16))//22 stand T2
                                    myList[i].SetZona(22);
                                else if ((numZona >= 17) && (numZona <= 22))//31 apron T1
                                    myList[i].SetZona(31);
                                else if ((numZona >= 23) && (numZona <= 25))//32 apron T2
                                    myList[i].SetZona(32);
                                else if ((numZona >= 26) && (numZona <= 36))//4 taxi
                                    myList[i].SetZona(4);
                                else
                                    myList[i].SetZona(0);
                            }
                            else //Si la vel no es superior a la limite de rodadura no entra aqui, descartando posibles blancos fijos dentro de las areas 4,5
                            {
                                if ((numZona >= 37) && (numZona <= 42))//01 Area tipo 4
                                    myList[i].SetZona(01);
                                else if ((numZona >= 43) && (numZona <= 48))//02 Area tipo 5
                                    myList[i].SetZona(02);
                                else
                                    puntoEncontrado[i] = false;
                            }
                        }
                    }
                }
                if (linea2 != null)
                {
                    linea1 = linea2;
                    lineaInicial = linea1;
                    linea2 = leer.ReadLine();
                }
                numZona++;
            }
            for (int i = 0; i < myList.Count(); i++)
            {
                if (!puntoEncontrado[i])
                {
                    myList[i].SetZona(0);//Su posicion es conocida, pero no esta en ninguna area definida. Tampoco sobrevuela LEBL
                }
            }
            leer.Close();
        }

        public void SetUTCcorregido()//A las horas de >24 les suma 24
        {
            if (myList.Count() != 0)
            {
                bool cambioDia = false;
                myList[0].SetUTCcorregido(myList[0].GetUTC());
                for (int i = 1; i < myList.Count(); i++)
                {
                    if (!cambioDia)
                    {
                        if (myList[i].GetUTC() < myList[i - 1].GetUTC())
                        {
                            myList[i].SetUTCcorregido(myList[i].GetUTC() + 24 * 60);
                            cambioDia = true;
                        }
                        else
                        {
                            myList[i].SetUTCcorregido(myList[i].GetUTC());
                        }
                    }
                    else myList[i].SetUTCcorregido(myList[i].GetUTC() + 24 * 60);
                }
            }
        }

        public void SetVelocidadDGPS() //Asignamos velocidad para poder despues asignar una zona
        {
            for (int i = 0; i < myList.Count() - 1; i++)
            {
                double[] posI = myList[i].GetPosicion();
                double tiempoI = myList[i].GetUTCcorregido() * 60;
                double[] posF = myList[i + 1].GetPosicion();
                double tiempoF = myList[i + 1].GetUTCcorregido() * 60;
                if (tiempoF - tiempoI != 0)
                {
                    myList[i].SetVelocidad((posF[0] - posI[0]) / (tiempoF - tiempoI), (posF[1] - posI[1]) / (tiempoF - tiempoI));
                }
            }
            //La ultima velocidad es igual a la penultima (aproximación)
            myList[myList.Count() - 1].SetVelocidad(myList[myList.Count() - 2].GetCartTrackVelX(), myList[myList.Count() - 2].GetCartTrackVelY());
        }

        public List<List<int>> AircraftICAODetected() //Retorna una matriz que en cada fila tiene los indices de myList que corresponden a un unico avion (compara ICAO Adress)
        {
            int indice = 0;
            List<string> listaAviones = new List<string>();

            while ((myList[indice].GetICAOAdress() == "No Data") && (indice < myList.Count()))
            {
                indice++;
            }

            if (indice < myList.Count())
            {
                listaAviones.Add(myList[indice].GetICAOAdress());
                for (int i = indice + 1; i < myList.Count(); i++)
                {
                    if (myList[i].GetICAOAdress() != "No Data")
                    {
                        bool yaExiste = false;
                        int n = 0;
                        while ((n < listaAviones.Count()) && (!yaExiste))
                        {
                            if (myList[i].GetICAOAdress() == listaAviones[n])
                            {
                                yaExiste = true;
                            }
                            else n++;
                        }
                        if (!yaExiste)
                            listaAviones.Add(myList[i].GetICAOAdress());
                    }
                }
            }

            List<List<int>> matrizAviones = new List<List<int>>();
            for (int i = 0; i < listaAviones.Count(); i++)
            {
                List<int> filaAviones = new List<int>();
                for (int n = 0; n < myList.Count(); n++)
                {
                    if (listaAviones[i] == myList[n].GetICAOAdress())
                    {
                        filaAviones.Add(n);
                    }
                }
                matrizAviones.Add(filaAviones);
            }
            return (matrizAviones);
        }

        public List<List<int>> AircraftTrackNumberDetected() //Retorna una matriz que en cada fila tiene los indices de myList que corresponden a un unico avion (compara numero de pista)
        {
            int indice = 0;
            List<string> listaAviones = new List<string>();

            while ((myList[indice].GetTrackNumber() == "No Data") && (indice < myList.Count()))
            {
                indice++;
            }

            if (indice < myList.Count())
            {
                listaAviones.Add(myList[indice].GetTrackNumber());
                for (int i = indice + 1; i < myList.Count(); i++)
                {
                    if (myList[i].GetTrackNumber() != "No Data")
                    {
                        bool yaExiste = false;
                        int n = 0;
                        while ((n < listaAviones.Count()) && (!yaExiste))
                        {
                            if (myList[i].GetTrackNumber() == listaAviones[n])
                            {
                                yaExiste = true;
                            }
                            else n++;
                        }
                        if (!yaExiste)
                            listaAviones.Add(myList[i].GetTrackNumber());
                    }
                }
            }

            List<List<int>> matrizAviones = new List<List<int>>();
            for (int i = 0; i < listaAviones.Count(); i++)
            {
                List<int> filaAviones = new List<int>();
                for (int n = 0; n < myList.Count(); n++)
                {
                    if (listaAviones[i] == myList[n].GetTrackNumber())
                    {
                        filaAviones.Add(n);
                    }
                }
                matrizAviones.Add(filaAviones);
            }
            return (matrizAviones);
        }

        public double[,] GetDimensiones(string icao)//Compara las dimensiones de los diferente puntos de un vehiculo
        {
            double[,] dimensiones = new double[2, 2];
            double[,] dimensionesi = new double[2, 2];
            if (icao != "")
            {
                int i = 0;
                bool encontrado = false;
                while ((i < myList.Count())&&(!encontrado))
                {
                    if(myList[i].GetICAOAdress()==icao)
                    {
                        encontrado = true;
                        dimensiones[0, 0] = Convert.ToInt32(myList[i].GetPosicion()[0]);
                        dimensiones[0, 1] = Convert.ToInt32(myList[i].GetPosicion()[0]);
                        dimensiones[1, 0] = Convert.ToInt32(myList[i].GetPosicion()[1]);
                        dimensiones[1, 1] = Convert.ToInt32(myList[i].GetPosicion()[1]);
                    }
                    i++;
                }

                while (i < myList.Count())
                {
                    if (myList[i].GetICAOAdress() == icao)
                    {
                        dimensionesi[0, 0] = Convert.ToInt32(myList[i].GetPosicion()[0]);
                        dimensionesi[0, 1] = Convert.ToInt32(myList[i].GetPosicion()[0]);
                        dimensionesi[1, 0] = Convert.ToInt32(myList[i].GetPosicion()[1]);
                        dimensionesi[1, 1] = Convert.ToInt32(myList[i].GetPosicion()[1]);
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
                    }
                    i++;
                }
            }
            return (dimensiones);
        }

        public double GetProporcion(double[,] dimensiones, double panelDimensionesHeight, double panelDimensionesWidth)//Valor con el que proporcionar el mapa
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
    }
}
