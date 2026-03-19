using PolyhydraGames.Core.Console.System;

namespace LurkHelper.Interfaces;

public interface IWindowFinder
{
    List<AppWindow> GetWindows(string appName);
    void SwitchToWindow(AppWindow window);
}
