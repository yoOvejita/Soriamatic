using System;
using System.Collections.Generic;
using System.Text;

namespace Soriamatic
{
    internal class PCC
    {
        private IDictionary<int, string> programaCompilado;// acá almacena el prog SLM[]
        private IDictionary<Tuple<int, char>, int> tablaDeSimbolos;//se guardan datos del código PEPE++
        private int[] flags;//Determinador de segunda pasada
        private List<string> programaPEPE; //El programa rescatado del archivo *.pp
        public PCC() {
            programaCompilado = new Dictionary<int, string>();
            tablaDeSimbolos = new Dictionary<Tuple<int, char>, int>();
            flags = new int[100];
            for (int i = 0; i < flags.Length; i++)
                flags[i] = -1;
            programaPEPE = new List<string>();
        }

        //Debemos definir un método que reciba el archivo de texto y de este extraiga la lista de strings
        //Debemos definir un método que genere la tablaDeSimbolos (primera pasada) y que llene el arreglo de flags
        //Debemos definir un método que realice la segunda pasada
        //Debemos definir un método que devuelva el archivo compilado


        //Debemos definir un método que reciba el archivo de texto y de este extraiga la lista de strings
        public void recibeTransformaArchivo()
        {
            //ocurre mágia
            programaPEPE.Add("10 rem calcula el doble de un lote");
            programaPEPE.Add("20 input n");
            programaPEPE.Add("30 rem evaluamos si es flag");
            programaPEPE.Add("40 if n == -9999 goto 99");
            programaPEPE.Add("50 rem calculamos el doble y guardamos en r");
            programaPEPE.Add("60 let r = n * 2");
            programaPEPE.Add("70 print r");
            programaPEPE.Add("80 rem aca iteramos para obtener siguiente numero");
            programaPEPE.Add("90 goto 20");
            programaPEPE.Add("99 end");
        }

        //Debemos definir un método que genere la tablaDeSimbolos (primera pasada) y que llene el arreglo de flags
        public void compilar()
        {
            int posicionDisponibleInstruccion = 0;//Con esto controla la linea actual en el SLM[]
            int posicionDisponibleValor = flags.Length - 1;//si el tamaño varia, solo se modifica en constructor

            foreach (string linea in programaPEPE)
            {
                string[] tokens = linea.Split(' ');
                //primero llenamos la tabla de simbolos
                #region llenado de tablaDeSimbolos
                tablaDeSimbolos.Add(Tuple.Create(int.Parse(tokens[0]), 'l'), posicionDisponibleInstruccion);
                switch (tokens[1])
                {
                    case "rem":
                        //nada
                        break;
                    case "input":
                        //1. es sencillo, solo verificar si ya existe la var que viene a continuacion
                        int numerico = tokens[2][0];
                        if(tablaDeSimbolos.TryGetValue(Tuple.Create(numerico, 'v'), out int posicionEnSLM))
                        {//si ya existe
                            programaCompilado.Add(posicionDisponibleInstruccion++, (1000 + posicionEnSLM)+"");
                        }
                        else
                        {//si no existe
                            tablaDeSimbolos.Add(Tuple.Create(numerico, 'v'), posicionDisponibleValor);
                            //en el prog SLM[] agregamos la instruccion de input y (opc) reservamos la pos mem de var
                            programaCompilado.Add(posicionDisponibleInstruccion++, (1000 + posicionDisponibleValor) + "");
                            programaCompilado.Add(posicionDisponibleValor--, "0000");
                        }
                        break;
                    case "print":
                        //es sencillo: primero buscamos el valor
                        numerico = tokens[2][0];
                        if (tablaDeSimbolos.TryGetValue(Tuple.Create(numerico, 'v'), out posicionEnSLM))
                        {//si ya existe
                            programaCompilado.Add(posicionDisponibleInstruccion++, (1000 + posicionEnSLM) + "");
                        }
                        else
                        {//si no existe
                            tablaDeSimbolos.Add(Tuple.Create(numerico, 'v'), posicionDisponibleValor);
                            //en el prog SLM[] agregamos la instruccion de input y (opc) reservamos la pos mem de var
                            programaCompilado.Add(posicionDisponibleInstruccion++, (1000 + posicionDisponibleValor) + "");
                            programaCompilado.Add(posicionDisponibleValor--, "0000");
                        }
                        break;
                    case "end":
                        //es sencillo: solo se pone la señal de fin
                        programaCompilado.Add(posicionDisponibleInstruccion++, "4300");
                        break;
                    case "goto":
                        //no es tan sencillo pero ahi va
                        //verificar si la linea a la que pretende ir existe
                        numerico = int.Parse(tokens[2]);
                        if (tablaDeSimbolos.TryGetValue(Tuple.Create(numerico, 'l'), out posicionEnSLM))
                        {//si ya existe: es facil
                            programaCompilado.Add(posicionDisponibleInstruccion++, (4000 + posicionEnSLM) + "");
                        }
                        else
                        {//si no existe: dejamos 4000 y marcamos en flags
                            programaCompilado.Add(posicionDisponibleInstruccion, "4000");
                            flags[posicionDisponibleInstruccion++] = numerico;//dejamos la linea PEPE++ que aun no existe
                        }
                        break;
                    case "if":
                        #region IF
                        //Es complicado; prestar atención
                        // SE cumple    [var/const] [desigualdad]   [var/const]
                        //              [2]         [3]             [4]

                        /***************************************************************
                         * 1: Agegar todas las variables o constantes a la tabala, si no están
                         *****************************************************************/

                        int posNum1, posNum2;//aca guardamos las posiciones de los numeros; nos será util a futuro
                        #region para primer elemento
                        if (int.TryParse(tokens[2], out numerico))//para el primer elemento
                        {//sii es num constante
                            if(!tablaDeSimbolos.TryGetValue(Tuple.Create(numerico,'c'), out posNum1))
                            {//si no está en la tabla,, lo agregamos
                                tablaDeSimbolos.Add(Tuple.Create(numerico, 'c'), posicionDisponibleValor);
                                programaCompilado.Add(posicionDisponibleValor, numerico + "");
                                posNum1 = posicionDisponibleValor--;//nos servirá más adelante; actualizando pos disponible de val
                            }
                        }
                        else
                        {//es una letra así que debe ser variable
                            numerico = tokens[2][0];//sacando valor ASCII
                            if (!tablaDeSimbolos.TryGetValue(Tuple.Create(numerico, 'v'), out posNum1))
                            {//si no está en la tabla,, lo agregamos
                                tablaDeSimbolos.Add(Tuple.Create(numerico, 'v'), posicionDisponibleValor);
                                programaCompilado.Add(posicionDisponibleValor, "0000");
                                posNum1 = posicionDisponibleValor--;//nos servirá más adelante; actualizando pos disponible de val
                            }
                        }
                        #endregion

                        #region para segundo elemento
                        if (int.TryParse(tokens[4], out numerico))//para el primer elemento
                        {//sii es num constante
                            if (!tablaDeSimbolos.TryGetValue(Tuple.Create(numerico, 'c'), out posNum2))
                            {//si no está en la tabla,, lo agregamos
                                tablaDeSimbolos.Add(Tuple.Create(numerico, 'c'), posicionDisponibleValor);
                                programaCompilado.Add(posicionDisponibleValor, numerico + "");
                                posNum2 = posicionDisponibleValor--;//nos servirá más adelante; actualizando pos disponible de val
                            }
                        }
                        else
                        {//es una letra así que debe ser variable
                            numerico = tokens[4][0];//sacando valor ASCII
                            if (!tablaDeSimbolos.TryGetValue(Tuple.Create(numerico, 'v'), out posNum2))
                            {//si no está en la tabla,, lo agregamos
                                tablaDeSimbolos.Add(Tuple.Create(numerico, 'v'), posicionDisponibleValor);
                                programaCompilado.Add(posicionDisponibleValor, "0000");
                                posNum2 = posicionDisponibleValor--;//nos servirá más adelante; actualizando pos disponible de val
                            }
                        }
                        #endregion

                        /***************************************************************
                         * 2: Ahora vamos a evaluar la expresión lógica simple > < >= <= == !=
                         *****************************************************************/

                        //Recordando que:   [goto]  [linea PEPE++]
                        //                  [5]     [6]
                        switch (tokens[3])
                        {
                            case "==":
                                /* IF X == Y GOTO Z
                                 * 20 X
                                 * 31 Y
                                 * 42 Z */
                                programaCompilado.Add(posicionDisponibleInstruccion++, (2000 + posNum1)+"");//acá usamos posNum1
                                programaCompilado.Add(posicionDisponibleInstruccion++, (3100 + posNum2) + "");//acá usamos posNum2
                                //comprobamos si la linea a la menciona goto satá en el futuro o no(COPIADO de goto y modificado)
                                //verificar si la linea a la que pretende ir existe
                                numerico = int.Parse(tokens[6]);
                                if (tablaDeSimbolos.TryGetValue(Tuple.Create(numerico, 'l'), out posicionEnSLM))
                                {//si ya existe: es facil
                                    programaCompilado.Add(posicionDisponibleInstruccion++, (4200 + posicionEnSLM) + "");
                                }
                                else
                                {//si no existe: dejamos 4000 y marcamos en flags
                                    programaCompilado.Add(posicionDisponibleInstruccion, "4200");
                                    flags[posicionDisponibleInstruccion++] = numerico;//dejamos la linea PEPE++ que aun no existe
                                }
                                break;
                            case "<":
                                /* IF X < Y GOTO Z
                                 * 20 X
                                 * 31 Y
                                 * 41 Z */
                                programaCompilado.Add(posicionDisponibleInstruccion++, (2000 + posNum1) + "");//acá usamos posNum1
                                programaCompilado.Add(posicionDisponibleInstruccion++, (3100 + posNum2) + "");//acá usamos posNum2
                                //el siguiente metodo lo definimos mas adelante. Hace lo mismo que el == anterior
                                verificarInsertar(int.Parse(tokens[6]), 4100, posicionDisponibleInstruccion++);
                                break;
                            case ">":
                                /* IF X > Y GOTO Z
                                 * 20 Y
                                 * 31 X
                                 * 41 Z */
                                programaCompilado.Add(posicionDisponibleInstruccion++, (2000 + posNum2) + "");//acá usamos posNum1
                                programaCompilado.Add(posicionDisponibleInstruccion++, (3100 + posNum1) + "");//acá usamos posNum2
                                verificarInsertar(int.Parse(tokens[6]), 4100, posicionDisponibleInstruccion++);
                                break;
                            //los difíciles
                            case "!=":
                                /* IF X != Y GOTO Z
                                 * 20 X
                                 * 31 Y
                                 * 42 F   SALTOCERO 
                                   40 Z
                                 F
                                 */
                                programaCompilado.Add(posicionDisponibleInstruccion++, (2000 + posNum1) + "");//acá usamos posNum1
                                programaCompilado.Add(posicionDisponibleInstruccion++, (3100 + posNum2) + "");//acá usamos posNum2
                                programaCompilado.Add(posicionDisponibleInstruccion++, (4200 + posicionDisponibleInstruccion + 1 ) + "");
                                verificarInsertar(int.Parse(tokens[6]), 4000, posicionDisponibleInstruccion++);
                                posicionDisponibleInstruccion++;//Un incremento extra pues no queremos sobrescribir la linea F
                                break;
                            case "<=":
                                /* IF X <= Y GOTO Z
                                 * 20 Y
                                 * 31 X
                                 * 41 F   
                                   40 Z
                                 F
                                 */
                                programaCompilado.Add(posicionDisponibleInstruccion++, (2000 + posNum2) + "");//acá usamos posNum1
                                programaCompilado.Add(posicionDisponibleInstruccion++, (3100 + posNum1) + "");//acá usamos posNum2
                                programaCompilado.Add(posicionDisponibleInstruccion++, (4100 + posicionDisponibleInstruccion + 1) + "");
                                verificarInsertar(int.Parse(tokens[6]), 4000, posicionDisponibleInstruccion++);
                                posicionDisponibleInstruccion++;//Un incremento extra pues no queremos sobrescribir la linea F
                                break;
                            case ">=":
                                /* IF X <= Y GOTO Z
                                 * 20 X
                                 * 31 Y
                                 * 41 F   
                                   40 Z
                                 F
                                 */
                                programaCompilado.Add(posicionDisponibleInstruccion++, (2000 + posNum1) + "");//acá usamos posNum1
                                programaCompilado.Add(posicionDisponibleInstruccion++, (3100 + posNum2) + "");//acá usamos posNum2
                                programaCompilado.Add(posicionDisponibleInstruccion++, (4100 + posicionDisponibleInstruccion + 1) + "");
                                verificarInsertar(int.Parse(tokens[6]), 4000, posicionDisponibleInstruccion++);
                                posicionDisponibleInstruccion++;//Un incremento extra pues no queremos sobrescribir la linea F
                                break;
                            default:
                                Console.WriteLine($"Error de IF: Operador lógico no reconocido -> {tokens[3]}");
                                break;
                        }

                        break;
                    #endregion
                    case "let":
                        #region LET
                        //Es complicado... pero nos ayudará nuestro ConversorPCC
                        /***************************************************************
                         * 1: Agegar todas las variables o constantes a la tabala, si no están
                         *****************************************************************/
                        //  60  let x   =   5   *   a
                        //  [0] [1] [2] [3] [4] ....
                        //Vamos a usar aquello que usamos en if, pero resumido en un metodo con control de paréntesis etc
                        posicionDisponibleValor = verificarAgregarValor(tokens[2], posicionDisponibleValor);//retorna porque puede ser que no exista y lo crea (al valor constante o variable)

                        for(int i = 4; i < tokens.Length; i++)
                            posicionDisponibleValor = verificarAgregarValor(tokens[i], posicionDisponibleValor);
                        /***************************************************************
                         * 2: Ahora vamos a evaluar la expresión algebraica
                         *****************************************************************/
                        string[] exp = linea.Split('=');// genera dos elementos: [60 let r ] [ n * 2]
                        ConversorPCC conversor = new ConversorPCC(exp[1].Trim(),tablaDeSimbolos,programaCompilado,
                            posicionDisponibleInstruccion,posicionDisponibleValor);
                        conversor.convertir();//a estas alturas estará en postfix
                        //evaluamos la expresion y ponemos el resultado en la variable // let r = 
                        //OJO: que la variable destino es el tokens[2]
                        int posicionResultado = conversor.evaluarExpresion();
                        posicionDisponibleInstruccion = conversor.getPosicionActual();
                        posicionDisponibleValor = conversor.getPosicionLibre();
                        if(tablaDeSimbolos.TryGetValue(Tuple.Create((int)tokens[2][0], 'v'), out int posicionDeVar))//Obtenemos la posición de la variable destino
                        {
                            programaCompilado.Add(posicionDisponibleInstruccion++, (2000 + posicionResultado)+"");//que cargue la posición de resultado
                            programaCompilado.Add(posicionDisponibleInstruccion++, (2100 + posicionDeVar)+"");//que lo almacene en la posición de variable
                        }


                        #endregion
                        break;
                    default:
                        Console.WriteLine($"Error de compilación: comando desconocido: {tokens[1]}");
                        break;
                }
                #endregion


            }
            //Acabó la primer pasada
            //Celebremos volcando la memoria ¿cómo se ve?
            Console.WriteLine("*********** PROGRAMA ************");
            for (int i = 0; i < 100; i++)
                if (programaCompilado.ContainsKey(i))
                    Console.WriteLine($"{i} ? {programaCompilado[i]}");
        }

        private void verificarInsertar(int lineaGOTO, int instruccionSLM, int posDispInstruccion)
        {
            if (tablaDeSimbolos.TryGetValue(Tuple.Create(lineaGOTO, 'l'), out int posicionSLM))
            {//si existe, es facil
                programaCompilado.Add(posDispInstruccion, (instruccionSLM + posicionSLM) + "");
            }
            else
            {//si no existe, se deja la instruccion (4100) y marcamos en flags 
                programaCompilado.Add(posDispInstruccion, instruccionSLM + "");
                flags[posDispInstruccion] = lineaGOTO;
            }

        }

        private int verificarAgregarValor(string token, int posicion)
        {
            if (!"()+-*/^%".Contains(token))//Si es parentesis u operador -> no se considera
            {
                if(int.TryParse(token, out int numerico))
                {//sí es numero constante
                    if(!tablaDeSimbolos.TryGetValue(Tuple.Create(numerico, 'c'), out int n))
                    {//si no está en la tabla de símbolos
                        Console.WriteLine($"Va a guardar {numerico} como 'c'");
                        tablaDeSimbolos.Add(Tuple.Create(numerico, 'c'), posicion);
                        programaCompilado.Add(posicion--, numerico + "");
                    }
                }
                else
                {//es una letra, asi que debe ser var
                    numerico = token[0];
                    if (!tablaDeSimbolos.TryGetValue(Tuple.Create(numerico, 'v'), out int n))
                    {//si no está en la tabla de símbolos
                        tablaDeSimbolos.Add(Tuple.Create(numerico, 'v'), posicion);
                        programaCompilado.Add(posicion--, "0000");
                    }
                }
            }
            return posicion;
        }
    }
}
