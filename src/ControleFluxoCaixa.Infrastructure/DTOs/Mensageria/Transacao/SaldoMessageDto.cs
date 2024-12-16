using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Infrastructure.DTOs.Mensageria.Transacao
{
    public class SaldoMessageDto
    {
        public int UsuarioId { get; set; }
        public decimal SaldoAtual { get; set; }
        public DateTime DataHora { get; set; }
    }
}
