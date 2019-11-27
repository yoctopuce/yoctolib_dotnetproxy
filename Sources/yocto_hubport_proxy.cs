/*********************************************************************
 *
 *  $Id: yocto_hubport_proxy.cs 38514 2019-11-26 16:54:39Z seb $
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
 *   The YHubPort class provides control over the power supply for every port
 *   on a YoctoHub, for instance using a YoctoHub-Ethernet, a YoctoHub-GSM-3G-NA, a YoctoHub-Shield or a YoctoHub-Wireless-g.
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

        public override string[] GetSimilarFunctions()
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
         *   Returns true if the Yocto-hub port is powered, false otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YHubPort.ENABLED_FALSE</c> or <c>YHubPort.ENABLED_TRUE</c>, according to true if the
         *   Yocto-hub port is powered, false otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YHubPort.ENABLED_INVALID</c>.
         * </para>
         */
        public int get_enabled()
        {
            if (_func == null)
            {
                string msg = "No HubPort connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Changes the activation of the Yocto-hub port.
         * <para>
         *   If the port is enabled, the
         *   connected module is powered. Otherwise, port power is shut down.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YHubPort.ENABLED_FALSE</c> or <c>YHubPort.ENABLED_TRUE</c>, according to the activation
         *   of the Yocto-hub port
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
            if (_func == null)
            {
                string msg = "No HubPort connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Enabled_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enabled(newval-1);
        }


        // private helper for magic property
        private void setprop_enabled(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Enabled_INVALID) return;
            if (newval == _enabled) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_enabled(newval-1);
            _enabled = newval;
        }

        // property with cached value for instant access (advertised value)
        public int PortState
        {
            get
            {
                if (_func == null) return _PortState_INVALID;
                return (_online ? _portState : _PortState_INVALID);
            }
        }

        // property with cached value for instant access (derived from advertised value)
        public int Enabled
        {
            get
            {
                if (_func == null) return _Enabled_INVALID;
                return (_online ? _enabled : _Enabled_INVALID);
            }
            set
            {
                setprop_enabled(value);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "OFF") _portState = 1;
            if (value == "OVRLD") _portState = 2;
            if (value == "ON") _portState = 3;
            if (value == "RUN") _portState = 4;
            if (value == "PROG") _portState = 5;
            if (value == "OFF" || value == "OVRLD") _enabled = 0;
            else if (value != "") _enabled = 1;
        }

        /**
         * <summary>
         *   Returns the current state of the Yocto-hub port.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YHubPort.PORTSTATE_OFF</c>, <c>YHubPort.PORTSTATE_OVRLD</c>,
         *   <c>YHubPort.PORTSTATE_ON</c>, <c>YHubPort.PORTSTATE_RUN</c> and <c>YHubPort.PORTSTATE_PROG</c>
         *   corresponding to the current state of the Yocto-hub port
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YHubPort.PORTSTATE_INVALID</c>.
         * </para>
         */
        public int get_portState()
        {
            if (_func == null)
            {
                string msg = "No HubPort connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_portState()+1;
        }

        /**
         * <summary>
         *   Returns the current baud rate used by this Yocto-hub port, in kbps.
         * <para>
         *   The default value is 1000 kbps, but a slower rate may be used if communication
         *   problems are encountered.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current baud rate used by this Yocto-hub port, in kbps
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YHubPort.BAUDRATE_INVALID</c>.
         * </para>
         */
        public int get_baudRate()
        {
            if (_func == null)
            {
                string msg = "No HubPort connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_baudRate();
            if (res == YAPI.INVALID_INT) res = _BaudRate_INVALID;
            return res;
        }
    }
    //--- (end of YHubPort implementation)
}

