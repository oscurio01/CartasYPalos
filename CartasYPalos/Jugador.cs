using CartasYPalos;
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
        List<Carta> Cartas { get; set; }

        int dinero;

        public Jugador()
        {
            SeRetira = false;
            EstaApostando = false;
            dinero = 1000;
            Cartas = new List<Carta>();
        }

        public void AñadirCartas(Baraja Baraja)
        {
            Carta carta = Baraja.Robar(TipoDeRobo.PrimeroEnLaBaraja);
            Cartas.Add(carta);
        }

        public void AñadirCartas(List<Carta> cartas)
        {
            Cartas.AddRange(cartas);
        }

        public void MostrarMano()
        {
            Console.WriteLine("Tus cartas son:");
            for(int i = 0; i < Cartas.Count(); i++)
            {
                Console.Write("=================");
            }
            Console.WriteLine();
            for (int i = 0; i < Cartas.Count; i++)
            {
                Console.Write($"|| {Cartas[i].TipoDeCarta} - {Cartas[i].Numero} ");
            }
            Console.WriteLine("||");
            for (int i = 0; i < Cartas.Count(); i++)
            {
                Console.Write("=================");
            }
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
            dinero -= apuesta;
            EstaApostando = true;
        }

        public void AñadirDinero(int dinero)
        {
            this.dinero += dinero;
        }

        public (int, TipoDeJugada) ObtenerJugada(List<Carta> CartasEnLaMesa)
        {
            AñadirCartas(CartasEnLaMesa);

            var palosEnMazo = Cartas.GroupBy(c => c.TipoDeCarta).Select(g => (g.Key, g.Count())).ToArray();
            var valorEnMazo = Cartas.GroupBy(c => c.Numero).Select(g => (g.Key, g.Count())).ToArray();

            int numeroMasAlto = Cartas.Max(c => (int)c.Numero);

            int algo = numeroMasAlto;
            bool esEscalera = EsEscalera();
            bool esColor = palosEnMazo.Any(count => count.Item2 >= 5);

            // Verificar las jugadas de acuerdo con las combinaciones
            if (EsEscaleraReal(esEscalera, esColor)) // Una escalera de color con las cartas 10, J, Q, K y As.
                return (numeroMasAlto, TipoDeJugada.EscaleraReal); 

            if (esColor && esEscalera) // Una escalera donde todas las cartas son del mismo palo.
                return (numeroMasAlto, TipoDeJugada.EscaleraDeColor);

            if (valorEnMazo.Any(A => A.Item2 == 4)) // Cuatro cartas del mismo valor.
                return (numeroMasAlto, TipoDeJugada.Póker);

            if (valorEnMazo.Any(A => A.Item2 == 3) && valorEnMazo.Any(A => A.Item2 == 2)) // Un trío y un par.
                return (numeroMasAlto, TipoDeJugada.Full); 

            if(esColor) // Cinco cartas del mismo palo, sin importar el orden numérico.
                return (numeroMasAlto, TipoDeJugada.Color);

            if(esEscalera) // Cinco cartas en secuencia numérica, sin importar el palo.
                return (numeroMasAlto, TipoDeJugada.Escalera); 

            if (valorEnMazo.Any(A => A.Item2 == 3)) // Tres cartas del mismo valor.
                return (numeroMasAlto, TipoDeJugada.Trío);

            if (valorEnMazo.Any(A => A.Item2 == 2) && valorEnMazo.Count(v => v.Item2 == 2) == 2) // Para una mano con dos pares de cartas del mismo valor.
                return (numeroMasAlto, TipoDeJugada.DoblePareja);

            if (valorEnMazo.Any(A => A.Item2 == 2)) // Para una mano que tenga dos cartas del mismo valor.
                return (numeroMasAlto, TipoDeJugada.Pareja);

            return (numeroMasAlto, TipoDeJugada.Ninguna);
        }

        private bool EsEscalera()
        {
            // Los ordena de menor a mayor
            var valoresUnicos = Cartas.Select(c => c.Numero).Distinct().OrderBy(v => v).ToList();
            for (int i = 0; i < valoresUnicos.Count - 1; i++)
            {
                // Comprueba que la carta siguiente sea un valor mayor al anterior pero no mayor de 1
                if (valoresUnicos[i] + 1 != valoresUnicos[i + 1])
                {
                    return false;
                }
            }
            return valoresUnicos.Count >= 5;
        }

        private bool EsEscaleraReal(bool color, bool escalera)
        {
            if(!color && !escalera)
                return false;

            var valoresUnicos = Cartas.Select(c => c.Numero).Distinct().ToList();
            return valoresUnicos.Contains(NumerosDeCarta.Diez) && valoresUnicos.Contains(NumerosDeCarta.J) &&
                valoresUnicos.Contains(NumerosDeCarta.Q) && valoresUnicos.Contains(NumerosDeCarta.K) &&
                valoresUnicos.Contains(NumerosDeCarta.A);

        }
    }
}