using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CartasYPalos
{
    public enum TipoDeJugada {
        Ninguna,        
        Pareja,         
        DoblePareja,    
        Trío,           
        Escalera,       
        Color,          
        Full,
        Póker,
        EscaleraDeColor, 
        EscaleraReal    
    }
    internal class Program
    {
        static bool Salir = false;
        static bool unJugadorApostado = false;
        static bool ApuestasIgualadas = true;
        static int cartasEnUnMazo = 2;
        static int numeroDeJugadores = 1;
        static int TurnoDelJugador = 0;
        static int Ronda = 0;
        static int dineroDeRonda = 0;
        static int dineroTotal = 0;
        static Baraja Baraja;
        static List<Carta> CartasEnLaMesa = new List<Carta>();
        static List<Jugador> Jugadores = new List<Jugador>();
        static Dictionary<Jugador, int> DiccionarioDeApuestas = new Dictionary<Jugador, int>();

        static void Main(string[] args)
        {
            Console.WriteLine("Dime la cantidad de jugadores que van a jugar: ");
            numeroDeJugadores = LeerUnNumeroCorrecto(7, 2);

            IniciarPartida();

            while (!Salir)
            {
                IniciarRonda();
                Partida();
            }

            foreach(Jugador jugador in Jugadores)
            {
                if (!jugador.SeRetira)
                    TurnoDelJugador = Jugadores.IndexOf(jugador);
            }

            Console.WriteLine($"Bien hecho jugador {TurnoDelJugador + 1}, parece que eres el unico en pie, te llevas {Jugadores[TurnoDelJugador].Dinero+dineroTotal}$ a casa");
        }

        static int LeerUnNumeroCorrecto(int maximo, int minimo = 0, string texto = "Numero no valido")
        {
            int numeroCorrecto;
            while (true)
            {
                Console.Write("> ");
                if (int.TryParse(Console.ReadLine(), out numeroCorrecto) && numeroCorrecto >= minimo && numeroCorrecto <= maximo)
                    return numeroCorrecto;
                else
                    Console.WriteLine(texto);
            }
        }

        static void IniciarPartida()
        {
            Baraja = new Baraja();

            for (int i = 0; i < numeroDeJugadores; i++)
            {
                Jugadores.Add(new Jugador());
                DarCartaAJugadores(i);

                DiccionarioDeApuestas.Add(Jugadores[i], 0);
            }
        }

        static void DarCartaAJugadores(int i)
        {
            for (int j = 0; j < cartasEnUnMazo; j++)
            {
                Jugadores[i].AñadirCartas(Baraja);
            }
        }

        static void IniciarRonda()
        {
            if(TurnoDelJugador != 0 && TurnoDelJugador != numeroDeJugadores)
                return;
            if (!ApuestasIgualadas && unJugadorApostado)
                return;

            switch (Ronda)
            {
                case 1:
                    for(int i = 0; i < 3; i++)
                    {
                        CartasEnLaMesa.Add(Baraja.Robar(TipoDeRobo.PrimeroEnLaBaraja));
                    }
                break;
                case 2:
                case 3:
                    CartasEnLaMesa.Add(Baraja.Robar(TipoDeRobo.PrimeroEnLaBaraja));
                    break;
                case 4:
                    FinalDePartida();
                    break;
            }

            foreach (Jugador jugador in Jugadores)
            {
                jugador.EstaApostando = false;
                DiccionarioDeApuestas[jugador] = 0;
            }

            dineroDeRonda = 0;
            
        }

        static void Partida()
        {
            int eleccion = 0;
            TurnoDelJugador = TurnoDelJugador % numeroDeJugadores;

            // Para comprobar si alguien aposto en la ronda anterior
            if(Ronda != 0)
                unJugadorApostado = dineroDeRonda != 0 ? true : DiccionarioDeApuestas.Values.Any(apuesta => apuesta > 0);
            // Si casi los jugadores abandonan, gana el ultimo en pie
            if (Jugadores.Select(c => c.SeRetira).Count(v => v == true) == numeroDeJugadores - 1 || Jugadores.Select(c => c.SeRetira).Count(v => v == true) == numeroDeJugadores)
            {
                Salir = true;
                return;
            }
            // Si el jugador se retira de esa partida y no aparece mas hasta la siguiente
            if (Jugadores[TurnoDelJugador].SeRetira)
            {
                ++TurnoDelJugador;
                if (TurnoDelJugador == numeroDeJugadores)
                    ++Ronda;
                return;
            }

            MostrarPantalla();

            eleccion = LeerUnNumeroCorrecto(3, 0);

            switch (eleccion)
            {
                case 1:
                    if (Jugadores[TurnoDelJugador].Dinero > 0 && Jugadores[TurnoDelJugador].Dinero >= dineroDeRonda)
                        Apostar();
                    else
                    {
                        Console.WriteLine("No tienes dinero");
                        // Forzamos al jugador a retirarse si no tiene dinero suficiente para apostar
                        Retirarse();
                    }
                    break;
                case 2:
                    if (unJugadorApostado)
                        Console.WriteLine("No puedes pasar hay una apuesta, apuestas o te retiras");
                    break;
                case 3:
                    Retirarse();
                    break;
                case 0:
                    Salir = true;
                    return;
            }

            ComprobacionDeRonda();
            Console.WriteLine("Pulsa Enter para continuar...");
            Console.ReadLine();
        }

        static void ComprobacionDeRonda()
        {
            ApuestasIgualadas = DiccionarioDeApuestas.Values.All(v => v == DiccionarioDeApuestas.Values.Max());
            // Para comprobar si alguien aposto dinero en esta ronda
            unJugadorApostado = dineroDeRonda != 0 ? true : DiccionarioDeApuestas.Values.Any(apuesta => apuesta > 0);

            // Compruebo si alguien ha apostado y si las apuestas estan iguales
            // Es decir que todos han apostado el maximo de la ronda 
            if (!ApuestasIgualadas)
            {
                // Compruebo si tanto si el jugado ha apostado, como si el dinero es el maximo de la ronda
                if (Jugadores[TurnoDelJugador].EstaApostando &&
                    DiccionarioDeApuestas[Jugadores[TurnoDelJugador]] == dineroDeRonda)
                {
                    for (int i = 0; i < numeroDeJugadores; i++)
                    {
                        // Revisa quien no tiene el mismo monto de apuestas que el dinero de la ronda
                        if (DiccionarioDeApuestas[Jugadores[i]] != dineroDeRonda)
                        {
                            TurnoDelJugador = Jugadores.IndexOf(Jugadores[i]);
                            break;
                        }
                    }
                }
            }
            else if (ApuestasIgualadas && unJugadorApostado)
            {
                TurnoDelJugador = numeroDeJugadores; // Si todas las apuestas son iguales y hay algo mas que un 0
                ++Ronda;
            }
            else
                ++TurnoDelJugador;

            if (TurnoDelJugador == numeroDeJugadores && !unJugadorApostado)
                ++Ronda;
        }
        
        static void MostrarPantalla()
        {
            Jugador jugadorActual = Jugadores[TurnoDelJugador];
            Console.Clear();
            Console.WriteLine("Ronda : {0}", Ronda);
            Console.Write(Ronda == 0 ?"Es la primera ronda, las cartas estan ocultas" :"Las cartas en la mesa son : ");
            if (CartasEnLaMesa.Count > 0)
            {
                for (int i = 0; i < CartasEnLaMesa.Count(); i++)
                {
                    Console.Write($"|| {CartasEnLaMesa[i].TipoDeCarta} - {CartasEnLaMesa[i].Numero} ");
                }
                Console.Write("||");
            }
            Console.WriteLine();
            Console.WriteLine($"Jugador {TurnoDelJugador + 1} Dinero {jugadorActual.Dinero}$");

            jugadorActual.MostrarMano();

            Console.WriteLine($"La apuesta actual es de: {dineroDeRonda}$ y el total : {dineroTotal}$");

            if(DiccionarioDeApuestas[jugadorActual] < dineroDeRonda)
                Console.WriteLine($"Te falta {dineroDeRonda - DiccionarioDeApuestas[jugadorActual]}$ para igualar");

            Console.WriteLine(@"Que quieres hacer?
===========
1.Apostar
2.Pasar
3.Retirarse
===========");
        }

        static void Apostar()
        {
            int eleccion;
            Jugador jugadorActual = Jugadores[TurnoDelJugador];

            Console.WriteLine("Tienes {0}$ tu apuesta es de {1}$. Y el monto es de {2}$", jugadorActual.Dinero, DiccionarioDeApuestas[jugadorActual], dineroDeRonda);
            eleccion = LeerUnNumeroCorrecto(jugadorActual.Dinero, 1, "Eso no es un numero o no llega al monto minimo");
            // Se suma el dinero de la apuesta al diccionario para comprobar que todos apuestan lo mismo
            DiccionarioDeApuestas[jugadorActual] += eleccion;
            // Si el dinero que apuestas es mayor que el monto actual este se actualiza
            if (DiccionarioDeApuestas[jugadorActual] > dineroDeRonda)
            {
                dineroDeRonda = DiccionarioDeApuestas[jugadorActual];
                Console.WriteLine("Se ha actualizado el minimo monto a : {0}", dineroDeRonda);
            }

            // Se le suma el dinero al total
            dineroTotal += eleccion;
            jugadorActual.Apuesta(eleccion);
        }


        static void Retirarse()
        {
            Jugadores[TurnoDelJugador].SeRetira = true;
            DiccionarioDeApuestas.Remove(Jugadores[TurnoDelJugador]);
        }

        static void FinalDePartida()
        {
            TurnoDelJugador = 0;

            Console.Clear();
            Console.WriteLine("Las cartas en la mesa son : ");
            Console.WriteLine("=============================================================================");
            for (int j = 0; j < CartasEnLaMesa.Count(); j++)
            {
                Console.Write($"|| {CartasEnLaMesa[j].TipoDeCarta} - {CartasEnLaMesa[j].Numero} ");
            }
            Console.WriteLine("||");
            Console.WriteLine("=============================================================================");

            // Cuando todos han apostado Y las cartas han salido
            // Se tiene que comparar quien de todos los jugadores es el ganador
            TipoDeJugada jugada;
            (TurnoDelJugador,jugada)  = QuienGanaLaApuesta();

            Console.WriteLine($"El ganador es : Jugador {TurnoDelJugador + 1 } con {jugada}");

            Jugadores[TurnoDelJugador].AñadirDinero(dineroTotal); // Se le suma el dinero total de esa partida

            // Se reinicia el dinero de ronda, el dinero total y las cartas de los jugadores
            dineroDeRonda = 0;
            dineroTotal = 0;
            Console.WriteLine(@"
Quieres jugar otra partida o quieres terminar la partida?
1.Continuar
2.Salir");
            int eleccion = LeerUnNumeroCorrecto(2, 1);

            if (eleccion == 1)
            {
                Ronda = 0;
                TurnoDelJugador = 0;
                CartasEnLaMesa.Clear();

                for (int i = 0; i < numeroDeJugadores; i++)
                {
                    Jugadores[i].LimpiarMano();
                    DarCartaAJugadores(i);
                }
            }
            else if (eleccion == 2)
            {
                foreach (var jugador in Jugadores)
                {
                    jugador.SeRetira = true;
                }
            }
        }

        static (int, TipoDeJugada) QuienGanaLaApuesta()
        {
            // Comprueba en cada jugador ObtenerJugada y si ese jugador no participa
            // no cuenta y se comprueba por orden de escala en TipoDeJugada
            int indice = 0;
            int numeroDelJugador = 0;
            int numeroMasAlto = -1;
            int ganador = 0;
            TipoDeJugada jugadaAnterior = TipoDeJugada.Ninguna;
            List<(int, TipoDeJugada)> Jugadas = new List<(int, TipoDeJugada)>();

            foreach(var jugador in Jugadores)
            {
                indice++;
                if(jugador.SeRetira)
                    continue;

                ++numeroDelJugador;
                Jugadas.Add(jugador.ObtenerJugada(CartasEnLaMesa));

                Console.WriteLine($"Añadiendo Jugador {indice} y su jugada es: {Jugadas[numeroDelJugador-1].Item2}");
                jugador.MostrarMano();
                Thread.Sleep(1000);
            }

            for (int i = 0; i < Jugadas.Count; i++)
            {
                // Comparamos cada jugada con la jugada ganadora actual
                if (Jugadas[i].Item2 > jugadaAnterior)
                {
                    jugadaAnterior = Jugadas[i].Item2;
                    numeroMasAlto = Jugadas[i].Item1;
                    ganador = i;
                }
                else if(Jugadas[i].Item2 == jugadaAnterior)
                {
                    if (Jugadas[i].Item1 > numeroMasAlto)
                        ganador = i;
                    else if (Jugadas[i].Item1 < numeroMasAlto)
                        ganador = i - 1;
                    else
                        ganador = 0;
                }
            }
            return (ganador, jugadaAnterior);
        }
    }
}
