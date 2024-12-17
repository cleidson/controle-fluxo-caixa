#!/bin/bash

# Esperar pelo SQL Server iniciar
echo "Aguardando SQL Server inicializar primary..."
RETRY_COUNT=30
SLEEP_INTERVAL=5

for ((i=1;i<=RETRY_COUNT;i++)); do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Password -Q "SELECT 1" > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "SQL Server pronto!"
        break
    fi
    echo "SQL Server não está pronto. Retentando primary em $SLEEP_INTERVAL segundos..."
    sleep $SLEEP_INTERVAL
done

if [ $i -eq $((RETRY_COUNT+1)) ]; then
    echo "Erro: SQL Server não inicializou no tempo esperado."
    exit 1
fi

# Executar o script de configuração
echo "Executando script de configuração..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Password -i /var/opt/mssql/backup/setup.sql
