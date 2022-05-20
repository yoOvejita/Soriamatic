using System;
using System.Collections.Generic;
using System.Text;

namespace Soriamatic
{
    internal class Conversor

        // ( ) + - * / % ^ 
    {
        // INFIX: (6 + 2) * 5 - 8 / 4

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

    }
}
