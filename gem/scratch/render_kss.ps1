Add-Type -AssemblyName System.Drawing

$colors = @{
    [char]' ' = [System.Drawing.Color]::FromArgb(0, 0, 0, 0)
    [char]'a' = [System.Drawing.Color]::FromArgb(255, 168, 232) # body
    [char]'b' = [System.Drawing.Color]::FromArgb(232, 60, 123)  # shoes/accents
    [char]'c' = [System.Drawing.Color]::FromArgb(217, 102, 144) # lower body shadow
    [char]'d' = [System.Drawing.Color]::FromArgb(240, 139, 178) # soft edge shadow
    [char]'e' = [System.Drawing.Color]::FromArgb(168, 24, 38)   # dark red outline
    [char]'f' = [System.Drawing.Color]::FromArgb(194, 25, 87)   # shoe shadow
    [char]'g' = [System.Drawing.Color]::FromArgb(87, 0, 9)      # deepest red
    [char]'h' = [System.Drawing.Color]::FromArgb(207, 67, 118)  # pink shadow
    [char]'i' = [System.Drawing.Color]::FromArgb(255, 122, 169) # light pink blush
    [char]'w' = [System.Drawing.Color]::FromArgb(255, 255, 255) # white shine
    [char]'k' = [System.Drawing.Color]::FromArgb(0, 0, 0)       # pupil
}

$lines = Get-Content "C:\Users\techa\.gemini\antigravity\brain\39a8bec9-7c71-4eb0-b800-7b25cdb2b6a9\scratch\kss_mapped.txt" | ForEach-Object {
    $_.Trim().Trim('"').Trim(',').Trim('"')
} | Where-Object { $_.Length -gt 0 }

$scale = 12
$bmp = New-Object System.Drawing.Bitmap (23 * $scale), (24 * $scale)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::FromArgb(40, 44, 52))

for ($y = 0; $y -lt $lines.Count; $y++) {
    $line = $lines[$y]
    for ($x = 0; $x -lt $line.Length; $x++) {
        $c = $line[$x]
        if ($colors.ContainsKey($c)) {
            $col = $colors[$c]
            if ($col.A -gt 0) {
                $brush = New-Object System.Drawing.SolidBrush $col
                $g.FillRectangle($brush, $x * $scale, $y * $scale, $scale, $scale)
                $brush.Dispose()
            }
        }
    }
}
$g.Dispose()
$bmp.Save("C:\Users\techa\.gemini\antigravity\brain\39a8bec9-7c71-4eb0-b800-7b25cdb2b6a9\kss_in_palette.png")
Write-Output "Saved kss_in_palette.png"
