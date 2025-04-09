/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YVoltageProxy, the Proxy API for Voltage
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
    //--- (YVoltage class start)
    static public partial class YoctoProxyManager
    {
        public static YVoltageProxy FindVoltage(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YVoltage func = null;
            YVoltageProxy res = (YVoltageProxy)YFunctionProxy.FindSimilarUnknownFunction("YVoltageProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YVoltageProxy)YFunctionProxy.FindSimilarKnownFunction("YVoltageProxy");
                if (res != null) return res;
                func = YVoltage.FirstVoltage();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YVoltageProxy)func.get_userData();
                    }
                }
            } else {
                func = YVoltage.FindVoltage(name);
                if (func.get_userData() != null) {
                    return (YVoltageProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YVoltageProxy(func, name);
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
 *   The <c>YVoltage</c> class allows you to read and configure Yoctopuce voltage sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YVoltageProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a voltage sensor for a given identifier.
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
         *   This function does not require that the voltage sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YVoltage.isOnline()</c> to test if the voltage sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a voltage sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the voltage sensor, for instance
         *   <c>MOTORCTL.voltage</c>.
         * </param>
         * <returns>
         *   a <c>YVoltage</c> object allowing you to drive the voltage sensor.
         * </returns>
         */
        public static YVoltageProxy FindVoltage(string func)
        {
            return YoctoProxyManager.FindVoltage(func);
        }
        //--- (end of YVoltage class start)
        //--- (YVoltage definitions)
        public const int _Enabled_INVALID = 0;
        public const int _Enabled_FALSE = 1;
        public const int _Enabled_TRUE = 2;

        // reference to real YoctoAPI object
        protected new YVoltage _func;
        // property cache
        protected int _enabled = _Enabled_INVALID;
        //--- (end of YVoltage definitions)

        //--- (YVoltage implementation)
        internal YVoltageProxy(YVoltage hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Voltage " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YVoltage) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YVoltage hwd = YVoltage.FindVoltage(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YVoltage hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Voltage callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Voltage available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YVoltage.FindVoltage</c> to obtain an object that can control the
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
            YVoltage it = YVoltage.FirstVoltage();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextVoltage();
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
         *   either <c>YVoltage.ENABLED_FALSE</c> or <c>YVoltage.ENABLED_TRUE</c>, according to the activation
         *   state of this input
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YVoltage.ENABLED_INVALID</c>.
         * </para>
         */
        public int get_enabled()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Voltage connected");
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
         *   either <c>YVoltage.ENABLED_FALSE</c> or <c>YVoltage.ENABLED_TRUE</c>, according to the activation
         *   state of this voltage input
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
        public int set_enabled(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Voltage connected");
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
    //--- (end of YVoltage implementation)
}

