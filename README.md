# TwoStepEncryptionDdms

**İki Adımlı Şifreleme ile Dinamik Veri Kayıt Sistemi** (Dynamic Data Registration System with Two-Step Encryption)

> **Proje durumu:** Uygulama 2023 yılında yüksek lisans tez çalışmasının parçası olarak tamamlanmıştır. Bu repodaki sonraki commit'ler tasarım, güvenlik ve kod düzeni iyileştirmeleridir; tezin deneysel yapısı değişmemiştir.

Bu proje bir yüksek lisans tez çalışmasının uygulama bölümüdür. Kullanıcıların çalışma anında kendi veri kümelerini (tablolarını) tanımlayıp kayıt girebildiği bir web uygulamasıdır; seçilen kolonlar **iki ayrı katmanda** şifrelenerek saklanır.

## Tez ve yayın

- **Tez:** [İki adımlı şifreleme ile dinamik veri kayıt sistemi](https://tez.yok.gov.tr/UlusalTezMerkezi/TezGoster?key=nLNfCsWgUluh5T2iyudShp78VZvg2siYuLd_W0rzwHVZ-HIQ_Fh6IAEnZIKH2WJ1), Sakarya Üniversitesi, 2023 (YÖK Ulusal Tez Merkezi, Tez No: 822565)
- **Bildiri:** [Dynamic Data Masking by Two-Step Encryption](https://ieeexplore.ieee.org/abstract/document/10296545), ASYU 2023, IEEE

## Amaç

Tez, hassas verinin hem uygulama katmanında hem veritabanı katmanında şifrelenmesini ve farklı AES blok modlarının (ECB, CBC, CFB) bu yapıdaki davranışını karşılaştırmayı amaçlar. Uygulama bu karşılaştırmayı yapabilmek için parametrik yazılmıştır:

| Adım | Katman | Yöntem |
|------|--------|--------|
| 1 | Uygulama (backend) | AES-256; blok modu **ECB**, **CBC** veya **CFB** seçilebilir. Anahtarı veri kümesinin sahibi girer. |
| 2 | Veritabanı | SQL Server asimetrik anahtarı (**RSA 2048**) ile kolon bazlı şifreleme |

Her adım ayrı ayrı açılıp kapatılabildiği için tek adımlı ve iki adımlı çalışma biçimleri aynı uygulama üzerinde denenip karşılaştırılabilir. Varsayılan ayar: CFB, iki adım da açık.

## Nasıl çalışır

1. Kullanıcı bir veri kümesi (tablo) oluşturur, hangi kolonların şifreleneceğini ve 1. adım için bir anahtarı belirler.
2. Kayıt girilirken şifreli kolonlar önce uygulamada AES ile şifrelenir, ardından veritabanına yazılırken SQL Server asimetrik anahtarıyla ikinci kez şifrelenir.
3. Okurken işlem tersine yürür. Anahtar olmadan şifreli kolonlar okunamaz.
4. AES anahtarının kendisi saklanmaz; veritabanında yalnızca ondan türetilmiş bir doğrulama değeri (PBKDF2) tutulur.

## Mimari

```
Sayfalar (*.aspx)        Web Forms arayüzü; BasePage ile yetki kontrolü
   |
Services/                İş kuralları (RecordService, DatasetService, AccountService)
   |
Repositories/            Parametreli SQL erişimi (Record, Dataset, User repository'leri)
   |
Class/DataIslemleri.cs   Bağlantı ve transaction yönetimi
   |
SQL Server               TwoStepDdms veritabanı; kullanıcı veri kümeleri "ddms" şemasında
```

- `Class/Aes256Encrypter.cs`: 1. adım (AES ECB/CBC/CFB) şifreleme.
- `Security/`: kimlik doğrulama (Forms Authentication), yetkilendirme, parola hash'leme, giriş denemesi sınırlama, SQL tanımlayıcı doğrulama.
- `Database/`: kurulum ve yetki betikleri.

Teknolojiler: ASP.NET Web Forms, .NET Framework 4.8, SQL Server Express.

## Kurulum

### Gereksinimler

- Windows; Visual Studio 2022 veya üstü (ASP.NET workload)
- .NET Framework 4.8 Developer Pack
- SQL Server Express (`.\SQLEXPRESS` örneği)
- `sqlcmd` komut satırı aracı
- NuGet erişimi (`packages/` klasörü repoda yok, derleme sırasında geri yüklenir)

### Adımlar

1. Repoyu klonlayın:

   ```powershell
   git clone https://github.com/ksZehra/TwoStepEncryptionDdms.git
   cd TwoStepEncryptionDdms
   ```

2. Veritabanını kurun. Betik rastgele güçlü bir asimetrik anahtar parolası üretir, `TwoStepDdms` veritabanını oluşturur ve `TwoStepEncryptionDdms\appSettings.local.config` dosyasını yazar:

   ```powershell
   .\Database\setup.ps1
   ```

   SQL Server örneğiniz farklıysa `-Server ".\ORNEKADI"` verin. Log arayüzünü de (Seq, Docker gerekir) kurmak isterseniz komuta `-Seq` ekleyin; ayrıntı için [Loglama](#loglama) bölümüne bakın.

3. Solution'ı Visual Studio'da açıp çalıştırın. NuGet paketleri otomatik geri yüklenir.

4. Giriş yapın. Yalnızca local ortam için örnek hesaplar:

   | Kullanıcı | Parola | Yetki |
   |-----------|--------|-------|
   | `admin` | `Admin123!` | Tüm yetkiler |
   | `demo` | `Demo123!` | Silme dışındaki işlemler |

### IIS ile çalıştırma (isteğe bağlı)

Uygulamayı IIS altında `http://localhost/TwoStepEncryptionDdms` olarak yayınlayacaksanız, kurulumu uygulama havuzu adıyla çalıştırın:

```powershell
.\Database\setup.ps1 -AppPoolName "DefaultAppPool"
```

Havuza `db_owner` verilmez; yalnızca `ddms` şemasında ve gerekli tablolarda işlem yetkisi ile şifreleme anahtarı yetkisi tanımlanır.

## Yapılandırma

Şifreleme davranışı `TwoStepEncryptionDdms\Web.config` içindeki `appSettings` ile belirlenir:

| Anahtar | Değer | Anlamı |
|---------|-------|--------|
| `ECRYPTTYPE` | `ECB`, `CBC`, `CFB` | 1. adımda kullanılacak AES blok modu (varsayılan `CFB`) |
| `ISBKENCRYPT` | `1` / `0` | 1. adım (uygulama katmanı AES) açık mı |
| `ISDBENCRYPT` | `1` / `0` | 2. adım (veritabanı RSA kolon şifreleme) açık mı |
| `SQL_SERVER`, `SQL_DATABASE`, `SQL_AUTH` | | Veritabanı bağlantısı |

Gizli değerler (`DBKEY`, `DBKEYPWD`) `Web.config` içinde tutulmaz; `appSettings.local.config` dosyasından okunur ve bu dosya git'e eklenmez. `setup.ps1` kullanmak istemezseniz `appSettings.example.config` dosyasını bu adla kopyalayıp doldurabilirsiniz. Kaybedilen `DBKEYPWD` geri getirilemez; şifreli veriye erişim de kaybolur.

## Loglama

[Serilog](https://serilog.net) ile `TwoStepEncryptionDdms\App_Data\logs` altına günlük dosyalar yazılır (30 gün saklanır, git'e eklenmez). Uygulama havuzu hesabının bu klasöre yazma yetkisi olmalıdır.

- `app-YYYYMMDD.log`: uygulama olayları ve hatalar. Kullanıcıya gösterilen hata mesajındaki `Kod` bu dosyada aranır.
- `audit-YYYYMMDD.log`: giriş (başarılı, başarısız, bloklanan), çıkış, kullanıcı kaydı, şifre değişikliği, veri kümesi ve kayıt silme, yetkisiz erişim ve hatalı anahtar denemeleri.

Satır biçimi: `zaman | seviye (INF, WRN, ERR) | kullanıcı | IP | sayfa | mesaj`. Hatalarda stack trace bir sonraki satırlarda yer alır. Parola, anahtar ve kayıt içeriği loglanmaz.

### İsteğe bağlı: Seq ile arama

Seq kurmak zorunlu değildir; kurulmazsa loglar yalnızca dosyaya yazılır. Tarih, seviye, kullanıcı, sayfa ve metne göre filtrelemek için [Seq](https://datalust.co/seq) log arayüzünü Docker ile tek komutla başlatabilirsiniz (Docker Desktop gerekir; Seq'in lisans koşullarını kendiniz kontrol edin). Seq arayüzüne yalnızca onu çalıştıran makineden (`localhost`) erişilir; ağdaki başka makineler bağlanamaz. Kimlik doğrulaması kapalı olduğu için paylaşımlı bir sunucuda kullanmayın.

1. Seq'i başlatın (repo kök klasöründe):
   ```bash
   docker compose up -d
   ```
2. `TwoStepEncryptionDdms\appSettings.local.config` içine şu satırı ekleyin. `setup.ps1 -Seq` kullandıysanız bu ve önceki adım otomatik yapılır:
   ```xml
   <add key="SEQ_URL" value="http://localhost:5341" />
   ```
3. Uygulamayı yeniden başlatıp bir işlem yapın, sonra `http://localhost:5341` adresini açın.

Durdurmak için `docker compose down` (kayıtlar `seq-data` birimi silinmedikçe korunur). `SEQ_URL` boşsa ya da Seq'e ulaşılamazsa uygulama yine çalışır. Seq'te her kayıt `LogType` (`app`/`audit`), `Event`, `ErrorCode`, `User`, `Ip` ve `Page` alanlarıyla gelir; örneğin `ErrorCode = 'BED596'` veya `LogType = 'audit' and Event = 'LoginFailed'` sorgusu yazılabilir.

## Güvenlik özeti

Forms Authentication ve sunucu tarafı yetki kontrolü, parametreli sorgular ve SQL tanımlayıcı beyaz listesi, CSRF koruması, PBKDF2 ile parola hash'leme, giriş denemesi sınırlama, güvenlik başlıkları (CSP dahil) ve en az yetkili veritabanı hesabı.

## Bilinen sınırlamalar

- Araştırma ve gösterim uygulamasıdır; üretim kullanımı için tasarlanmamıştır.
- `Aes256Encrypter` tez deneyinin parçasıdır ve araştırma kodu olarak bırakılmıştır, üretim için tasarlanmamıştır: anahtar bir parola türetme fonksiyonundan (KDF) geçmeden doğrudan kullanılır ve `ECRYPTTYPE=2` dalı sabit IV kullanır.
- ECB modu karşılaştırma için bulunur; aynı düz metin aynı şifreli metni üretir, gerçek kullanımda önerilmez.
- `Web.config` içinde `debug="true"` bulunur: Smart App Control açık makinelerde dinamik derleme engellenmesin diye local ortamda böyle bırakılmıştır. Release yayınında kaldırılmalıdır.
- Örnek kullanıcı parolaları herkese açıktır; yalnızca local ortamda kullanın.
