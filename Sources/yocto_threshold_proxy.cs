/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YThresholdProxy, the Proxy API for Threshold
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
using System.Globalization;
using System.Text.RegularExpressions;
using YoctoLib;

namespace YoctoProxyAPI
{
    //--- (YThreshold class start)
    static public partial class YoctoProxyManager
    {
        public static YThresholdProxy FindThreshold(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YThreshold func = null;
            YThresholdProxy res = (YThresholdProxy)YFunctionProxy.FindSimilarUnknownFunction("YThresholdProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YThresholdProxy)YFunctionProxy.FindSimilarKnownFunction("YThresholdProxy");
                if (res != null) return res;
                func = YThreshold.FirstThreshold();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YThresholdProxy)func.get_userData();
                    }
                }
            } else {
                func = YThreshold.FindThreshold(name);
                if (func.get_userData() != null) {
                    return (YThresholdProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YThresholdProxy(func, name);
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
 *   The <c>Threshold</c> class allows you define a threshold on a Yoctopuce sensor
 *   to trigger a predefined action, on specific devices where this is implemented.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YThresholdProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a threshold function for a given identifier.
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
         *   This function does not require that the threshold function is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YThreshold.isOnline()</c> to test if the threshold function is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a threshold function by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the threshold function, for instance
         *   <c>MyDevice.threshold1</c>.
         * </param>
         * <returns>
         *   a <c>YThreshold</c> object allowing you to drive the threshold function.
         * </returns>
         */
        public static YThresholdProxy FindThreshold(string func)
        {
            return YoctoProxyManager.FindThreshold(func);
        }
        //--- (end of YThreshold class start)
        //--- (YThreshold definitions)
        public const int _ThresholdState_INVALID = 0;
        public const int _ThresholdState_SAFE = 1;
        public const int _ThresholdState_ALERT = 2;
        public const string _TargetSensor_INVALID = YAPI.INVALID_STRING;
        public const double _AlertLevel_INVALID = Double.NaN;
        public const double _SafeLevel_INVALID = Double.NaN;

        // reference to real YoctoAPI object
        protected new YThreshold _func;
        // property cache
        protected int _thresholdState = _ThresholdState_INVALID;
        protected double _alertLevel = _AlertLevel_INVALID;
        protected double _safeLevel = _SafeLevel_INVALID;
        //--- (end of YThreshold definitions)

        //--- (YThreshold implementation)
        internal YThresholdProxy(YThreshold hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Threshold " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YThreshold) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YThreshold hwd = YThreshold.FindThreshold(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YThreshold hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Threshold callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Threshold available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YThreshold.FindThreshold</c> to obtain an object that can control the
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
            YThreshold it = YThreshold.FirstThreshold();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextThreshold();
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
            _alertLevel = _func.get_alertLevel();
            _safeLevel = _func.get_safeLevel();
        }

        /**
         * <summary>
         *   Returns current state of the threshold function.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YThreshold.THRESHOLDSTATE_SAFE</c> or <c>YThreshold.THRESHOLDSTATE_ALERT</c>, according
         *   to current state of the threshold function
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YThreshold.THRESHOLDSTATE_INVALID</c>.
         * </para>
         */
        public int get_thresholdState()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Threshold connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_thresholdState()+1;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Current state of the threshold function.</value>
        public int ThresholdState
        {
            get
            {
                if (_func == null) {
                    return _ThresholdState_INVALID;
                }
                if (_online) {
                    return _thresholdState;
                }
                return _ThresholdState_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "SAFE") {
                _thresholdState = 1;
            }
            if (value == "ALERT") {
                _thresholdState = 2;
            }
        }

        /**
         * <summary>
         *   Returns the name of the sensor monitored by the threshold function.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the name of the sensor monitored by the threshold function
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YThreshold.TARGETSENSOR_INVALID</c>.
         * </para>
         */
        public string get_targetSensor()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Threshold connected");
            }
            return _func.get_targetSensor();
        }

        /**
         * <summary>
         *   Changes the sensor alert level triggering the threshold function.
         * <para>
         *   Remember to call the matching module <c>saveToFlash()</c>
         *   method if you want to preserve the setting after reboot.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the sensor alert level triggering the threshold function
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
        public int set_alertLevel(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Threshold connected");
            }
            if (newval == _AlertLevel_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_alertLevel(newval);
        }

        /**
         * <summary>
         *   Returns the sensor alert level, triggering the threshold function.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the sensor alert level, triggering the threshold function
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YThreshold.ALERTLEVEL_INVALID</c>.
         * </para>
         */
        public double get_alertLevel()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Threshold connected");
            }
            res = _func.get_alertLevel();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>Sensor alert level, triggering the threshold function.</value>
        public double AlertLevel
        {
            get
            {
                if (_func == null) {
                    return _AlertLevel_INVALID;
                }
                if (_online) {
                    return _alertLevel;
                }
                return _AlertLevel_INVALID;
            }
            set
            {
                setprop_alertLevel(value);
            }
        }

        // private helper for magic property
        private void setprop_alertLevel(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _AlertLevel_INVALID) {
                return;
            }
            if (newval == _alertLevel) {
                return;
            }
            _func.set_alertLevel(newval);
            _alertLevel = newval;
        }

        /**
         * <summary>
         *   Changes the sensor acceptable level for disabling the threshold function.
         * <para>
         *   Remember to call the matching module <c>saveToFlash()</c>
         *   method if you want to preserve the setting after reboot.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the sensor acceptable level for disabling the threshold function
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
        public int set_safeLevel(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Threshold connected");
            }
            if (newval == _SafeLevel_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_safeLevel(newval);
        }

        /**
         * <summary>
         *   Returns the sensor acceptable level for disabling the threshold function.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the sensor acceptable level for disabling the threshold function
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YThreshold.SAFELEVEL_INVALID</c>.
         * </para>
         */
        public double get_safeLevel()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Threshold connected");
            }
            res = _func.get_safeLevel();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>Sensor acceptable level for disabling the threshold function.</value>
        public double SafeLevel
        {
            get
            {
                if (_func == null) {
                    return _SafeLevel_INVALID;
                }
                if (_online) {
                    return _safeLevel;
                }
                return _SafeLevel_INVALID;
            }
            set
            {
                setprop_safeLevel(value);
            }
        }

        // private helper for magic property
        private void setprop_safeLevel(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _SafeLevel_INVALID) {
                return;
            }
            if (newval == _safeLevel) {
                return;
            }
            _func.set_safeLevel(newval);
            _safeLevel = newval;
        }
    }
    //--- (end of YThreshold implementation)
}

