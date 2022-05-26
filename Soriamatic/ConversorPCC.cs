using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Soriamatic
{
    internal class ConversorPCC : Conversor
    {
        IDictionary<Tuple<int,char>,int> tablaDeSimbolos;//Para la busqueda de variables // "perro", 3
        IDictionary<int, string> programa;//Para continuar armando el programa SLM[]
        int posicionActual, posicionLibre;
        public ConversorPCC(string infix, IDictionary<Tuple<int, char>, int> tablaSimbolos, IDictionary<int,string> prog, 
            int posAct, int posLibre) : base(infix)
        {
            tablaDeSimbolos = tablaSimbolos;
            programa = prog;
            posicionActual = posAct;
            posicionLibre = posLibre;
        }
        // Ya podemos usar el metodo convertir.

        //Nos concentramos en la evaluación
        new public int evaluarExpresion()
        {
            string[] valores = exp_postfix.Split(' ');
            Queue cola = new Queue();
            Stack pila = new Stack();
            for (int i = 0; i < valores.Length; i++)
                cola.Enqueue(valores[i]);
            cola.Enqueue(")");
            string actual = "";
            actual = (string)cola.Dequeue();
            while (!actual.Equals(")"))
            {
                if (esOperador(actual))
                {
                    int y = (int)pila.Pop();
                    int x = (int)pila.Pop();
                    switch (actual)
                    {
                        case "+":

                            //x = 9, y = 10
                            //2009
                            //3010
                            //2198
                            programa.Add(posicionActual++,(2000 + x)+ "");
                            programa.Add(posicionActual++, (3000 + y) + "");
                            programa.Add(posicionActual++, (2100 + posicionLibre) + "");
                            pila.Push(posicionLibre--);
                            break;
                        case "-":
                            programa.Add(posicionActual++, (2000 + x) + "");
                            programa.Add(posicionActual++, (3100 + y) + "");
                            programa.Add(posicionActual++, (2100 + posicionLibre) + "");
                            pila.Push(posicionLibre--);
                            break;
                        case "*":
                            programa.Add(posicionActual++, (2000 + x) + "");
                            programa.Add(posicionActual++, (3200 + y) + "");
                            programa.Add(posicionActual++, (2100 + posicionLibre) + "");
                            pila.Push(posicionLibre--);
                            break;
                        case "/":
                            programa.Add(posicionActual++, (2000 + x) + "");
                            programa.Add(posicionActual++, (3300 + y) + "");
                            programa.Add(posicionActual++, (2100 + posicionLibre) + "");
                            pila.Push(posicionLibre--);
                            break;
                        default:
                            Console.WriteLine("Error: Operador no reconocido.");
                            break;
                    }
                }
                else
                {
                    int pos;
                    if (int.TryParse(actual, out int num))
                    {//si es num constante
                        pos = tablaDeSimbolos[Tuple.Create(num, 'c')]; // int,char -> num,'c'
                    }
                    else {//es una variable // consultar la pos de mem de esta variable
                        pos = tablaDeSimbolos[Tuple.Create((int)Convert.ToChar(actual), 'v')]; // int, char -> actual (x),'v'
                    }
                    pila.Push(pos);
                }

                actual = (string)cola.Dequeue();
            }
            return (int)pila.Pop();
        }

        public string informe() {
            string texto = $"posicionActual: {posicionActual}\nposicionLibre: {posicionLibre}\nmemoria:\n";
            for (int i = 0; i < 100; i++)
                if (programa.ContainsKey(i))
                    texto += $"{i} ? {programa[i]}\n";
            texto += $"********************************************\nExpresion postfix: {exp_postfix}";
            return texto;
        }

    }
}
