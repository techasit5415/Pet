using System.Windows;
using System.Windows.Media;
namespace DesktopPet;

// Original Kirby-inspired pixel artwork. Each cell is one logical pixel.
// Keep drawing in whole grid coordinates to preserve the retro look.
sealed class PetDrawing : FrameworkElement {
    PetState state;
    double phase;
    int direction=1;
    const int Grid=28;
    int pixelSize=2;
    internal int PixelSize {
        get => pixelSize;
        set { pixelSize = Math.Clamp(value, 1, 8); InvalidateVisual(); }
    }
    static readonly Dictionary<char,Brush> Palette = new() {
        ['o']=Frozen(74,35,64),     // outline
        ['p']=Frozen(255,155,198), // pink body
        ['s']=Frozen(233,104,164), // shadow / cheeks
        ['h']=Frozen(255,200,221), // highlight
        ['r']=Frozen(222,43,87),   // red shoes
        ['b']=Frozen(52,69,131),   // blue eyes
        ['w']=Frozen(255,246,250)  // eye highlight
    };
    static Brush Frozen(byte r,byte g,byte b) {
        var brush=new SolidColorBrush(Color.FromRgb(r,g,b)); brush.Freeze(); return brush;
    }
    public PetDrawing() { RenderOptions.SetEdgeMode(this,EdgeMode.Aliased); }
    internal void Update(PetState s,double p,int d) { state=s; phase=p; direction=d; InvalidateVisual(); }
    protected override void OnRender(DrawingContext dc) {
        base.OnRender(dc);
        var pixels=new char[Grid,Grid];
        void Put(int x,int y,char c) { if(x>=0&&x<Grid&&y>=0&&y<Grid)pixels[x,y]=c; }
        void Box(int x,int y,int w,int h,char c) {
            for(int yy=y;yy<y+h;yy++)for(int xx=x;xx<x+w;xx++)Put(xx,yy,c);
        }
        void Oval(int cx,int cy,int rx,int ry,char fill) {
            bool Inside(int x,int y)=> (x-cx)*(x-cx)/(double)(rx*rx)+(y-cy)*(y-cy)/(double)(ry*ry)<=1;
            for(int yy=cy-ry;yy<=cy+ry;yy++)for(int xx=cx-rx;xx<=cx+rx;xx++) {
                if(!Inside(xx,yy))continue;
                bool border=!Inside(xx-1,yy)||!Inside(xx+1,yy)||!Inside(xx,yy-1)||!Inside(xx,yy+1);
                Put(xx,yy,border?'o':fill);
            }
        }
        int bob=state==PetState.Walking?(int)Math.Round(Math.Sin(phase*12)):0;
        if(state==PetState.React)bob=-(int)Math.Round(Math.Abs(Math.Sin(phase*10))*2);
        if(state==PetState.Sleep) {
            Oval(14,25,9,2,'r');
            Oval(14,22,11,4,'p');
            Box(7,20,5,1,'h');
            Box(10,22,3,1,'o'); Box(17,22,3,1,'o');
            Box(8,24,2,1,'s'); Box(21,24,2,1,'s');
            // Pixel Z, animated without font smoothing.
            int zy=((int)(phase*2)%2);
            Box(20,4+zy,5,1,'w'); Put(23,5+zy,'w'); Put(22,6+zy,'w'); Put(21,7+zy,'w'); Box(20,8+zy,5,1,'w');
        } else {
            bool sitting=state==PetState.Sit;
            int cy=(sitting?18:16)+bob;
            int step=state==PetState.Walking?((int)(phase*8)%2==0?1:-1):0;
            int footY=state==PetState.Jump?24:25;
            Oval(9-step,footY-Math.Max(0,step),5,2,'r');
            Oval(19+step,footY-Math.Max(0,-step),5,2,'r');
            Oval(4,cy+2,3,3,'p');
            Oval(24,cy+(state==PetState.React||state==PetState.Drag?-4:1),3,3,'p');
            Oval(14,cy,10,sitting?7:9,'p');
            Box(8,cy-5,4,1,'h'); Box(7,cy-4,2,1,'h');
            Box(9,cy+6,10,1,'s');
            bool blink=((int)(phase*10)%47==0)||state==PetState.React;
            if(blink) { Box(11,cy-2,2,1,'o'); Box(17,cy-2,2,1,'o'); }
            else {
                Box(11,cy-4,2,5,'o'); Box(17,cy-4,2,5,'o');
                Put(11,cy-4,'w'); Put(17,cy-4,'w');
                Put(12,cy,'b'); Put(18,cy,'b');
            }
            Box(7,cy+1,3,1,'s'); Box(20,cy+1,3,1,'s');
            if(state==PetState.React) { Box(14,cy+2,3,2,'o'); Box(14,cy+3,3,1,'r'); }
            else { Put(14,cy+2,'o'); Put(16,cy+2,'o'); Put(15,cy+3,'o'); }
            if(state==PetState.React) {
                Box(21,1,2,1,'r'); Box(25,1,2,1,'r'); Box(20,2,8,2,'r'); Box(21,4,6,1,'r'); Box(22,5,4,1,'r'); Box(23,6,2,1,'r');
            }
            if(state==PetState.Drag) { Box(23,1,2,4,'w'); Box(23,6,2,1,'w'); }
        }
        int p = pixelSize;
        double offsetX = 3 * p;
        double offsetY = 6 * p;
        double centerX = 17 * p;
        dc.PushTransform(new ScaleTransform(direction,1,centerX,0));
        // Transparent cells are not drawn, so desktop clicks pass through them.
        for(int yy=0;yy<Grid;yy++)for(int xx=0;xx<Grid;xx++) {
            if(Palette.TryGetValue(pixels[xx,yy],out var brush))
                dc.DrawRectangle(brush,null,new Rect(offsetX+xx*p,offsetY+yy*p,p,p));
        }
        dc.Pop();
    }
}
