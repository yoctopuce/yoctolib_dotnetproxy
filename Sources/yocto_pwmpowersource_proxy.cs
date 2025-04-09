/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YPwmPowerSourceProxy, the Proxy API for PwmPowerSource
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
    //--- (YPwmPowerSource class start)
    static public partial class YoctoProxyManager
    {
        public static YPwmPowerSourceProxy FindPwmPowerSource(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YPwmPowerSource func = null;
            YPwmPowerSourceProxy res = (YPwmPowerSourceProxy)YFunctionProxy.FindSimilarUnknownFunction("YPwmPowerSourceProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YPwmPowerSourceProxy)YFunctionProxy.FindSimilarKnownFunction("YPwmPowerSourceProxy");
                if (res != null) return res;
                func = YPwmPowerSource.FirstPwmPowerSource();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YPwmPowerSourceProxy)func.get_userData();
                    }
                }
            } else {
                func = YPwmPowerSource.FindPwmPowerSource(name);
                if (func.get_userData() != null) {
                    return (YPwmPowerSourceProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YPwmPowerSourceProxy(func, name);
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
 *   The <c>YPwmPowerSource</c> class allows you to configure
 *   the voltage source used by all PWM outputs on the same device.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YPwmPowerSourceProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a PWM generator power source for a given identifier.
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
         *   This function does not require that the PWM generator power source is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YPwmPowerSource.isOnline()</c> to test if the PWM generator power source is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a PWM generator power source by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the PWM generator power source, for instance
         *   <c>YPWMTX01.pwmPowerSource</c>.
         * </param>
         * <returns>
         *   a <c>YPwmPowerSource</c> object allowing you to drive the PWM generator power source.
         * </returns>
         */
        public static YPwmPowerSourceProxy FindPwmPowerSource(string func)
        {
            return YoctoProxyManager.FindPwmPowerSource(func);
        }
        //--- (end of YPwmPowerSource class start)
        //--- (YPwmPowerSource definitions)
        public const int _PowerMode_INVALID = 0;
        public const int _PowerMode_USB_5V = 1;
        public const int _PowerMode_USB_3V = 2;
        public const int _PowerMode_EXT_V = 3;
        public const int _PowerMode_OPNDRN = 4;

        // reference to real YoctoAPI object
        protected new YPwmPowerSource _func;
        // property cache
        protected int _powerMode = _PowerMode_INVALID;
        //--- (end of YPwmPowerSource definitions)

        //--- (YPwmPowerSource implementation)
        internal YPwmPowerSourceProxy(YPwmPowerSource hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("PwmPowerSource " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YPwmPowerSource) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YPwmPowerSource hwd = YPwmPowerSource.FindPwmPowerSource(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YPwmPowerSource hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering PwmPowerSource callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type PwmPowerSource available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YPwmPowerSource.FindPwmPowerSource</c> to obtain an object that can control the
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
            YPwmPowerSource it = YPwmPowerSource.FirstPwmPowerSource();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextPwmPowerSource();
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
            _powerMode = _func.get_powerMode()+1;
        }

        /**
         * <summary>
         *   Returns the selected power source for the PWM on the same device.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YPwmPowerSource.POWERMODE_USB_5V</c>, <c>YPwmPowerSource.POWERMODE_USB_3V</c>,
         *   <c>YPwmPowerSource.POWERMODE_EXT_V</c> and <c>YPwmPowerSource.POWERMODE_OPNDRN</c> corresponding to
         *   the selected power source for the PWM on the same device
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPwmPowerSource.POWERMODE_INVALID</c>.
         * </para>
         */
        public int get_powerMode()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmPowerSource connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_powerMode()+1;
        }

        /**
         * <summary>
         *   Changes  the PWM power source.
         * <para>
         *   PWM can use isolated 5V from USB, isolated 3V from USB or
         *   voltage from an external power source. The PWM can also work in open drain  mode. In that
         *   mode, the PWM actively pulls the line down.
         *   Warning: this setting is common to all PWM on the same device. If you change that parameter,
         *   all PWM located on the same device are  affected.
         *   If you want the change to be kept after a device reboot, make sure  to call the matching
         *   module <c>saveToFlash()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YPwmPowerSource.POWERMODE_USB_5V</c>, <c>YPwmPowerSource.POWERMODE_USB_3V</c>,
         *   <c>YPwmPowerSource.POWERMODE_EXT_V</c> and <c>YPwmPowerSource.POWERMODE_OPNDRN</c> corresponding to
         *    the PWM power source
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
        public int set_powerMode(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No PwmPowerSource connected");
            }
            if (newval == _PowerMode_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_powerMode(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Selected power source for the PWM on the same device.</value>
        public int PowerMode
        {
            get
            {
                if (_func == null) {
                    return _PowerMode_INVALID;
                }
                if (_online) {
                    return _powerMode;
                }
                return _PowerMode_INVALID;
            }
            set
            {
                setprop_powerMode(value);
            }
        }

        // private helper for magic property
        private void setprop_powerMode(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _PowerMode_INVALID) {
                return;
            }
            if (newval == _powerMode) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_powerMode(newval-1);
            _powerMode = newval;
        }
    }
    //--- (end of YPwmPowerSource implementation)
}

