/*********************************************************************
 *
 *  $Id: yocto_bluetoothlink_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
 *
 *  Implements YBluetoothLinkProxy, the Proxy API for BluetoothLink
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
    //--- (YBluetoothLink class start)
    static public partial class YoctoProxyManager
    {
        public static YBluetoothLinkProxy FindBluetoothLink(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YBluetoothLink func = null;
            YBluetoothLinkProxy res = (YBluetoothLinkProxy)YFunctionProxy.FindSimilarUnknownFunction("YBluetoothLinkProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YBluetoothLinkProxy)YFunctionProxy.FindSimilarKnownFunction("YBluetoothLinkProxy");
                if (res != null) return res;
                func = YBluetoothLink.FirstBluetoothLink();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YBluetoothLinkProxy)func.get_userData();
                    }
                }
            } else {
                func = YBluetoothLink.FindBluetoothLink(name);
                if (func.get_userData() != null) {
                    return (YBluetoothLinkProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YBluetoothLinkProxy(func, name);
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
 *   BluetoothLink function provides control over Bluetooth link
 *   and status for devices that are Bluetooth-enabled.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YBluetoothLinkProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a Bluetooth sound controller for a given identifier.
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
         *   This function does not require that the Bluetooth sound controller is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YBluetoothLink.isOnline()</c> to test if the Bluetooth sound controller is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a Bluetooth sound controller by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the Bluetooth sound controller, for instance
         *   <c>MyDevice.bluetoothLink1</c>.
         * </param>
         * <returns>
         *   a <c>YBluetoothLink</c> object allowing you to drive the Bluetooth sound controller.
         * </returns>
         */
        public static YBluetoothLinkProxy FindBluetoothLink(string func)
        {
            return YoctoProxyManager.FindBluetoothLink(func);
        }
        //--- (end of YBluetoothLink class start)
        //--- (YBluetoothLink definitions)
        public const string _OwnAddress_INVALID = YAPI.INVALID_STRING;
        public const string _PairingPin_INVALID = YAPI.INVALID_STRING;
        public const string _RemoteAddress_INVALID = YAPI.INVALID_STRING;
        public const string _RemoteName_INVALID = YAPI.INVALID_STRING;
        public const int _Mute_INVALID = 0;
        public const int _Mute_FALSE = 1;
        public const int _Mute_TRUE = 2;
        public const int _PreAmplifier_INVALID = -1;
        public const int _Volume_INVALID = -1;
        public const int _LinkState_INVALID = 0;
        public const int _LinkState_DOWN = 1;
        public const int _LinkState_FREE = 2;
        public const int _LinkState_SEARCH = 3;
        public const int _LinkState_EXISTS = 4;
        public const int _LinkState_LINKED = 5;
        public const int _LinkState_PLAY = 6;
        public const int _LinkQuality_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YBluetoothLink _func;
        // property cache
        protected string _pairingPin = _PairingPin_INVALID;
        protected string _remoteAddress = _RemoteAddress_INVALID;
        protected int _mute = _Mute_INVALID;
        protected int _preAmplifier = _PreAmplifier_INVALID;
        protected int _linkQuality = _LinkQuality_INVALID;
        //--- (end of YBluetoothLink definitions)

        //--- (YBluetoothLink implementation)
        internal YBluetoothLinkProxy(YBluetoothLink hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("BluetoothLink " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YBluetoothLink) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YBluetoothLink hwd = YBluetoothLink.FindBluetoothLink(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YBluetoothLink hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering BluetoothLink callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type BluetoothLink available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YBluetoothLink.FindBluetoothLink</c> to obtain an object that can control the
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
            YBluetoothLink it = YBluetoothLink.FirstBluetoothLink();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextBluetoothLink();
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
            _pairingPin = _func.get_pairingPin();
            _remoteAddress = _func.get_remoteAddress();
            _mute = _func.get_mute()+1;
            _preAmplifier = _func.get_preAmplifier();
        }

        /**
         * <summary>
         *   Returns the MAC-48 address of the bluetooth interface, which is unique on the bluetooth network.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the MAC-48 address of the bluetooth interface, which is unique on the
         *   bluetooth network
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBluetoothLink.OWNADDRESS_INVALID</c>.
         * </para>
         */
        public string get_ownAddress()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            return _func.get_ownAddress();
        }

        /**
         * <summary>
         *   Returns an opaque string if a PIN code has been configured in the device to access
         *   the SIM card, or an empty string if none has been configured or if the code provided
         *   was rejected by the SIM card.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to an opaque string if a PIN code has been configured in the device to access
         *   the SIM card, or an empty string if none has been configured or if the code provided
         *   was rejected by the SIM card
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBluetoothLink.PAIRINGPIN_INVALID</c>.
         * </para>
         */
        public string get_pairingPin()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            return _func.get_pairingPin();
        }

        /**
         * <summary>
         *   Changes the PIN code used by the module for bluetooth pairing.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module to save the
         *   new value in the device flash.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the PIN code used by the module for bluetooth pairing
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
        public int set_pairingPin(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            if (newval == _PairingPin_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_pairingPin(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>N opaque string if a PIN code has been configured in the device to access</value>
        public string PairingPin
        {
            get
            {
                if (_func == null) {
                    return _PairingPin_INVALID;
                }
                if (_online) {
                    return _pairingPin;
                }
                return _PairingPin_INVALID;
            }
            set
            {
                setprop_pairingPin(value);
            }
        }

        // private helper for magic property
        private void setprop_pairingPin(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _PairingPin_INVALID) {
                return;
            }
            if (newval == _pairingPin) {
                return;
            }
            _func.set_pairingPin(newval);
            _pairingPin = newval;
        }

        /**
         * <summary>
         *   Returns the MAC-48 address of the remote device to connect to.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the MAC-48 address of the remote device to connect to
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBluetoothLink.REMOTEADDRESS_INVALID</c>.
         * </para>
         */
        public string get_remoteAddress()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            return _func.get_remoteAddress();
        }

        /**
         * <summary>
         *   Changes the MAC-48 address defining which remote device to connect to.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the MAC-48 address defining which remote device to connect to
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
        public int set_remoteAddress(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            if (newval == _RemoteAddress_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_remoteAddress(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>MAC-48 address of the remote device to connect to.</value>
        public string RemoteAddress
        {
            get
            {
                if (_func == null) {
                    return _RemoteAddress_INVALID;
                }
                if (_online) {
                    return _remoteAddress;
                }
                return _RemoteAddress_INVALID;
            }
            set
            {
                setprop_remoteAddress(value);
            }
        }

        // private helper for magic property
        private void setprop_remoteAddress(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _RemoteAddress_INVALID) {
                return;
            }
            if (newval == _remoteAddress) {
                return;
            }
            _func.set_remoteAddress(newval);
            _remoteAddress = newval;
        }

        /**
         * <summary>
         *   Returns the bluetooth name the remote device, if found on the bluetooth network.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the bluetooth name the remote device, if found on the bluetooth network
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBluetoothLink.REMOTENAME_INVALID</c>.
         * </para>
         */
        public string get_remoteName()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            return _func.get_remoteName();
        }

        /**
         * <summary>
         *   Returns the state of the mute function.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YBluetoothLink.MUTE_FALSE</c> or <c>YBluetoothLink.MUTE_TRUE</c>, according to the state
         *   of the mute function
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBluetoothLink.MUTE_INVALID</c>.
         * </para>
         */
        public int get_mute()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_mute()+1;
        }

        /**
         * <summary>
         *   Changes the state of the mute function.
         * <para>
         *   Remember to call the matching module
         *   <c>saveToFlash()</c> method to save the setting permanently.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YBluetoothLink.MUTE_FALSE</c> or <c>YBluetoothLink.MUTE_TRUE</c>, according to the state
         *   of the mute function
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
        public int set_mute(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            if (newval == _Mute_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_mute(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>State of the mute function.</value>
        public int Mute
        {
            get
            {
                if (_func == null) {
                    return _Mute_INVALID;
                }
                if (_online) {
                    return _mute;
                }
                return _Mute_INVALID;
            }
            set
            {
                setprop_mute(value);
            }
        }

        // private helper for magic property
        private void setprop_mute(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Mute_INVALID) {
                return;
            }
            if (newval == _mute) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_mute(newval-1);
            _mute = newval;
        }

        /**
         * <summary>
         *   Returns the audio pre-amplifier volume, in per cents.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the audio pre-amplifier volume, in per cents
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBluetoothLink.PREAMPLIFIER_INVALID</c>.
         * </para>
         */
        public int get_preAmplifier()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            res = _func.get_preAmplifier();
            if (res == YAPI.INVALID_INT) {
                res = _PreAmplifier_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the audio pre-amplifier volume, in per cents.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the audio pre-amplifier volume, in per cents
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
        public int set_preAmplifier(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            if (newval == _PreAmplifier_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_preAmplifier(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Audio pre-amplifier volume, in per cents.</value>
        public int PreAmplifier
        {
            get
            {
                if (_func == null) {
                    return _PreAmplifier_INVALID;
                }
                if (_online) {
                    return _preAmplifier;
                }
                return _PreAmplifier_INVALID;
            }
            set
            {
                setprop_preAmplifier(value);
            }
        }

        // private helper for magic property
        private void setprop_preAmplifier(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _PreAmplifier_INVALID) {
                return;
            }
            if (newval == _preAmplifier) {
                return;
            }
            _func.set_preAmplifier(newval);
            _preAmplifier = newval;
        }

        /**
         * <summary>
         *   Returns the connected headset volume, in per cents.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the connected headset volume, in per cents
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBluetoothLink.VOLUME_INVALID</c>.
         * </para>
         */
        public int get_volume()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            res = _func.get_volume();
            if (res == YAPI.INVALID_INT) {
                res = _Volume_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the connected headset volume, in per cents.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the connected headset volume, in per cents
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
        public int set_volume(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            if (newval == _Volume_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_volume(newval);
        }

        /**
         * <summary>
         *   Returns the bluetooth link state.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YBluetoothLink.LINKSTATE_DOWN</c>, <c>YBluetoothLink.LINKSTATE_FREE</c>,
         *   <c>YBluetoothLink.LINKSTATE_SEARCH</c>, <c>YBluetoothLink.LINKSTATE_EXISTS</c>,
         *   <c>YBluetoothLink.LINKSTATE_LINKED</c> and <c>YBluetoothLink.LINKSTATE_PLAY</c> corresponding to
         *   the bluetooth link state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBluetoothLink.LINKSTATE_INVALID</c>.
         * </para>
         */
        public int get_linkState()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_linkState()+1;
        }

        /**
         * <summary>
         *   Returns the bluetooth receiver signal strength, in pourcents, or 0 if no connection is established.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the bluetooth receiver signal strength, in pourcents, or 0 if no
         *   connection is established
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBluetoothLink.LINKQUALITY_INVALID</c>.
         * </para>
         */
        public int get_linkQuality()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            res = _func.get_linkQuality();
            if (res == YAPI.INVALID_INT) {
                res = _LinkQuality_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Bluetooth receiver signal strength, in pourcents, or 0 if no connection is established.</value>
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

        /**
         * <summary>
         *   Attempt to connect to the previously selected remote device.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int connect()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            return _func.connect();
        }

        /**
         * <summary>
         *   Disconnect from the previously selected remote device.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int disconnect()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No BluetoothLink connected");
            }
            return _func.disconnect();
        }
    }
    //--- (end of YBluetoothLink implementation)
}

