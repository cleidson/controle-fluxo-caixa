#!/bin/bash

set -e

# Configuração do ambiente de replicação
echo "Configurando o Postgres secundário para replicação..."

# Limpar o diretório de dados do secundário
if [ -d "/var/lib/postgresql/data" ]; then
  echo "Limpando o diretório de dados existente..."
  rm -rf /var/lib/postgresql/data/*
fi

# Aguarda a inicialização do primário
until pg_basebackup -h postgres-primary -D /var/lib/postgresql/data -U replica_user -R -P -W; do
  echo "Aguardando o postgres-primary estar disponível..."
  sleep 5
done

echo "Backup do banco primário concluído com sucesso!"
echo "Iniciando o servidor secundário em modo standby..."

# Inicia o Postgres
exec docker-entrypoint.sh postgres
