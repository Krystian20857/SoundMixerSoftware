using System;
using SoundMixerSoftware.Updater.Github;

namespace SoundMixerSoftware.Updater
{
   public class NewVersionEventArgs : EventArgs
   {
      public GithubRelease GithubRelease { get; set; }
      public Release Release { get; set; }

      public NewVersionEventArgs(GithubRelease githubRelease, Release release)
      {
         GithubRelease = githubRelease;
         Release = release;
      }
   }
}