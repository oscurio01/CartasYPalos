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
        static bool PasarDeRonda = true;
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
            Console.WriteLine($"Bien hecho jugador {TurnoDelJugador + 1}, parece que tienes vida y afecto propio");
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

        // Al ser todos los jugadores es necesiario enviar
        // el indice exacto del jugador para hacer la operacion
        static void DarCartaAJugadores(int i)
        {
            for (int j = 0; j < cartasEnUnMazo; j++)
            {
                Jugadores[i].AñadirCartas(Baraja.Robar(TipoDeRobo.PrimeroEnLaBaraja));
            }
        }

        static void IniciarRonda()
        {
            if(TurnoDelJugador != 0 && TurnoDelJugador != numeroDeJugadores)
                return;
            if(!PasarDeRonda)
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
            }
        }

        static void Partida()
        {
            int eleccion = 0;

            TurnoDelJugador = TurnoDelJugador % numeroDeJugadores;

            bool unJugadorApostado = DiccionarioDeApuestas.Values.Any(apuesta => apuesta > 0);

            // Si casi los jugadores abandonan, gana el ultimo en pie
            if (Jugadores.Select(c => c.SeRetira).Count(v => v == true) == numeroDeJugadores - 1)
            {
                Salir = true;
                return;
            }

            if (Jugadores[TurnoDelJugador].SeRetira || Jugadores[TurnoDelJugador].EstaApostando)
            {
                ++TurnoDelJugador;
                if (TurnoDelJugador == numeroDeJugadores)
                    ++Ronda;
                return;
            }

            Console.Clear();
            Console.WriteLine("Ronda : {0}", Ronda);
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

            Console.WriteLine($"La apuesta actual es de: {dineroDeRonda} y el total : {dineroTotal}");

            Console.WriteLine(@"Que quieres hacer?
===========
1.Apostar
2.Pasar
3.Retirarse
===========");


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
                    if (!unJugadorApostado)
                        ++TurnoDelJugador;
                    else
                        Console.WriteLine("No puedes pasar hay una apuesta, apuestas o te retiras");
                    break;
                case 3:
                    Retirarse();
                    break;
                case 0:
                    Salir = true;
                    return;
            }

            bool todosApostaron = DiccionarioDeApuestas.Values.All(apuesta => apuesta > 0);

            if(!todosApostaron && unJugadorApostado)
            {
                if (Jugadores[TurnoDelJugador].EstaApostando)
                {
                    for(int i = 0; i < numeroDeJugadores; i++)
                    {
                        if (DiccionarioDeApuestas[Jugadores[i]] == 0)
                        {
                            TurnoDelJugador = Jugadores.IndexOf(Jugadores[i]);
                            break;
                        }
                    }

                }
                else if(!Jugadores[TurnoDelJugador].EstaApostando)
                {

                }
                else
                {
                    ++TurnoDelJugador;
                }
            }
            else if (todosApostaron)
            {
                ++TurnoDelJugador;
            }

            if (TurnoDelJugador == numeroDeJugadores)
                ++Ronda;

            Console.WriteLine("Pulsa Enter para continuar...");
            Console.ReadLine();
        }
        
        static void Apostar()
        {
            int eleccion;
            Console.WriteLine("Cuanto dinero quieres apostar? €{0}", Jugadores[TurnoDelJugador].Dinero);
            eleccion = LeerUnNumeroCorrecto(Jugadores[TurnoDelJugador].Dinero, dineroDeRonda, "Eso no es un numero o no llega al monto minimo");

            if (eleccion > dineroDeRonda)
                Console.WriteLine("Se ha actualizado el minimo monto a : {0}", eleccion);

            dineroDeRonda = eleccion;

            DiccionarioDeApuestas[Jugadores[TurnoDelJugador]] = eleccion;

            dineroTotal += dineroDeRonda;
            Jugadores[TurnoDelJugador].Apuesta(eleccion);
        }


        static void Retirarse()
        {
            Jugadores[TurnoDelJugador].SeRetira = true;
        }

        static void FinalDePartida()
        {
            // Cuando todos han apostado Y las cartas han salido
            // Se tiene que comparar quien de todos los jugadores es el ganador
            // Se le suma el dinero total de esa partida y se resetea tanto
            // el dinero de ronda, el dinero total y las cartas de los jugadores
            TurnoDelJugador = 0;

            foreach (var jugador in Jugadores)
            {

                // Escoges que cartas de la mesa quieres usar
                Jugadores[TurnoDelJugador].AñadirCartas(SeleccionarCartas(3));
                Jugadores[TurnoDelJugador].MostrarMano();
                Console.ReadLine();

                TurnoDelJugador++;
            }
            

            QuienGanaLaApuesta();
        }

        static List<Carta> SeleccionarCartas(int cantidad)
        {
            int eleccion = 0;

            List<Carta> seleccionadas = new List<Carta>();

            // Usamos un HashSet para evitar repetidos
            HashSet<int> indicesYaSeleccionados = new HashSet<int>();

            Console.Clear();
            Console.Write("Las cartas en la mesa son : ");
            if (CartasEnLaMesa.Count > 0)
            {
                for (int j = 0; j < CartasEnLaMesa.Count(); j++)
                {
                    Console.Write($"|| {CartasEnLaMesa[j].TipoDeCarta} - {CartasEnLaMesa[j].Numero} ");
                }
                Console.Write("||");
            }
            Console.WriteLine();

            Console.WriteLine("Por favor Jugador {0} escribe las tres cartas de la mesa que vas a usar", TurnoDelJugador + 1);

            while (seleccionadas.Count < cantidad)
            {
                eleccion = LeerUnNumeroCorrecto(CartasEnLaMesa.Count(), 1, "Esa carta no existe");
                eleccion--;

                if (indicesYaSeleccionados.Contains(eleccion))
                {
                    Console.WriteLine("Ya has seleccionado esa carta, elige otra.");
                }
                else
                {
                    // Añade la carta a la baraja personal 
                    seleccionadas.Add(CartasEnLaMesa[eleccion]);
                    // Se añade al indice para evitar repeticiones
                    indicesYaSeleccionados.Add(eleccion);

                    Console.WriteLine($"Has seleccionado: {CartasEnLaMesa[eleccion].TipoDeCarta} - {CartasEnLaMesa[eleccion].Numero}");
                }
            }

            return seleccionadas;
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
