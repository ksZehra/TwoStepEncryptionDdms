<#
    TwoStepEncryptionDdms - Kurulum betiği

    Yaptığı işler:
      1. 2. katman (veritabanı) asimetrik anahtarı için rastgele güçlü bir parola üretir (veya -DbKeyPassword ile verileni kullanır)
      2. Database\setup.sql dosyasını çalıştırarak veritabanını kurar
      3. TwoStepEncryptionDdms\appSettings.local.config dosyasını oluşturur (git'e eklenmez)
      4. -AppPoolName verilirse IIS uygulama havuzuna veritabanı yetkisi verir (grant-iis-access.sql)

    Örnek:
      .\Database\setup.ps1
      .\Database\setup.ps1 -Server ".\SQLEXPRESS" -AppPoolName "DefaultAppPool"

    Not: Gerçek dosya zaten varsa üzerine yazılmaz (parola kaybolursa şifreli veriye erişilemez); -Force ile yeniden oluşturulabilir.
#>
param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$AppPoolName = "",
    [string]$DbKeyPassword = "",
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$localConfig = Join-Path $root 'TwoStepEncryptionDdms\appSettings.local.config'
$setupSql = Join-Path $PSScriptRoot 'setup.sql'
$grantSql = Join-Path $PSScriptRoot 'grant-iis-access.sql'

if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    throw "sqlcmd bulunamadı. SQL Server komut satırı araçlarını (sqlcmd) kurun."
}

if ((Test-Path $localConfig) -and -not $Force) {
    throw "appSettings.local.config zaten var: $localConfig`nMevcut parolayı kaybetmemek için üzerine yazılmadı. Yeniden oluşturmak için -Force kullanın."
}

$checkOutput = & sqlcmd -S $Server -E -h -1 -W -Q "SET NOCOUNT ON; IF DB_ID(N'TwoStepDdms') IS NULL SELECT 0 ELSE SELECT COUNT(*) FROM TwoStepDdms.sys.asymmetric_keys WHERE name = N'DdmsAsymKey'"
if ($LASTEXITCODE -ne 0) { throw "SQL Server'a bağlanılamadı: $Server" }
$keyExists = ($checkOutput | Select-Object -First 1)

if ($keyExists -eq '1' -and [string]::IsNullOrWhiteSpace($DbKeyPassword)) {
    throw "Veritabanında 'DdmsAsymKey' zaten var. Parolasını biliyorsanız -DbKeyPassword ile verin; bilmiyorsanız veritabanını silip yeniden kurun."
}

if ([string]::IsNullOrWhiteSpace($DbKeyPassword)) {
    # Yalnızca harf ve rakam: sqlcmd değişkeni ve XML içinde güvenle taşınır. 40 karakter ~ 230 bit.
    $chars = 'ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789'.ToCharArray()
    $limit = 256 - (256 % $chars.Length)
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    $buffer = New-Object byte[] 1
    $sb = New-Object System.Text.StringBuilder
    while ($sb.Length -lt 40) {
        $rng.GetBytes($buffer)
        if ($buffer[0] -lt $limit) { [void]$sb.Append($chars[$buffer[0] % $chars.Length]) }
    }
    $rng.Dispose()
    $DbKeyPassword = $sb.ToString()
}

# Parola önce config dosyasına yazılır: kurulum yarıda kalsa bile key oluşmuşsa parola kaybolmaz.
$xml = @"
<?xml version="1.0" encoding="utf-8"?>
<appSettings>
  <add key="DBKEY" value="DdmsAsymKey" />
  <add key="DBKEYPWD" value="$DbKeyPassword" />
  <add key="SQL_SERVER" value="$Server" />
</appSettings>
"@
[System.IO.File]::WriteAllText($localConfig, $xml, (New-Object System.Text.UTF8Encoding($false)))
Write-Host "Local ayar dosyası oluşturuldu: $localConfig (git'e eklenmez, kimseyle paylaşmayın)"

Write-Host "Veritabanı kuruluyor ($Server)..."
& sqlcmd -S $Server -E -f 65001 -i $setupSql -v DbKeyPassword="$DbKeyPassword"
if ($LASTEXITCODE -ne 0) { throw "setup.sql çalıştırılamadı. Gerekirse düzeltip betiği -Force ile yeniden çalıştırın." }

if (-not [string]::IsNullOrWhiteSpace($AppPoolName)) {
    & sqlcmd -S $Server -E -f 65001 -i $grantSql -v AppPoolName="$AppPoolName"
    if ($LASTEXITCODE -ne 0) { throw "grant-iis-access.sql çalıştırılamadı." }
}

Write-Host "Tamam. Örnek kullanıcılar: admin / Admin123!  ve  demo / Demo123!  (yalnızca local ortamda kullanın)"
