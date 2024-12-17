#!/bin/bash

echo "Iniciando Log Shipping..."

while true; do
  echo "[BACKUP LOG] Realizando backup no servidor primário"
  /opt/mssql-tools/bin/sqlcmd -S sql-primary -U sa -P 'YourStrong!Password' -Q "
    BACKUP LOG ControleFluxoCaixa
    TO DISK = '/var/opt/shared-data/ControleFluxoCaixa_log.trn'
    WITH INIT, COMPRESSION;
  "

  echo "[RESTORE LOG] Restaurando log no servidor secundário"
  /opt/mssql-tools/bin/sqlcmd -S sql-secondary -U sa -P 'YourStrong!Password' -Q "
    RESTORE LOG ControleFluxoCaixa
    FROM DISK = '/var/opt/shared-data/ControleFluxoCaixa_log.trn'
    WITH NORECOVERY;
  "

  sleep 60
done
