/*********************************************************************
 *
 *  $Id: yocto_temperature_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YTemperatureProxy, the Proxy API for Temperature
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
    //--- (YTemperature class start)
    static public partial class YoctoProxyManager
    {
        public static YTemperatureProxy FindTemperature(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YTemperature func = null;
            YTemperatureProxy res = (YTemperatureProxy)YFunctionProxy.FindSimilarUnknownFunction("YTemperatureProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YTemperatureProxy)YFunctionProxy.FindSimilarKnownFunction("YTemperatureProxy");
                if (res != null) return res;
                func = YTemperature.FirstTemperature();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YTemperatureProxy)func.get_userData();
                    }
                }
            } else {
                func = YTemperature.FindTemperature(name);
                if (func.get_userData() != null) {
                    return (YTemperatureProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YTemperatureProxy(func, name);
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
 *   The YTemperature class allows you to read and configure Yoctopuce temperature
 *   sensors, for instance using a Yocto-Meteo-V2, a Yocto-PT100, a Yocto-Temperature or a Yocto-Thermocouple.
 * <para>
 *   It inherits from YSensor class the core functions to read measurements, to
 *   register callback functions, to access the autonomous datalogger.
 *   This class adds the ability to configure some specific parameters for some
 *   sensors (connection type, temperature mapping table).
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YTemperatureProxy : YSensorProxy
    {
        //--- (end of YTemperature class start)
        //--- (YTemperature definitions)
        public const int _SensorType_INVALID = 0;
        public const int _SensorType_DIGITAL = 1;
        public const int _SensorType_TYPE_K = 2;
        public const int _SensorType_TYPE_E = 3;
        public const int _SensorType_TYPE_J = 4;
        public const int _SensorType_TYPE_N = 5;
        public const int _SensorType_TYPE_R = 6;
        public const int _SensorType_TYPE_S = 7;
        public const int _SensorType_TYPE_T = 8;
        public const int _SensorType_PT100_4WIRES = 9;
        public const int _SensorType_PT100_3WIRES = 10;
        public const int _SensorType_PT100_2WIRES = 11;
        public const int _SensorType_RES_OHM = 12;
        public const int _SensorType_RES_NTC = 13;
        public const int _SensorType_RES_LINEAR = 14;
        public const int _SensorType_RES_INTERNAL = 15;
        public const int _SensorType_IR = 16;
        public const int _SensorType_RES_PT1000 = 17;
        public const double _SignalValue_INVALID = Double.NaN;
        public const string _SignalUnit_INVALID = YAPI.INVALID_STRING;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YTemperature _func;
        // property cache
        protected int _sensorType = _SensorType_INVALID;
        protected string _signalUnit = _SignalUnit_INVALID;
        //--- (end of YTemperature definitions)

        //--- (YTemperature implementation)
        internal YTemperatureProxy(YTemperature hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Temperature " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YTemperature) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YTemperature hwd = YTemperature.FindTemperature(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YTemperature hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Temperature callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YTemperature it = YTemperature.FirstTemperature();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextTemperature();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _signalUnit = _func.get_signalUnit();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _sensorType = _func.get_sensorType()+1;
        }

        /**
         * <summary>
         *   Changes the measuring unit for the measured temperature.
         * <para>
         *   That unit is a string.
         *   If that strings end with the letter F all temperatures values will returned in
         *   Fahrenheit degrees. If that String ends with the letter K all values will be
         *   returned in Kelvin degrees. If that string ends with the letter C all values will be
         *   returned in Celsius degrees.  If the string ends with any other character the
         *   change will be ignored. Remember to call the
         *   <c>saveToFlash()</c> method of the module if the modification must be kept.
         *   WARNING: if a specific calibration is defined for the temperature function, a
         *   unit system change will probably break it.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the measuring unit for the measured temperature
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
                string msg = "No Temperature connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Unit_INVALID) return YAPI.SUCCESS;
            return _func.set_unit(newval);
        }


        /**
         * <summary>
         *   Returns the temperature sensor type.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YTemperature.SENSORTYPE_DIGITAL</c>, <c>YTemperature.SENSORTYPE_TYPE_K</c>,
         *   <c>YTemperature.SENSORTYPE_TYPE_E</c>, <c>YTemperature.SENSORTYPE_TYPE_J</c>,
         *   <c>YTemperature.SENSORTYPE_TYPE_N</c>, <c>YTemperature.SENSORTYPE_TYPE_R</c>,
         *   <c>YTemperature.SENSORTYPE_TYPE_S</c>, <c>YTemperature.SENSORTYPE_TYPE_T</c>,
         *   <c>YTemperature.SENSORTYPE_PT100_4WIRES</c>, <c>YTemperature.SENSORTYPE_PT100_3WIRES</c>,
         *   <c>YTemperature.SENSORTYPE_PT100_2WIRES</c>, <c>YTemperature.SENSORTYPE_RES_OHM</c>,
         *   <c>YTemperature.SENSORTYPE_RES_NTC</c>, <c>YTemperature.SENSORTYPE_RES_LINEAR</c>,
         *   <c>YTemperature.SENSORTYPE_RES_INTERNAL</c>, <c>YTemperature.SENSORTYPE_IR</c> and
         *   <c>YTemperature.SENSORTYPE_RES_PT1000</c> corresponding to the temperature sensor type
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YTemperature.SENSORTYPE_INVALID</c>.
         * </para>
         */
        public int get_sensorType()
        {
            if (_func == null)
            {
                string msg = "No Temperature connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_sensorType()+1;
        }

        /**
         * <summary>
         *   Changes the temperature sensor type.
         * <para>
         *   This function is used
         *   to define the type of thermocouple (K,E...) used with the device.
         *   It has no effect if module is using a digital sensor or a thermistor.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YTemperature.SENSORTYPE_DIGITAL</c>, <c>YTemperature.SENSORTYPE_TYPE_K</c>,
         *   <c>YTemperature.SENSORTYPE_TYPE_E</c>, <c>YTemperature.SENSORTYPE_TYPE_J</c>,
         *   <c>YTemperature.SENSORTYPE_TYPE_N</c>, <c>YTemperature.SENSORTYPE_TYPE_R</c>,
         *   <c>YTemperature.SENSORTYPE_TYPE_S</c>, <c>YTemperature.SENSORTYPE_TYPE_T</c>,
         *   <c>YTemperature.SENSORTYPE_PT100_4WIRES</c>, <c>YTemperature.SENSORTYPE_PT100_3WIRES</c>,
         *   <c>YTemperature.SENSORTYPE_PT100_2WIRES</c>, <c>YTemperature.SENSORTYPE_RES_OHM</c>,
         *   <c>YTemperature.SENSORTYPE_RES_NTC</c>, <c>YTemperature.SENSORTYPE_RES_LINEAR</c>,
         *   <c>YTemperature.SENSORTYPE_RES_INTERNAL</c>, <c>YTemperature.SENSORTYPE_IR</c> and
         *   <c>YTemperature.SENSORTYPE_RES_PT1000</c> corresponding to the temperature sensor type
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
        public int set_sensorType(int newval)
        {
            if (_func == null)
            {
                string msg = "No Temperature connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _SensorType_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_sensorType(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int SensorType
        {
            get
            {
                if (_func == null) return _SensorType_INVALID;
                return (_online ? _sensorType : _SensorType_INVALID);
            }
            set
            {
                setprop_sensorType(value);
            }
        }

        // private helper for magic property
        private void setprop_sensorType(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _SensorType_INVALID) return;
            if (newval == _sensorType) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_sensorType(newval-1);
            _sensorType = newval;
        }

        /**
         * <summary>
         *   Returns the current value of the electrical signal measured by the sensor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current value of the electrical signal measured by the sensor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YTemperature.SIGNALVALUE_INVALID</c>.
         * </para>
         */
        public double get_signalValue()
        {
            if (_func == null)
            {
                string msg = "No Temperature connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_signalValue();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        // property with cached value for instant access (constant value)
        public string SignalUnit
        {
            get
            {
                if (_func == null) return _SignalUnit_INVALID;
                return (_online ? _signalUnit : _SignalUnit_INVALID);
            }
        }

        /**
         * <summary>
         *   Returns the measuring unit of the electrical signal used by the sensor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the measuring unit of the electrical signal used by the sensor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YTemperature.SIGNALUNIT_INVALID</c>.
         * </para>
         */
        public string get_signalUnit()
        {
            if (_func == null)
            {
                string msg = "No Temperature connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_signalUnit();
        }

        /**
         * <summary>
         *   Configures NTC thermistor parameters in order to properly compute the temperature from
         *   the measured resistance.
         * <para>
         *   For increased precision, you can enter a complete mapping
         *   table using set_thermistorResponseTable. This function can only be used with a
         *   temperature sensor based on thermistors.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="res25">
         *   thermistor resistance at 25 degrees Celsius
         * </param>
         * <param name="beta">
         *   Beta value
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_ntcParameters(double res25, double beta)
        {
            if (_func == null)
            {
                string msg = "No Temperature connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.set_ntcParameters(res25, beta);
        }

        /**
         * <summary>
         *   Records a thermistor response table, in order to interpolate the temperature from
         *   the measured resistance.
         * <para>
         *   This function can only be used with a temperature
         *   sensor based on thermistors.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tempValues">
         *   array of floating point numbers, corresponding to all
         *   temperatures (in degrees Celsius) for which the resistance of the
         *   thermistor is specified.
         * </param>
         * <param name="resValues">
         *   array of floating point numbers, corresponding to the resistance
         *   values (in Ohms) for each of the temperature included in the first
         *   argument, index by index.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_thermistorResponseTable(double[] tempValues, double[] resValues)
        {
            if (_func == null)
            {
                string msg = "No Temperature connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.set_thermistorResponseTable(new List<double>(tempValues), new List<double>(resValues));
        }

        /**
         * <summary>
         *   Retrieves the thermistor response table previously configured using the
         *   <c>set_thermistorResponseTable</c> function.
         * <para>
         *   This function can only be used with a
         *   temperature sensor based on thermistors.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="tempValues">
         *   array of floating point numbers, that is filled by the function
         *   with all temperatures (in degrees Celsius) for which the resistance
         *   of the thermistor is specified.
         * </param>
         * <param name="resValues">
         *   array of floating point numbers, that is filled by the function
         *   with the value (in Ohms) for each of the temperature included in the
         *   first argument, index by index.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int loadThermistorResponseTable(double[] tempValues, double[] resValues)
        {
            if (_func == null)
            {
                string msg = "No Temperature connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.loadThermistorResponseTable(new List<double>(tempValues), new List<double>(resValues));
        }
    }
    //--- (end of YTemperature implementation)
}

