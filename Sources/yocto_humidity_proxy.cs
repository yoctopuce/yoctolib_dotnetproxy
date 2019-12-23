/*********************************************************************
 *
 *  $Id: yocto_humidity_proxy.cs 38913 2019-12-20 18:59:49Z mvuilleu $
 *
 *  Implements YHumidityProxy, the Proxy API for Humidity
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
    //--- (YHumidity class start)
    static public partial class YoctoProxyManager
    {
        public static YHumidityProxy FindHumidity(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YHumidity func = null;
            YHumidityProxy res = (YHumidityProxy)YFunctionProxy.FindSimilarUnknownFunction("YHumidityProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YHumidityProxy)YFunctionProxy.FindSimilarKnownFunction("YHumidityProxy");
                if (res != null) return res;
                func = YHumidity.FirstHumidity();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YHumidityProxy)func.get_userData();
                    }
                }
            } else {
                func = YHumidity.FindHumidity(name);
                if (func.get_userData() != null) {
                    return (YHumidityProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YHumidityProxy(func, name);
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
 *   The <c>YHumidity</c> class allows you to read and configure Yoctopuce humidity sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YHumidityProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a humidity sensor for a given identifier.
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
         *   This function does not require that the humidity sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YHumidity.isOnline()</c> to test if the humidity sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a humidity sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the humidity sensor, for instance
         *   <c>YCO2MK02.humidity</c>.
         * </param>
         * <returns>
         *   a <c>YHumidity</c> object allowing you to drive the humidity sensor.
         * </returns>
         */
        public static YHumidityProxy FindHumidity(string func)
        {
            return YoctoProxyManager.FindHumidity(func);
        }
        //--- (end of YHumidity class start)
        //--- (YHumidity definitions)
        public const double _RelHum_INVALID = Double.NaN;
        public const double _AbsHum_INVALID = Double.NaN;

        // reference to real YoctoAPI object
        protected new YHumidity _func;
        // property cache
        //--- (end of YHumidity definitions)

        //--- (YHumidity implementation)
        internal YHumidityProxy(YHumidity hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Humidity " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YHumidity) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YHumidity hwd = YHumidity.FindHumidity(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YHumidity hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Humidity callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Humidity available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YHumidity.FindHumidity</c> to obtain an object that can control the
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
            YHumidity it = YHumidity.FirstHumidity();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextHumidity();
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
         *   Changes the primary unit for measuring humidity.
         * <para>
         *   That unit is a string.
         *   If that strings starts with the letter 'g', the primary measured value is the absolute
         *   humidity, in g/m3. Otherwise, the primary measured value will be the relative humidity
         *   (RH), in per cents.
         * </para>
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification
         *   must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the primary unit for measuring humidity
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
        public int set_unit(string newval)
        {
            if (_func == null)
            {
                string msg = "No Humidity connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Unit_INVALID) return YAPI.SUCCESS;
            return _func.set_unit(newval);
        }


        /**
         * <summary>
         *   Returns the current relative humidity, in per cents.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current relative humidity, in per cents
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>humidity._Relhum_INVALID</c>.
         * </para>
         */
        public double get_relHum()
        {
            if (_func == null)
            {
                string msg = "No Humidity connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_relHum();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the current absolute humidity, in grams per cubic meter of air.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current absolute humidity, in grams per cubic meter of air
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>humidity._Abshum_INVALID</c>.
         * </para>
         */
        public double get_absHum()
        {
            if (_func == null)
            {
                string msg = "No Humidity connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_absHum();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }
    }
    //--- (end of YHumidity implementation)
}

