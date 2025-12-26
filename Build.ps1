$ErrorActionPreference = "Stop"
$frameworkPath = "C:\Windows\Microsoft.NET\Framework\v4.0.30319"
$csc = Join-Path $frameworkPath "csc.exe"

# WPF Assemblies live in a subfolder or GAC. We need to find them or point to the reference assembly path.
# Actually, strict path is `C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF` usually.

$wpfPath = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF"
if (-not (Test-Path $wpfPath)) {
    # Try finding it
    Write-Warning "WPF Path standard location not found. Searching..."
    $wpfPath = Get-ChildItem "C:\Windows\Microsoft.NET\Framework" -Filter "PresentationFramework.dll" -Recurse | Select-Object -First 1 | % { $_.DirectoryName }
}

$presCore = Join-Path $wpfPath "PresentationCore.dll"
$presFW = Join-Path $wpfPath "PresentationFramework.dll"
$winBase = Join-Path $wpfPath "WindowsBase.dll"
# System.Xaml is usually in the base framework folder for 4.0
$sysXaml = Join-Path $frameworkPath "System.Xaml.dll" 

if (-not (Test-Path $presFW)) { Write-Error "Could not locate WPF assemblies." }

$srcDir = "$PSScriptRoot\Src"
$outDir = "$PSScriptRoot\Output"
if (-not (Test-Path $outDir)) { New-Item -ItemType Directory -Path $outDir | Out-Null }

$modPsm1 = "$srcDir\PSLastOutput\PSLastOutput.psm1"
$modPsd1 = "$srcDir\PSLastOutput\PSLastOutput.psd1"
$installerCs = "$srcDir\Installer\Program.cs"
$promoVideo = "$srcDir\promo.mp4"
$outputExe = "$outDir\PSLastOutputSetup.exe"

if (Test-Path $outputExe) { Remove-Item $outputExe -Force }

Write-Host "Compiling WPF Installer using $csc..."

$compileArgs = @(
    "/nologo",
    "/target:winexe",
    "/reference:`"$presCore`"",
    "/reference:`"$presFW`"",
    "/reference:`"$winBase`"",
    "/reference:`"$sysXaml`"",
    "/reference:System.dll",
    "/out:`"$outputExe`"",
    "/resource:`"$modPsm1`",PSLastOutput.psm1",
    "/resource:`"$modPsd1`",PSLastOutput.psd1",
    "/resource:`"$promoVideo`",promo.mp4",
    "`"$installerCs`""
)

Start-Process -FilePath $csc -ArgumentList $compileArgs -Wait -NoNewWindow
if (Test-Path $outputExe) {
    Write-Host "Build Success! WPF Installer is at: $outputExe" -ForegroundColor Green
}
else {
    Write-Error "Build Failed."
}
