/*********************************************************************
 *
 *  $Id: svn_id $
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
 *   The <c>YPower</c> class allows you to read and configure Yoctopuce electrical power sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 *   This class adds the ability to access the energy counter and the power factor.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YPowerProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a electrical power sensor for a given identifier.
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
         *   This function does not require that the electrical power sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YPower.isOnline()</c> to test if the electrical power sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a electrical power sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the electrical power sensor, for instance
         *   <c>YWATTMK1.power</c>.
         * </param>
         * <returns>
         *   a <c>YPower</c> object allowing you to drive the electrical power sensor.
         * </returns>
         */
        public static YPowerProxy FindPower(string func)
        {
            return YoctoProxyManager.FindPower(func);
        }
        //--- (end of YPower class start)
        //--- (YPower definitions)
        public const double _PowerFactor_INVALID = Double.NaN;
        public const double _CosPhi_INVALID = Double.NaN;
        public const double _Meter_INVALID = Double.NaN;
        public const double _DeliveredEnergyMeter_INVALID = Double.NaN;
        public const double _ReceivedEnergyMeter_INVALID = Double.NaN;
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

        /**
         * <summary>
         *   Enumerates all functions of type Power available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YPower.FindPower</c> to obtain an object that can control the
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
         *   Returns the power factor (PF), i.e.
         * <para>
         *   ratio between the active power consumed (in W)
         *   and the apparent power provided (VA).
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the power factor (PF), i.e
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPower.POWERFACTOR_INVALID</c>.
         * </para>
         */
        public double get_powerFactor()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Power connected");
            }
            res = _func.get_powerFactor();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the Displacement Power factor (DPF), i.e.
         * <para>
         *   cosine of the phase shift between
         *   the voltage and current fundamentals.
         *   On the Yocto-Watt (V1), the value returned by this method correponds to the
         *   power factor as this device is cannot estimate the true DPF.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the Displacement Power factor (DPF), i.e
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPower.COSPHI_INVALID</c>.
         * </para>
         */
        public double get_cosPhi()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Power connected");
            }
            res = _func.get_cosPhi();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the energy counter, maintained by the wattmeter by integrating the
         *   power consumption over time.
         * <para>
         *   This is the sum of forward and backwad energy transfers,
         *   if you are insterested in only one direction, use  get_receivedEnergyMeter() or
         *   get_deliveredEnergyMeter(). Note that this counter is reset at each start of the device.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the energy counter, maintained by the wattmeter by integrating the
         *   power consumption over time
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPower.METER_INVALID</c>.
         * </para>
         */
        public double get_meter()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Power connected");
            }
            res = _func.get_meter();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the energy counter, maintained by the wattmeter by integrating the power consumption over time,
         *   but only when positive.
         * <para>
         *   Note that this counter is reset at each start of the device.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the energy counter, maintained by the wattmeter by
         *   integrating the power consumption over time,
         *   but only when positive
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPower.DELIVEREDENERGYMETER_INVALID</c>.
         * </para>
         */
        public double get_deliveredEnergyMeter()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Power connected");
            }
            res = _func.get_deliveredEnergyMeter();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the energy counter, maintained by the wattmeter by integrating the power consumption over time,
         *   but only when negative.
         * <para>
         *   Note that this counter is reset at each start of the device.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the energy counter, maintained by the wattmeter by
         *   integrating the power consumption over time,
         *   but only when negative
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YPower.RECEIVEDENERGYMETER_INVALID</c>.
         * </para>
         */
        public double get_receivedEnergyMeter()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Power connected");
            }
            res = _func.get_receivedEnergyMeter();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
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
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Power connected");
            }
            res = _func.get_meterTimer();
            if (res == YAPI.INVALID_INT) {
                res = _MeterTimer_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Resets the energy counters.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int reset()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Power connected");
            }
            return _func.reset();
        }
    }
    //--- (end of YPower implementation)
}

