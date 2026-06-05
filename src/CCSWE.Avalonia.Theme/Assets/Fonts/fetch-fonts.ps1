# =============================================================================
# Font acquisition for CCSWE.Avalonia.Theme  (run from this Assets/Fonts/ folder)
# PowerShell equivalent of fetch-fonts.sh — for Windows/.NET desktop devs.
# =============================================================================
# Pulls the upstream OFL variable TTFs (DM Sans + Plus Jakarta Sans) + each
# OFL.txt so the library is self-contained. Families resolve to the names
# Fonts.axaml references: "Plus Jakarta Sans" and "DM Sans".
$ErrorActionPreference = "Stop"
$base = "https://github.com/google/fonts/raw/main/ofl"

Write-Host "Fetching Plus Jakarta Sans..."
Invoke-WebRequest "$base/plusjakartasans/PlusJakartaSans%5Bwght%5D.ttf" -OutFile "PlusJakartaSans[wght].ttf"
Invoke-WebRequest "$base/plusjakartasans/OFL.txt"                       -OutFile "PlusJakartaSans-OFL.txt"

Write-Host "Fetching DM Sans..."
Invoke-WebRequest "$base/dmsans/DMSans%5Bopsz,wght%5D.ttf" -OutFile "DMSans[opsz,wght].ttf"
Invoke-WebRequest "$base/dmsans/OFL.txt"                   -OutFile "DMSans-OFL.txt"

Write-Host 'Done. Ensure the csproj includes: <AvaloniaResource Include="Assets/Fonts/*.ttf" />'
Write-Host "Keep the *-OFL.txt files in the repo to satisfy the OFL redistribution clause."
