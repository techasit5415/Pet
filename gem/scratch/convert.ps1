Add-Type -AssemblyName System.Drawing

$orig = [System.Drawing.Bitmap]::FromFile("C:\Users\techa\.gemini\antigravity\brain\39a8bec9-7c71-4eb0-b800-7b25cdb2b6a9\kss_jump.png")
$outLines = @()
for ($y = 0; $y -lt $orig.Height; $y++) {
    $line = ""
    for ($x = 0; $x -lt $orig.Width; $x++) {
        $p = $orig.GetPixel($x, $y)
        if ($p.A -eq 0) {
            $line += " "
        } else {
            $hex = "{0:X2}{1:X2}{2:X2}" -f $p.R, $p.G, $p.B
            $c = switch ($hex) {
                "F8A0E8" { "a" }
                "F070E0" { "d" }
                "E040D0" { "c" }
                "C010B0" { "h" }
                "700058" { "e" }
                "000000" { "g" }
                "F81020" { "b" }
                "C00000" { "f" }
                "F8F8F8" { "w" }
                "303030" { "k" }
                Default  { "a" }
            }
            $line += $c
        }
    }
    $outLines += ('"{0}",' -f $line)
}
$orig.Dispose()
$outLines | Set-Content "C:\Users\techa\.gemini\antigravity\brain\39a8bec9-7c71-4eb0-b800-7b25cdb2b6a9\scratch\kss_mapped.txt"
Write-Output "Written kss_mapped.txt"
