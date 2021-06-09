using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace DayZLootEdit
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            
            InitializeComponent();

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string versionText = "Version %1";
            StringBuilder builder = new StringBuilder(versionText);
            builder.Replace("%1", version);
            versionText = builder.ToString();
            lblVersion.Content = versionText.ToString();

            string copyright = string.Empty;
            Assembly assembly = typeof(InfoWindow).Assembly;
            if (assembly != null)
            {
                object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
                if ((attributes != null) && (attributes.Length > 0))
                {
                    copyright = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                }
                if (string.IsNullOrEmpty(copyright))
                {
                    copyright = "(C) ME";
                }
                lblCopyright.Content = copyright.ToString();
            }

            MemoryStream memStream = new MemoryStream(ASCIIEncoding.Default.GetBytes(Properties.Resources.info));
            rtbCredits.SelectAll();
            rtbCredits.Selection.Load(memStream, DataFormats.Rtf);

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
