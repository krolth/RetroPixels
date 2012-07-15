#if WINDOWS_PHONE
using System;
using System.Reflection;
using Microsoft.Phone.Tasks;
#endif

using Microsoft.Xna.Framework;

namespace WallAll
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class AboutMenuScreen : MenuScreen
    {
        const string companyName = "8BigDogs";
        const string Coding1 = "Sharad Cornejo Altuzar";
        const string Coding2 = "Fernando Altuzar";
        const string Art1 = "Stephany Valdés";
        //const string Music = "Music: http://incompetech.com/";
        const string ConceptBy = "Concept by: ";
        const string ABA = "http://wonderfl.net/user/ABA";
        
        MenuEntry companyMenuEntry;
        MenuEntry codingMenuEntry;
        MenuEntry coding2MenuEntry;
        //MenuEntry artMenuEntry;
        //MenuEntry musicMenuEntry;
        MenuEntry conceptMenuEntry1;
        MenuEntry conceptMenuEntry2;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutMenuScreen()
            : base(string.Empty)
        {
            companyMenuEntry = new MenuEntry(companyName);
            companyMenuEntry.IsTitle = true;

            codingMenuEntry = new MenuEntry(Coding1) { Font = ScreenManager.FontSmall };
            coding2MenuEntry = new MenuEntry(Coding2) { Font = ScreenManager.FontSmall };
            //  artMenuEntry = new MenuEntry(Art1) { Font = ScreenManager.FontSmall };
            //musicMenuEntry = new MenuEntry(Music) { Font = ScreenManager.FontSmall };
            conceptMenuEntry1 = new MenuEntry(ConceptBy) { Font = ScreenManager.FontSmall };
            conceptMenuEntry2 = new MenuEntry(ABA) { Font = ScreenManager.FontSmall };

            string version = GetGameVersion();
            var versionEntry = new MenuEntry(version);

            // Add entries to the menu.
            MenuEntries.Add(companyMenuEntry);
            MenuEntries.Add(codingMenuEntry);
            MenuEntries.Add(coding2MenuEntry);
            //MenuEntries.Add(propsMenuEntry);
            //MenuEntries.Add(musicMenuEntry);
            MenuEntries.Add(conceptMenuEntry1);
            MenuEntries.Add(conceptMenuEntry2);
            MenuEntries.Add(versionEntry);
        }

        private string GetGameVersion()
        {
            string output = "Version: ";
            try
            {
#if WINDOWS_PHONE
                Version version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;
                return output + version.ToString();
#endif
            }
            catch
            { }

            //Version for windows
            return output + "1.0.0.1";
        }

        //        void WebsiteMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        //        {
        //#if WINDOWS_PHONE
        //            WebBrowserTask wb = new WebBrowserTask();
        //            wb.URL = "http://www.k45games.com";
        //            wb.Show();
        //#endif
        //        }
    }
}
