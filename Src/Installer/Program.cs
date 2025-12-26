using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Windows; 
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Diagnostics;

namespace PSLastOutputInstaller
{
    public class InstallerWindow : Window
    {
        private Grid mainGrid;
        private Panel contentPanel; // We'll use a Border or Grid for content
        private Button btnNext;
        private Button btnCancel;
        private int currentPage = 0;
        
        // State
        private CheckBox chkAgree;
        private TextBox txtLog;
        private ProgressBar progressBar;
        
        private string tempVideoPath;
        private MediaElement videoPlayer;
        private Border rightPanel;

        public InstallerWindow()
        {
            this.Title = "PSLastOutput Setup";
            this.Width = 800;
            this.Height = 500;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.NoResize;
            this.Background = SystemColors.ControlBrush;

            // Main Grid: 2 Columns
            mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(280) }); // Video Side
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // Content
            this.Content = mainGrid;

            // --- Left Panel (Video) ---
            Border leftPanel = new Border();
            leftPanel.Background = Brushes.Black;
            Grid.SetColumn(leftPanel, 0);
            mainGrid.Children.Add(leftPanel);

            videoPlayer = new MediaElement();
            videoPlayer.LoadedBehavior = MediaState.Manual;
            videoPlayer.UnloadedBehavior = MediaState.Stop;
            videoPlayer.Stretch = Stretch.UniformToFill;
            videoPlayer.MediaEnded += (s, e) => {
                videoPlayer.Position = TimeSpan.Zero;
                videoPlayer.Play();
            };
            leftPanel.Child = videoPlayer;

            // --- Right Panel (Content) ---
            rightPanel = new Border();
            rightPanel.Padding = new Thickness(0);
            Grid.SetColumn(rightPanel, 1);
            mainGrid.Children.Add(rightPanel);
            
            // Right Panel Layout (Dock-ish using Grid rows)
            Grid rightGrid = new Grid();
            rightGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) }); // Header
            rightGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }); // Content
            rightGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) }); // Footer
            rightPanel.Child = rightGrid;

            // Header
            Image headerImage = new Image();
            headerImage.Stretch = Stretch.UniformToFill;
            headerImage.Source = GenerateUniverseSource(520, 80);
            Grid.SetRow(headerImage, 0);
            rightGrid.Children.Add(headerImage);

            // Scrollable Content Container
            contentPanel = new StackPanel(); 
            contentPanel.Margin = new Thickness(20);
            Grid.SetRow(contentPanel, 1);
            rightGrid.Children.Add(contentPanel);

            // Footer
            Border footer = new Border();
            footer.Background = SystemColors.ControlLightBrush;
            Grid.SetRow(footer, 2);
            rightGrid.Children.Add(footer);

            StackPanel btnPanel = new StackPanel();
            btnPanel.Orientation = Orientation.Horizontal;
            btnPanel.HorizontalAlignment = HorizontalAlignment.Right;
            btnPanel.VerticalAlignment = VerticalAlignment.Center;
            btnPanel.Margin = new Thickness(0,0,10,0);
            footer.Child = btnPanel;

            btnNext = new Button() { Content = "Next >", Width = 80, Height = 25, Margin = new Thickness(0,0,10,0) };
            btnNext.Click += OnNextClick;
            
            btnCancel = new Button() { Content = "Cancel", Width = 80, Height = 25 };
            btnCancel.IsCancel = true;
            btnCancel.Click += (s,e) => this.Close();

            btnPanel.Children.Add(btnNext);
            btnPanel.Children.Add(btnCancel);

            // Events
            this.Loaded += OnWindowLoaded;
            this.Closed += OnWindowClosed;

            ShowPage(0);
        }

        private ImageSource GenerateUniverseSource(int width, int height)
        {
            // Create a DrawingVisual to draw the universe
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                Rect r = new Rect(0, 0, width, height);
                LinearGradientBrush grad = new LinearGradientBrush(Colors.Black, Colors.Indigo, 45.0);
                dc.DrawRectangle(grad, null, r);
                
                Random rnd = new Random();
                for (int i = 0; i < 250; i++)
                {
                    double x = rnd.NextDouble() * width;
                    double y = rnd.NextDouble() * height;
                    double s = rnd.NextDouble() * 2 + 1;
                    byte alpha = (byte)rnd.Next(100, 255);
                    SolidColorBrush starBrush = new SolidColorBrush(Color.FromArgb(alpha, 255, 255, 255));
                    dc.DrawEllipse(starBrush, null, new Point(x,y), s/2, s/2);
                }
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            return rtb;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                tempVideoPath = Path.Combine(Path.GetTempPath(), "setup_promo.mp4");
                var asm = Assembly.GetExecutingAssembly();
                
                // We must use the resource name exactly as compiled.
                // In csc /resource:file,name -> The manifest name is 'name'.
                using (var s = asm.GetManifestResourceStream("promo.mp4"))
                {
                    if (s != null)
                    {
                        using (var fs = new FileStream(tempVideoPath, FileMode.Create))
                        {
                            s.CopyTo(fs);
                        }
                        videoPlayer.Source = new Uri(tempVideoPath);
                        videoPlayer.Play();
                    }
                }
            }
            catch {}
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            try { 
                videoPlayer.Stop(); 
                videoPlayer.Source = null;
            } catch {}
            
            // Clean up file after a brief delay or let OS handle temp
            // We can't easily delete if open, but we closed it.
        }

        private void ShowPage(int pageIndex)
        {
            currentPage = pageIndex;
            contentPanel.Children.Clear();

            TextBlock title = new TextBlock() { Text = "", FontSize = 18, FontWeight = FontWeights.Bold, Foreground = Brushes.DarkSlateBlue, Margin = new Thickness(0,0,0,10) };
            contentPanel.Children.Add(title);

            if (pageIndex == 0)
            {
                title.Text = "Welcome to PSLastOutput Setup";
                TextBlock desc = new TextBlock() { Text = "This wizard will guide you through the installation of PSLastOutput.\n\nPSLastOutput is a utility to capture and copy your last PowerShell command and output easily.\n\nClick Next to continue.", TextWrapping = TextWrapping.Wrap };
                contentPanel.Children.Add(desc);
                
                btnNext.Content = "Next >";
                btnNext.IsEnabled = true;
            }
            else if (pageIndex == 1)
            {
                title.Text = "Terms and Conditions";
                TextBox txtTerms = new TextBox() { 
                    Height = 150, 
                    TextWrapping = TextWrapping.Wrap, 
                    IsReadOnly = true, 
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Text = "PSLastOutput License Agreement\r\n\r\n1. Usage: You are free to use this software for any purpose.\r\n2. Liability: The author is not liable for any damages.\r\n3. Privacy: No data is sent to the cloud. All processing is local.\r\n4. Aesthetics: The user agrees that this installer looks cool.\r\n\r\nBy clicking 'I Agree', you accept these terms."
                };
                contentPanel.Children.Add(txtTerms);

                chkAgree = new CheckBox() { Content = "I accept the terms in the License Agreement", Margin = new Thickness(0,20,0,0) };
                chkAgree.Checked += (s, e) => btnNext.IsEnabled = true;
                chkAgree.Unchecked += (s, e) => btnNext.IsEnabled = false;
                contentPanel.Children.Add(chkAgree);

                btnNext.IsEnabled = false;
                btnNext.Content = "Install";
            }
            else if (pageIndex == 2)
            {
                title.Text = "Installing...";
                progressBar = new ProgressBar() { Height = 20, IsIndeterminate = true, Margin = new Thickness(0,0,0,10) };
                contentPanel.Children.Add(progressBar);
                
                txtLog = new TextBox() { Height = 150, TextWrapping = TextWrapping.Wrap, IsReadOnly = true, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
                contentPanel.Children.Add(txtLog);

                btnNext.Visibility = Visibility.Hidden;
                btnCancel.IsEnabled = false;

                Thread t = new Thread(DoInstall);
                t.IsBackground = true;
                t.Start();
            }
            else if (pageIndex == 3)
            {
                title.Text = "Installation Complete";
                TextBlock desc = new TextBlock() { Text = "PSLastOutput has been successfully installed.\n\nPlease restart your PowerShell windows to start using the tool.\n\nPress Ctrl+Shift+C to use it!", TextWrapping = TextWrapping.Wrap };
                contentPanel.Children.Add(desc);
                
                btnNext.Visibility = Visibility.Visible;
                btnNext.Content = "Finish";
                btnNext.IsEnabled = true;
                btnNext.Click -= OnNextClick;
                btnNext.Click += (s,e) => this.Close();
                btnCancel.Visibility = Visibility.Collapsed;
            }
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            ShowPage(currentPage + 1);
        }

        private void Log(string msg)
        {
            Dispatcher.Invoke(new Action(() => {
                txtLog.AppendText(msg + "\r\n");
                txtLog.ScrollToEnd();
            }));
        }

        private void DoInstall()
        {
            try
            {
                Thread.Sleep(500);
                
                string docDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var psInstallPaths = new List<string>();
                string legacyPath = Path.Combine(docDir, @"WindowsPowerShell\Modules");
                string corePath = Path.Combine(docDir, @"PowerShell\Modules");

                if (Directory.Exists(Path.GetDirectoryName(legacyPath))) psInstallPaths.Add(legacyPath);
                if (Directory.Exists(Path.GetDirectoryName(corePath))) psInstallPaths.Add(corePath);

                if (psInstallPaths.Count == 0)
                {
                    Log("Creating Modules directory...");
                    Directory.CreateDirectory(legacyPath);
                    psInstallPaths.Add(legacyPath);
                }

                var assembly = Assembly.GetExecutingAssembly();
                var resourceNames = assembly.GetManifestResourceNames();

                foreach (var modPath in psInstallPaths)
                {
                    Log("Target: " + modPath);
                    string targetDir = Path.Combine(modPath, "PSLastOutput");
                    if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                    foreach (var resName in resourceNames)
                    {
                        if (resName.EndsWith(".psm1") || resName.EndsWith(".psd1"))
                        {
                            string fileName = resName.Contains("psm1") ? "PSLastOutput.psm1" : "PSLastOutput.psd1";
                            string dest = Path.Combine(targetDir, fileName);
                            Log("Extracting " + fileName + "...");
                            
                            using (Stream s = assembly.GetManifestResourceStream(resName))
                            using (FileStream fs = new FileStream(dest, FileMode.Create))
                            {
                                s.CopyTo(fs);
                            }
                        }
                    }
                }

                // FIX: Set Execution Policy to RemoteSigned for CurrentUser
                Log("Configuring Execution Policy...");
                try {
                     Process p = Process.Start(new ProcessStartInfo {
                        FileName = "powershell.exe",
                        Arguments = "-NoProfile -Command \"Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned -Force\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                     if (p != null) p.WaitForExit();
                } catch (Exception ex) {
                    Log("Policy Note: " + ex.Message); 
                }

                Log("Updating PowerShell Profile...");
                var profilePaths = new List<string>();
                profilePaths.Add(Path.Combine(docDir, @"WindowsPowerShell\Microsoft.PowerShell_profile.ps1"));
                profilePaths.Add(Path.Combine(docDir, @"PowerShell\Microsoft.PowerShell_profile.ps1"));

                string importLine = "Import-Module PSLastOutput -ErrorAction SilentlyContinue";

                foreach (var p in profilePaths)
                {
                     try
                     {
                         string pDir = Path.GetDirectoryName(p);
                         if (Directory.Exists(pDir))
                         {
                             if (!File.Exists(p)) File.WriteAllText(p, "# PowerShell Profile\r\n");
                             string content = File.ReadAllText(p);
                             if (!content.Contains("Import-Module PSLastOutput"))
                             {
                                 File.AppendAllText(p, "\r\n" + importLine + "\r\n");
                                 Log("Updated: " + p);
                             }
                         }
                     }
                     catch { }
                }
                
                Dispatcher.Invoke(new Action(() => {
                    progressBar.IsIndeterminate = false;
                    progressBar.Value = 100;
                    ShowPage(3);
                }));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() => MessageBox.Show(this, "Error: " + ex.Message, "Install Error", MessageBoxButton.OK, MessageBoxImage.Error)));
                Dispatcher.Invoke(new Action(() => this.Close()));
            }
        }
    }

    public class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
             if (args.Contains("/uninstall") || args.Contains("-uninstall"))
            {
                MessageBox.Show("Uninstalling PSLastOutput...", "Uninstall", MessageBoxButton.OK, MessageBoxImage.Information);
                try {
                     string docDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                     string[] paths = { Path.Combine(docDir, @"WindowsPowerShell\Modules\PSLastOutput"), Path.Combine(docDir, @"PowerShell\Modules\PSLastOutput") };
                     foreach(var p in paths) { 
                        if(Directory.Exists(p)) Directory.Delete(p, true); 
                     }
                     MessageBox.Show("Uninstalled.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                } catch (Exception x) { MessageBox.Show("Error: "+x.Message); }
                return;
            }

            App app = new App();
            app.Run(new InstallerWindow());
        }
    }
}
