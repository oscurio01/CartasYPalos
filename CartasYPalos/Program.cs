using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        static int cartasEnUnMazo = 2;
        static int numeroDeJugadores = 1;
        static int TurnoDelJugador = 0;
        static int Ronda = 0;
        static int dineroDeRonda = 0;
        static int dineroTotal = 0;
        static Baraja Baraja;
        static List<Carta> CartasEnLaMesa = new List<Carta>();
        static List<Jugador> Jugadores = new List<Jugador>();

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
            Console.WriteLine($"Bien hecho jugador {TurnoDelJugador + 1}, parece que tienes vida y afecto propio");
        }

        static int LeerUnNumeroCorrecto(int maximo, int minimo = 0)
        {
            int numeroCorrecto;
            while (true)
            {
                Console.Write("> ");
                if (int.TryParse(Console.ReadLine(), out numeroCorrecto) && numeroCorrecto >= minimo && numeroCorrecto <= maximo)
                    return numeroCorrecto;
                else
                    Console.WriteLine("Numero no valido");
            }
        }

        static void IniciarPartida()
        {
            Baraja = new Baraja();

            for (int i = 0; i < numeroDeJugadores; i++)
            {
                Jugadores.Add(new Jugador());
                DarCartaAJugadores(i);
            }
        }

        // Al ser todos los jugadores es necesiario enviar
        // el indice exacto del jugador para hacer la operacion
        static void DarCartaAJugadores(int i)
        {
            for (int j = 0; j < cartasEnUnMazo; j++)
            {
                Jugadores[i].AñadirCartas(cartasEnUnMazo, Baraja.Robar(TipoDeRobo.PrimeroEnLaBaraja));
            }
        }

        static void IniciarRonda()
        {
            switch (Ronda)
            {
                case 1:
                    for(int i = 0; i <= 3; i++)
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
        }

        static void Partida()
        {
            TurnoDelJugador = TurnoDelJugador % numeroDeJugadores;

            int eleccion = 0;
            Console.Clear();
            Console.Write("Las cartas en la mesa son : ");
            if(CartasEnLaMesa.Count > 0)
            {
                for (int i = 0; i < CartasEnLaMesa.Count(); i++)
                {
                    Console.Write($"|| {CartasEnLaMesa[i].TipoDeCarta} - {CartasEnLaMesa[i].Numero} ");
                }
                Console.Write("||");
            }
            Console.WriteLine();
            Console.WriteLine($"Jugador {TurnoDelJugador + 1} ");

            Jugadores[TurnoDelJugador].MostrarMano();

            Console.WriteLine($"La apuesta actual es: {dineroDeRonda}");

            Console.WriteLine(@"Que quieres hacer?
===========
1.Apostar
2.Pasar
===========");

            eleccion = LeerUnNumeroCorrecto(2, 0);
            
            if(eleccion == 1)
            {
                if (Jugadores[TurnoDelJugador].Dinero > 0 && Jugadores[TurnoDelJugador].Dinero > dineroDeRonda)
                    Apostar();
                else
                    Console.WriteLine("No tienes dinero");
            }
            else if (eleccion == 2)
                Pasar();
            else if(eleccion == 0)
            {
                Salir = true;
                return;
            }

            ++TurnoDelJugador;
            if (TurnoDelJugador == numeroDeJugadores)
                ++Ronda;

            Console.WriteLine("Pulsa Enter para continuar...");
            Console.ReadLine();
        }
        
        static void Apostar()
        {
            int eleccion;
            Console.WriteLine("Cuanto dinero quieres apostar? €{0}", Jugadores[TurnoDelJugador].Dinero);
            eleccion = LeerUnNumeroCorrecto(Jugadores[TurnoDelJugador].Dinero, dineroDeRonda);

            if (eleccion > dineroDeRonda)
                Console.WriteLine("Se ha actualizado el minimo monto a : {0}", eleccion);

            dineroDeRonda = eleccion;

            Jugadores[TurnoDelJugador].DineroApostado = eleccion;

            dineroTotal += dineroDeRonda;
            Jugadores[TurnoDelJugador].Apuesta(eleccion);
        }

        static void Pasar()
        {
            if(Ronda == 0 && dineroDeRonda == 0)
            {

            }
        }

        static void FinalDePartida()
        {
            // Cuando todos han apostado Y las cartas han salido
            // Se tiene que comparar quien de todos los jugadores es el ganador
            // Se le suma el dinero total de esa partida y se resetea tanto
            // el dinero de ronda, el dinero total y las cartas de los jugadores
            QuienGanaLaApuesta();
        }
        static int QuienGanaLaApuesta()
        {
            int ganador = 0;
            // Comprueba en cada jugador ObtenerJugada y si ese jugador no participa
            // no cuenta y se comprueba por orden de escala en TipoDeJugada
            // el que menor numero tenga gana y si hay empate se reparte

            return ganador;

        }
    }
}
