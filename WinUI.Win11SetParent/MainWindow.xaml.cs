using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Versioning;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.Win11SetParent
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        const int WS_OVERLAPPED = 0x00000000;
        const int WS_CAPTION = 0x00C00000;
        const int WS_SYSMENU = 0x00080000;
        const int WS_THICKFRAME = 0x00040000;
        const int WS_MINIMIZEBOX = 0x00020000;
        const int WS_MAXIMIZEBOX = 0x00010000;

        int WS_CHILD = 0x40000000;
        int WS_VISIBLE = 0x10000000;
        int WS_BORDER = 0x00800000;
        int WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;

        [DllImport("USER32.dll", ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [SupportedOSPlatform("windows5.0")]
        internal static extern HWND SetParent(HWND hWndChild, HWND hWndNewParent);

        [DllImport("USER32.dll", ExactSpelling = true, EntryPoint = "SetWindowLongW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [SupportedOSPlatform("windows5.0")]
        internal static extern int SetWindowLong(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, int dwNewLong);
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Window() { Content = new Grid() { Background = new SolidColorBrush(Colors.DarkBlue)} };
            newWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(5000,5000));

            var appWindow = newWindow.AppWindow;
            newWindow.Activate();

            var childWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(newWindow);
            var parentWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);

            var childHandle = new HWND(childWindowHandle);
            var parentHandle = new HWND(parentWindowHandle);

            SetParent(childHandle, parentHandle);
            SetWindowLong(childHandle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, WS_OVERLAPPEDWINDOW);
            //SetWindowLong(childHandle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, WS_CHILD | WS_VISIBLE | WS_BORDER);

            appWindow.Resize(new Windows.Graphics.SizeInt32(200, 200));

        }
    }

    internal enum WINDOW_LONG_PTR_INDEX
    {
        GWL_EXSTYLE = -20,
        GWLP_HINSTANCE = -6,
        GWLP_HWNDPARENT = -8,
        GWLP_ID = -12,
        GWL_STYLE = -16,
        GWLP_USERDATA = -21,
        GWLP_WNDPROC = -4,
        GWL_HINSTANCE = -6,
        GWL_ID = -12,
        GWL_USERDATA = -21,
        GWL_WNDPROC = -4,
        GWL_HWNDPARENT = -8,
    }

    internal readonly partial struct HWND : IEquatable<HWND>
    {
        internal readonly IntPtr Value;

        internal HWND(IntPtr value) => this.Value = value;

        internal static HWND Null => default;

        internal bool IsNull => Value == default;

        public static implicit operator IntPtr(HWND value) => value.Value;

        public static explicit operator HWND(IntPtr value) => new HWND(value);

        public static bool operator ==(HWND left, HWND right) => left.Value == right.Value;

        public static bool operator !=(HWND left, HWND right) => !(left == right);

        public bool Equals(HWND other) => this.Value == other.Value;

        public override bool Equals(object obj) => obj is HWND other && this.Equals(other);

        public override int GetHashCode() => this.Value.GetHashCode();
    }
}
