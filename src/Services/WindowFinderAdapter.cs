using LurkHelper.Interfaces;
using PolyhydraGames.Core.Console.System;

namespace LurkHelper.Services;

public sealed class WindowFinderAdapter : IWindowFinder
{
    public List<AppWindow> GetWindows(string appName) => WindowFinder.GetWindows(appName);

    public void SwitchToWindow(AppWindow window) => WindowFinder.SwitchToWindow(window);
}
