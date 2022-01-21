/*********************************************************************
 *
 *  $Id: yocto_datalogger_proxy.cs 45843 2021-08-04 07:51:59Z mvuilleu $
 *
 *  Implements YDataLoggerProxy, the Proxy API for DataLogger
 *
 *  - - - - - - - - - License information: - - - - - - - - -
 *
 *  Copyright (C) 2011 and beyond by Yoctopuce Sarl, Switzerland.
 *
 *  Yoctopuce Sarl (hereafter Licensor) grants to you a perpetual
 *  non-exclusive license to use, modify, copy and integrate this
 *  file into your software for the sole purpose of interfacing
 *  with Yoctopuce products.
 *
 *  You may reproduce and distribute copies of this file in
 *  source or object form, as long as the sole purpose of this
 *  code is to interface with Yoctopuce products. You must retain
 *  this notice in the distributed source file.
 *
 *  You should refer to Yoctopuce General Terms and Conditions
 *  for additional information regarding your rights and
 *  obligations.
 *
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED 'AS IS' WITHOUT
 *  WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
 *  WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS
 *  FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
 *  EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
 *  INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 *  COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR
 *  SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT
 *  LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
 *  CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
 *  BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
 *  WARRANTY, OR OTHERWISE.
 *
 *********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using YoctoLib;


namespace YoctoProxyAPI
{
    //--- (generated code: YDataLogger class start)
    static public partial class YoctoProxyManager
    {
        public static YDataLoggerProxy FindDataLogger(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YDataLogger func = null;
            YDataLoggerProxy res = (YDataLoggerProxy)YFunctionProxy.FindSimilarUnknownFunction("YDataLoggerProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YDataLoggerProxy)YFunctionProxy.FindSimilarKnownFunction("YDataLoggerProxy");
                if (res != null) return res;
                func = YDataLogger.FirstDataLogger();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YDataLoggerProxy)func.get_userData();
                    }
                }
            } else {
                // allow to get datalogger from the name of any function
                int p = name.IndexOf(".");
                if (p > 0) name = name.Substring(0, p) + ".dataLogger";
                func = YDataLogger.FindDataLogger(name);
                if (func.get_userData() != null) {
                    return (YDataLoggerProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YDataLoggerProxy(func, name);
            }
            if (func != null) {
                res.linkToHardware(name);
                if(func.isOnline()) res.arrival();
            }
            return res;
        }
    }

/**
 * <summary>
 *   A non-volatile memory for storing ongoing measured data is available on most Yoctopuce
 *   sensors.
 * <para>
 *   Recording can happen automatically, without requiring a permanent
 *   connection to a computer.
 *   The <c>YDataLogger</c> class controls the global parameters of the internal data
 *   logger. Recording control (start/stop) as well as data retreival is done at
 *   sensor objects level.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YDataLoggerProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a data logger for a given identifier.
         * <para>
         *   The identifier can be specified using several formats:
         * </para>
         * <para>
         * </para>
         * <para>
         *   - FunctionLogicalName
         * </para>
         * <para>
         *   - ModuleSerialNumber.FunctionIdentifier
         * </para>
         * <para>
         *   - ModuleSerialNumber.FunctionLogicalName
         * </para>
         * <para>
         *   - ModuleLogicalName.FunctionIdentifier
         * </para>
         * <para>
         *   - ModuleLogicalName.FunctionLogicalName
         * </para>
         * <para>
         * </para>
         * <para>
         *   This function does not require that the data logger is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YDataLogger.isOnline()</c> to test if the data logger is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a data logger by logical name, no error is notified: the first instance
         *   found is returned. The search is performed first by hardware name,
         *   then by logical name.
         * </para>
         * <para>
         *   If a call to this object's is_online() method returns FALSE although
         *   you are certain that the matching device is plugged, make sure that you did
         *   call registerHub() at application initialization time.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="func">
         *   a string that uniquely characterizes the data logger, for instance
         *   <c>LIGHTMK4.dataLogger</c>.
         * </param>
         * <returns>
         *   a <c>YDataLogger</c> object allowing you to drive the data logger.
         * </returns>
         */
        public static YDataLoggerProxy FindDataLogger(string func)
        {
            return YoctoProxyManager.FindDataLogger(func);
        }
        //--- (end of generated code: YDataLogger class start)
        //--- (generated code: YDataLogger definitions)
        public const int _CurrentRunIndex_INVALID = -1;
        public const long _TimeUTC_INVALID = YAPI.INVALID_LONG;
        public const int _Recording_INVALID = 0;
        public const int _Recording_OFF = 1;
        public const int _Recording_ON = 2;
        public const int _Recording_PENDING = 3;
        public const int _AutoStart_INVALID = 0;
        public const int _AutoStart_OFF = 1;
        public const int _AutoStart_ON = 2;
        public const int _BeaconDriven_INVALID = 0;
        public const int _BeaconDriven_OFF = 1;
        public const int _BeaconDriven_ON = 2;
        public const int _Usage_INVALID = -1;
        public const int _ClearHistory_INVALID = 0;
        public const int _ClearHistory_FALSE = 1;
        public const int _ClearHistory_TRUE = 2;

        // reference to real YoctoAPI object
        protected new YDataLogger _func;
        // property cache
        protected int _recording = _Recording_INVALID;
        protected int _autoStart = _AutoStart_INVALID;
        protected int _beaconDriven = _BeaconDriven_INVALID;
        //--- (end of generated code: YDataLogger definitions)

        //--- (generated code: YDataLogger implementation)
        internal YDataLoggerProxy(YDataLogger hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("DataLogger " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YDataLogger) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YDataLogger hwd = YDataLogger.FindDataLogger(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YDataLogger hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering DataLogger callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type DataLogger available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YDataLogger.FindDataLogger</c> to obtain an object that can control the
         *   corresponding device.
         * </para>
         * </summary>
         * <returns>
         *   an array of strings, each string containing the unique hardwareId
         *   of a device function currently connected.
         * </returns>
         */
        public static new string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YDataLogger it = YDataLogger.FirstDataLogger();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextDataLogger();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _autoStart = _func.get_autoStart()+1;
            _beaconDriven = _func.get_beaconDriven()+1;
        }

        /**
         * <summary>
         *   Returns the current run number, corresponding to the number of times the module was
         *   powered on with the dataLogger enabled at some point.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current run number, corresponding to the number of times the module was
         *   powered on with the dataLogger enabled at some point
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YDataLogger.CURRENTRUNINDEX_INVALID</c>.
         * </para>
         */
        public int get_currentRunIndex()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            res = _func.get_currentRunIndex();
            if (res == YAPI.INVALID_INT) {
                res = _CurrentRunIndex_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the Unix timestamp for current UTC time, if known.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the Unix timestamp for current UTC time, if known
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YDataLogger.TIMEUTC_INVALID</c>.
         * </para>
         */
        public long get_timeUTC()
        {
            long res;
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            res = _func.get_timeUTC();
            if (res == YAPI.INVALID_INT) {
                res = _TimeUTC_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the current UTC time reference used for recorded data.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the current UTC time reference used for recorded data
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_timeUTC(long newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            if (newval == _TimeUTC_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_timeUTC(newval);
        }

        /**
         * <summary>
         *   Returns the current activation state of the data logger.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YDataLogger.RECORDING_OFF</c>, <c>YDataLogger.RECORDING_ON</c> and
         *   <c>YDataLogger.RECORDING_PENDING</c> corresponding to the current activation state of the data logger
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YDataLogger.RECORDING_INVALID</c>.
         * </para>
         */
        public int get_recording()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_recording()+1;
        }

        /**
         * <summary>
         *   Changes the activation state of the data logger to start/stop recording data.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YDataLogger.RECORDING_OFF</c>, <c>YDataLogger.RECORDING_ON</c> and
         *   <c>YDataLogger.RECORDING_PENDING</c> corresponding to the activation state of the data logger to
         *   start/stop recording data
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_recording(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            if (newval == _Recording_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_recording(newval-1);
        }

        // property with cached value for instant access (advertised value)
        /// <value>Current activation state of the data logger.</value>
        public int Recording
        {
            get
            {
                if (_func == null) {
                    return _Recording_INVALID;
                }
                if (_online) {
                    return _recording;
                }
                return _Recording_INVALID;
            }
            set
            {
                setprop_recording(value);
            }
        }

        // private helper for magic property
        private void setprop_recording(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Recording_INVALID) {
                return;
            }
            if (newval == _recording) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_recording(newval-1);
            _recording = newval;
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "OFF") {
                _recording = 1;
            }
            if (value == "ON") {
                _recording = 2;
            }
            if (value == "PENDING") {
                _recording = 3;
            }
        }

        /**
         * <summary>
         *   Returns the default activation state of the data logger on power up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YDataLogger.AUTOSTART_OFF</c> or <c>YDataLogger.AUTOSTART_ON</c>, according to the
         *   default activation state of the data logger on power up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YDataLogger.AUTOSTART_INVALID</c>.
         * </para>
         */
        public int get_autoStart()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_autoStart()+1;
        }

        /**
         * <summary>
         *   Changes the default activation state of the data logger on power up.
         * <para>
         *   Do not forget to call the <c>saveToFlash()</c> method of the module to save the
         *   configuration change.  Note: if the device doesn't have any time source at his disposal when
         *   starting up, it will wait for ~8 seconds before automatically starting to record  with
         *   an arbitrary timestamp
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YDataLogger.AUTOSTART_OFF</c> or <c>YDataLogger.AUTOSTART_ON</c>, according to the
         *   default activation state of the data logger on power up
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_autoStart(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            if (newval == _AutoStart_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_autoStart(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Default activation state of the data logger on power up.</value>
        public int AutoStart
        {
            get
            {
                if (_func == null) {
                    return _AutoStart_INVALID;
                }
                if (_online) {
                    return _autoStart;
                }
                return _AutoStart_INVALID;
            }
            set
            {
                setprop_autoStart(value);
            }
        }

        // private helper for magic property
        private void setprop_autoStart(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _AutoStart_INVALID) {
                return;
            }
            if (newval == _autoStart) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_autoStart(newval-1);
            _autoStart = newval;
        }

        /**
         * <summary>
         *   Returns true if the data logger is synchronised with the localization beacon.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YDataLogger.BEACONDRIVEN_OFF</c> or <c>YDataLogger.BEACONDRIVEN_ON</c>, according to true
         *   if the data logger is synchronised with the localization beacon
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YDataLogger.BEACONDRIVEN_INVALID</c>.
         * </para>
         */
        public int get_beaconDriven()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_beaconDriven()+1;
        }

        /**
         * <summary>
         *   Changes the type of synchronisation of the data logger.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YDataLogger.BEACONDRIVEN_OFF</c> or <c>YDataLogger.BEACONDRIVEN_ON</c>, according to the
         *   type of synchronisation of the data logger
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_beaconDriven(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            if (newval == _BeaconDriven_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_beaconDriven(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>True if the data logger is synchronised with the localization beacon.</value>
        public int BeaconDriven
        {
            get
            {
                if (_func == null) {
                    return _BeaconDriven_INVALID;
                }
                if (_online) {
                    return _beaconDriven;
                }
                return _BeaconDriven_INVALID;
            }
            set
            {
                setprop_beaconDriven(value);
            }
        }

        // private helper for magic property
        private void setprop_beaconDriven(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _BeaconDriven_INVALID) {
                return;
            }
            if (newval == _beaconDriven) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_beaconDriven(newval-1);
            _beaconDriven = newval;
        }

        /**
         * <summary>
         *   Returns the percentage of datalogger memory in use.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the percentage of datalogger memory in use
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YDataLogger.USAGE_INVALID</c>.
         * </para>
         */
        public int get_usage()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            res = _func.get_usage();
            if (res == YAPI.INVALID_INT) {
                res = _Usage_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Clears the data logger memory and discards all recorded data streams.
         * <para>
         *   This method also resets the current run index to zero.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int forgetAllDataStreams()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            return _func.forgetAllDataStreams();
        }

        /**
         * <summary>
         *   Returns a list of <c>YDataSet</c> objects that can be used to retrieve
         *   all measures stored by the data logger.
         * <para>
         * </para>
         * <para>
         *   This function only works if the device uses a recent firmware,
         *   as <c>YDataSet</c> objects are not supported by firmwares older than
         *   version 13000.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a list of <c>YDataSet</c> object.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty list.
         * </para>
         */
        public virtual YDataSetProxy[] get_dataSets()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DataLogger connected");
            }
            int i;
            int arrlen;
            YDataSet[] std_res;
            YDataSetProxy[] proxy_res;
            std_res = _func.get_dataSets().ToArray();
            arrlen = std_res.Length;
            proxy_res = new YDataSetProxy[arrlen];
            i = 0;
            while (i < arrlen) {
                proxy_res[i] = new YDataSetProxy(std_res[i]);
                i = i + 1;
            }
            return proxy_res;
        }
    }
    //--- (end of generated code: YDataLogger implementation)
}

