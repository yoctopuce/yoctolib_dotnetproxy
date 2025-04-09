/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YHubPortProxy, the Proxy API for HubPort
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
    //--- (YHubPort class start)
    static public partial class YoctoProxyManager
    {
        public static YHubPortProxy FindHubPort(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YHubPort func = null;
            YHubPortProxy res = (YHubPortProxy)YFunctionProxy.FindSimilarUnknownFunction("YHubPortProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YHubPortProxy)YFunctionProxy.FindSimilarKnownFunction("YHubPortProxy");
                if (res != null) return res;
                func = YHubPort.FirstHubPort();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YHubPortProxy)func.get_userData();
                    }
                }
            } else {
                func = YHubPort.FindHubPort(name);
                if (func.get_userData() != null) {
                    return (YHubPortProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YHubPortProxy(func, name);
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
 *   The <c>YHubPort</c> class provides control over the power supply for slave ports
 *   on a YoctoHub.
 * <para>
 *   It provide information about the device connected to it.
 *   The logical name of a YHubPort is always automatically set to the
 *   unique serial number of the Yoctopuce device connected to it.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YHubPortProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a YoctoHub slave port for a given identifier.
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
         *   This function does not require that the YoctoHub slave port is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YHubPort.isOnline()</c> to test if the YoctoHub slave port is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a YoctoHub slave port by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the YoctoHub slave port, for instance
         *   <c>YHUBETH1.hubPort1</c>.
         * </param>
         * <returns>
         *   a <c>YHubPort</c> object allowing you to drive the YoctoHub slave port.
         * </returns>
         */
        public static YHubPortProxy FindHubPort(string func)
        {
            return YoctoProxyManager.FindHubPort(func);
        }
        //--- (end of YHubPort class start)
        //--- (YHubPort definitions)
        public const int _Enabled_INVALID = 0;
        public const int _Enabled_FALSE = 1;
        public const int _Enabled_TRUE = 2;
        public const int _PortState_INVALID = 0;
        public const int _PortState_OFF = 1;
        public const int _PortState_OVRLD = 2;
        public const int _PortState_ON = 3;
        public const int _PortState_RUN = 4;
        public const int _PortState_PROG = 5;
        public const int _BaudRate_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YHubPort _func;
        // property cache
        protected int _portState = _PortState_INVALID;
        protected int _enabled = _Enabled_INVALID;
        //--- (end of YHubPort definitions)

        //--- (YHubPort implementation)
        internal YHubPortProxy(YHubPort hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("HubPort " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YHubPort) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YHubPort hwd = YHubPort.FindHubPort(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YHubPort hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering HubPort callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type HubPort available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YHubPort.FindHubPort</c> to obtain an object that can control the
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
            YHubPort it = YHubPort.FirstHubPort();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextHubPort();
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
        }

        /**
         * <summary>
         *   Returns true if the YoctoHub port is powered, false otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YHubPort.ENABLED_FALSE</c> or <c>YHubPort.ENABLED_TRUE</c>, according to true if the
         *   YoctoHub port is powered, false otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YHubPort.ENABLED_INVALID</c>.
         * </para>
         */
        public int get_enabled()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No HubPort connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Changes the activation of the YoctoHub port.
         * <para>
         *   If the port is enabled, the
         *   connected module is powered. Otherwise, port power is shut down.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YHubPort.ENABLED_FALSE</c> or <c>YHubPort.ENABLED_TRUE</c>, according to the activation
         *   of the YoctoHub port
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
                throw new YoctoApiProxyException("No HubPort connected");
            }
            if (newval == _Enabled_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enabled(newval-1);
        }

        /**
         * <summary>
         *   Returns the current state of the YoctoHub port.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YHubPort.PORTSTATE_OFF</c>, <c>YHubPort.PORTSTATE_OVRLD</c>,
         *   <c>YHubPort.PORTSTATE_ON</c>, <c>YHubPort.PORTSTATE_RUN</c> and <c>YHubPort.PORTSTATE_PROG</c>
         *   corresponding to the current state of the YoctoHub port
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YHubPort.PORTSTATE_INVALID</c>.
         * </para>
         */
        public int get_portState()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No HubPort connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_portState()+1;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Current state of the YoctoHub port.</value>
        public int PortState
        {
            get
            {
                if (_func == null) {
                    return _PortState_INVALID;
                }
                if (_online) {
                    return _portState;
                }
                return _PortState_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "OFF") {
                _portState = 1;
            }
            if (value == "OVRLD") {
                _portState = 2;
            }
            if (value == "ON") {
                _portState = 3;
            }
            if (value == "RUN") {
                _portState = 4;
            }
            if (value == "PROG") {
                _portState = 5;
            }
            if (value == "OFF" || value == "OVRLD") {
                _enabled = _Enabled_FALSE;
            } else {
                if (!(value == "")) {
                    _enabled = _Enabled_TRUE;
                }
            }
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>True if the port output is enabled.</value>
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

        /**
         * <summary>
         *   Returns the current baud rate used by this YoctoHub port, in kbps.
         * <para>
         *   The default value is 1000 kbps, but a slower rate may be used if communication
         *   problems are encountered.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current baud rate used by this YoctoHub port, in kbps
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YHubPort.BAUDRATE_INVALID</c>.
         * </para>
         */
        public int get_baudRate()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No HubPort connected");
            }
            res = _func.get_baudRate();
            if (res == YAPI.INVALID_INT) {
                res = _BaudRate_INVALID;
            }
            return res;
        }
    }
    //--- (end of YHubPort implementation)
}

