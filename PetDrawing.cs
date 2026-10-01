using System.Windows;
using System.Windows.Media;

namespace DesktopPet;

// Pixel colors and silhouette are sampled from the reference image. Spaces are
// transparent; the black cells inside the eyes are filled explicitly below.
sealed class PetDrawing : FrameworkElement {
    const int SpriteWidth = 19;
    const int SpriteHeight = 16;
    const int InhaleWidth = 22;
    const int JumpWidth = 28;
    const int JumpHeight = 29;
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
    static readonly char[,] InhaleSprite = BuildInhaleSprite();
    static readonly char[,] JumpSprite = BuildJumpSprite();
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
        ['k'] = Frozen(0, 0, 0),       // eye pupil
        ['q'] = Frozen(48, 27, 42),   // jump outline
        ['t'] = Frozen(255, 218, 112) // warm air sparkle
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

        if (state == PetState.Jump) {
            DrawJump(dc);
            return;
        }
        if (state == PetState.Inhale) {
            DrawInhale(dc);
            return;
        }

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

    void DrawInhale(DrawingContext dc) {
        double cell = pixelSize * 1.5;
        double originX = Math.Round((34 * pixelSize - InhaleWidth * cell) / 2);
        double originY = 35 * pixelSize - SpriteHeight * cell;
        if (Math.Sin(phase * 12) > 0.6) originY -= cell;
        dc.PushTransform(new ScaleTransform(direction, 1, 17 * pixelSize, 0));
        for (int y = 0; y < SpriteHeight; y++)
            for (int x = 0; x < InhaleWidth; x++)
                if (Palette.TryGetValue(InhaleSprite[x, y], out var brush))
                    dc.DrawRectangle(brush, null, new Rect(originX + x * cell, originY + y * cell, cell, cell));

        int frame = (int)(phase * 7) % 3;
        Pixel(dc, originX, originY, cell, 20, 5, 't');
        Pixel(dc, originX, originY, cell, 19, 6, 't');
        Pixel(dc, originX, originY, cell, 20, 6, 'w');
        Pixel(dc, originX, originY, cell, 21, 6, 't');
        Pixel(dc, originX, originY, cell, 20, 7, 't');
        Pixel(dc, originX, originY, cell, 21 - frame, 10, 'i');
        dc.Pop();
    }

    static char[,] BuildInhaleSprite() {
        var pixels = new char[InhaleWidth, SpriteHeight];
        for (int y = 0; y < SpriteHeight; y++)
            for (int x = 0; x < SpriteWidth; x++)
                pixels[x, y] = Sprite[y][x];
        for (int y = 5; y <= 7; y++) {
            pixels[9, y] = 'k';
            pixels[12, y] = 'k';
        }

        // Rosy cheeks and a tiny round mouth replace the wide side opening.
        pixels[6, 8] = 'i'; pixels[7, 8] = 'i';
        pixels[6, 9] = 'i'; pixels[7, 9] = 'i';
        pixels[13, 8] = 'i'; pixels[14, 8] = 'i';
        pixels[10, 9] = 'i'; pixels[11, 9] = 'e'; pixels[12, 9] = 'i';
        pixels[10, 10] = 'e'; pixels[11, 10] = 'g'; pixels[12, 10] = 'e';
        pixels[10, 11] = 'i'; pixels[11, 11] = 'b'; pixels[12, 11] = 'i';
        return pixels;
    }

    void DrawJump(DrawingContext dc) {
        double cell = pixelSize;
        double originX = Math.Round((34 * pixelSize - JumpWidth * cell) / 2);
        double originY = 35 * pixelSize - JumpHeight * cell;
        dc.PushTransform(new ScaleTransform(direction, 1, 17 * pixelSize, 0));
        for (int y = 0; y < JumpHeight; y++)
            for (int x = 0; x < JumpWidth; x++)
                if (Palette.TryGetValue(JumpSprite[x, y], out var brush))
                    dc.DrawRectangle(brush, null, new Rect(originX + x * cell, originY + y * cell, cell, cell));
        dc.Pop();
    }

    static char[,] BuildJumpSprite() {
        var pixels = new char[JumpWidth, JumpHeight];
        var body = new bool[JumpWidth, JumpHeight];

        void Mark(int y, int left, int right) {
            for (int x = left; x <= right; x++) body[x, y] = true;
        }
        void Put(int x, int y, char color) {
            if (x >= 0 && x < JumpWidth && y >= 0 && y < JumpHeight) pixels[x, y] = color;
        }

        // Build the round body and the forward arm as one silhouette.
        (int Left, int Right)[] head = {
            (11, 21), (9, 22), (8, 23), (7, 24), (6, 24),
            (6, 25), (5, 25), (5, 25), (5, 25), (5, 25),
            (5, 25), (6, 25), (7, 25), (8, 24), (8, 24),
            (9, 23), (10, 23), (11, 22), (12, 22), (13, 21),
            (14, 20)
        };
        for (int row = 0; row < head.Length; row++)
            Mark(row + 2, head[row].Left, head[row].Right);
        (int Left, int Right)[] arm = {
            (3, 8), (2, 9), (1, 9), (0, 9), (0, 9),
            (0, 9), (1, 9), (2, 9), (4, 9)
        };
        for (int row = 0; row < arm.Length; row++)
            Mark(row + 8, arm[row].Left, arm[row].Right);
        Mark(0, 19, 21);
        Mark(1, 18, 22);

        bool BodyAt(int x, int y) =>
            x >= 0 && x < JumpWidth && y >= 0 && y < JumpHeight && body[x, y];
        for (int y = 0; y < JumpHeight; y++) {
            for (int x = 0; x < JumpWidth; x++) {
                if (!body[x, y]) continue;
                bool edge = !BodyAt(x - 1, y) || !BodyAt(x + 1, y) ||
                    !BodyAt(x, y - 1) || !BodyAt(x, y + 1);
                char color = edge ? 'q' : x >= 23 || y >= 19 ? 'd' : 'a';
                Put(x, y, color);
            }
        }

        // Gentle highlights and a bit of blush keep the face round.
        for (int x = 18; x <= 21; x++) Put(x, 3, 'i');
        Put(19, 4, 'i'); Put(20, 4, 'i');
        Put(3, 11, 'i'); Put(4, 11, 'i'); Put(3, 12, 'd');
        Put(10, 14, 'i'); Put(11, 14, 'i');
        Put(20, 14, 'i'); Put(21, 14, 'i');

        // Two tall eyes and an open mouth.
        for (int y = 8; y <= 13; y++) {
            Put(12, y, 'e'); Put(13, y, y == 8 ? 'w' : 'g');
            Put(17, y, 'e'); Put(18, y, y == 8 ? 'w' : 'g');
        }
        Put(12, 8, 'q'); Put(17, 8, 'q');
        for (int x = 14; x <= 16; x++) Put(x, 15, 'e');
        Put(14, 16, 'e'); Put(15, 16, 'g'); Put(16, 16, 'e');
        Put(14, 17, 'e'); Put(15, 17, 'g'); Put(16, 17, 'e');
        Put(15, 18, 'b');

        // The shoes overlap the belly. Their top rows stay open so the
        // dark outline does not make them look detached.
        void Shoe(int top, (int Left, int Right)[] rows) {
            bool ShoeAt(int x, int y) {
                int row = y - top;
                return row >= 0 && row < rows.Length &&
                    x >= rows[row].Left && x <= rows[row].Right;
            }
            for (int row = 0; row < rows.Length; row++) {
                for (int x = rows[row].Left; x <= rows[row].Right; x++) {
                    int y = top + row;
                    bool edge = x == rows[row].Left || x == rows[row].Right ||
                        !ShoeAt(x, y + 1) ||
                        (row > 0 && !ShoeAt(x, y - 1));
                    Put(x, y, edge ? 'q' : row >= rows.Length - 3 ? 'f' : 'b');
                }
            }
        }
        Shoe(17, new (int, int)[] {
            (6, 11), (5, 12), (5, 13), (5, 13), (6, 13),
            (6, 13), (7, 12), (8, 12), (9, 11), (10, 11)
        });
        Shoe(20, new (int, int)[] {
            (18, 23), (17, 24), (17, 25), (18, 25), (19, 25),
            (20, 25), (21, 25), (22, 24), (23, 24)
        });
        Put(8, 19, 'w'); Put(9, 19, 'i'); Put(22, 22, 'i');
        return pixels;
    }
}
