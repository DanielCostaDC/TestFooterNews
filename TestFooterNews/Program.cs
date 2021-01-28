using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Rendering;

namespace TestFooterNews
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().Start(AppMain, args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            var builder = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new X11PlatformOptions
                {
                    EnableMultiTouch = true,
                    UseDBusMenu = true,
                    UseGpu = true,
                    UseDeferredRendering = true
                })
                .With(new Win32PlatformOptions
                {
                    EnableMultitouch = true,
                    AllowEglInitialization = true
                })
                .UseReactiveUI()
                .UseSkia()
                .LogToTrace();

            var wp = builder.WindowingSubsystemInitializer;
            return builder.UseWindowingSubsystem(() =>
            {
                wp();
                AvaloniaLocator.CurrentMutable.Bind<IRenderTimer>().ToConstant(new RenderTimer());
            });
        }

        private static void AppMain(Application app, string[] args)
        {
            app.Run(new MainWindow());
        }
    }

    public class RenderTimer : IRenderTimer
    {
        public event Action<TimeSpan> Tick;
        private Thread _renderTick;
        public RenderTimer()
        {
            _renderTick = new Thread(() =>
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                while (true)
                {
                    Tick?.Invoke(sw.Elapsed);
                }
            });
            _renderTick.IsBackground = true;
            _renderTick.Start();
        }
    }
}