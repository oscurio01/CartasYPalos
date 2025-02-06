using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartasYPalos
{
    public enum TipoDeRobo { PrimeroEnLaBaraja, SeleccionarCarta, Aleatorio }

    internal class Baraja
    {

        List<Carta> Cartas { get; set; }

        public Baraja() 
        {
            Cartas = new List<Carta>();
            GenerarCartas();
        }

        void GenerarCartas()
        {
            foreach (TipoDeCarta tipo in Enum.GetValues(typeof(TipoDeCarta)))
            { 
                foreach(NumerosDeCarta numEsp in Enum.GetValues(typeof(NumerosDeCarta)))
                {
                    Cartas.Add(new Carta(tipo, numEsp));
                }
            }
            
            Barajar();
        }

        public void Barajar()
        {
            Random random = new Random();
            Cartas = Cartas.OrderBy(c => random.Next()).ToList();
        }

        // Robar
        public Carta Robar(TipoDeRobo eleccion, int numero = 0)
        {
            if (Cartas.Count == 0)
                return null;
            else if (eleccion == TipoDeRobo.Aleatorio)
            {
                Random rand = new Random();
                numero = rand.Next();
            }
            Carta carta = Cartas[numero];
            Cartas.RemoveAt(numero);
            return carta;

        }
    }
}
