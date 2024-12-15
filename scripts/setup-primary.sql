-- Habilitar modo de alta disponibilidade e replica��o
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'always on availability groups', 1;
RECONFIGURE;

-- Criar endpoint de replica��o
CREATE ENDPOINT [Hadr_Endpoint]
STATE=STARTED
AS TCP (LISTENER_PORT=5022)
FOR DATA_MIRRORING (ROLE=ALL, AUTHENTICATION=WINDOWS NEGOTIATE, ENCRYPTION=REQUIRED ALGORITHMS AES);

-- Configurar o modo de recupera��o FULL
ALTER DATABASE master SET RECOVERY FULL;
