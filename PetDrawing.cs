using System.Windows;
using System.Windows.Media;

namespace DesktopPet;

// Pixel colors and silhouette are sampled from the reference image. Spaces are
// transparent; the black cells inside the eyes are filled explicitly below.
sealed class PetDrawing : FrameworkElement {
    const int SpriteWidth = 19;
    const int SpriteHeight = 16;
    static readonly string[] Sprite = {
        "       ebbe        ",
        "     idaaaadb      ",
        "    daaaaaaaadg    ",
        "   iaaaaebaeiad    ",
        "   aaaaawgawgaae   ",
        "  iaaaaae ab aab   ",
        " badaaaab ad aad   ",
        "gaaaaabbd aa bdadg ",
        "aaaaaaaaaaaaaaadda ",
        "cadcdaaaaaaeaaadcc ",
        "eccccdaaaaaaaaahhg ",
        "  ghccdaaaaaaaaeg  ",
        "  ffhcccdddccche   ",
        " bwbbbhccccccebbb  ",
        "ebbbbbfhccchefbbbg ",
        "effffffe  eeeffffg "
    };
    static readonly Dictionary<char, Brush> Palette = new() {
        ['a'] = Frozen(255, 168, 232), // body
        ['b'] = Frozen(232, 60, 123),  // bright shoes and accents
        ['c'] = Frozen(217, 102, 144), // lower body shadow
        ['d'] = Frozen(240, 139, 178), // soft edge shadow
        ['e'] = Frozen(168, 24, 38),   // dark red
        ['f'] = Frozen(194, 25, 87),   // shoe shadow
        ['g'] = Frozen(87, 0, 9),      // deepest red
        ['h'] = Frozen(207, 67, 118),  // pink shadow
        ['i'] = Frozen(255, 122, 169), // light pink accent
        ['w'] = Frozen(255, 255, 255), // eye and shoe shine
        ['k'] = Frozen(0, 0, 0)        // eye pupil
    };

    PetState state;
    double phase;
    int direction = 1;
    int pixelSize = 2;

    internal int PixelSize {
        get => pixelSize;
        set { pixelSize = Math.Clamp(value, 1, 8); InvalidateVisual(); }
    }

    static Brush Frozen(byte r, byte g, byte b) {
        var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
        brush.Freeze();
        return brush;
    }

    public PetDrawing() => RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

    internal void Update(PetState s, double p, int d) {
        state = s;
        phase = p;
        direction = d;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc) {
        base.OnRender(dc);

        double cell = pixelSize * 1.5;
        double originX = Math.Round((34 * pixelSize - SpriteWidth * cell) / 2);
        double originY = 35 * pixelSize - SpriteHeight * cell;
        int visibleRows = state == PetState.Sleep ? 10 : state == PetState.Sit ? 13 : SpriteHeight;
        originY += (SpriteHeight - visibleRows) * cell;
        if (state == PetState.Walking && Math.Sin(phase * 12) > 0.5) originY -= cell;
        if (state == PetState.React) originY -= Math.Abs(Math.Sin(phase * 10)) > 0.6 ? cell : 0;

        double centerX = 17 * pixelSize;
        dc.PushTransform(new ScaleTransform(direction, 1, centerX, 0));

        bool blink = state == PetState.Sleep || state == PetState.React || (int)(phase * 10) % 47 == 0;
        bool leftStep = (int)(phase * 8) % 2 == 0;
        for (int pass = 0; pass < 2; pass++) {
            for (int y = 0; y < visibleRows; y++) {
                int sourceY = y * (SpriteHeight - 1) / (visibleRows - 1);
                for (int x = 0; x < SpriteWidth; x++) {
                    char color = Sprite[sourceY][x];
                    if ((x == 9 || x == 12) && sourceY >= 5 && sourceY <= 7) color = 'k';
                    if (blink && x >= 8 && x <= 12 && sourceY >= 3 && sourceY <= 7)
                        color = sourceY == 5 && (x == 8 || x == 9 || x == 11 || x == 12) ? 'e' : 'a';
                    bool shoe = sourceY >= 13 && (x <= 7 || x >= 10) && "befgw".Contains(color);
                    if (shoe != (pass == 1) || !Palette.TryGetValue(color, out var brush)) continue;
                    int lift = state == PetState.Walking && shoe && (x <= 7) == leftStep ? 1 : 0;
                    dc.DrawRectangle(brush, null, new Rect(originX + x * cell, originY + (y - lift) * cell, cell, cell));
                }
            }
        }

        if (state == PetState.React) {
            // A small pixel heart appears above the raised right hand.
            Pixel(dc, originX, originY, cell, 15, -4, 'b');
            Pixel(dc, originX, originY, cell, 17, -4, 'b');
            for (int x = 14; x <= 18; x++) Pixel(dc, originX, originY, cell, x, -3, 'b');
            for (int x = 15; x <= 17; x++) Pixel(dc, originX, originY, cell, x, -2, 'b');
            Pixel(dc, originX, originY, cell, 16, -1, 'b');
        } else if (state == PetState.Drag) {
            Pixel(dc, originX, originY, cell, 17, -3, 'w');
            Pixel(dc, originX, originY, cell, 17, -2, 'w');
            Pixel(dc, originX, originY, cell, 17, -1, 'w');
        } else if (state == PetState.Sleep) {
            int floatY = (int)(phase * 2) % 2;
            Pixel(dc, originX, originY, cell, 16, -5 - floatY, 'w');
            Pixel(dc, originX, originY, cell, 17, -5 - floatY, 'w');
            Pixel(dc, originX, originY, cell, 17, -4 - floatY, 'w');
            Pixel(dc, originX, originY, cell, 16, -3 - floatY, 'w');
            Pixel(dc, originX, originY, cell, 16, -2 - floatY, 'w');
            Pixel(dc, originX, originY, cell, 17, -2 - floatY, 'w');
        }

        dc.Pop();
    }

    static void Pixel(DrawingContext dc, double ox, double oy, double size, int x, int y, char color) =>
        dc.DrawRectangle(Palette[color], null, new Rect(ox + x * size, oy + y * size, size, size));
}
