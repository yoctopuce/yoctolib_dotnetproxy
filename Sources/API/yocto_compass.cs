namespace YoctoLib 
{/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements yFindCompass(), the high-level API for Compass functions
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using YDEV_DESCR = System.Int32;
using YFUN_DESCR = System.Int32;

#pragma warning disable 1591
//--- (YCompass return codes)
//--- (end of YCompass return codes)
//--- (YCompass dlldef_core)
//--- (end of YCompass dlldef_core)
//--- (YCompass dll_core_map)
//--- (end of YCompass dll_core_map)
//--- (YCompass dlldef)
//--- (end of YCompass dlldef)
//--- (YCompass yapiwrapper)
//--- (end of YCompass yapiwrapper)
//--- (YCompass class start)
/**
 * <summary>
 *   The <c>YCompass</c> class allows you to read and configure Yoctopuce compass functions.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measurements,
 *   to register callback functions, and to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YCompass : YSensor
{
//--- (end of YCompass class start)
    //--- (YCompass definitions)
    public new delegate void ValueCallback(YCompass func, string value);
    public new delegate void TimedReportCallback(YCompass func, YMeasure measure);

    public const int BANDWIDTH_INVALID = YAPI.INVALID_UINT;
    public const int AXIS_X = 0;
    public const int AXIS_Y = 1;
    public const int AXIS_Z = 2;
    public const int AXIS_INVALID = -1;
    public const double MAGNETICHEADING_INVALID = YAPI.INVALID_DOUBLE;
    protected int _bandwidth = BANDWIDTH_INVALID;
    protected int _axis = AXIS_INVALID;
    protected double _magneticHeading = MAGNETICHEADING_INVALID;
    protected ValueCallback _valueCallbackCompass = null;
    protected TimedReportCallback _timedReportCallbackCompass = null;
    //--- (end of YCompass definitions)

    public YCompass(string func)
        : base(func)
    {
        _className = "Compass";
        //--- (YCompass attributes initialization)
        //--- (end of YCompass attributes initialization)
    }

    //--- (YCompass implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("bandwidth"))
        {
            _bandwidth = json_val.getInt("bandwidth");
        }
        if (json_val.has("axis"))
        {
            _axis = json_val.getInt("axis");
        }
        if (json_val.has("magneticHeading"))
        {
            _magneticHeading = Math.Round(json_val.getDouble("magneticHeading") / 65.536) / 1000.0;
        }
        base._parseAttr(json_val);
    }


    /**
     * <summary>
     *   Returns the measure update frequency, measured in Hz.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the measure update frequency, measured in Hz
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCompass.BANDWIDTH_INVALID</c>.
     * </para>
     */
    public int get_bandwidth()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return BANDWIDTH_INVALID;
                }
            }
            res = this._bandwidth;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the measure update frequency, measured in Hz.
     * <para>
     *   When the
     *   frequency is lower, the device performs averaging.
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the measure update frequency, measured in Hz
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
    public int set_bandwidth(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("bandwidth", rest_val);
        }
    }


    public int get_axis()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return AXIS_INVALID;
                }
            }
            res = this._axis;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the magnetic heading, regardless of the configured bearing.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the magnetic heading, regardless of the configured bearing
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCompass.MAGNETICHEADING_INVALID</c>.
     * </para>
     */
    public double get_magneticHeading()
    {
        double res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return MAGNETICHEADING_INVALID;
                }
            }
            res = this._magneticHeading;
        }
        return res;
    }


    /**
     * <summary>
     *   Retrieves a compass function for a given identifier.
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
     *   This function does not require that the compass function is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YCompass.isOnline()</c> to test if the compass function is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a compass function by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the compass function, for instance
     *   <c>Y3DMK002.compass</c>.
     * </param>
     * <returns>
     *   a <c>YCompass</c> object allowing you to drive the compass function.
     * </returns>
     */
    public static YCompass FindCompass(string func)
    {
        YCompass obj;
        lock (YAPI.globalLock) {
            obj = (YCompass) YFunction._FindFromCache("Compass", func);
            if (obj == null) {
                obj = new YCompass(func);
                YFunction._AddToCache("Compass", func, obj);
            }
        }
        return obj;
    }


    /**
     * <summary>
     *   Registers the callback function that is invoked on every change of advertised value.
     * <para>
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
     *   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the function object of which the value has changed, and the character string describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerValueCallback(ValueCallback callback)
    {
        string val;
        if (callback != null) {
            YFunction._UpdateValueCallbackList(this, true);
        } else {
            YFunction._UpdateValueCallbackList(this, false);
        }
        this._valueCallbackCompass = callback;
        // Immediately invoke value callback with current value
        if (callback != null && this.isOnline()) {
            val = this._advertisedValue;
            if (!(val == "")) {
                this._invokeValueCallback(val);
            }
        }
        return 0;
    }


    public override int _invokeValueCallback(string value)
    {
        if (this._valueCallbackCompass != null) {
            this._valueCallbackCompass(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }


    /**
     * <summary>
     *   Registers the callback function that is invoked on every periodic timed notification.
     * <para>
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
     *   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the function object of which the value has changed, and an <c>YMeasure</c> object describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerTimedReportCallback(TimedReportCallback callback)
    {
        YSensor sensor;
        sensor = this;
        if (callback != null) {
            YFunction._UpdateTimedReportCallbackList(sensor, true);
        } else {
            YFunction._UpdateTimedReportCallbackList(sensor, false);
        }
        this._timedReportCallbackCompass = callback;
        return 0;
    }


    public override int _invokeTimedReportCallback(YMeasure value)
    {
        if (this._timedReportCallbackCompass != null) {
            this._timedReportCallbackCompass(this, value);
        } else {
            base._invokeTimedReportCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of compass functions started using <c>yFirstCompass()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned compass functions order.
     *   If you want to find a specific a compass function, use <c>Compass.findCompass()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCompass</c> object, corresponding to
     *   a compass function currently online, or a <c>null</c> pointer
     *   if there are no more compass functions to enumerate.
     * </returns>
     */
    public YCompass nextCompass()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindCompass(hwid);
    }

    //--- (end of YCompass implementation)

    //--- (YCompass functions)

    /**
     * <summary>
     *   Starts the enumeration of compass functions currently accessible.
     * <para>
     *   Use the method <c>YCompass.nextCompass()</c> to iterate on
     *   next compass functions.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCompass</c> object, corresponding to
     *   the first compass function currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YCompass FirstCompass()
    {
        YFUN_DESCR[] v_fundescr = new YFUN_DESCR[1];
        YDEV_DESCR dev = default(YDEV_DESCR);
        int neededsize = 0;
        int err = 0;
        string serial = null;
        string funcId = null;
        string funcName = null;
        string funcVal = null;
        string errmsg = "";
        int size = Marshal.SizeOf(v_fundescr[0]);
        IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(v_fundescr[0]));
        err = YAPI.apiGetFunctionsByClass("Compass", 0, p, size, ref neededsize, ref errmsg);
        Marshal.Copy(p, v_fundescr, 0, 1);
        Marshal.FreeHGlobal(p);
        if ((YAPI.YISERR(err) | (neededsize == 0)))
            return null;
        serial = "";
        funcId = "";
        funcName = "";
        funcVal = "";
        errmsg = "";
        if ((YAPI.YISERR(YAPI.yapiGetFunctionInfo(v_fundescr[0], ref dev, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg))))
            return null;
        return FindCompass(serial + "." + funcId);
    }

    //--- (end of YCompass functions)
}
#pragma warning restore 1591


}
