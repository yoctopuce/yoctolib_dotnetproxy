/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YCompassProxy, the Proxy API for Compass
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
    //--- (YCompass class start)
    static public partial class YoctoProxyManager
    {
        public static YCompassProxy FindCompass(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YCompass func = null;
            YCompassProxy res = (YCompassProxy)YFunctionProxy.FindSimilarUnknownFunction("YCompassProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YCompassProxy)YFunctionProxy.FindSimilarKnownFunction("YCompassProxy");
                if (res != null) return res;
                func = YCompass.FirstCompass();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YCompassProxy)func.get_userData();
                    }
                }
            } else {
                func = YCompass.FindCompass(name);
                if (func.get_userData() != null) {
                    return (YCompassProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YCompassProxy(func, name);
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
 *   The <c>YCompass</c> class allows you to read and configure Yoctopuce compass functions.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YCompassProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a compass function for a given identifier.
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
         *   This function does not require that the compass function is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YCompass.isOnline()</c> to test if the compass function is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a compass function by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the compass function, for instance
         *   <c>Y3DMK002.compass</c>.
         * </param>
         * <returns>
         *   a <c>YCompass</c> object allowing you to drive the compass function.
         * </returns>
         */
        public static YCompassProxy FindCompass(string func)
        {
            return YoctoProxyManager.FindCompass(func);
        }
        //--- (end of YCompass class start)
        //--- (YCompass definitions)
        public const int _Bandwidth_INVALID = -1;
        public const int _Axis_INVALID = 0;
        public const int _Axis_X = 1;
        public const int _Axis_Y = 2;
        public const int _Axis_Z = 3;
        public const double _MagneticHeading_INVALID = Double.NaN;

        // reference to real YoctoAPI object
        protected new YCompass _func;
        // property cache
        protected int _bandwidth = _Bandwidth_INVALID;
        //--- (end of YCompass definitions)

        //--- (YCompass implementation)
        internal YCompassProxy(YCompass hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Compass " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YCompass) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YCompass hwd = YCompass.FindCompass(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YCompass hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Compass callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Compass available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YCompass.FindCompass</c> to obtain an object that can control the
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
            YCompass it = YCompass.FirstCompass();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextCompass();
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
            _bandwidth = _func.get_bandwidth();
        }

        /**
         * <summary>
         *   Returns the measure update frequency, measured in Hz.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the measure update frequency, measured in Hz
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCompass.BANDWIDTH_INVALID</c>.
         * </para>
         */
        public int get_bandwidth()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Compass connected");
            }
            res = _func.get_bandwidth();
            if (res == YAPI.INVALID_INT) {
                res = _Bandwidth_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the measure update frequency, measured in Hz.
         * <para>
         *   When the
         *   frequency is lower, the device performs averaging.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the measure update frequency, measured in Hz
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
        public int set_bandwidth(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Compass connected");
            }
            if (newval == _Bandwidth_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_bandwidth(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Measure update frequency, measured in Hz.</value>
        public int Bandwidth
        {
            get
            {
                if (_func == null) {
                    return _Bandwidth_INVALID;
                }
                if (_online) {
                    return _bandwidth;
                }
                return _Bandwidth_INVALID;
            }
            set
            {
                setprop_bandwidth(value);
            }
        }

        // private helper for magic property
        private void setprop_bandwidth(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Bandwidth_INVALID) {
                return;
            }
            if (newval == _bandwidth) {
                return;
            }
            _func.set_bandwidth(newval);
            _bandwidth = newval;
        }

        /**
         * <summary>
         *   Returns the magnetic heading, regardless of the configured bearing.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the magnetic heading, regardless of the configured bearing
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCompass.MAGNETICHEADING_INVALID</c>.
         * </para>
         */
        public double get_magneticHeading()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Compass connected");
            }
            res = _func.get_magneticHeading();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }
    }
    //--- (end of YCompass implementation)
}

