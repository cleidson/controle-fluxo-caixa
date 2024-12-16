using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Infrastructure.DTOs.Mensageria.Transacao
{
    public class TransacaoMessageDto
    {
        public int UsuarioId { get; set; } 
        public decimal Valor { get; set; } 
        public DateTime DataHora { get; set; } 
        public string? Descricao { get; set; } 
        public required string Tipo { get; set; } 
    }
}
