/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YStepperMotorProxy, the Proxy API for StepperMotor
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
    //--- (YStepperMotor class start)
    static public partial class YoctoProxyManager
    {
        public static YStepperMotorProxy FindStepperMotor(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YStepperMotor func = null;
            YStepperMotorProxy res = (YStepperMotorProxy)YFunctionProxy.FindSimilarUnknownFunction("YStepperMotorProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YStepperMotorProxy)YFunctionProxy.FindSimilarKnownFunction("YStepperMotorProxy");
                if (res != null) return res;
                func = YStepperMotor.FirstStepperMotor();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YStepperMotorProxy)func.get_userData();
                    }
                }
            } else {
                func = YStepperMotor.FindStepperMotor(name);
                if (func.get_userData() != null) {
                    return (YStepperMotorProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YStepperMotorProxy(func, name);
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
 *   The <c>YStepperMotor</c> class allows you to drive a stepper motor.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YStepperMotorProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a stepper motor for a given identifier.
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
         *   This function does not require that the stepper motor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YStepperMotor.isOnline()</c> to test if the stepper motor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a stepper motor by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the stepper motor, for instance
         *   <c>MyDevice.stepperMotor1</c>.
         * </param>
         * <returns>
         *   a <c>YStepperMotor</c> object allowing you to drive the stepper motor.
         * </returns>
         */
        public static YStepperMotorProxy FindStepperMotor(string func)
        {
            return YoctoProxyManager.FindStepperMotor(func);
        }
        //--- (end of YStepperMotor class start)
        //--- (YStepperMotor definitions)
        public const int _MotorState_INVALID = 0;
        public const int _MotorState_ABSENT = 1;
        public const int _MotorState_ALERT = 2;
        public const int _MotorState_HI_Z = 3;
        public const int _MotorState_STOP = 4;
        public const int _MotorState_RUN = 5;
        public const int _MotorState_BATCH = 6;
        public const int _Diags_INVALID = -1;
        public const double _StepPos_INVALID = Double.NaN;
        public const double _Speed_INVALID = Double.NaN;
        public const double _PullinSpeed_INVALID = Double.NaN;
        public const double _MaxAccel_INVALID = Double.NaN;
        public const double _MaxSpeed_INVALID = Double.NaN;
        public const int _Stepping_INVALID = 0;
        public const int _Stepping_MICROSTEP16 = 1;
        public const int _Stepping_MICROSTEP8 = 2;
        public const int _Stepping_MICROSTEP4 = 3;
        public const int _Stepping_HALFSTEP = 4;
        public const int _Stepping_FULLSTEP = 5;
        public const int _Overcurrent_INVALID = -1;
        public const int _TCurrStop_INVALID = -1;
        public const int _TCurrRun_INVALID = -1;
        public const string _AlertMode_INVALID = YAPI.INVALID_STRING;
        public const string _AuxMode_INVALID = YAPI.INVALID_STRING;
        public const int _AuxSignal_INVALID = YAPI.INVALID_INT;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YStepperMotor _func;
        // property cache
        //--- (end of YStepperMotor definitions)

        //--- (YStepperMotor implementation)
        internal YStepperMotorProxy(YStepperMotor hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("StepperMotor " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YStepperMotor) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YStepperMotor hwd = YStepperMotor.FindStepperMotor(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YStepperMotor hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering StepperMotor callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type StepperMotor available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YStepperMotor.FindStepperMotor</c> to obtain an object that can control the
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
            YStepperMotor it = YStepperMotor.FirstStepperMotor();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextStepperMotor();
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
         *   Returns the motor working state.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YStepperMotor.MOTORSTATE_ABSENT</c>, <c>YStepperMotor.MOTORSTATE_ALERT</c>,
         *   <c>YStepperMotor.MOTORSTATE_HI_Z</c>, <c>YStepperMotor.MOTORSTATE_STOP</c>,
         *   <c>YStepperMotor.MOTORSTATE_RUN</c> and <c>YStepperMotor.MOTORSTATE_BATCH</c> corresponding to the
         *   motor working state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.MOTORSTATE_INVALID</c>.
         * </para>
         */
        public int get_motorState()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_motorState()+1;
        }

        /**
         * <summary>
         *   Returns the stepper motor controller diagnostics, as a bitmap.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the stepper motor controller diagnostics, as a bitmap
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.DIAGS_INVALID</c>.
         * </para>
         */
        public int get_diags()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            res = _func.get_diags();
            if (res == YAPI.INVALID_INT) {
                res = _Diags_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the current logical motor position, measured in steps.
         * <para>
         *   This command does not cause any motor move, as its purpose is only to set up
         *   the origin of the position counter. The fractional part of the position,
         *   that corresponds to the physical position of the rotor, is not changed.
         *   To trigger a motor move, use methods <c>moveTo()</c> or <c>moveRel()</c>
         *   instead.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the current logical motor position, measured in steps
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
        public int set_stepPos(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            if (newval == _StepPos_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_stepPos(newval);
        }

        /**
         * <summary>
         *   Returns the current logical motor position, measured in steps.
         * <para>
         *   The value may include a fractional part when micro-stepping is in use.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the current logical motor position, measured in steps
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.STEPPOS_INVALID</c>.
         * </para>
         */
        public double get_stepPos()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            res = _func.get_stepPos();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns current motor speed, measured in steps per second.
         * <para>
         *   To change speed, use method <c>changeSpeed()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to current motor speed, measured in steps per second
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.SPEED_INVALID</c>.
         * </para>
         */
        public double get_speed()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            res = _func.get_speed();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the motor speed immediately reachable from stop state, measured in steps per second.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the motor speed immediately reachable from stop state,
         *   measured in steps per second
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
        public int set_pullinSpeed(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            if (newval == _PullinSpeed_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_pullinSpeed(newval);
        }

        /**
         * <summary>
         *   Returns the motor speed immediately reachable from stop state, measured in steps per second.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the motor speed immediately reachable from stop state,
         *   measured in steps per second
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.PULLINSPEED_INVALID</c>.
         * </para>
         */
        public double get_pullinSpeed()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            res = _func.get_pullinSpeed();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the maximal motor acceleration, measured in steps per second^2.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the maximal motor acceleration, measured in steps per second^2
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
        public int set_maxAccel(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            if (newval == _MaxAccel_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_maxAccel(newval);
        }

        /**
         * <summary>
         *   Returns the maximal motor acceleration, measured in steps per second^2.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the maximal motor acceleration, measured in steps per second^2
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.MAXACCEL_INVALID</c>.
         * </para>
         */
        public double get_maxAccel()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            res = _func.get_maxAccel();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the maximal motor speed, measured in steps per second.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the maximal motor speed, measured in steps per second
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
        public int set_maxSpeed(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            if (newval == _MaxSpeed_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_maxSpeed(newval);
        }

        /**
         * <summary>
         *   Returns the maximal motor speed, measured in steps per second.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the maximal motor speed, measured in steps per second
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.MAXSPEED_INVALID</c>.
         * </para>
         */
        public double get_maxSpeed()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            res = _func.get_maxSpeed();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the stepping mode used to drive the motor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YStepperMotor.STEPPING_MICROSTEP16</c>, <c>YStepperMotor.STEPPING_MICROSTEP8</c>,
         *   <c>YStepperMotor.STEPPING_MICROSTEP4</c>, <c>YStepperMotor.STEPPING_HALFSTEP</c> and
         *   <c>YStepperMotor.STEPPING_FULLSTEP</c> corresponding to the stepping mode used to drive the motor
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.STEPPING_INVALID</c>.
         * </para>
         */
        public int get_stepping()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_stepping()+1;
        }

        /**
         * <summary>
         *   Changes the stepping mode used to drive the motor.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YStepperMotor.STEPPING_MICROSTEP16</c>, <c>YStepperMotor.STEPPING_MICROSTEP8</c>,
         *   <c>YStepperMotor.STEPPING_MICROSTEP4</c>, <c>YStepperMotor.STEPPING_HALFSTEP</c> and
         *   <c>YStepperMotor.STEPPING_FULLSTEP</c> corresponding to the stepping mode used to drive the motor
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
        public int set_stepping(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            if (newval == _Stepping_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_stepping(newval-1);
        }

        /**
         * <summary>
         *   Returns the overcurrent alert and emergency stop threshold, measured in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the overcurrent alert and emergency stop threshold, measured in mA
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.OVERCURRENT_INVALID</c>.
         * </para>
         */
        public int get_overcurrent()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            res = _func.get_overcurrent();
            if (res == YAPI.INVALID_INT) {
                res = _Overcurrent_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the overcurrent alert and emergency stop threshold, measured in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the overcurrent alert and emergency stop threshold, measured in mA
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
        public int set_overcurrent(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            if (newval == _Overcurrent_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_overcurrent(newval);
        }

        /**
         * <summary>
         *   Returns the torque regulation current when the motor is stopped, measured in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the torque regulation current when the motor is stopped, measured in mA
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.TCURRSTOP_INVALID</c>.
         * </para>
         */
        public int get_tCurrStop()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            res = _func.get_tCurrStop();
            if (res == YAPI.INVALID_INT) {
                res = _TCurrStop_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the torque regulation current when the motor is stopped, measured in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the torque regulation current when the motor is stopped, measured in mA
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
        public int set_tCurrStop(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            if (newval == _TCurrStop_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_tCurrStop(newval);
        }

        /**
         * <summary>
         *   Returns the torque regulation current when the motor is running, measured in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the torque regulation current when the motor is running, measured in mA
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.TCURRRUN_INVALID</c>.
         * </para>
         */
        public int get_tCurrRun()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            res = _func.get_tCurrRun();
            if (res == YAPI.INVALID_INT) {
                res = _TCurrRun_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the torque regulation current when the motor is running, measured in mA.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the torque regulation current when the motor is running, measured in mA
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
        public int set_tCurrRun(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            if (newval == _TCurrRun_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_tCurrRun(newval);
        }

        /**
         * <summary>
         *   Returns the current value of the signal generated on the auxiliary output.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current value of the signal generated on the auxiliary output
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YStepperMotor.AUXSIGNAL_INVALID</c>.
         * </para>
         */
        public int get_auxSignal()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.get_auxSignal();
        }

        /**
         * <summary>
         *   Changes the value of the signal generated on the auxiliary output.
         * <para>
         *   Acceptable values depend on the auxiliary output signal type configured.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the value of the signal generated on the auxiliary output
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
        public int set_auxSignal(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            if (newval == _AuxSignal_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_auxSignal(newval);
        }

        /**
         * <summary>
         *   Reinitialize the controller and clear all alert flags.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int reset()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.reset();
        }

        /**
         * <summary>
         *   Starts the motor backward at the specified speed, to search for the motor home position.
         * <para>
         * </para>
         * </summary>
         * <param name="speed">
         *   desired speed, in steps per second.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int findHomePosition(double speed)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.findHomePosition(speed);
        }

        /**
         * <summary>
         *   Starts the motor at a given speed.
         * <para>
         *   The time needed to reach the requested speed
         *   will depend on the acceleration parameters configured for the motor.
         * </para>
         * </summary>
         * <param name="speed">
         *   desired speed, in steps per second. The minimal non-zero speed
         *   is 0.001 pulse per second.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int changeSpeed(double speed)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.changeSpeed(speed);
        }

        /**
         * <summary>
         *   Starts the motor to reach a given absolute position.
         * <para>
         *   The time needed to reach the requested
         *   position will depend on the acceleration and max speed parameters configured for
         *   the motor.
         * </para>
         * </summary>
         * <param name="absPos">
         *   absolute position, measured in steps from the origin.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int moveTo(double absPos)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.moveTo(absPos);
        }

        /**
         * <summary>
         *   Starts the motor to reach a given relative position.
         * <para>
         *   The time needed to reach the requested
         *   position will depend on the acceleration and max speed parameters configured for
         *   the motor.
         * </para>
         * </summary>
         * <param name="relPos">
         *   relative position, measured in steps from the current position.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int moveRel(double relPos)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.moveRel(relPos);
        }

        /**
         * <summary>
         *   Starts the motor to reach a given relative position, keeping the speed under the
         *   specified limit.
         * <para>
         *   The time needed to reach the requested position will depend on
         *   the acceleration parameters configured for the motor.
         * </para>
         * </summary>
         * <param name="relPos">
         *   relative position, measured in steps from the current position.
         * </param>
         * <param name="maxSpeed">
         *   limit speed, in steps per second.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int moveRelSlow(double relPos, double maxSpeed)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.moveRelSlow(relPos, maxSpeed);
        }

        /**
         * <summary>
         *   Keep the motor in the same state for the specified amount of time, before processing next command.
         * <para>
         * </para>
         * </summary>
         * <param name="waitMs">
         *   wait time, specified in milliseconds.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int pause(int waitMs)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.pause(waitMs);
        }

        /**
         * <summary>
         *   Stops the motor with an emergency alert, without taking any additional precaution.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int emergencyStop()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.emergencyStop();
        }

        /**
         * <summary>
         *   Move one step in the direction opposite the direction set when the most recent alert was raised.
         * <para>
         *   The move occurs even if the system is still in alert mode (end switch depressed). Caution.
         *   use this function with great care as it may cause mechanical damages !
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int alertStepOut()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.alertStepOut();
        }

        /**
         * <summary>
         *   Move one single step in the selected direction without regards to end switches.
         * <para>
         *   The move occurs even if the system is still in alert mode (end switch depressed). Caution.
         *   use this function with great care as it may cause mechanical damages !
         * </para>
         * </summary>
         * <param name="dir">
         *   Value +1 or -1, according to the desired direction of the move
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int alertStepDir(int dir)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.alertStepDir(dir);
        }

        /**
         * <summary>
         *   Stops the motor smoothly as soon as possible, without waiting for ongoing move completion.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int abortAndBrake()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.abortAndBrake();
        }

        /**
         * <summary>
         *   Turn the controller into Hi-Z mode immediately, without waiting for ongoing move completion.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int abortAndHiZ()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No StepperMotor connected");
            }
            return _func.abortAndHiZ();
        }
    }
    //--- (end of YStepperMotor implementation)
}

