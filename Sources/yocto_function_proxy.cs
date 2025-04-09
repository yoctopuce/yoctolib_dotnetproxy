/*********************************************************************
 *
 *  $Id: yocto_function_proxy.cs 64097 2025-01-08 10:59:01Z seb $
 *
 *  Implements YFunctionProxy, the Proxy API for Function
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
    //--- (generated code: YFunction class start)
    static public partial class YoctoProxyManager
    {
        public static YFunctionProxy FindFunction(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YFunction func = null;
            YFunctionProxy res = (YFunctionProxy)YFunctionProxy.FindSimilarUnknownFunction("YFunctionProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YFunctionProxy)YFunctionProxy.FindSimilarKnownFunction("YFunctionProxy");
                if (res != null) return res;
                func = YFunction.FirstFunction();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YFunctionProxy)func.get_userData();
                    }
                }
            } else {
                func = YFunction.FindFunction(name);
                if (func.get_userData() != null) {
                    return (YFunctionProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YFunctionProxy(func, name);
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
 *   This is the parent class for all public objects representing device functions documented in
 *   the high-level programming API.
 * <para>
 *   This abstract class does all the real job, but without
 *   knowledge of the specific function attributes.
 * </para>
 * <para>
 *   Instantiating a child class of YFunction does not cause any communication.
 *   The instance simply keeps track of its function identifier, and will dynamically bind
 *   to a matching device at the time it is really being used to read or set an attribute.
 *   In order to allow true hot-plug replacement of one device by another, the binding stay
 *   dynamic through the life of the object.
 * </para>
 * <para>
 *   The YFunction class implements a generic high-level cache for the attribute values of
 *   the specified function, pre-parsed from the REST API string.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a function for a given identifier.
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
         *   This function does not require that the function is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YFunction.isOnline()</c> to test if the function is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a function by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the function, for instance
         *   <c>MyDevice.</c>.
         * </param>
         * <returns>
         *   a <c>YFunction</c> object allowing you to drive the function.
         * </returns>
         */
        public static YFunctionProxy FindFunction(string func)
        {
            return YoctoProxyManager.FindFunction(func);
        }
        //--- (end of generated code: YFunction class start)
        //--- (generated code: YFunction definitions)
        public const string _LogicalName_INVALID = YAPI.INVALID_STRING;
        public const string _AdvertisedValue_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected YFunction _func;
        // property cache
        protected string _logicalName = _LogicalName_INVALID;
        //--- (end of generated code: YFunction definitions)
        protected string _advertisedValue = "";
        protected string _friendlyName = "";
        protected string _functionId = "";
        protected string _serialNumber = "";
        protected string _hwdid = "";
        protected bool _online = false;
        protected YModule _m = null;
        protected string _instantiationName;
        protected int _instanceId = 0;

        static int FctCount = 0;
        static List<YFunctionProxy> _allProxies = new List<YFunctionProxy>();
        public static void freeall()
        {
            _allProxies.Clear();
        }

        // used for consistency checks in test software, no use for the end user
        public int InstanceId
        {
            get { return _instanceId; }
        }

        // used internally, no use for the end user
        internal bool IsUnknown 
        { 
            get { return _func == null; }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
        }

        /**
         * <summary>
         *   Returns the unique hardware identifier of the function in the form <c>SERIAL.FUNCTIONID</c>.
         * <para>
         *   The unique hardware identifier is composed of the device serial
         *   number and of the hardware identifier of the function (for example <c>RELAYLO1-123456.relay1</c>).
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string that uniquely identifies the function (ex: <c>RELAYLO1-123456.relay1</c>)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns  <c>YFunction.HARDWAREID_INVALID</c>.
         * </para>
         */
        public virtual string get_hardwareId()
        {
            return _hwdid;
        }

        public string HardwareId
        {
            get { return _hwdid; }
        }

        internal string get_funcHardwareId()
        {
            if (_func == null) return "";
            string s = "";
            try
            {
                s = _func.get_hardwareId();
            }
            catch (Exception) { return ""; }

            return s;
        }

        /**
         * <summary>
         *   Returns the hardware identifier of the function, without reference to the module.
         * <para>
         *   For example
         *   <c>relay1</c>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string that identifies the function (ex: <c>relay1</c>)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns  <c>YFunction.FUNCTIONID_INVALID</c>.
         * </para>
         */
        public string get_functionId()
        {
            if (_func == null)
            {
                string msg = "No Function connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_functionId();
        }

        public string FunctionId
        {
            get { return _functionId; }
        }

        /**
         * <summary>
         *   Returns a global identifier of the function in the format <c>MODULE_NAME&#46;FUNCTION_NAME</c>.
         * <para>
         *   The returned string uses the logical names of the module and of the function if they are defined,
         *   otherwise the serial number of the module and the hardware identifier of the function
         *   (for example: <c>MyCustomName.relay1</c>)
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string that uniquely identifies the function using logical names
         *   (ex: <c>MyCustomName.relay1</c>)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns  <c>YFunction.FRIENDLYNAME_INVALID</c>.
         * </para>
         */
        public virtual string get_friendlyName()
        {
            if (_func == null)
            {
                string msg = "No Function connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_friendlyName();
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
        }

        /**
         * <summary>
         *   Checks if the function is currently reachable, without raising any error.
         * <para>
         *   If there is a cached value for the function in cache, that has not yet
         *   expired, the device is considered reachable.
         *   No exception is raised if there is an error while trying to contact the
         *   device hosting the function.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>true</c> if the function can be reached, and <c>false</c> otherwise
         * </returns>
         */
        public bool isOnline()
        {
            if (_func == null)
            {
                return false;
            }
            return _func.isOnline();
        }

        public bool IsOnline
        {
            get { return _online; }
        }

        public bool Online
        {
            get { return _online; }
        }

        /**
         * <summary>
         *   Gets the <c>YModule</c> object for the device on which the function is located.
         * <para>
         *   If the function cannot be located on any module, the returned instance of
         *   <c>YModule</c> is not shown as on-line.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an instance of <c>YModule</c>
         * </returns>
         */
        public YModuleProxy get_module()
        {
            if (_func == null)
            {
                string msg = "No Function connected";
                throw new YoctoApiProxyException(msg);
            }
            return YoctoProxyManager.FindModule(_serialNumber);
        }

        //
        // WARNING: EXTRA-SENSITIVE CODE ! Don't change a single dot in the code below without running
        //                                 the "hidden/AutoTest" test suite (using the dedicated test tool)
        //
        public static YFunctionProxy FindSimilarUnknownFunction(String className)
        {
            for (int i = 0; i < _allProxies.Count; i++)
            {
                string c = _allProxies[i].GetType().ToString();
                if ((_allProxies[i].GetType().ToString() == InternalStuff.currentNameSpace + "." + className) && _allProxies[i].IsUnknown)
                {
                    return _allProxies[i];
                }
            }
            return null;
        }

        //
        // WARNING: EXTRA-SENSITIVE CODE ! Don't change a single dot in the code below without running
        //                                 the "hidden/AutoTest" test suite (using the dedicated test tool)
        //
        public static YFunctionProxy FindSimilarKnownFunction(String className)
        {
            for (int i = 0; i < _allProxies.Count; i++)
            {
                string c = _allProxies[i].GetType().ToString();
                if ((_allProxies[i].GetType().ToString() == InternalStuff.currentNameSpace + "." + className) && !_allProxies[i].IsUnknown)
                {
                    return _allProxies[i];
                }
            }
            return null;
        }

        //
        // WARNING: EXTRA-SENSITIVE CODE ! Don't change a single dot in the code below without running
        //                                 the "hidden/AutoTest" test suite (using the dedicated test tool)
        //
        public static void deviceArrival(YModule m)
        {
            string ms = m.get_serialNumber();
            //InternalStuff.log("*** device arrival(" + m.get_serialNumber() + ")");
            string key = ms + ".module";
            string mynamespace = InternalStuff.currentNameSpace;

            // try to find some unknown module proxy can be linked to the new arrival
            //InternalStuff.log("*** looking for existing Module proxies");
            for (int j = 0; j < _allProxies.Count; j++)
            {
                if (_allProxies[j].IsUnknown)
                {
                    if (_allProxies[j].GetType().ToString() == mynamespace + ".YModuleProxy")
                    {
                        //InternalStuff.log(" found");
                        _allProxies[j].linkToHardware(ms);
                        _allProxies[j].arrival();
                    }
                }
                else if (_allProxies[j].get_funcHardwareId() == key)
                {
                    _allProxies[j].linkToHardware(ms);
                    _allProxies[j].arrival();
                }
            }

            string myNameSpace = InternalStuff.currentNameSpace;

            // try to find some unknown function proxy  can be linked to the new arrival
            //InternalStuff.log("*** looking for existing Function proxies");
            for (int i = 0; i < m.functionCount(); i++)
            {
                string hwid = ms + "." + m.functionId(i);
                string type = m.functionType(i);
                string basetype = m.functionBaseType(i);

                for (int j = 0; j < _allProxies.Count; j++)
                {
                    if (_allProxies[j].IsUnknown)
                    {
                        string proxyType = _allProxies[j].GetType().ToString();
                        if (proxyType == myNameSpace + ".Y" + type + "Proxy" || proxyType == myNameSpace + ".Y" + basetype + "Proxy")
                        {
                            //InternalStuff.log(" found " + type);
                            _allProxies[j].linkToHardware(hwid);
                            _allProxies[j].arrival();
                        }
                    }
                    else if (_allProxies[j].get_funcHardwareId() == hwid)
                    {
                        _allProxies[j].linkToHardware(hwid);
                        _allProxies[j].arrival();
                    }
                }
            }

            m.registerConfigChangeCallback(configChangeCallback);
            configChangeCallback(m); // not triggered automatically at register
            InternalStuff.log("Arrival completed");
        }

        //
        // WARNING: EXTRA-SENSITIVE CODE ! Don't change a single dot in the code below without running
        //                                 the "hidden/AutoTest" test suite (using the dedicated test tool)
        //
        public static void deviceRemoval(YModule m)
        {
            string serial = m.get_serialNumber();
            //InternalStuff.log("device removal(" + serial + ")");

            for (int j = 0; j < _allProxies.Count; j++)
                if (_allProxies[j].Online)
                    if (_allProxies[j].get_hardwareId().Substring(0, serial.Length) == serial)
                        _allProxies[j].removal();
        }

        private static void configChangeCallback(YModule module)
        {
            string id = module.get_serialNumber();
            //InternalStuff.log(" Module " + id + " config change  ");
            if (id == "") return;  // just in case
            foreach (YFunctionProxy f in _allProxies)
            {
                if (f.HardwareId.Length>=id.Length)
                if (f.HardwareId.Substring(0, id.Length) == id) f.moduleConfigHasChanged();
            }
        }

        protected virtual void valueChangeCallback(YFunction source, string value)
        {
            //InternalStuff.log("new value (" + value + ")");
            _advertisedValue = value;
        }

        public string AdvertisedValue
        {
            get
            {
                if (_func == null) return YAPI.INVALID_STRING;
                return (_online ? _advertisedValue : YAPI.INVALID_STRING);
            }
        }

        public void arrival()
        {
            //InternalStuff.log(_hwdid + " comes online");
            _online = true;
            functionArrival();
        }

        public void removal()
        {
            _online = false;
            //InternalStuff.log(_hwdid + " has been removed");
        }

        public bool TestOnline()
        {
            if (_func == null) return false;
            //InternalStuff.log("test if " + _hwdid + "(" + _instantiationName + ")  is online");
            if (!_func.isOnline())
            {
                //InternalStuff.log(" Nope, " + _hwdid + "(" + _instantiationName + ") is still offline");
                _online = false;
                return false;
            }
            //InternalStuff.log(" Yes, " + _hwdid + "(" + _instantiationName + ") is online");
            _m = _func.get_module();
            _hwdid = _func.get_hardwareId();
            _serialNumber = _func.get_serialNumber();
            _functionId = _func.get_functionId();
            _friendlyName = _func.get_friendlyName();

            _online = true;
            //InternalStuff.log(_hwdid + " is online");
            return true;
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal virtual void base_init(YFunction hwd, string instantiationName)
        {
            string errmsg = "";
            _func = hwd;
            _instantiationName = instantiationName;
            InternalStuff.initYoctopuceLibrary(ref errmsg, true);
            if (_func != null)
            {
                try
                {
                    _func.set_userData(this);
                    _hwdid = _func.get_hardwareId();
                    //InternalStuff.log(" hwdID = " + _hwdid);
                }
                catch (Exception)
                {
                    //InternalStuff.log("Failed to find out HwdID, device is probably offline ");
                }
            }

        }

        //--- (generated code: YFunction implementation)
        internal YFunctionProxy(YFunction hwd, string instantiationName)
        {
            InternalStuff.log("YFunction constructor for " + instantiationName);

            // integrity test
            FctCount++;
            _instanceId = FctCount;

            base_init(hwd, instantiationName);
            _allProxies.Add(this);
        }

        // link the instance to a real YoctoAPI object
        internal virtual void linkToHardware(string hwdName)
        {
            YFunction hwd = YFunction.FindFunction(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YFunction hwd)
        {
            if (hwd == null) return;
            _func = hwd;
            TestOnline();
        }

        /**
         * <summary>
         *   Returns an array of strings representing hardware identifiers for all Function functions presently connected.
         * <para>
         * </para>
         * </summary>
         */
        public static  string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YFunction it = YFunction.FirstFunction();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextFunction();
            }
            return res.ToArray();
        }

        protected virtual void functionArrival()
        {
            moduleConfigHasChanged();
            valueChangeCallback(_func, _func.get_advertisedValue());
        }

        protected virtual void moduleConfigHasChanged()
       	{
            _serialNumber = _func.get_serialNumber();
            _functionId = _func.get_functionId();
            _friendlyName = _func.get_friendlyName();
            _logicalName = _func.get_logicalName();
        }

        /**
         * <summary>
         *   Returns the logical name of the function.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the logical name of the function
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YFunction.LOGICALNAME_INVALID</c>.
         * </para>
         */
        public string get_logicalName()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Function connected");
            }
            return _func.get_logicalName();
        }

        /**
         * <summary>
         *   Changes the logical name of the function.
         * <para>
         *   You can use <c>yCheckLogicalName()</c>
         *   prior to this call to make sure that your parameter is valid.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the logical name of the function
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
        public int set_logicalName(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Function connected");
            }
            if (newval == _LogicalName_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_logicalName(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Logical name of the function.</value>
        public string LogicalName
        {
            get
            {
                if (_func == null) {
                    return _LogicalName_INVALID;
                }
                if (_online) {
                    return _logicalName;
                }
                return _LogicalName_INVALID;
            }
            set
            {
                setprop_logicalName(value);
            }
        }

        // private helper for magic property
        private void setprop_logicalName(string newval)
        {
            if (!(YAPI.CheckLogicalName(newval))) {
                throw new YoctoApiProxyException("Invalid logical name");
            }
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _LogicalName_INVALID) {
                return;
            }
            if (newval == _logicalName) {
                return;
            }
            _func.set_logicalName(newval);
            _logicalName = newval;
        }

        /**
         * <summary>
         *   Returns a short string representing the current state of the function.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to a short string representing the current state of the function
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YFunction.ADVERTISEDVALUE_INVALID</c>.
         * </para>
         */
        public string get_advertisedValue()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Function connected");
            }
            return _func.get_advertisedValue();
        }

        /**
         * <summary>
         *   Disables the propagation of every new advertised value to the parent hub.
         * <para>
         *   You can use this function to save bandwidth and CPU on computers with limited
         *   resources, or to prevent unwanted invocations of the HTTP callback.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int muteValueCallbacks()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Function connected");
            }
            return _func.muteValueCallbacks();
        }

        /**
         * <summary>
         *   Re-enables the propagation of every new advertised value to the parent hub.
         * <para>
         *   This function reverts the effect of a previous call to <c>muteValueCallbacks()</c>.
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int unmuteValueCallbacks()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Function connected");
            }
            return _func.unmuteValueCallbacks();
        }

        /**
         * <summary>
         *   Returns the current value of a single function attribute, as a text string, as quickly as
         *   possible but without using the cached value.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="attrName">
         *   the name of the requested attribute
         * </param>
         * <returns>
         *   a string with the value of the the attribute
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty string.
         * </para>
         */
        public virtual string loadAttribute(string attrName)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Function connected");
            }
            return _func.loadAttribute(attrName);
        }

        /**
         * <summary>
         *   Indicates whether changes to the function are prohibited or allowed.
         * <para>
         *   Returns <c>true</c> if the function is blocked by an admin password
         *   or if the function is not available.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>true</c> if the function is write-protected or not online.
         * </returns>
         */
        public virtual bool isReadOnly()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Function connected");
            }
            return _func.isReadOnly();
        }

        /**
         * <summary>
         *   Returns the serial number of the module, as set by the factory.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the serial number of the module, as set by the factory.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns YFunction.SERIALNUMBER_INVALID.
         * </para>
         */
        public virtual string get_serialNumber()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Function connected");
            }
            return _func.get_serialNumber();
        }
    }
    //--- (end of generated code: YFunction implementation)

}

