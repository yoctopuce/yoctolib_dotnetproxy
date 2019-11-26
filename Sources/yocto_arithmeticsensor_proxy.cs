/*********************************************************************
 *
 *  $Id: yocto_arithmeticsensor_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YArithmeticSensorProxy, the Proxy API for ArithmeticSensor
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

namespace YoctoProxyAPI
{
    //--- (YArithmeticSensor class start)
    static public partial class YoctoProxyManager
    {
        public static YArithmeticSensorProxy FindArithmeticSensor(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YArithmeticSensor func = null;
            YArithmeticSensorProxy res = (YArithmeticSensorProxy)YFunctionProxy.FindSimilarUnknownFunction("YArithmeticSensorProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YArithmeticSensorProxy)YFunctionProxy.FindSimilarKnownFunction("YArithmeticSensorProxy");
                if (res != null) return res;
                func = YArithmeticSensor.FirstArithmeticSensor();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YArithmeticSensorProxy)func.get_userData();
                    }
                }
            } else {
                func = YArithmeticSensor.FindArithmeticSensor(name);
                if (func.get_userData() != null) {
                    return (YArithmeticSensorProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YArithmeticSensorProxy(func, name);
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
 *   The YArithmeticSensor class allows some Yoctopuce devices to compute in real-time
 *   values based on an arithmetic formula involving one or more measured signals as
 *   well as the temperature.
 * <para>
 *   This functionality is only available on specific
 *   Yoctopuce devices, for instance using a Yocto-MaxiMicroVolt-Rx.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YArithmeticSensorProxy : YSensorProxy
    {
        //--- (end of YArithmeticSensor class start)
        //--- (YArithmeticSensor definitions)
        public const string _Description_INVALID = YAPI.INVALID_STRING;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YArithmeticSensor _func;
        // property cache
        //--- (end of YArithmeticSensor definitions)

        //--- (YArithmeticSensor implementation)
        internal YArithmeticSensorProxy(YArithmeticSensor hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("ArithmeticSensor " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YArithmeticSensor) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YArithmeticSensor hwd = YArithmeticSensor.FindArithmeticSensor(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YArithmeticSensor hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering ArithmeticSensor callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YArithmeticSensor it = YArithmeticSensor.FirstArithmeticSensor();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextArithmeticSensor();
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
         *   Changes the measuring unit for the arithmetic sensor.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the measuring unit for the arithmetic sensor
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
                string msg = "No ArithmeticSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Unit_INVALID) return YAPI.SUCCESS;
            return _func.set_unit(newval);
        }


        /**
         * <summary>
         *   Returns a short informative description of the formula.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to a short informative description of the formula
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YArithmeticSensor.DESCRIPTION_INVALID</c>.
         * </para>
         */
        public string get_description()
        {
            if (_func == null)
            {
                string msg = "No ArithmeticSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_description();
        }

        /**
         * <summary>
         *   Defines the arithmetic function by means of an algebraic expression.
         * <para>
         *   The expression
         *   may include references to device sensors, by their physical or logical name, to
         *   usual math functions and to auxiliary functions defined separately.
         * </para>
         * </summary>
         * <param name="expr">
         *   the algebraic expression defining the function.
         * </param>
         * <param name="descr">
         *   short informative description of the expression.
         * </param>
         * <returns>
         *   the current expression value if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns YAPI.INVALID_DOUBLE.
         * </para>
         */
        public virtual double defineExpression(string expr, string descr)
        {
            if (_func == null)
            {
                string msg = "No ArithmeticSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.defineExpression(expr, descr);
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Retrieves the algebraic expression defining the arithmetic function, as previously
         *   configured using the <c>defineExpression</c> function.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string containing the mathematical expression.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual string loadExpression()
        {
            if (_func == null)
            {
                string msg = "No ArithmeticSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.loadExpression();
        }

        /**
         * <summary>
         *   Defines a auxiliary function by means of a table of reference points.
         * <para>
         *   Intermediate values
         *   will be interpolated between specified reference points. The reference points are given
         *   as pairs of floating point numbers.
         *   The auxiliary function will be available for use by all ArithmeticSensor objects of the
         *   device. Up to nine auxiliary function can be defined in a device, each containing up to
         *   96 reference points.
         * </para>
         * </summary>
         * <param name="name">
         *   auxiliary function name, up to 16 characters.
         * </param>
         * <param name="inputValues">
         *   array of floating point numbers, corresponding to the function input value.
         * </param>
         * <param name="outputValues">
         *   array of floating point numbers, corresponding to the output value
         *   desired for each of the input value, index by index.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int defineAuxiliaryFunction(string name, double[] inputValues, double[] outputValues)
        {
            if (_func == null)
            {
                string msg = "No ArithmeticSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.defineAuxiliaryFunction(name, new List<double>(inputValues), new List<double>(outputValues));
        }

        /**
         * <summary>
         *   Retrieves the reference points table defining an auxiliary function previously
         *   configured using the <c>defineAuxiliaryFunction</c> function.
         * <para>
         * </para>
         * </summary>
         * <param name="name">
         *   auxiliary function name, up to 16 characters.
         * </param>
         * <param name="inputValues">
         *   array of floating point numbers, that is filled by the function
         *   with all the function reference input value.
         * </param>
         * <param name="outputValues">
         *   array of floating point numbers, that is filled by the function
         *   output value for each of the input value, index by index.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int loadAuxiliaryFunction(string name, double[] inputValues, double[] outputValues)
        {
            if (_func == null)
            {
                string msg = "No ArithmeticSensor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.loadAuxiliaryFunction(name, new List<double>(inputValues), new List<double>(outputValues));
        }
    }
    //--- (end of YArithmeticSensor implementation)
}

