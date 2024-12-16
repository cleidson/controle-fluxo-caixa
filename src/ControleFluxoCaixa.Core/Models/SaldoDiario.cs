using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Core.Models
{
    public class SaldoDiario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public decimal SaldoAtual { get; set; }
        public DateTime DataHora { get; set; }
    }
}
