/*
    TwoStepEncryptionDdms - Veritabanı kurulum script'i

    Kullanım (SQL Server Express):
        sqlcmd -S ".\SQLEXPRESS" -E -f 65001 -i Database\setup.sql -v DbKeyPassword="<asimetrik anahtar şifresi>"

    - "-f 65001": Dosyanın UTF-8 olarak okunmasını sağlar. Türkçe karakterlerin bozulmaması için gereklidir.

    - DbKeyPassword: 2. katman (veritabanı) şifrelemesinde kullanılan asimetrik anahtarın şifresi.
      TwoStepEncryptionDdms\appSettings.local.config içindeki DBKEYPWD değeri bu parolayla aynı olmalıdır.
      Kolay yol: Database\setup.ps1 rastgele güçlü bir parola üretir, bu script'i çalıştırır ve local config dosyasını oluşturur.
    - Script tekrar çalıştırılabilir; var olan nesneleri yeniden oluşturmaz.
    - Kullanıcı veri kümeleri ddms şemasında tutulur. Eski kurulumdaki (dbo'da duran) kayıtlı veri kümeleri
      bu script yeniden çalıştırıldığında ddms'e taşınır. Uygulama hesabının yetkileri grant-iis-access.sql ile verilir.

    Parolalar uygulama tarafında PBKDF2 (salt'lı) ile hash'lenir. T-SQL ile PBKDF2 üretilemediği için
    örnek kullanıcılar eski MD5 formatında eklenir; uygulama bu hash'i ilk başarılı girişte otomatik olarak
    PBKDF2 formatına taşır.

    Örnek kullanıcılar (yalnızca local geliştirme ve test içindir):
        admin  / Admin123!   -> Yönetici profili, tüm yetkiler
        demo   / Demo123!    -> Standart kullanıcı profili (Register ekranından kayıt olanlar da bu profili alır)
*/

:on error exit

IF DB_ID(N'TwoStepDdms') IS NULL
    CREATE DATABASE TwoStepDdms;
GO

USE TwoStepDdms;
GO

/* ---------- Profil ve permission'lar ---------- */

IF OBJECT_ID(N'dbo.TBL_PROFIL', N'U') IS NULL
CREATE TABLE dbo.TBL_PROFIL
(
    PROFILKODU  VARCHAR(20)   NOT NULL CONSTRAINT PK_TBL_PROFIL PRIMARY KEY,
    PROFILADI   NVARCHAR(100) NOT NULL
);
GO

IF OBJECT_ID(N'dbo.TBL_PROFILSEC', N'U') IS NULL
CREATE TABLE dbo.TBL_PROFILSEC
(
    ID          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TBL_PROFILSEC PRIMARY KEY,
    PROFILKODU  VARCHAR(20) NOT NULL CONSTRAINT FK_TBL_PROFILSEC_PROFIL REFERENCES dbo.TBL_PROFIL(PROFILKODU),
    LIST        BIT NOT NULL CONSTRAINT DF_TBL_PROFILSEC_LIST   DEFAULT (0),
    NEW         BIT NOT NULL CONSTRAINT DF_TBL_PROFILSEC_NEW    DEFAULT (0),
    EDIT        BIT NOT NULL CONSTRAINT DF_TBL_PROFILSEC_EDIT   DEFAULT (0),
    [VIEW]      BIT NOT NULL CONSTRAINT DF_TBL_PROFILSEC_VIEW   DEFAULT (0),
    [DELETE]    BIT NOT NULL CONSTRAINT DF_TBL_PROFILSEC_DELETE DEFAULT (0)
);
GO

/* ---------- Kullanıcılar ---------- */

IF OBJECT_ID(N'dbo.TBL_USERS', N'U') IS NULL
CREATE TABLE dbo.TBL_USERS
(
    KID           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TBL_USERS PRIMARY KEY,
    KAD           NVARCHAR(50)  NULL,
    KSOYAD        NVARCHAR(50)  NULL,
    KEMAIL        NVARCHAR(250) NOT NULL,
    KULLANICIADI  NVARCHAR(100) NOT NULL CONSTRAINT UQ_TBL_USERS_KULLANICIADI UNIQUE,
    KSIFRE        VARCHAR(255)  NOT NULL,   -- PBKDF2$SHA256$iterasyon$salt$hash (uygulama üretir)
    KDURUM        CHAR(1)       NOT NULL CONSTRAINT DF_TBL_USERS_KDURUM DEFAULT ('1'),
    PROFILKODU    VARCHAR(20)   NOT NULL CONSTRAINT FK_TBL_USERS_PROFIL REFERENCES dbo.TBL_PROFIL(PROFILKODU)
);
GO

IF COL_LENGTH(N'dbo.TBL_USERS', N'KSIFRE') < 255
    ALTER TABLE dbo.TBL_USERS ALTER COLUMN KSIFRE VARCHAR(255) NOT NULL;
GO

/* ---------- Dinamik veri kümeleri ---------- */

-- Kullanıcıların oluşturduğu her veri kümesi (table) ve 1. katman (AES) key'i
IF OBJECT_ID(N'dbo.TBL_USERAUTHORIZATIONS', N'U') IS NULL
CREATE TABLE dbo.TBL_USERAUTHORIZATIONS
(
    TBLID             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TBL_USERAUTHORIZATIONS PRIMARY KEY,
    TBLAD             NVARCHAR(128) NOT NULL CONSTRAINT UQ_TBL_USERAUTHORIZATIONS_TBLAD UNIQUE,
    KEYVERIFIER       NVARCHAR(255) NULL,   -- 1. katman (AES) key'inin PBKDF2 verification değeri; key'in kendisi saklanmaz
    OLUSTURANID       INT           NOT NULL,
    OLUSTURAN         NVARCHAR(101) NULL,
    OLUSTURMATARIHI   DATETIME      NOT NULL,
    GUNCELLEYENID     INT           NULL,
    GUNCELLEYEN       NVARCHAR(101) NULL,
    GUNCELLEMETARIHI  DATETIME      NULL
);
GO

-- Eski kurulumlar: SYMETRICKEY column'u KEYVERIFIER olur; plain text key'ler ilk doğru girişte verification değerine dönüştürülür.
IF COL_LENGTH(N'dbo.TBL_USERAUTHORIZATIONS', N'SYMETRICKEY') IS NOT NULL
   AND COL_LENGTH(N'dbo.TBL_USERAUTHORIZATIONS', N'KEYVERIFIER') IS NULL
    EXEC sp_rename N'dbo.TBL_USERAUTHORIZATIONS.SYMETRICKEY', N'KEYVERIFIER', N'COLUMN';
GO

IF COL_LENGTH(N'dbo.TBL_USERAUTHORIZATIONS', N'KEYVERIFIER') < 510
    ALTER TABLE dbo.TBL_USERAUTHORIZATIONS ALTER COLUMN KEYVERIFIER NVARCHAR(255) NULL;
GO

-- Veri kümelerinde encrypt edilecek column'lar
IF OBJECT_ID(N'dbo.TBL_SIFRELIKOLONLAR', N'U') IS NULL
CREATE TABLE dbo.TBL_SIFRELIKOLONLAR
(
    ID        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TBL_SIFRELIKOLONLAR PRIMARY KEY,
    TBLAD     NVARCHAR(128) NOT NULL,
    KOLONADI  NVARCHAR(128) NOT NULL
);
GO

/* ---------- Kullanıcı veri kümeleri schema'sı ---------- */

IF SCHEMA_ID(N'ddms') IS NULL
    EXEC(N'CREATE SCHEMA ddms');
GO

-- Eski kurulumlar: TBL_USERAUTHORIZATIONS'ta kayıtlı ve dbo'da duran veri kümesi table'ları ddms schema'sına taşınır.
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += N'ALTER SCHEMA ddms TRANSFER dbo.' + QUOTENAME(t.name) + N';'
FROM sys.tables t
INNER JOIN dbo.TBL_USERAUTHORIZATIONS a ON a.TBLAD COLLATE DATABASE_DEFAULT = t.name COLLATE DATABASE_DEFAULT
WHERE t.schema_id = SCHEMA_ID(N'dbo') AND t.name NOT LIKE N'TBL[_]%';
IF LEN(@sql) > 0 EXEC(@sql);
GO

/* ---------- 2. katman: database asymmetric key ---------- */

IF ASYMKEY_ID(N'DdmsAsymKey') IS NULL
    CREATE ASYMMETRIC KEY DdmsAsymKey
        WITH ALGORITHM = RSA_2048
        ENCRYPTION BY PASSWORD = N'$(DbKeyPassword)';
GO

/* ---------- Başlangıç verileri ---------- */

IF NOT EXISTS (SELECT 1 FROM dbo.TBL_PROFIL WHERE PROFILKODU = 'admin')
    INSERT INTO dbo.TBL_PROFIL (PROFILKODU, PROFILADI) VALUES ('admin', N'Y' + NCHAR(246) + N'netici');  -- Yönetici

-- 'nyk': Register ekranından kayıt olan kullanıcıların varsayılan profili
IF NOT EXISTS (SELECT 1 FROM dbo.TBL_PROFIL WHERE PROFILKODU = 'nyk')
    INSERT INTO dbo.TBL_PROFIL (PROFILKODU, PROFILADI) VALUES ('nyk', N'Standart Kullan' + NCHAR(305) + N'c' + NCHAR(305));  -- Standart Kullanıcı

IF NOT EXISTS (SELECT 1 FROM dbo.TBL_PROFILSEC WHERE PROFILKODU = 'admin')
    INSERT INTO dbo.TBL_PROFILSEC (PROFILKODU, LIST, NEW, EDIT, [VIEW], [DELETE]) VALUES ('admin', 1, 1, 1, 1, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.TBL_PROFILSEC WHERE PROFILKODU = 'nyk')
    INSERT INTO dbo.TBL_PROFILSEC (PROFILKODU, LIST, NEW, EDIT, [VIEW], [DELETE]) VALUES ('nyk', 1, 1, 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM dbo.TBL_USERS WHERE KULLANICIADI = N'admin')
    INSERT INTO dbo.TBL_USERS (KAD, KSOYAD, KEMAIL, KULLANICIADI, KSIFRE, KDURUM, PROFILKODU)
    VALUES (N'Sistem', N'Y' + NCHAR(246) + N'neticisi', N'admin@example.com', N'admin',  -- Yöneticisi
            LOWER(CONVERT(VARCHAR(32), HASHBYTES('MD5', 'Admin123!'), 2)), '1', 'admin');

IF NOT EXISTS (SELECT 1 FROM dbo.TBL_USERS WHERE KULLANICIADI = N'demo')
    INSERT INTO dbo.TBL_USERS (KAD, KSOYAD, KEMAIL, KULLANICIADI, KSIFRE, KDURUM, PROFILKODU)
    VALUES (N'Demo', N'Kullan' + NCHAR(305) + N'c' + NCHAR(305), N'demo@example.com', N'demo',  -- Kullanıcı
            LOWER(CONVERT(VARCHAR(32), HASHBYTES('MD5', 'Demo123!'), 2)), '1', 'nyk');
GO

PRINT N'TwoStepDdms kurulumu tamamlandi.';
GO
