-- Habilitar modo de alta disponibilidade e replicação
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'always on availability groups', 1;
RECONFIGURE;

-- Criar endpoint de replicação
CREATE ENDPOINT [Hadr_Endpoint]
STATE=STARTED
AS TCP (LISTENER_PORT=5022)
FOR DATA_MIRRORING (ROLE=ALL, AUTHENTICATION=WINDOWS NEGOTIATE, ENCRYPTION=REQUIRED ALGORITHMS AES);

-- Configurar o modo de recuperação FULL
ALTER DATABASE master SET RECOVERY FULL;
