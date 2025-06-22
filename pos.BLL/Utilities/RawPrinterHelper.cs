using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace pos_system.pos.BLL.Utilities
{
    public class RawPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        public static bool SendStringToPrinter(string printerName, string text)
        {
            IntPtr hPrinter = IntPtr.Zero;
            DOCINFOA di = new DOCINFOA
            {
                pDocName = "Barcode Label",
                pDataType = "RAW"
            };
            Debug.WriteLine("Data in here");
            try
            {
                // Open printer
                if (!OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Could not open printer. Error code: {error}");
                }

                // Start document
                if (!StartDocPrinter(hPrinter, 1, di))
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Failed to start document. Error code: {error}");
                }

                // Start page
                if (!StartPagePrinter(hPrinter))
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Failed to start page. Error code: {error}");
                }

                // Get bytes to send
                byte[] bytes = Encoding.ASCII.GetBytes(text);
                int bytesLength = bytes.Length;
                IntPtr pBytes = Marshal.AllocCoTaskMem(bytesLength);
                Marshal.Copy(bytes, 0, pBytes, bytesLength);

                // Send data
                if (!WritePrinter(hPrinter, pBytes, bytesLength, out int bytesWritten))
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Failed to send data to printer. Error code: {error}");
                }

                // Verify all bytes were written
                if (bytesWritten != bytesLength)
                {
                    throw new Exception($"Incomplete write. Sent {bytesWritten} of {bytesLength} bytes");
                }

                return true;
            }
            finally
            {
                // End page and document
                if (hPrinter != IntPtr.Zero)
                {
                    EndPagePrinter(hPrinter);
                    EndDocPrinter(hPrinter);
                    ClosePrinter(hPrinter);
                }

                // Free memory if needed
                // (Marshal.FreeCoTaskMem is not needed here as it's in try block)
            }
        }
        public static bool SendBytesToPrinter(string printerName, byte[] bytes)
        {
            nint hPrinter = nint.Zero;
            DOCINFOA di = new DOCINFOA();
            di.pDocName = "POS Receipt";
            di.pDataType = "RAW";

            if (!OpenPrinter(printerName, out hPrinter, nint.Zero))
                return false;

            if (StartDocPrinter(hPrinter, 1, di))
            {
                if (StartPagePrinter(hPrinter))
                {
                    int dwWritten = 0;
                    nint pBytes = Marshal.AllocCoTaskMem(bytes.Length);
                    Marshal.Copy(bytes, 0, pBytes, bytes.Length);

                    bool success = WritePrinter(hPrinter, pBytes, bytes.Length, out dwWritten);
                    Marshal.FreeCoTaskMem(pBytes);

                    EndPagePrinter(hPrinter);
                }
                EndDocPrinter(hPrinter);
            }
            ClosePrinter(hPrinter);
            return true;
        }
    }
}