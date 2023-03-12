using kube.net.lib;

namespace kube.net;

public static class Program
{
    public static void Main(string[] args)
    {
        var console = new KubeConsole();
        console.MainLoop();
    }
}
