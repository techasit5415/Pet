$cert = (Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -eq "CN=DesktopPetDev" } | Select-Object -First 1)
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("TrustedPublisher", "CurrentUser")
$store.Open("ReadWrite")
$store.Add($cert)
$store.Close()

$rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root", "CurrentUser")
$rootStore.Open("ReadWrite")
$rootStore.Add($cert)
$rootStore.Close()

Set-AuthenticodeSignature -Certificate $cert -FilePath "C:\Users\techa\Downloads\Pet\bin\Debug\net8.0-windows\DesktopPet.dll"
Set-AuthenticodeSignature -Certificate $cert -FilePath "C:\Users\techa\Downloads\Pet\bin\Debug\net8.0-windows\DesktopPet.exe"
Set-AuthenticodeSignature -Certificate $cert -FilePath "C:\Users\techa\Downloads\Pet\publish\DesktopPet.exe"
Write-Output "Trusted and signed"
