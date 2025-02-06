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
            TipoDeJugada mejorJugada = TipoDeJugada.Ninguna;
            AñadirCartas(CartasEnLaMesa);

            var palosEnMazo = Cartas.GroupBy(c => c.TipoDeCarta).ToDictionary(g => g.Key, g => g.Count());
            var valorEnMazo = Cartas.GroupBy(c => c.Numero).ToDictionary(g => g.Key, g=> g.Count());

            var combinaciones = ObtenerTodasLasCombinacionesDe5Cartas(Cartas);

            foreach(var combinacion in combinaciones)
            {
                var jugada = EvaluarCombinacion(combinaciones);
            }


            return mejorJugada;
        }
        /*
        #region NOtocar
        public class Carta
        {
            public int Valor { get; set; }  // 2-14 para las cartas 2-10, J=11, Q=12, K=13, As=14
            public string Palo { get; set; } // Corazones, Diamantes, Tréboles, Picas
        }

        public class Jugador2
        {
            public List<Carta> Cartas { get; set; }

            public TipoDeJugada ObtenerJugada()
            {
                // Agrupar las cartas por su valor
                var valoresContados = Cartas
                    .GroupBy(c => c.Valor)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Agrupar las cartas por su palo
                var palosContados = Cartas
                    .GroupBy(c => c.Palo)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Verificar si hay Escalera
                bool esEscalera = EsEscalera();

                // Verificar si hay Color
                bool esColor = palosContados.Values.Any(count => count >= 5);

                // Verificar las jugadas de acuerdo con las combinaciones
                if (EsEscaleraReal(esEscalera, esColor)) return TipoDeJugada.EscaleraReal;
                if (esColor && esEscalera) return TipoDeJugada.EscaleraDeColor;
                if (valoresContados.ContainsValue(4)) return TipoDeJugada.Póker;
                if (valoresContados.ContainsValue(3) && valoresContados.ContainsValue(2)) return TipoDeJugada.Full;
                if (valoresContados.ContainsValue(3)) return TipoDeJugada.Trío;
                if (valoresContados.ContainsValue(2) && valoresContados.Count(v => v.Value == 2) == 2) return TipoDeJugada.DoblePareja;
                if (valoresContados.ContainsValue(2)) return TipoDeJugada.Pareja;

                return TipoDeJugada.Ninguna;
            }

            private bool EsEscalera()
            {
                // Lógica para verificar si hay una escalera
                var valoresUnicos = Cartas.Select(c => c.Valor).Distinct().OrderBy(v => v).ToList();
                for (int i = 0; i < valoresUnicos.Count - 1; i++)
                {
                    if (valoresUnicos[i] + 1 != valoresUnicos[i + 1])
                    {
                        return false;
                    }
                }
                return valoresUnicos.Count >= 5;
            }

            private bool EsEscaleraReal(bool esEscalera, bool esColor)
            {
                // Comprobar si es una escalera real
                if (!esEscalera || !esColor) return false;

                var valoresUnicos = Cartas.Select(c => c.Valor).Distinct().ToList();
                return valoresUnicos.Contains(10) && valoresUnicos.Contains(11) && valoresUnicos.Contains(12) && valoresUnicos.Contains(13) && valoresUnicos.Contains(14);
            }
        }

        #endregion
        */
        private List<List<Carta>> ObtenerTodasLasCombinacionesDe5Cartas(List<Carta> cartas)
        {
            var combinaciones = new List<List<Carta>>();

            for (int i = 0; i < cartas.Count - 4; i++)
            {
                for (int j = i + 1; j < cartas.Count -3; j++)
                {
                    for(int k = j + 1; k < cartas.Count -2; k++)
                    {
                        for(int l = k + 1; l < cartas.Count -1; l++)
                        {
                            for(int m = l + 1; m < cartas.Count; m++)
                            {
                                combinaciones.Add(new List<Carta> { cartas[i], cartas[j], cartas[k], cartas[l], cartas[m] });
                            }
                        }
                    }
                }
            }

            return combinaciones;
        }

        private TipoDeJugada EvaluarCombinacion(List<List<Carta>> cartas)
        {
            foreach(var carta in cartas)
            {

            }

            return TipoDeJugada.Ninguna;
        }
    }
}
