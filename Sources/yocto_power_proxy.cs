/*********************************************************************
 *
 *  $Id: yocto_power_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YPowerProxy, the Proxy API for Power
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
    //--- (YPower class start)
    static public partial class YoctoProxyManager
    {
        public static YPowerProxy FindPower(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YPower func = null;
            YPowerProxy res = (YPowerProxy)YFunctionProxy.FindSimilarUnknownFunction("YPowerProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YPowerProxy)YFunctionProxy.FindSimilarKnownFunction("YPowerProxy");
                if (res != null) return res;
                func = YPower.FirstPower();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YPowerProxy)func.get_userData();
                    }
                }
            } else {
                func = YPower.FindPower(name);
                if (func.get_userData() != null) {
                    return (YPowerProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YPowerProxy(func, name);
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
 *   The YPower class allows you to read and configure Yoctopuce power
 *   sensors, for instance using a Yocto-Watt.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements,
 *   to register callback functions, to access the autonomous datalogger.
 *   This class adds the ability to access the energy counter and the power factor.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YPowerProxy : YSensorProxy
    {
        //--- (end of YPower class start)
        //--- (YPower definitions)
        public const int _ShutdownCountdown_INVALID = -1;
        public const double _CosPhi_INVALID = Double.NaN;
        public const double _Meter_INVALID = Double.NaN;
        public const int _MeterTimer_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YPower _func;
        // property cache
        //--- (end of YPower definitions)

        //--- (YPower implementation)
        internal YPowerProxy(YPower hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Power " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YPower) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YPower hwd = YPower.FindPower(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YPower hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Power callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YPower it = YPower.FirstPower();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextPower();
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
         *   Returns the power factor (the ratio between the real power consumed,
         *   measured in W, and the apparent power provided, measured in VA).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the power factor (the ratio between the real power consumed,
         *   measured in W, and the apparent power provided, measured in VA)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPower.COSPHI_INVALID</c>.
         * </para>
         */
        public double get_cosPhi()
        {
            if (_func == null)
            {
                string msg = "No Power connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_cosPhi();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the energy counter, maintained by the wattmeter by integrating the power consumption over time.
         * <para>
         *   Note that this counter is reset at each start of the device.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the energy counter, maintained by the wattmeter by
         *   integrating the power consumption over time
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPower.METER_INVALID</c>.
         * </para>
         */
        public double get_meter()
        {
            if (_func == null)
            {
                string msg = "No Power connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_meter();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the elapsed time since last energy counter reset, in seconds.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the elapsed time since last energy counter reset, in seconds
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPower.METERTIMER_INVALID</c>.
         * </para>
         */
        public int get_meterTimer()
        {
            if (_func == null)
            {
                string msg = "No Power connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_meterTimer();
            if (res == YAPI.INVALID_INT) res = _MeterTimer_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Resets the energy counter.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int reset()
        {
            if (_func == null)
            {
                string msg = "No Power connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.reset();
        }
    }
    //--- (end of YPower implementation)
}

