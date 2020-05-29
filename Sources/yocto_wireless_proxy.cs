/*********************************************************************
 *
 *  $Id: yocto_wireless_proxy.cs 40656 2020-05-25 14:13:34Z mvuilleu $
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
using System.IO;
using System.Threading;
using System.Timers;
using System.Globalization;
using System.Text.RegularExpressions;
using YoctoLib;

namespace YoctoProxyAPI
{
    //--- (YWireless class start)
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
 *   and status for devices that are wireless-enabled.
 * <para>
 *   Note that TCP/IP parameters are configured separately, using class <c>YNetwork</c>.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YWirelessProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a wireless LAN interface for a given identifier.
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
         *   This function does not require that the wireless LAN interface is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YWireless.isOnline()</c> to test if the wireless LAN interface is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a wireless LAN interface by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the wireless LAN interface, for instance
         *   <c>YHUBWLN1.wireless</c>.
         * </param>
         * <returns>
         *   a <c>YWireless</c> object allowing you to drive the wireless LAN interface.
         * </returns>
         */
        public static YWirelessProxy FindWireless(string func)
        {
            return YoctoProxyManager.FindWireless(func);
        }
        //--- (end of YWireless class start)
        //--- (YWireless definitions)
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
        //--- (end of YWireless definitions)

        //--- (YWireless implementation)
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

        /**
         * <summary>
         *   Enumerates all functions of type Wireless available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YWireless.FindWireless</c> to obtain an object that can control the
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
         *   On failure, throws an exception or returns <c>wireless._Linkquality_INVALID</c>.
         * </para>
         */
        public int get_linkQuality()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
            }
            res = _func.get_linkQuality();
            if (res == YAPI.INVALID_INT) {
                res = _LinkQuality_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Link quality, expressed in percent.</value>
        public int LinkQuality
        {
            get
            {
                if (_func == null) {
                    return _LinkQuality_INVALID;
                }
                if (_online) {
                    return _linkQuality;
                }
                return _LinkQuality_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            value = (YAPI._atoi(value)).ToString();
            _linkQuality = YAPI._atoi(value);
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>Wireless network name (SSID).</value>
        public string Ssid
        {
            get
            {
                if (_func == null) {
                    return _Ssid_INVALID;
                }
                if (_online) {
                    return _ssid;
                }
                return _Ssid_INVALID;
            }
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
         *   On failure, throws an exception or returns <c>wireless._Ssid_INVALID</c>.
         * </para>
         */
        public string get_ssid()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
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
         *   On failure, throws an exception or returns <c>wireless._Channel_INVALID</c>.
         * </para>
         */
        public int get_channel()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
            }
            res = _func.get_channel();
            if (res == YAPI.INVALID_INT) {
                res = _Channel_INVALID;
            }
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
         *   a value among <c>wireless._Security_UNKNOWN</c>, <c>wireless._Security_OPEN</c>,
         *   <c>wireless._Security_WEP</c>, <c>wireless._Security_WPA</c> and <c>wireless._Security_WPA2</c>
         *   corresponding to the security algorithm used by the selected wireless network
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>wireless._Security_INVALID</c>.
         * </para>
         */
        public int get_security()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
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
         *   On failure, throws an exception or returns <c>wireless._Message_INVALID</c>.
         * </para>
         */
        public string get_message()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
            }
            return _func.get_message();
        }

        /**
         * <summary>
         *   Returns the current state of the wireless interface.
         * <para>
         *   The state <c>wireless._Wlanstate_DOWN</c> means that the network interface is
         *   not connected to a network. The state <c>wireless._Wlanstate_SCANNING</c> means that the network
         *   interface is scanning available
         *   frequencies. During this stage, the device is not reachable, and the network settings are not yet
         *   applied. The state
         *   <c>wireless._Wlanstate_CONNECTED</c> means that the network settings have been successfully applied
         *   ant that the device is reachable
         *   from the wireless network. If the device is configured to use ad-hoc or Soft AP mode, it means that
         *   the wireless network
         *   is up and that other devices can join the network. The state <c>wireless._Wlanstate_REJECTED</c>
         *   means that the network interface has
         *   not been able to join the requested network. The description of the error can be obtain with the
         *   <c>get_message()</c> method.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>wireless._Wlanstate_DOWN</c>, <c>wireless._Wlanstate_SCANNING</c>,
         *   <c>wireless._Wlanstate_CONNECTED</c> and <c>wireless._Wlanstate_REJECTED</c> corresponding to the
         *   current state of the wireless interface
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>wireless._Wlanstate_INVALID</c>.
         * </para>
         */
        public int get_wlanState()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
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
         *   switches to <c>wireless._Wlanstate_DOWN</c>, then to <c>wireless._Wlanstate_SCANNING</c>. When the
         *   scan is completed,
         *   <c>get_wlanState()</c> returns either <c>wireless._Wlanstate_DOWN</c> or
         *   <c>wireless._Wlanstate_SCANNING</c>. At this
         *   point, the list of detected network can be retrieved with the <c>get_detectedWlans()</c> method.
         * </para>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         * </summary>
         */
        public virtual int startWlanScan()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
            }
            return _func.softAPNetwork(ssid, securityKey);
        }

        /**
         * <summary>
         *   Returns a list of <c>YWlanRecord</c> objects that describe detected Wireless networks.
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
            if (_func == null) {
                throw new YoctoApiProxyException("No Wireless connected");
            }
            int i;
            int arrlen;
            YWlanRecord[] std_res;
            YWlanRecordProxy[] proxy_res;
            std_res = _func.get_detectedWlans().ToArray();
            arrlen = std_res.Length;
            proxy_res = new YWlanRecordProxy[arrlen];
            i = 0;
            while (i < arrlen) {
                proxy_res[i] = new YWlanRecordProxy(std_res[i]);
                i = i + 1;
            }
            return proxy_res;
        }
    }
    //--- (end of YWireless implementation)
}

