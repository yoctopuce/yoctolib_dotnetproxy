using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using YoctoLib;

namespace YoctoProxyAPI
{

    class YoctoApiProxyException : Exception
    {
        public YoctoApiProxyException(string message) : base(message)
        {
            InternalStuff.log(message);
        }
    }

    static internal class InternalStuff
    {

        private static Mutex apiMutex = null;
        private static Mutex logMutex = null;
        private static bool LibraryAPIInitialized = false;
        private static bool InnerThreadMustStop = false;

        const int LogBufferMaxCount = 1000;
        private static List<String> logBuffer = new List<string>() ;


        public static void apilog(string msg)
        {
            InternalStuff.log(msg);

        }

        public static string currentNameSpace
        {
            get
            {
                var myType = typeof(InternalStuff);
                return myType.Namespace;
            }
        }

    public static bool initYoctopuceLibrary(ref string errmsg, bool silent)
    {
      if (LibraryAPIInitialized) return true;
      log(".NET Proxy library initialization");
      apiMutex.WaitOne();

      LibraryAPIInitialized = YAPI.InitAPI(YAPI.DETECT_NONE, ref errmsg) == YAPI.SUCCESS;
      YAPI.RegisterLogFunction(apilog);
      apiMutex.ReleaseMutex();
      if (!silent)
      {
        string msg = "Yoctopuce low-level API initialization failed (" + errmsg + ")";
        throw new YoctoApiProxyException(msg);
      }
      if (LibraryAPIInitialized)
      {
        //InternalStuff.log("registering arrival/removal");
        YAPI.RegisterDeviceArrivalCallback(YFunctionProxy.deviceArrival);
        YAPI.RegisterDeviceRemovalCallback(YFunctionProxy.deviceRemoval);
      }

      new Thread(() =>
      {
        Thread.CurrentThread.IsBackground = true;
        InternalStuff.log("Starting inner processing thread...");
        bool aborting = false;
        string err = "";
        while (!InnerThreadMustStop && !aborting)
        {

          //InternalStuff.log("YAPI processing...");
           try { if (LibraryAPIInitialized) YAPI.UpdateDeviceList(ref err); }
          catch (System.Threading.ThreadAbortException)
          {
            InternalStuff.log("Inner processing thead is aborting during UpdateDeviceList!");
            aborting = true;
            YAPI.FreeAPI();
            LibraryAPIInitialized = false;
            InternalStuff.log("API has been freed in a hurry");
          }
          catch (Exception e) { InternalStuff.log("YUpdateDeviceList error !!(" + e.Message + ")"); }
          for (int i = 0; i < 20 & !InnerThreadMustStop & !aborting; i++)
            try
            {
              if (LibraryAPIInitialized) YAPI.Sleep(100, ref err);
            }
            catch (System.Threading.ThreadAbortException)
            {
              InternalStuff.log("Inner processing thead is aborting during ysleep!");
              aborting = true;
              YAPI.FreeAPI();
              LibraryAPIInitialized = false;
              InternalStuff.log("API has been freed in a hurry");
            }
            catch (Exception e){InternalStuff.log("YSleep error (" + e.GetType().ToString() + " " + e.Message + ")! ");}

        }

        InternalStuff.log("Stopping inner processing thread...");


        InnerThreadMustStop = false;
      }).Start();




      return LibraryAPIInitialized;
    }

    public static void FreeLibrary()
        {
            InternalStuff.log("Freeing  Yoctopuce library");
            if (!LibraryAPIInitialized) return;

            // wait for the inner thread to stop;
            InnerThreadMustStop = true;
            int retries = 0;
            while (InnerThreadMustStop && retries < 50) { Thread.Sleep(100); retries++; }

            apiMutex.WaitOne();
            YAPI.FreeAPI();
            LibraryAPIInitialized = false;
            apiMutex.ReleaseMutex();
        }

        static InternalStuff()
        {
            logMutex = new Mutex();
            InternalStuff.log("+---------------------+");
            InternalStuff.log("|       START         |");
            InternalStuff.log("+---------------------+");
            InternalStuff.log("DLL path is " + System.Reflection.Assembly.GetExecutingAssembly().Location);
            apiMutex = new Mutex();
        }

        public static void log(string str)
        {
           string line = DateTime.Now.ToString("yyyyMMdd hh:mm:ss.fff ") + str.TrimEnd('\r', '\n');
          logBuffer.Add(line);
         //  File.AppendAllText(@"C:\tmp\@log.txt", line+"\r\n");
         if (logBuffer.Count > LogBufferMaxCount) logBuffer.RemoveRange(0, logBuffer.Count - LogBufferMaxCount);

        }


        public static string getLogLine(string lastline)
        {
          if (logBuffer.Count <= 0) return "";
          if (lastline == "") return logBuffer[0];
          if (lastline == logBuffer[logBuffer.Count - 1]) return "";
          if ((logBuffer.Count>=2) && ( logBuffer[logBuffer.Count - 2]== lastline)) return logBuffer[logBuffer.Count - 1];
          int index = logBuffer.LastIndexOf(lastline);
          if (index < 0) return "";
          return logBuffer[index+1];
        }


        private static UInt16 hsl2rgbInt(UInt32 temp1, UInt32 temp2, UInt16 temp3)
        {
            if (temp3 >= 170) return (UInt16)((temp1 + 127) / 255);
            if (temp3 > 42)
            {
                if (temp3 <= 127) return (UInt16)((temp2 + 127) / 255);
                temp3 = (UInt16)(170 - temp3);
            }
            return (UInt16)((temp1 * 255 + (Int32)(temp2 - temp1) * (6 * temp3) + 32512) / 65025);
        }


        // Convert an RGB value to HSL using the nearest value to current HSL value
        public static int rgb2hsl(int rgb, int hsl)
        {
            UInt16 R, G, B, H, S, L;
            UInt32 temp1, temp2, rgbVal;
            UInt16 temp3, lmax, lmin, corr, divi;

            // First convert current HSL value to rgb, to check if this is actually the same
            H = (byte)(hsl >> 16);
            S = (byte)(hsl >> 8);
            L = (byte)hsl;
            if (S == 0)
            {
                R = G = B = L;
            } else
            {
                if (L <= 127) temp2 = L * (UInt32)(255 + S);
                else temp2 = (UInt32)((L + S) * 255 - L * S);
                temp1 = (UInt32)(510 * L - temp2);
                // R
                temp3 = (UInt16)(H + 85);
                if (temp3 > 255) temp3 = (UInt16)(temp3 - 255);
                R = hsl2rgbInt(temp1, temp2, temp3);
                // G
                temp3 = H;
                if (temp3 > 255) temp3 = (UInt16)(temp3 - 255);
                G = hsl2rgbInt(temp1, temp2, temp3);
                // B
                if (H >= 85) temp3 = (UInt16)(H - 85);
                else temp3 = (UInt16)(H + 170);
                B = hsl2rgbInt(temp1, temp2, temp3);
                if (R > 255) R = 255;  // just in case
                if (G > 255) G = 255;
                if (B > 255) B = 255;
            }
            rgbVal = ((UInt32)R << 16) + ((UInt32)G << 8) + B;
            if (rgbVal == rgb)
            {   // no change
                return hsl;
            }
            // We know that the rgb value is different
            // Now compute the HSL value closest to current value
            R = (byte)(rgb >> 16);
            G = (byte)(rgb >> 8);
            B = (byte)rgb;
            lmax = (R > G ? R : G);
            lmin = (R < G ? R : G);
            if (B > lmax) lmax = B;
            if (B < lmin) lmin = B;
            L = (UInt16)((lmax + lmin + 1) / 2);
            if (lmax == lmin)
            {
                H = S = 0;
            } else
            {
                corr = (UInt16)((lmax + lmin) / 2);
                if (L <= 127) S = (UInt16)((255 * (lmax - lmin) + corr) / (lmax + lmin));
                else S = (UInt16)((255 * (lmax - lmin) + 255 - corr) / (510 - (lmax + lmin)));
                corr = (UInt16)(3 * (lmax - lmin));
                divi = (UInt16)(2 * corr);
                if (R == lmax) { H = 0; R = G; G = B; }
                else if (G == lmax) { H = 85; G = R; R = B; }
                else { H = 170; }
                if (R >= G)
                    H += (UInt16)((255 * (R - G) + corr) / divi);
                else
                    H += (UInt16)(255 - (UInt16)((255 * (G - R) - corr) / divi));
                if (H > 255) H -= 255;
                if (S > 255) S = 255; // just in case
                if (L > 255) L = 255;
            }
            return ((Int32)H << 16) + ((Int32)S << 8) + L;
        }
    }

    // this is used in the LabVIEW Generator to force LabVIEW
    // to keep reference in the DLL and prevent it from
    // unloading it unexpectedly.

    public class refKeeper
    {
        public string getDllActualPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }
    }


    public class YAPIProxy
    {
        protected static string apiversion = "";
        protected static string dllpath = "";
        protected static string dllarch = "";

        /**
         * <summary>
         *   Returns the version identifier for the Yoctopuce library in use.
         * <para>
         *   The version is a string in the form <c>"Major.Minor.Build"</c>,
         *   for instance <c>"1.01.5535"</c>. For languages using an external
         *   DLL (for instance C#, VisualBasic or Delphi), the character string
         *   includes as well the DLL version, for instance
         *   <c>"1.01.5535 (1.01.5439)"</c>.
         * </para>
         * <para>
         *   If you want to verify in your code that the library version is
         *   compatible with the version that you have used during development,
         *   verify that the major number is strictly equal and that the minor
         *   number is greater or equal. The build number is not relevant
         *   with respect to the library compatibility.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a character string describing the library version.
         * </returns>
         */
        public static string GetAPIVersion()
        {
            return YoctoProxyManager.GetAPIVersion();
        }

        public static string APIVersion
        {
            get {
                if(apiversion == "") {
                    apiversion = YoctoProxyManager.GetAPIVersion();
                }
                return apiversion;
            }
        }

        /**
         * <summary>
         *   Returns the paths of the DLLs for the Yoctopuce library in use.
         * <para>
         *   For architectures that require multiple DLLs, in particular when using
         *   a .NET assembly DLL, the returned string takes the form
         *   <c>"DotNetProxy=/...; yapi=/...;"</c>,
         *   where the first path corresponds to the .NET assembly DLL and the
         *   second path corresponds to the low-level communication library.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a character string describing the DLL path.
         * </returns>
         */
        public static string GetDllPath()
        {
            return YoctoProxyManager.GetDllPath();
        }

        public static string DllPath
        {
            get {
                if(dllpath == "") {
                    dllpath = YoctoProxyManager.GetDllPath();
                }
                return dllpath;
            }
        }

        /**
         * <summary>
         *   Returns the system architecture for the Yoctopuce communication library in use.
         * <para>
         *   On Windows, the architecture can be <c>"Win32"</c> or <c>"Win64"</c>.
         *   On ARM machines, the architecture is <c>"Armhf32"</c> or <c>"Aarch64"</c>.
         *   On other Linux machines, the architecture is <c>"Linux32"</c> or <c>"Linux64"</c>.
         *   On MacOS, the architecture is <c>"MacOs32"</c> or <c>"MacOs64"</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a character string describing the system architecture of the
         *   low-level communication library.
         * </returns>
         */
        public static string GetDllArchitecture()
        {
            return YoctoProxyManager.GetDllArchitecture();
        }

        public static string DllArchitecture
        {
            get {
                if(dllarch == "") {
                    dllarch = YoctoProxyManager.GetDllArchitecture();
                }
                return dllarch;
            }
        }

        /**
         * <summary>
         *   Setup the Yoctopuce library to use modules connected on a given machine.
         * <para>
         *   Idealy this
         *   call will be made once at the begining of your application.  The
         *   parameter will determine how the API will work. Use the following values:
         * </para>
         * <para>
         *   <b>usb</b>: When the <c>usb</c> keyword is used, the API will work with
         *   devices connected directly to the USB bus. Some programming languages such a JavaScript,
         *   PHP, and Java don't provide direct access to USB hardware, so <c>usb</c> will
         *   not work with these. In this case, use a VirtualHub or a networked YoctoHub (see below).
         * </para>
         * <para>
         *   <b><i>x.x.x.x</i></b> or <b><i>hostname</i></b>: The API will use the devices connected to the
         *   host with the given IP address or hostname. That host can be a regular computer
         *   running a VirtualHub, or a networked YoctoHub such as YoctoHub-Ethernet or
         *   YoctoHub-Wireless. If you want to use the VirtualHub running on you local
         *   computer, use the IP address 127.0.0.1.
         * </para>
         * <para>
         *   <b>callback</b>: that keyword make the API run in "<i>HTTP Callback</i>" mode.
         *   This a special mode allowing to take control of Yoctopuce devices
         *   through a NAT filter when using a VirtualHub or a networked YoctoHub. You only
         *   need to configure your hub to call your server script on a regular basis.
         *   This mode is currently available for PHP and Node.JS only.
         * </para>
         * <para>
         *   Be aware that only one application can use direct USB access at a
         *   given time on a machine. Multiple access would cause conflicts
         *   while trying to access the USB modules. In particular, this means
         *   that you must stop the VirtualHub software before starting
         *   an application that uses direct USB access. The workaround
         *   for this limitation is to setup the library to use the VirtualHub
         *   rather than direct USB access.
         * </para>
         * <para>
         *   If access control has been activated on the hub, virtual or not, you want to
         *   reach, the URL parameter should look like:
         * </para>
         * <para>
         *   <c>http://username:password@address:port</c>
         * </para>
         * <para>
         *   You can call <i>RegisterHub</i> several times to connect to several machines. On
         *   the other hand, it is useless and even counterproductive to call <i>RegisterHub</i>
         *   with to same address multiple times during the life of the application.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="url">
         *   a string containing either <c>"usb"</c>,<c>"callback"</c> or the
         *   root URL of the hub to monitor
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public static string RegisterHub(string url)
        {
            return YoctoProxyManager.RegisterHub(url);
        }

        /**
         * <summary>
         *   Fault-tolerant alternative to <c>yRegisterHub()</c>.
         * <para>
         *   This function has the same
         *   purpose and same arguments as <c>yRegisterHub()</c>, but does not trigger
         *   an error when the selected hub is not available at the time of the function call.
         *   This makes it possible to register a network hub independently of the current
         *   connectivity, and to try to contact it only when a device is actively needed.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="url">
         *   a string containing either <c>"usb"</c>,<c>"callback"</c> or the
         *   root URL of the hub to monitor
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public static string PreregisterHub(string url)
        {
            return YoctoProxyManager.PreRegisterHub(url);
        }

        /**
         * <summary>
         *   Retrieves Yoctopuce low-level library diagnostic logs.
         * <para>
         *   This method allows to progessively retrieve API logs. The interface is line-based:
         *   it must called it within a loop until the returned value is an empty string.
         *   Make sure to exit the loop when an empty string is returned, as feeding an empty
         *   string into the <c>lastLogLine</c> parameter for the next call would restart
         *   enumerating logs from the oldest message available.
         * </para>
         * </summary>
         * <param name="lastLogLine">
         *   On first call, provide an empty string.
         *   On subsequent calls, provide the last log line returned by <c>GetLog()</c>.
         * </param>
         * <returns>
         *   a string with the log line immediately following the one given in argument,
         *   if such line exist. Returns an empty string otherwise, when completed.
         * </returns>
         */
        public static string GetLog(string lastLogLine)
        {
          return YoctoProxyManager.getLogLine(lastLogLine);
        }

        /**
         * <summary>
         *   Test if the hub is reachable.
         * <para>
         *   This method do not register the hub, it only test if the
         *   hub is usable. The url parameter follow the same convention as the <c>yRegisterHub</c>
         *   method. This method is useful to verify the authentication parameters for a hub. It
         *   is possible to force this method to return after mstimeout milliseconds.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="url">
         *   a string containing either <c>"usb"</c>,<c>"callback"</c> or the
         *   root URL of the hub to monitor
         * </param>
         * <param name="mstimeout">
         *   the number of millisecond available to test the connection.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure returns a negative error code.
         * </para>
         */
        public static string TestHub(string url, int mstimeout)
        {
            return YoctoProxyManager.TestHub(url, mstimeout);
        }

        /**
         * <summary>
         *   Waits for all pending communications with Yoctopuce devices to be
         *   completed then frees dynamically allocated resources used by
         *   the Yoctopuce library.
         * <para>
         * </para>
         * <para>
         *   From an operating system standpoint, it is generally not required to call
         *   this function since the OS will automatically free allocated resources
         *   once your program is completed. However there are two situations when
         *   you may really want to use that function:
         * </para>
         * <para>
         *   - Free all dynamically allocated memory blocks in order to
         *   track a memory leak.
         * </para>
         * <para>
         *   - Send commands to devices right before the end
         *   of the program. Since commands are sent in an asynchronous way
         *   the program could exit before all commands are effectively sent.
         * </para>
         * <para>
         *   You should not call any other library function after calling
         *   <c>yFreeAPI()</c>, or your program will crash.
         * </para>
         * </summary>
         */
        public static void FreeAPI()
        {
            YoctoProxyManager.FreeAPI();
        }

        /**
         * <summary>
         *   Modifies the network connection delay for <c>yRegisterHub()</c> and <c>yUpdateDeviceList()</c>.
         * <para>
         *   This delay impacts only the YoctoHubs and VirtualHub
         *   which are accessible through the network. By default, this delay is of 20000 milliseconds,
         *   but depending or you network you may want to change this delay,
         *   gor example if your network infrastructure is based on a GSM connection.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="networkMsTimeout">
         *   the network connection delay in milliseconds.
         * @noreturn
         * </param>
         */
        public static void SetNetworkTimeout(int networkMsTimeout)
        {
            YAPIProxy.NetworkTimeout = networkMsTimeout;
        }

        /**
         * <summary>
         *   Returns the network connection delay for <c>yRegisterHub()</c> and <c>yUpdateDeviceList()</c>.
         * <para>
         *   This delay impacts only the YoctoHubs and VirtualHub
         *   which are accessible through the network. By default, this delay is of 20000 milliseconds,
         *   but depending or you network you may want to change this delay,
         *   for example if your network infrastructure is based on a GSM connection.
         * </para>
         * </summary>
         * <returns>
         *   the network connection delay in milliseconds.
         * </returns>
         */
        public static int GetNetworkTimeout()
        {
            return YAPIProxy.NetworkTimeout;
        }

        public static int NetworkTimeout
        {
            get {
                return YoctoProxyManager.NetworkTimeout;
            }
            set {
                YoctoProxyManager.NetworkTimeout = value;
            }
        }
    }

  static public partial class YoctoProxyManager
  {
    private static YAPIProxy proxyInterface = new YAPIProxy();

    public static string getLogLine(string lastline)
    {
      return InternalStuff.getLogLine(lastline);
    }

    public static YAPIProxy GetAPIProxy()
    {
      return proxyInterface;
    }

    public static refKeeper getRefKeeperObject()
    {
      return new refKeeper();
    }

    public static string DllPath
    {
      get
      {
        string DNPPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string YAPIPath = YAPI.GetYAPIDllPath();
        return "DotNetProxy=" + DNPPath + "; yapi=" + YAPIPath + ";";
      }
    }

    public static string DllVersion
    {
      get
      {
        string version = default(string);
        string date = default(string);
        YAPI.apiGetAPIVersion(ref version, ref date);
        return "PATCH_WITH_BUILD (" + version + ")";
      }
    }

    public static string DllArchitecture
    {
      get
      {
        return YAPI.GetDllArchitecture();
      }
    }

    public static string CheckDllVersion(string expectedVersion)
    {
      string DNPPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
      string DNPVersion = "PATCH_WITH_BUILD";
      string joker = "PATCH" + "_WITH_" + "BUILD"; // split to avoid auto-replacement
      string msg;

      // Disable all checks if DotNetProxy is a debug version
      if (DNPVersion == joker || DNPVersion == "") return "";

      // Check compatibility with applicative layer
      if (DNPVersion != expectedVersion && expectedVersion != joker && expectedVersion != "")
      {
        msg = "DotNetProxy library version mismatch (expected " + expectedVersion + ", found " + DNPVersion + " in " + DNPPath + ")";
        return msg;
      }

      // Check compatibility with YAPI layer
      string YAPIVersion = default(string);
      string YAPIDate = default(string);
      try
      {
        YAPI.apiGetAPIVersion(ref YAPIVersion, ref YAPIDate);
      }
      catch (System.DllNotFoundException ex)
      {
        msg = "Unable to load YAPI library (" + ex.Message + ")";
        return msg;
      }
      catch (System.BadImageFormatException)
      {
        if (IntPtr.Size == 4)
        {
          msg = "YAPI library version mismatch (using 64 bits yapi.dll with 32 bit application)";
        }
        else
        {
          msg = "YAPI library version mismatch (using 32 bits yapi.dll with 64 bit application)";
        }
        return msg;
      }
      if (DNPVersion != YAPIVersion && YAPIVersion != joker && YAPIVersion != "")
      {
        string YAPIPath = YAPI.GetYAPIDllPath();
        msg = "YAPI library version mismatch (expected " + DNPVersion + ", found " + YAPIVersion + " in " + YAPIPath + ")";
        return msg;
      }

      return "";
    }

    public static String GetAPIVersion()
    {
        string check = YoctoProxyManager.CheckDllVersion("");
        if(check != "") {
            return "ERROR: "+check;
        }
        return YAPI.GetAPIVersion();
    }

    public static String GetDllPath()
    {
        string check = YoctoProxyManager.CheckDllVersion("");
        if(check != "") {
            return "ERROR: "+check;
        }
        return YoctoProxyManager.DllPath;
    }

    public static String GetDllArchitecture()
    {
        string check = YoctoProxyManager.CheckDllVersion("");
        if(check != "") {
            return "ERROR: "+check;
        }
        return YoctoProxyManager.DllArchitecture;
    }

    public static string RegisterHub(string addr)
    {
      string errmsg = "";
      if (!InternalStuff.initYoctopuceLibrary(ref errmsg, true))
      {
        string msg = addr + " Yoctopuce API init failed (" + errmsg + ")";
        return msg;
      }

      InternalStuff.log("Registering Hub (" + addr + ") ...");
      if (YAPI.RegisterHub(addr, ref errmsg) != YAPI.SUCCESS)
      {
        string msg = addr + " Hub registering failed (" + errmsg + ")";
        return msg;
      }

      try { YAPI.UpdateDeviceList(ref errmsg); } catch (Exception) { }
      return "";
    }

    public static string PreRegisterHub(string addr)
    {
      string errmsg = "";
      InternalStuff.initYoctopuceLibrary(ref errmsg, true);

      InternalStuff.log("Pre-registering Hub (" + addr + ") ...");
      if (YAPI.PreregisterHub(addr, ref errmsg) != YAPI.SUCCESS)
      {
        string msg = addr + " Hub  asynch registering failed (" + errmsg + ")";
        return msg;
      }

      try { YAPI.UpdateDeviceList(ref errmsg); } catch (Exception) { }
      return "";
    }

    public static void FreeAPI()
    {
      InternalStuff.log("Freeing API...");
      InternalStuff.FreeLibrary();
      YFunctionProxy.freeall();
    }

    public static int NetworkTimeout
    {
      get { return YAPI.GetNetworkTimeout(); }
      set { YAPI.SetNetworkTimeout(value); }
    }

    public static string TestHub(string url, int mstimeout)
    {
      string res = "";
      YAPI.TestHub(url, mstimeout, ref res);
      return res;
    }

  }


    public class YDataloggerContext
    {
        YSensor _sensor = null;
        YDataSet _dataset = null;
        int _progress = -1;
        private List<YMeasure> _preview = null;
        private List<YMeasure> _data = null;

        internal YDataloggerContext(YSensor s, int start, int stop)
        {
            if (start < 0) start = 0;
            if (stop < 0) stop = 0;

            _sensor = s;
            _dataset = _sensor.get_recordedData(start, stop);
            _progress = _dataset.loadMore();
            _preview = _dataset.get_preview();

        }

        public int loadMore()
        {
            if (_dataset == null) throw new Exception("YDataloggerContext data has already been freed");
            _progress = _dataset.loadMore();
            if (_progress >= 100)
            { _data = _dataset.get_measures(); }
            return _progress;
        }


        public int progress { get { return _progress; } }


        public int get_previewRecordsCount { get { return _preview.Count; } }

        public double get_previewMinAt(int index)
        {
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than data count " + _preview.Count.ToString() + " )");
            return _preview[index].get_minValue();
        }

        public double get_previewMaxAt(int index)
        {
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than data count " + _preview.Count.ToString() + " )");
            return _preview[index].get_maxValue();
        }

        public double get_previewAvgAt(int index)
        {
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than data count " + _preview.Count.ToString() + " )");
            return _preview[index].get_averageValue();
        }

        public double get_previewStartTimeAt(int index)
        {
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than data count " + _preview.Count.ToString() + " )");
            return _preview[index].get_startTimeUTC();

        }

        public double get_previewEndTimeAt(int index)
        {
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _preview.Count) throw new Exception("Preview index (" + index.ToString() + ") is larger than data count " + _preview.Count.ToString() + " )");
            return _preview[index].get_endTimeUTC();
        }

        public int get_dataRecordsCount
        {
            get
            {
                if (_data == null) return -1;
                return _data.Count;
            }
        }

        public double get_dataMinAt(int index)
        {
            if (_data == null) throw new Exception("data not loaded yet");
            if (index < 0) throw new Exception("data index cannot be negative");
            if (index >= _data.Count) throw new Exception("data index (" + index.ToString() + ") is larger than data count " + _data.Count.ToString() + " )");
            return _data[index].get_minValue();
        }

        public double get_dataMaxAt(int index)
        {
            if (_data == null) throw new Exception("data not loaded yet");
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _data.Count) throw new Exception("data index (" + index.ToString() + ") is larger than data count " + _data.Count.ToString() + " )");
            return _data[index].get_maxValue();
        }

        public double get_dataAvgAt(int index)
        {
            if (_data == null) throw new Exception("data not loaded yet");
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _data.Count) throw new Exception("data index (" + index.ToString() + ") is larger than data count " + _data.Count.ToString() + " )");
            return _data[index].get_averageValue();
        }

        public double get_dataStartTimeAt(int index)
        {
            if (_data == null) throw new Exception("data not loaded yet");
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _data.Count) throw new Exception("data index (" + index.ToString() + ") is larger than data count " + _data.Count.ToString() + " )");
            return _data[index].get_startTimeUTC();

        }

        public double get_dataEndTimeAt(int index)
        {
            if (_data == null) throw new Exception("data not loaded yet");
            if (index < 0) throw new Exception("Preview index cannot be negative");
            if (index >= _data.Count) throw new Exception("data index (" + index.ToString() + ") is larger than data count " + _data.Count.ToString() + " )");
            return _data[index].get_endTimeUTC();
        }

        public void freeData()
        {
            _data.Clear();
            _preview.Clear();
            _progress = -1;
            _dataset = null;
        }

    }

    static public class YDataLoggerContents
    {
        public static YDataloggerContext init(string name, int start, int stop)
        {
            YSensor s = YSensor.FindSensor(name);
            if (!s.isOnline()) throw new Exception("sensor " + name + " is offline or does not exist.");
            YDataloggerContext ctx = new YDataloggerContext(s, start, stop);
            return ctx;
        }
    }


}
