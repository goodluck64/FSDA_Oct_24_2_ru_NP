using System.Runtime.InteropServices;

namespace UDP_ImageServer;

internal static class OpenFileDialog
{
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OpenFileName ofn);

    public static string Show()
    {
        var ofn = new OpenFileName();

        ofn.lStructSize = Marshal.SizeOf(ofn);
        ofn.lpstrFilter = "PNG Files (*.png)\0*.png";
        ofn.lpstrFile = new string(new char[256]);
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrFileTitle = new string(new char[64]);
        ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
        ofn.lpstrTitle = "Open File Dialog...";

        if (GetOpenFileName(ref ofn))
        {
            return ofn.lpstrFile;
        }

        return string.Empty;
    }
}