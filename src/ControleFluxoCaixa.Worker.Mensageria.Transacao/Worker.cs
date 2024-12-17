using ControleFluxoCaixa.Core.Logic.Enums;
using ControleFluxoCaixa.Core.Logic.Interfaces.Mensageria;
using ControleFluxoCaixa.Core.Logic.Interfaces.Services;
using ControleFluxoCaixa.Infrastructure.DTOs.Mensageria.Transacao;
using ControleFluxoCaixa.Core.Models;

namespace ControleFluxoCaixa.Worker.Mensageria.Transacao
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageQueueService _messageQueueService;
        private readonly ITransacaoService _transacaoService;

        public Worker(ILogger<Worker> logger, IMessageQueueService messageQueueService, ITransacaoService transacaoService)
        {
            _logger = logger;
            _messageQueueService = messageQueueService;
            _transacaoService = transacaoService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker iniciado às {time}", DateTimeOffset.Now);

            const int delayEntreTentativasMs = 5000; // 5 segundos de espera entre retentativas
            int tentativas = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Tentativa de conexão ao RabbitMQ (tentativa {tentativa})...", ++tentativas);

                    // Consome mensagens da fila
                    await _messageQueueService.ConsumeAsync<TransacaoMessageDto>(QueueName.FilaTransacao, async transacao =>
                    {
                        try
                        {
                            _logger.LogInformation("Mensagem recebida: {descricao} | Valor: {valor} | Tipo: {tipo}",
                                transacao.Descricao, transacao.Valor, transacao.Tipo);

                            // Processa a transação e persiste no banco
                            await _transacaoService.AddTransacaoAsync(new ControleFluxoCaixa.Core.Models.Transacao
                            {
                                UsuarioId = transacao.UsuarioId,
                                Valor = transacao.Valor,
                                Tipo = transacao.Tipo,
                                Descricao = transacao.Descricao,
                                DataHora = DateTime.Now
                            });

                            _logger.LogInformation("Transação processada com sucesso para o usuário {usuarioId}.", transacao.UsuarioId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro ao processar a transação: {descricao}", transacao.Descricao);
                            throw; // Rejeita a mensagem no RabbitMQ
                        }
                    });

                    // Se o consumo foi bem-sucedido, resetar o contador de tentativas
                    tentativas = 0;

                    // Aguarda indefinidamente até nova mensagem ou interrupção
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Worker cancelado.");
                    break;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
                {
                    _logger.LogWarning(ex, "Conexão com RabbitMQ falhou. Tentando novamente em {delayMs} ms...", delayEntreTentativasMs);
                    await Task.Delay(delayEntreTentativasMs, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado no Worker. Tentando novamente em {delayMs} ms...", delayEntreTentativasMs);
                    await Task.Delay(delayEntreTentativasMs, stoppingToken);
                }
            }

            _logger.LogInformation("Worker finalizado às {time}", DateTimeOffset.Now);
        }

    }
}
