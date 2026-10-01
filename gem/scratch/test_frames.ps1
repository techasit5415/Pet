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

# Frame 1: Refined Jump (Rising / Mid-air)
# 23 wide x 24 high
$frame1 = @(
    "  eeeee  eeeeeee  eee  ",
    " ehddchegecdadccegcdeg ",
    "ehdaaadccaaaaaaadchaaeg",
    "ecaaaaadaaaaaaaaaaddacg",
    "ecaaaaaawwaacaaacaaaadg",
    "ehaaaaaawwacegacegaaaacg",
    " edaaaaaaaahweaheaadheg",
    " eeddaaaaaihkdaihkdaeeg",
    "  ecaaaaaaahkdaihkdaeg ",
    "  ecaaaaddacgaacgadddg ",
    "  ecaaadccdaeeadeacdcg ",
    "  ehaaaaddaaaaadaaadcg ",
    "  ehdaaaaaaaaadkeaaaacg",
    "  eedaaaaaaaaadgeaaeheg",
    " eegcdaaaaaaaaabeeeaedg",
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

# Frame 2: Refined Jump (Apex / Falling - happy kicking feet & waving hands)
$frame2 = @(
    "  eeeee  eeeeeee  eee  ",
    " ehddchegecdadccegcdeg ",
    "ehdaaadccaaaaaaadchaaeg",
    "ecaaaaadaaaaaaaaaaddacg",
    "ecaaaaaawwaacaaacaaaadg",
    "ehaaaaaawwacegacegaaaacg",
    " edaaaaaaaahweaheaadheg",
    " eeddaaaaaihkdaihkdaeeg",
    "  ecaaaaaaahkdaihkdaeg ",
    "  ecaaaaddacgaacgadddg ",
    "  ecaaadccdaeeadeacdcg ",
    "  ehaaaaddaaaaadaaadcg ",
    "  ehdaaaaaaaaadkeaaaacg",
    "  eedaaaaaaaaadgeaaeheg",
    " eegcdaaaaaaaaabeeeaedg",
    " eedddaaaaaaaaaaaaadcg ",
    " eehddaaaaaaaaaaadceg  ",
    " ebbbeehddaaaaaadceg   ",
    " ebbwbbfeeheddddceg    ",
    " efbbbbffeehcccheeg    ",
    "  efffffegeehbbbfeg    ",
    "   eeeee  gfbbwbbeg    ",
    "          gefffffe     ",
    "           gggggg      "
)

$scale = 12
$bmp = New-Object System.Drawing.Bitmap (23 * 2 * $scale + 3 * $scale), (24 * $scale + 2 * $scale)
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

DrawSprite $frame1 ($scale) ($scale)
DrawSprite $frame2 (25 * $scale) ($scale)

$g.Dispose()
$bmp.Save("C:\Users\techa\.gemini\antigravity\brain\39a8bec9-7c71-4eb0-b800-7b25cdb2b6a9\jump_frames_test.png")
Write-Output "Saved jump_frames_test.png"
