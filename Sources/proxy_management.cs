using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;

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
            log("YoctoLabview library init");
            apiMutex.WaitOne();

            LibraryAPIInitialized = YAPI.InitAPI(YAPI.DETECT_NONE, ref errmsg) == YAPI.SUCCESS;
            YAPI.RegisterLogFunction(apilog);
            apiMutex.ReleaseMutex();
            if (!silent)
            {
                string msg = "Yoctopuce API initialization failed (" + errmsg + ")";
                throw new YoctoApiProxyException(msg);
            }
            if (LibraryAPIInitialized)
            {
                InternalStuff.log("registering arrival/removal");
                YAPI.RegisterDeviceArrivalCallback(YFunctionProxy.deviceArrival);
                YAPI.RegisterDeviceRemovalCallback(YFunctionProxy.deviceRemoval);
            }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                InternalStuff.log("Starting inner processing thread...");
                string err = "";
                while (!InnerThreadMustStop)
                {
                    InternalStuff.log("YAPI processing...");
                    try
                    {
                        if (LibraryAPIInitialized) YAPI.UpdateDeviceList(ref err);
                    }
                    catch (Exception e) {
                        InternalStuff.log("YUpdateDeviceList error !!(" + e.Message + ")");
                    }
                    for (int i = 0; i < 20 & !InnerThreadMustStop; i++)
                        try
                        {
                            if (LibraryAPIInitialized) YAPI.Sleep(100, ref err);
                        }
                        catch (Exception e) { InternalStuff.log("YSleep error (" + e.Message + ")"); }

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
            //Console.WriteLine(str);
            return;
            /*
            logMutex.WaitOne();
            try
            {
              File.AppendAllText(@"c:\tmp\log.txt", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff") + " : " + str + "\r\n");
            }
            catch (Exception ) { }
            logMutex.ReleaseMutex();
            */
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
        public static void rgb2hsl(int rgb, ref int hsl)
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
                return;
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
            hsl = ((Int32)H << 16) + ((Int32)S << 8) + L;
        }
    }



    public abstract class YFunctionProxy
    {
        public const string _AdvertisedValue_INVALID = YAPI.INVALID_STRING;
        protected YFunction _func;
        protected String _hwdid = "";
        protected bool _online = false;
        protected YModule _m = null;
        protected string _advertisedValue;
        string _instantiationName;
        string _logicalName = "";
        static List<YFunctionProxy> _allProxies = new List<YFunctionProxy>();

        public bool isUnknown { get { return _func == null; } }
        int FctID = 0;
        static int FctCount = 0;


        public static void freeall()
        {
            _allProxies.Clear();
        }

        public static YFunctionProxy FindSimilarUnknownFunction(String className)
        {
            for (int i = 0; i < _allProxies.Count; i++)
            {
                string c = _allProxies[i].GetType().ToString();
                if ((_allProxies[i].GetType().ToString() == InternalStuff.currentNameSpace + "." + className) && _allProxies[i].isUnknown)
                {
                    return _allProxies[i];
                }
            }
            return null;
        }

        public static YFunctionProxy FindSimilarKnownFunction(String className)
        {
            for (int i = 0; i < _allProxies.Count; i++)
            {
                string c = _allProxies[i].GetType().ToString();
                if ((_allProxies[i].GetType().ToString() == InternalStuff.currentNameSpace + "." + className) && !_allProxies[i].isUnknown)
                {
                    return _allProxies[i];
                }
            }
            return null;
        }


        public static void deviceArrival(YModule m)
        {
            string ms = m.get_serialNumber();
            InternalStuff.log("*** device arrival(" + m.get_serialNumber() + ")");
            string key = ms + ".module";
            string mynamespace = InternalStuff.currentNameSpace;

            // try to find some unknown module proxy can be linked to the new arrival
            InternalStuff.log("*** looking for existing Module proxies");
            for (int j = 0; j < _allProxies.Count; j++)
            {
                if (_allProxies[j].isUnknown)
                {
                    if (_allProxies[j].GetType().ToString() == mynamespace + ".YModuleProxy")
                    {
                        InternalStuff.log(" found");
                        _allProxies[j].linkToHardware(ms);
                        _allProxies[j].arrival();
                    }
                }
                else if (_allProxies[j].get_fullHardwareId() == key)
                {
                    _allProxies[j].linkToHardware(ms);
                    _allProxies[j].arrival();
                }
            }

            string myNameSpace = InternalStuff.currentNameSpace;

            // try to find some unknown function proxy  can be linked to the new arrival
            InternalStuff.log("*** looking for existing Function proxies");
            for (int i = 0; i < m.functionCount(); i++)
            {
                string hwid = ms + "." + m.functionId(i);
                string type = m.functionType(i);
                string basetype = m.functionBaseType(i);

                for (int j = 0; j < _allProxies.Count; j++)
                {
                    if (_allProxies[j].isUnknown)
                    {
                        string proxyType = _allProxies[j].GetType().ToString();
                        if (proxyType == myNameSpace + ".Y" + type + "Proxy" || proxyType == myNameSpace + ".Y" + basetype + "Proxy")
                        {
                            InternalStuff.log(" found " + type);
                            _allProxies[j].linkToHardware(hwid);
                            _allProxies[j].arrival();
                        }
                    }
                    else if (_allProxies[j].get_fullHardwareId() == hwid)
                    {
                        _allProxies[j].linkToHardware(hwid);
                        _allProxies[j].arrival();
                    }
                }
            }

            m.registerConfigChangeCallback(configChangeCallback);
            configChangeCallback(m); // not triggered automatically at register
            InternalStuff.log("Arrival completed");
        }

        public static void deviceRemoval(YModule m)
        {
            string serial = m.get_serialNumber();
            InternalStuff.log("device removal(" + serial + ")");

            for (int j = 0; j < _allProxies.Count; j++)
                if (_allProxies[j].Online)
                    if (_allProxies[j].get_hardwareId().Substring(0, serial.Length) == serial)
                        _allProxies[j].removal();
        }

        private static void configChangeCallback(YModule module)
        {
            string id = module.get_serialNumber();
            InternalStuff.log(" Module " + id + " config change  ");
            if (id == "") return;  // just in case
            foreach (YFunctionProxy f in _allProxies)
            {
                if (f.HardwareId.Length>=id.Length)
                if (f.HardwareId.Substring(0, id.Length) == id) f.moduleConfigHasChanged();
            }
        }

        protected virtual void moduleConfigHasChanged()
        {
            _logicalName = _func.get_logicalName();
        }

        protected virtual void functionArrival()
        {
            moduleConfigHasChanged();
            valueChangeCallback(_func, _func.get_advertisedValue());
        }

        protected virtual void valueChangeCallback(YFunction source, string value)
        {
            InternalStuff.log("new value (" + value + ")");
            _advertisedValue = value;
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal virtual void base_init(YFunction hwd, string instantiationName)
        {
            string errmsg = "";
            _func = hwd;
            _instantiationName = instantiationName;
            InternalStuff.initYoctopuceLibrary(ref errmsg, true);
            if (_func != null)
            {
                try
                {
                    _func.set_userData(this);
                    _hwdid = _func.get_hardwareId();
                    InternalStuff.log(" hwdID = " + _hwdid);
                }
                catch (Exception)
                {
                    InternalStuff.log("Failed to find out HwdID, device is probably offline ");
                }
            }

        }

        // link the instance to a real YoctoAPI object (in subclasses)
        internal virtual void linkToHardware(string hwdName)
        {
            YFunction hwd = YFunction.FindFunction(hwdName);
            base_init(hwd, hwdName);
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YFunction hwd)
        {
            if (hwd == null) return;
            _func = hwd;
            TestOnline();
        }

        public YFunctionProxy(YFunction hwd, string instantiationName)
        {
            InternalStuff.log("Function constructor with " + instantiationName);

            // integrity test
            FctCount++;
            FctID = FctCount;

            base_init(hwd, instantiationName);
            _allProxies.Add(this);
        }

        public void arrival()
        {
            InternalStuff.log(_hwdid + " comes online");
            _online = true;
            functionArrival();
        }

        public void removal()
        {
            _online = false;
            InternalStuff.log(_hwdid + " has been removed");
        }

        public bool TestOnline()
        {
            if (_func == null) return false;
            InternalStuff.log("test if " + _hwdid + "(" + _instantiationName + ")  is online");
            if (!_func.isOnline())
            {
                InternalStuff.log(" Nope, " + _hwdid + "(" + _instantiationName + ") is still offline");
                _online = false;
                return false;
            }
            InternalStuff.log(" Yes, " + _hwdid + "(" + _instantiationName + ") is online");
            _m = _func.get_module();
            _hwdid = _func.get_hardwareId();

            _online = true;
            InternalStuff.log(_hwdid + " is online");
            return true;
        }

        public virtual string get_hardwareId()
        {
            return _hwdid;
        }

        internal string get_fullHardwareId()
        {
            if (_func == null) return "";
            string s = "";
            try
            {
                s = _func.get_hardwareId();
            }
            catch (Exception) { return ""; }

            return s;
        }

        // used for consistency checks, no use for the end user
        public int FunctionID
        {
            get { return FctID; }
        }

        // Public properties
        public string HardwareId
        {
            get
            {
                return _hwdid;
            }
        }

        public bool Online
        {
            get
            {
                return _online;
            }
        }



        public string AdvertisedValue
        {
            get
            {
                if (_func == null) return "";
                return (_online ? _advertisedValue : "");
            }
        }

        public string LogicalName
        {
            get
            {
                if (_func == null) return "";
                return _logicalName;
            }
            set
            {
                if (!YAPI.CheckLogicalName(value)) throw new InvalidDataException("(" + value + ") is not a valid logical name.");
                if (_func == null) return;
                if (!_online) return;
                if (value == _logicalName) return;
                _func.set_logicalName(value);
                _logicalName = value;
            }


        }

        public virtual string[] GetSimilarFunctions()
        {
            return new string[0];
        }

    }

    // this is used in the LabVIEW Generator to force LabVIEW
    // to keep reference in the DLL and prevent it from 
    // unloading it unexpectedly.

    public class refKeeper
     {
       public string getDllActualPath()
        {  return System.Reflection.Assembly.GetExecutingAssembly().Location; 

        }
     }

    static public partial class YoctoProxyManager
    {

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
            if(DNPVersion != expectedVersion && expectedVersion != joker && expectedVersion != "")
            {
                msg = "DotNetProxy library version mismatch (expected " + expectedVersion + ", found " + DNPVersion + " in "+ DNPPath+")";
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
