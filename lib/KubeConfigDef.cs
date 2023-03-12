// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618
namespace kube.net.lib;

public class KubeClusterConfig
{
    public string server { get; set; }
    public string certificateAuthorityData { get; set; }
}
public class KubeCluster
{
    public string name { get; set; }
    // public KubeClusterConfig cluster { get; set; }
    public Dictionary<string, string> cluster { get; set; }
}

public class KubeContextConfig
{
    public string cluster { get; set; }
    public string user { get; set; }
}

public class KubeContext
{
    public string name { get; set; }
    public KubeContextConfig context { get; set; }
}

public class KubeUserConfig
{
    public string token { get; set; }
    public string clientCertificateData { get; set; }
    public string clientKeyData { get; set; }
}

public class KubeUser
{
    public string name { get; set; }
    public KubeUserConfig user { get; set; }
}

public class KubeConfigDef
{
    public string apiVersion { get; set; }
    public string currentContext { get; set; }
    public string kind { get; set; }
    public Dictionary<string, string> preferences { get; set; }
    public KubeCluster[] clusters { get; set; }
    public KubeContext[] contexts { get; set; }
    public KubeUser[] users { get; set; }
}