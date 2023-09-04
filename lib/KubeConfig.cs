using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace kube.net.lib;

public class KubeConfig
{
    private string _defaultConfigFile;
    private string _defaultBackupFile;

    /// <summary>
    /// Constructor sets the default locations to your home directory.
    /// </summary>
    public KubeConfig()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _defaultConfigFile = $"{home}/.kube/config";
        _defaultBackupFile = $"{_defaultConfigFile}-backup";
    }
    
    
    /// <summary>
    /// Return a list of known context names
    /// </summary>
    /// <returns>A list of context names as string</returns>
    public List<string> Contexts()
    {
        var config = LoadConfig();
        return config.contexts.Select(ctx => ctx.name).OrderBy(ctx => ctx.FirstOrDefault()).ToList();
    }

    /// <summary>
    /// Set the `current-context` property to what is given.
    /// </summary>
    /// <param name="context">The desired current context</param>
    public void SetActiveContext(string context)
    {
        var config = LoadConfig();
        config.currentContext = context;
        SaveConfig(config);
    }

    /// <summary>
    /// Remove a context (and related info) from the configuration
    /// </summary>
    /// <param name="context">The unlucky context name</param>
    public void DeleteContext(string context)
    {
        var config = LoadConfig();
        var newConfig = new KubeConfigDef
        {
            contexts = config.contexts.Where(ctx => ctx.name != context).ToArray(),
            clusters = config.clusters.Where(cl => cl.name != context).ToArray(),
            users = config.users.Where(u => u.name != context).ToArray(),
            currentContext = config.currentContext,
            kind = config.kind,
            preferences = config.preferences
        };
        
        SaveConfig(newConfig);
    }

    /// <summary>
    /// Load the current kube config
    /// </summary>
    /// <returns>An instance of KubeConfigDef with (hopefully) values in it.</returns>
    private KubeConfigDef LoadConfig()
    {
        var deserializer = new DeserializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance).IgnoreUnmatchedProperties().Build();
        var yaml = System.IO.File.ReadAllText(_defaultConfigFile);
        var config = deserializer.Deserialize<KubeConfigDef>(yaml);
        
        return config;
    }

    /// <summary>
    /// Read the clipboard, and try to parse the thing in it as YAML conforming to KubeConfigDef
    /// </summary>
    /// <returns>An instance of KubeConfigDef or null</returns>
    public KubeConfigDef? LoadConfigFromClipboard()
    {
        var txt = TextCopy.ClipboardService.GetText();
        if (txt == null)
        {
            return null;
        }

        var deserializer = new DeserializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance)
            .IgnoreUnmatchedProperties().Build();
        return deserializer.Deserialize<KubeConfigDef>(txt);
    }

    /// <summary>
    /// Merge two kube configurations into one and write to disk
    /// </summary>
    /// <param name="snippet">An instance of KubeConfigDef that you wish to merge into your kube config</param>
    public List<string> MergeConfig(KubeConfigDef snippet) 
    {
        // see: KubernetesClientConfiguration::MergeConfig
        // hell naw
        var config = LoadConfig();
        var newConfig = new KubeConfigDef
        {
            contexts = config.contexts.Concat(snippet.contexts).ToArray(),
            clusters = config.clusters.Concat(snippet.clusters).ToArray(),
            users = config.users.Concat(snippet.users).ToArray(),
            currentContext = config.currentContext,
            kind = config.kind,
            preferences = config.preferences
        };

        SaveConfig(newConfig);

        // Return all the context names that were added
        return snippet.contexts.Select(ctx => ctx.name).OrderBy(ctx => ctx.FirstOrDefault()).ToList();
    }

    /// <summary>
    /// Save the current KubeConfigDef into your kube config.
    /// It creates a backup of the current config.
    /// </summary>
    /// <param name="config">The current configuration as a KubeConfigDef instance</param>
    private void SaveConfig(KubeConfigDef config)
    {
        BackupCurrentConfig();
        var serializer = new SerializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
        var yaml = serializer.Serialize(config);
        using StreamWriter output = new StreamWriter(_defaultConfigFile);
        output.WriteLine(yaml);
    }

    /// <summary>
    /// Copy the current kube config to a backup file (config-backup-[timestamp])
    /// </summary>
    private void BackupCurrentConfig()
    {
        try
        {
            File.Copy(_defaultConfigFile, _defaultBackupFile);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}