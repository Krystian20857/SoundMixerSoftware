using System;

namespace SoundMixerSoftware.Common.Web
{
    public class SpeedConverter
    {
        #region Private Fields
        
        
        #endregion
        
        #region Public Properties

        public string[] Suffixes { get; set; } = {"B/s", "KB/s", "MB/s", "GB/s", "TB/s"};

        public string Prefix { get; set; } = "";

        #endregion
        
        #region Constructor

        public SpeedConverter()
        {
            
        }
        
        #endregion
        
        #region Public Methods
        
        public string FormatSpeed(double speed, int precision = 2)
        {
            var index = 0;
            while (speed >= 1024 && index < Suffixes.Length)
            {
                index++;
                speed /= 1024;
            }

            speed = Math.Round(speed, precision);
            
            return $"{Prefix}{speed}{Suffixes[index]}";
        }

        public double GetFromFormatted(string speed)
        {
            for (var n = speed.Length - 1;n > 0; n--)
            {
                if (n - 1 < 0)
                    return 0D;
                var char2 = speed[n - 1];
                if (char.IsDigit(char2))
                {
                    var suffix = speed.Substring(n, speed.Length - n);
                    var suffixIndex = Array.IndexOf(Suffixes, suffix);
                    if(suffixIndex == -1)
                        continue;
                    return double.TryParse(speed.Substring(0, n), out var result) ? result : 0;
                }
            }

            return 0.0D;
        }
        
        #endregion
    }
}