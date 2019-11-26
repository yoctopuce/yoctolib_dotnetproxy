/*********************************************************************
 *
 *  $Id: yocto_servo_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YServoProxy, the Proxy API for Servo
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
    //--- (YServo class start)
    static public partial class YoctoProxyManager
    {
        public static YServoProxy FindServo(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YServo func = null;
            YServoProxy res = (YServoProxy)YFunctionProxy.FindSimilarUnknownFunction("YServoProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YServoProxy)YFunctionProxy.FindSimilarKnownFunction("YServoProxy");
                if (res != null) return res;
                func = YServo.FirstServo();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YServoProxy)func.get_userData();
                    }
                }
            } else {
                func = YServo.FindServo(name);
                if (func.get_userData() != null) {
                    return (YServoProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YServoProxy(func, name);
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
 *   The YServo class is designed to drive remote-control servo motors
 *   outputs, for instance using a Yocto-Servo.
 * <para>
 *   This class allows you not only to move
 *   a servo to a given position, but also to specify the time interval
 *   in which the move should be performed. This makes it possible to
 *   synchronize two servos involved in a same move.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YServoProxy : YFunctionProxy
    {
        //--- (end of YServo class start)
        //--- (YServo definitions)
        public const int _Position_INVALID = YAPI.INVALID_INT;
        public const int _Enabled_INVALID = 0;
        public const int _Enabled_FALSE = 1;
        public const int _Enabled_TRUE = 2;
        public const int _Range_INVALID = -1;
        public const int _Neutral_INVALID = -1;
        public const int _PositionAtPowerOn_INVALID = YAPI.INVALID_INT;
        public const int _EnabledAtPowerOn_INVALID = 0;
        public const int _EnabledAtPowerOn_FALSE = 1;
        public const int _EnabledAtPowerOn_TRUE = 2;

        // reference to real YoctoAPI object
        protected new YServo _func;
        // property cache
        protected int _position = _Position_INVALID;
        protected int _range = _Range_INVALID;
        protected int _neutral = _Neutral_INVALID;
        protected int _positionAtPowerOn = _PositionAtPowerOn_INVALID;
        protected int _enabledAtPowerOn = _EnabledAtPowerOn_INVALID;
        protected int _enabled = _Enabled_INVALID;
        //--- (end of YServo definitions)

        //--- (YServo implementation)
        internal YServoProxy(YServo hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Servo " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YServo) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YServo hwd = YServo.FindServo(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YServo hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Servo callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YServo it = YServo.FirstServo();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextServo();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _enabled = _func.get_enabled()+1;
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _range = _func.get_range();
            _neutral = _func.get_neutral();
            _positionAtPowerOn = _func.get_positionAtPowerOn();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _enabledAtPowerOn = _func.get_enabledAtPowerOn()+1;
        }

        // property with cached value for instant access (advertised value)
        public int Position
        {
            get
            {
                if (_func == null) return _Position_INVALID;
                return (_online ? _position : _Position_INVALID);
            }
            set
            {
                setprop_position(value);
            }
        }

        // property with cached value for instant access (derived from advertised value)
        public int Enabled
        {
            get
            {
                if (_func == null) return _Enabled_INVALID;
                return (_online ? _enabled : _Enabled_INVALID);
            }
            set
            {
                setprop_enabled(value);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            if (value == "OFF") _enabled = _Enabled_FALSE;
            else _enabled = _Enabled_TRUE;
            if (_enabled == _Enabled_TRUE) // then parse numeric value
            Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture,out _position);
        }

        /**
         * <summary>
         *   Returns the current servo position.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current servo position
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YServo.POSITION_INVALID</c>.
         * </para>
         */
        public int get_position()
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_position();
        }

        /**
         * <summary>
         *   Changes immediately the servo driving position.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to immediately the servo driving position
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
        public int set_position(int newval)
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Position_INVALID) return YAPI.SUCCESS;
            return _func.set_position(newval);
        }


        // private helper for magic property
        private void setprop_position(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Position_INVALID) return;
            if (newval == _position) return;
            _func.set_position(newval);
            _position = newval;
        }

        /**
         * <summary>
         *   Returns the state of the servos.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YServo.ENABLED_FALSE</c> or <c>YServo.ENABLED_TRUE</c>, according to the state of the servos
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YServo.ENABLED_INVALID</c>.
         * </para>
         */
        public int get_enabled()
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Stops or starts the servo.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YServo.ENABLED_FALSE</c> or <c>YServo.ENABLED_TRUE</c>
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
        public int set_enabled(int newval)
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Enabled_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enabled(newval-1);
        }


        // private helper for magic property
        private void setprop_enabled(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Enabled_INVALID) return;
            if (newval == _enabled) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_enabled(newval-1);
            _enabled = newval;
        }

        /**
         * <summary>
         *   Returns the current range of use of the servo.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current range of use of the servo
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YServo.RANGE_INVALID</c>.
         * </para>
         */
        public int get_range()
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_range();
            if (res == YAPI.INVALID_INT) res = _Range_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the range of use of the servo, specified in per cents.
         * <para>
         *   A range of 100% corresponds to a standard control signal, that varies
         *   from 1 [ms] to 2 [ms], When using a servo that supports a double range,
         *   from 0.5 [ms] to 2.5 [ms], you can select a range of 200%.
         *   Be aware that using a range higher than what is supported by the servo
         *   is likely to damage the servo. Remember to call the matching module
         *   <c>saveToFlash()</c> method, otherwise this call will have no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the range of use of the servo, specified in per cents
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
        public int set_range(int newval)
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Range_INVALID) return YAPI.SUCCESS;
            return _func.set_range(newval);
        }


        // property with cached value for instant access (configuration)
        public int Range
        {
            get
            {
                if (_func == null) return _Range_INVALID;
                return (_online ? _range : _Range_INVALID);
            }
            set
            {
                setprop_range(value);
            }
        }

        // private helper for magic property
        private void setprop_range(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Range_INVALID) return;
            if (newval == _range) return;
            _func.set_range(newval);
            _range = newval;
        }

        /**
         * <summary>
         *   Returns the duration in microseconds of a neutral pulse for the servo.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the duration in microseconds of a neutral pulse for the servo
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YServo.NEUTRAL_INVALID</c>.
         * </para>
         */
        public int get_neutral()
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_neutral();
            if (res == YAPI.INVALID_INT) res = _Neutral_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the duration of the pulse corresponding to the neutral position of the servo.
         * <para>
         *   The duration is specified in microseconds, and the standard value is 1500 [us].
         *   This setting makes it possible to shift the range of use of the servo.
         *   Be aware that using a range higher than what is supported by the servo is
         *   likely to damage the servo. Remember to call the matching module
         *   <c>saveToFlash()</c> method, otherwise this call will have no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the duration of the pulse corresponding to the neutral position of the servo
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
        public int set_neutral(int newval)
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Neutral_INVALID) return YAPI.SUCCESS;
            return _func.set_neutral(newval);
        }


        // property with cached value for instant access (configuration)
        public int Neutral
        {
            get
            {
                if (_func == null) return _Neutral_INVALID;
                return (_online ? _neutral : _Neutral_INVALID);
            }
            set
            {
                setprop_neutral(value);
            }
        }

        // private helper for magic property
        private void setprop_neutral(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Neutral_INVALID) return;
            if (newval == _neutral) return;
            _func.set_neutral(newval);
            _neutral = newval;
        }

        /**
         * <summary>
         *   Performs a smooth move at constant speed toward a given position.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="target">
         *   new position at the end of the move
         * </param>
         * <param name="ms_duration">
         *   total duration of the move, in milliseconds
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
        public int move(int target,int ms_duration)
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.move(target, ms_duration);
        }

        /**
         * <summary>
         *   Returns the servo position at device power up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the servo position at device power up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YServo.POSITIONATPOWERON_INVALID</c>.
         * </para>
         */
        public int get_positionAtPowerOn()
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_positionAtPowerOn();
        }

        /**
         * <summary>
         *   Configure the servo position at device power up.
         * <para>
         *   Remember to call the matching
         *   module <c>saveToFlash()</c> method, otherwise this call will have no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer
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
        public int set_positionAtPowerOn(int newval)
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PositionAtPowerOn_INVALID) return YAPI.SUCCESS;
            return _func.set_positionAtPowerOn(newval);
        }


        // property with cached value for instant access (configuration)
        public int PositionAtPowerOn
        {
            get
            {
                if (_func == null) return _PositionAtPowerOn_INVALID;
                return (_online ? _positionAtPowerOn : _PositionAtPowerOn_INVALID);
            }
            set
            {
                setprop_positionAtPowerOn(value);
            }
        }

        // private helper for magic property
        private void setprop_positionAtPowerOn(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _PositionAtPowerOn_INVALID) return;
            if (newval == _positionAtPowerOn) return;
            _func.set_positionAtPowerOn(newval);
            _positionAtPowerOn = newval;
        }

        /**
         * <summary>
         *   Returns the servo signal generator state at power up.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YServo.ENABLEDATPOWERON_FALSE</c> or <c>YServo.ENABLEDATPOWERON_TRUE</c>, according to
         *   the servo signal generator state at power up
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YServo.ENABLEDATPOWERON_INVALID</c>.
         * </para>
         */
        public int get_enabledAtPowerOn()
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabledAtPowerOn()+1;
        }

        /**
         * <summary>
         *   Configure the servo signal generator state at power up.
         * <para>
         *   Remember to call the matching module <c>saveToFlash()</c>
         *   method, otherwise this call will have no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YServo.ENABLEDATPOWERON_FALSE</c> or <c>YServo.ENABLEDATPOWERON_TRUE</c>
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
        public int set_enabledAtPowerOn(int newval)
        {
            if (_func == null)
            {
                string msg = "No Servo connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _EnabledAtPowerOn_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enabledAtPowerOn(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int EnabledAtPowerOn
        {
            get
            {
                if (_func == null) return _EnabledAtPowerOn_INVALID;
                return (_online ? _enabledAtPowerOn : _EnabledAtPowerOn_INVALID);
            }
            set
            {
                setprop_enabledAtPowerOn(value);
            }
        }

        // private helper for magic property
        private void setprop_enabledAtPowerOn(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _EnabledAtPowerOn_INVALID) return;
            if (newval == _enabledAtPowerOn) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_enabledAtPowerOn(newval-1);
            _enabledAtPowerOn = newval;
        }
    }
    //--- (end of YServo implementation)
}

