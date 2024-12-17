-- Habilita a replica��o
ALTER SYSTEM SET wal_level = replica;
ALTER SYSTEM SET max_wal_senders = 10;
ALTER SYSTEM SET hot_standby = on;

-- Permite conex�es de replica��o
CREATE USER replica_user REPLICATION LOGIN PASSWORD 'replica_password';
SELECT pg_reload_conf();
