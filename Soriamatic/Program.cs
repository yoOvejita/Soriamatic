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
            /*
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
            */

            Conversor cc = new Conversor("3 + ( 4 * 5 )");
            cc.convertir();
            Console.WriteLine(cc.evaluarExpresion());


            /*
             Les propongo mejorar nuestra maquina con lo siguiente:
            1. Mayor memoria, ya no 100 casillas.
            2. Agregar instrucción adicional para calcular el resto (mod %)
            3. Agregar instrucción adicional para calcular potencia
            4. Cambiar la codificación de líneas para que maneje hexadecimales en lugar de numeros base 10
            5. Agregar instrucción adicional para imprimir nueva línea.
            6. Agregar posibilidad de cálculo de flotantes además de enteros.
            7. Agregar posibilidad de leer cadenas: necesita una nueva instrucción.
                En el primer espacio de almacenamiento la palabra indicará la longitud de la cadena en el operador y a continuación
                la segunda mitad de cada palabra será la letra
                será facil con conversiones a hexadecimal y con que soporta hexa nuestra maquina.
                Ej. Digamos que la instrucción 60 es LEER_CADENA, entonces 6021 indicaría que la cadena estará en la casilla 21
                Entonces, estando en la casilla 21 habrá por ejemplo:
                21 ? 0448
                22 ? 044F
                23 ? 044C
                24 ? 0441

                Donde 48, 4F, 4C y 41 son H, O, L y A respectivamente... siendo la palabra "HOLA"
            8. Agregar instrucción adicional para mostrar cadenas (las realizadas en el punto anterior)
                Apuntará a una dirección de memoria donde se sabe que está la primera palabra con la longitud de la cadena
                Se leran las siguientes casillas de memoria hasta completar la longitud de cadena y se convertiran
                a char y entonces a cadena para finalmente mostrarlo.
             */
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
