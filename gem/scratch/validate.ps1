# Row 5: 23 chars
# "ehaaaaaawwacgdacgaaaacg" (or with e: "ehaaaaaawwacegacegaaaacg" -> 23 chars: "ehaaaaaawwaceacegaaaacg")
# Let's check kss_jump row 5:
# x=0..1: eh
# x=2..7: aaaaaa
# x=8..9: ww
# x=10..11: ac
# x=12..13: eg (brow above left eye)
# x=14..16: acg (cheek/space and brow above right eye)
# x=17..20: aaaa
# x=21..22: cg
# Total: 2 + 6 + 2 + 2 + 2 + 3 + 4 + 2 = 23 chars! -> "ehaaaaaawwacegacgaaaacg"

# Row 9: 23 chars
# "  ecaaabiidacgaacgadddg " had 24 chars because of 2 leading spaces + 22 chars
# "  ecaaabiacgaacgadddg  " -> let's check:
# 2 spaces + eca (3) + aabii (5) + d (1) + acg (3) + aacg (4) + adddg (5) = 23 chars!
# "  ecaaabiidacgaacgadddg" (without trailing space = 23 chars!)

$f1 = @(
    "  eeeee  eeeeeee  eee  ",
    " ehddchegecdadccegcdeg ",
    "ehdaaadccaaaaaaadchaaeg",
    "ecaaaaadaaaaaaaaaaddacg",
    "ecaaaaaawwaacaaacaaaadg",
    "ehaaaaaawwacegacgaaaacg",
    " edaaaaaaaahweaheaadheg",
    " eeddaaaaaihkdaihkdaeeg",
    "  ecaaabiiahkdaihkdaeg ",
    "  ecaaabiidacgaacgadddg",
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
    "  eefeg   gefffg       ",
    "   ggg     gggg        "
)

$f2 = @(
    "  eeeee  eeeeeee  eee  ",
    " ehddchegecdadccegcdeg ",
    "ehdaaadccaaaaaaadchaaeg",
    "ecaaaaadaaaaaaaaaaddacg",
    "ecaaaaaawwaacaaacaaaadg",
    "ehaaaaaawwacegacgaaaacg",
    " edaaaaaaaahweaheaadheg",
    " eeddaaaaaihkdaihkdaeeg",
    "  ecaaabiiahkdaihkdaeg ",
    "  ecaaabiidacgaacgadddg",
    "  ecaaadccdaeaadeacdcg ",
    "  ehaaaaddaaaaadaaadcg ",
    "  ehdaaaaaaaaaadkeaaacg",
    "  eedaaaaaaaaaadgeaaheg",
    " eegcdaaaaaaaaaadkeaedg",
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

Write-Output "F1 count: $($f1.Length)"
$err = 0
for ($i = 0; $i -lt $f1.Length; $i++) {
    $len = $f1[$i].Length
    if ($len -ne 23) {
        Write-Output "F1 row $i length is $len"
        $err++
    }
}

Write-Output "F2 count: $($f2.Length)"
for ($i = 0; $i -lt $f2.Length; $i++) {
    $len = $f2[$i].Length
    if ($len -ne 23) {
        Write-Output "F2 row $i length is $len"
        $err++
    }
}
if ($err -eq 0) {
    Write-Output "All 24 rows are EXACTLY 23 characters long!"
}
