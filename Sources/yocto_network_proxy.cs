/*********************************************************************
 *
 *  $Id: yocto_network_proxy.cs 53886 2023-04-05 08:06:39Z mvuilleu $
 *
 *  Implements YNetworkProxy, the Proxy API for Network
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
    //--- (YNetwork class start)
    static public partial class YoctoProxyManager
    {
        public static YNetworkProxy FindNetwork(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YNetwork func = null;
            YNetworkProxy res = (YNetworkProxy)YFunctionProxy.FindSimilarUnknownFunction("YNetworkProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YNetworkProxy)YFunctionProxy.FindSimilarKnownFunction("YNetworkProxy");
                if (res != null) return res;
                func = YNetwork.FirstNetwork();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YNetworkProxy)func.get_userData();
                    }
                }
            } else {
                func = YNetwork.FindNetwork(name);
                if (func.get_userData() != null) {
                    return (YNetworkProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YNetworkProxy(func, name);
            }
            if (func != null) {
                res.linkToHardware(name);
                if(func.isOnline()) res.arrival();
            }
            return res;
        }
    }

/**
 * <c>YNetwork</c> objects provide access to TCP/IP parameters of Yoctopuce
 * devices that include a built-in network interface.
 * <para>
 * </para>
 */
    public class YNetworkProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a network interface for a given identifier.
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
         *   This function does not require that the network interface is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YNetwork.isOnline()</c> to test if the network interface is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a network interface by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the network interface, for instance
         *   <c>YHUBETH1.network</c>.
         * </param>
         * <returns>
         *   a <c>YNetwork</c> object allowing you to drive the network interface.
         * </returns>
         */
        public static YNetworkProxy FindNetwork(string func)
        {
            return YoctoProxyManager.FindNetwork(func);
        }
        //--- (end of YNetwork class start)
        //--- (YNetwork definitions)
        public const int _Readiness_INVALID = 0;
        public const int _Readiness_DOWN = 1;
        public const int _Readiness_EXISTS = 2;
        public const int _Readiness_LINKED = 3;
        public const int _Readiness_LAN_OK = 4;
        public const int _Readiness_WWW_OK = 5;
        public const string _MacAddress_INVALID = YAPI.INVALID_STRING;
        public const string _IpAddress_INVALID = YAPI.INVALID_STRING;
        public const string _SubnetMask_INVALID = YAPI.INVALID_STRING;
        public const string _Router_INVALID = YAPI.INVALID_STRING;
        public const string _CurrentDNS_INVALID = YAPI.INVALID_STRING;
        public const string _IpConfig_INVALID = YAPI.INVALID_STRING;
        public const string _PrimaryDNS_INVALID = YAPI.INVALID_STRING;
        public const string _SecondaryDNS_INVALID = YAPI.INVALID_STRING;
        public const string _NtpServer_INVALID = YAPI.INVALID_STRING;
        public const string _UserPassword_INVALID = YAPI.INVALID_STRING;
        public const string _AdminPassword_INVALID = YAPI.INVALID_STRING;
        public const int _HttpPort_INVALID = -1;
        public const string _DefaultPage_INVALID = YAPI.INVALID_STRING;
        public const int _Discoverable_INVALID = 0;
        public const int _Discoverable_FALSE = 1;
        public const int _Discoverable_TRUE = 2;
        public const int _WwwWatchdogDelay_INVALID = -1;
        public const string _CallbackUrl_INVALID = YAPI.INVALID_STRING;
        public const int _CallbackMethod_INVALID = 0;
        public const int _CallbackMethod_POST = 1;
        public const int _CallbackMethod_GET = 2;
        public const int _CallbackMethod_PUT = 3;
        public const int _CallbackEncoding_INVALID = 0;
        public const int _CallbackEncoding_FORM = 1;
        public const int _CallbackEncoding_JSON = 2;
        public const int _CallbackEncoding_JSON_ARRAY = 3;
        public const int _CallbackEncoding_CSV = 4;
        public const int _CallbackEncoding_YOCTO_API = 5;
        public const int _CallbackEncoding_JSON_NUM = 6;
        public const int _CallbackEncoding_EMONCMS = 7;
        public const int _CallbackEncoding_AZURE = 8;
        public const int _CallbackEncoding_INFLUXDB = 9;
        public const int _CallbackEncoding_MQTT = 10;
        public const int _CallbackEncoding_YOCTO_API_JZON = 11;
        public const int _CallbackEncoding_PRTG = 12;
        public const int _CallbackEncoding_INFLUXDB_V2 = 13;
        public const int _CallbackTemplate_INVALID = 0;
        public const int _CallbackTemplate_OFF = 1;
        public const int _CallbackTemplate_ON = 2;
        public const string _CallbackCredentials_INVALID = YAPI.INVALID_STRING;
        public const int _CallbackInitialDelay_INVALID = -1;
        public const string _CallbackSchedule_INVALID = YAPI.INVALID_STRING;
        public const int _CallbackMinDelay_INVALID = -1;
        public const int _CallbackMaxDelay_INVALID = -1;
        public const int _PoeCurrent_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YNetwork _func;
        // property cache
        protected int _readiness = _Readiness_INVALID;
        protected string _macAddress = _MacAddress_INVALID;
        protected string _primaryDNS = _PrimaryDNS_INVALID;
        protected string _secondaryDNS = _SecondaryDNS_INVALID;
        protected string _ntpServer = _NtpServer_INVALID;
        protected string _userPassword = _UserPassword_INVALID;
        protected string _adminPassword = _AdminPassword_INVALID;
        protected int _httpPort = _HttpPort_INVALID;
        protected string _defaultPage = _DefaultPage_INVALID;
        protected int _discoverable = _Discoverable_INVALID;
        protected int _wwwWatchdogDelay = _WwwWatchdogDelay_INVALID;
        protected string _callbackUrl = _CallbackUrl_INVALID;
        protected int _callbackMethod = _CallbackMethod_INVALID;
        protected int _callbackEncoding = _CallbackEncoding_INVALID;
        protected int _callbackTemplate = _CallbackTemplate_INVALID;
        protected string _callbackCredentials = _CallbackCredentials_INVALID;
        protected int _callbackInitialDelay = _CallbackInitialDelay_INVALID;
        protected string _callbackSchedule = _CallbackSchedule_INVALID;
        protected int _callbackMinDelay = _CallbackMinDelay_INVALID;
        protected int _callbackMaxDelay = _CallbackMaxDelay_INVALID;
        protected string _ipAddress = _IpAddress_INVALID;
        //--- (end of YNetwork definitions)

        //--- (YNetwork implementation)
        internal YNetworkProxy(YNetwork hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Network " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YNetwork) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YNetwork hwd = YNetwork.FindNetwork(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YNetwork hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Network callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Network available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YNetwork.FindNetwork</c> to obtain an object that can control the
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
            YNetwork it = YNetwork.FirstNetwork();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextNetwork();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _ipAddress = _func.get_ipAddress();
            _macAddress = _func.get_macAddress();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _primaryDNS = _func.get_primaryDNS();
            _secondaryDNS = _func.get_secondaryDNS();
            _ntpServer = _func.get_ntpServer();
            _userPassword = _func.get_userPassword();
            _adminPassword = _func.get_adminPassword();
            _httpPort = _func.get_httpPort();
            _defaultPage = _func.get_defaultPage();
            _discoverable = _func.get_discoverable()+1;
            _wwwWatchdogDelay = _func.get_wwwWatchdogDelay();
            _callbackUrl = _func.get_callbackUrl();
            _callbackMethod = _func.get_callbackMethod()+1;
            _callbackEncoding = _func.get_callbackEncoding()+1;
            _callbackTemplate = _func.get_callbackTemplate()+1;
            _callbackCredentials = _func.get_callbackCredentials();
            _callbackInitialDelay = _func.get_callbackInitialDelay();
            _callbackSchedule = _func.get_callbackSchedule();
            _callbackMinDelay = _func.get_callbackMinDelay();
            _callbackMaxDelay = _func.get_callbackMaxDelay();
        }

        /**
         * <summary>
         *   Returns the current established working mode of the network interface.
         * <para>
         *   Level zero (DOWN_0) means that no hardware link has been detected. Either there is no signal
         *   on the network cable, or the selected wireless access point cannot be detected.
         *   Level 1 (LIVE_1) is reached when the network is detected, but is not yet connected.
         *   For a wireless network, this shows that the requested SSID is present.
         *   Level 2 (LINK_2) is reached when the hardware connection is established.
         *   For a wired network connection, level 2 means that the cable is attached at both ends.
         *   For a connection to a wireless access point, it shows that the security parameters
         *   are properly configured. For an ad-hoc wireless connection, it means that there is
         *   at least one other device connected on the ad-hoc network.
         *   Level 3 (DHCP_3) is reached when an IP address has been obtained using DHCP.
         *   Level 4 (DNS_4) is reached when the DNS server is reachable on the network.
         *   Level 5 (WWW_5) is reached when global connectivity is demonstrated by properly loading the
         *   current time from an NTP server.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YNetwork.READINESS_DOWN</c>, <c>YNetwork.READINESS_EXISTS</c>,
         *   <c>YNetwork.READINESS_LINKED</c>, <c>YNetwork.READINESS_LAN_OK</c> and
         *   <c>YNetwork.READINESS_WWW_OK</c> corresponding to the current established working mode of the network interface
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.READINESS_INVALID</c>.
         * </para>
         */
        public int get_readiness()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_readiness()+1;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Current established working mode of the network interface.</value>
        public int Readiness
        {
            get
            {
                if (_func == null) {
                    return _Readiness_INVALID;
                }
                if (_online) {
                    return _readiness;
                }
                return _Readiness_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "DOWN") {
                _readiness = 1;
            }
            if (value == "EXISTS") {
                _readiness = 2;
            }
            if (value == "LINKED") {
                _readiness = 3;
            }
            if (value == "LAN_OK") {
                _readiness = 4;
            }
            if (value == "WWW_OK") {
                _readiness = 5;
            }
            _ipAddress = _func.get_ipAddress();
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>IP address currently in use by the device.</value>
        public string IpAddress
        {
            get
            {
                if (_func == null) {
                    return _IpAddress_INVALID;
                }
                if (_online) {
                    return _ipAddress;
                }
                return _IpAddress_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the MAC address of the network interface.
         * <para>
         *   The MAC address is also available on a sticker
         *   on the module, in both numeric and barcode forms.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the MAC address of the network interface
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.MACADDRESS_INVALID</c>.
         * </para>
         */
        public string get_macAddress()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_macAddress();
        }

        // property with cached value for instant access (constant value)
        /// <value>MAC address of the network interface. The MAC address is also available on a sticker</value>
        public string MacAddress
        {
            get
            {
                if (_func == null) {
                    return _MacAddress_INVALID;
                }
                if (_online) {
                    return _macAddress;
                }
                return _MacAddress_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the IP address currently in use by the device.
         * <para>
         *   The address may have been configured
         *   statically, or provided by a DHCP server.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the IP address currently in use by the device
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.IPADDRESS_INVALID</c>.
         * </para>
         */
        public string get_ipAddress()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_ipAddress();
        }

        /**
         * <summary>
         *   Returns the subnet mask currently used by the device.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the subnet mask currently used by the device
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.SUBNETMASK_INVALID</c>.
         * </para>
         */
        public string get_subnetMask()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_subnetMask();
        }

        /**
         * <summary>
         *   Returns the IP address of the router on the device subnet (default gateway).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the IP address of the router on the device subnet (default gateway)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.ROUTER_INVALID</c>.
         * </para>
         */
        public string get_router()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_router();
        }

        /**
         * <summary>
         *   Returns the IP address of the DNS server currently used by the device.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the IP address of the DNS server currently used by the device
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CURRENTDNS_INVALID</c>.
         * </para>
         */
        public string get_currentDNS()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_currentDNS();
        }

        /**
         * <summary>
         *   Returns the IP configuration of the network interface.
         * <para>
         * </para>
         * <para>
         *   If the network interface is setup to use a static IP address, the string starts with "STATIC:" and
         *   is followed by three
         *   parameters, separated by "/". The first is the device IP address, followed by the subnet mask
         *   length, and finally the
         *   router IP address (default gateway). For instance: "STATIC:192.168.1.14/16/192.168.1.1"
         * </para>
         * <para>
         *   If the network interface is configured to receive its IP from a DHCP server, the string start with
         *   "DHCP:" and is followed by
         *   three parameters separated by "/". The first is the fallback IP address, then the fallback subnet
         *   mask length and finally the
         *   fallback router IP address. These three parameters are used when no DHCP reply is received.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the IP configuration of the network interface
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.IPCONFIG_INVALID</c>.
         * </para>
         */
        public string get_ipConfig()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_ipConfig();
        }

        /**
         * <summary>
         *   Returns the IP address of the primary name server to be used by the module.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the IP address of the primary name server to be used by the module
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.PRIMARYDNS_INVALID</c>.
         * </para>
         */
        public string get_primaryDNS()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_primaryDNS();
        }

        /**
         * <summary>
         *   Changes the IP address of the primary name server to be used by the module.
         * <para>
         *   When using DHCP, if a value is specified, it overrides the value received from the DHCP server.
         *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the IP address of the primary name server to be used by the module
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
        public int set_primaryDNS(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _PrimaryDNS_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_primaryDNS(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>IP address of the primary name server to be used by the module.</value>
        public string PrimaryDNS
        {
            get
            {
                if (_func == null) {
                    return _PrimaryDNS_INVALID;
                }
                if (_online) {
                    return _primaryDNS;
                }
                return _PrimaryDNS_INVALID;
            }
            set
            {
                setprop_primaryDNS(value);
            }
        }

        // private helper for magic property
        private void setprop_primaryDNS(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _PrimaryDNS_INVALID) {
                return;
            }
            if (newval == _primaryDNS) {
                return;
            }
            _func.set_primaryDNS(newval);
            _primaryDNS = newval;
        }

        /**
         * <summary>
         *   Returns the IP address of the secondary name server to be used by the module.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the IP address of the secondary name server to be used by the module
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.SECONDARYDNS_INVALID</c>.
         * </para>
         */
        public string get_secondaryDNS()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_secondaryDNS();
        }

        /**
         * <summary>
         *   Changes the IP address of the secondary name server to be used by the module.
         * <para>
         *   When using DHCP, if a value is specified, it overrides the value received from the DHCP server.
         *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the IP address of the secondary name server to be used by the module
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
        public int set_secondaryDNS(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _SecondaryDNS_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_secondaryDNS(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>IP address of the secondary name server to be used by the module.</value>
        public string SecondaryDNS
        {
            get
            {
                if (_func == null) {
                    return _SecondaryDNS_INVALID;
                }
                if (_online) {
                    return _secondaryDNS;
                }
                return _SecondaryDNS_INVALID;
            }
            set
            {
                setprop_secondaryDNS(value);
            }
        }

        // private helper for magic property
        private void setprop_secondaryDNS(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _SecondaryDNS_INVALID) {
                return;
            }
            if (newval == _secondaryDNS) {
                return;
            }
            _func.set_secondaryDNS(newval);
            _secondaryDNS = newval;
        }

        /**
         * <summary>
         *   Returns the IP address of the NTP server to be used by the device.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the IP address of the NTP server to be used by the device
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.NTPSERVER_INVALID</c>.
         * </para>
         */
        public string get_ntpServer()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_ntpServer();
        }

        /**
         * <summary>
         *   Changes the IP address of the NTP server to be used by the module.
         * <para>
         *   Use an empty
         *   string to restore the factory set  address.
         *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the IP address of the NTP server to be used by the module
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
        public int set_ntpServer(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _NtpServer_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_ntpServer(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>IP address of the NTP server to be used by the device.</value>
        public string NtpServer
        {
            get
            {
                if (_func == null) {
                    return _NtpServer_INVALID;
                }
                if (_online) {
                    return _ntpServer;
                }
                return _NtpServer_INVALID;
            }
            set
            {
                setprop_ntpServer(value);
            }
        }

        // private helper for magic property
        private void setprop_ntpServer(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _NtpServer_INVALID) {
                return;
            }
            if (newval == _ntpServer) {
                return;
            }
            _func.set_ntpServer(newval);
            _ntpServer = newval;
        }

        /**
         * <summary>
         *   Returns a hash string if a password has been set for "user" user,
         *   or an empty string otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to a hash string if a password has been set for "user" user,
         *   or an empty string otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.USERPASSWORD_INVALID</c>.
         * </para>
         */
        public string get_userPassword()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_userPassword();
        }

        /**
         * <summary>
         *   Changes the password for the "user" user.
         * <para>
         *   This password becomes instantly required
         *   to perform any use of the module. If the specified value is an
         *   empty string, a password is not required anymore.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the password for the "user" user
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
        public int set_userPassword(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _UserPassword_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_userPassword(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Hash string if a password has been set for "user" user,</value>
        public string UserPassword
        {
            get
            {
                if (_func == null) {
                    return _UserPassword_INVALID;
                }
                if (_online) {
                    return _userPassword;
                }
                return _UserPassword_INVALID;
            }
            set
            {
                setprop_userPassword(value);
            }
        }

        // private helper for magic property
        private void setprop_userPassword(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _UserPassword_INVALID) {
                return;
            }
            if (newval == _userPassword) {
                return;
            }
            _func.set_userPassword(newval);
            _userPassword = newval;
        }

        /**
         * <summary>
         *   Returns a hash string if a password has been set for user "admin",
         *   or an empty string otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to a hash string if a password has been set for user "admin",
         *   or an empty string otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.ADMINPASSWORD_INVALID</c>.
         * </para>
         */
        public string get_adminPassword()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_adminPassword();
        }

        /**
         * <summary>
         *   Changes the password for the "admin" user.
         * <para>
         *   This password becomes instantly required
         *   to perform any change of the module state. If the specified value is an
         *   empty string, a password is not required anymore.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the password for the "admin" user
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
        public int set_adminPassword(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _AdminPassword_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_adminPassword(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Hash string if a password has been set for user "admin",</value>
        public string AdminPassword
        {
            get
            {
                if (_func == null) {
                    return _AdminPassword_INVALID;
                }
                if (_online) {
                    return _adminPassword;
                }
                return _AdminPassword_INVALID;
            }
            set
            {
                setprop_adminPassword(value);
            }
        }

        // private helper for magic property
        private void setprop_adminPassword(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _AdminPassword_INVALID) {
                return;
            }
            if (newval == _adminPassword) {
                return;
            }
            _func.set_adminPassword(newval);
            _adminPassword = newval;
        }

        /**
         * <summary>
         *   Returns the TCP port used to serve the hub web UI.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the TCP port used to serve the hub web UI
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.HTTPPORT_INVALID</c>.
         * </para>
         */
        public int get_httpPort()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            res = _func.get_httpPort();
            if (res == YAPI.INVALID_INT) {
                res = _HttpPort_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the the TCP port used to serve the hub web UI.
         * <para>
         *   The default value is port 80,
         *   which is the default for all Web servers. Regardless of the value set here,
         *   the hub will always reply on port 4444, which is used by default by Yoctopuce
         *   API library. When you change this parameter, remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the the TCP port used to serve the hub web UI
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
        public int set_httpPort(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _HttpPort_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_httpPort(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>TCP port used to serve the hub web UI.</value>
        public int HttpPort
        {
            get
            {
                if (_func == null) {
                    return _HttpPort_INVALID;
                }
                if (_online) {
                    return _httpPort;
                }
                return _HttpPort_INVALID;
            }
            set
            {
                setprop_httpPort(value);
            }
        }

        // private helper for magic property
        private void setprop_httpPort(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _HttpPort_INVALID) {
                return;
            }
            if (newval == _httpPort) {
                return;
            }
            _func.set_httpPort(newval);
            _httpPort = newval;
        }

        /**
         * <summary>
         *   Returns the HTML page to serve for the URL "/"" of the hub.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the HTML page to serve for the URL "/"" of the hub
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.DEFAULTPAGE_INVALID</c>.
         * </para>
         */
        public string get_defaultPage()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_defaultPage();
        }

        /**
         * <summary>
         *   Changes the default HTML page returned by the hub.
         * <para>
         *   If not value are set the hub return
         *   "index.html" which is the web interface of the hub. It is possible to change this page
         *   for file that has been uploaded on the hub. The maximum filename size is 15 characters.
         *   When you change this parameter, remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the default HTML page returned by the hub
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
        public int set_defaultPage(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _DefaultPage_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_defaultPage(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>HTML page to serve for the URL "/"" of the hub.</value>
        public string DefaultPage
        {
            get
            {
                if (_func == null) {
                    return _DefaultPage_INVALID;
                }
                if (_online) {
                    return _defaultPage;
                }
                return _DefaultPage_INVALID;
            }
            set
            {
                setprop_defaultPage(value);
            }
        }

        // private helper for magic property
        private void setprop_defaultPage(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _DefaultPage_INVALID) {
                return;
            }
            if (newval == _defaultPage) {
                return;
            }
            _func.set_defaultPage(newval);
            _defaultPage = newval;
        }

        /**
         * <summary>
         *   Returns the activation state of the multicast announce protocols to allow easy
         *   discovery of the module in the network neighborhood (uPnP/Bonjour protocol).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YNetwork.DISCOVERABLE_FALSE</c> or <c>YNetwork.DISCOVERABLE_TRUE</c>, according to the
         *   activation state of the multicast announce protocols to allow easy
         *   discovery of the module in the network neighborhood (uPnP/Bonjour protocol)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.DISCOVERABLE_INVALID</c>.
         * </para>
         */
        public int get_discoverable()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_discoverable()+1;
        }

        /**
         * <summary>
         *   Changes the activation state of the multicast announce protocols to allow easy
         *   discovery of the module in the network neighborhood (uPnP/Bonjour protocol).
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YNetwork.DISCOVERABLE_FALSE</c> or <c>YNetwork.DISCOVERABLE_TRUE</c>, according to the
         *   activation state of the multicast announce protocols to allow easy
         *   discovery of the module in the network neighborhood (uPnP/Bonjour protocol)
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
        public int set_discoverable(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _Discoverable_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_discoverable(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Activation state of the multicast announce protocols to allow easy</value>
        public int Discoverable
        {
            get
            {
                if (_func == null) {
                    return _Discoverable_INVALID;
                }
                if (_online) {
                    return _discoverable;
                }
                return _Discoverable_INVALID;
            }
            set
            {
                setprop_discoverable(value);
            }
        }

        // private helper for magic property
        private void setprop_discoverable(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Discoverable_INVALID) {
                return;
            }
            if (newval == _discoverable) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_discoverable(newval-1);
            _discoverable = newval;
        }

        /**
         * <summary>
         *   Returns the allowed downtime of the WWW link (in seconds) before triggering an automated
         *   reboot to try to recover Internet connectivity.
         * <para>
         *   A zero value disables automated reboot
         *   in case of Internet connectivity loss.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the allowed downtime of the WWW link (in seconds) before triggering an automated
         *   reboot to try to recover Internet connectivity
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.WWWWATCHDOGDELAY_INVALID</c>.
         * </para>
         */
        public int get_wwwWatchdogDelay()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            res = _func.get_wwwWatchdogDelay();
            if (res == YAPI.INVALID_INT) {
                res = _WwwWatchdogDelay_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the allowed downtime of the WWW link (in seconds) before triggering an automated
         *   reboot to try to recover Internet connectivity.
         * <para>
         *   A zero value disables automated reboot
         *   in case of Internet connectivity loss. The smallest valid non-zero timeout is
         *   90 seconds. Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the allowed downtime of the WWW link (in seconds) before triggering an automated
         *   reboot to try to recover Internet connectivity
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
        public int set_wwwWatchdogDelay(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _WwwWatchdogDelay_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_wwwWatchdogDelay(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Allowed downtime of the WWW link (in seconds) before triggering an automated</value>
        public int WwwWatchdogDelay
        {
            get
            {
                if (_func == null) {
                    return _WwwWatchdogDelay_INVALID;
                }
                if (_online) {
                    return _wwwWatchdogDelay;
                }
                return _WwwWatchdogDelay_INVALID;
            }
            set
            {
                setprop_wwwWatchdogDelay(value);
            }
        }

        // private helper for magic property
        private void setprop_wwwWatchdogDelay(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _WwwWatchdogDelay_INVALID) {
                return;
            }
            if (newval == _wwwWatchdogDelay) {
                return;
            }
            _func.set_wwwWatchdogDelay(newval);
            _wwwWatchdogDelay = newval;
        }

        /**
         * <summary>
         *   Returns the callback URL to notify of significant state changes.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the callback URL to notify of significant state changes
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CALLBACKURL_INVALID</c>.
         * </para>
         */
        public string get_callbackUrl()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_callbackUrl();
        }

        /**
         * <summary>
         *   Changes the callback URL to notify significant state changes.
         * <para>
         *   Remember to call the
         *   <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the callback URL to notify significant state changes
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
        public int set_callbackUrl(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _CallbackUrl_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_callbackUrl(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Callback URL to notify of significant state changes.</value>
        public string CallbackUrl
        {
            get
            {
                if (_func == null) {
                    return _CallbackUrl_INVALID;
                }
                if (_online) {
                    return _callbackUrl;
                }
                return _CallbackUrl_INVALID;
            }
            set
            {
                setprop_callbackUrl(value);
            }
        }

        // private helper for magic property
        private void setprop_callbackUrl(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CallbackUrl_INVALID) {
                return;
            }
            if (newval == _callbackUrl) {
                return;
            }
            _func.set_callbackUrl(newval);
            _callbackUrl = newval;
        }

        /**
         * <summary>
         *   Returns the HTTP method used to notify callbacks for significant state changes.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YNetwork.CALLBACKMETHOD_POST</c>, <c>YNetwork.CALLBACKMETHOD_GET</c> and
         *   <c>YNetwork.CALLBACKMETHOD_PUT</c> corresponding to the HTTP method used to notify callbacks for
         *   significant state changes
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CALLBACKMETHOD_INVALID</c>.
         * </para>
         */
        public int get_callbackMethod()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_callbackMethod()+1;
        }

        /**
         * <summary>
         *   Changes the HTTP method used to notify callbacks for significant state changes.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YNetwork.CALLBACKMETHOD_POST</c>, <c>YNetwork.CALLBACKMETHOD_GET</c> and
         *   <c>YNetwork.CALLBACKMETHOD_PUT</c> corresponding to the HTTP method used to notify callbacks for
         *   significant state changes
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
        public int set_callbackMethod(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _CallbackMethod_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_callbackMethod(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>HTTP method used to notify callbacks for significant state changes.</value>
        public int CallbackMethod
        {
            get
            {
                if (_func == null) {
                    return _CallbackMethod_INVALID;
                }
                if (_online) {
                    return _callbackMethod;
                }
                return _CallbackMethod_INVALID;
            }
            set
            {
                setprop_callbackMethod(value);
            }
        }

        // private helper for magic property
        private void setprop_callbackMethod(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CallbackMethod_INVALID) {
                return;
            }
            if (newval == _callbackMethod) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_callbackMethod(newval-1);
            _callbackMethod = newval;
        }

        /**
         * <summary>
         *   Returns the encoding standard to use for representing notification values.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YNetwork.CALLBACKENCODING_FORM</c>, <c>YNetwork.CALLBACKENCODING_JSON</c>,
         *   <c>YNetwork.CALLBACKENCODING_JSON_ARRAY</c>, <c>YNetwork.CALLBACKENCODING_CSV</c>,
         *   <c>YNetwork.CALLBACKENCODING_YOCTO_API</c>, <c>YNetwork.CALLBACKENCODING_JSON_NUM</c>,
         *   <c>YNetwork.CALLBACKENCODING_EMONCMS</c>, <c>YNetwork.CALLBACKENCODING_AZURE</c>,
         *   <c>YNetwork.CALLBACKENCODING_INFLUXDB</c>, <c>YNetwork.CALLBACKENCODING_MQTT</c>,
         *   <c>YNetwork.CALLBACKENCODING_YOCTO_API_JZON</c>, <c>YNetwork.CALLBACKENCODING_PRTG</c> and
         *   <c>YNetwork.CALLBACKENCODING_INFLUXDB_V2</c> corresponding to the encoding standard to use for
         *   representing notification values
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CALLBACKENCODING_INVALID</c>.
         * </para>
         */
        public int get_callbackEncoding()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_callbackEncoding()+1;
        }

        /**
         * <summary>
         *   Changes the encoding standard to use for representing notification values.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YNetwork.CALLBACKENCODING_FORM</c>, <c>YNetwork.CALLBACKENCODING_JSON</c>,
         *   <c>YNetwork.CALLBACKENCODING_JSON_ARRAY</c>, <c>YNetwork.CALLBACKENCODING_CSV</c>,
         *   <c>YNetwork.CALLBACKENCODING_YOCTO_API</c>, <c>YNetwork.CALLBACKENCODING_JSON_NUM</c>,
         *   <c>YNetwork.CALLBACKENCODING_EMONCMS</c>, <c>YNetwork.CALLBACKENCODING_AZURE</c>,
         *   <c>YNetwork.CALLBACKENCODING_INFLUXDB</c>, <c>YNetwork.CALLBACKENCODING_MQTT</c>,
         *   <c>YNetwork.CALLBACKENCODING_YOCTO_API_JZON</c>, <c>YNetwork.CALLBACKENCODING_PRTG</c> and
         *   <c>YNetwork.CALLBACKENCODING_INFLUXDB_V2</c> corresponding to the encoding standard to use for
         *   representing notification values
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
        public int set_callbackEncoding(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _CallbackEncoding_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_callbackEncoding(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Encoding standard to use for representing notification values.</value>
        public int CallbackEncoding
        {
            get
            {
                if (_func == null) {
                    return _CallbackEncoding_INVALID;
                }
                if (_online) {
                    return _callbackEncoding;
                }
                return _CallbackEncoding_INVALID;
            }
            set
            {
                setprop_callbackEncoding(value);
            }
        }

        // private helper for magic property
        private void setprop_callbackEncoding(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CallbackEncoding_INVALID) {
                return;
            }
            if (newval == _callbackEncoding) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_callbackEncoding(newval-1);
            _callbackEncoding = newval;
        }

        /**
         * <summary>
         *   Returns the activation state of the custom template file to customize callback
         *   format.
         * <para>
         *   If the custom callback template is disabled, it will be ignored even
         *   if present on the YoctoHub.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YNetwork.CALLBACKTEMPLATE_OFF</c> or <c>YNetwork.CALLBACKTEMPLATE_ON</c>, according to
         *   the activation state of the custom template file to customize callback
         *   format
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CALLBACKTEMPLATE_INVALID</c>.
         * </para>
         */
        public int get_callbackTemplate()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_callbackTemplate()+1;
        }

        /**
         * <summary>
         *   Enable the use of a template file to customize callbacks format.
         * <para>
         *   When the custom callback template file is enabled, the template file
         *   will be loaded for each callback in order to build the data to post to the
         *   server. If template file does not exist on the YoctoHub, the callback will
         *   fail with an error message indicating the name of the expected template file.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YNetwork.CALLBACKTEMPLATE_OFF</c> or <c>YNetwork.CALLBACKTEMPLATE_ON</c>
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
        public int set_callbackTemplate(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _CallbackTemplate_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_callbackTemplate(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Activation state of the custom template file to customize callback</value>
        public int CallbackTemplate
        {
            get
            {
                if (_func == null) {
                    return _CallbackTemplate_INVALID;
                }
                if (_online) {
                    return _callbackTemplate;
                }
                return _CallbackTemplate_INVALID;
            }
            set
            {
                setprop_callbackTemplate(value);
            }
        }

        // private helper for magic property
        private void setprop_callbackTemplate(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CallbackTemplate_INVALID) {
                return;
            }
            if (newval == _callbackTemplate) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_callbackTemplate(newval-1);
            _callbackTemplate = newval;
        }

        /**
         * <summary>
         *   Returns a hashed version of the notification callback credentials if set,
         *   or an empty string otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to a hashed version of the notification callback credentials if set,
         *   or an empty string otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CALLBACKCREDENTIALS_INVALID</c>.
         * </para>
         */
        public string get_callbackCredentials()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_callbackCredentials();
        }

        /**
         * <summary>
         *   Changes the credentials required to connect to the callback address.
         * <para>
         *   The credentials
         *   must be provided as returned by function <c>get_callbackCredentials</c>,
         *   in the form <c>username:hash</c>. The method used to compute the hash varies according
         *   to the the authentication scheme implemented by the callback, For Basic authentication,
         *   the hash is the MD5 of the string <c>username:password</c>. For Digest authentication,
         *   the hash is the MD5 of the string <c>username:realm:password</c>. For a simpler
         *   way to configure callback credentials, use function <c>callbackLogin</c> instead.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the credentials required to connect to the callback address
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
        public int set_callbackCredentials(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _CallbackCredentials_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_callbackCredentials(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Hashed version of the notification callback credentials if set,</value>
        public string CallbackCredentials
        {
            get
            {
                if (_func == null) {
                    return _CallbackCredentials_INVALID;
                }
                if (_online) {
                    return _callbackCredentials;
                }
                return _CallbackCredentials_INVALID;
            }
            set
            {
                setprop_callbackCredentials(value);
            }
        }

        // private helper for magic property
        private void setprop_callbackCredentials(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CallbackCredentials_INVALID) {
                return;
            }
            if (newval == _callbackCredentials) {
                return;
            }
            _func.set_callbackCredentials(newval);
            _callbackCredentials = newval;
        }

        /**
         * <summary>
         *   Connects to the notification callback and saves the credentials required to
         *   log into it.
         * <para>
         *   The password is not stored into the module, only a hashed
         *   copy of the credentials are saved. Remember to call the
         *   <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="username">
         *   username required to log to the callback
         * </param>
         * <param name="password">
         *   password required to log to the callback
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
        public int callbackLogin(string username,string password)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.callbackLogin(username, password);
        }

        /**
         * <summary>
         *   Returns the initial waiting time before first callback notifications, in seconds.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the initial waiting time before first callback notifications, in seconds
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CALLBACKINITIALDELAY_INVALID</c>.
         * </para>
         */
        public int get_callbackInitialDelay()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            res = _func.get_callbackInitialDelay();
            if (res == YAPI.INVALID_INT) {
                res = _CallbackInitialDelay_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the initial waiting time before first callback notifications, in seconds.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the initial waiting time before first callback notifications, in seconds
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
        public int set_callbackInitialDelay(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _CallbackInitialDelay_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_callbackInitialDelay(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Initial waiting time before first callback notifications, in seconds.</value>
        public int CallbackInitialDelay
        {
            get
            {
                if (_func == null) {
                    return _CallbackInitialDelay_INVALID;
                }
                if (_online) {
                    return _callbackInitialDelay;
                }
                return _CallbackInitialDelay_INVALID;
            }
            set
            {
                setprop_callbackInitialDelay(value);
            }
        }

        // private helper for magic property
        private void setprop_callbackInitialDelay(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CallbackInitialDelay_INVALID) {
                return;
            }
            if (newval == _callbackInitialDelay) {
                return;
            }
            _func.set_callbackInitialDelay(newval);
            _callbackInitialDelay = newval;
        }

        /**
         * <summary>
         *   Returns the HTTP callback schedule strategy, as a text string.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the HTTP callback schedule strategy, as a text string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CALLBACKSCHEDULE_INVALID</c>.
         * </para>
         */
        public string get_callbackSchedule()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.get_callbackSchedule();
        }

        /**
         * <summary>
         *   Changes the HTTP callback schedule strategy, as a text string.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the HTTP callback schedule strategy, as a text string
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
        public int set_callbackSchedule(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _CallbackSchedule_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_callbackSchedule(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>HTTP callback schedule strategy, as a text string.</value>
        public string CallbackSchedule
        {
            get
            {
                if (_func == null) {
                    return _CallbackSchedule_INVALID;
                }
                if (_online) {
                    return _callbackSchedule;
                }
                return _CallbackSchedule_INVALID;
            }
            set
            {
                setprop_callbackSchedule(value);
            }
        }

        // private helper for magic property
        private void setprop_callbackSchedule(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CallbackSchedule_INVALID) {
                return;
            }
            if (newval == _callbackSchedule) {
                return;
            }
            _func.set_callbackSchedule(newval);
            _callbackSchedule = newval;
        }

        /**
         * <summary>
         *   Returns the minimum waiting time between two HTTP callbacks, in seconds.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the minimum waiting time between two HTTP callbacks, in seconds
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CALLBACKMINDELAY_INVALID</c>.
         * </para>
         */
        public int get_callbackMinDelay()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            res = _func.get_callbackMinDelay();
            if (res == YAPI.INVALID_INT) {
                res = _CallbackMinDelay_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the minimum waiting time between two HTTP callbacks, in seconds.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the minimum waiting time between two HTTP callbacks, in seconds
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
        public int set_callbackMinDelay(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _CallbackMinDelay_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_callbackMinDelay(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Minimum waiting time between two HTTP callbacks, in seconds.</value>
        public int CallbackMinDelay
        {
            get
            {
                if (_func == null) {
                    return _CallbackMinDelay_INVALID;
                }
                if (_online) {
                    return _callbackMinDelay;
                }
                return _CallbackMinDelay_INVALID;
            }
            set
            {
                setprop_callbackMinDelay(value);
            }
        }

        // private helper for magic property
        private void setprop_callbackMinDelay(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CallbackMinDelay_INVALID) {
                return;
            }
            if (newval == _callbackMinDelay) {
                return;
            }
            _func.set_callbackMinDelay(newval);
            _callbackMinDelay = newval;
        }

        /**
         * <summary>
         *   Returns the waiting time between two HTTP callbacks when there is nothing new.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the waiting time between two HTTP callbacks when there is nothing new
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.CALLBACKMAXDELAY_INVALID</c>.
         * </para>
         */
        public int get_callbackMaxDelay()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            res = _func.get_callbackMaxDelay();
            if (res == YAPI.INVALID_INT) {
                res = _CallbackMaxDelay_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the waiting time between two HTTP callbacks when there is nothing new.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the waiting time between two HTTP callbacks when there is nothing new
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
        public int set_callbackMaxDelay(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            if (newval == _CallbackMaxDelay_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_callbackMaxDelay(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Waiting time between two HTTP callbacks when there is nothing new.</value>
        public int CallbackMaxDelay
        {
            get
            {
                if (_func == null) {
                    return _CallbackMaxDelay_INVALID;
                }
                if (_online) {
                    return _callbackMaxDelay;
                }
                return _CallbackMaxDelay_INVALID;
            }
            set
            {
                setprop_callbackMaxDelay(value);
            }
        }

        // private helper for magic property
        private void setprop_callbackMaxDelay(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CallbackMaxDelay_INVALID) {
                return;
            }
            if (newval == _callbackMaxDelay) {
                return;
            }
            _func.set_callbackMaxDelay(newval);
            _callbackMaxDelay = newval;
        }

        /**
         * <summary>
         *   Returns the current consumed by the module from Power-over-Ethernet (PoE), in milliamps.
         * <para>
         *   The current consumption is measured after converting PoE source to 5 Volt, and should
         *   never exceed 1800 mA.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current consumed by the module from Power-over-Ethernet (PoE), in milliamps
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YNetwork.POECURRENT_INVALID</c>.
         * </para>
         */
        public int get_poeCurrent()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            res = _func.get_poeCurrent();
            if (res == YAPI.INVALID_INT) {
                res = _PoeCurrent_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the configuration of the network interface to enable the use of an
         *   IP address received from a DHCP server.
         * <para>
         *   Until an address is received from a DHCP
         *   server, the module uses the IP parameters specified to this function.
         *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
         * </para>
         * </summary>
         * <param name="fallbackIpAddr">
         *   fallback IP address, to be used when no DHCP reply is received
         * </param>
         * <param name="fallbackSubnetMaskLen">
         *   fallback subnet mask length when no DHCP reply is received, as an
         *   integer (e.g. 24 means 255.255.255.0)
         * </param>
         * <param name="fallbackRouter">
         *   fallback router IP address, to be used when no DHCP reply is received
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int useDHCP(string fallbackIpAddr, int fallbackSubnetMaskLen, string fallbackRouter)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.useDHCP(fallbackIpAddr, fallbackSubnetMaskLen, fallbackRouter);
        }

        /**
         * <summary>
         *   Changes the configuration of the network interface to enable the use of an
         *   IP address received from a DHCP server.
         * <para>
         *   Until an address is received from a DHCP
         *   server, the module uses an IP of the network 169.254.0.0/16 (APIPA).
         *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int useDHCPauto()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.useDHCPauto();
        }

        /**
         * <summary>
         *   Changes the configuration of the network interface to use a static IP address.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
         * </para>
         * </summary>
         * <param name="ipAddress">
         *   device IP address
         * </param>
         * <param name="subnetMaskLen">
         *   subnet mask length, as an integer (e.g. 24 means 255.255.255.0)
         * </param>
         * <param name="router">
         *   router IP address (default gateway)
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int useStaticIP(string ipAddress, int subnetMaskLen, string router)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.useStaticIP(ipAddress, subnetMaskLen, router);
        }

        /**
         * <summary>
         *   Pings host to test the network connectivity.
         * <para>
         *   Sends four ICMP ECHO_REQUEST requests from the
         *   module to the target host. This method returns a string with the result of the
         *   4 ICMP ECHO_REQUEST requests.
         * </para>
         * </summary>
         * <param name="host">
         *   the hostname or the IP address of the target
         * </param>
         * <para>
         * </para>
         * <returns>
         *   a string with the result of the ping.
         * </returns>
         */
        public virtual string ping(string host)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.ping(host);
        }

        /**
         * <summary>
         *   Trigger an HTTP callback quickly.
         * <para>
         *   This function can even be called within
         *   an HTTP callback, in which case the next callback will be triggered 5 seconds
         *   after the end of the current callback, regardless if the minimum time between
         *   callbacks configured in the device.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int triggerCallback()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.triggerCallback();
        }

        /**
         * <summary>
         *   Setup periodic HTTP callbacks (simplified function).
         * <para>
         * </para>
         * </summary>
         * <param name="interval">
         *   a string representing the callback periodicity, expressed in
         *   seconds, minutes or hours, eg. "60s", "5m", "1h", "48h".
         * </param>
         * <param name="offset">
         *   an integer representing the time offset relative to the period
         *   when the callback should occur. For instance, if the periodicity is
         *   24h, an offset of 7 will make the callback occur each day at 7AM.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_periodicCallbackSchedule(string interval, int offset)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Network connected");
            }
            return _func.set_periodicCallbackSchedule(interval, offset);
        }
    }
    //--- (end of YNetwork implementation)
}

