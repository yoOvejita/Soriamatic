using System;
using System.Collections.Generic;
using System.Text;

namespace Academico
{
    public class Estudiante
    {
        private string nombre;
        public string Nombre {
            get { return "Nomre: " + nombre; }
            set { nombre = "Sr. " + value; }
        }
        private string apellido { get; set; }
        public int edad { get; set; } = 5;

        
        public Estudiante()
        {
            nombre = "NA";
            apellido = "NA";
            edad = 0;
        }

        public Estudiante(string n)
        {
            nombre = n;
            apellido = "NA";
            edad = 0;
        }

        public Estudiante(string nombre, string apellido, int edad)
        {
            this.nombre = nombre;
            this.apellido = apellido;
            this.edad = edad;
        }

        public void hablar() {
            Console.WriteLine("Hola, soy "+ nombre);
        }
        
    }

    namespace Enseñanza
    {
        public class Docente
        {
            int edad;
            public Docente(){
            }
            public static void reñir() {
                Console.WriteLine("!!");
            }
        }
    }
}
