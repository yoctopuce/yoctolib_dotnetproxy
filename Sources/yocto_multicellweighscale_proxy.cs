/*********************************************************************
 *
 *  $Id: yocto_multicellweighscale_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YMultiCellWeighScaleProxy, the Proxy API for MultiCellWeighScale
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
    //--- (YMultiCellWeighScale class start)
    static public partial class YoctoProxyManager
    {
        public static YMultiCellWeighScaleProxy FindMultiCellWeighScale(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YMultiCellWeighScale func = null;
            YMultiCellWeighScaleProxy res = (YMultiCellWeighScaleProxy)YFunctionProxy.FindSimilarUnknownFunction("YMultiCellWeighScaleProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YMultiCellWeighScaleProxy)YFunctionProxy.FindSimilarKnownFunction("YMultiCellWeighScaleProxy");
                if (res != null) return res;
                func = YMultiCellWeighScale.FirstMultiCellWeighScale();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YMultiCellWeighScaleProxy)func.get_userData();
                    }
                }
            } else {
                func = YMultiCellWeighScale.FindMultiCellWeighScale(name);
                if (func.get_userData() != null) {
                    return (YMultiCellWeighScaleProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YMultiCellWeighScaleProxy(func, name);
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
 *   The YMultiCellWeighScale class provides a weight measurement from a set of ratiometric
 *   sensors, for instance using a Yocto-MaxiBridge.
 * <para>
 *   It can be used to control the bridge excitation parameters, in order to avoid
 *   measure shifts caused by temperature variation in the electronics, and can also
 *   automatically apply an additional correction factor based on temperature to
 *   compensate for offsets in the load cells themselves.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YMultiCellWeighScaleProxy : YSensorProxy
    {
        //--- (end of YMultiCellWeighScale class start)
        //--- (YMultiCellWeighScale definitions)
        public const int _CellCount_INVALID = -1;
        public const int _Excitation_INVALID = 0;
        public const int _Excitation_OFF = 1;
        public const int _Excitation_DC = 2;
        public const int _Excitation_AC = 3;
        public const double _TempAvgAdaptRatio_INVALID = Double.NaN;
        public const double _TempChgAdaptRatio_INVALID = Double.NaN;
        public const double _CompTempAvg_INVALID = Double.NaN;
        public const double _CompTempChg_INVALID = Double.NaN;
        public const double _Compensation_INVALID = Double.NaN;
        public const double _ZeroTracking_INVALID = Double.NaN;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YMultiCellWeighScale _func;
        // property cache
        protected int _cellCount = _CellCount_INVALID;
        protected int _excitation = _Excitation_INVALID;
        protected double _tempAvgAdaptRatio = _TempAvgAdaptRatio_INVALID;
        protected double _tempChgAdaptRatio = _TempChgAdaptRatio_INVALID;
        protected double _zeroTracking = _ZeroTracking_INVALID;
        //--- (end of YMultiCellWeighScale definitions)

        //--- (YMultiCellWeighScale implementation)
        internal YMultiCellWeighScaleProxy(YMultiCellWeighScale hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("MultiCellWeighScale " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YMultiCellWeighScale) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YMultiCellWeighScale hwd = YMultiCellWeighScale.FindMultiCellWeighScale(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YMultiCellWeighScale hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering MultiCellWeighScale callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YMultiCellWeighScale it = YMultiCellWeighScale.FirstMultiCellWeighScale();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextMultiCellWeighScale();
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
            _cellCount = _func.get_cellCount();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _excitation = _func.get_excitation()+1;
            _tempAvgAdaptRatio = _func.get_tempAvgAdaptRatio();
            _tempChgAdaptRatio = _func.get_tempChgAdaptRatio();
            _zeroTracking = _func.get_zeroTracking();
        }

        /**
         * <summary>
         *   Changes the measuring unit for the weight.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the measuring unit for the weight
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
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Unit_INVALID) return YAPI.SUCCESS;
            return _func.set_unit(newval);
        }


        /**
         * <summary>
         *   Returns the number of load cells in use.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of load cells in use
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiCellWeighScale.CELLCOUNT_INVALID</c>.
         * </para>
         */
        public int get_cellCount()
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_cellCount();
            if (res == YAPI.INVALID_INT) res = _CellCount_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the number of load cells in use.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the number of load cells in use
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
        public int set_cellCount(int newval)
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _CellCount_INVALID) return YAPI.SUCCESS;
            return _func.set_cellCount(newval);
        }


        // property with cached value for instant access (configuration)
        public int CellCount
        {
            get
            {
                if (_func == null) return _CellCount_INVALID;
                return (_online ? _cellCount : _CellCount_INVALID);
            }
            set
            {
                setprop_cellCount(value);
            }
        }

        // private helper for magic property
        private void setprop_cellCount(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _CellCount_INVALID) return;
            if (newval == _cellCount) return;
            _func.set_cellCount(newval);
            _cellCount = newval;
        }

        /**
         * <summary>
         *   Returns the current load cell bridge excitation method.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YMultiCellWeighScale.EXCITATION_OFF</c>, <c>YMultiCellWeighScale.EXCITATION_DC</c>
         *   and <c>YMultiCellWeighScale.EXCITATION_AC</c> corresponding to the current load cell bridge excitation method
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiCellWeighScale.EXCITATION_INVALID</c>.
         * </para>
         */
        public int get_excitation()
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_excitation()+1;
        }

        /**
         * <summary>
         *   Changes the current load cell bridge excitation method.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YMultiCellWeighScale.EXCITATION_OFF</c>, <c>YMultiCellWeighScale.EXCITATION_DC</c>
         *   and <c>YMultiCellWeighScale.EXCITATION_AC</c> corresponding to the current load cell bridge excitation method
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
        public int set_excitation(int newval)
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Excitation_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_excitation(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int Excitation
        {
            get
            {
                if (_func == null) return _Excitation_INVALID;
                return (_online ? _excitation : _Excitation_INVALID);
            }
            set
            {
                setprop_excitation(value);
            }
        }

        // private helper for magic property
        private void setprop_excitation(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Excitation_INVALID) return;
            if (newval == _excitation) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_excitation(newval-1);
            _excitation = newval;
        }

        /**
         * <summary>
         *   Changes the averaged temperature update rate, in per mille.
         * <para>
         *   The purpose of this adaptation ratio is to model the thermal inertia of the load cell.
         *   The averaged temperature is updated every 10 seconds, by applying this adaptation rate
         *   to the difference between the measures ambient temperature and the current compensation
         *   temperature. The standard rate is 0.2 per mille, and the maximal rate is 65 per mille.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the averaged temperature update rate, in per mille
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
        public int set_tempAvgAdaptRatio(double newval)
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_tempAvgAdaptRatio(newval);
        }


        // property with cached value for instant access (configuration)
        public double TempAvgAdaptRatio
        {
            get
            {
                if (_func == null) return _TempAvgAdaptRatio_INVALID;
                return (_online ? _tempAvgAdaptRatio : _TempAvgAdaptRatio_INVALID);
            }
            set
            {
                setprop_tempAvgAdaptRatio(value);
            }
        }

        // private helper for magic property
        private void setprop_tempAvgAdaptRatio(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _tempAvgAdaptRatio) return;
            _func.set_tempAvgAdaptRatio(newval);
            _tempAvgAdaptRatio = newval;
        }

        /**
         * <summary>
         *   Returns the averaged temperature update rate, in per mille.
         * <para>
         *   The purpose of this adaptation ratio is to model the thermal inertia of the load cell.
         *   The averaged temperature is updated every 10 seconds, by applying this adaptation rate
         *   to the difference between the measures ambient temperature and the current compensation
         *   temperature. The standard rate is 0.2 per mille, and the maximal rate is 65 per mille.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the averaged temperature update rate, in per mille
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiCellWeighScale.TEMPAVGADAPTRATIO_INVALID</c>.
         * </para>
         */
        public double get_tempAvgAdaptRatio()
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_tempAvgAdaptRatio();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Changes the temperature change update rate, in per mille.
         * <para>
         *   The temperature change is updated every 10 seconds, by applying this adaptation rate
         *   to the difference between the measures ambient temperature and the current temperature used for
         *   change compensation. The standard rate is 0.6 per mille, and the maximal rate is 65 per mille.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the temperature change update rate, in per mille
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
        public int set_tempChgAdaptRatio(double newval)
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_tempChgAdaptRatio(newval);
        }


        // property with cached value for instant access (configuration)
        public double TempChgAdaptRatio
        {
            get
            {
                if (_func == null) return _TempChgAdaptRatio_INVALID;
                return (_online ? _tempChgAdaptRatio : _TempChgAdaptRatio_INVALID);
            }
            set
            {
                setprop_tempChgAdaptRatio(value);
            }
        }

        // private helper for magic property
        private void setprop_tempChgAdaptRatio(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _tempChgAdaptRatio) return;
            _func.set_tempChgAdaptRatio(newval);
            _tempChgAdaptRatio = newval;
        }

        /**
         * <summary>
         *   Returns the temperature change update rate, in per mille.
         * <para>
         *   The temperature change is updated every 10 seconds, by applying this adaptation rate
         *   to the difference between the measures ambient temperature and the current temperature used for
         *   change compensation. The standard rate is 0.6 per mille, and the maximal rate is 65 per mille.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the temperature change update rate, in per mille
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiCellWeighScale.TEMPCHGADAPTRATIO_INVALID</c>.
         * </para>
         */
        public double get_tempChgAdaptRatio()
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_tempChgAdaptRatio();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the current averaged temperature, used for thermal compensation.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current averaged temperature, used for thermal compensation
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiCellWeighScale.COMPTEMPAVG_INVALID</c>.
         * </para>
         */
        public double get_compTempAvg()
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_compTempAvg();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the current temperature variation, used for thermal compensation.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current temperature variation, used for thermal compensation
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiCellWeighScale.COMPTEMPCHG_INVALID</c>.
         * </para>
         */
        public double get_compTempChg()
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_compTempChg();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the current current thermal compensation value.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current current thermal compensation value
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiCellWeighScale.COMPENSATION_INVALID</c>.
         * </para>
         */
        public double get_compensation()
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_compensation();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Changes the zero tracking threshold value.
         * <para>
         *   When this threshold is larger than
         *   zero, any measure under the threshold will automatically be ignored and the
         *   zero compensation will be updated.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the zero tracking threshold value
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
        public int set_zeroTracking(double newval)
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_zeroTracking(newval);
        }


        // property with cached value for instant access (configuration)
        public double ZeroTracking
        {
            get
            {
                if (_func == null) return _ZeroTracking_INVALID;
                return (_online ? _zeroTracking : _ZeroTracking_INVALID);
            }
            set
            {
                setprop_zeroTracking(value);
            }
        }

        // private helper for magic property
        private void setprop_zeroTracking(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _zeroTracking) return;
            _func.set_zeroTracking(newval);
            _zeroTracking = newval;
        }

        /**
         * <summary>
         *   Returns the zero tracking threshold value.
         * <para>
         *   When this threshold is larger than
         *   zero, any measure under the threshold will automatically be ignored and the
         *   zero compensation will be updated.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the zero tracking threshold value
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMultiCellWeighScale.ZEROTRACKING_INVALID</c>.
         * </para>
         */
        public double get_zeroTracking()
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_zeroTracking();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Adapts the load cell signal bias (stored in the corresponding genericSensor)
         *   so that the current signal corresponds to a zero weight.
         * <para>
         * </para>
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
        public virtual int tare()
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.tare();
        }

        /**
         * <summary>
         *   Configures the load cells span parameters (stored in the corresponding genericSensors)
         *   so that the current signal corresponds to the specified reference weight.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="currWeight">
         *   reference weight presently on the load cell.
         * </param>
         * <param name="maxWeight">
         *   maximum weight to be expected on the load cell.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int setupSpan(double currWeight, double maxWeight)
        {
            if (_func == null)
            {
                string msg = "No MultiCellWeighScale connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.setupSpan(currWeight, maxWeight);
        }
    }
    //--- (end of YMultiCellWeighScale implementation)
}

