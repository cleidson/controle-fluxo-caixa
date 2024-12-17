#!/bin/bash

# Espera o SQL Server inicializar
echo "Aguardando SQL Server iniciar secondary..."
/usr/config/wait-for-sql.sh &

# Executa o SQL script de configuração secundária
echo "Executando setup SQL secundário..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -i /var/opt/mssql/backup/setup.sql

# Inicializa o SQL Server
echo "Inicializando o SQL Server secundário..."
exec /opt/mssql/bin/sqlservr
