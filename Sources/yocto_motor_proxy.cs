/*********************************************************************
 *
 *  $Id: yocto_motor_proxy.cs 38913 2019-12-20 18:59:49Z mvuilleu $
 *
 *  Implements YMotorProxy, the Proxy API for Motor
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
    //--- (YMotor class start)
    static public partial class YoctoProxyManager
    {
        public static YMotorProxy FindMotor(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YMotor func = null;
            YMotorProxy res = (YMotorProxy)YFunctionProxy.FindSimilarUnknownFunction("YMotorProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YMotorProxy)YFunctionProxy.FindSimilarKnownFunction("YMotorProxy");
                if (res != null) return res;
                func = YMotor.FirstMotor();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YMotorProxy)func.get_userData();
                    }
                }
            } else {
                func = YMotor.FindMotor(name);
                if (func.get_userData() != null) {
                    return (YMotorProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YMotorProxy(func, name);
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
 *   The <c>YMotor</c> class allows you to drive a DC motor.
 * <para>
 *   It can be used to configure the
 *   power sent to the motor to make it turn both ways, but also to drive accelerations
 *   and decelerations. The motor will then accelerate automatically: you will not
 *   have to monitor it. The API also allows to slow down the motor by shortening
 *   its terminals: the motor will then act as an electromagnetic brake.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YMotorProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a motor for a given identifier.
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
         *   This function does not require that the motor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YMotor.isOnline()</c> to test if the motor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a motor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the motor, for instance
         *   <c>MOTORCTL.motor</c>.
         * </param>
         * <returns>
         *   a <c>YMotor</c> object allowing you to drive the motor.
         * </returns>
         */
        public static YMotorProxy FindMotor(string func)
        {
            return YoctoProxyManager.FindMotor(func);
        }
        //--- (end of YMotor class start)
        //--- (YMotor definitions)
        public const int _MotorStatus_INVALID = 0;
        public const int _MotorStatus_IDLE = 1;
        public const int _MotorStatus_BRAKE = 2;
        public const int _MotorStatus_FORWD = 3;
        public const int _MotorStatus_BACKWD = 4;
        public const int _MotorStatus_LOVOLT = 5;
        public const int _MotorStatus_HICURR = 6;
        public const int _MotorStatus_HIHEAT = 7;
        public const int _MotorStatus_FAILSF = 8;
        public const double _DrivingForce_INVALID = Double.NaN;
        public const double _BrakingForce_INVALID = Double.NaN;
        public const double _CutOffVoltage_INVALID = Double.NaN;
        public const int _OverCurrentLimit_INVALID = -1;
        public const double _Frequency_INVALID = Double.NaN;
        public const int _StarterTime_INVALID = -1;
        public const int _FailSafeTimeout_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YMotor _func;
        // property cache
        protected int _motorStatus = _MotorStatus_INVALID;
        protected double _cutOffVoltage = _CutOffVoltage_INVALID;
        protected int _overCurrentLimit = _OverCurrentLimit_INVALID;
        protected double _frequency = _Frequency_INVALID;
        protected int _starterTime = _StarterTime_INVALID;
        protected int _failSafeTimeout = _FailSafeTimeout_INVALID;
        //--- (end of YMotor definitions)

        //--- (YMotor implementation)
        internal YMotorProxy(YMotor hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Motor " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YMotor) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YMotor hwd = YMotor.FindMotor(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YMotor hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Motor callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Motor available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YMotor.FindMotor</c> to obtain an object that can control the
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
            YMotor it = YMotor.FirstMotor();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextMotor();
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
            _cutOffVoltage = _func.get_cutOffVoltage();
            _overCurrentLimit = _func.get_overCurrentLimit();
            _frequency = _func.get_frequency();
            _starterTime = _func.get_starterTime();
            _failSafeTimeout = _func.get_failSafeTimeout();
        }

        // property with cached value for instant access (advertised value)
        public int MotorStatus
        {
            get
            {
                if (_func == null) return _MotorStatus_INVALID;
                return (_online ? _motorStatus : _MotorStatus_INVALID);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "IDLE") _motorStatus = 1;
            if (value == "BRAKE") _motorStatus = 2;
            if (value == "FORWD") _motorStatus = 3;
            if (value == "BACKWD") _motorStatus = 4;
            if (value == "LOVOLT") _motorStatus = 5;
            if (value == "HICURR") _motorStatus = 6;
            if (value == "HIHEAT") _motorStatus = 7;
            if (value == "FAILSF") _motorStatus = 8;
        }

        /**
         * <summary>
         *   Return the controller state.
         * <para>
         *   Possible states are:
         *   IDLE   when the motor is stopped/in free wheel, ready to start;
         *   FORWD  when the controller is driving the motor forward;
         *   BACKWD when the controller is driving the motor backward;
         *   BRAKE  when the controller is braking;
         *   LOVOLT when the controller has detected a low voltage condition;
         *   HICURR when the controller has detected an over current condition;
         *   HIHEAT when the controller has detected an overheat condition;
         *   FAILSF when the controller switched on the failsafe security.
         * </para>
         * <para>
         *   When an error condition occurred (LOVOLT, HICURR, HIHEAT, FAILSF), the controller
         *   status must be explicitly reset using the <c>resetStatus</c> function.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>motor._Motorstatus_IDLE</c>, <c>motor._Motorstatus_BRAKE</c>,
         *   <c>motor._Motorstatus_FORWD</c>, <c>motor._Motorstatus_BACKWD</c>,
         *   <c>motor._Motorstatus_LOVOLT</c>, <c>motor._Motorstatus_HICURR</c>,
         *   <c>motor._Motorstatus_HIHEAT</c> and <c>motor._Motorstatus_FAILSF</c>
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>motor._Motorstatus_INVALID</c>.
         * </para>
         */
        public int get_motorStatus()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_motorStatus()+1;
        }

        /**
         * <summary>
         *   Changes immediately the power sent to the motor.
         * <para>
         *   The value is a percentage between -100%
         *   to 100%. If you want go easy on your mechanics and avoid excessive current consumption,
         *   try to avoid brutal power changes. For example, immediate transition from forward full power
         *   to reverse full power is a very bad idea. Each time the driving power is modified, the
         *   braking power is set to zero.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to immediately the power sent to the motor
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
        public int set_drivingForce(double newval)
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_drivingForce(newval);
        }


        /**
         * <summary>
         *   Returns the power sent to the motor, as a percentage between -100% and +100%.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the power sent to the motor, as a percentage between -100% and +100%
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>motor._Drivingforce_INVALID</c>.
         * </para>
         */
        public double get_drivingForce()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_drivingForce();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Changes immediately the braking force applied to the motor (in percents).
         * <para>
         *   The value 0 corresponds to no braking (free wheel). When the braking force
         *   is changed, the driving power is set to zero. The value is a percentage.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to immediately the braking force applied to the motor (in percents)
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
        public int set_brakingForce(double newval)
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_brakingForce(newval);
        }


        /**
         * <summary>
         *   Returns the braking force applied to the motor, as a percentage.
         * <para>
         *   The value 0 corresponds to no braking (free wheel).
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the braking force applied to the motor, as a percentage
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>motor._Brakingforce_INVALID</c>.
         * </para>
         */
        public double get_brakingForce()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_brakingForce();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Changes the threshold voltage under which the controller automatically switches to error state
         *   and prevents further current draw.
         * <para>
         *   This setting prevent damage to a battery that can
         *   occur when drawing current from an "empty" battery.
         *   Note that whatever the cutoff threshold, the controller switches to undervoltage
         *   error state if the power supply goes under 3V, even for a very brief time.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the threshold voltage under which the controller
         *   automatically switches to error state
         *   and prevents further current draw
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
        public int set_cutOffVoltage(double newval)
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_cutOffVoltage(newval);
        }


        // property with cached value for instant access (configuration)
        public double CutOffVoltage
        {
            get
            {
                if (_func == null) return _CutOffVoltage_INVALID;
                return (_online ? _cutOffVoltage : _CutOffVoltage_INVALID);
            }
            set
            {
                setprop_cutOffVoltage(value);
            }
        }

        // private helper for magic property
        private void setprop_cutOffVoltage(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _cutOffVoltage) return;
            _func.set_cutOffVoltage(newval);
            _cutOffVoltage = newval;
        }

        /**
         * <summary>
         *   Returns the threshold voltage under which the controller automatically switches to error state
         *   and prevents further current draw.
         * <para>
         *   This setting prevents damage to a battery that can
         *   occur when drawing current from an "empty" battery.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the threshold voltage under which the controller
         *   automatically switches to error state
         *   and prevents further current draw
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>motor._Cutoffvoltage_INVALID</c>.
         * </para>
         */
        public double get_cutOffVoltage()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_cutOffVoltage();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the current threshold (in mA) above which the controller automatically
         *   switches to error state.
         * <para>
         *   A zero value means that there is no limit.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current threshold (in mA) above which the controller automatically
         *   switches to error state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>motor._Overcurrentlimit_INVALID</c>.
         * </para>
         */
        public int get_overCurrentLimit()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_overCurrentLimit();
            if (res == YAPI.INVALID_INT) res = _OverCurrentLimit_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the current threshold (in mA) above which the controller automatically
         *   switches to error state.
         * <para>
         *   A zero value means that there is no limit. Note that whatever the
         *   current limit is, the controller switches to OVERCURRENT status if the current
         *   goes above 32A, even for a very brief time. Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the current threshold (in mA) above which the controller automatically
         *   switches to error state
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
        public int set_overCurrentLimit(int newval)
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _OverCurrentLimit_INVALID) return YAPI.SUCCESS;
            return _func.set_overCurrentLimit(newval);
        }


        // property with cached value for instant access (configuration)
        public int OverCurrentLimit
        {
            get
            {
                if (_func == null) return _OverCurrentLimit_INVALID;
                return (_online ? _overCurrentLimit : _OverCurrentLimit_INVALID);
            }
            set
            {
                setprop_overCurrentLimit(value);
            }
        }

        // private helper for magic property
        private void setprop_overCurrentLimit(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _OverCurrentLimit_INVALID) return;
            if (newval == _overCurrentLimit) return;
            _func.set_overCurrentLimit(newval);
            _overCurrentLimit = newval;
        }

        /**
         * <summary>
         *   Changes the PWM frequency used to control the motor.
         * <para>
         *   Low frequency is usually
         *   more efficient and may help the motor to start, but an audible noise might be
         *   generated. A higher frequency reduces the noise, but more energy is converted
         *   into heat. Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the PWM frequency used to control the motor
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
        public int set_frequency(double newval)
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_frequency(newval);
        }


        // property with cached value for instant access (configuration)
        public double Frequency
        {
            get
            {
                if (_func == null) return _Frequency_INVALID;
                return (_online ? _frequency : _Frequency_INVALID);
            }
            set
            {
                setprop_frequency(value);
            }
        }

        // private helper for magic property
        private void setprop_frequency(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _frequency) return;
            _func.set_frequency(newval);
            _frequency = newval;
        }

        /**
         * <summary>
         *   Returns the PWM frequency used to control the motor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the PWM frequency used to control the motor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>motor._Frequency_INVALID</c>.
         * </para>
         */
        public double get_frequency()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_frequency();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the duration (in ms) during which the motor is driven at low frequency to help
         *   it start up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the duration (in ms) during which the motor is driven at low frequency to help
         *   it start up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>motor._Startertime_INVALID</c>.
         * </para>
         */
        public int get_starterTime()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_starterTime();
            if (res == YAPI.INVALID_INT) res = _StarterTime_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the duration (in ms) during which the motor is driven at low frequency to help
         *   it start up.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the duration (in ms) during which the motor is driven at low frequency to help
         *   it start up
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
        public int set_starterTime(int newval)
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _StarterTime_INVALID) return YAPI.SUCCESS;
            return _func.set_starterTime(newval);
        }


        // property with cached value for instant access (configuration)
        public int StarterTime
        {
            get
            {
                if (_func == null) return _StarterTime_INVALID;
                return (_online ? _starterTime : _StarterTime_INVALID);
            }
            set
            {
                setprop_starterTime(value);
            }
        }

        // private helper for magic property
        private void setprop_starterTime(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _StarterTime_INVALID) return;
            if (newval == _starterTime) return;
            _func.set_starterTime(newval);
            _starterTime = newval;
        }

        /**
         * <summary>
         *   Returns the delay in milliseconds allowed for the controller to run autonomously without
         *   receiving any instruction from the control process.
         * <para>
         *   When this delay has elapsed,
         *   the controller automatically stops the motor and switches to FAILSAFE error.
         *   Failsafe security is disabled when the value is zero.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the delay in milliseconds allowed for the controller to run autonomously without
         *   receiving any instruction from the control process
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>motor._Failsafetimeout_INVALID</c>.
         * </para>
         */
        public int get_failSafeTimeout()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_failSafeTimeout();
            if (res == YAPI.INVALID_INT) res = _FailSafeTimeout_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the delay in milliseconds allowed for the controller to run autonomously without
         *   receiving any instruction from the control process.
         * <para>
         *   When this delay has elapsed,
         *   the controller automatically stops the motor and switches to FAILSAFE error.
         *   Failsafe security is disabled when the value is zero.
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the delay in milliseconds allowed for the controller to run autonomously without
         *   receiving any instruction from the control process
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
        public int set_failSafeTimeout(int newval)
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _FailSafeTimeout_INVALID) return YAPI.SUCCESS;
            return _func.set_failSafeTimeout(newval);
        }


        // property with cached value for instant access (configuration)
        public int FailSafeTimeout
        {
            get
            {
                if (_func == null) return _FailSafeTimeout_INVALID;
                return (_online ? _failSafeTimeout : _FailSafeTimeout_INVALID);
            }
            set
            {
                setprop_failSafeTimeout(value);
            }
        }

        // private helper for magic property
        private void setprop_failSafeTimeout(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _FailSafeTimeout_INVALID) return;
            if (newval == _failSafeTimeout) return;
            _func.set_failSafeTimeout(newval);
            _failSafeTimeout = newval;
        }

        /**
         * <summary>
         *   Rearms the controller failsafe timer.
         * <para>
         *   When the motor is running and the failsafe feature
         *   is active, this function should be called periodically to prove that the control process
         *   is running properly. Otherwise, the motor is automatically stopped after the specified
         *   timeout. Calling a motor <i>set</i> function implicitly rearms the failsafe timer.
         * </para>
         * </summary>
         */
        public virtual int keepALive()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.keepALive();
        }

        /**
         * <summary>
         *   Reset the controller state to IDLE.
         * <para>
         *   This function must be invoked explicitly
         *   after any error condition is signaled.
         * </para>
         * </summary>
         */
        public virtual int resetStatus()
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.resetStatus();
        }

        /**
         * <summary>
         *   Changes progressively the power sent to the motor for a specific duration.
         * <para>
         * </para>
         * </summary>
         * <param name="targetPower">
         *   desired motor power, in percents (between -100% and +100%)
         * </param>
         * <param name="delay">
         *   duration (in ms) of the transition
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int drivingForceMove(double targetPower, int delay)
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.drivingForceMove(targetPower, delay);
        }

        /**
         * <summary>
         *   Changes progressively the braking force applied to the motor for a specific duration.
         * <para>
         * </para>
         * </summary>
         * <param name="targetPower">
         *   desired braking force, in percents
         * </param>
         * <param name="delay">
         *   duration (in ms) of the transition
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int brakingForceMove(double targetPower, int delay)
        {
            if (_func == null)
            {
                string msg = "No Motor connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.brakingForceMove(targetPower, delay);
        }
    }
    //--- (end of YMotor implementation)
}

