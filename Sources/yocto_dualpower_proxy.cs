/*********************************************************************
 *
 *  $Id: yocto_dualpower_proxy.cs 40656 2020-05-25 14:13:34Z mvuilleu $
 *
 *  Implements YDualPowerProxy, the Proxy API for DualPower
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
    //--- (YDualPower class start)
    static public partial class YoctoProxyManager
    {
        public static YDualPowerProxy FindDualPower(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YDualPower func = null;
            YDualPowerProxy res = (YDualPowerProxy)YFunctionProxy.FindSimilarUnknownFunction("YDualPowerProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YDualPowerProxy)YFunctionProxy.FindSimilarKnownFunction("YDualPowerProxy");
                if (res != null) return res;
                func = YDualPower.FirstDualPower();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YDualPowerProxy)func.get_userData();
                    }
                }
            } else {
                func = YDualPower.FindDualPower(name);
                if (func.get_userData() != null) {
                    return (YDualPowerProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YDualPowerProxy(func, name);
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
 *   The <c>YDualPower</c> class allows you to control
 *   the power source to use for module functions that require high current.
 * <para>
 *   The module can also automatically disconnect the external power
 *   when a voltage drop is observed on the external power source
 *   (external battery running out of power).
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YDualPowerProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a dual power switch for a given identifier.
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
         *   This function does not require that the dual power switch is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YDualPower.isOnline()</c> to test if the dual power switch is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a dual power switch by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the dual power switch, for instance
         *   <c>SERVORC1.dualPower</c>.
         * </param>
         * <returns>
         *   a <c>YDualPower</c> object allowing you to drive the dual power switch.
         * </returns>
         */
        public static YDualPowerProxy FindDualPower(string func)
        {
            return YoctoProxyManager.FindDualPower(func);
        }
        //--- (end of YDualPower class start)
        //--- (YDualPower definitions)
        public const int _PowerState_INVALID = 0;
        public const int _PowerState_OFF = 1;
        public const int _PowerState_FROM_USB = 2;
        public const int _PowerState_FROM_EXT = 3;
        public const int _PowerControl_INVALID = 0;
        public const int _PowerControl_AUTO = 1;
        public const int _PowerControl_FROM_USB = 2;
        public const int _PowerControl_FROM_EXT = 3;
        public const int _PowerControl_OFF = 4;
        public const int _ExtVoltage_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YDualPower _func;
        // property cache
        protected int _powerState = _PowerState_INVALID;
        protected int _powerControl = _PowerControl_INVALID;
        //--- (end of YDualPower definitions)

        //--- (YDualPower implementation)
        internal YDualPowerProxy(YDualPower hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("DualPower " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YDualPower) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YDualPower hwd = YDualPower.FindDualPower(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YDualPower hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering DualPower callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type DualPower available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YDualPower.FindDualPower</c> to obtain an object that can control the
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
            YDualPower it = YDualPower.FirstDualPower();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextDualPower();
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
            _powerControl = _func.get_powerControl()+1;
        }

        /**
         * <summary>
         *   Returns the current power source for module functions that require lots of current.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>dualpower._Powerstate_OFF</c>, <c>dualpower._Powerstate_FROM_USB</c> and
         *   <c>dualpower._Powerstate_FROM_EXT</c> corresponding to the current power source for module
         *   functions that require lots of current
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>dualpower._Powerstate_INVALID</c>.
         * </para>
         */
        public int get_powerState()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DualPower connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_powerState()+1;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Current power source for module functions that require lots of current.</value>
        public int PowerState
        {
            get
            {
                if (_func == null) {
                    return _PowerState_INVALID;
                }
                if (_online) {
                    return _powerState;
                }
                return _PowerState_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "OFF") {
                _powerState = 1;
            }
            if (value == "FROM_USB") {
                _powerState = 2;
            }
            if (value == "FROM_EXT") {
                _powerState = 3;
            }
            if (value == "USB") {
                _powerState = _PowerState_FROM_USB;
            }
            if (value == "EXT") {
                _powerState = _PowerState_FROM_EXT;
            }
        }

        /**
         * <summary>
         *   Returns the selected power source for module functions that require lots of current.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>dualpower._Powercontrol_AUTO</c>, <c>dualpower._Powercontrol_FROM_USB</c>,
         *   <c>dualpower._Powercontrol_FROM_EXT</c> and <c>dualpower._Powercontrol_OFF</c> corresponding to the
         *   selected power source for module functions that require lots of current
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>dualpower._Powercontrol_INVALID</c>.
         * </para>
         */
        public int get_powerControl()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DualPower connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_powerControl()+1;
        }

        /**
         * <summary>
         *   Changes the selected power source for module functions that require lots of current.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>dualpower._Powercontrol_AUTO</c>, <c>dualpower._Powercontrol_FROM_USB</c>,
         *   <c>dualpower._Powercontrol_FROM_EXT</c> and <c>dualpower._Powercontrol_OFF</c> corresponding to the
         *   selected power source for module functions that require lots of current
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
        public int set_powerControl(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No DualPower connected");
            }
            if (newval == _PowerControl_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_powerControl(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Selected power source for module functions that require lots of current.</value>
        public int PowerControl
        {
            get
            {
                if (_func == null) {
                    return _PowerControl_INVALID;
                }
                if (_online) {
                    return _powerControl;
                }
                return _PowerControl_INVALID;
            }
            set
            {
                setprop_powerControl(value);
            }
        }

        // private helper for magic property
        private void setprop_powerControl(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _PowerControl_INVALID) {
                return;
            }
            if (newval == _powerControl) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_powerControl(newval-1);
            _powerControl = newval;
        }

        /**
         * <summary>
         *   Returns the measured voltage on the external power source, in millivolts.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the measured voltage on the external power source, in millivolts
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>dualpower._Extvoltage_INVALID</c>.
         * </para>
         */
        public int get_extVoltage()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No DualPower connected");
            }
            res = _func.get_extVoltage();
            if (res == YAPI.INVALID_INT) {
                res = _ExtVoltage_INVALID;
            }
            return res;
        }
    }
    //--- (end of YDualPower implementation)
}

