
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
  - **Tabela `Transacoes`**: Histórico completo de crédito e débito.
  - **Tabela `SaldosDiarios`**: Saldo consolidado diário (caso seja processado incrementalmente) ou a qualquer momento..

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

## Diagrama da Arquitetura modelo C4
![Diagrama de arquitetura](https://github.com/cleidson/controle-fluxo-caixa/blob/master/docs/imagens/Arquitetura-Sistema-C4.png)


## Diagrama de componentes modelo C4 - Clean Architecture
![Texto Alternativo](https://github.com/cleidson/controle-fluxo-caixa/blob/master/docs/imagens/C4-Components-Clean-Architecture.png)
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

- **API**: .NET 9 (ou outra linguagem/plataforma REST)

#### Resumo das Ferramentas utilizadas e Seu Papel

| **Ferramenta**        | **Função**                                                                                 | **URL de Acesso**                       |
|------------------------|------------------------------------------------------------------------------------------|-----------------------------------------|
| **PostgreSQL**         | Banco de dados primário e secundário configurado com replicação para garantir redundância.| `postgres-primary:5432` e `postgres-secondary:5433` |
| **RabbitMQ**           | Sistema de mensageria assíncrona para processamento de eventos e comunicação entre serviços. | [http://localhost:15672](http://localhost:15672)   |
| **Prometheus**         | Coleta e armazena métricas de performance e saúde do sistema.                            | [http://localhost:9090](http://localhost:9090)     |
| **Grafana**            | Exibe métricas coletadas pelo Prometheus em dashboards visuais e informativos.           | [http://localhost:3000](http://localhost:3000)     |
| **Weave Scope**        | Visualização em tempo real dos contêineres Docker e da infraestrutura.                   | [http://localhost:4040](http://localhost:4040)     |
| **Portainer**          | Interface gráfica para gerenciar contêineres Docker, redes, volumes e imagens.           | [https://localhost:9443](https://localhost:9443)   |



### **Dependências**

- Cliente REST para interação com a API de Transações.
- Banco de dados configurado com replicador ativo (Base Réplica).

## **Execução do Projeto**

1. **Setup do Ambiente**:

#### **Requisitos para Rodar o Projeto**

1. **Visual Studio 2022**
   - Certifique-se de ter o Visual Studio 2022 instalado.
   - O **projeto Docker Compose** deve ser configurado como **Set as Startup Project**.

2. **Docker Desktop para Windows**
   - Versão necessária: **4.33.1 (161083)**
   - Baixe e instale o Docker Desktop a partir do link oficial:
     [Docker Desktop para Windows](https://docs.docker.com/desktop/setup/install/windows-install/)

### **Configuração do Projeto**

1. **Passos Iniciais**:
   - Clone o repositório:
     ```bash
     git clone <URL_DO_REPOSITORIO>
     cd <NOME_DO_PROJETO>
     ```

2. **Configuração no Visual Studio**:
   - Abra a solução no **Visual Studio 2022**.
   - Defina o **`docker-compose`** como **Startup Project**:
     - Clique com o botão direito no projeto `docker-compose` → **Set as Startup Project**.

3. **Verifique o Docker**:
   - Certifique-se de que o **Docker Desktop** está rodando na versão **4.33.1**.
   - Confirme com o comando:
     ```bash
     docker --version
     ```

4. **Inicie a Aplicação**:
   - Rode o projeto pelo Visual Studio (**F5** ou **Ctrl+F5**).
   - O Docker Compose irá subir todos os serviços necessários.

---

#### **Documentação Adicional**
- **Docker Desktop Setup**: [Instalação do Docker no Windows](https://docs.docker.com/desktop/setup/install/windows-install/)
- **Visual Studio 2022**: [Download Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/downloads/)

---

#### **Acesso aos Serviços**

| **Serviço**            | **URL de Acesso**                        |
|-------------------------|-----------------------------------------|
| **API (HTTP)**          | [http://localhost:8080](http://localhost:8080) |
| **RabbitMQ**            | [http://localhost:15672](http://localhost:15672) |
| **Prometheus**          | [http://localhost:9090](http://localhost:9090)   |
| **Grafana**             | [http://localhost:3000](http://localhost:3000)   |
| **Weave Scope**         | [http://localhost:4040](http://localhost:4040)   |
| **Portainer**           | [https://localhost:9443](https://localhost:9443) |

---

### **Credenciais Padrão**
- **RabbitMQ**:  
   - Usuário: `guest`  
   - Senha: `guest`  

- **Grafana**:  
   - Usuário: `admin`  
   - Senha: `admin`  .

2. **Execução da API**:

   - Execute a API de Transação para registrar as transações.
   - Execute a API de Consulta para verificar o saldo consolidado (api/SaldoDiario/{data}).

### Melhorias e Evoluções Futuras

Este projeto pode ser aprimorado com as seguintes evoluções:

1. **Implementação de Autenticação e Autorização**  
   - Garantir maior segurança e controle de acesso às APIs, utilizando padrões modernos como **JWT** (JSON Web Token) ou **OAuth 2.0**.

2. **Integração de API Gateway (Kong)**  
   - Implementar o **Kong API Gateway** para:
     - Gerenciamento centralizado das APIs.
     - Aplicação de políticas de segurança, como **autenticação** e **autorização**.
     - Definição de regras de **rate limiting** para controle de tráfego.

3. **Implementação de Rate Limit**  
   - Configurar **rate limiting** para evitar abusos e sobrecarga das APIs, garantindo melhor escalabilidade e resiliência do sistema.

4. **Monitoramento e Observabilidade Aprimorados**  
   - Adicionar mais métricas para os serviços utilizando o **Prometheus**.
   - Criar dashboards no **Grafana** para acompanhar logs e desempenho em tempo real.
   - Uso de elastic e APM para monitoramento de logs.

---

### Justificativas para o Custo do Projeto

1. **Alta Disponibilidade com Replicação de Banco de Dados**  
   - A utilização de uma **base de dados com réplica** garante maior resiliência em caso de falhas, sendo fundamental para **disaster recovery** e continuidade do serviço.

2. **Escalabilidade e Performance**  
   - A estrutura preparada para integração com **API Gateway** e **rate limiting** proporciona um ambiente escalável e protegido, justificando a robustez do sistema para cenários de alta demanda.

3. **Manutenção Simplificada**  
   - A separação em serviços e o uso de contêineres facilitam a **manutenção** e **evolução** do projeto ao longo do tempo.

4. **Segurança e Controle**  
   - A implementação de **autenticação** e **autorização** garante que o acesso às APIs seja restrito e monitorado, aumentando a segurança e protegendo os dados sensíveis do sistema.

---

### Próximos Passos
- Adicionar um **load balancer** para balanceamento de carga das APIs.
- Implementar **cache** (ex.: Redis) para otimizar o desempenho em consultas frequentes.
---

Essas evoluções tornam o sistema mais **robusto**, **escalável** e **seguro**, preparando-o para operar em ambientes complexos e de alta demanda.


## **Licença**

Este projeto está licenciado sob a [MIT License](LICENSE).
