using Nexora.Debugging;

namespace Nexora;

public class NexoraConsts
{
    public const string LocalizationSourceName = "Nexora";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "e9932603d5334ce08321a8e0ec211160";
}
