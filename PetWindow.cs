using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
namespace DesktopPet;
enum PetState { Idle, Walking, Jump, Sit, Sleep, React, Drag }
public sealed class PetWindow : Window {
    readonly PetDrawing drawing = new();
    readonly Random random = new();
    readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(33) };
    readonly Stopwatch clock = Stopwatch.StartNew();
    readonly string settingsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"MarkDesktopPet","follow.txt");
    List<Native.Rect> areas = new();
    IntPtr handle;
    PetState state;
    double x,y,velocityY,remaining=2,last,refresh,phase;
    int direction=1,widthPx=140,heightPx=140;
    bool follow,paused,pressed,moved;
    Native.Point press,offset;
    const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    public PetWindow() {
        Width=140; Height=140; WindowStyle=WindowStyle.None; ResizeMode=ResizeMode.NoResize;
        AllowsTransparency=true; Background=Brushes.Transparent; Topmost=true; ShowInTaskbar=false; ShowActivated=false;
        Content=drawing;
        follow=System.IO.File.Exists(settingsPath) && System.IO.File.ReadAllText(settingsPath)=="true";
        var menu = new ContextMenu();
        Add(menu,"Follow mouse",()=> { follow=!follow; Save(); },()=>follow);
        Add(menu,"Pause",()=>paused=!paused,()=>paused);
        Add(menu,"Jump",()=>Set(PetState.Jump));
        Add(menu,"Sit",()=>Set(PetState.Sit,6));
        Add(menu,"Sleep",()=>Set(PetState.Sleep,12));
        Add(menu,"Move to next monitor",NextMonitor);
        Add(menu,"Start with Windows",ToggleStartup,StartupEnabled);
        Add(menu,"Exit",Close);
        drawing.ContextMenu=menu;
        MouseLeftButtonDown+=Down; MouseMove+=Move; MouseLeftButtonUp+=Up;
        LostMouseCapture+=(_,_)=> { if(pressed) { pressed=false; Set(PetState.Idle); } };
        SourceInitialized+=(_,_)=> {
            handle=new WindowInteropHelper(this).Handle;
            Native.GetWindowRect(handle,out var rect); widthPx=rect.Right-rect.Left; heightPx=rect.Bottom-rect.Top;
            areas=Native.WorkAreas();
            if(areas.Count==0) { Close(); return; }
            Native.GetCursorPos(out var cursor); var a=Nearest(cursor.X,cursor.Y);
            x=a.Left+Math.Max(0,(a.Right-a.Left-widthPx)/2); y=a.Bottom-heightPx;
            Place(); last=clock.Elapsed.TotalSeconds; timer.Start();
        };
        timer.Tick+=Tick;
        Closed+=(_,_)=>timer.Stop();
    }
    void Add(ContextMenu menu,string label,Action action,Func<bool>? check=null) {
        var item=new MenuItem { Header=label, IsCheckable=check!=null };
        menu.Opened+=(_,_)=> { if(check!=null)item.IsChecked=check(); };
        item.Click+=(_,_)=> { try { action(); } catch(Exception e) { MessageBox.Show(e.Message,"Desktop Pet"); } };
        menu.Items.Add(item);
    }
    void Save() { System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(settingsPath)!); System.IO.File.WriteAllText(settingsPath,follow?"true":"false"); }
    bool StartupEnabled() { using var key=Registry.CurrentUser.OpenSubKey(RunKey); return key?.GetValue("MarkDesktopPet")!=null; }
    void ToggleStartup() {
        using var key=Registry.CurrentUser.CreateSubKey(RunKey);
        if(StartupEnabled()) key.DeleteValue("MarkDesktopPet",false);
        else {
            var exe=Environment.ProcessPath!;
            if(System.IO.Path.GetFileNameWithoutExtension(exe).Equals("dotnet",StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Publish first, then launch DesktopPet.exe to enable startup.");
            key.SetValue("MarkDesktopPet","\""+exe+"\"");
        }
    }
    Native.Rect Nearest(double px,double py) => areas.OrderBy(a=> {
        var dx=px-Math.Clamp(px,a.Left,a.Right); var dy=py-Math.Clamp(py,a.Top,a.Bottom); return dx*dx+dy*dy;
    }).First();
    void NextMonitor() {
        var current=Nearest(x+widthPx/2,y+heightPx/2); int i=areas.FindIndex(a=>a.Left==current.Left&&a.Top==current.Top);
        var next=areas[(i+1)%areas.Count]; x=next.Left+Math.Max(0,(next.Right-next.Left-widthPx)/2); y=next.Bottom-heightPx;
        Set(PetState.Idle); Place();
    }
    void Set(PetState next,double seconds=0) {
        state=next; remaining=seconds>0?seconds:2+random.NextDouble()*4;
        if(next==PetState.Jump) { velocityY=-450; remaining=3; }
    }
    void Down(object sender,MouseButtonEventArgs e) {
        Native.GetCursorPos(out press); offset=new Native.Point { X=press.X-(int)x,Y=press.Y-(int)y };
        pressed=true; moved=false; CaptureMouse(); e.Handled=true;
    }
    void Move(object sender,MouseEventArgs e) {
        if(!pressed)return; Native.GetCursorPos(out var p);
        if(Math.Abs(p.X-press.X)+Math.Abs(p.Y-press.Y)>5) moved=true;
        if(moved) { state=PetState.Drag; x=p.X-offset.X; y=p.Y-offset.Y; Clamp(); Place(); }
    }
    void Up(object sender,MouseButtonEventArgs e) {
        if(!pressed)return; pressed=false; ReleaseMouseCapture(); Set(moved?PetState.Idle:PetState.React, moved?2:1);
    }
    void Clamp() {
        var a=Nearest(x+widthPx/2,y+heightPx/2);
        x=Math.Clamp(x,a.Left,Math.Max(a.Left,a.Right-widthPx)); y=Math.Clamp(y,a.Top,Math.Max(a.Top,a.Bottom-heightPx));
    }
    void Place() => Native.SetWindowPos(handle,IntPtr.Zero,(int)Math.Round(x),(int)Math.Round(y),0,0,0x15);
    void Tick(object? sender,EventArgs e) {
        double now=clock.Elapsed.TotalSeconds,dt=Math.Clamp(now-last,0,0.08); last=now;
        refresh+=dt;
        if(refresh>1) { var updated=Native.WorkAreas(); if(updated.Count>0)areas=updated; refresh=0; Clamp(); }
        if(paused||pressed||drawing.ContextMenu?.IsOpen==true) { Place(); return; }
        phase+=dt; remaining-=dt;
        Native.GetCursorPos(out var mouse);
        if(remaining<=0&&state!=PetState.Jump) {
            var choices=new[] { PetState.Idle,PetState.Walking,PetState.Walking,PetState.Jump,PetState.Sit,PetState.Sleep };
            direction=random.Next(2)==0?-1:1; Set(choices[random.Next(choices.Length)]);
        }
        if(follow && state!=PetState.Jump && state!=PetState.React) {
            double distance=mouse.X-(x+widthPx/2);
            if(Math.Abs(distance)>35 || Nearest(mouse.X,mouse.Y).Left!=Nearest(x,y).Left) { state=PetState.Walking; direction=distance>=0?1:-1; }
            else if(state==PetState.Walking) Set(PetState.Idle);
        }
        var area=Nearest(x+widthPx/2,y+heightPx/2);
        double ground=area.Bottom-heightPx;
        if(state==PetState.Walking) {
            double nx=x+direction*90*dt;
            if(nx<area.Left||nx+widthPx>area.Right) {
                // Traverse touching horizontal monitors; disconnected layouts use the menu.
                var neighbors=areas.Where(a=> direction>0 ? Math.Abs(a.Left-area.Right)<=2 : Math.Abs(a.Right-area.Left)<=2).ToList();
                if(neighbors.Count>0) {
                    var target=neighbors.OrderBy(a=>Math.Abs(a.Bottom-area.Bottom)).First();
                    x=direction>0?target.Left:target.Right-widthPx; y=target.Bottom-heightPx;
                } else { direction=-direction; x=Math.Clamp(nx,area.Left,Math.Max(area.Left,area.Right-widthPx)); }
            } else x=nx;
        }
        if(state==PetState.Jump) { velocityY+=1000*dt; y+=velocityY*dt; if(y>=ground) { y=ground; Set(PetState.Idle); } }
        else if(y<ground) y=Math.Min(ground,y+260*dt);
        Clamp(); Place(); drawing.Update(state,phase,direction);
    }
}
