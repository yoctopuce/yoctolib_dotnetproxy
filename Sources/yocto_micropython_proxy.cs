/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YMicroPythonProxy, the Proxy API for MicroPython
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
    //--- (YMicroPython class start)
    static public partial class YoctoProxyManager
    {
        public static YMicroPythonProxy FindMicroPython(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YMicroPython func = null;
            YMicroPythonProxy res = (YMicroPythonProxy)YFunctionProxy.FindSimilarUnknownFunction("YMicroPythonProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YMicroPythonProxy)YFunctionProxy.FindSimilarKnownFunction("YMicroPythonProxy");
                if (res != null) return res;
                func = YMicroPython.FirstMicroPython();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YMicroPythonProxy)func.get_userData();
                    }
                }
            } else {
                func = YMicroPython.FindMicroPython(name);
                if (func.get_userData() != null) {
                    return (YMicroPythonProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YMicroPythonProxy(func, name);
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
 *   The <c>YMicroPython</c> class provides control of the MicroPython interpreter
 *   that can be found on some Yoctopuce devices.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YMicroPythonProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a MicroPython interpreter for a given identifier.
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
         *   This function does not require that the MicroPython interpreter is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YMicroPython.isOnline()</c> to test if the MicroPython interpreter is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a MicroPython interpreter by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the MicroPython interpreter, for instance
         *   <c>MyDevice.microPython</c>.
         * </param>
         * <returns>
         *   a <c>YMicroPython</c> object allowing you to drive the MicroPython interpreter.
         * </returns>
         */
        public static YMicroPythonProxy FindMicroPython(string func)
        {
            return YoctoProxyManager.FindMicroPython(func);
        }
        //--- (end of YMicroPython class start)
        //--- (YMicroPython definitions)
        public const string _LastMsg_INVALID = YAPI.INVALID_STRING;
        public const int _HeapUsage_INVALID = -1;
        public const int _XheapUsage_INVALID = -1;
        public const string _CurrentScript_INVALID = YAPI.INVALID_STRING;
        public const string _StartupScript_INVALID = YAPI.INVALID_STRING;
        public const int _DebugMode_INVALID = 0;
        public const int _DebugMode_OFF = 1;
        public const int _DebugMode_ON = 2;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YMicroPython _func;
        // property cache
        protected string _startupScript = _StartupScript_INVALID;
        //--- (end of YMicroPython definitions)

        //--- (YMicroPython implementation)
        internal YMicroPythonProxy(YMicroPython hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("MicroPython " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YMicroPython) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YMicroPython hwd = YMicroPython.FindMicroPython(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YMicroPython hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering MicroPython callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type MicroPython available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YMicroPython.FindMicroPython</c> to obtain an object that can control the
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
            YMicroPython it = YMicroPython.FirstMicroPython();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextMicroPython();
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
            _startupScript = _func.get_startupScript();
        }

        /**
         * <summary>
         *   Returns the last message produced by a python script.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the last message produced by a python script
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMicroPython.LASTMSG_INVALID</c>.
         * </para>
         */
        public string get_lastMsg()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            return _func.get_lastMsg();
        }

        /**
         * <summary>
         *   Returns the percentage of micropython main memory in use,
         *   as observed at the end of the last garbage collection.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the percentage of micropython main memory in use,
         *   as observed at the end of the last garbage collection
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMicroPython.HEAPUSAGE_INVALID</c>.
         * </para>
         */
        public int get_heapUsage()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            res = _func.get_heapUsage();
            if (res == YAPI.INVALID_INT) {
                res = _HeapUsage_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the percentage of micropython external memory in use,
         *   as observed at the end of the last garbage collection.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the percentage of micropython external memory in use,
         *   as observed at the end of the last garbage collection
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMicroPython.XHEAPUSAGE_INVALID</c>.
         * </para>
         */
        public int get_xheapUsage()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            res = _func.get_xheapUsage();
            if (res == YAPI.INVALID_INT) {
                res = _XheapUsage_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the name of currently active script, if any.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the name of currently active script, if any
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMicroPython.CURRENTSCRIPT_INVALID</c>.
         * </para>
         */
        public string get_currentScript()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            return _func.get_currentScript();
        }

        /**
         * <summary>
         *   Stops current running script, and/or selects a script to run immediately in a
         *   fresh new environment.
         * <para>
         *   If the MicroPython interpreter is busy running a script,
         *   this function will abort it immediately and reset the execution environment.
         *   If a non-empty string is given as argument, the new script will be started.
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
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_currentScript(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            if (newval == _CurrentScript_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_currentScript(newval);
        }

        /**
         * <summary>
         *   Returns the name of the script to run when the device is powered on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the name of the script to run when the device is powered on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMicroPython.STARTUPSCRIPT_INVALID</c>.
         * </para>
         */
        public string get_startupScript()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            return _func.get_startupScript();
        }

        /**
         * <summary>
         *   Changes the script to run when the device is powered on.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the script to run when the device is powered on
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
        public int set_startupScript(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            if (newval == _StartupScript_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_startupScript(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Name of the script to run when the device is powered on.</value>
        public string StartupScript
        {
            get
            {
                if (_func == null) {
                    return _StartupScript_INVALID;
                }
                if (_online) {
                    return _startupScript;
                }
                return _StartupScript_INVALID;
            }
            set
            {
                setprop_startupScript(value);
            }
        }

        // private helper for magic property
        private void setprop_startupScript(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _StartupScript_INVALID) {
                return;
            }
            if (newval == _startupScript) {
                return;
            }
            _func.set_startupScript(newval);
            _startupScript = newval;
        }

        /**
         * <summary>
         *   Returns the activation state of micropython debugging interface.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YMicroPython.DEBUGMODE_OFF</c> or <c>YMicroPython.DEBUGMODE_ON</c>, according to the
         *   activation state of micropython debugging interface
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMicroPython.DEBUGMODE_INVALID</c>.
         * </para>
         */
        public int get_debugMode()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_debugMode()+1;
        }

        /**
         * <summary>
         *   Changes the activation state of micropython debugging interface.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YMicroPython.DEBUGMODE_OFF</c> or <c>YMicroPython.DEBUGMODE_ON</c>, according to the
         *   activation state of micropython debugging interface
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
        public int set_debugMode(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            if (newval == _DebugMode_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_debugMode(newval-1);
        }

        /**
         * <summary>
         *   Submit MicroPython code for execution in the interpreter.
         * <para>
         *   If the MicroPython interpreter is busy, this function will
         *   block until it becomes available. The code is then uploaded,
         *   compiled and executed on the fly, without beeing stored on the device filesystem.
         * </para>
         * <para>
         *   There is no implicit reset of the MicroPython interpreter with
         *   this function. Use method <c>reset()</c> if you need to start
         *   from a fresh environment to run your code.
         * </para>
         * <para>
         *   Note that although MicroPython is mostly compatible with recent Python 3.x
         *   interpreters, the limited ressources on the device impose some restrictions,
         *   in particular regarding the libraries that can be used. Please refer to
         *   the documentation for more details.
         * </para>
         * </summary>
         * <param name="codeName">
         *   name of the code file (used for error reporting only)
         * </param>
         * <param name="mpyCode">
         *   MicroPython code to compile and execute
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int eval(string codeName, string mpyCode)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            return _func.eval(codeName, mpyCode);
        }

        /**
         * <summary>
         *   Stops current execution, and reset the MicroPython interpreter to initial state.
         * <para>
         *   All global variables are cleared, and all imports are forgotten.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int reset()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            return _func.reset();
        }

        /**
         * <summary>
         *   Returns a string with last logs of the MicroPython interpreter.
         * <para>
         *   This method return only logs that are still in the module.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string with last MicroPython logs.
         *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
         * </returns>
         */
        public virtual string get_lastLogs()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No MicroPython connected");
            }
            return _func.get_lastLogs();
        }
    }
    //--- (end of YMicroPython implementation)
}

