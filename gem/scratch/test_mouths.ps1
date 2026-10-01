Add-Type -AssemblyName System.Drawing

$colors = @{
    [char]' ' = [System.Drawing.Color]::FromArgb(0, 0, 0, 0)
    [char]'a' = [System.Drawing.Color]::FromArgb(255, 168, 232) # body
    [char]'b' = [System.Drawing.Color]::FromArgb(232, 60, 123)  # shoes/accents/tongue
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

# Base Jump Sprite
# 01234567890123456789012
# M1: Classic KSS vertical cute mouth :o with rosy blush cheeks
$m1 = @(
    "  eeeee  eeeeeee  eee  ",
    " ehddchegecdadccegcdeg ",
    "ehdaaadccaaaaaaadchaaeg",
    "ecaaaaadaaaaaaaaaaddacg",
    "ecaaaaaawwaacaaacaaaadg",
    "ehaaaaaawwacegacegaaaacg",
    " edaaaaaaaahweaheaadheg",
    " eeddaaaaaihkdaihkdaeeg",
    "  ecaaabiiahkdaihkdaeg ",
    "  ecaaabiidacgaacgadddg ",
    "  ecaaadccdaeaadeacdcg ",
    "  ehaaaaddaaaaadaaadcg ",
    "  ehdaaaaaaaaaadkeaaacg",
    "  eedaaaaaaaaaadgeaaheg",
    " eegcdaaaaaaaaaadkeaedg",
    " efeeeddaaaaaaaaaaadcg ",
    " ebfehddaaaaaaaaadceg  ",
    " ebbfehddaaaaaaadceg   ",
    " ebbfeehedddddddceg    ",
    " ebbwbfgeehcccheeg     ",
    " efbbbfgggggefegg      ",
    " eebbbeg  gfbbbeg      ",
    "  eefeg   gkfffg       ",
    "   ggg     gggg        "
)

# M2: Joyful open laughing mouth :D with cute tongue
$m2 = @(
    "  eeeee  eeeeeee  eee  ",
    " ehddchegecdadccegcdeg ",
    "ehdaaadccaaaaaaadchaaeg",
    "ecaaaaadaaaaaaaaaaddacg",
    "ecaaaaaawwaacaaacaaaadg",
    "ehaaaaaawwacegacegaaaacg",
    " edaaaaaaaahweaheaadheg",
    " eeddaaaaaihkdaihkdaeeg",
    "  ecaaabiiahkdaihkdaeg ",
    "  ecaaabiidacgaacgadddg ",
    "  ecaaadccdaeaadeacdcg ",
    "  ehaaaaddaaaaadaaadcg ",
    "  ehdaaaaaaaaaeekeaacg ",
    "  eedaaaaaaaaaegbkeheg ",
    " eegcdaaaaaaaaaeekeaedg",
    " efeeeddaaaaaaaaaaadcg ",
    " ebfehddaaaaaaaaadceg  ",
    " ebbfehddaaaaaaadceg   ",
    " ebbfeehedddddddceg    ",
    " ebbwbfgeehcccheeg     ",
    " efbbbfgggggefegg      ",
    " eebbbeg  gfbbbeg      ",
    "  eefeg   gkfffg       ",
    "   ggg     gggg        "
)

# M3: Cute round open mouth :O with tongue
$m3 = @(
    "  eeeee  eeeeeee  eee  ",
    " ehddchegecdadccegcdeg ",
    "ehdaaadccaaaaaaadchaaeg",
    "ecaaaaadaaaaaaaaaaddacg",
    "ecaaaaaawwaacaaacaaaadg",
    "ehaaaaaawwacegacegaaaacg",
    " edaaaaaaaahweaheaadheg",
    " eeddaaaaaihkdaihkdaeeg",
    "  ecaaabiiahkdaihkdaeg ",
    "  ecaaabiidacgaacgadddg ",
    "  ecaaadccdaeaadeacdcg ",
    "  ehaaaaddaaaaadaaadcg ",
    "  ehdaaaaaaaaaaeaaaacg ",
    "  eedaaaaaaaaaegkeaheg ",
    " eegcdaaaaaaaaaebeeaedg",
    " efeeeddaaaaaaaaaaadcg ",
    " ebfehddaaaaaaaaadceg  ",
    " ebbfehddaaaaaaadceg   ",
    " ebbfeehedddddddceg    ",
    " ebbwbfgeehcccheeg     ",
    " efbbbfgggggefegg      ",
    " eebbbeg  gfbbbeg      ",
    "  eefeg   gkfffg       ",
    "   ggg     gggg        "
)

$scale = 12
$bmp = New-Object System.Drawing.Bitmap (23 * 3 * $scale + 4 * $scale), (24 * $scale + 2 * $scale)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::FromArgb(40, 44, 52))

function DrawSprite($lines, $offsetX, $offsetY) {
    for ($y = 0; $y -lt $lines.Count; $y++) {
        $line = $lines[$y]
        for ($x = 0; $x -lt $line.Length; $x++) {
            $c = $line[$x]
            if ($colors.ContainsKey($c)) {
                $col = $colors[$c]
                if ($col.A -gt 0) {
                    $brush = New-Object System.Drawing.SolidBrush $col
                    $g.FillRectangle($brush, ($offsetX + $x * $scale), ($offsetY + $y * $scale), $scale, $scale)
                    $brush.Dispose()
                }
            }
        }
    }
}

DrawSprite $m1 ($scale) ($scale)
DrawSprite $m2 (25 * $scale) ($scale)
DrawSprite $m3 (49 * $scale) ($scale)

$g.Dispose()
$bmp.Save("C:\Users\techa\.gemini\antigravity\brain\39a8bec9-7c71-4eb0-b800-7b25cdb2b6a9\mouth_comparison.png")
Write-Output "Saved mouth_comparison.png"
