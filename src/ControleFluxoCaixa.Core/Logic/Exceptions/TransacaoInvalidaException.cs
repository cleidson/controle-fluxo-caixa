using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Core.Logic.Exceptions
{
    public class TransacaoInvalidaException : Exception
    {
        public TransacaoInvalidaException(string message) : base(message) { }
    }
}
