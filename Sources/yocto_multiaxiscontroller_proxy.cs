/*********************************************************************
 *
 *  $Id: yocto_multiaxiscontroller_proxy.cs 40190 2020-04-29 13:16:45Z mvuilleu $
 *
 *  Implements YMultiAxisControllerProxy, the Proxy API for MultiAxisController
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
    //--- (YMultiAxisController class start)
    static public partial class YoctoProxyManager
    {
        public static YMultiAxisControllerProxy FindMultiAxisController(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YMultiAxisController func = null;
            YMultiAxisControllerProxy res = (YMultiAxisControllerProxy)YFunctionProxy.FindSimilarUnknownFunction("YMultiAxisControllerProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YMultiAxisControllerProxy)YFunctionProxy.FindSimilarKnownFunction("YMultiAxisControllerProxy");
                if (res != null) return res;
                func = YMultiAxisController.FirstMultiAxisController();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YMultiAxisControllerProxy)func.get_userData();
                    }
                }
            } else {
                func = YMultiAxisController.FindMultiAxisController(name);
                if (func.get_userData() != null) {
                    return (YMultiAxisControllerProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YMultiAxisControllerProxy(func, name);
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
 *   The <c>YMultiAxisController</c> class allows you to drive multiple stepper motors
 *   synchronously.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YMultiAxisControllerProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a multi-axis controller for a given identifier.
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
         *   This function does not require that the multi-axis controller is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YMultiAxisController.isOnline()</c> to test if the multi-axis controller is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a multi-axis controller by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the multi-axis controller, for instance
         *   <c>MyDevice.multiAxisController</c>.
         * </param>
         * <returns>
         *   a <c>YMultiAxisController</c> object allowing you to drive the multi-axis controller.
         * </returns>
         */
        public static YMultiAxisControllerProxy FindMultiAxisController(string func)
        {
            return YoctoProxyManager.FindMultiAxisController(func);
        }
        //--- (end of YMultiAxisController class start)
        //--- (YMultiAxisController definitions)
        public const int _NAxis_INVALID = -1;
        public const int _GlobalState_INVALID = 0;
        public const int _GlobalState_ABSENT = 1;
        public const int _GlobalState_ALERT = 2;
        public const int _GlobalState_HI_Z = 3;
        public const int _GlobalState_STOP = 4;
        public const int _GlobalState_RUN = 5;
        public const int _GlobalState_BATCH = 6;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YMultiAxisController _func;
        // property cache
        //--- (end of YMultiAxisController definitions)

        //--- (YMultiAxisController implementation)
        internal YMultiAxisControllerProxy(YMultiAxisController hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("MultiAxisController " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YMultiAxisController) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YMultiAxisController hwd = YMultiAxisController.FindMultiAxisController(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YMultiAxisController hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering MultiAxisController callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type MultiAxisController available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YMultiAxisController.FindMultiAxisController</c> to obtain an object that can control the
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
            YMultiAxisController it = YMultiAxisController.FirstMultiAxisController();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextMultiAxisController();
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
         *   Returns the number of synchronized controllers.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of synchronized controllers
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>multiaxiscontroller._Naxis_INVALID</c>.
         * </para>
         */
        public int get_nAxis()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
            }
            res = _func.get_nAxis();
            if (res == YAPI.INVALID_INT) {
                res = _NAxis_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the number of synchronized controllers.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the number of synchronized controllers
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
        public int set_nAxis(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
            }
            if (newval == _NAxis_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_nAxis(newval);
        }

        /**
         * <summary>
         *   Returns the stepper motor set overall state.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>multiaxiscontroller._Globalstate_ABSENT</c>,
         *   <c>multiaxiscontroller._Globalstate_ALERT</c>, <c>multiaxiscontroller._Globalstate_HI_Z</c>,
         *   <c>multiaxiscontroller._Globalstate_STOP</c>, <c>multiaxiscontroller._Globalstate_RUN</c> and
         *   <c>multiaxiscontroller._Globalstate_BATCH</c> corresponding to the stepper motor set overall state
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>multiaxiscontroller._Globalstate_INVALID</c>.
         * </para>
         */
        public int get_globalState()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_globalState()+1;
        }

        /**
         * <summary>
         *   Reinitialize all controllers and clear all alert flags.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int reset()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
            }
            return _func.reset();
        }

        /**
         * <summary>
         *   Starts all motors backward at the specified speeds, to search for the motor home position.
         * <para>
         * </para>
         * </summary>
         * <param name="speed">
         *   desired speed for all axis, in steps per second.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int findHomePosition(double[] speed)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
            }
            return _func.findHomePosition(new List<double>(speed));
        }

        /**
         * <summary>
         *   Starts all motors synchronously to reach a given absolute position.
         * <para>
         *   The time needed to reach the requested position will depend on the lowest
         *   acceleration and max speed parameters configured for all motors.
         *   The final position will be reached on all axis at the same time.
         * </para>
         * </summary>
         * <param name="absPos">
         *   absolute position, measured in steps from each origin.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int moveTo(double[] absPos)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
            }
            return _func.moveTo(new List<double>(absPos));
        }

        /**
         * <summary>
         *   Starts all motors synchronously to reach a given relative position.
         * <para>
         *   The time needed to reach the requested position will depend on the lowest
         *   acceleration and max speed parameters configured for all motors.
         *   The final position will be reached on all axis at the same time.
         * </para>
         * </summary>
         * <param name="relPos">
         *   relative position, measured in steps from the current position.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int moveRel(double[] relPos)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
            }
            return _func.moveRel(new List<double>(relPos));
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int pause(int waitMs)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int emergencyStop()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
            }
            return _func.emergencyStop();
        }

        /**
         * <summary>
         *   Stops the motor smoothly as soon as possible, without waiting for ongoing move completion.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int abortAndBrake()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
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
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int abortAndHiZ()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MultiAxisController connected");
            }
            return _func.abortAndHiZ();
        }
    }
    //--- (end of YMultiAxisController implementation)
}

