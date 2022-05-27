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
                        break;
                    case "let":
                        break;
                    default:
                        Console.WriteLine($"Error de compilación: comando desconocido: {tokens[1]}");
                        break;
                }
                #endregion
            }
        }


    }
}
