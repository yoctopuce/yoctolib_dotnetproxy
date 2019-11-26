/*********************************************************************
 *
 *  $Id: yocto_cellular_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YCellularProxy, the Proxy API for Cellular
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

namespace YoctoProxyAPI
{
    //--- (generated code: YCellular class start)
    static public partial class YoctoProxyManager
    {
        public static YCellularProxy FindCellular(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YCellular func = null;
            YCellularProxy res = (YCellularProxy)YFunctionProxy.FindSimilarUnknownFunction("YCellularProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YCellularProxy)YFunctionProxy.FindSimilarKnownFunction("YCellularProxy");
                if (res != null) return res;
                func = YCellular.FirstCellular();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YCellularProxy)func.get_userData();
                    }
                }
            } else {
                func = YCellular.FindCellular(name);
                if (func.get_userData() != null) {
                    return (YCellularProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YCellularProxy(func, name);
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
 *   The YCellular class provides control over cellular network parameters
 *   and status for devices that are GSM-enabled, for instance using a YoctoHub-GSM-3G-NA, a YoctoHub-GSM-3G-EU or a YoctoHub-GSM-2G.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YCellularProxy : YFunctionProxy
    {
        //--- (end of generated code: YCellular class start)
        //--- (generated code: YCellular definitions)
        public const int _LinkQuality_INVALID = -1;
        public const string _CellOperator_INVALID = YAPI.INVALID_STRING;
        public const string _CellIdentifier_INVALID = YAPI.INVALID_STRING;
        public const int _CellType_INVALID = 0;
        public const int _CellType_GPRS = 1;
        public const int _CellType_EGPRS = 2;
        public const int _CellType_WCDMA = 3;
        public const int _CellType_HSDPA = 4;
        public const int _CellType_NONE = 5;
        public const int _CellType_CDMA = 6;
        public const string _Imsi_INVALID = YAPI.INVALID_STRING;
        public const string _Message_INVALID = YAPI.INVALID_STRING;
        public const string _Pin_INVALID = YAPI.INVALID_STRING;
        public const string _LockedOperator_INVALID = YAPI.INVALID_STRING;
        public const int _AirplaneMode_INVALID = 0;
        public const int _AirplaneMode_OFF = 1;
        public const int _AirplaneMode_ON = 2;
        public const int _EnableData_INVALID = 0;
        public const int _EnableData_HOMENETWORK = 1;
        public const int _EnableData_ROAMING = 2;
        public const int _EnableData_NEVER = 3;
        public const int _EnableData_NEUTRALITY = 4;
        public const string _Apn_INVALID = YAPI.INVALID_STRING;
        public const string _ApnSecret_INVALID = YAPI.INVALID_STRING;
        public const int _PingInterval_INVALID = -1;
        public const int _DataSent_INVALID = -1;
        public const int _DataReceived_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YCellular _func;
        // property cache
        protected string _pin = _Pin_INVALID;
        protected string _lockedOperator = _LockedOperator_INVALID;
        protected int _enableData = _EnableData_INVALID;
        protected string _apn = _Apn_INVALID;
        protected int _pingInterval = _PingInterval_INVALID;
        //--- (end of generated code: YCellular definitions)

        //--- (generated code: YCellular implementation)
        internal YCellularProxy(YCellular hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Cellular " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YCellular) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YCellular hwd = YCellular.FindCellular(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YCellular hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Cellular callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YCellular it = YCellular.FirstCellular();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextCellular();
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
            _pin = _func.get_pin();
            _lockedOperator = _func.get_lockedOperator();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _enableData = _func.get_enableData()+1;
            _apn = _func.get_apn();
            _pingInterval = _func.get_pingInterval();
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
         *   On failure, throws an exception or returns <c>YCellular.LINKQUALITY_INVALID</c>.
         * </para>
         */
        public int get_linkQuality()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_linkQuality();
            if (res == YAPI.INVALID_INT) res = _LinkQuality_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the name of the cell operator currently in use.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the name of the cell operator currently in use
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.CELLOPERATOR_INVALID</c>.
         * </para>
         */
        public string get_cellOperator()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_cellOperator();
        }

        /**
         * <summary>
         *   Returns the unique identifier of the cellular antenna in use: MCC, MNC, LAC and Cell ID.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the unique identifier of the cellular antenna in use: MCC, MNC, LAC and Cell ID
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.CELLIDENTIFIER_INVALID</c>.
         * </para>
         */
        public string get_cellIdentifier()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_cellIdentifier();
        }

        /**
         * <summary>
         *   Active cellular connection type.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YCellular.CELLTYPE_GPRS</c>, <c>YCellular.CELLTYPE_EGPRS</c>,
         *   <c>YCellular.CELLTYPE_WCDMA</c>, <c>YCellular.CELLTYPE_HSDPA</c>, <c>YCellular.CELLTYPE_NONE</c>
         *   and <c>YCellular.CELLTYPE_CDMA</c>
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.CELLTYPE_INVALID</c>.
         * </para>
         */
        public int get_cellType()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_cellType()+1;
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
         *   On failure, throws an exception or returns <c>YCellular.IMSI_INVALID</c>.
         * </para>
         */
        public string get_imsi()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_imsi();
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
         *   On failure, throws an exception or returns <c>YCellular.MESSAGE_INVALID</c>.
         * </para>
         */
        public string get_message()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_message();
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
         *   On failure, throws an exception or returns <c>YCellular.PIN_INVALID</c>.
         * </para>
         */
        public string get_pin()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_pin();
        }

        /**
         * <summary>
         *   Changes the PIN code used by the module to access the SIM card.
         * <para>
         *   This function does not change the code on the SIM card itself, but only changes
         *   the parameter used by the device to try to get access to it. If the SIM code
         *   does not work immediately on first try, it will be automatically forgotten
         *   and the message will be set to "Enter SIM PIN". The method should then be
         *   invoked again with right correct PIN code. After three failed attempts in a row,
         *   the message is changed to "Enter SIM PUK" and the SIM card PUK code must be
         *   provided using method <c>sendPUK</c>.
         * </para>
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module to save the
         *   new value in the device flash.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the PIN code used by the module to access the SIM card
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
        public int set_pin(string newval)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Pin_INVALID) return YAPI.SUCCESS;
            return _func.set_pin(newval);
        }


        // property with cached value for instant access (configuration)
        public string Pin
        {
            get
            {
                if (_func == null) return _Pin_INVALID;
                return (_online ? _pin : _Pin_INVALID);
            }
            set
            {
                setprop_pin(value);
            }
        }

        // private helper for magic property
        private void setprop_pin(string newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Pin_INVALID) return;
            if (newval == _pin) return;
            _func.set_pin(newval);
            _pin = newval;
        }

        /**
         * <summary>
         *   Returns the name of the only cell operator to use if automatic choice is disabled,
         *   or an empty string if the SIM card will automatically choose among available
         *   cell operators.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the name of the only cell operator to use if automatic choice is disabled,
         *   or an empty string if the SIM card will automatically choose among available
         *   cell operators
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.LOCKEDOPERATOR_INVALID</c>.
         * </para>
         */
        public string get_lockedOperator()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_lockedOperator();
        }

        /**
         * <summary>
         *   Changes the name of the cell operator to be used.
         * <para>
         *   If the name is an empty
         *   string, the choice will be made automatically based on the SIM card. Otherwise,
         *   the selected operator is the only one that will be used.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the name of the cell operator to be used
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
        public int set_lockedOperator(string newval)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _LockedOperator_INVALID) return YAPI.SUCCESS;
            return _func.set_lockedOperator(newval);
        }


        // property with cached value for instant access (configuration)
        public string LockedOperator
        {
            get
            {
                if (_func == null) return _LockedOperator_INVALID;
                return (_online ? _lockedOperator : _LockedOperator_INVALID);
            }
            set
            {
                setprop_lockedOperator(value);
            }
        }

        // private helper for magic property
        private void setprop_lockedOperator(string newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _LockedOperator_INVALID) return;
            if (newval == _lockedOperator) return;
            _func.set_lockedOperator(newval);
            _lockedOperator = newval;
        }

        /**
         * <summary>
         *   Returns true if the airplane mode is active (radio turned off).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YCellular.AIRPLANEMODE_OFF</c> or <c>YCellular.AIRPLANEMODE_ON</c>, according to true if
         *   the airplane mode is active (radio turned off)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.AIRPLANEMODE_INVALID</c>.
         * </para>
         */
        public int get_airplaneMode()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_airplaneMode()+1;
        }

        /**
         * <summary>
         *   Changes the activation state of airplane mode (radio turned off).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YCellular.AIRPLANEMODE_OFF</c> or <c>YCellular.AIRPLANEMODE_ON</c>, according to the
         *   activation state of airplane mode (radio turned off)
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
        public int set_airplaneMode(int newval)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _AirplaneMode_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_airplaneMode(newval-1);
        }


        /**
         * <summary>
         *   Returns the condition for enabling IP data services (GPRS).
         * <para>
         *   When data services are disabled, SMS are the only mean of communication.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YCellular.ENABLEDATA_HOMENETWORK</c>, <c>YCellular.ENABLEDATA_ROAMING</c>,
         *   <c>YCellular.ENABLEDATA_NEVER</c> and <c>YCellular.ENABLEDATA_NEUTRALITY</c> corresponding to the
         *   condition for enabling IP data services (GPRS)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.ENABLEDATA_INVALID</c>.
         * </para>
         */
        public int get_enableData()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enableData()+1;
        }

        /**
         * <summary>
         *   Changes the condition for enabling IP data services (GPRS).
         * <para>
         *   The service can be either fully deactivated, or limited to the SIM home network,
         *   or enabled for all partner networks (roaming). Caution: enabling data services
         *   on roaming networks may cause prohibitive communication costs !
         * </para>
         * <para>
         *   When data services are disabled, SMS are the only mean of communication.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YCellular.ENABLEDATA_HOMENETWORK</c>, <c>YCellular.ENABLEDATA_ROAMING</c>,
         *   <c>YCellular.ENABLEDATA_NEVER</c> and <c>YCellular.ENABLEDATA_NEUTRALITY</c> corresponding to the
         *   condition for enabling IP data services (GPRS)
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
        public int set_enableData(int newval)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _EnableData_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enableData(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int EnableData
        {
            get
            {
                if (_func == null) return _EnableData_INVALID;
                return (_online ? _enableData : _EnableData_INVALID);
            }
            set
            {
                setprop_enableData(value);
            }
        }

        // private helper for magic property
        private void setprop_enableData(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _EnableData_INVALID) return;
            if (newval == _enableData) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_enableData(newval-1);
            _enableData = newval;
        }

        /**
         * <summary>
         *   Returns the Access Point Name (APN) to be used, if needed.
         * <para>
         *   When left blank, the APN suggested by the cell operator will be used.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the Access Point Name (APN) to be used, if needed
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.APN_INVALID</c>.
         * </para>
         */
        public string get_apn()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_apn();
        }

        /**
         * <summary>
         *   Returns the Access Point Name (APN) to be used, if needed.
         * <para>
         *   When left blank, the APN suggested by the cell operator will be used.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string
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
        public int set_apn(string newval)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Apn_INVALID) return YAPI.SUCCESS;
            return _func.set_apn(newval);
        }


        // property with cached value for instant access (configuration)
        public string Apn
        {
            get
            {
                if (_func == null) return _Apn_INVALID;
                return (_online ? _apn : _Apn_INVALID);
            }
            set
            {
                setprop_apn(value);
            }
        }

        // private helper for magic property
        private void setprop_apn(string newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Apn_INVALID) return;
            if (newval == _apn) return;
            _func.set_apn(newval);
            _apn = newval;
        }

        /**
         * <summary>
         *   Returns an opaque string if APN authentication parameters have been configured
         *   in the device, or an empty string otherwise.
         * <para>
         *   To configure these parameters, use <c>set_apnAuth()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to an opaque string if APN authentication parameters have been configured
         *   in the device, or an empty string otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.APNSECRET_INVALID</c>.
         * </para>
         */
        public string get_apnSecret()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_apnSecret();
        }

        /**
         * <summary>
         *   Returns the automated connectivity check interval, in seconds.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the automated connectivity check interval, in seconds
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.PINGINTERVAL_INVALID</c>.
         * </para>
         */
        public int get_pingInterval()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_pingInterval();
            if (res == YAPI.INVALID_INT) res = _PingInterval_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the automated connectivity check interval, in seconds.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the automated connectivity check interval, in seconds
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
        public int set_pingInterval(int newval)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PingInterval_INVALID) return YAPI.SUCCESS;
            return _func.set_pingInterval(newval);
        }


        // property with cached value for instant access (configuration)
        public int PingInterval
        {
            get
            {
                if (_func == null) return _PingInterval_INVALID;
                return (_online ? _pingInterval : _PingInterval_INVALID);
            }
            set
            {
                setprop_pingInterval(value);
            }
        }

        // private helper for magic property
        private void setprop_pingInterval(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _PingInterval_INVALID) return;
            if (newval == _pingInterval) return;
            _func.set_pingInterval(newval);
            _pingInterval = newval;
        }

        /**
         * <summary>
         *   Returns the number of bytes sent so far.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of bytes sent so far
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.DATASENT_INVALID</c>.
         * </para>
         */
        public int get_dataSent()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_dataSent();
            if (res == YAPI.INVALID_INT) res = _DataSent_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the value of the outgoing data counter.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the value of the outgoing data counter
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
        public int set_dataSent(int newval)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _DataSent_INVALID) return YAPI.SUCCESS;
            return _func.set_dataSent(newval);
        }


        /**
         * <summary>
         *   Returns the number of bytes received so far.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of bytes received so far
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YCellular.DATARECEIVED_INVALID</c>.
         * </para>
         */
        public int get_dataReceived()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_dataReceived();
            if (res == YAPI.INVALID_INT) res = _DataReceived_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the value of the incoming data counter.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the value of the incoming data counter
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
        public int set_dataReceived(int newval)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _DataReceived_INVALID) return YAPI.SUCCESS;
            return _func.set_dataReceived(newval);
        }


        /**
         * <summary>
         *   Sends a PUK code to unlock the SIM card after three failed PIN code attempts, and
         *   setup a new PIN into the SIM card.
         * <para>
         *   Only ten consecutive tentatives are permitted:
         *   after that, the SIM card will be blocked permanently without any mean of recovery
         *   to use it again. Note that after calling this method, you have usually to invoke
         *   method <c>set_pin()</c> to tell the YoctoHub which PIN to use in the future.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="puk">
         *   the SIM PUK code
         * </param>
         * <param name="newPin">
         *   new PIN code to configure into the SIM card
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int sendPUK(string puk, string newPin)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.sendPUK(puk, newPin);
        }

        /**
         * <summary>
         *   Configure authentication parameters to connect to the APN.
         * <para>
         *   Both
         *   PAP and CHAP authentication are supported.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="username">
         *   APN username
         * </param>
         * <param name="password">
         *   APN password
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_apnAuth(string username, string password)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.set_apnAuth(username, password);
        }

        /**
         * <summary>
         *   Clear the transmitted data counters.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int clearDataCounters()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.clearDataCounters();
        }

        /**
         * <summary>
         *   Sends an AT command to the GSM module and returns the command output.
         * <para>
         *   The command will only execute when the GSM module is in standard
         *   command state, and should leave it in the exact same state.
         *   Use this function with great care !
         * </para>
         * </summary>
         * <param name="cmd">
         *   the AT command to execute, like for instance: "+CCLK?".
         * </param>
         * <para>
         * </para>
         * <returns>
         *   a string with the result of the commands. Empty lines are
         *   automatically removed from the output.
         * </returns>
         */
        public virtual string _AT(string cmd)
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func._AT(cmd);
        }

        /**
         * <summary>
         *   Returns the list detected cell operators in the neighborhood.
         * <para>
         *   This function will typically take between 30 seconds to 1 minute to
         *   return. Note that any SIM card can usually only connect to specific
         *   operators. All networks returned by this function might therefore
         *   not be available for connection.
         * </para>
         * </summary>
         * <returns>
         *   a list of string (cell operator names).
         * </returns>
         */
        public virtual string[] get_availableOperators()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_availableOperators().ToArray();
        }

        /**
         * <summary>
         *   Returns a list of nearby cellular antennas, as required for quick
         *   geolocation of the device.
         * <para>
         *   The first cell listed is the serving
         *   cell, and the next ones are the neighbor cells reported by the
         *   serving cell.
         * </para>
         * </summary>
         * <returns>
         *   a list of YCellRecords.
         * </returns>
         */
        public virtual YCellRecordProxy[] quickCellSurvey()
        {
            if (_func == null)
            {
                string msg = "No Cellular connected";
                throw new YoctoApiProxyException(msg);
            }
            int i = 0;
            var std_res = _func.quickCellSurvey();
            YCellRecordProxy[] proxy_res = new YCellRecordProxy[std_res.Count];
            foreach (var record in std_res) {
                proxy_res[i++] = new YCellRecordProxy(record);
            }
            return proxy_res;
        }
    }
    //--- (end of generated code: YCellular implementation)
}

