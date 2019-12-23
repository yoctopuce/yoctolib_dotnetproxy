/*********************************************************************
 *
 *  $Id: yocto_tilt_proxy.cs 38913 2019-12-20 18:59:49Z mvuilleu $
 *
 *  Implements YTiltProxy, the Proxy API for Tilt
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
    //--- (YTilt class start)
    static public partial class YoctoProxyManager
    {
        public static YTiltProxy FindTilt(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YTilt func = null;
            YTiltProxy res = (YTiltProxy)YFunctionProxy.FindSimilarUnknownFunction("YTiltProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YTiltProxy)YFunctionProxy.FindSimilarKnownFunction("YTiltProxy");
                if (res != null) return res;
                func = YTilt.FirstTilt();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YTiltProxy)func.get_userData();
                    }
                }
            } else {
                func = YTilt.FindTilt(name);
                if (func.get_userData() != null) {
                    return (YTiltProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YTiltProxy(func, name);
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
 *   The <c>YSensor</c> class is the parent class for all Yoctopuce sensor types.
 * <para>
 *   It can be
 *   used to read the current value and unit of any sensor, read the min/max
 *   value, configure autonomous recording frequency and access recorded data.
 *   It also provide a function to register a callback invoked each time the
 *   observed value changes, or at a predefined interval. Using this class rather
 *   than a specific subclass makes it possible to create generic applications
 *   that work with any Yoctopuce sensor, even those that do not yet exist.
 *   Note: The <c>YAnButton</c> class is the only analog input which does not inherit
 *   from <c>YSensor</c>.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YTiltProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a tilt sensor for a given identifier.
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
         *   This function does not require that the tilt sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YTilt.isOnline()</c> to test if the tilt sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a tilt sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the tilt sensor, for instance
         *   <c>Y3DMK002.tilt1</c>.
         * </param>
         * <returns>
         *   a <c>YTilt</c> object allowing you to drive the tilt sensor.
         * </returns>
         */
        public static YTiltProxy FindTilt(string func)
        {
            return YoctoProxyManager.FindTilt(func);
        }
        //--- (end of YTilt class start)
        //--- (YTilt definitions)
        public const int _Bandwidth_INVALID = -1;
        public const int _Axis_INVALID = 0;
        public const int _Axis_X = 1;
        public const int _Axis_Y = 2;
        public const int _Axis_Z = 3;

        // reference to real YoctoAPI object
        protected new YTilt _func;
        // property cache
        protected int _bandwidth = _Bandwidth_INVALID;
        //--- (end of YTilt definitions)

        //--- (YTilt implementation)
        internal YTiltProxy(YTilt hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Tilt " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YTilt) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YTilt hwd = YTilt.FindTilt(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YTilt hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Tilt callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Tilt available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YTilt.FindTilt</c> to obtain an object that can control the
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
            YTilt it = YTilt.FirstTilt();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextTilt();
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
         *   Returns the measure update frequency, measured in Hz (Yocto-3D-V2 only).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the measure update frequency, measured in Hz (Yocto-3D-V2 only)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>tilt._Bandwidth_INVALID</c>.
         * </para>
         */
        public int get_bandwidth()
        {
            if (_func == null)
            {
                string msg = "No Tilt connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_bandwidth();
            if (res == YAPI.INVALID_INT) res = _Bandwidth_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the measure update frequency, measured in Hz (Yocto-3D-V2 only).
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
         *   an integer corresponding to the measure update frequency, measured in Hz (Yocto-3D-V2 only)
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_bandwidth(int newval)
        {
            if (_func == null)
            {
                string msg = "No Tilt connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Bandwidth_INVALID) return YAPI.SUCCESS;
            return _func.set_bandwidth(newval);
        }


        // property with cached value for instant access (configuration)
        public int Bandwidth
        {
            get
            {
                if (_func == null) return _Bandwidth_INVALID;
                return (_online ? _bandwidth : _Bandwidth_INVALID);
            }
            set
            {
                setprop_bandwidth(value);
            }
        }

        // private helper for magic property
        private void setprop_bandwidth(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Bandwidth_INVALID) return;
            if (newval == _bandwidth) return;
            _func.set_bandwidth(newval);
            _bandwidth = newval;
        }
    }
    //--- (end of YTilt implementation)
}

