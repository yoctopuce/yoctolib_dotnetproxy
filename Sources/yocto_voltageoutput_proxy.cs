/*********************************************************************
 *
 *  $Id: yocto_voltageoutput_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YVoltageOutputProxy, the Proxy API for VoltageOutput
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
    //--- (YVoltageOutput class start)
    static public partial class YoctoProxyManager
    {
        public static YVoltageOutputProxy FindVoltageOutput(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YVoltageOutput func = null;
            YVoltageOutputProxy res = (YVoltageOutputProxy)YFunctionProxy.FindSimilarUnknownFunction("YVoltageOutputProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YVoltageOutputProxy)YFunctionProxy.FindSimilarKnownFunction("YVoltageOutputProxy");
                if (res != null) return res;
                func = YVoltageOutput.FirstVoltageOutput();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YVoltageOutputProxy)func.get_userData();
                    }
                }
            } else {
                func = YVoltageOutput.FindVoltageOutput(name);
                if (func.get_userData() != null) {
                    return (YVoltageOutputProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YVoltageOutputProxy(func, name);
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
 *   The YVoltageOutput class allows you to drive a voltage output, for instance using a Yocto-0-10V-Tx.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YVoltageOutputProxy : YFunctionProxy
    {
        //--- (end of YVoltageOutput class start)
        //--- (YVoltageOutput definitions)
        public const double _CurrentVoltage_INVALID = Double.NaN;
        public const string _VoltageTransition_INVALID = YAPI.INVALID_STRING;
        public const double _VoltageAtStartUp_INVALID = Double.NaN;

        // reference to real YoctoAPI object
        protected new YVoltageOutput _func;
        // property cache
        protected double _currentVoltage = _CurrentVoltage_INVALID;
        protected double _voltageAtStartUp = _VoltageAtStartUp_INVALID;
        //--- (end of YVoltageOutput definitions)

        //--- (YVoltageOutput implementation)
        internal YVoltageOutputProxy(YVoltageOutput hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("VoltageOutput " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YVoltageOutput) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YVoltageOutput hwd = YVoltageOutput.FindVoltageOutput(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YVoltageOutput hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering VoltageOutput callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YVoltageOutput it = YVoltageOutput.FirstVoltageOutput();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextVoltageOutput();
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
            _voltageAtStartUp = _func.get_voltageAtStartUp();
        }

        /**
         * <summary>
         *   Changes the output voltage, in V.
         * <para>
         *   Valid range is from 0 to 10V.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the output voltage, in V
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
        public int set_currentVoltage(double newval)
        {
            if (_func == null)
            {
                string msg = "No VoltageOutput connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_currentVoltage(newval);
        }


        // property with cached value for instant access (advertised value)
        public double CurrentVoltage
        {
            get
            {
                if (_func == null) return _CurrentVoltage_INVALID;
                return (_online ? _currentVoltage : _CurrentVoltage_INVALID);
            }
            set
            {
                setprop_currentVoltage(value);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture,out _currentVoltage);
        }

        // private helper for magic property
        private void setprop_currentVoltage(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _currentVoltage) return;
            _func.set_currentVoltage(newval);
            _currentVoltage = newval;
        }

        /**
         * <summary>
         *   Returns the output voltage set point, in V.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the output voltage set point, in V
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YVoltageOutput.CURRENTVOLTAGE_INVALID</c>.
         * </para>
         */
        public double get_currentVoltage()
        {
            if (_func == null)
            {
                string msg = "No VoltageOutput connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_currentVoltage();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Changes the output voltage at device start up.
         * <para>
         *   Remember to call the matching
         *   module <c>saveToFlash()</c> method, otherwise this call has no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the output voltage at device start up
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
        public int set_voltageAtStartUp(double newval)
        {
            if (_func == null)
            {
                string msg = "No VoltageOutput connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_voltageAtStartUp(newval);
        }


        // property with cached value for instant access (configuration)
        public double VoltageAtStartUp
        {
            get
            {
                if (_func == null) return _VoltageAtStartUp_INVALID;
                return (_online ? _voltageAtStartUp : _VoltageAtStartUp_INVALID);
            }
            set
            {
                setprop_voltageAtStartUp(value);
            }
        }

        // private helper for magic property
        private void setprop_voltageAtStartUp(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _voltageAtStartUp) return;
            _func.set_voltageAtStartUp(newval);
            _voltageAtStartUp = newval;
        }

        /**
         * <summary>
         *   Returns the selected voltage output at device startup, in V.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the selected voltage output at device startup, in V
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YVoltageOutput.VOLTAGEATSTARTUP_INVALID</c>.
         * </para>
         */
        public double get_voltageAtStartUp()
        {
            if (_func == null)
            {
                string msg = "No VoltageOutput connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_voltageAtStartUp();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Performs a smooth transition of output voltage.
         * <para>
         *   Any explicit voltage
         *   change cancels any ongoing transition process.
         * </para>
         * </summary>
         * <param name="V_target">
         *   new output voltage value at the end of the transition
         *   (floating-point number, representing the end voltage in V)
         * </param>
         * <param name="ms_duration">
         *   total duration of the transition, in milliseconds
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         */
        public virtual int voltageMove(double V_target, int ms_duration)
        {
            if (_func == null)
            {
                string msg = "No VoltageOutput connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.voltageMove(V_target, ms_duration);
        }
    }
    //--- (end of YVoltageOutput implementation)
}
