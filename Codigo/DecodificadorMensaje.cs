using System;
using System.Collections.Generic;
using System.IO;

namespace Codigo
{
    public class DecodificadorMensaje
    {
        //Atributos

        int CAT; //Categoria: 2 numeros. Ej: 10
        string SIC; //3 numeros (es string porque sino al poner 000 llegaria un 0). 
        string SAC; //3 numeros (idem SIC)
        double UTC; //En minutos y decimas de minuto. 
        string trackNumber; //4 cifras
        string FL; //X cifras.
        string callSign; //Target Identification
        string ICAOAdress; //Poner en hexadecimal
        double cartX; //Cartesian coordinatex, X component[m]
        double cartY; //[m]
        double polarRho; //Polar coordinates, Rho component[m].
        double polarTheta; //[º]
        double cartFromPolarX; //Función en la libreria que covierte de polar a cartesiano. En la tabla mostrar info en polar, pero hace falta en 
        // cartesiando para plotear. Idem para los siguientes "cartFrom".
        double cartFromPolarY;
        string WGS84Lat; //Latitud en WGS84. Sin signo, hay abajo una variable para el signo.
        string WGS84Long; //Longitud en WGS84. Sin signo.
        string[] WGS84Sign; //Vector con 2 posiciones. En la primera "N" ó "S"; en la segunda "E" ó "W". Ej: WGS84Sign=[N,E]
        double MLATfromSMRX; //Hay una funcion que cambia la referencia del SMR al MLAT
        double MLATfromSMRY;
        double cartTrackVelX; //Cartesian track velocity (X component).
        double cartTrackVelY;  //[m/s]
        double polarTrackVelV; //Polar track velocity (kt).
        double polarTrackVelAngle; //[º].
        double accX; //Calculated Acceleration X component
        double accY;
        int zona; //Diferencia entre cada zona del aeropuerto: runway, stand, apron, taxi y airborne
        int indiceDGPS; //Para el dichero DGPS, guarda la posicion del vector myList con el que se tienen que comparar lo de la lista DGPS
        double UTCcorregido; //En minutos y decimas de minuto. 

        //Constructor: asigna los valores nulos
        public DecodificadorMensaje()
        {
            this.CAT = -1;
            this.SIC = "No Data";
            this.SAC = "No Data";
            this.UTC = -1;
            this.trackNumber = "No Data";
            this.FL = null;
            this.callSign = "No Data";
            this.ICAOAdress = "No Data";
            this.cartX = Math.Pow(10, 8); //Valor imposible para CartX y siguientes double/int que puedan ser negativos;
            this.cartY = Math.Pow(10, 8);
            this.polarRho = Math.Pow(10, 8);
            this.polarTheta = Math.Pow(10, 8);
            this.cartFromPolarX = Math.Pow(10, 8);
            this.cartFromPolarY = Math.Pow(10, 8);
            this.WGS84Lat = null;
            this.WGS84Long = null;
            this.WGS84Sign = null;
            this.MLATfromSMRX = Math.Pow(10, 8);
            this.MLATfromSMRY = Math.Pow(10, 8);
            this.cartTrackVelX = Math.Pow(10, 8);
            this.cartTrackVelY = Math.Pow(10, 8);
            this.polarTrackVelV = Math.Pow(10, 8);
            this.polarTrackVelAngle = Math.Pow(10, 8);
            this.accX= Math.Pow(10, 8);
            this.accY= Math.Pow(10, 8);
            this.zona = -1; //Solo permanece asi si su posicion no es conocida. 
            this.indiceDGPS = -1;
            this.UTCcorregido = -1;
        }

        //Funciones principales

        public void GetMlatFromSMR() //Cambia la referencia del SMR al MLAT para centrar todos los datos en el mismo origen
        {
            string[] Mlat = new string[] { "411749426N", "0020442410E" }; //Coordenadas del MLAT y SMR
            string[] SMR = new string[] { "411744226N", "0020542411E" };
            double[] cartMlat = new double[2];
            double[] cartSMR = new double[2];
            cartMlat[0] = ConvertWGStoRad(Mlat[0], 0);
            cartMlat[1] = ConvertWGStoRad(Mlat[1], 1);
            cartSMR[0] = ConvertWGStoRad(SMR[0], 0);
            cartSMR[1] = ConvertWGStoRad(SMR[1], 1);
            double[] incremento = new double[2];
            incremento = ConvertRadtoXY(cartMlat, cartSMR[0], cartSMR[1]);
            if ((this.SIC == "007") &&(this.cartX != Math.Pow(10, 8)) && (this.cartY != Math.Pow(10, 8)))
            {
                this.MLATfromSMRX = this.cartX + incremento[0];
                this.MLATfromSMRY = this.cartY + incremento[1];
            }
        }

        public int[] GetCartFromWGS84() //int[x,y] Utiliza ConvertWGStoRad y lego ConvertRadtoXY. Lo referencia al MLAT
        {
            //CAT21 usa ADS-B. Este esta centrado en MLAT
            string[] Mlat = new string[] { "411749426N", "0020442410E" }; //Coordenadas del MLAT y SMR
            double[] cartMlat = new double[2];
            int[] result = new int[2];
            if ((this.WGS84Lat != null) && (this.WGS84Long != null))
            {
                cartMlat[0] = ConvertWGStoRad(Mlat[0], 0);
                cartMlat[1] = ConvertWGStoRad(Mlat[1], 1);
                result[0] = Convert.ToInt32(ConvertRadtoXY(cartMlat, ConvertWGStoRad(this.WGS84Lat + this.WGS84Sign[0], 0), ConvertWGStoRad(this.WGS84Long + this.WGS84Sign[1], 1))[0]);
                result[1] = Convert.ToInt32(ConvertRadtoXY(cartMlat, ConvertWGStoRad(this.WGS84Lat + this.WGS84Sign[0], 0), ConvertWGStoRad(this.WGS84Long + this.WGS84Sign[1], 1))[1]);
                return (result);
            }
            else
            {
                result[0] = Convert.ToInt32(Math.Pow(10, 8));
                result[1] = Convert.ToInt32(Math.Pow(10, 8));
                return (result);
            }
        }
        
        static string BytesEnString(byte[] buffer, int offset, int count)
        /*Dado un vector de bytes, un offset y un entero count, devuelve un string de 
         * los bytes expresados en binario, tantos bytes como indique count y a partir del byte en la posicion offset*/
        {
            string byteString;
            string ceros;
            string resultado = "";
            int contador = 0; //Cuenta por qué byte vamos
            int i = 0; //Cuenta los ceros a añadir para el byte actual
            while (contador < count)
            {
                byteString = Convert.ToString(buffer[offset], 2);
                ceros = "";
                //Añadir ceros
                i = 0;
                while (i < 8 - byteString.Length)
                {
                    ceros = ceros + "0";
                    i = i + 1;
                }
                //Guardamos el byte en string e incrementamos contador y offset
                resultado = resultado + ceros + byteString;
                offset = offset + 1;
                contador = contador + 1;
            }

            return resultado;
        }

        static int ConvertirCA2aEntero(string CA2)
        /*Convierte un string en Complemento a 2 a número entero decimal (es decir, suponiendo que LSB tiene valor 1)*/
        /*ADVERTENCIA: El string debe tener al menos dos caracteres, ya que MSB solo sirve para indicar el signo del numero*/
        {
            //Comprobamos si es un número negativo
            bool negativo;
            if (Convert.ToInt32("" + CA2[0]) == 1)
                negativo = true;
            else
                negativo = false;

            //Lectura del numero y conversion a decimal
            int result;
            string binario = "";
            int i = 1; //Primer bit solo indica el signo
            if (negativo)
            {
                while (i < CA2.Length)
                {
                    if (Convert.ToInt32("" + CA2[i]) == 1)
                        binario = binario + "0";
                    if (Convert.ToInt32("" + CA2[i]) == 0)
                        binario = binario + "1";
                    i = i + 1;
                }
                result = Convert.ToInt32(binario, 2) + 1;
                result = -result;
            }
            else
            {
                while (i < CA2.Length)
                {
                    if (Convert.ToInt32("" + CA2[i]) == 1)
                        binario = binario + "1";
                    if (Convert.ToInt32("" + CA2[i]) == 0)
                        binario = binario + "0";
                    i = i + 1;
                }
                result = Convert.ToInt32(binario, 2);
            }
            return result;
        }

        static string TraducirCallSign(string caracteresBinario)
        /*Dado un string en binario con los 8 caracteres de un CallSign (6 bits cada uno) te devuelve el string en ASCII*/
        {
            string callSign = "";       //string con el CallSign descifrado
            string caracter;            //Caracter actual a descifrar
            int caractBit;              //8 caracteres
            int bit = 0;                //6 bits por caracter (b6 b5 b4 b3 b2 b1), en total 48 bits a descifrar
            int parte1;                 //Cada caracter se dividirá en dos partes: b6b5 (parte1) y b4b3b2b1 (parte2) 
            int parte2;                 //Decodificación basada en el International Alphabet Number 5 (Ver Documento ICAO Anexo 10, Volumen IV, sección 3.1.2.9)
            bool callsignUtil = false;  //Controla si el callsign recibido es legible. Si solo se reciben caracteres especiales y/o espacios se considera ilegible 

            //El string de entrada debe tener un (múltiplo de 6) caracteres
            if ((caracteresBinario.Length) % 6 != 0)
                callSign = "not correctly interpreted";
            else
            {
                while (bit < caracteresBinario.Length)
                {
                    //Dividimos el caracter actual en dos partes
                    caractBit = 0;
                    caracter = "";
                    while (caractBit < 2)
                    {
                        caracter = caracter + caracteresBinario[bit];
                        caractBit = caractBit + 1;
                        bit = bit + 1;
                    }
                    parte1 = Convert.ToInt32(caracter, 2);
                    caracter = "";
                    while (caractBit < 6)
                    {
                        caracter = caracter + caracteresBinario[bit];
                        caractBit = caractBit + 1;
                        bit = bit + 1;
                    }
                    parte2 = Convert.ToInt32(caracter, 2);

                    //Decodificacion del caracter segun IA-5
                    if (parte1 == 0)
                    {
                        if (parte2 == 0)
                            callSign = callSign + " ";
                        if (parte2 == 1)
                        {
                            callSign = callSign + "A";
                            callsignUtil = true;
                        }
                        if (parte2 == 2)
                        {
                            callSign = callSign + "B";
                            callsignUtil = true;
                        }
                        if (parte2 == 3)
                        {
                            callSign = callSign + "C";
                            callsignUtil = true;
                        }
                        if (parte2 == 4)
                        {
                            callSign = callSign + "D";
                            callsignUtil = true;
                        }
                        if (parte2 == 5)
                        {
                            callSign = callSign + "E";
                            callsignUtil = true;
                        }
                        if (parte2 == 6)
                        {
                            callSign = callSign + "F";
                            callsignUtil = true;
                        }
                        if (parte2 == 7)
                        {
                            callSign = callSign + "G";
                            callsignUtil = true;
                        }
                        if (parte2 == 8)
                        {
                            callSign = callSign + "H";
                            callsignUtil = true;
                        }
                        if (parte2 == 9)
                        {
                            callSign = callSign + "I";
                            callsignUtil = true;
                        }
                        if (parte2 == 10)
                        {
                            callSign = callSign + "J";
                            callsignUtil = true;
                        }
                        if (parte2 == 11)
                        {
                            callSign = callSign + "K";
                            callsignUtil = true;
                        }
                        if (parte2 == 12)
                        {
                            callSign = callSign + "L";
                            callsignUtil = true;
                        }
                        if (parte2 == 13)
                        {
                            callSign = callSign + "M";
                            callsignUtil = true;
                        }
                        if (parte2 == 14)
                        {
                            callSign = callSign + "N";
                            callsignUtil = true;
                        }
                        if (parte2 == 15)
                        {
                            callSign = callSign + "O";
                            callsignUtil = true;
                        }
                    }
                    if (parte1 == 1)
                    {
                        if (parte2 == 0)
                        {
                            callSign = callSign + "P";
                            callsignUtil = true;
                        }
                        if (parte2 == 1)
                        {
                            callSign = callSign + "Q";
                            callsignUtil = true;
                        }
                        if (parte2 == 2)
                        {
                            callSign = callSign + "R";
                            callsignUtil = true;
                        }
                        if (parte2 == 3)
                        {
                            callSign = callSign + "S";
                            callsignUtil = true;
                        }
                        if (parte2 == 4)
                        {
                            callSign = callSign + "T";
                            callsignUtil = true;
                        }
                        if (parte2 == 5)
                        {
                            callSign = callSign + "U";
                            callsignUtil = true;
                        }
                        if (parte2 == 6)
                        {
                            callSign = callSign + "V";
                            callsignUtil = true;
                        }
                        if (parte2 == 7)
                        {
                            callSign = callSign + "W";
                            callsignUtil = true;
                        }
                        if (parte2 == 8)
                        {
                            callSign = callSign + "X";
                            callsignUtil = true;
                        }
                        if (parte2 == 9)
                        {
                            callSign = callSign + "Y";
                            callsignUtil = true;
                        }
                        if (parte2 == 10)
                        {
                            callSign = callSign + "Z";
                            callsignUtil = true;
                        }
                    }
                    if (parte1 == 2)
                    {
                        if (parte2 == 0)
                            //Caracter especial
                            callSign = callSign + "";
                    }
                    if (parte1 == 3)
                    {
                        if (parte2 == 0)
                        {
                            callSign = callSign + "0";
                            callsignUtil = true;
                        }
                        if (parte2 == 1)
                        {
                            callSign = callSign + "1";
                            callsignUtil = true;
                        }
                        if (parte2 == 2)
                        {
                            callSign = callSign + "2";
                            callsignUtil = true;
                        }
                        if (parte2 == 3)
                        {
                            callSign = callSign + "3";
                            callsignUtil = true;
                        }
                        if (parte2 == 4)
                        {
                            callSign = callSign + "4";
                            callsignUtil = true;
                        }
                        if (parte2 == 5)
                        {
                            callSign = callSign + "5";
                            callsignUtil = true;
                        }
                        if (parte2 == 6)
                        {
                            callSign = callSign + "6";
                            callsignUtil = true;
                        }
                        if (parte2 == 7)
                        {
                            callSign = callSign + "7";
                            callsignUtil = true;
                        }
                        if (parte2 == 8)
                        {
                            callSign = callSign + "8";
                            callsignUtil = true;
                        }
                        if (parte2 == 9)
                        {
                            callSign = callSign + "9";
                            callsignUtil = true;
                        }
                    }
                }
                if (!callsignUtil)
                    callSign = "No Data";
            }
            return callSign;
        }

        public void LeerSiguienteMensaje(FileStream fs)
        //Decodifica el siguiente mensaje de un archivo Asterix y rellena el DecodificadorMensaje con su informacion
        {
            //Creo un buffer y leo los primeros tres bits
            byte[] buffer = new byte[1024];
            int byteActual = 0; //Contador que lleva por qué byte vamos en el buffer

            //Leo el primer byte con la CAT
            fs.Read(buffer, byteActual, 1);

            //Si el buffer está lleno, rellenamos el mensaje. Estará lleno si el primer byte es distinto a 0 (CAT!=0)
            if (buffer[byteActual] != 0)
            {
                this.CAT=buffer[byteActual];
                byteActual = byteActual + 1;

                //Segun el tipo de CAT se decodifica de una forma u otra
                if (CAT == 10)
                {
                    DecodificarCAT10(fs, buffer, byteActual);
                    GetMlatFromSMR();
                }
                else if (CAT == 20)
                {
                    DecodificarCAT20(fs, buffer, byteActual);
                    GetMlatFromSMR();
                }
                else
                {
                    SaltarMensaje(fs, buffer, byteActual);
                }
            }
        }

        public void SaltarMensaje(FileStream fs, byte[] buffer, int byteActual)
        {
            //No necesario decodificar, pero hace falta leer length para saltar mensaje
            fs.Read(buffer, byteActual, 2);
            int LEN = Convert.ToInt32(BytesEnString(buffer, 1, 2), 2);
            byteActual = byteActual + 2;
            fs.Read(buffer, byteActual, LEN - 3);
        }

        static int[] BitVector(string stringBits)
        {
            //Convierte un string de 0 y 1 en un vector de int que en cada componente guarda un dígito
            int[] resultado = new int[stringBits.Length];
            int i = 0;
            while (i < stringBits.Length)
            {
                resultado[i] = Convert.ToInt32(Convert.ToString(stringBits[i]));
                i = i + 1;
            }
            return resultado;
        }

        public void DecodificarCAT10(FileStream fs, byte[] buffer, int byteActual)
        {
            //Leemos la Length del mensaje
            fs.Read(buffer, byteActual, 2);
            int LEN = Convert.ToInt32(BytesEnString(buffer, 1, 2), 2);
            byteActual = byteActual + 2;

            //Guardamos el resto del mensaje en buffer
            fs.Read(buffer, byteActual, LEN - 3);

            //Leemos FSPEC
            string lectura;             //Guarda el byte actual de FSPEC en un string
            int bit;                        //Contador del bit actual que se está leyendo
            bool[] FSPEC = new bool[28];    //el vector indica qué Data Items se han enviado en el mensaje
            int FRN = 0;                    //Indica la posicion actual de FSPEC

            bool FX = true; //Cuando se lee FSPEC, si el último bit es 1 se sigue leyendo el siguiente byte. Si no, acaba FSPEC.
            while (FX)
            {
                lectura = BytesEnString(buffer, byteActual, 1);

                //Leemos los bits y guardamos qué Data Items se han enviado
                bit = 0;
                while (bit < 7)
                {
                    if (Convert.ToInt32("" + lectura[bit]) == 1)
                        FSPEC[FRN] = true;
                    if (Convert.ToInt32("" + lectura[bit]) == 0)
                        FSPEC[FRN] = false;

                    bit = bit + 1;
                    FRN = FRN + 1;
                }
                //leemos último bit
                if (Convert.ToInt32("" + lectura[bit]) == 0)
                    FX = false;

                byteActual = byteActual + 1;
            }

            //Lectura de cada uno de los Data Fields

            //FRN=1, Data Item I010/010: Data Source Identifier
            if (FSPEC[0])
            {
                //SAC
                lectura = Convert.ToString(buffer[byteActual], 10);
                while (lectura.Length < 3)
                    lectura = "0" + lectura;
                this.SAC= lectura;
                byteActual++;

                //SIC
                lectura = Convert.ToString(buffer[byteActual], 10);
                while (lectura.Length < 3)
                    lectura = "0" + lectura;
                this.SIC = lectura;
                byteActual++;
            }

            //FRN=2, Data Item I010/000: Message Type
            if (FSPEC[1])
            {
                byteActual++;
            }

            //FRN=3, Data Item I010/020: Target Report Descriptor (Variable Length Data)
            if (FSPEC[2])
            {
                //Lectura de los bytes que forman el Data Item (mismo algoritmo que FSPEC)
                FX = true;
                while (FX)
                {
                    lectura = BytesEnString(buffer, byteActual, 1);
                    bit = 0;
                    while (bit < 7)
                    {
                        bit = bit + 1;
                    }
                    if (Convert.ToInt32("" + lectura[bit]) == 0)
                    {
                        FX = false;
                    }
                    byteActual = byteActual + 1;
                }
            }

            //FRN=4, Data Item I020/140: Time of Day
            if (FSPEC[3])
            {
                lectura = BytesEnString(buffer, byteActual, 3);
                //Conversión de hora UTC a minutos y decimas de minuto
                //El LSB vale 1/128 segundos. Dividiendo el valor entre 128 obtenemos segundos
                int lecturaUTC = Convert.ToInt32(lectura, 2);       //Conversion a decimal entero
                this.UTC=Convert.ToDouble(lecturaUTC) / (60 * 128);  //Al dividir se obtienen decimales, pero eso conversion a double

                byteActual = byteActual + 3;
            }

            //FRN=5, Data Item I010/041: Position in WGS-84 Coordinates
            if (FSPEC[4])
            {
                string item41Lat = BytesEnString(buffer, byteActual, 4);
                byteActual = byteActual + 4;
                //obtencion del numero decimal de las coordenadas en grados
                double item41Latint = Convert.ToDouble((ConvertirCA2aEntero(item41Lat)) * 180) / (Math.Pow(2, 31));
                string item41Long = BytesEnString(buffer, byteActual, 4);
                byteActual = byteActual + 4;
                double item41Longint = Convert.ToDouble((ConvertirCA2aEntero(item41Long)) * 180) / (Math.Pow(2, 31));
                string[] WGS84Sign = new string[2];

                //Se rellena la matriz de signo
                if (item41Latint > 0)
                {
                    WGS84Sign[0] = "N";
                }
                else
                {
                    WGS84Sign[0] = "S";
                }
                if (item41Longint > 0)
                {
                    WGS84Sign[1] = "E";
                }
                else
                {
                    WGS84Sign[1] = "W";
                }
                this.WGS84Sign = WGS84Sign;

                //falta convertir los números decimales en 6 dígitos sin coma y sin signo.
                //primero se elimina el simbolo y se combierte a string
                double item41LatintPos = Math.Abs(item41Latint);
                double item41LongintPos = Math.Abs(item41Longint);
                //convertir a string y en caso que se trate de un número inferior a 10 grados añadir un 0 al convertir a string para que no se confunda después. Ej: 2,2157º
                //en long, en caso de que sea inferior a 10º se añaden 2 ceros, si menor a 100 se añade 1 

                //2,2157891≠022157891 sino que es igual a :    2º ; 0.2157891*60=12.947346=12’;  0.947346*60=56.84076=56''
                //0.84076*1000=840.76=841
                string LAT;
                string LONG;

                //Lat
                int grados = Convert.ToInt32(Math.Floor(item41LatintPos));
                int minutos;
                int segundos;
                int decimas;//3 decimas
                double decimales = (item41LatintPos - Convert.ToDouble(grados)) * 60;
                minutos = Convert.ToInt32(Math.Floor(decimales));
                decimales = (decimales - Convert.ToDouble(minutos)) * 60;
                segundos = Convert.ToInt32(Math.Floor(decimales));
                decimales = (decimales - Convert.ToDouble(segundos)) * 1000;
                decimas = Convert.ToInt32(Math.Round(decimales));
                string Grados = Convert.ToString(grados);
                string Minutos = Convert.ToString(minutos);
                string Segundos = Convert.ToString(segundos);
                string Decimas = Convert.ToString(decimas);

                if (grados < 10)
                {
                    Grados = "0" + Grados;
                }
                if (minutos < 10)
                {
                    Minutos = "0" + Minutos;
                }
                if (segundos < 10)
                {
                    Segundos = "0" + Segundos;
                }
                if (decimas < 100)
                {
                    Decimas = "0" + Decimas;
                    if (decimas < 10)
                    {
                        Decimas = "0" + Decimas;
                    }
                }
                LAT = Grados + Minutos + Segundos + Decimas;

                //Long
                grados = Convert.ToInt32(Math.Floor(item41LongintPos));
                decimales = (item41LongintPos - Convert.ToDouble(grados)) * 60;
                minutos = Convert.ToInt32(Math.Floor(decimales));
                decimales = (decimales - Convert.ToDouble(minutos)) * 60;
                segundos = Convert.ToInt32(Math.Floor(decimales));
                decimales = (decimales - Convert.ToDouble(segundos)) * 1000;
                decimas = Convert.ToInt32(Math.Round(decimales));
                Grados = Convert.ToString(grados);
                Minutos = Convert.ToString(minutos);
                Segundos = Convert.ToString(segundos);
                Decimas = Convert.ToString(decimas);

                if (grados < 100)
                {
                    Grados = "0" + Grados;
                    if (grados < 10)
                    {
                        Grados = "0" + Grados;
                    }
                }
                if (minutos < 10)
                {
                    Minutos = "0" + Minutos;
                }
                if (segundos < 10)
                {
                    Segundos = "0" + Segundos;
                }
                if (decimas < 100)
                {
                    Decimas = "0" + Decimas;
                    if (decimas < 10)
                    {
                        Decimas = "0" + Decimas;
                    }
                }
                LONG = Grados + Minutos + Segundos + Decimas;
                this.WGS84Lat = LAT;//Latitud en WGS84. Sin signo, hay arriba una variable para el signo
                this.WGS84Long = LONG;
                //string[] WGS84Sign; //Vector con 2 posiciones. En la primera "N" ó "S"; en la segunda "E" ó "W". Ej: WGS84Sign=[N,E]
            }

            //FRN=6, Data Item I010/040: Measured Position in Polar Co-ordinates
            if (FSPEC[5])
            {
                //Rho
                lectura = BytesEnString(buffer, byteActual, 2);
                this.polarRho=Convert.ToInt32(lectura, 2); //LSB=1 m
                byteActual = byteActual + 2;
                //Theta
                lectura = BytesEnString(buffer, byteActual, 2);
                this.polarTheta=Convert.ToInt32(lectura, 2) * 0.0055;  //LSB=0.0055°
                byteActual = byteActual + 2;
            }

            //FRN=7, Data Item I010/042: Position in Cartesian Coordinates (respecto al centro de Coordenadas)
            if (FSPEC[6])
            {
                //Coordenada X
                lectura = BytesEnString(buffer, byteActual, 2);
                this.cartX=ConvertirCA2aEntero(lectura);    //LSB = 1 m
                byteActual = byteActual + 2;
                //Coordenada Y
                lectura = BytesEnString(buffer, byteActual, 2);
                this.cartY=ConvertirCA2aEntero(lectura);    //LSB = 1 m
                byteActual = byteActual + 2;
            }

            //FRN=8, Data Item I010/200, Calculated Track Velocity in Polar Co-ordinates
            if (FSPEC[7])
            {
                lectura = BytesEnString(buffer, byteActual, 2);
                this.polarTrackVelV=Convert.ToDouble(lectura) * 0.22;  //LSB=0.22 kt
                byteActual = byteActual + 2;
                lectura = BytesEnString(buffer, byteActual, 2);
                this.polarTrackVelAngle=Convert.ToDouble(lectura) * 0.0055;   //LSB=0.0055°
                byteActual = byteActual + 2;
            }

            //FRN=9, Data Item I010/202: Calculated Track Velocity in Cartesian Coord.
            if (FSPEC[8])
            {
                lectura = BytesEnString(buffer, byteActual, 2);
                this.cartTrackVelX=ConvertirCA2aEntero(lectura) * 0.25;  //LSB=0.25 m/s
                byteActual = byteActual + 2;
                lectura = BytesEnString(buffer, byteActual, 2);
                this.cartTrackVelY=ConvertirCA2aEntero(lectura) * 0.25;
                byteActual = byteActual + 2;
            }

            //FRN=10, Data Item I010/161: Track Number
            if (FSPEC[9])
            {
                lectura = BytesEnString(buffer, byteActual, 2);
                lectura = Convert.ToString(Convert.ToInt32(lectura, 2));
                while (lectura.Length < 4)
                    lectura = "0" + lectura;
                this.trackNumber= lectura;
                byteActual = byteActual + 2;
            }

            //FRN=11, Data Item I010/170: Track Status (Variable Length Data)
            if (FSPEC[10])
            {
                //Lectura de los bytes que forman el Data Item (mismo algoritmo que Target Report Descriptor)
                FX = true;
                while (FX)
                {
                    lectura = BytesEnString(buffer, byteActual, 1);
                    bit = 0;
                    while (bit < 7)
                    {
                        bit = bit + 1;
                    }
                    if (Convert.ToInt32("" + lectura[bit]) == 0)
                    {
                        FX = false;
                    }
                    byteActual = byteActual + 1;
                }
                //Some sensors are not be able to scan the whole coverage in one refresh period, 
                //therefore, track extrapolation is performed in un-scanned sectors. CST is then set to 01.
            }

            //FRN=12, Data Item I010/060: Mode-3/A Code in Octal Representation
            if (FSPEC[11])
            {
                byteActual = byteActual + 2;
            }

            //FRN=13, Data Item I020/220: Target Address
            if (FSPEC[12])
            {
                lectura = BytesEnString(buffer, byteActual, 3);
                int ICAOAdd = Convert.ToInt32(lectura, 2);
                this.ICAOAdress=Convert.ToString(ICAOAdd, 16).ToUpper();//ToUpper convierte letras minusculas en mayusculas
                byteActual = byteActual + 3;
            }

            //FRN=14, Data Item I020/245: Target Identification (CallSign)
            if (FSPEC[13])
            {
                byteActual = byteActual + 1;
                //Los seis siguientes bits siempre son 0. Call sign comienza en el segundo byte
                lectura = BytesEnString(buffer, byteActual, 6);
                //CallSign es formado por 8 caracteres de 6 bits cada uno
                //El algoritmo de decodificacion puede encontrarse en el Documento ICAO Anexo 10, Volumen IV, sección 3.1.2.9
                this.callSign=TraducirCallSign(lectura);
                byteActual = byteActual + 6;
            }

            //FRN=15, Data Item I010/250: Mode S MB Data (Repetitive Data Item)
            if (FSPEC[14])
            {
                //No Decodificado, no llegan datos en el .ast CAT010
                lectura = BytesEnString(buffer, byteActual, 1);
                int numBytes = Convert.ToInt32(lectura, 2);
                byteActual = byteActual + 1 + numBytes;
            }

            //FRN=16, Data Item I010/300: Vehicle Fleet Identification
            if (FSPEC[15])
            {
                //No Decodificado, no llegan datos en el .ast CAT010
                byteActual = byteActual + 1;
            }

            //FRN=17, Data Item I020/090: Flight Level in Binary Representation (Mode S Altitude)
            if (FSPEC[16])
            {
                lectura = BytesEnString(buffer, byteActual, 2);
                //Los dos primeros bits se almacenan en la variable "bitsVGdeFL"
                bit = 2;
                //Leemos el resto del Data Item
                string resultado = "";
                while (bit < 16)
                {
                    resultado = resultado + lectura[bit];
                    bit = bit + 1;
                }
                //Conversion de CA2 a decimal
                double result = ConvertirCA2aEntero(resultado) * 0.25;
                //Guardamos si el numero es negativo
                bool negativo = false;
                if (result < 0)
                    negativo = true;
                result = Math.Abs(result);

                resultado = Convert.ToString(Math.Round(result));
                while (resultado.Length < 4)
                    resultado = "0" + resultado;
                if (negativo)
                    resultado = "-" + resultado;
                this.FL=Convert.ToString(resultado);

                byteActual = byteActual + 2;
            }

            //FRN=18, Data Item I020/110: Measured Height (Cartesian (Local) Coordinates)
            if (FSPEC[17])
            {
                //No Decodificado, no llegan datos en el .ast CAT010
                byteActual = byteActual + 2;
            }

            //FRN=19, Data Item I010/270, Target Size & Orientation
            if (FSPEC[18])
            {
                lectura = BytesEnString(buffer, byteActual, 1);
                bit = 7;
                byteActual = byteActual + 1;
                if (Convert.ToInt32("" + lectura[bit]) == 1)
                {
                    lectura = BytesEnString(buffer, byteActual, 1);
                    bit = 7;
                    byteActual = byteActual + 1;
                    if (Convert.ToInt32("" + lectura[bit]) == 1)
                    {
                        lectura = BytesEnString(buffer, byteActual, 1);
                        bit = 7;
                        byteActual = byteActual + 1;
                    }
                }
            }

            //FRN=20, Data Item I010/550, System Status
            if (FSPEC[19])
            {
                //Decodificación no necesaria
                byteActual = byteActual + 1;
            }

            //FRN=21, Data Item I020/310: Pre-programmed Message
            if (FSPEC[20])
            {
                //No Decodificado, no llegan datos en el .ast CAT010
                byteActual = byteActual + 1;
            }

            //FRN=22, Data Item I010/500, Standard Deviation of Position
            if (FSPEC[21])
            {
                //No Decodificado, no llegan datos en el .ast CAT010
                byteActual = byteActual + 4;
            }

            //FRN=23, Data Item I010/280, Presence
            if (FSPEC[22])
            {
                ///No Decodificado, no llegan datos en el .ast CAT010
                lectura = BytesEnString(buffer, byteActual, 1);
                int numBytes = Convert.ToInt32(lectura, 2);
                byteActual = byteActual + 1 + numBytes;
            }

            //FRN=24, Data Item I010/131, Amplitude of Primary Plot
            if (FSPEC[23])
            {
                //No Decodificado, no llegan datos en el .ast CAT010
                byteActual = byteActual + 1;
            }

            //FRN=25, Data Item I010/210: Calculated Acceleration
            if (FSPEC[24])
            {
                lectura = BytesEnString(buffer, byteActual, 1);
                this.accX = ConvertirCA2aEntero(lectura) * 0.25;  //LSB=0.25 m/s^2
                byteActual = byteActual + 1;
                lectura = BytesEnString(buffer, byteActual, 1);
                this.accY = ConvertirCA2aEntero(lectura) * 0.25;
                byteActual = byteActual + 1;
            }

            //FRN=27, Data Item SP: Special Purpose Field
            if (FSPEC[26])
            {
                //No Decodificado, no llegan datos en el .ast CAT010
                //El primer byte indica la Length de SP, incluido este primer byte
                lectura = BytesEnString(buffer, byteActual, 1);
                int numBytes = Convert.ToInt16(lectura);
                byteActual = byteActual + numBytes;
            }

            //FRN=28, Data Item RE: Reserved Expansion Field
            if (FSPEC[27])
            {
                //No Decodificado, no llegan datos en el .ast CAT010
                //El primer byte indica la Length de RE, incluido este primer byte
                lectura = BytesEnString(buffer, byteActual, 1);
                int numBytes = Convert.ToInt16(lectura);
                byteActual = byteActual + numBytes;
            }
        }

        public void DecodificarCAT20(FileStream fs, byte[] buffer, int byteActual)
        {
            //Leemos la Length del mensaje
            fs.Read(buffer, byteActual, 2);
            int LEN = Convert.ToInt32(BytesEnString(buffer, 1, 2), 2);
            byteActual = byteActual + 2;

            //Guardamos el resto del mensaje en buffer
            fs.Read(buffer, byteActual, LEN - 3);

            //Leemos FSPEC
            string lectura;             //Guarda el byte actual de FSPEC en un string
            int bit;                        //Contador del bit actual que se está leyendo
            bool[] FSPEC = new bool[28];    //el vector indica qué Data Items se han enviado en el mensaje
            int FRN = 0;                    //Indica la posicion actual de FSPEC

            bool FX = true; //Cuando se lee FSPEC, si el último bit es 1 se sigue leyendo el siguiente byte. Si no, acaba FSPEC.
            while (FX)
            {
                lectura = BytesEnString(buffer, byteActual, 1);

                //Leemos los bits y guardamos qué Data Items se han enviado
                bit = 0;
                while (bit < 7)
                {
                    if (Convert.ToInt32("" + lectura[bit]) == 1)
                        FSPEC[FRN] = true;
                    if (Convert.ToInt32("" + lectura[bit]) == 0)
                        FSPEC[FRN] = false;

                    bit = bit + 1;
                    FRN = FRN + 1;
                }
                //leemos último bit
                if (Convert.ToInt32("" + lectura[bit]) == 0)
                    FX = false;

                byteActual = byteActual + 1;
            }

            //Lectura de cada uno de los Data Fields

            //FRN=1, Data Item I020/010: Data Source Identifier
            if (FSPEC[0])
            {
                //SAC
                lectura = Convert.ToString(buffer[byteActual], 10);
                while (lectura.Length < 3)
                    lectura = "0" + lectura;
                this.SAC = lectura;
                byteActual++;

                //SIC
                lectura = Convert.ToString(buffer[byteActual], 10);
                while (lectura.Length < 3)
                    lectura = "0" + lectura;
                this.SIC = lectura;
                byteActual++;
            }

            //FRN=2, Data Item I020/020: Target Report Descriptor (Variable Length Data)
            if (FSPEC[1])
            {
                //Lectura de los bytes que forman el Data Item (mismo algoritmo que FSPEC)
                FX = true;
                while (FX)
                {
                    lectura = BytesEnString(buffer, byteActual, 1);
                    bit = 0;
                    while (bit < 7)
                    {
                        bit = bit + 1;
                    }
                    if (Convert.ToInt32("" + lectura[bit]) == 0)
                    {
                        FX = false;
                    }
                    byteActual = byteActual + 1;
                }
            }

            //FRN=3, Data Item I020/140: Time of Day
            if (FSPEC[2])
            {
                lectura = BytesEnString(buffer, byteActual, 3);
                //Conversión de hora UTC a minutos y decimas de minuto
                //El LSB vale 1/128 segundos. Dividiendo el valor entre 128 obtenemos segundos
                int lecturaUTC = Convert.ToInt32(lectura, 2);       //Conversion a decimal entero
                this.UTC = Convert.ToDouble(lecturaUTC) / (60 * 128);  //Al dividir se obtienen decimales, pero eso conversion a double

                byteActual = byteActual + 3;
            }

            //FRN=4, Data Item I020/041: Position in WGS-84 Coordinates
            if (FSPEC[3])
            {
                //Decodificación no requerida
                byteActual = byteActual + 8;
            }

            //FRN=5, Data Item I020/042: Position in Cartesian Coordinates (respecto al centro de Coordenadas)
            if (FSPEC[4])
            {
                //Coordenada X
                lectura = BytesEnString(buffer, byteActual, 3);
                this.cartX = ConvertirCA2aEntero(lectura) * 0.5;    //LSB = 0.5 m
                byteActual = byteActual + 3;
                //Coordenada Y
                lectura = BytesEnString(buffer, byteActual, 3);
                this.cartY = ConvertirCA2aEntero(lectura) * 0.5;    //LSB = 0.5 m
                byteActual = byteActual + 3;
            }

            //FRN=6, Data Item I020/161: Track Number
            if (FSPEC[5])
            {
                lectura = BytesEnString(buffer, byteActual, 2);
                lectura = Convert.ToString(Convert.ToInt32(lectura, 2));
                while (lectura.Length < 4)
                    lectura = "0" + lectura;
                this.trackNumber = lectura;
                byteActual = byteActual + 2;
            }

            //FRN=7, Data Item I020/170: Track Status (Variable Length Data)
            if (FSPEC[6])
            {
                //Lectura de los bytes que forman el Data Item (mismo algoritmo que Target Report Descriptor)
                FX = true;
                while (FX)
                {
                    lectura = BytesEnString(buffer, byteActual, 1);
                    bit = 7;
                    if (Convert.ToInt32("" + lectura[bit]) == 0)
                    {
                        FX = false;
                    }
                    byteActual = byteActual + 1;
                }
            }

            //FRN=8, Data Item I020/070: Mode-3/A Code in Octal Representation
            if (FSPEC[7])
            {
                byteActual = byteActual + 2;
            }

            //FRN=9, Data Item I020/202: Calculated Track Velocity in Cartesian Coord.
            if (FSPEC[8])
            {
                lectura = BytesEnString(buffer, byteActual, 2);
                this.cartTrackVelX = ConvertirCA2aEntero(lectura) * 0.25;
                byteActual = byteActual + 2;
                lectura = BytesEnString(buffer, byteActual, 2);
                this.cartTrackVelY = ConvertirCA2aEntero(lectura) * 0.25;
                byteActual = byteActual + 2;
            }

            //FRN=10, Data Item I020/090: Flight Level in Binary Representation (Mode S Altitude)
            if (FSPEC[9])
            {
                lectura = BytesEnString(buffer, byteActual, 2);
                //Los dos primeros bits se almacenan en la variable "bitsVGdeFL"
                bit = 2;
                //Leemos el resto del Data Item
                string resultado = "";
                while (bit < 16)
                {
                    resultado = resultado + lectura[bit];
                    bit = bit + 1;
                }
                //Conversion de CA2 a decimal
                double result = ConvertirCA2aEntero(resultado) * 0.25;
                //Guardamos si el numero es negativo
                bool negativo = false;
                if (result < 0)
                    negativo = true;
                result = Math.Abs(result);

                resultado = Convert.ToString(Math.Round(result));
                while (resultado.Length < 4)
                    resultado = "0" + resultado;
                if (negativo)
                    resultado = "-" + resultado;
                this.FL = Convert.ToString(resultado);

                byteActual = byteActual + 2;
            }

            //FRN=11, Data Item I020/100: Mode-C Code
            if (FSPEC[10])
            {
                //Decodificación no requerida
                byteActual = byteActual + 4;
            }

            //FRN=12, Data Item I020/220: Target Address
            if (FSPEC[11])
            {
                lectura = BytesEnString(buffer, byteActual, 3);
                int ICAOAdd = Convert.ToInt32(lectura, 2);
                this.ICAOAdress = Convert.ToString(ICAOAdd, 16).ToUpper();
                byteActual = byteActual + 3;
            }

            //FRN=13, Data Item I020/245: Target Identification (CallSign)
            if (FSPEC[12])
            {
                byteActual = byteActual + 1;
                //Los seis siguientes bits siempre son 0. Call sign comienza en el segundo byte
                lectura = BytesEnString(buffer, byteActual, 6);
                //CallSign es formado por 8 caracteres de 6 bits cada uno
                //El algoritmo de decodificacion puede encontrarse en el Documento ICAO Anexo 10, Volumen IV, sección 3.1.2.9
                this.callSign = TraducirCallSign(lectura);
                byteActual = byteActual + 6;
            }

            //FRN=14, Data Item I020/110: Measured Height (Cartesian (Local) Coordinates)
            if (FSPEC[13])
            {
                //Decodificación no requerida
                byteActual = byteActual + 2;
            }

            //FRN=15, Data Item I020/105: Geometric Height (WGS-84)
            if (FSPEC[14])
            {
                //Decodificación no requerida
                byteActual = byteActual + 2;
            }

            //FRN=16, Data Item I020/210: Calculated Acceleration
            if (FSPEC[15])
            {
                //Decodificación no requerida
                byteActual = byteActual + 2;
            }

            //FRN=17, Data Item I020/300: Vehicle Fleet Identification
            if (FSPEC[16])
            {
                //Decodificación no requerida
                byteActual = byteActual + 1;
            }

            //FRN=18, Data Item I020/310: Pre-programmed Message
            if (FSPEC[17])
            {
                //Decodificación no requerida
                byteActual = byteActual + 1;
            }

            //FRN=19, Data Item I020/500: Position Accuracy (DOP) (Compound Data Item)
            if (FSPEC[18])
            {
                //Primary Subfield: el primer byte indica el contenido de los siguientes bytes
                //(En principio sólo los tres primeros bits indican el contenido, los otros 5 deberían ser "spare bits set to zero")
                bool[] primarySub = new bool[8];

                lectura = BytesEnString(buffer, byteActual, 1);
                bit = 0;
                while (bit < 8)
                {
                    if (Convert.ToInt16("" + lectura[bit]) == 1)
                        primarySub[bit] = true;
                    if (Convert.ToInt16("" + lectura[bit]) == 0)
                        primarySub[bit] = false;
                    bit++;
                }

                byteActual++;

                //Subfield #1 (bit DOP true)
                if (primarySub[0])
                {
                    byteActual = byteActual + 2;
                }

                //Subfield #2 (bit SDP true)
                if (primarySub[1])
                {
                    byteActual = byteActual + 2;
                }

                //Subfield #3 (bit SDH true)
                if (primarySub[2])
                {
                    byteActual = byteActual + 2;
                }
            }

            //FRN=20, Data Item I020/400: Contributing Devices (Repetitive Data Item)
            if (FSPEC[19])
            {
                //El primer byte indica el numero de bytes a leer después de este
                lectura = BytesEnString(buffer, byteActual, 1);
                byteActual++;
                int numBytes = Convert.ToInt32(lectura, 2);
                if (numBytes != 0)
                {
                    byteActual = byteActual + numBytes;
                }
            }

            //FRN=21, Data Item I020/250: Mode S MB Data (Repetitive Data Item)
            if (FSPEC[20])
            {
                //Decodificación no requerida
                lectura = BytesEnString(buffer, byteActual, 1);
                int numBytes = Convert.ToInt32(lectura, 2);
                byteActual = byteActual + 1 + numBytes;
            }

            //FRN=22, Data Item I020/230: Comms/ACAS Capability and Flight Status (Two-octet fixed data length)
            if (FSPEC[21])
            {
                //Decodificación no requerida
                byteActual = byteActual + 2;
            }

            //FRN=23, Data Item I020/260: ACAS Resolution Advisory Report (Seven-octet fixed data length)
            if (FSPEC[22])
            {
                //Decodificación no requerida
                byteActual = byteActual + 7;
            }

            //FRN=24, Data Item I020/030: Warning/Error Conditions (Variable length Data Item)
            if (FSPEC[23])
            {
                //Decodificación no requerida
                FX = true;
                while (FX)
                {
                    lectura = BytesEnString(buffer, byteActual, 1);
                    if (Convert.ToInt16("" + lectura[7]) == 0)
                        FX = false;
                    byteActual++;
                }
            }

            //FRN=25, Data Item I020/055: Mode-1 Code in Octal Representation (1 byte)
            if (FSPEC[24])
            {
                //Decodificación no requerida
                byteActual++;
            }

            //FRN=26, Data Item I020/050: Mode-2 Code in Octal Representation (2 bytes)
            if (FSPEC[25])
            {
                //Decodificación no requerida
                byteActual = byteActual + 2;
            }

            //FRN=27, Data Item RE: Reserved Expansion Field
            if (FSPEC[26])
            {
                //El primer byte indica la Length de RE, incluido este primer byte
                lectura = BytesEnString(buffer, byteActual, 1);
                int numBytes = Convert.ToInt16(lectura) - 1;//restamos 1 para no contar el de LEN
                byteActual = byteActual + 1;

                if (numBytes > 0)
                {
                    bool[] subFSPEC = new bool[8];
                    lectura = BytesEnString(buffer, byteActual, 1);
                    bit = 0;
                    int indice = 0;
                    while (bit < 8)
                    {
                        if (Convert.ToInt32("" + lectura[bit]) == 1)
                            subFSPEC[indice] = true;
                        if (Convert.ToInt32("" + lectura[bit]) == 0)
                            subFSPEC[indice] = false;

                        bit = bit + 1;
                        indice = indice + 1;
                    }
                    byteActual = byteActual + 1;

                    //PA, Position Accuracy
                    //Si llega, la tabla PA substituye la tabla que se lee en el item PA
                    if (subFSPEC[0])
                    {
                        bool[] primarySub = new bool[8];
                        lectura = BytesEnString(buffer, byteActual, 1);
                        bit = 0;
                        while (bit < 8)
                        {
                            if (Convert.ToInt16("" + lectura[bit]) == 1)
                                primarySub[bit] = true;
                            if (Convert.ToInt16("" + lectura[bit]) == 0)
                                primarySub[bit] = false;
                            bit++;
                        }
                        byteActual = byteActual + 1;

                        //Subfield #1: DOP of Position
                        if (primarySub[0])
                        {
                            //DOPx
                            byteActual = byteActual + 2;
                            //DOPy
                            byteActual = byteActual + 2;
                            //DOPxy
                            //Si DOPx o DOPy valen 0, entonces DOPxy vale 0
                            byteActual = byteActual + 2;
                        }

                        //Subfield #2 Standard Deviation of Position (Cartesian)
                        if (primarySub[1])
                        {
                            //Standard Deviation of X component
                            byteActual = byteActual + 2;
                            //Standard Deviation of Y component
                            byteActual = byteActual + 2;
                            //Coeficiente de Correlación, en Complemento a 2
                            //Si stDevX o stDevY valen 0, entonces el coef. de correlacion vale 0
                            byteActual = byteActual + 2;
                        }

                        //Subfield #3 Standard Deviation of Geometric Height 
                        if (primarySub[2])
                        {
                            //Standard Deviation of Height
                            byteActual = byteActual + 2;
                        }

                        //Subfield #4 Standard Deviation of Position (WGS-84)
                        if (primarySub[3])
                        {
                            //No decodificamos
                            byteActual = byteActual + 6;
                        }
                        //El resto son spare
                    }
                    if (subFSPEC[1])
                    {
                        //No decodificamos
                        byteActual = byteActual + 4;
                    }
                    if (subFSPEC[2])
                    {
                        //No decodificamos
                        byteActual = byteActual + 2;
                    }
                    if (subFSPEC[3])
                    {
                        //No decodificamos
                        byteActual = byteActual + 3;
                    }
                    if (subFSPEC[4])
                    {
                        //No decodificamos
                        lectura = BytesEnString(buffer, byteActual, 1);
                        bool[] primerSub = new bool[8];
                        bool[] segundoSub = new bool[8];
                        bool[] tercerSub = new bool[3];
                        bit = 0;
                        while (bit < 8)
                        {
                            if (Convert.ToInt16("" + lectura[bit]) == 1)
                                primerSub[bit] = true;
                            if (Convert.ToInt16("" + lectura[bit]) == 0)
                                primerSub[bit] = false;
                            bit++;
                        }
                        byteActual = byteActual + 1;

                        if (primerSub[7])
                        {
                            lectura = BytesEnString(buffer, byteActual, 1);
                            bit = 0;
                            while (bit < 8)
                            {
                                if (Convert.ToInt16("" + lectura[bit]) == 1)
                                    segundoSub[bit] = true;
                                if (Convert.ToInt16("" + lectura[bit]) == 0)
                                    segundoSub[bit] = false;
                                bit++;
                            }
                            byteActual = byteActual + 1;

                            if (segundoSub[7])
                            {
                                lectura = BytesEnString(buffer, byteActual, 1);
                                bit = 0;
                                while (bit < 3)
                                {
                                    if (Convert.ToInt16("" + lectura[bit]) == 1)
                                        tercerSub[bit] = true;
                                    if (Convert.ToInt16("" + lectura[bit]) == 0)
                                        tercerSub[bit] = false;
                                    bit++;
                                }
                                byteActual = byteActual + 1;
                            }
                        }
                        if (primerSub[0])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (primerSub[1])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (primerSub[2])
                        {
                            byteActual = byteActual + 3;
                        }
                        if (primerSub[3])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (primerSub[4])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (primerSub[5])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (primerSub[6])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (segundoSub[0])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (segundoSub[1])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (segundoSub[2])
                        {
                            byteActual = byteActual + 3;
                        }
                        if (segundoSub[3])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (segundoSub[4])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (segundoSub[5])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (segundoSub[6])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (tercerSub[0])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (tercerSub[1])
                        {
                            byteActual = byteActual + 1;
                        }
                        if (tercerSub[2])
                        {
                            byteActual = byteActual + 3;
                        }
                    }
                    //El resto son spare
                }
            }

            //FRN=28, Data Item SP: Special Purpose Field
            if (FSPEC[27])
            {
                //Decodificación no requerida
                //El primer byte indica la Length de SP, incluido este primer byte
                lectura = BytesEnString(buffer, byteActual, 1);
                int numBytes = Convert.ToInt16(lectura);
                byteActual = byteActual + numBytes;
            }
        }

        public void DecodificarDGPS(string linea, string Icao) //Lee el .txt de D-GPS y le asigna la misma estructura que al fichero .ast
        {
            this.CAT = 10;
            this.SIC = "107";  //Predeterminado: origen MLAT, ICAO Address introducido por usuario para todos los paquetes, datos centrados (referenciados) en MLAT.
            this.SAC = "000";
            this.ICAOAdress = Icao;

            string[] entradas = linea.Split('\t');
            string[] entradasTime = entradas[5].Split(' ');
            if (entradasTime[0] == "")
            {
                if (entradasTime[3] == "")
                {
                    entradasTime[0] = entradasTime[1];
                    entradasTime[1] = entradasTime[2];
                    entradasTime[2] = entradasTime[4];
                }
                else
                {
                    entradasTime[0] = entradasTime[1];
                    entradasTime[1] = entradasTime[2];
                    entradasTime[2] = entradasTime[3];
                }
            }
            else if (entradasTime[2] == "")
            {
                entradasTime[2] = entradasTime[3];
            }
            double a = Convert.ToDouble(entradasTime[2]);
            this.UTC = (Convert.ToDouble(entradasTime[0]) * 60) + Convert.ToDouble(entradasTime[1]) + (Convert.ToDouble(entradasTime[2]) / (60 * 100));//Pasamos a minutos y decimas de minuto
            string[] Mlat = new string[] { "411749426N", "0020442410E" };//Coordenadas del mlat
            double[] cartMlat = new double[2];
            double[] posicion = new double[2];
            cartMlat[0] = ConvertWGStoRad(Mlat[0], 0);//Mlat en radianes
            cartMlat[1] = ConvertWGStoRad(Mlat[1], 1);
            posicion = ConvertRadtoXY(cartMlat, Convert.ToDouble(entradas[1]) * Math.PI / (180 * Math.Pow(10, 10)), Convert.ToDouble(entradas[2]) * Math.PI / (180 * Math.Pow(10, 10)));// /Math.Pow(10, 10) porque no transforma bien el . decimal
            this.cartX = posicion[0];//Pasamos WGS a XY
            this.cartY = posicion[1];
        }

        //Subfunciones
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

        public double ConvertWGStoRad(string coordenada, int orientacion)//Si es lat, orientacion=0; Si es long, orientacion=1
        {
            //Retorna una coordenada en radianes //El imput es sin decimas de segundo
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
                if (Convert.ToString(coordenada[10]) == "W")
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

        public void ConvertCartFromPolar() //int[x,y] //No se usa
        {
            //CAT10 usa SMR para datos polares, por lo que esta centrado en SMR. Le sumamos //SMR y le restamos MLAT para referenciarlo a MLAT. Theta esta referencia en el norte
            double Rt = 6378000;//m Radio ecuatorial
            string[] Mlat = new string[] { "411749426N", "0020442410E" }; //Coordenadas del MLAT y SMR
            string[] SMR = new string[] { "411744226N", "0020542411E" };
            double[] cartMlat = new double[2];
            double[] cartSMR = new double[2];
            cartMlat[0] = Rt * ConvertWGStoRad(Mlat[1], 1);
            cartMlat[1] = Rt * ConvertWGStoRad(Mlat[0], 0);
            cartSMR[0] = Rt * ConvertWGStoRad(SMR[1], 1);
            cartSMR[1] = Rt * ConvertWGStoRad(SMR[0], 0);
            if ((this.polarRho != Math.Pow(10, 8)) && (this.polarTheta != Math.Pow(10, 8)))
            {
                this.cartFromPolarX = Convert.ToInt32(this.polarRho * Math.Sin(this.polarTheta) + cartSMR[0] - cartMlat[0]);
                this.cartFromPolarY = Convert.ToInt32(this.polarRho * Math.Cos(this.polarTheta) + cartSMR[1] - cartMlat[1]);
            }
        }

        public string ConvertUTC(string decimas) //Pasa UTC de minutos y decimas de minuto a un string en HH:mm::ss.sss
        {
            //Si decimas="SinDecimas" retorna formato hh:mm:ss; Sino dejar decimas="".
            double minutes = this.UTC;
            string resultado;
            if (minutes != -1)
            {
                double hours = Math.Floor(minutes / 60);
                double roundMinutes = Math.Floor(minutes - hours * 60);
                double seconds;
                if (decimas == "")
                    seconds = Math.Round((minutes - (hours * 60) - roundMinutes) * 60, 3);//Permitimos 3 decimas de segundo
                else
                    seconds = Math.Round((minutes - (hours * 60) - roundMinutes) * 60);
                string finalHours = Convert.ToString(hours);
                string finalMinutes = Convert.ToString(roundMinutes);
                string finalSeconds=Convert.ToString(seconds);    
                if (hours < 10)
                    finalHours = "0" + finalHours;
                if (roundMinutes < 10)
                    finalMinutes = "0" + finalMinutes;
                if (seconds < 10)
                    finalSeconds = "0" + finalSeconds;
                string prefinal = finalHours + ":" + finalMinutes + ":" + finalSeconds;
                if (decimas == "")
                {
                    if (prefinal.Length == 8)
                    {
                        resultado = prefinal + ",000";
                    }
                    else if (prefinal.Length == 10)
                    {
                        resultado = prefinal + "00";
                    }
                    else if (prefinal.Length == 11)
                    {
                        resultado = prefinal + "0";
                    }
                    else resultado = prefinal;
                }
                else resultado = prefinal;
            }
            else resultado = "No Data";

            return resultado;
        }

        public double[] GetPosicion() //Miramos si los datos de posicion del paquete vienen en WGS, del SMR, o en coordenadas cartesianas del MLAT
        {
            double[] posicion = new double[2];//Indice 0=X; indice 1=Y
            if ((GetCartFromWGS84()[0] != Math.Pow(10, 8)) && (GetCartFromWGS84()[1] != Math.Pow(10, 8)))
            {
                posicion[0] = GetCartFromWGS84()[0];
                posicion[1] = GetCartFromWGS84()[1];
            }
            else if ((GetMLATfromSMRX() != Math.Pow(10, 8)) && (GetMLATfromSMRY() != Math.Pow(10, 8)))
            {
                posicion[0] = GetMLATfromSMRX();
                posicion[1] = GetMLATfromSMRY();
            }
            else
            {
                posicion[0] = GetCartX();
                posicion[1] = GetCartY();
            }
            return (posicion);
        }

        //Funciones GET

        public int GetCAT()
        {
            return (this.CAT);
        }
        public string GetSIC()
        {
            return (this.SIC);
        }
        public string GetSAC()
        {
            return (this.SAC);
        }
        public double GetUTC()
        {
            return (this.UTC);
        }
        public string GetTrackNumber()
        {
            return (this.trackNumber);
        }
        public string GetFL()
        {
            return (this.FL);
        }
        public string GetCallSign()
        {
            return (this.callSign);
        }
        public string GetICAOAdress()
        {
            return (this.ICAOAdress);
        }
        public double GetCartX()
        {
            return (this.cartX);
        }
        public double GetCartY()
        {
            return (this.cartY);
        }
        public double GetPolarRho()
        {
            return (this.polarRho);
        }
        public double GetPolarTheta()
        {
            return (this.polarTheta);
        }
        public double GetCartFromPolarX()
        {
            return (this.cartFromPolarX);
        }
        public double GetCartFromPolarY()
        {
            return (this.cartFromPolarY);
        }
        public string GetWGS84Lat()
        {
            //Retornamos todo menos las decimas. Lat 41 51 23 111
            if (this.WGS84Lat != null)
            {
                return (Convert.ToString(this.WGS84Lat[0]) + Convert.ToString(this.WGS84Lat[1]) + Convert.ToString(this.WGS84Lat[2]) + Convert.ToString(this.WGS84Lat[3]) +
                    Convert.ToString(this.WGS84Lat[4]) + Convert.ToString(this.WGS84Lat[5]));
            }
            else return (this.WGS84Lat);
        }
        public string GetWGS84Long()
        {
            //Retornamos todo menos las decimas. Lat 041 51 23 111
            if (this.WGS84Long != null)
            {
                return (Convert.ToString(this.WGS84Long[0]) + Convert.ToString(this.WGS84Long[1]) + Convert.ToString(this.WGS84Long[2]) + Convert.ToString(this.WGS84Long[3]) +
                    Convert.ToString(this.WGS84Long[4]) + Convert.ToString(this.WGS84Long[5]) + Convert.ToString(this.WGS84Long[6]));
            }
            else return (this.WGS84Long);
        }
        public string GetWGS84Sign(int componente)
        {
            return (this.WGS84Sign[componente]);
        }
        public double GetCartTrackVelX()
        {
            return (this.cartTrackVelX);
        }
        public double GetCartTrackVelY()
        {
            return (this.cartTrackVelY);
        }
        public double GetPolarTrackVelV()
        {
            return (this.polarTrackVelV);
        }
        public double GetPolarTrackVelAngle()
        {
            return (this.polarTrackVelAngle);
        }
        public double GetMLATfromSMRX()
        {
            return (this.MLATfromSMRX);
        }
        public double GetMLATfromSMRY()
        {
            return (this.MLATfromSMRY);
        }
        public double GetAccX()
        {
            return (this.accX);
        }
        public double GetAccY()
        {
            return (this.accY);
        }
        public int GetZona()
        {
            return (this.zona);
        }
        public int GetIndiceDGPS()
        {
            return (this.indiceDGPS);
        }
        public double GetUTCcorregido()
        {
            return (this.UTCcorregido);
        }

        
        //Funciones SET
        public void SetZona(int Zona)
        {
            this.zona = Zona;
        }
        public void SetIndiceDGPS(int indice)
        {
            this.indiceDGPS = indice;
        }
        public void SetUTCcorregido(double UTCc)
        {
            this.UTCcorregido = UTCc;
        }

        public void SetVelocidad(double Vx, double Vy)
        {
            this.cartTrackVelX = Vx; //Cartesian track velocity (X component).
            this.cartTrackVelY = Vy;
        }
    }
}

