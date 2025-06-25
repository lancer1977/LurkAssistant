namespace LurkHelper.Interfaces;

public interface IApplicationConfig
{
    int ItterationDelay { get;  }
    int WindowSwapDelay { get;  }
    string AppName { get;  }
    List<string> Lurks { get;  }
    string BrowserExecutable { get; }
}