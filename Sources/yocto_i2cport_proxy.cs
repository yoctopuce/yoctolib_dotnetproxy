/*********************************************************************
 *
 *  $Id: yocto_i2cport_proxy.cs 40656 2020-05-25 14:13:34Z mvuilleu $
 *
 *  Implements YI2cPortProxy, the Proxy API for I2cPort
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
    //--- (YI2cPort class start)
    static public partial class YoctoProxyManager
    {
        public static YI2cPortProxy FindI2cPort(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YI2cPort func = null;
            YI2cPortProxy res = (YI2cPortProxy)YFunctionProxy.FindSimilarUnknownFunction("YI2cPortProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YI2cPortProxy)YFunctionProxy.FindSimilarKnownFunction("YI2cPortProxy");
                if (res != null) return res;
                func = YI2cPort.FirstI2cPort();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YI2cPortProxy)func.get_userData();
                    }
                }
            } else {
                func = YI2cPort.FindI2cPort(name);
                if (func.get_userData() != null) {
                    return (YI2cPortProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YI2cPortProxy(func, name);
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
 *   The <c>YI2cPort</c> classe allows you to fully drive a Yoctopuce I2C port.
 * <para>
 *   It can be used to send and receive data, and to configure communication
 *   parameters (baud rate, etc).
 *   Note that Yoctopuce I2C ports are not exposed as virtual COM ports.
 *   They are meant to be used in the same way as all Yoctopuce devices.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YI2cPortProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves an I2C port for a given identifier.
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
         *   This function does not require that the I2C port is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YI2cPort.isOnline()</c> to test if the I2C port is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   an I2C port by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the I2C port, for instance
         *   <c>YI2CMK01.i2cPort</c>.
         * </param>
         * <returns>
         *   a <c>YI2cPort</c> object allowing you to drive the I2C port.
         * </returns>
         */
        public static YI2cPortProxy FindI2cPort(string func)
        {
            return YoctoProxyManager.FindI2cPort(func);
        }
        //--- (end of YI2cPort class start)
        //--- (YI2cPort definitions)
        public const int _RxCount_INVALID = -1;
        public const int _TxCount_INVALID = -1;
        public const int _ErrCount_INVALID = -1;
        public const int _RxMsgCount_INVALID = -1;
        public const int _TxMsgCount_INVALID = -1;
        public const string _LastMsg_INVALID = YAPI.INVALID_STRING;
        public const string _CurrentJob_INVALID = YAPI.INVALID_STRING;
        public const string _StartupJob_INVALID = YAPI.INVALID_STRING;
        public const int _JobMaxTask_INVALID = -1;
        public const int _JobMaxSize_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;
        public const string _Protocol_INVALID = YAPI.INVALID_STRING;
        public const int _I2cVoltageLevel_INVALID = 0;
        public const int _I2cVoltageLevel_OFF = 1;
        public const int _I2cVoltageLevel_3V3 = 2;
        public const int _I2cVoltageLevel_1V8 = 3;
        public const string _I2cMode_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YI2cPort _func;
        // property cache
        protected string _startupJob = _StartupJob_INVALID;
        protected int _jobMaxTask = _JobMaxTask_INVALID;
        protected int _jobMaxSize = _JobMaxSize_INVALID;
        protected string _protocol = _Protocol_INVALID;
        protected int _i2cVoltageLevel = _I2cVoltageLevel_INVALID;
        protected string _i2cMode = _I2cMode_INVALID;
        //--- (end of YI2cPort definitions)

        //--- (YI2cPort implementation)
        internal YI2cPortProxy(YI2cPort hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("I2cPort " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YI2cPort) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YI2cPort hwd = YI2cPort.FindI2cPort(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YI2cPort hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering I2cPort callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type I2cPort available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YI2cPort.FindI2cPort</c> to obtain an object that can control the
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
            YI2cPort it = YI2cPort.FirstI2cPort();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextI2cPort();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _jobMaxTask = _func.get_jobMaxTask();
            _jobMaxSize = _func.get_jobMaxSize();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _startupJob = _func.get_startupJob();
            _protocol = _func.get_protocol();
            _i2cVoltageLevel = _func.get_i2cVoltageLevel()+1;
            _i2cMode = _func.get_i2cMode();
        }

        /**
         * <summary>
         *   Returns the total number of bytes received since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of bytes received since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Rxcount_INVALID</c>.
         * </para>
         */
        public int get_rxCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            res = _func.get_rxCount();
            if (res == YAPI.INVALID_INT) {
                res = _RxCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the total number of bytes transmitted since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of bytes transmitted since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Txcount_INVALID</c>.
         * </para>
         */
        public int get_txCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            res = _func.get_txCount();
            if (res == YAPI.INVALID_INT) {
                res = _TxCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the total number of communication errors detected since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of communication errors detected since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Errcount_INVALID</c>.
         * </para>
         */
        public int get_errCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            res = _func.get_errCount();
            if (res == YAPI.INVALID_INT) {
                res = _ErrCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the total number of messages received since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of messages received since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Rxmsgcount_INVALID</c>.
         * </para>
         */
        public int get_rxMsgCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            res = _func.get_rxMsgCount();
            if (res == YAPI.INVALID_INT) {
                res = _RxMsgCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the total number of messages send since last reset.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of messages send since last reset
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Txmsgcount_INVALID</c>.
         * </para>
         */
        public int get_txMsgCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            res = _func.get_txMsgCount();
            if (res == YAPI.INVALID_INT) {
                res = _TxMsgCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the latest message fully received (for Line and Frame protocols).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the latest message fully received (for Line and Frame protocols)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Lastmsg_INVALID</c>.
         * </para>
         */
        public string get_lastMsg()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.get_lastMsg();
        }

        /**
         * <summary>
         *   Returns the name of the job file currently in use.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the name of the job file currently in use
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Currentjob_INVALID</c>.
         * </para>
         */
        public string get_currentJob()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.get_currentJob();
        }

        /**
         * <summary>
         *   Selects a job file to run immediately.
         * <para>
         *   If an empty string is
         *   given as argument, stops running current job file.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string
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
        public int set_currentJob(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            if (newval == _CurrentJob_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_currentJob(newval);
        }

        /**
         * <summary>
         *   Returns the job file to use when the device is powered on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the job file to use when the device is powered on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Startupjob_INVALID</c>.
         * </para>
         */
        public string get_startupJob()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.get_startupJob();
        }

        /**
         * <summary>
         *   Changes the job to use when the device is powered on.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the job to use when the device is powered on
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
        public int set_startupJob(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            if (newval == _StartupJob_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_startupJob(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Job file to use when the device is powered on.</value>
        public string StartupJob
        {
            get
            {
                if (_func == null) {
                    return _StartupJob_INVALID;
                }
                if (_online) {
                    return _startupJob;
                }
                return _StartupJob_INVALID;
            }
            set
            {
                setprop_startupJob(value);
            }
        }

        // private helper for magic property
        private void setprop_startupJob(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _StartupJob_INVALID) {
                return;
            }
            if (newval == _startupJob) {
                return;
            }
            _func.set_startupJob(newval);
            _startupJob = newval;
        }

        /**
         * <summary>
         *   Returns the maximum number of tasks in a job that the device can handle.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum number of tasks in a job that the device can handle
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Jobmaxtask_INVALID</c>.
         * </para>
         */
        public int get_jobMaxTask()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            res = _func.get_jobMaxTask();
            if (res == YAPI.INVALID_INT) {
                res = _JobMaxTask_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Maximum number of tasks in a job that the device can handle.</value>
        public int JobMaxTask
        {
            get
            {
                if (_func == null) {
                    return _JobMaxTask_INVALID;
                }
                if (_online) {
                    return _jobMaxTask;
                }
                return _JobMaxTask_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns maximum size allowed for job files.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to maximum size allowed for job files
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Jobmaxsize_INVALID</c>.
         * </para>
         */
        public int get_jobMaxSize()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            res = _func.get_jobMaxSize();
            if (res == YAPI.INVALID_INT) {
                res = _JobMaxSize_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Maximum size allowed for job files.</value>
        public int JobMaxSize
        {
            get
            {
                if (_func == null) {
                    return _JobMaxSize_INVALID;
                }
                if (_online) {
                    return _jobMaxSize;
                }
                return _JobMaxSize_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the type of protocol used to send I2C messages, as a string.
         * <para>
         *   Possible values are
         *   "Line" for messages separated by LF or
         *   "Char" for continuous stream of codes.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the type of protocol used to send I2C messages, as a string
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._Protocol_INVALID</c>.
         * </para>
         */
        public string get_protocol()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.get_protocol();
        }

        /**
         * <summary>
         *   Changes the type of protocol used to send I2C messages.
         * <para>
         *   Possible values are
         *   "Line" for messages separated by LF or
         *   "Char" for continuous stream of codes.
         *   The suffix "/[wait]ms" can be added to reduce the transmit rate so that there
         *   is always at lest the specified number of milliseconds between each message sent.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the type of protocol used to send I2C messages
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
        public int set_protocol(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            if (newval == _Protocol_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_protocol(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Type of protocol used to send I2C messages, as a string.</value>
        public string Protocol
        {
            get
            {
                if (_func == null) {
                    return _Protocol_INVALID;
                }
                if (_online) {
                    return _protocol;
                }
                return _Protocol_INVALID;
            }
            set
            {
                setprop_protocol(value);
            }
        }

        // private helper for magic property
        private void setprop_protocol(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Protocol_INVALID) {
                return;
            }
            if (newval == _protocol) {
                return;
            }
            _func.set_protocol(newval);
            _protocol = newval;
        }

        /**
         * <summary>
         *   Returns the voltage level used on the I2C bus.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>i2cport._I2cvoltagelevel_OFF</c>, <c>i2cport._I2cvoltagelevel_3V3</c> and
         *   <c>i2cport._I2cvoltagelevel_1V8</c> corresponding to the voltage level used on the I2C bus
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._I2cvoltagelevel_INVALID</c>.
         * </para>
         */
        public int get_i2cVoltageLevel()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_i2cVoltageLevel()+1;
        }

        /**
         * <summary>
         *   Changes the voltage level used on the I2C bus.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>i2cport._I2cvoltagelevel_OFF</c>, <c>i2cport._I2cvoltagelevel_3V3</c> and
         *   <c>i2cport._I2cvoltagelevel_1V8</c> corresponding to the voltage level used on the I2C bus
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
        public int set_i2cVoltageLevel(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            if (newval == _I2cVoltageLevel_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_i2cVoltageLevel(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Voltage level used on the I2C bus.</value>
        public int I2cVoltageLevel
        {
            get
            {
                if (_func == null) {
                    return _I2cVoltageLevel_INVALID;
                }
                if (_online) {
                    return _i2cVoltageLevel;
                }
                return _I2cVoltageLevel_INVALID;
            }
            set
            {
                setprop_i2cVoltageLevel(value);
            }
        }

        // private helper for magic property
        private void setprop_i2cVoltageLevel(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _I2cVoltageLevel_INVALID) {
                return;
            }
            if (newval == _i2cVoltageLevel) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_i2cVoltageLevel(newval-1);
            _i2cVoltageLevel = newval;
        }

        /**
         * <summary>
         *   Returns the I2C port communication parameters, as a string such as
         *   "400kbps,2000ms,NoRestart".
         * <para>
         *   The string includes the baud rate, the
         *   recovery delay after communications errors, and if needed the option
         *   <c>NoRestart</c> to use a Stop/Start sequence instead of the
         *   Restart state when performing read on the I2C bus.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the I2C port communication parameters, as a string such as
         *   "400kbps,2000ms,NoRestart"
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>i2cport._I2cmode_INVALID</c>.
         * </para>
         */
        public string get_i2cMode()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.get_i2cMode();
        }

        /**
         * <summary>
         *   Changes the I2C port communication parameters, with a string such as
         *   "400kbps,2000ms".
         * <para>
         *   The string includes the baud rate, the
         *   recovery delay after communications errors, and if needed the option
         *   <c>NoRestart</c> to use a Stop/Start sequence instead of the
         *   Restart state when performing read on the I2C bus.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the I2C port communication parameters, with a string such as
         *   "400kbps,2000ms"
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
        public int set_i2cMode(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            if (newval == _I2cMode_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_i2cMode(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>I2C port communication parameters, as a string such as</value>
        public string I2cMode
        {
            get
            {
                if (_func == null) {
                    return _I2cMode_INVALID;
                }
                if (_online) {
                    return _i2cMode;
                }
                return _I2cMode_INVALID;
            }
            set
            {
                setprop_i2cMode(value);
            }
        }

        // private helper for magic property
        private void setprop_i2cMode(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _I2cMode_INVALID) {
                return;
            }
            if (newval == _i2cMode) {
                return;
            }
            _func.set_i2cMode(newval);
            _i2cMode = newval;
        }

        /**
         * <summary>
         *   Reads a single line (or message) from the receive buffer, starting at current stream position.
         * <para>
         *   This function is intended to be used when the serial port is configured for a message protocol,
         *   such as 'Line' mode or frame protocols.
         * </para>
         * <para>
         *   If data at current stream position is not available anymore in the receive buffer,
         *   the function returns the oldest available line and moves the stream position just after.
         *   If no new full line is received, the function returns an empty line.
         * </para>
         * </summary>
         * <returns>
         *   a string with a single line of text
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual string readLine()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.readLine();
        }

        /**
         * <summary>
         *   Searches for incoming messages in the serial port receive buffer matching a given pattern,
         *   starting at current position.
         * <para>
         *   This function will only compare and return printable characters
         *   in the message strings. Binary protocols are handled as hexadecimal strings.
         * </para>
         * <para>
         *   The search returns all messages matching the expression provided as argument in the buffer.
         *   If no matching message is found, the search waits for one up to the specified maximum timeout
         *   (in milliseconds).
         * </para>
         * </summary>
         * <param name="pattern">
         *   a limited regular expression describing the expected message format,
         *   or an empty string if all messages should be returned (no filtering).
         *   When using binary protocols, the format applies to the hexadecimal
         *   representation of the message.
         * </param>
         * <param name="maxWait">
         *   the maximum number of milliseconds to wait for a message if none is found
         *   in the receive buffer.
         * </param>
         * <returns>
         *   an array of strings containing the messages found, if any.
         *   Binary messages are converted to hexadecimal representation.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual string[] readMessages(string pattern, int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.readMessages(pattern, maxWait).ToArray();
        }

        /**
         * <summary>
         *   Changes the current internal stream position to the specified value.
         * <para>
         *   This function
         *   does not affect the device, it only changes the value stored in the API object
         *   for the next read operations.
         * </para>
         * </summary>
         * <param name="absPos">
         *   the absolute position index for next read operations.
         * </param>
         * <returns>
         *   nothing.
         * </returns>
         */
        public virtual int read_seek(int absPos)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.read_seek(absPos);
        }

        /**
         * <summary>
         *   Returns the current absolute stream position pointer of the API object.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the absolute position index for next read operations.
         * </returns>
         */
        public virtual int read_tell()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.read_tell();
        }

        /**
         * <summary>
         *   Returns the number of bytes available to read in the input buffer starting from the
         *   current absolute stream position pointer of the API object.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the number of bytes available to read
         * </returns>
         */
        public virtual int read_avail()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.read_avail();
        }

        /**
         * <summary>
         *   Sends a text line query to the serial port, and reads the reply, if any.
         * <para>
         *   This function is intended to be used when the serial port is configured for 'Line' protocol.
         * </para>
         * </summary>
         * <param name="query">
         *   the line query to send (without CR/LF)
         * </param>
         * <param name="maxWait">
         *   the maximum number of milliseconds to wait for a reply.
         * </param>
         * <returns>
         *   the next text line received after sending the text query, as a string.
         *   Additional lines can be obtained by calling readLine or readMessages.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual string queryLine(string query, int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.queryLine(query, maxWait);
        }

        /**
         * <summary>
         *   Sends a binary message to the serial port, and reads the reply, if any.
         * <para>
         *   This function is intended to be used when the serial port is configured for
         *   Frame-based protocol.
         * </para>
         * </summary>
         * <param name="hexString">
         *   the message to send, coded in hexadecimal
         * </param>
         * <param name="maxWait">
         *   the maximum number of milliseconds to wait for a reply.
         * </param>
         * <returns>
         *   the next frame received after sending the message, as a hex string.
         *   Additional frames can be obtained by calling readHex or readMessages.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual string queryHex(string hexString, int maxWait)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.queryHex(hexString, maxWait);
        }

        /**
         * <summary>
         *   Saves the job definition string (JSON data) into a job file.
         * <para>
         *   The job file can be later enabled using <c>selectJob()</c>.
         * </para>
         * </summary>
         * <param name="jobfile">
         *   name of the job file to save on the device filesystem
         * </param>
         * <param name="jsonDef">
         *   a string containing a JSON definition of the job
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int uploadJob(string jobfile, string jsonDef)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.uploadJob(jobfile, jsonDef);
        }

        /**
         * <summary>
         *   Load and start processing the specified job file.
         * <para>
         *   The file must have
         *   been previously created using the user interface or uploaded on the
         *   device filesystem using the <c>uploadJob()</c> function.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="jobfile">
         *   name of the job file (on the device filesystem)
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int selectJob(string jobfile)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.selectJob(jobfile);
        }

        /**
         * <summary>
         *   Clears the serial port buffer and resets counters to zero.
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
        public virtual int reset()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.reset();
        }

        /**
         * <summary>
         *   Sends a one-way message (provided as a a binary buffer) to a device on the I2C bus.
         * <para>
         *   This function checks and reports communication errors on the I2C bus.
         * </para>
         * </summary>
         * <param name="slaveAddr">
         *   the 7-bit address of the slave device (without the direction bit)
         * </param>
         * <param name="buff">
         *   the binary buffer to be sent
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int i2cSendBin(int slaveAddr, byte[] buff)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.i2cSendBin(slaveAddr, buff);
        }

        /**
         * <summary>
         *   Sends a one-way message (provided as a list of integer) to a device on the I2C bus.
         * <para>
         *   This function checks and reports communication errors on the I2C bus.
         * </para>
         * </summary>
         * <param name="slaveAddr">
         *   the 7-bit address of the slave device (without the direction bit)
         * </param>
         * <param name="values">
         *   a list of data bytes to be sent
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int i2cSendArray(int slaveAddr, int[] values)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.i2cSendArray(slaveAddr, new List<int>(values));
        }

        /**
         * <summary>
         *   Sends a one-way message (provided as a a binary buffer) to a device on the I2C bus,
         *   then read back the specified number of bytes from device.
         * <para>
         *   This function checks and reports communication errors on the I2C bus.
         * </para>
         * </summary>
         * <param name="slaveAddr">
         *   the 7-bit address of the slave device (without the direction bit)
         * </param>
         * <param name="buff">
         *   the binary buffer to be sent
         * </param>
         * <param name="rcvCount">
         *   the number of bytes to receive once the data bytes are sent
         * </param>
         * <returns>
         *   a list of bytes with the data received from slave device.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty binary buffer.
         * </para>
         */
        public virtual byte[] i2cSendAndReceiveBin(int slaveAddr, byte[] buff, int rcvCount)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.i2cSendAndReceiveBin(slaveAddr, buff, rcvCount);
        }

        /**
         * <summary>
         *   Sends a one-way message (provided as a list of integer) to a device on the I2C bus,
         *   then read back the specified number of bytes from device.
         * <para>
         *   This function checks and reports communication errors on the I2C bus.
         * </para>
         * </summary>
         * <param name="slaveAddr">
         *   the 7-bit address of the slave device (without the direction bit)
         * </param>
         * <param name="values">
         *   a list of data bytes to be sent
         * </param>
         * <param name="rcvCount">
         *   the number of bytes to receive once the data bytes are sent
         * </param>
         * <returns>
         *   a list of bytes with the data received from slave device.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual int[] i2cSendAndReceiveArray(int slaveAddr, int[] values, int rcvCount)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.i2cSendAndReceiveArray(slaveAddr, new List<int>(values), rcvCount).ToArray();
        }

        /**
         * <summary>
         *   Sends a text-encoded I2C code stream to the I2C bus, as is.
         * <para>
         *   An I2C code stream is a string made of hexadecimal data bytes,
         *   but that may also include the I2C state transitions code:
         *   "{S}" to emit a start condition,
         *   "{R}" for a repeated start condition,
         *   "{P}" for a stop condition,
         *   "xx" for receiving a data byte,
         *   "{A}" to ack a data byte received and
         *   "{N}" to nack a data byte received.
         *   If a newline ("\n") is included in the stream, the message
         *   will be terminated and a newline will also be added to the
         *   receive stream.
         * </para>
         * </summary>
         * <param name="codes">
         *   the code stream to send
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeStr(string codes)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.writeStr(codes);
        }

        /**
         * <summary>
         *   Sends a text-encoded I2C code stream to the I2C bus, and terminate
         *   the message en relâchant le bus.
         * <para>
         *   An I2C code stream is a string made of hexadecimal data bytes,
         *   but that may also include the I2C state transitions code:
         *   "{S}" to emit a start condition,
         *   "{R}" for a repeated start condition,
         *   "{P}" for a stop condition,
         *   "xx" for receiving a data byte,
         *   "{A}" to ack a data byte received and
         *   "{N}" to nack a data byte received.
         *   At the end of the stream, a stop condition is added if missing
         *   and a newline is added to the receive buffer as well.
         * </para>
         * </summary>
         * <param name="codes">
         *   the code stream to send
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeLine(string codes)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.writeLine(codes);
        }

        /**
         * <summary>
         *   Sends a single byte to the I2C bus.
         * <para>
         *   Depending on the I2C bus state, the byte
         *   will be interpreted as an address byte or a data byte.
         * </para>
         * </summary>
         * <param name="code">
         *   the byte to send
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeByte(int code)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.writeByte(code);
        }

        /**
         * <summary>
         *   Sends a byte sequence (provided as a hexadecimal string) to the I2C bus.
         * <para>
         *   Depending on the I2C bus state, the first byte will be interpreted as an
         *   address byte or a data byte.
         * </para>
         * </summary>
         * <param name="hexString">
         *   a string of hexadecimal byte codes
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeHex(string hexString)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.writeHex(hexString);
        }

        /**
         * <summary>
         *   Sends a binary buffer to the I2C bus, as is.
         * <para>
         *   Depending on the I2C bus state, the first byte will be interpreted
         *   as an address byte or a data byte.
         * </para>
         * </summary>
         * <param name="buff">
         *   the binary buffer to send
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeBin(byte[] buff)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.writeBin(buff);
        }

        /**
         * <summary>
         *   Sends a byte sequence (provided as a list of bytes) to the I2C bus.
         * <para>
         *   Depending on the I2C bus state, the first byte will be interpreted as an
         *   address byte or a data byte.
         * </para>
         * </summary>
         * <param name="byteList">
         *   a list of byte codes
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int writeArray(int[] byteList)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No I2cPort connected");
            }
            return _func.writeArray(new List<int>(byteList));
        }
    }
    //--- (end of YI2cPort implementation)
}

