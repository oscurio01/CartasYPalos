using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartasYPalos
{
    public enum TipoDeCarta { pica, diamante, trebol, corazon}
    public enum numeroEspecial { J, Q, K, A}
    public enum Color { Blanco, Rojo}

    internal class Carta
    {
        public string Numero { get { return numero; } }
        public TipoDeCarta TipoDeCarta {  get { return _tipoCarta; } set { _tipoCarta = value; } }
        public Color Color {get { return color; } set { color = value; } }

        private string numero;
        private Color color;
        private TipoDeCarta _tipoCarta;

        public Carta(TipoDeCarta tipoCarta, string numero)
        {
            _tipoCarta = tipoCarta;
            this.numero = numero;
        }

        public Carta(TipoDeCarta tipoCarta, numeroEspecial numeroEspecial)
        {
            _tipoCarta = tipoCarta;
            numero = numeroEspecial.ToString();
        }
    }
}
