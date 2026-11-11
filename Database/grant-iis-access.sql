/*
    TwoStepEncryptionDdms - IIS uygulama havuzu için veritabanı yetkisi (en az yetki)

    Uygulama IIS altında çalışırken veritabanına, uygulama havuzunun sanal hesabıyla
    (IIS APPPOOL\<havuz adı>) bağlanır. Bu script o hesaba giriş ve gereken en az yetkiyi verir.
    Önce Database\setup.sql çalıştırılmış olmalıdır (ddms şeması ve asimetrik anahtar orada oluşur).

    Kullanım:
        sqlcmd -S ".\SQLEXPRESS" -E -i Database\grant-iis-access.sql -v AppPoolName="DefaultAppPool"

    AppPoolName: IIS Manager'da uygulamanın kullandığı havuzun adı (ör. DefaultAppPool).

    Verilen yetkiler:
      - ddms şeması (kullanıcı veri kümeleri): CREATE TABLE, ALTER (DROP TABLE ve yeniden adlandırma için), SELECT, INSERT, UPDATE, DELETE
      - dbo şeması (sistem tabloları): yalnızca SELECT, INSERT, UPDATE, DELETE
      - DdmsAsymKey: veritabanı seviyesindeki RSA şifreleme ve çözme için gerekli CONTROL yetkisi
    db_owner rolü verilmez; daha önce verilmişse kaldırılır.
*/

:on error exit

USE master;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\$(AppPoolName)')
    CREATE LOGIN [IIS APPPOOL\$(AppPoolName)] FROM WINDOWS;
GO

USE TwoStepDdms;
GO

IF SCHEMA_ID(N'ddms') IS NULL
    RAISERROR(N'ddms şeması bulunamadı. Önce Database\setup.sql çalıştırılmalıdır.', 16, 1);
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'IIS APPPOOL\$(AppPoolName)')
    CREATE USER [IIS APPPOOL\$(AppPoolName)] FOR LOGIN [IIS APPPOOL\$(AppPoolName)];
GO

IF IS_ROLEMEMBER(N'db_owner', N'IIS APPPOOL\$(AppPoolName)') = 1
    ALTER ROLE db_owner DROP MEMBER [IIS APPPOOL\$(AppPoolName)];
GO

GRANT CREATE TABLE TO [IIS APPPOOL\$(AppPoolName)];
GRANT ALTER, SELECT, INSERT, UPDATE, DELETE ON SCHEMA::ddms TO [IIS APPPOOL\$(AppPoolName)];
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [IIS APPPOOL\$(AppPoolName)];
GRANT CONTROL ON ASYMMETRIC KEY::DdmsAsymKey TO [IIS APPPOOL\$(AppPoolName)];
GO

PRINT N'IIS APPPOOL\$(AppPoolName) hesabına TwoStepDdms üzerinde en az yetki verildi.';
GO
