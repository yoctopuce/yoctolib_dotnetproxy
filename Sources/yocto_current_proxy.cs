/*********************************************************************
 *
 *  $Id: yocto_current_proxy.cs 40656 2020-05-25 14:13:34Z mvuilleu $
 *
 *  Implements YCurrentProxy, the Proxy API for Current
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
    //--- (YCurrent class start)
    static public partial class YoctoProxyManager
    {
        public static YCurrentProxy FindCurrent(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YCurrent func = null;
            YCurrentProxy res = (YCurrentProxy)YFunctionProxy.FindSimilarUnknownFunction("YCurrentProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YCurrentProxy)YFunctionProxy.FindSimilarKnownFunction("YCurrentProxy");
                if (res != null) return res;
                func = YCurrent.FirstCurrent();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YCurrentProxy)func.get_userData();
                    }
                }
            } else {
                func = YCurrent.FindCurrent(name);
                if (func.get_userData() != null) {
                    return (YCurrentProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YCurrentProxy(func, name);
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
 *   The <c>YCurrent</c> class allows you to read and configure Yoctopuce current sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YCurrentProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a current sensor for a given identifier.
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
         *   This function does not require that the current sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YCurrent.isOnline()</c> to test if the current sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a current sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the current sensor, for instance
         *   <c>YAMPMK01.current1</c>.
         * </param>
         * <returns>
         *   a <c>YCurrent</c> object allowing you to drive the current sensor.
         * </returns>
         */
        public static YCurrentProxy FindCurrent(string func)
        {
            return YoctoProxyManager.FindCurrent(func);
        }
        //--- (end of YCurrent class start)
        //--- (YCurrent definitions)
        public const int _Enabled_INVALID = 0;
        public const int _Enabled_FALSE = 1;
        public const int _Enabled_TRUE = 2;

        // reference to real YoctoAPI object
        protected new YCurrent _func;
        // property cache
        protected int _enabled = _Enabled_INVALID;
        //--- (end of YCurrent definitions)

        //--- (YCurrent implementation)
        internal YCurrentProxy(YCurrent hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Current " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YCurrent) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YCurrent hwd = YCurrent.FindCurrent(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YCurrent hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Current callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Current available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YCurrent.FindCurrent</c> to obtain an object that can control the
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
            YCurrent it = YCurrent.FirstCurrent();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextCurrent();
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
            _enabled = _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Returns the activation state of this input.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>current._Enabled_FALSE</c> or <c>current._Enabled_TRUE</c>, according to the activation
         *   state of this input
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>current._Enabled_INVALID</c>.
         * </para>
         */
        public int get_enabled()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Current connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Changes the activation state of this voltage input.
         * <para>
         *   When AC measurements are disabled,
         *   the device will always assume a DC signal, and vice-versa. When both AC and DC measurements
         *   are active, the device switches between AC and DC mode based on the relative amplitude
         *   of variations compared to the average value.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>current._Enabled_FALSE</c> or <c>current._Enabled_TRUE</c>, according to the activation
         *   state of this voltage input
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
        public int set_enabled(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Current connected");
            }
            if (newval == _Enabled_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enabled(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Activation state of this input.</value>
        public int Enabled
        {
            get
            {
                if (_func == null) {
                    return _Enabled_INVALID;
                }
                if (_online) {
                    return _enabled;
                }
                return _Enabled_INVALID;
            }
            set
            {
                setprop_enabled(value);
            }
        }

        // private helper for magic property
        private void setprop_enabled(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Enabled_INVALID) {
                return;
            }
            if (newval == _enabled) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_enabled(newval-1);
            _enabled = newval;
        }
    }
    //--- (end of YCurrent implementation)
}

