namespace LurkHelper.Interfaces;

public interface IApplicationConfig
{
    int IterationDelay { get;  }
    int WindowSwapDelay { get;  }
    string AppName { get;  }
    List<string> Lurks { get;  }
    string BrowserExecutable { get; }
}