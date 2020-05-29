/*********************************************************************
 *
 *  $Id: yocto_module_proxy.cs 40752 2020-05-28 13:32:54Z mvuilleu $
 *
 *  Implements YModuleProxy, the Proxy API for Module
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
using YoctoLib;

namespace YoctoProxyAPI
{
  //--- (generated code: YModule class start)
    static public partial class YoctoProxyManager
    {
        public static YModuleProxy FindModule(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YModule func = null;
            YModuleProxy res = (YModuleProxy)YFunctionProxy.FindSimilarUnknownFunction("YModuleProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YModuleProxy)YFunctionProxy.FindSimilarKnownFunction("YModuleProxy");
                if (res != null) return res;
                func = YModule.FirstModule();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YModuleProxy)func.get_userData();
                    }
                }
            } else {
                // allow to get module from the name of any function
                int p = name.IndexOf(".");
                if (p > 0) name = name.Substring(0, p) + ".module";
                func = YModule.FindModule(name);
                if (func.get_userData() != null) {
                    return (YModuleProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YModuleProxy(func, name);
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
 *   The <c>YModule</c> class can be used with all Yoctopuce USB devices.
 * <para>
 *   It can be used to control the module global parameters, and
 *   to enumerate the functions provided by each module.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YModuleProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Allows you to find a module from its serial number or from its logical name.
         * <para>
         * </para>
         * <para>
         *   This function does not require that the module is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YModule.isOnline()</c> to test if the module is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a module by logical name, no error is notified: the first instance
         *   found is returned. The search is performed first by hardware name,
         *   then by logical name.
         * </para>
         * <para>
         * </para>
         * <para>
         *   If a call to this object's is_online() method returns FALSE although
         *   you are certain that the device is plugged, make sure that you did
         *   call registerHub() at application initialization time.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="func">
         *   a string containing either the serial number or
         *   the logical name of the desired module
         * </param>
         * <returns>
         *   a <c>YModule</c> object allowing you to drive the module
         *   or get additional information on the module.
         * </returns>
         */
        public static YModuleProxy FindModule(string func)
        {
            return YoctoProxyManager.FindModule(func);
        }
        //--- (end of generated code: YModule class start)
    //--- (generated code: YModule definitions)
        public const string _ProductName_INVALID = YAPI.INVALID_STRING;
        public const string _SerialNumber_INVALID = YAPI.INVALID_STRING;
        public const int _ProductId_INVALID = -1;
        public const int _ProductRelease_INVALID = -1;
        public const string _FirmwareRelease_INVALID = YAPI.INVALID_STRING;
        public const int _PersistentSettings_INVALID = 0;
        public const int _PersistentSettings_LOADED = 1;
        public const int _PersistentSettings_SAVED = 2;
        public const int _PersistentSettings_MODIFIED = 3;
        public const int _Luminosity_INVALID = -1;
        public const int _Beacon_INVALID = 0;
        public const int _Beacon_OFF = 1;
        public const int _Beacon_ON = 2;
        public const long _UpTime_INVALID = YAPI.INVALID_LONG;
        public const int _UsbCurrent_INVALID = -1;
        public const int _RebootCountdown_INVALID = YAPI.INVALID_INT;
        public const int _UserVar_INVALID = YAPI.INVALID_INT;

        // reference to real YoctoAPI object
        protected new YModule _func;
        // property cache
        protected string _productName = _ProductName_INVALID;
        protected int _productId = _ProductId_INVALID;
        protected int _productRelease = _ProductRelease_INVALID;
        protected int _luminosity = _Luminosity_INVALID;
        //--- (end of generated code: YModule definitions)
        protected string _firmwareRelease = _FirmwareRelease_INVALID;
        protected int _beacon = _Beacon_INVALID;
		
    	private void beaconChangeCallback(YModule module, int beacon)
    	{
            InternalStuff.log("beacon callback: " + beacon.ToString());
            _beacon = beacon+1;
    	}

        // property with cached value for instant access (configuration)
        public int Beacon
    	{
      		get
      		{
                if (_func == null)
                {
                    return _Beacon_INVALID;
                }
                if (_online)
                {
                    return _beacon;
                }
        		return _Beacon_INVALID;
      		}
      		set
      		{
                if (_func == null)
                {
                    return;
                }
                if (!(_online))
                {
                    return;
                }
                if (value == _Beacon_INVALID)
                {
                    return;
                }
                if (value == _beacon)
                {
                    return;
                }
                // our enums start at 0 instead of the 'usual' -1 for invalid
                _func.set_beacon(value - 1);
                _beacon = value;
      		}
    	}


    //--- (generated code: YModule implementation)
        internal YModuleProxy(YModule hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Module " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YModule) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YModule hwd = YModule.FindModule(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YModule hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Module callback");
            _func.registerBeaconCallback(beaconChangeCallback);
        }

        /**
         * <summary>
         *   Enumère toutes les fonctions de type Module.
         * <para>
         *   Returns an array of strings representing hardware identifiers for all Module functions presently connected.
         * </para>
         * </summary>
         */
        public static new string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YModule it = YModule.FirstModule();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextModule();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _beacon = _func.get_beacon()+1;
            _productName = _func.get_productName();
            _productId = _func.get_productId();
            _productRelease = _func.get_productRelease();
            _firmwareRelease = _func.get_firmwareRelease();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _luminosity = _func.get_luminosity();
        }

        /**
         * <summary>
         *   Returns the commercial name of the module, as set by the factory.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the commercial name of the module, as set by the factory
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Productname_INVALID</c>.
         * </para>
         */
        public string get_productName()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_productName();
        }

        // property with cached value for instant access (constant value)
        /// <value>Commercial name of the module, as set by the factory.</value>
        public string ProductName
        {
            get
            {
                if (_func == null) {
                    return _ProductName_INVALID;
                }
                if (_online) {
                    return _productName;
                }
                return _ProductName_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the USB device identifier of the module.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the USB device identifier of the module
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Productid_INVALID</c>.
         * </para>
         */
        public int get_productId()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            res = _func.get_productId();
            if (res == YAPI.INVALID_INT) {
                res = _ProductId_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>USB device identifier of the module.</value>
        public int ProductId
        {
            get
            {
                if (_func == null) {
                    return _ProductId_INVALID;
                }
                if (_online) {
                    return _productId;
                }
                return _ProductId_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the release number of the module hardware, preprogrammed at the factory.
         * <para>
         *   The original hardware release returns value 1, revision B returns value 2, etc.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the release number of the module hardware, preprogrammed at the factory
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Productrelease_INVALID</c>.
         * </para>
         */
        public int get_productRelease()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            res = _func.get_productRelease();
            if (res == YAPI.INVALID_INT) {
                res = _ProductRelease_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Release number of the module hardware, preprogrammed at the factory.</value>
        public int ProductRelease
        {
            get
            {
                if (_func == null) {
                    return _ProductRelease_INVALID;
                }
                if (_online) {
                    return _productRelease;
                }
                return _ProductRelease_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the version of the firmware embedded in the module.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the version of the firmware embedded in the module
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Firmwarerelease_INVALID</c>.
         * </para>
         */
        public string get_firmwareRelease()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_firmwareRelease();
        }

        // property with cached value for instant access (constant value)
        /// <value>Version of the firmware embedded in the module.</value>
        public string FirmwareRelease
        {
            get
            {
                if (_func == null) {
                    return _FirmwareRelease_INVALID;
                }
                if (_online) {
                    return _firmwareRelease;
                }
                return _FirmwareRelease_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the current state of persistent module settings.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>module._Persistentsettings_LOADED</c>, <c>module._Persistentsettings_SAVED</c> and
         *   <c>module._Persistentsettings_MODIFIED</c> corresponding to the current state of persistent module settings
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Persistentsettings_INVALID</c>.
         * </para>
         */
        public int get_persistentSettings()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_persistentSettings()+1;
        }

        /**
         * <summary>
         *   Returns the luminosity of the  module informative LEDs (from 0 to 100).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the luminosity of the  module informative LEDs (from 0 to 100)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Luminosity_INVALID</c>.
         * </para>
         */
        public int get_luminosity()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            res = _func.get_luminosity();
            if (res == YAPI.INVALID_INT) {
                res = _Luminosity_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the luminosity of the module informative leds.
         * <para>
         *   The parameter is a
         *   value between 0 and 100.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the luminosity of the module informative leds
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
        public int set_luminosity(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            if (newval == _Luminosity_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_luminosity(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Luminosity of the  module informative LEDs (from 0 to 100).</value>
        public int Luminosity
        {
            get
            {
                if (_func == null) {
                    return _Luminosity_INVALID;
                }
                if (_online) {
                    return _luminosity;
                }
                return _Luminosity_INVALID;
            }
            set
            {
                setprop_luminosity(value);
            }
        }

        // private helper for magic property
        private void setprop_luminosity(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Luminosity_INVALID) {
                return;
            }
            if (newval == _luminosity) {
                return;
            }
            _func.set_luminosity(newval);
            _luminosity = newval;
        }

        /**
         * <summary>
         *   Returns the state of the localization beacon.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>module._Beacon_OFF</c> or <c>module._Beacon_ON</c>, according to the state of the localization beacon
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Beacon_INVALID</c>.
         * </para>
         */
        public int get_beacon()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_beacon()+1;
        }

        /**
         * <summary>
         *   Turns on or off the module localization beacon.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>module._Beacon_OFF</c> or <c>module._Beacon_ON</c>
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
        public int set_beacon(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            if (newval == _Beacon_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_beacon(newval-1);
        }

        /**
         * <summary>
         *   Returns the number of milliseconds spent since the module was powered on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of milliseconds spent since the module was powered on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Uptime_INVALID</c>.
         * </para>
         */
        public long get_upTime()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_upTime();
        }

        /**
         * <summary>
         *   Returns the current consumed by the module on the USB bus, in milli-amps.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current consumed by the module on the USB bus, in milli-amps
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Usbcurrent_INVALID</c>.
         * </para>
         */
        public int get_usbCurrent()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            res = _func.get_usbCurrent();
            if (res == YAPI.INVALID_INT) {
                res = _UsbCurrent_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the remaining number of seconds before the module restarts, or zero when no
         *   reboot has been scheduled.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the remaining number of seconds before the module restarts, or zero when no
         *   reboot has been scheduled
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Rebootcountdown_INVALID</c>.
         * </para>
         */
        public int get_rebootCountdown()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_rebootCountdown();
        }

        /**
         * <summary>
         *   Returns the value previously stored in this attribute.
         * <para>
         *   On startup and after a device reboot, the value is always reset to zero.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the value previously stored in this attribute
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>module._Uservar_INVALID</c>.
         * </para>
         */
        public int get_userVar()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_userVar();
        }

        /**
         * <summary>
         *   Stores a 32 bit value in the device RAM.
         * <para>
         *   This attribute is at programmer disposal,
         *   should he need to store a state variable.
         *   On startup and after a device reboot, the value is always reset to zero.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer
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
        public int set_userVar(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            if (newval == _UserVar_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_userVar(newval);
        }

        /**
         * <summary>
         *   Saves current settings in the nonvolatile memory of the module.
         * <para>
         *   Warning: the number of allowed save operations during a module life is
         *   limited (about 100000 cycles). Do not call this function within a loop.
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int saveToFlash()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.saveToFlash();
        }

        /**
         * <summary>
         *   Reloads the settings stored in the nonvolatile memory, as
         *   when the module is powered on.
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
        public virtual int revertFromFlash()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.revertFromFlash();
        }

        /**
         * <summary>
         *   Schedules a simple module reboot after the given number of seconds.
         * <para>
         * </para>
         * </summary>
         * <param name="secBeforeReboot">
         *   number of seconds before rebooting
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int reboot(int secBeforeReboot)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.reboot(secBeforeReboot);
        }

        /**
         * <summary>
         *   Schedules a module reboot into special firmware update mode.
         * <para>
         * </para>
         * </summary>
         * <param name="secBeforeReboot">
         *   number of seconds before rebooting
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int triggerFirmwareUpdate(int secBeforeReboot)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.triggerFirmwareUpdate(secBeforeReboot);
        }

        /**
         * <summary>
         *   Triggers a configuration change callback, to check if they are supported or not.
         * <para>
         * </para>
         * </summary>
         */
        public virtual int triggerConfigChangeCallback()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.triggerConfigChangeCallback();
        }

        /**
         * <summary>
         *   Tests whether the byn file is valid for this module.
         * <para>
         *   This method is useful to test if the module needs to be updated.
         *   It is possible to pass a directory as argument instead of a file. In this case, this method returns
         *   the path of the most recent
         *   appropriate <c>.byn</c> file. If the parameter <c>onlynew</c> is true, the function discards
         *   firmwares that are older or
         *   equal to the installed firmware.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="path">
         *   the path of a byn file or a directory that contains byn files
         * </param>
         * <param name="onlynew">
         *   returns only files that are strictly newer
         * </param>
         * <para>
         * </para>
         * <returns>
         *   the path of the byn file to use or a empty string if no byn files matches the requirement
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a string that start with "error:".
         * </para>
         */
        public virtual string checkFirmware(string path, bool onlynew)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.checkFirmware(path, onlynew);
        }

        /**
         * <summary>
         *   Prepares a firmware update of the module.
         * <para>
         *   This method returns a <c>YFirmwareUpdate</c> object which
         *   handles the firmware update process.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="path">
         *   the path of the <c>.byn</c> file to use.
         * </param>
         * <param name="force">
         *   true to force the firmware update even if some prerequisites appear not to be met
         * </param>
         * <returns>
         *   a <c>YFirmwareUpdate</c> object or NULL on error.
         * </returns>
         */
        public virtual YFirmwareUpdateProxy updateFirmwareEx(string path, bool force)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return new YFirmwareUpdateProxy(_func.updateFirmwareEx(path, force));
        }

        /**
         * <summary>
         *   Prepares a firmware update of the module.
         * <para>
         *   This method returns a <c>YFirmwareUpdate</c> object which
         *   handles the firmware update process.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="path">
         *   the path of the <c>.byn</c> file to use.
         * </param>
         * <returns>
         *   a <c>YFirmwareUpdate</c> object or NULL on error.
         * </returns>
         */
        public virtual YFirmwareUpdateProxy updateFirmware(string path)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return new YFirmwareUpdateProxy(_func.updateFirmware(path));
        }

        /**
         * <summary>
         *   Returns all the settings and uploaded files of the module.
         * <para>
         *   Useful to backup all the
         *   logical names, calibrations parameters, and uploaded files of a device.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a binary buffer with all the settings.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an binary object of size 0.
         * </para>
         */
        public virtual byte[] get_allSettings()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_allSettings();
        }

        /**
         * <summary>
         *   Restores all the settings and uploaded files to the module.
         * <para>
         *   This method is useful to restore all the logical names and calibrations parameters,
         *   uploaded files etc. of a device from a backup.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modifications must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="settings">
         *   a binary buffer with all the settings.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_allSettingsAndFiles(byte[] settings)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.set_allSettingsAndFiles(settings);
        }

        /**
         * <summary>
         *   Tests if the device includes a specific function.
         * <para>
         *   This method takes a function identifier
         *   and returns a boolean.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="funcId">
         *   the requested function identifier
         * </param>
         * <returns>
         *   true if the device has the function identifier
         * </returns>
         */
        public virtual bool hasFunction(string funcId)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.hasFunction(funcId);
        }

        /**
         * <summary>
         *   Retrieve all hardware identifier that match the type passed in argument.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="funType">
         *   The type of function (Relay, LightSensor, Voltage,...)
         * </param>
         * <returns>
         *   an array of strings.
         * </returns>
         */
        public virtual string[] get_functionIds(string funType)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_functionIds(funType).ToArray();
        }

        /**
         * <summary>
         *   Restores all the settings of the device.
         * <para>
         *   Useful to restore all the logical names and calibrations parameters
         *   of a module from a backup.Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modifications must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="settings">
         *   a binary buffer with all the settings.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_allSettings(byte[] settings)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.set_allSettings(settings);
        }

        /**
         * <summary>
         *   Returns the unique hardware identifier of the module.
         * <para>
         *   The unique hardware identifier is made of the device serial
         *   number followed by string ".module".
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string that uniquely identifies the module
         * </returns>
         */
        public override string get_hardwareId()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_hardwareId();
        }

        /**
         * <summary>
         *   Downloads the specified built-in file and returns a binary buffer with its content.
         * <para>
         * </para>
         * </summary>
         * <param name="pathname">
         *   name of the new file to load
         * </param>
         * <returns>
         *   a binary buffer with the file content
         * </returns>
         * <para>
         *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
         * </para>
         */
        public virtual byte[] download(string pathname)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.download(pathname);
        }

        /**
         * <summary>
         *   Returns the icon of the module.
         * <para>
         *   The icon is a PNG image and does not
         *   exceeds 1536 bytes.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a binary buffer with module icon, in png format.
         *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
         * </returns>
         */
        public virtual byte[] get_icon2d()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_icon2d();
        }

        /**
         * <summary>
         *   Returns a string with last logs of the module.
         * <para>
         *   This method return only
         *   logs that are still in the module.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string with last logs of the module.
         *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
         * </returns>
         */
        public virtual string get_lastLogs()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_lastLogs();
        }

        /**
         * <summary>
         *   Adds a text message to the device logs.
         * <para>
         *   This function is useful in
         *   particular to trace the execution of HTTP callbacks. If a newline
         *   is desired after the message, it must be included in the string.
         * </para>
         * </summary>
         * <param name="text">
         *   the string to append to the logs.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int log(string text)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.log(text);
        }

        /**
         * <summary>
         *   Returns a list of all the modules that are plugged into the current module.
         * <para>
         *   This method only makes sense when called for a YoctoHub/VirtualHub.
         *   Otherwise, an empty array will be returned.
         * </para>
         * </summary>
         * <returns>
         *   an array of strings containing the sub modules.
         * </returns>
         */
        public virtual string[] get_subDevices()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_subDevices().ToArray();
        }

        /**
         * <summary>
         *   Returns the serial number of the YoctoHub on which this module is connected.
         * <para>
         *   If the module is connected by USB, or if the module is the root YoctoHub, an
         *   empty string is returned.
         * </para>
         * </summary>
         * <returns>
         *   a string with the serial number of the YoctoHub or an empty string
         * </returns>
         */
        public virtual string get_parentHub()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_parentHub();
        }

        /**
         * <summary>
         *   Returns the URL used to access the module.
         * <para>
         *   If the module is connected by USB, the
         *   string 'usb' is returned.
         * </para>
         * </summary>
         * <returns>
         *   a string with the URL of the module.
         * </returns>
         */
        public virtual string get_url()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Module connected");
            }
            return _func.get_url();
        }
    }
    //--- (end of generated code: YModule implementation)
}

