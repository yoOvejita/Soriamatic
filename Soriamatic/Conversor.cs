using System;
using System.Collections;
using System.Text;

namespace Soriamatic
{
    internal class Conversor

        // ( ) + - * / % ^ 
    {
        // INFIX: ( 6 + 2 ) * 5 - 8 / 4

        // POSTFIX: 6 2 + 5  * 8 4 / -
        //               38
        /* infix -> postfix
         * 
         1. Meter ( a la pila
         2. Agregar ) al final del infix
         3. Mientras no este vacia, recorremos el infix --->
            numero: concatenamos al postfix
            (: lo ponemos en la pila
            operador: sacar operadores de la cima si son mayores o iguales en precedencia, lo
        ponemos en postfix. El operador actual lo ponemos en la pila.

                        ()



                        ^
                      /  * %
                     +      -


                    ---------->
          
         */

        private string exp_infix;
        protected string exp_postfix;
        Stack pila;
        public Conversor(string infix)
        {
            exp_infix = infix;
        }

        public void convertir() {
            pila = new Stack();
            string postfix = "";
            string[] valores = exp_infix.Split(' ');// sdaewaxazz -> [sd, ew, x, zz]
            Queue infix = new Queue();
            for(int i= 0; i < valores.Length; i++)
                infix.Enqueue(valores[i]);

            //Comienza el algoritmo
            pila.Push("(");
            infix.Enqueue(")");
            while (pila.Count > 0)
            {
                string actual = (string)infix.Dequeue();
                switch (actual)
                {
                    case "(":
                        pila.Push(actual);
                        break;
                    case ")":
                        while (!pila.Peek().Equals("("))
                            postfix += pila.Pop() + " ";
                        pila.Pop();// (
                        break;
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                    case "^":
                    case "%":
                        while (esOperador((string)pila.Peek()) && !esMenorQue((string)pila.Peek(), actual))
                            postfix += pila.Pop() + " ";
                        pila.Push(actual);
                        break;
                    default:
                        postfix += actual + " ";
                        break;
                }
            }
            exp_postfix = postfix.TrimEnd();
        }
        protected bool esOperador(string op)
        {
            //también con tryparse
            //if (op.Equals("+"))
            //con arreglo y contiene
            //return op.Equals("+") || op.Equals("-");
            string operadores = "+-*/^%";
            return operadores.Contains(op);
        }

        private bool esMenorQue(string op1, string op2) {
            if ("+-".Contains(op2))
                return false;
            if ("/%*".Contains(op1) && "^/%*".Contains(op2))
                return false;
            if(op1.Equals("^") && op2.Equals("^"))
                return false;
            return true;
        }

        public int evaluarExpresion()
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
                    switch (actual) {
                        case "+":
                            pila.Push(x + y);
                            break;
                        case "-":
                            pila.Push(x - y);
                            break;
                        case "*":
                            pila.Push(x * y);
                            break;
                        case "/":
                            pila.Push(x / y);
                            break;
                        case "%":
                            pila.Push(x % y);
                            break;
                        case "^":
                            pila.Push(Math.Pow(x,y));
                            break;
                    }
                }
                else
                    pila.Push(int.Parse(actual));

                actual = (string)cola.Dequeue();
            }
            return (int)pila.Pop();
        }
    }
}
