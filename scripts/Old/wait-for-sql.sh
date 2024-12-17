#!/bin/bash
# Espera o SQL Server estar pronto antes de executar comandos
echo "Esperando o SQL Server estar pronto await for sql arquivo..."
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${SA_PASSWORD}" -Q "SELECT 1" > /dev/null 2>&1; do
    echo "SQL Server não está pronto. Retentando em 5 segundos..."
    sleep 5
done
echo "SQL Server está pronto!"
exec "$@"
