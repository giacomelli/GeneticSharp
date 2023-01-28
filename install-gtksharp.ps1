Write-Output "Downloading installer..."
Invoke-WebRequest -uri https://github.com/mono/gtk-sharp/releases/download/2.12.45/gtk-sharp-2.12.45.msi -OutFile ./gtk-sharp-2.12.45.msi 

Write-Output "Starting installer..."
./gtk-sharp-2.12.45.msi /quiet /qn /norestart

Write-Output "Waiting for installer finish..."
Start-Sleep 10
Wait-Process -Name "msiexec" -Timeout 300

Write-Output "Done."