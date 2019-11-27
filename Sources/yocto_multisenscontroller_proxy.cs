/*********************************************************************
 *
 *  $Id: yocto_multisenscontroller_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YMultiSensControllerProxy, the Proxy API for MultiSensController
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
    //--- (YMultiSensController class start)
    static public partial class YoctoProxyManager
    {
        public static YMultiSensControllerProxy FindMultiSensController(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YMultiSensController func = null;
            YMultiSensControllerProxy res = (YMultiSensControllerProxy)YFunctionProxy.FindSimilarUnknownFunction("YMultiSensControllerProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YMultiSensControllerProxy)YFunctionProxy.FindSimilarKnownFunction("YMultiSensControllerProxy");
                if (res != null) return res;
                func = YMultiSensController.FirstMultiSensController();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YMultiSensControllerProxy)func.get_userData();
                    }
                }
            } else {
                func = YMultiSensController.FindMultiSensController(name);
                if (func.get_userData() != null) {
                    return (YMultiSensControllerProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YMultiSensControllerProxy(func, name);
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
 *   The YMultiSensController class allows you to setup a customized
 *   sensor chain on devices featuring that functionality, for instance using a Yocto-Temperature-IR.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YMultiSensControllerProxy : YFunctionProxy
    {
        //--- (end of YMultiSensController class start)
        //--- (YMultiSensController definitions)
        public const int _NSensors_INVALID = -1;
        public const int _MaxSensors_INVALID = -1;
        public const int _MaintenanceMode_INVALID = 0;
        public const int _MaintenanceMode_FALSE = 1;
        public const int _MaintenanceMode_TRUE = 2;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YMultiSensController _func;
        // property cache
        protected int _nSensors = _NSensors_INVALID;
        //--- (end of YMultiSensController definitions)

        //--- (YMultiSensController implementation)
        internal YMultiSensControllerProxy(YMultiSensController hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("MultiSensController " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YMultiSensController) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YMultiSensController hwd = YMultiSensController.FindMultiSensController(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YMultiSensController hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering MultiSensController callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YMultiSensController it = YMultiSensController.FirstMultiSensController();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextMultiSensController();
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
            _nSensors = _func.get_nSensors();
        }

        /**
         * <summary>
         *   Returns the number of sensors to poll.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of sensors to poll
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiSensController.NSENSORS_INVALID</c>.
         * </para>
         */
        public int get_nSensors()
        {
            if (_func == null)
            {
                string msg = "No MultiSensController connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_nSensors();
            if (res == YAPI.INVALID_INT) res = _NSensors_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the number of sensors to poll.
         * <para>
         *   Remember to call the
         *   <c>saveToFlash()</c> method of the module if the
         *   modification must be kept. It is recommended to restart the
         *   device with  <c>module->reboot()</c> after modifying
         *   (and saving) this settings
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the number of sensors to poll
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
        public int set_nSensors(int newval)
        {
            if (_func == null)
            {
                string msg = "No MultiSensController connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _NSensors_INVALID) return YAPI.SUCCESS;
            return _func.set_nSensors(newval);
        }


        // property with cached value for instant access (configuration)
        public int NSensors
        {
            get
            {
                if (_func == null) return _NSensors_INVALID;
                return (_online ? _nSensors : _NSensors_INVALID);
            }
            set
            {
                setprop_nSensors(value);
            }
        }

        // private helper for magic property
        private void setprop_nSensors(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _NSensors_INVALID) return;
            if (newval == _nSensors) return;
            _func.set_nSensors(newval);
            _nSensors = newval;
        }

        /**
         * <summary>
         *   Returns the maximum configurable sensor count allowed on this device.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum configurable sensor count allowed on this device
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiSensController.MAXSENSORS_INVALID</c>.
         * </para>
         */
        public int get_maxSensors()
        {
            if (_func == null)
            {
                string msg = "No MultiSensController connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_maxSensors();
            if (res == YAPI.INVALID_INT) res = _MaxSensors_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns true when the device is in maintenance mode.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YMultiSensController.MAINTENANCEMODE_FALSE</c> or <c>YMultiSensController.MAINTENANCEMODE_TRUE</c>,
         *   according to true when the device is in maintenance mode
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiSensController.MAINTENANCEMODE_INVALID</c>.
         * </para>
         */
        public int get_maintenanceMode()
        {
            if (_func == null)
            {
                string msg = "No MultiSensController connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_maintenanceMode()+1;
        }

        /**
         * <summary>
         *   Changes the device mode to enable maintenance and to stop sensor polling.
         * <para>
         *   This way, the device does not automatically restart when it cannot
         *   communicate with one of the sensors.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YMultiSensController.MAINTENANCEMODE_FALSE</c> or <c>YMultiSensController.MAINTENANCEMODE_TRUE</c>,
         *   according to the device mode to enable maintenance and to stop sensor polling
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
        public int set_maintenanceMode(int newval)
        {
            if (_func == null)
            {
                string msg = "No MultiSensController connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _MaintenanceMode_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_maintenanceMode(newval-1);
        }


        /**
         * <summary>
         *   Configures the I2C address of the only sensor connected to the device.
         * <para>
         *   It is recommended to put the the device in maintenance mode before
         *   changing sensor addresses.  This method is only intended to work with a single
         *   sensor connected to the device, if several sensors are connected, the result
         *   is unpredictable.
         *   Note that the device is probably expecting to find a string of sensors with specific
         *   addresses. Check the device documentation to find out which addresses should be used.
         * </para>
         * </summary>
         * <param name="addr">
         *   new address of the connected sensor
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int setupAddress(int addr)
        {
            if (_func == null)
            {
                string msg = "No MultiSensController connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.setupAddress(addr);
        }
    }
    //--- (end of YMultiSensController implementation)
}

