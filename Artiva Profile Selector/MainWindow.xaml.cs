using System;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Artiva_Profile_Selector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ArtivaProfile oldProfile;
        private string registryPath;
        private string workstationPath;
        private string studioPath;
        private bool saved;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            profileNameBox.GotFocus += Name_OnFocus;
            profileNameBox.LostFocus += Name_LostFocus;
            tabControl.SelectionChanged += Selection_Change;

            ObservableCollection<ArtivaProfile> ArtivaProfiles = new ObservableCollection<ArtivaProfile>();

            oldProfile = new ArtivaProfile
            {
                ProfileName = "",
                Description = "",
                HostName = "",
                HostPort = "",
                User = "",
                Password = "",
                Namespace = "",
                DeskNumber = ""
            };
            registryPath = @"Software\VB and VBA Program Settings\Ontario Systems API Manager\Profiles";
            workstationPath = @"C:\Program Files (x86)\Ontario\Workstation\guiApp.exe";
            studioPath = @"C:\Program Files (x86)\Ontario\Stuido\studio.exe";
            saved = false;

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath))
                {
                    if (key != null)
                    {
                        foreach (string subkey in key.GetValueNames())
                        {
                            var val = (string)(key.GetValue(subkey));
                            var item = new ArtivaProfile {
                                ProfileName = subkey,
                                Description = val.Split('\x7')[0],
                                HostName = val.Split('\x7')[1],
                                HostPort = val.Split('\x7')[2],
                                User = val.Split('\x7')[3],
                                Password = val.Split('\x7')[4],
                                Namespace = val.Split('\x7')[5],
                                DeskNumber = val.Split('\x7')[6]
                            };
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

        private void OpenWorkstation_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(workstationPath, ((ArtivaProfile)Profiles.SelectedItem).ProfileName);
        }

        private void OpenStudio_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(studioPath, ((ArtivaProfile)Profiles.SelectedItem).ProfileName);
        }

        private void AddProfile_Click(object sender, RoutedEventArgs e)
        {
            this.tabControl.SelectedIndex = 1;

            profileNameBox.Text = "New Profile";
            descriptionBox.Text = "";
            hostNameBox.Text = "";
            hostPortBox.Text = "1600";
            userBox.Text = "";
            passwordBox.Text = "";
            namespaceBox.Text = "";
            deskNumberBox.Text = "";

        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            string oldProfileName = oldProfile.ProfileName;

            ArtivaProfile newProfile = new ArtivaProfile
            {
                ProfileName = profileNameBox.Text,
                Description = descriptionBox.Text,
                HostName = hostNameBox.Text,
                HostPort = hostPortBox.Text,
                User = userBox.Text,
                Password = passwordBox.Text,
                Namespace = namespaceBox.Text,
                DeskNumber = deskNumberBox.Text
            };
            
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath, true))
                {
                    if (key != null)
                    {
                        //Replace old profile
                        key.DeleteValue(oldProfile.ProfileName);
                        key.SetValue(newProfile.ProfileName, newProfile.ToDelimitedString(), RegistryValueKind.String);
                    }
                    else
                    {
                        //New profile
                        key.SetValue(newProfile.ProfileName, newProfile.ToDelimitedString(), RegistryValueKind.String);
                    }
                }
                saved = true;
                MessageBox.Show("Profile Saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DelProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete the profile " + ((ArtivaProfile)Profiles.SelectedItem).ProfileName + "?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                ArtivaProfile oldProfile = (ArtivaProfile)Profiles.SelectedItem;
                
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath, true))
                    {
                        if (key != null)
                        {
                            key.DeleteValue(oldProfile.ProfileName);
                        }
                    }
                    MessageBox.Show("Profile Deleted");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Name_OnFocus(object sender, EventArgs e)
        {
            //Ensure a profile is selected
            if ((ArtivaProfile)Profiles.SelectedItem != null)
            {
                //Save the old profile name so it can be deleted and replaced with a new profile
                if (oldProfile.ProfileName == ((ArtivaProfile)Profiles.SelectedItem).ProfileName)
                {
                    ArtivaProfile oldProfile = (ArtivaProfile)Profiles.SelectedItem;
                }
            }
        }

        private void Name_LostFocus(object sender, EventArgs e)
        {
            foreach (ArtivaProfile row in Profiles.ItemsSource)
            {
                if (profileNameBox.Text == row.ProfileName)
                {
                    MessageBox.Show("A profile with that name already exists.\n\nChoose a different name.");
                    profileNameBox.Focus();
                }
            }
        }

        private void Selection_Change(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                if (!saved)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Leave without saving changes?", "Leave Confirmation", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.No)
                    {
                        tabControl.SelectedIndex = 1;
                    }
                }
                else
                {
                    saved = false;
                }
            }
        }    
    }

    public class ArtivaProfile
    {
        public string ProfileName { get; set; }
        public string Description { get; set; }
        public string HostName { get; set; }
        public string HostPort { get; set; }
        public string Namespace { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DeskNumber { get; set; }

        public string ToDelimitedString()
        {
            return Description = Description + '\x7' + HostName + '\x7' + HostPort + '\x7' + User + '\x7' + Password + '\x7' + Namespace + '\x7' + DeskNumber;
        }
    }
}
