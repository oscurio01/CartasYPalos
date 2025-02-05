using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartasYPalos
{
    // El jugador le dan 2 cartas al principio de ronda
    // Los jugadores pueden pasar o pueden apostar 
    // Si alguien apuesta o el diner de la apuesta
    // no es 0 y pasa, ese jugador se retira de esa partida
    // Si todos pasan no sube el dinero de la apuesta
    // El que se quede sin dinero pierde, no puede jugar
    // En la segunda ronda
    internal class Jugador
    {
        public bool SeRetira;
        public bool EstaApostando;
        public int Dinero { get { return dinero; } }
        public List<Carta> Cartas { get; set; }

        int dinero;

        public Jugador()
        {
            SeRetira = false;
            EstaApostando = false;
            dinero = 1000;
            Cartas = new List<Carta>();
        }

        public void AñadirCartas(Carta carta)
        {
            Cartas.Add(carta);
        }

        public void AñadirCartas(List<Carta> cartas)
        {
            Cartas.AddRange(cartas);
        }

        public void MostrarMano()
        {
            Console.WriteLine("Tus cartas son:");
            for (int i = 0; i < Cartas.Count; i++)
            {
                Console.Write($"|| {Cartas[i].TipoDeCarta} - {Cartas[i].Numero} ");
            }
            Console.Write("||");
            Console.WriteLine();
        }

        public void LimpiarMano()
        {
            Cartas.Clear();
        }

        public Carta SacarCarta(int numero)
        {
            if (Cartas.Count == 0)
                return null;

            Carta carta = Cartas[numero];
            Cartas.RemoveAt(numero);
            return carta;
        }
        public void Apuesta(int apuesta)
        {
            dinero -= apuesta ;
            EstaApostando = true;
        }

        public TipoDeJugada ObtenerJugada(List<Carta>CartasEnLaMesa)
        {
            // Pareja: Para una mano que tenga dos cartas del mismo valor.
            // DoblePareja: Para una mano con dos pares de cartas del mismo valor.
            // Trío: Tres cartas del mismo valor.
            // Escalera: Cinco cartas en secuencia numérica, sin importar el palo.
            // Color: Cinco cartas del mismo palo, sin importar el orden numérico.
            // Full: Un trío y un par.
            // Póker: Cuatro cartas del mismo valor.
            // EscaleraDeColor: Una escalera donde todas las cartas son del mismo palo.
            // EscaleraReal: Una escalera de color con las cartas 10, J, Q, K y As.

            return TipoDeJugada.Ninguna;
        }

    }
}
