
# Arquitetura de Consolidado Diário

## **Visão Geral do Sistema**

Este projeto foi desenvolvido para atender à necessidade de um comerciante em gerenciar o fluxo de caixa diário de forma eficiente e organizada. O sistema permite o registro de lançamentos de **débito** e **crédito** ao longo do dia, além de gerar um **relatório consolidado diário**, que apresenta o saldo atualizado de maneira clara e acessível. A solução combina tecnologias modernas para oferecer alta performance, consistência nos dados e escalabilidade para atender às demandas crescentes do negócio.

## **Fluxo Operacional**

1. **Inserção de Transação**:

   - O cliente realiza uma operação (crédito ou débito) através da API de transação.
   - A API publica a transação na fila de mensagens (**Fila Transação**) e retorna o status para o cliente.

2. **Processamento da Fila**:

   - Um **Consumer** lê as mensagens da fila e insere os dados na tabela `transacoes` na base principal.

3. **Atualização da Base Réplica**:

   - A base principal replica automaticamente os dados para a **Base Réplica**, que é usada para consultas.

4. **Consulta do Consolidado Diário**:

   - Uma API de consulta acessa a base réplica para fornecer os dados do saldo consolidado diário ao cliente.

## **Componentes da Arquitetura**

### **1. API de Transação**

- **Função**: Gerencia as transações (crédito e débito) enviadas pelos clientes.
- **Responsabilidades**:
  - Publicar mensagens na fila de transação.
  - Retornar respostas imediatas ao cliente.

### **2. Message Broker**

- **Fila Transação**:
  - Centraliza todas as mensagens de crédito e débito.
  - Garante resiliência e desacoplamento entre a API e o processamento de dados.

### **3. Consumer**

- **Função**: Processa as mensagens da fila de transação.
- **Responsabilidades**:
  - Insere as transações na tabela `transacoes` na base principal.

### **4. Base Principal**

- **Função**: Armazena os dados principais do sistema, incluindo:
  - **Tabela `transacoes`**: Histórico completo de crédito e débito.
  - **Tabela `saldos_diarios`** (opcional): Saldo consolidado diário (caso seja processado incrementalmente).

### **5. Base Réplica**

- **Função**: Fornece uma cópia de somente leitura dos dados para consultas.
- **Benefícios**:
  - Reduz a carga na base principal.
  - Garante alta performance em consultas.

### **6. API de Consulta**

- **Função**: Permite que os clientes consultem:
  - O saldo consolidado diário.
  - O histórico de transações (se necessário).
- **Fonte de Dados**: Base Réplica.

## **Diagrama da Arquitetura**

O diagrama a seguir representa a arquitetura implementada:

```
Cliente (App/Web)
   |
   |---> [API de Transação]
                |
                |--> [Message Broker (Fila Transação)]
                |         |
                |         V
                |    [Consumer (Processador)]
                |         |
                |         V
                |--> [Base Principal (SQL)]
                          |
                          |
                          V
                 [Base Réplica (Somente Leitura)]
                          |
                          |
                          V
                 [API de Consulta (Saldo Consolidado)]
```

## **Fluxo de Dados**

1. **Transações (Crédito/Débito)**:
   - Registradas na API e publicadas na Fila Transação.
2. **Armazenamento**:
   - Inseridas na base principal pela lógica do Consumer.
3. **Consulta do Consolidado**:
   - Saldo e transações do dia acessados diretamente da Base Réplica.

## **Considerações Importantes**

1. **Consistência**:

   - A Base Réplica pode apresentar um pequeno atraso em relação à Base Principal devido ao processo de replicação.

2. **Escalabilidade**:

   - A fila de mensagens permite escalar horizontalmente o processamento das transações.

3. **Desempenho**:

   - Consultas ao consolidado diário são feitas na Base Réplica para evitar impacto na Base Principal.

4. **Monitoramento**:

   - Recomenda-se monitorar:
     - A fila de mensagens (tamanho, mensagens pendentes).
     - O tempo de replicação entre as bases.

## **Requisitos Técnicos**

### **Tecnologias Utilizadas**

- **API**: .NET 8 (ou outra linguagem/plataforma REST).
- **Message Broker**: RabbitMQ (ou Kafka, conforme necessidade).
- **Banco de Dados**: PostgreSQL ou Oracle (com suporte à replicação).
- **Monitoramento**: Prometheus e Grafana para logs e métricas.

### **Dependências**

- Cliente REST para interação com a API de Transações.
- Banco de dados configurado com replicador ativo (Base Réplica).

## **Execução do Projeto**

1. **Setup do Ambiente**:

   - Configure o banco de dados principal e a réplica.
   - Suba o Message Broker (RabbitMQ/Kafka).
   - Inicie o consumidor para processar as filas.

2. **Execução da API**:

   - Execute a API de Transação para registrar as transações.
   - Execute a API de Consulta para verificar o saldo consolidado.

3. **Testes**:

   - Realize testes de carga para verificar a escalabilidade do sistema.
   - Teste cenários de falhas no Message Broker e replicador de dados.

## **Contribuição**

Contribuições são bem-vindas! Siga as etapas abaixo para contribuir:

1. Faça um fork do repositório.
2. Crie um branch para a sua feature (`git checkout -b feature/nova-feature`).
3. Realize as alterações e commit (`git commit -m 'Adiciona nova feature'`).
4. Envie um pull request para revisão.

## **Licença**

Este projeto está licenciado sob a [MIT License](LICENSE).
