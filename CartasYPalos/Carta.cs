using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartasYPalos
{
    public enum TipoDeCarta { pica, diamante, trebol, corazon}
    public enum numeroEspecial { J, Q, K, A}
    internal class Carta
    {
        public TipoDeCarta TipoDeCarta {  get { return _tipoCarta; } set { _tipoCarta = value; } }
        public string Numero { get { return numero; } }

        private TipoDeCarta _tipoCarta;
        private string numero;

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
