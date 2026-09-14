using System.Windows;
using System.Threading;
namespace DesktopPet;
public class App : Application {
    [STAThread] public static void Main() {
        using var mutex = new Mutex(true, "Local\\MarkDesktopPet", out bool first);
        if (!first) return;
        new App().Run(new PetWindow());
    }
}
