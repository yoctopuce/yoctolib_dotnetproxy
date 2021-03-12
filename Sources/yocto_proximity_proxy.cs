/*********************************************************************
 *
 *  $Id: yocto_proximity_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
 *
 *  Implements YProximityProxy, the Proxy API for Proximity
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
    //--- (YProximity class start)
    static public partial class YoctoProxyManager
    {
        public static YProximityProxy FindProximity(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YProximity func = null;
            YProximityProxy res = (YProximityProxy)YFunctionProxy.FindSimilarUnknownFunction("YProximityProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YProximityProxy)YFunctionProxy.FindSimilarKnownFunction("YProximityProxy");
                if (res != null) return res;
                func = YProximity.FirstProximity();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YProximityProxy)func.get_userData();
                    }
                }
            } else {
                func = YProximity.FindProximity(name);
                if (func.get_userData() != null) {
                    return (YProximityProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YProximityProxy(func, name);
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
 *   The <c>YProximity</c> class allows you to read and configure Yoctopuce proximity sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 *   This class adds the ability to setup a detection threshold and to count the
 *   number of detected state changes.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YProximityProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a proximity sensor for a given identifier.
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
         *   This function does not require that the proximity sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YProximity.isOnline()</c> to test if the proximity sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a proximity sensor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the proximity sensor, for instance
         *   <c>YPROXIM1.proximity1</c>.
         * </param>
         * <returns>
         *   a <c>YProximity</c> object allowing you to drive the proximity sensor.
         * </returns>
         */
        public static YProximityProxy FindProximity(string func)
        {
            return YoctoProxyManager.FindProximity(func);
        }
        //--- (end of YProximity class start)
        //--- (YProximity definitions)
        public const double _SignalValue_INVALID = Double.NaN;
        public const int _DetectionThreshold_INVALID = -1;
        public const int _DetectionHysteresis_INVALID = -1;
        public const int _PresenceMinTime_INVALID = -1;
        public const int _RemovalMinTime_INVALID = -1;
        public const int _IsPresent_INVALID = 0;
        public const int _IsPresent_FALSE = 1;
        public const int _IsPresent_TRUE = 2;
        public const long _LastTimeApproached_INVALID = YAPI.INVALID_LONG;
        public const long _LastTimeRemoved_INVALID = YAPI.INVALID_LONG;
        public const long _PulseCounter_INVALID = YAPI.INVALID_LONG;
        public const long _PulseTimer_INVALID = YAPI.INVALID_LONG;
        public const int _ProximityReportMode_INVALID = 0;
        public const int _ProximityReportMode_NUMERIC = 1;
        public const int _ProximityReportMode_PRESENCE = 2;
        public const int _ProximityReportMode_PULSECOUNT = 3;

        // reference to real YoctoAPI object
        protected new YProximity _func;
        // property cache
        protected int _detectionThreshold = _DetectionThreshold_INVALID;
        protected int _detectionHysteresis = _DetectionHysteresis_INVALID;
        protected int _presenceMinTime = _PresenceMinTime_INVALID;
        protected int _removalMinTime = _RemovalMinTime_INVALID;
        protected int _proximityReportMode = _ProximityReportMode_INVALID;
        //--- (end of YProximity definitions)

        //--- (YProximity implementation)
        internal YProximityProxy(YProximity hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Proximity " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YProximity) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YProximity hwd = YProximity.FindProximity(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YProximity hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Proximity callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Proximity available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YProximity.FindProximity</c> to obtain an object that can control the
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
            YProximity it = YProximity.FirstProximity();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextProximity();
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
            _detectionThreshold = _func.get_detectionThreshold();
            _detectionHysteresis = _func.get_detectionHysteresis();
            _presenceMinTime = _func.get_presenceMinTime();
            _removalMinTime = _func.get_removalMinTime();
            _proximityReportMode = _func.get_proximityReportMode()+1;
        }

        /**
         * <summary>
         *   Returns the current value of signal measured by the proximity sensor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current value of signal measured by the proximity sensor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.SIGNALVALUE_INVALID</c>.
         * </para>
         */
        public double get_signalValue()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            res = _func.get_signalValue();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the threshold used to determine the logical state of the proximity sensor, when considered
         *   as a binary input (on/off).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the threshold used to determine the logical state of the proximity
         *   sensor, when considered
         *   as a binary input (on/off)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.DETECTIONTHRESHOLD_INVALID</c>.
         * </para>
         */
        public int get_detectionThreshold()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            res = _func.get_detectionThreshold();
            if (res == YAPI.INVALID_INT) {
                res = _DetectionThreshold_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the threshold used to determine the logical state of the proximity sensor, when considered
         *   as a binary input (on/off).
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the threshold used to determine the logical state of the proximity
         *   sensor, when considered
         *   as a binary input (on/off)
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
        public int set_detectionThreshold(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            if (newval == _DetectionThreshold_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_detectionThreshold(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Threshold used to determine the logical state of the proximity sensor, when considered</value>
        public int DetectionThreshold
        {
            get
            {
                if (_func == null) {
                    return _DetectionThreshold_INVALID;
                }
                if (_online) {
                    return _detectionThreshold;
                }
                return _DetectionThreshold_INVALID;
            }
            set
            {
                setprop_detectionThreshold(value);
            }
        }

        // private helper for magic property
        private void setprop_detectionThreshold(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _DetectionThreshold_INVALID) {
                return;
            }
            if (newval == _detectionThreshold) {
                return;
            }
            _func.set_detectionThreshold(newval);
            _detectionThreshold = newval;
        }

        /**
         * <summary>
         *   Returns the hysteresis used to determine the logical state of the proximity sensor, when considered
         *   as a binary input (on/off).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the hysteresis used to determine the logical state of the proximity
         *   sensor, when considered
         *   as a binary input (on/off)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.DETECTIONHYSTERESIS_INVALID</c>.
         * </para>
         */
        public int get_detectionHysteresis()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            res = _func.get_detectionHysteresis();
            if (res == YAPI.INVALID_INT) {
                res = _DetectionHysteresis_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the hysteresis used to determine the logical state of the proximity sensor, when considered
         *   as a binary input (on/off).
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the hysteresis used to determine the logical state of the proximity
         *   sensor, when considered
         *   as a binary input (on/off)
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
        public int set_detectionHysteresis(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            if (newval == _DetectionHysteresis_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_detectionHysteresis(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Hysteresis used to determine the logical state of the proximity sensor, when considered</value>
        public int DetectionHysteresis
        {
            get
            {
                if (_func == null) {
                    return _DetectionHysteresis_INVALID;
                }
                if (_online) {
                    return _detectionHysteresis;
                }
                return _DetectionHysteresis_INVALID;
            }
            set
            {
                setprop_detectionHysteresis(value);
            }
        }

        // private helper for magic property
        private void setprop_detectionHysteresis(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _DetectionHysteresis_INVALID) {
                return;
            }
            if (newval == _detectionHysteresis) {
                return;
            }
            _func.set_detectionHysteresis(newval);
            _detectionHysteresis = newval;
        }

        /**
         * <summary>
         *   Returns the minimal detection duration before signalling a presence event.
         * <para>
         *   Any shorter detection is
         *   considered as noise or bounce (false positive) and filtered out.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the minimal detection duration before signalling a presence event
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.PRESENCEMINTIME_INVALID</c>.
         * </para>
         */
        public int get_presenceMinTime()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            res = _func.get_presenceMinTime();
            if (res == YAPI.INVALID_INT) {
                res = _PresenceMinTime_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the minimal detection duration before signalling a presence event.
         * <para>
         *   Any shorter detection is
         *   considered as noise or bounce (false positive) and filtered out.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the minimal detection duration before signalling a presence event
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
        public int set_presenceMinTime(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            if (newval == _PresenceMinTime_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_presenceMinTime(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Minimal detection duration before signalling a presence event. Any shorter detection is</value>
        public int PresenceMinTime
        {
            get
            {
                if (_func == null) {
                    return _PresenceMinTime_INVALID;
                }
                if (_online) {
                    return _presenceMinTime;
                }
                return _PresenceMinTime_INVALID;
            }
            set
            {
                setprop_presenceMinTime(value);
            }
        }

        // private helper for magic property
        private void setprop_presenceMinTime(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _PresenceMinTime_INVALID) {
                return;
            }
            if (newval == _presenceMinTime) {
                return;
            }
            _func.set_presenceMinTime(newval);
            _presenceMinTime = newval;
        }

        /**
         * <summary>
         *   Returns the minimal detection duration before signalling a removal event.
         * <para>
         *   Any shorter detection is
         *   considered as noise or bounce (false positive) and filtered out.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the minimal detection duration before signalling a removal event
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.REMOVALMINTIME_INVALID</c>.
         * </para>
         */
        public int get_removalMinTime()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            res = _func.get_removalMinTime();
            if (res == YAPI.INVALID_INT) {
                res = _RemovalMinTime_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the minimal detection duration before signalling a removal event.
         * <para>
         *   Any shorter detection is
         *   considered as noise or bounce (false positive) and filtered out.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the minimal detection duration before signalling a removal event
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
        public int set_removalMinTime(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            if (newval == _RemovalMinTime_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_removalMinTime(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Minimal detection duration before signalling a removal event. Any shorter detection is</value>
        public int RemovalMinTime
        {
            get
            {
                if (_func == null) {
                    return _RemovalMinTime_INVALID;
                }
                if (_online) {
                    return _removalMinTime;
                }
                return _RemovalMinTime_INVALID;
            }
            set
            {
                setprop_removalMinTime(value);
            }
        }

        // private helper for magic property
        private void setprop_removalMinTime(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _RemovalMinTime_INVALID) {
                return;
            }
            if (newval == _removalMinTime) {
                return;
            }
            _func.set_removalMinTime(newval);
            _removalMinTime = newval;
        }

        /**
         * <summary>
         *   Returns true if the input (considered as binary) is active (detection value is smaller than the specified <c>threshold</c>), and false otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YProximity.ISPRESENT_FALSE</c> or <c>YProximity.ISPRESENT_TRUE</c>, according to true if
         *   the input (considered as binary) is active (detection value is smaller than the specified
         *   <c>threshold</c>), and false otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.ISPRESENT_INVALID</c>.
         * </para>
         */
        public int get_isPresent()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_isPresent()+1;
        }

        /**
         * <summary>
         *   Returns the number of elapsed milliseconds between the module power on and the last observed
         *   detection (the input contact transitioned from absent to present).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last observed
         *   detection (the input contact transitioned from absent to present)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.LASTTIMEAPPROACHED_INVALID</c>.
         * </para>
         */
        public long get_lastTimeApproached()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            return _func.get_lastTimeApproached();
        }

        /**
         * <summary>
         *   Returns the number of elapsed milliseconds between the module power on and the last observed
         *   detection (the input contact transitioned from present to absent).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last observed
         *   detection (the input contact transitioned from present to absent)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.LASTTIMEREMOVED_INVALID</c>.
         * </para>
         */
        public long get_lastTimeRemoved()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            return _func.get_lastTimeRemoved();
        }

        /**
         * <summary>
         *   Returns the pulse counter value.
         * <para>
         *   The value is a 32 bit integer. In case
         *   of overflow (>=2^32), the counter will wrap. To reset the counter, just
         *   call the resetCounter() method.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the pulse counter value
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.PULSECOUNTER_INVALID</c>.
         * </para>
         */
        public long get_pulseCounter()
        {
            long res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            res = _func.get_pulseCounter();
            if (res == YAPI.INVALID_INT) {
                res = _PulseCounter_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the timer of the pulse counter (ms).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the timer of the pulse counter (ms)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.PULSETIMER_INVALID</c>.
         * </para>
         */
        public long get_pulseTimer()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            return _func.get_pulseTimer();
        }

        /**
         * <summary>
         *   Returns the parameter (sensor value, presence or pulse count) returned by the get_currentValue function and callbacks.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YProximity.PROXIMITYREPORTMODE_NUMERIC</c>,
         *   <c>YProximity.PROXIMITYREPORTMODE_PRESENCE</c> and <c>YProximity.PROXIMITYREPORTMODE_PULSECOUNT</c>
         *   corresponding to the parameter (sensor value, presence or pulse count) returned by the
         *   get_currentValue function and callbacks
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YProximity.PROXIMITYREPORTMODE_INVALID</c>.
         * </para>
         */
        public int get_proximityReportMode()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_proximityReportMode()+1;
        }

        /**
         * <summary>
         *   Changes the  parameter  type (sensor value, presence or pulse count) returned by the get_currentValue function and callbacks.
         * <para>
         *   The edge count value is limited to the 6 lowest digits. For values greater than one million, use
         *   get_pulseCounter().
         *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YProximity.PROXIMITYREPORTMODE_NUMERIC</c>,
         *   <c>YProximity.PROXIMITYREPORTMODE_PRESENCE</c> and <c>YProximity.PROXIMITYREPORTMODE_PULSECOUNT</c>
         *   corresponding to the  parameter  type (sensor value, presence or pulse count) returned by the
         *   get_currentValue function and callbacks
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
        public int set_proximityReportMode(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            if (newval == _ProximityReportMode_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_proximityReportMode(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Parameter (sensor value, presence or pulse count) returned by the get_currentValue function and callbacks.</value>
        public int ProximityReportMode
        {
            get
            {
                if (_func == null) {
                    return _ProximityReportMode_INVALID;
                }
                if (_online) {
                    return _proximityReportMode;
                }
                return _ProximityReportMode_INVALID;
            }
            set
            {
                setprop_proximityReportMode(value);
            }
        }

        // private helper for magic property
        private void setprop_proximityReportMode(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _ProximityReportMode_INVALID) {
                return;
            }
            if (newval == _proximityReportMode) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_proximityReportMode(newval-1);
            _proximityReportMode = newval;
        }

        /**
         * <summary>
         *   Resets the pulse counter value as well as its timer.
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
        public virtual int resetCounter()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Proximity connected");
            }
            return _func.resetCounter();
        }
    }
    //--- (end of YProximity implementation)
}

