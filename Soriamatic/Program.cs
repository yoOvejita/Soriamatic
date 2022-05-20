using System;
using Academico;

namespace Soriamatic
{
    internal class Program
    {
        public static int[] memoria = new int[100];
        public static int linea = 0;
        public static int acumulador = 0;
        public static int instruccionActual, codOperacion, operando;
        const int LEER = 10, ESCRIBIR = 11;
        const int CARGAR = 20, ALMACENAR = 21;
        const int SUMAR = 30, RESTAR = 31, MULTIPLICAR = 32, DIVIDIR = 33;
        const int SALTAR = 40, SALTARNEG = 41, SALTARCERO = 42, ALTO = 43;
        static void Main(string[] args)
        {
           /* Ejemplo de suma de dos numeros
            * 0 ? 1007 Leer n1
            * 1 ? 1008 Leer n2
            * 2 ? 2007 Carar al acumulador n1
            * 3 ? 3008 Sumar n2 + acumulador -> acumulador
            * 4 ? 2109 Almacena lo del acumulador en la posición 09
            * 5 ? 1109 Muestra lo que está en 09
            * 6 ? 4300 Alto
            * 7 ? 0000
            * 8 ? -9999
            */

            /* Ejemplo demostrar el mayor de 2 numeros
             * 0 ? 1010 Leer n1
             * 1 ? 1011 Leer n2
             * 2 ? 2010 Cargar al acumulador n1
             * 3 ? 3111 Restamos
             * 4 ? 4108 si es neg -> n2 es mayor
             * 5 ? 1110
             * 6 ? 4300
             * 7 ? 0000
             * 8 ? 1111
             * 9 ? 4300
             * 10? 0000
             * 11? 0000
             */

            // CARGAR PROGRAMA
            while (linea < memoria.Length){
                Console.Write(linea + " ? ");
                instruccionActual = int.Parse(Console.ReadLine());
                if (instruccionActual == -9999)
                    break;
                memoria[linea++] = instruccionActual;
            }

            // EJECUTAR PROGRAMA
            linea = 0;
            while (linea >= 0)
            {
                instruccionActual = memoria[linea++];
                selectorInstruccion(instruccionActual);
            }

            Estudiante est = new Estudiante { edad = 20};
            Estudiante est2 = new Estudiante("pepe", "perales",60);
            est.hablar();
            Academico.Enseñanza.Docente.reñir();

            Academico.Enseñanza.Docente doc = new Academico.Enseñanza.Docente(); 
            Console.WriteLine(est.edad);
        }

        public static void selectorInstruccion(int regActual)
        {
            codOperacion = regActual / 100;// 1012
            operando = regActual % 100;
            switch (codOperacion)
            {
                case LEER:
                    Console.Write("Ingrese un numero entero: ");
                    memoria[operando] = int.Parse(Console.ReadLine());
                    break;
                case ESCRIBIR:
                    Console.WriteLine("> " + memoria[operando]);
                    break;
                case CARGAR:
                    acumulador = memoria[operando];
                    break;
                case ALMACENAR:
                    memoria[operando] = acumulador;
                    break;
                case SUMAR:
                    acumulador += memoria[operando];
                    break;
                case RESTAR:
                    acumulador -= memoria[operando];
                    break;
                case MULTIPLICAR:
                    acumulador *= memoria[operando];
                    break;
                case DIVIDIR:
                    if (memoria[operando] != 0)
                        acumulador /= memoria[operando];
                    else {
                        linea = -1;
                        Console.WriteLine("Error: division por 0");
                    }
                        
                    break;
                case SALTAR:
                    linea = operando;
                    break;
                case SALTARNEG:
                    if(acumulador < 0)
                        linea = operando;
                    break;
                case SALTARCERO:
                    if(acumulador == 0)
                        linea = operando;
                    break;
                case ALTO:
                    linea = -1;
                    Console.WriteLine("Ejecución concluida");
                    break;
            }
        }
    }
}
