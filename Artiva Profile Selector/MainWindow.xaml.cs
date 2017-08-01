using System;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace Artiva_Profile_Selector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ArtivaProfiles = new ObservableCollection<ArtivaProfile>();

            var path = @"";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        foreach (String subkey in key.GetValueNames())
                        {
                            var item = new ArtivaProfile { Namespace = subkey, Description = "", User = "" };

                            ArtivaProfiles.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Profiles.ItemsSource = ArtivaProfiles;
                
        }

        private void openWorkstation_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open Workstation Clicked");
        }

        private void openStudio_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open Studio Clicked");
        }

        private void addProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Profile Clicked");
        }
    }

    public class ArtivaProfile
    {
        public string Namespace { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
    }
}
