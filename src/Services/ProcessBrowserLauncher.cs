using System.Diagnostics;
using LurkHelper.Interfaces;

namespace LurkHelper.Services;

public sealed class ProcessBrowserLauncher : IBrowserLauncher
{
    public void OpenNewWindow(string executable, string url)
    {
        Process.Start(executable, "--new-window " + url);
    }
}
