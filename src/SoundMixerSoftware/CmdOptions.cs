using CommandLine;

namespace SoundMixerSoftware
{
    public class CmdOptions
    {
        [Option('f', "force-show", Required = false, Default = false, HelpText= "Force application to show main window regardless to settings.")]
        public bool ForceShow { get; set; }

        [Option('s', "squirrel-firstrun", Required = false, Default = "", HelpText = "Indicated running application after installation.")]
        public bool SquirrelFirstRun { get; set; }
    }
}