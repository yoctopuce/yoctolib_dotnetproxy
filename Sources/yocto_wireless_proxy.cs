/*********************************************************************
 *
 *  $Id: yocto_wireless_proxy.cs 38478 2019-11-26 08:01:52Z seb $
 *
 *  Implements YWirelessProxy, the Proxy API for Wireless
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
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;

namespace YoctoProxyAPI
{
    //--- (generated code: YWireless class start)
    static public partial class YoctoProxyManager
    {
        public static YWirelessProxy FindWireless(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YWireless func = null;
            YWirelessProxy res = (YWirelessProxy)YFunctionProxy.FindSimilarUnknownFunction("YWirelessProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YWirelessProxy)YFunctionProxy.FindSimilarKnownFunction("YWirelessProxy");
                if (res != null) return res;
                func = YWireless.FirstWireless();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YWirelessProxy)func.get_userData();
                    }
                }
            } else {
                func = YWireless.FindWireless(name);
                if (func.get_userData() != null) {
                    return (YWirelessProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YWirelessProxy(func, name);
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
 *   The YWireless class provides control over wireless network parameters
 *   and status for devices that are wireless-enabled, for instance using a YoctoHub-Wireless-g, a YoctoHub-Wireless-SR or a YoctoHub-Wireless.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YWirelessProxy : YFunctionProxy
    {
        //--- (end of generated code: YWireless class start)
        //--- (generated code: YWireless definitions)
        public const int _LinkQuality_INVALID = -1;
        public const string _Ssid_INVALID = YAPI.INVALID_STRING;
        public const int _Channel_INVALID = -1;
        public const int _Security_INVALID = 0;
        public const int _Security_UNKNOWN = 1;
        public const int _Security_OPEN = 2;
        public const int _Security_WEP = 3;
        public const int _Security_WPA = 4;
        public const int _Security_WPA2 = 5;
        public const string _Message_INVALID = YAPI.INVALID_STRING;
        public const string _WlanConfig_INVALID = YAPI.INVALID_STRING;
        public const int _WlanState_INVALID = 0;
        public const int _WlanState_DOWN = 1;
        public const int _WlanState_SCANNING = 2;
        public const int _WlanState_CONNECTED = 3;
        public const int _WlanState_REJECTED = 4;

        // reference to real YoctoAPI object
        protected new YWireless _func;
        // property cache
        protected int _linkQuality = _LinkQuality_INVALID;
        protected string _ssid = _Ssid_INVALID;
        //--- (end of generated code: YWireless definitions)

        //--- (generated code: YWireless implementation)
        internal YWirelessProxy(YWireless hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Wireless " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YWireless) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YWireless hwd = YWireless.FindWireless(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YWireless hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Wireless callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YWireless it = YWireless.FirstWireless();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextWireless();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _ssid = _func.get_ssid();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _ssid = _func.get_ssid();
        }

        // property with cached value for instant access (advertised value)
        public int LinkQuality
        {
            get
            {
                if (_func == null) return _LinkQuality_INVALID;
                return (_online ? _linkQuality : _LinkQuality_INVALID);
            }
        }

        // property with cached value for instant access (derived from advertised value)
        public string Ssid
        {
            get
            {
                if (_func == null) return _Ssid_INVALID;
                return (_online ? _ssid : _Ssid_INVALID);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            value = Regex.Replace(value,"[^-0-9]","");
            Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture,out _linkQuality);
        }

        /**
         * <summary>
         *   Returns the link quality, expressed in percent.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the link quality, expressed in percent
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWireless.LINKQUALITY_INVALID</c>.
         * </para>
         */
        public int get_linkQuality()
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_linkQuality();
            if (res == YAPI.INVALID_INT) res = _LinkQuality_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the wireless network name (SSID).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the wireless network name (SSID)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWireless.SSID_INVALID</c>.
         * </para>
         */
        public string get_ssid()
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_ssid();
        }

        /**
         * <summary>
         *   Returns the 802.11 channel currently used, or 0 when the selected network has not been found.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the 802.11 channel currently used, or 0 when the selected network has not been found
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWireless.CHANNEL_INVALID</c>.
         * </para>
         */
        public int get_channel()
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_channel();
            if (res == YAPI.INVALID_INT) res = _Channel_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the security algorithm used by the selected wireless network.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YWireless.SECURITY_UNKNOWN</c>, <c>YWireless.SECURITY_OPEN</c>,
         *   <c>YWireless.SECURITY_WEP</c>, <c>YWireless.SECURITY_WPA</c> and <c>YWireless.SECURITY_WPA2</c>
         *   corresponding to the security algorithm used by the selected wireless network
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWireless.SECURITY_INVALID</c>.
         * </para>
         */
        public int get_security()
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_security()+1;
        }

        /**
         * <summary>
         *   Returns the latest status message from the wireless interface.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the latest status message from the wireless interface
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWireless.MESSAGE_INVALID</c>.
         * </para>
         */
        public string get_message()
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_message();
        }

        /**
         * <summary>
         *   Returns the current state of the wireless interface.
         * <para>
         *   The state <c>YWireless.WLANSTATE_DOWN</c> means that the network interface is
         *   not connected to a network. The state <c>YWireless.WLANSTATE_SCANNING</c> means that the network
         *   interface is scanning available
         *   frequencies. During this stage, the device is not reachable, and the network settings are not yet
         *   applied. The state
         *   <c>YWireless.WLANSTATE_CONNECTED</c> means that the network settings have been successfully applied
         *   ant that the device is reachable
         *   from the wireless network. If the device is configured to use ad-hoc or Soft AP mode, it means that
         *   the wireless network
         *   is up and that other devices can join the network. The state <c>YWireless.WLANSTATE_REJECTED</c>
         *   means that the network interface has
         *   not been able to join the requested network. The description of the error can be obtain with the
         *   <c>get_message()</c> method.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YWireless.WLANSTATE_DOWN</c>, <c>YWireless.WLANSTATE_SCANNING</c>,
         *   <c>YWireless.WLANSTATE_CONNECTED</c> and <c>YWireless.WLANSTATE_REJECTED</c> corresponding to the
         *   current state of the wireless interface
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YWireless.WLANSTATE_INVALID</c>.
         * </para>
         */
        public int get_wlanState()
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_wlanState()+1;
        }

        /**
         * <summary>
         *   Triggers a scan of the wireless frequency and builds the list of available networks.
         * <para>
         *   The scan forces a disconnection from the current network. At then end of the process, the
         *   the network interface attempts to reconnect to the previous network. During the scan, the <c>wlanState</c>
         *   switches to <c>YWireless.WLANSTATE_DOWN</c>, then to <c>YWireless.WLANSTATE_SCANNING</c>. When the
         *   scan is completed,
         *   <c>get_wlanState()</c> returns either <c>YWireless.WLANSTATE_DOWN</c> or
         *   <c>YWireless.WLANSTATE_SCANNING</c>. At this
         *   point, the list of detected network can be retrieved with the <c>get_detectedWlans()</c> method.
         * </para>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         * </summary>
         */
        public virtual int startWlanScan()
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.startWlanScan();
        }

        /**
         * <summary>
         *   Changes the configuration of the wireless lan interface to connect to an existing
         *   access point (infrastructure mode).
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
         * </para>
         * </summary>
         * <param name="ssid">
         *   the name of the network to connect to
         * </param>
         * <param name="securityKey">
         *   the network key, as a character string
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int joinNetwork(string ssid, string securityKey)
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.joinNetwork(ssid, securityKey);
        }

        /**
         * <summary>
         *   Changes the configuration of the wireless lan interface to create an ad-hoc
         *   wireless network, without using an access point.
         * <para>
         *   On the YoctoHub-Wireless-g
         *   and YoctoHub-Wireless-n,
         *   you should use <c>softAPNetwork()</c> instead, which emulates an access point
         *   (Soft AP) which is more efficient and more widely supported than ad-hoc networks.
         * </para>
         * <para>
         *   When a security key is specified for an ad-hoc network, the network is protected
         *   by a WEP40 key (5 characters or 10 hexadecimal digits) or WEP128 key (13 characters
         *   or 26 hexadecimal digits). It is recommended to use a well-randomized WEP128 key
         *   using 26 hexadecimal digits to maximize security.
         *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module
         *   to apply this setting.
         * </para>
         * </summary>
         * <param name="ssid">
         *   the name of the network to connect to
         * </param>
         * <param name="securityKey">
         *   the network key, as a character string
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int adhocNetwork(string ssid, string securityKey)
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.adhocNetwork(ssid, securityKey);
        }

        /**
         * <summary>
         *   Changes the configuration of the wireless lan interface to create a new wireless
         *   network by emulating a WiFi access point (Soft AP).
         * <para>
         *   This function can only be
         *   used with the YoctoHub-Wireless-g and the YoctoHub-Wireless-n.
         * </para>
         * <para>
         *   On the YoctoHub-Wireless-g, when a security key is specified for a SoftAP network,
         *   the network is protected by a WEP40 key (5 characters or 10 hexadecimal digits) or
         *   WEP128 key (13 characters or 26 hexadecimal digits). It is recommended to use a
         *   well-randomized WEP128 key using 26 hexadecimal digits to maximize security.
         * </para>
         * <para>
         *   On the YoctoHub-Wireless-n, when a security key is specified for a SoftAP network,
         *   the network will be protected by WPA2.
         * </para>
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
         * </para>
         * </summary>
         * <param name="ssid">
         *   the name of the network to connect to
         * </param>
         * <param name="securityKey">
         *   the network key, as a character string
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int softAPNetwork(string ssid, string securityKey)
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.softAPNetwork(ssid, securityKey);
        }

        /**
         * <summary>
         *   Returns a list of YWlanRecord objects that describe detected Wireless networks.
         * <para>
         *   This list is not updated when the module is already connected to an access point (infrastructure mode).
         *   To force an update of this list, <c>startWlanScan()</c> must be called.
         *   Note that an languages without garbage collections, the returned list must be freed by the caller.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a list of <c>YWlanRecord</c> objects, containing the SSID, channel,
         *   link quality and the type of security of the wireless network.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty list.
         * </para>
         */
        public virtual YWlanRecordProxy[] get_detectedWlans()
        {
            if (_func == null)
            {
                string msg = "No Wireless connected";
                throw new YoctoApiProxyException(msg);
            }
            int i = 0;
            var std_res = _func.get_detectedWlans();
            YWlanRecordProxy[] proxy_res = new YWlanRecordProxy[std_res.Count];
            foreach (var record in std_res) {
                proxy_res[i++] = new YWlanRecordProxy(record);
            }
            return proxy_res;
        }
    }
    //--- (end of generated code: YWireless implementation)
}

