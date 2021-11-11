using System;

namespace ModernClientNet6.MVVM.Model
{
    [Serializable]
    public class Settings
    {
        public bool RememberMe { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public string Theme { get; set; }

        public Settings()
        {
            RememberMe = false;
            Login = "";
            Pass = "";
            Theme = "DarkTheme";
        }
    }
}
