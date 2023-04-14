/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YInputCaptureProxy, the Proxy API for InputCapture
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
    //--- (YInputCapture class start)
    static public partial class YoctoProxyManager
    {
        public static YInputCaptureProxy FindInputCapture(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YInputCapture func = null;
            YInputCaptureProxy res = (YInputCaptureProxy)YFunctionProxy.FindSimilarUnknownFunction("YInputCaptureProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YInputCaptureProxy)YFunctionProxy.FindSimilarKnownFunction("YInputCaptureProxy");
                if (res != null) return res;
                func = YInputCapture.FirstInputCapture();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YInputCaptureProxy)func.get_userData();
                    }
                }
            } else {
                func = YInputCapture.FindInputCapture(name);
                if (func.get_userData() != null) {
                    return (YInputCaptureProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YInputCaptureProxy(func, name);
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
 *   The <c>YInputCapture</c> class allows you to access data samples
 *   measured by a Yoctopuce electrical sensor.
 * <para>
 *   The data capture can be
 *   triggered manually, or be configured to detect specific events.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YInputCaptureProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves an instant snapshot trigger for a given identifier.
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
         *   This function does not require that the instant snapshot trigger is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YInputCapture.isOnline()</c> to test if the instant snapshot trigger is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   an instant snapshot trigger by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the instant snapshot trigger, for instance
         *   <c>MyDevice.inputCapture</c>.
         * </param>
         * <returns>
         *   a <c>YInputCapture</c> object allowing you to drive the instant snapshot trigger.
         * </returns>
         */
        public static YInputCaptureProxy FindInputCapture(string func)
        {
            return YoctoProxyManager.FindInputCapture(func);
        }
        //--- (end of YInputCapture class start)
        //--- (YInputCapture definitions)
        public const long _LastCaptureTime_INVALID = YAPI.INVALID_LONG;
        public const int _NSamples_INVALID = -1;
        public const int _SamplingRate_INVALID = -1;
        public const int _CaptureType_INVALID = 0;
        public const int _CaptureType_NONE = 1;
        public const int _CaptureType_TIMED = 2;
        public const int _CaptureType_V_MAX = 3;
        public const int _CaptureType_V_MIN = 4;
        public const int _CaptureType_I_MAX = 5;
        public const int _CaptureType_I_MIN = 6;
        public const int _CaptureType_P_MAX = 7;
        public const int _CaptureType_P_MIN = 8;
        public const int _CaptureType_V_AVG_MAX = 9;
        public const int _CaptureType_V_AVG_MIN = 10;
        public const int _CaptureType_V_RMS_MAX = 11;
        public const int _CaptureType_V_RMS_MIN = 12;
        public const int _CaptureType_I_AVG_MAX = 13;
        public const int _CaptureType_I_AVG_MIN = 14;
        public const int _CaptureType_I_RMS_MAX = 15;
        public const int _CaptureType_I_RMS_MIN = 16;
        public const int _CaptureType_P_AVG_MAX = 17;
        public const int _CaptureType_P_AVG_MIN = 18;
        public const int _CaptureType_PF_MIN = 19;
        public const int _CaptureType_DPF_MIN = 20;
        public const double _CondValue_INVALID = Double.NaN;
        public const int _CondAlign_INVALID = -1;
        public const int _CaptureTypeAtStartup_INVALID = 0;
        public const int _CaptureTypeAtStartup_NONE = 1;
        public const int _CaptureTypeAtStartup_TIMED = 2;
        public const int _CaptureTypeAtStartup_V_MAX = 3;
        public const int _CaptureTypeAtStartup_V_MIN = 4;
        public const int _CaptureTypeAtStartup_I_MAX = 5;
        public const int _CaptureTypeAtStartup_I_MIN = 6;
        public const int _CaptureTypeAtStartup_P_MAX = 7;
        public const int _CaptureTypeAtStartup_P_MIN = 8;
        public const int _CaptureTypeAtStartup_V_AVG_MAX = 9;
        public const int _CaptureTypeAtStartup_V_AVG_MIN = 10;
        public const int _CaptureTypeAtStartup_V_RMS_MAX = 11;
        public const int _CaptureTypeAtStartup_V_RMS_MIN = 12;
        public const int _CaptureTypeAtStartup_I_AVG_MAX = 13;
        public const int _CaptureTypeAtStartup_I_AVG_MIN = 14;
        public const int _CaptureTypeAtStartup_I_RMS_MAX = 15;
        public const int _CaptureTypeAtStartup_I_RMS_MIN = 16;
        public const int _CaptureTypeAtStartup_P_AVG_MAX = 17;
        public const int _CaptureTypeAtStartup_P_AVG_MIN = 18;
        public const int _CaptureTypeAtStartup_PF_MIN = 19;
        public const int _CaptureTypeAtStartup_DPF_MIN = 20;
        public const double _CondValueAtStartup_INVALID = Double.NaN;

        // reference to real YoctoAPI object
        protected new YInputCapture _func;
        // property cache
        protected long _lastCaptureTime = _LastCaptureTime_INVALID;
        protected int _nSamples = _NSamples_INVALID;
        protected int _condAlign = _CondAlign_INVALID;
        protected int _captureTypeAtStartup = _CaptureTypeAtStartup_INVALID;
        protected double _condValueAtStartup = _CondValueAtStartup_INVALID;
        protected double _condValue = _CondValue_INVALID;
        //--- (end of YInputCapture definitions)

        //--- (YInputCapture implementation)
        internal YInputCaptureProxy(YInputCapture hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("InputCapture " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YInputCapture) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YInputCapture hwd = YInputCapture.FindInputCapture(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YInputCapture hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering InputCapture callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumère toutes les fonctions de type InputCapture.
         * <para>
         *   Returns an array of strings representing hardware identifiers for all InputCapture functions
         *   presently connected.
         * </para>
         * </summary>
         */
        public static new string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YInputCapture it = YInputCapture.FirstInputCapture();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextInputCapture();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _condValue = _func.get_condValue();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _nSamples = _func.get_nSamples();
            _condAlign = _func.get_condAlign();
            _captureTypeAtStartup = _func.get_captureTypeAtStartup()+1;
            _condValueAtStartup = _func.get_condValueAtStartup();
        }

        /**
         * <summary>
         *   Returns the number of elapsed milliseconds between the module power on
         *   and the last capture (time of trigger), or zero if no capture has been done.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of elapsed milliseconds between the module power on
         *   and the last capture (time of trigger), or zero if no capture has been done
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputCapture.LASTCAPTURETIME_INVALID</c>.
         * </para>
         */
        public long get_lastCaptureTime()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            return _func.get_lastCaptureTime();
        }

        // property with cached value for instant access (advertised value)
        /// <value>Number of elapsed milliseconds between the module power on</value>
        public long LastCaptureTime
        {
            get
            {
                if (_func == null) {
                    return _LastCaptureTime_INVALID;
                }
                if (_online) {
                    return _lastCaptureTime;
                }
                return _LastCaptureTime_INVALID;
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            _lastCaptureTime = YAPI._hexStrToInt(value);
            _condValue = _func.get_condValue();
        }

        // property with cached value for instant access (derived from advertised value)
        /// <value>Current threshold value for automatic conditional capture.</value>
        public double CondValue
        {
            get
            {
                if (_func == null) {
                    return _CondValue_INVALID;
                }
                if (_online) {
                    return _condValue;
                }
                return _CondValue_INVALID;
            }
            set
            {
                setprop_condValue(value);
            }
        }

        // private helper for magic property
        private void setprop_condValue(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CondValue_INVALID) {
                return;
            }
            if (newval == _condValue) {
                return;
            }
            _func.set_condValue(newval);
            _condValue = newval;
        }

        /**
         * <summary>
         *   Returns the number of samples that will be captured.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of samples that will be captured
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputCapture.NSAMPLES_INVALID</c>.
         * </para>
         */
        public int get_nSamples()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            res = _func.get_nSamples();
            if (res == YAPI.INVALID_INT) {
                res = _NSamples_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the type of automatic conditional capture.
         * <para>
         *   The maximum number of samples depends on the device memory.
         * </para>
         * <para>
         *   If you want the change to be kept after a device reboot,
         *   make sure  to call the matching module <c>saveToFlash()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the type of automatic conditional capture
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_nSamples(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            if (newval == _NSamples_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_nSamples(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Number of samples that will be captured.</value>
        public int NSamples
        {
            get
            {
                if (_func == null) {
                    return _NSamples_INVALID;
                }
                if (_online) {
                    return _nSamples;
                }
                return _NSamples_INVALID;
            }
            set
            {
                setprop_nSamples(value);
            }
        }

        // private helper for magic property
        private void setprop_nSamples(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _NSamples_INVALID) {
                return;
            }
            if (newval == _nSamples) {
                return;
            }
            _func.set_nSamples(newval);
            _nSamples = newval;
        }

        /**
         * <summary>
         *   Returns the sampling frequency, in Hz.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the sampling frequency, in Hz
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputCapture.SAMPLINGRATE_INVALID</c>.
         * </para>
         */
        public int get_samplingRate()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            res = _func.get_samplingRate();
            if (res == YAPI.INVALID_INT) {
                res = _SamplingRate_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the type of automatic conditional capture.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YInputCapture.CAPTURETYPE_NONE</c>, <c>YInputCapture.CAPTURETYPE_TIMED</c>,
         *   <c>YInputCapture.CAPTURETYPE_V_MAX</c>, <c>YInputCapture.CAPTURETYPE_V_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_I_MAX</c>, <c>YInputCapture.CAPTURETYPE_I_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_P_MAX</c>, <c>YInputCapture.CAPTURETYPE_P_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_V_AVG_MAX</c>, <c>YInputCapture.CAPTURETYPE_V_AVG_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_V_RMS_MAX</c>, <c>YInputCapture.CAPTURETYPE_V_RMS_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_I_AVG_MAX</c>, <c>YInputCapture.CAPTURETYPE_I_AVG_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_I_RMS_MAX</c>, <c>YInputCapture.CAPTURETYPE_I_RMS_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_P_AVG_MAX</c>, <c>YInputCapture.CAPTURETYPE_P_AVG_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_PF_MIN</c> and <c>YInputCapture.CAPTURETYPE_DPF_MIN</c> corresponding
         *   to the type of automatic conditional capture
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputCapture.CAPTURETYPE_INVALID</c>.
         * </para>
         */
        public int get_captureType()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_captureType()+1;
        }

        /**
         * <summary>
         *   Changes the type of automatic conditional capture.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YInputCapture.CAPTURETYPE_NONE</c>, <c>YInputCapture.CAPTURETYPE_TIMED</c>,
         *   <c>YInputCapture.CAPTURETYPE_V_MAX</c>, <c>YInputCapture.CAPTURETYPE_V_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_I_MAX</c>, <c>YInputCapture.CAPTURETYPE_I_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_P_MAX</c>, <c>YInputCapture.CAPTURETYPE_P_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_V_AVG_MAX</c>, <c>YInputCapture.CAPTURETYPE_V_AVG_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_V_RMS_MAX</c>, <c>YInputCapture.CAPTURETYPE_V_RMS_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_I_AVG_MAX</c>, <c>YInputCapture.CAPTURETYPE_I_AVG_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_I_RMS_MAX</c>, <c>YInputCapture.CAPTURETYPE_I_RMS_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_P_AVG_MAX</c>, <c>YInputCapture.CAPTURETYPE_P_AVG_MIN</c>,
         *   <c>YInputCapture.CAPTURETYPE_PF_MIN</c> and <c>YInputCapture.CAPTURETYPE_DPF_MIN</c> corresponding
         *   to the type of automatic conditional capture
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_captureType(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            if (newval == _CaptureType_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_captureType(newval-1);
        }

        /**
         * <summary>
         *   Changes current threshold value for automatic conditional capture.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to current threshold value for automatic conditional capture
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_condValue(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            if (newval == _CondValue_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_condValue(newval);
        }

        /**
         * <summary>
         *   Returns current threshold value for automatic conditional capture.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to current threshold value for automatic conditional capture
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputCapture.CONDVALUE_INVALID</c>.
         * </para>
         */
        public double get_condValue()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            res = _func.get_condValue();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the relative position of the trigger event within the capture window.
         * <para>
         *   When the value is 50%, the capture is centered on the event.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the relative position of the trigger event within the capture window
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputCapture.CONDALIGN_INVALID</c>.
         * </para>
         */
        public int get_condAlign()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            res = _func.get_condAlign();
            if (res == YAPI.INVALID_INT) {
                res = _CondAlign_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the relative position of the trigger event within the capture window.
         * <para>
         *   The new value must be between 10% (on the left) and 90% (on the right).
         *   When the value is 50%, the capture is centered on the event.
         * </para>
         * <para>
         *   If you want the change to be kept after a device reboot,
         *   make sure  to call the matching module <c>saveToFlash()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the relative position of the trigger event within the capture window
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_condAlign(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            if (newval == _CondAlign_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_condAlign(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Relative position of the trigger event within the capture window.</value>
        public int CondAlign
        {
            get
            {
                if (_func == null) {
                    return _CondAlign_INVALID;
                }
                if (_online) {
                    return _condAlign;
                }
                return _CondAlign_INVALID;
            }
            set
            {
                setprop_condAlign(value);
            }
        }

        // private helper for magic property
        private void setprop_condAlign(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CondAlign_INVALID) {
                return;
            }
            if (newval == _condAlign) {
                return;
            }
            _func.set_condAlign(newval);
            _condAlign = newval;
        }

        /**
         * <summary>
         *   Returns the type of automatic conditional capture
         *   applied at device power on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YInputCapture.CAPTURETYPEATSTARTUP_NONE</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_TIMED</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_V_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_V_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_I_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_I_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_P_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_P_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_V_AVG_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_V_AVG_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_V_RMS_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_V_RMS_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_I_AVG_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_I_AVG_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_I_RMS_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_I_RMS_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_P_AVG_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_P_AVG_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_PF_MIN</c>
         *   and <c>YInputCapture.CAPTURETYPEATSTARTUP_DPF_MIN</c> corresponding to the type of automatic conditional capture
         *   applied at device power on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputCapture.CAPTURETYPEATSTARTUP_INVALID</c>.
         * </para>
         */
        public int get_captureTypeAtStartup()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_captureTypeAtStartup()+1;
        }

        /**
         * <summary>
         *   Changes the type of automatic conditional capture
         *   applied at device power on.
         * <para>
         * </para>
         * <para>
         *   If you want the change to be kept after a device reboot,
         *   make sure  to call the matching module <c>saveToFlash()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YInputCapture.CAPTURETYPEATSTARTUP_NONE</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_TIMED</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_V_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_V_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_I_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_I_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_P_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_P_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_V_AVG_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_V_AVG_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_V_RMS_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_V_RMS_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_I_AVG_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_I_AVG_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_I_RMS_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_I_RMS_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_P_AVG_MAX</c>,
         *   <c>YInputCapture.CAPTURETYPEATSTARTUP_P_AVG_MIN</c>, <c>YInputCapture.CAPTURETYPEATSTARTUP_PF_MIN</c>
         *   and <c>YInputCapture.CAPTURETYPEATSTARTUP_DPF_MIN</c> corresponding to the type of automatic conditional capture
         *   applied at device power on
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_captureTypeAtStartup(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            if (newval == _CaptureTypeAtStartup_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_captureTypeAtStartup(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Type of automatic conditional capture</value>
        public int CaptureTypeAtStartup
        {
            get
            {
                if (_func == null) {
                    return _CaptureTypeAtStartup_INVALID;
                }
                if (_online) {
                    return _captureTypeAtStartup;
                }
                return _CaptureTypeAtStartup_INVALID;
            }
            set
            {
                setprop_captureTypeAtStartup(value);
            }
        }

        // private helper for magic property
        private void setprop_captureTypeAtStartup(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CaptureTypeAtStartup_INVALID) {
                return;
            }
            if (newval == _captureTypeAtStartup) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_captureTypeAtStartup(newval-1);
            _captureTypeAtStartup = newval;
        }

        /**
         * <summary>
         *   Changes current threshold value for automatic conditional
         *   capture applied at device power on.
         * <para>
         * </para>
         * <para>
         *   If you want the change to be kept after a device reboot,
         *   make sure  to call the matching module <c>saveToFlash()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to current threshold value for automatic conditional
         *   capture applied at device power on
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_condValueAtStartup(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            if (newval == _CondValueAtStartup_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_condValueAtStartup(newval);
        }

        /**
         * <summary>
         *   Returns the threshold value for automatic conditional
         *   capture applied at device power on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the threshold value for automatic conditional
         *   capture applied at device power on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YInputCapture.CONDVALUEATSTARTUP_INVALID</c>.
         * </para>
         */
        public double get_condValueAtStartup()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            res = _func.get_condValueAtStartup();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (configuration)
        /// <value>Threshold value for automatic conditional</value>
        public double CondValueAtStartup
        {
            get
            {
                if (_func == null) {
                    return _CondValueAtStartup_INVALID;
                }
                if (_online) {
                    return _condValueAtStartup;
                }
                return _CondValueAtStartup_INVALID;
            }
            set
            {
                setprop_condValueAtStartup(value);
            }
        }

        // private helper for magic property
        private void setprop_condValueAtStartup(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _CondValueAtStartup_INVALID) {
                return;
            }
            if (newval == _condValueAtStartup) {
                return;
            }
            _func.set_condValueAtStartup(newval);
            _condValueAtStartup = newval;
        }

        /**
         * <summary>
         *   Returns all details about the last automatic input capture.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an <c>YInputCaptureData</c> object including
         *   data series and all related meta-information.
         *   On failure, throws an exception or returns an capture object.
         * </returns>
         */
        public virtual YInputCaptureDataProxy get_lastCapture()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            return new YInputCaptureDataProxy(_func.get_lastCapture());
        }

        /**
         * <summary>
         *   Returns a new immediate capture of the device inputs.
         * <para>
         * </para>
         * </summary>
         * <param name="msDuration">
         *   duration of the capture window,
         *   in milliseconds (eg. between 20 and 1000).
         * </param>
         * <returns>
         *   an <c>YInputCaptureData</c> object including
         *   data series for the specified duration.
         *   On failure, throws an exception or returns an capture object.
         * </returns>
         */
        public virtual YInputCaptureDataProxy get_immediateCapture(int msDuration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No InputCapture connected");
            }
            return new YInputCaptureDataProxy(_func.get_immediateCapture(msDuration));
        }
    }
    //--- (end of YInputCapture implementation)
}

