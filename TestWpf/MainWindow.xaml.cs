using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetWindowDisplayAffinity(IntPtr hwnd, uint affinity);

        public MainWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += Window_PreviewKeyDown;
            this.PreviewKeyUp += Window_PreviewKeyUp;
            this.SizeChanged += MainWindow_SizeChanged;
           
            this.Loaded += (s,e) => {
                // Get the window handle
                var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

                // Call the function with the desired affinity
                // For example, to disable all display affinity, use 0
                uint affinity = 7;
                SetWindowDisplayAffinity(hwnd, affinity);

            };

        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
           
        }

        private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.LWin || e.Key == Key.RWin)
            {
           
                this.ResizeMode = ResizeMode.NoResize;
                this.WindowState = WindowState.Maximized;
                e.Handled = true;
            }
        }
    }
}