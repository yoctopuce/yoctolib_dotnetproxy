/*********************************************************************
 *
 *  $Id: yocto_refframe_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YRefFrameProxy, the Proxy API for RefFrame
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
    //--- (YRefFrame class start)
    static public partial class YoctoProxyManager
    {
        public static YRefFrameProxy FindRefFrame(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YRefFrame func = null;
            YRefFrameProxy res = (YRefFrameProxy)YFunctionProxy.FindSimilarUnknownFunction("YRefFrameProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YRefFrameProxy)YFunctionProxy.FindSimilarKnownFunction("YRefFrameProxy");
                if (res != null) return res;
                func = YRefFrame.FirstRefFrame();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YRefFrameProxy)func.get_userData();
                    }
                }
            } else {
                func = YRefFrame.FindRefFrame(name);
                if (func.get_userData() != null) {
                    return (YRefFrameProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YRefFrameProxy(func, name);
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
 *   The YRefFrame class is used to setup the base orientation of the Yoctopuce inertial
 *   sensors, for instance using a Yocto-3D-V2.
 * <para>
 *   Thanks to this, orientation functions relative to the earth surface plane
 *   can use the proper reference frame. The class also implements a tridimensional
 *   sensor calibration process, which can compensate for local variations
 *   of standard gravity and improve the precision of the tilt sensors.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YRefFrameProxy : YFunctionProxy
    {
        //--- (end of YRefFrame class start)
        //--- (YRefFrame definitions)
        public const int _MountPos_INVALID = -1;
        public const double _Bearing_INVALID = Double.NaN;
        public const string _CalibrationParam_INVALID = YAPI.INVALID_STRING;
        public const int _FusionMode_INVALID = 0;
        public const int _FusionMode_NDOF = 1;
        public const int _FusionMode_NDOF_FMC_OFF = 2;
        public const int _FusionMode_M4G = 3;
        public const int _FusionMode_COMPASS = 4;
        public const int _FusionMode_IMU = 5;
        public const int _MOUNTPOSITION_INVALID = 0;
        public const int _MOUNTPOSITION_BOTTOM = 1;
        public const int _MOUNTPOSITION_TOP = 2;
        public const int _MOUNTPOSITION_FRONT = 3;
        public const int _MOUNTPOSITION_REAR = 4;
        public const int _MOUNTPOSITION_RIGHT = 5;
        public const int _MOUNTPOSITION_LEFT = 6;

        private int _MOUNTPOSITION2Int(YRefFrame.MOUNTPOSITION realenum)
        {
            switch (realenum) {
                case YRefFrame.MOUNTPOSITION.BOTTOM:
                    return _MOUNTPOSITION_BOTTOM;
                case YRefFrame.MOUNTPOSITION.TOP:
                    return _MOUNTPOSITION_TOP;
                case YRefFrame.MOUNTPOSITION.FRONT:
                    return _MOUNTPOSITION_FRONT;
                case YRefFrame.MOUNTPOSITION.REAR:
                    return _MOUNTPOSITION_REAR;
                case YRefFrame.MOUNTPOSITION.RIGHT:
                    return _MOUNTPOSITION_RIGHT;
                case YRefFrame.MOUNTPOSITION.LEFT:
                    return _MOUNTPOSITION_LEFT;
                case YRefFrame.MOUNTPOSITION.INVALID:
                    default:
                    return _MOUNTPOSITION_INVALID;
            }
        }

        private YRefFrame.MOUNTPOSITION _Int2MOUNTPOSITION(int value)
        {
            switch (value) {
                case _MOUNTPOSITION_BOTTOM:
                    return YRefFrame.MOUNTPOSITION.BOTTOM;
                case _MOUNTPOSITION_TOP:
                    return YRefFrame.MOUNTPOSITION.TOP;
                case _MOUNTPOSITION_FRONT:
                    return YRefFrame.MOUNTPOSITION.FRONT;
                case _MOUNTPOSITION_REAR:
                    return YRefFrame.MOUNTPOSITION.REAR;
                case _MOUNTPOSITION_RIGHT:
                    return YRefFrame.MOUNTPOSITION.RIGHT;
                case _MOUNTPOSITION_LEFT:
                    return YRefFrame.MOUNTPOSITION.LEFT;
                case _MOUNTPOSITION_INVALID:
                    default:
                    return YRefFrame.MOUNTPOSITION.INVALID;
            }
        }
        public const int _MOUNTORIENTATION_INVALID = 0;
        public const int _MOUNTORIENTATION_TWELVE = 1;
        public const int _MOUNTORIENTATION_THREE = 2;
        public const int _MOUNTORIENTATION_SIX = 3;
        public const int _MOUNTORIENTATION_NINE = 4;

        private int _MOUNTORIENTATION2Int(YRefFrame.MOUNTORIENTATION realenum)
        {
            switch (realenum) {
                case YRefFrame.MOUNTORIENTATION.TWELVE:
                    return _MOUNTORIENTATION_TWELVE;
                case YRefFrame.MOUNTORIENTATION.THREE:
                    return _MOUNTORIENTATION_THREE;
                case YRefFrame.MOUNTORIENTATION.SIX:
                    return _MOUNTORIENTATION_SIX;
                case YRefFrame.MOUNTORIENTATION.NINE:
                    return _MOUNTORIENTATION_NINE;
                case YRefFrame.MOUNTORIENTATION.INVALID:
                    default:
                    return _MOUNTORIENTATION_INVALID;
            }
        }

        private YRefFrame.MOUNTORIENTATION _Int2MOUNTORIENTATION(int value)
        {
            switch (value) {
                case _MOUNTORIENTATION_TWELVE:
                    return YRefFrame.MOUNTORIENTATION.TWELVE;
                case _MOUNTORIENTATION_THREE:
                    return YRefFrame.MOUNTORIENTATION.THREE;
                case _MOUNTORIENTATION_SIX:
                    return YRefFrame.MOUNTORIENTATION.SIX;
                case _MOUNTORIENTATION_NINE:
                    return YRefFrame.MOUNTORIENTATION.NINE;
                case _MOUNTORIENTATION_INVALID:
                    default:
                    return YRefFrame.MOUNTORIENTATION.INVALID;
            }
        }

        // reference to real YoctoAPI object
        protected new YRefFrame _func;
        // property cache
        protected double _bearing = _Bearing_INVALID;
        protected int _fusionMode = _FusionMode_INVALID;
        //--- (end of YRefFrame definitions)

        //--- (YRefFrame implementation)
        internal YRefFrameProxy(YRefFrame hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("RefFrame " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YRefFrame) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YRefFrame hwd = YRefFrame.FindRefFrame(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YRefFrame hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering RefFrame callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YRefFrame it = YRefFrame.FirstRefFrame();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextRefFrame();
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
            _bearing = _func.get_bearing();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _fusionMode = _func.get_fusionMode()+1;
        }

        /**
         * <summary>
         *   Changes the reference bearing used by the compass.
         * <para>
         *   The relative bearing
         *   indicated by the compass is the difference between the measured magnetic
         *   heading and the reference bearing indicated here.
         * </para>
         * <para>
         *   For instance, if you setup as reference bearing the value of the earth
         *   magnetic declination, the compass will provide the orientation relative
         *   to the geographic North.
         * </para>
         * <para>
         *   Similarly, when the sensor is not mounted along the standard directions
         *   because it has an additional yaw angle, you can set this angle in the reference
         *   bearing so that the compass provides the expected natural direction.
         * </para>
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the reference bearing used by the compass
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
        public int set_bearing(double newval)
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            if (Double.IsNaN(newval)) return YAPI.SUCCESS;
            return _func.set_bearing(newval);
        }


        // property with cached value for instant access (configuration)
        public double Bearing
        {
            get
            {
                if (_func == null) return _Bearing_INVALID;
                return (_online ? _bearing : _Bearing_INVALID);
            }
            set
            {
                setprop_bearing(value);
            }
        }

        // private helper for magic property
        private void setprop_bearing(double newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (Double.IsNaN(newval)) return;
            if (newval == _bearing) return;
            _func.set_bearing(newval);
            _bearing = newval;
        }

        /**
         * <summary>
         *   Returns the reference bearing used by the compass.
         * <para>
         *   The relative bearing
         *   indicated by the compass is the difference between the measured magnetic
         *   heading and the reference bearing indicated here.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the reference bearing used by the compass
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRefFrame.BEARING_INVALID</c>.
         * </para>
         */
        public double get_bearing()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            double res = _func.get_bearing();
            if (res == YAPI.INVALID_DOUBLE) res = Double.NaN;
            return res;
        }

        /**
         * <summary>
         *   Returns the BNO055 fusion mode.
         * <para>
         *   Note this feature is only availabe on Yocto-3D-V2.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YRefFrame.FUSIONMODE_NDOF</c>, <c>YRefFrame.FUSIONMODE_NDOF_FMC_OFF</c>,
         *   <c>YRefFrame.FUSIONMODE_M4G</c>, <c>YRefFrame.FUSIONMODE_COMPASS</c> and
         *   <c>YRefFrame.FUSIONMODE_IMU</c> corresponding to the BNO055 fusion mode
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YRefFrame.FUSIONMODE_INVALID</c>.
         * </para>
         */
        public int get_fusionMode()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_fusionMode()+1;
        }

        /**
         * <summary>
         *   Change the BNO055 fusion mode.
         * <para>
         *   Note: this feature is only availabe on Yocto-3D-V2.
         *   Remember to call the matching module <c>saveToFlash()</c> method to save the setting permanently.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YRefFrame.FUSIONMODE_NDOF</c>, <c>YRefFrame.FUSIONMODE_NDOF_FMC_OFF</c>,
         *   <c>YRefFrame.FUSIONMODE_M4G</c>, <c>YRefFrame.FUSIONMODE_COMPASS</c> and <c>YRefFrame.FUSIONMODE_IMU</c>
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
        public int set_fusionMode(int newval)
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _FusionMode_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_fusionMode(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int FusionMode
        {
            get
            {
                if (_func == null) return _FusionMode_INVALID;
                return (_online ? _fusionMode : _FusionMode_INVALID);
            }
            set
            {
                setprop_fusionMode(value);
            }
        }

        // private helper for magic property
        private void setprop_fusionMode(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _FusionMode_INVALID) return;
            if (newval == _fusionMode) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_fusionMode(newval-1);
            _fusionMode = newval;
        }

        /**
         * <summary>
         *   Returns the installation position of the device, as configured
         *   in order to define the reference frame for the compass and the
         *   pitch/roll tilt sensors.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among the <c>YRefFrame.MOUNTPOSITION</c> enumeration
         *   (<c>YRefFrame.MOUNTPOSITION_BOTTOM</c>,   <c>YRefFrame.MOUNTPOSITION_TOP</c>,
         *   <c>YRefFrame.MOUNTPOSITION_FRONT</c>,    <c>YRefFrame.MOUNTPOSITION_RIGHT</c>,
         *   <c>YRefFrame.MOUNTPOSITION_REAR</c>,     <c>YRefFrame.MOUNTPOSITION_LEFT</c>),
         *   corresponding to the installation in a box, on one of the six faces.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns YRefFrame.MOUNTPOSITION_INVALID.
         * </para>
         */
        public virtual int get_mountPosition()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _MOUNTPOSITION2Int(_func.get_mountPosition());
        }

        /**
         * <summary>
         *   Returns the installation orientation of the device, as configured
         *   in order to define the reference frame for the compass and the
         *   pitch/roll tilt sensors.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among the enumeration <c>YRefFrame.MOUNTORIENTATION</c>
         *   (<c>YRefFrame.MOUNTORIENTATION_TWELVE</c>, <c>YRefFrame.MOUNTORIENTATION_THREE</c>,
         *   <c>YRefFrame.MOUNTORIENTATION_SIX</c>,     <c>YRefFrame.MOUNTORIENTATION_NINE</c>)
         *   corresponding to the orientation of the "X" arrow on the device,
         *   as on a clock dial seen from an observer in the center of the box.
         *   On the bottom face, the 12H orientation points to the front, while
         *   on the top face, the 12H orientation points to the rear.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns YRefFrame.MOUNTORIENTATION_INVALID.
         * </para>
         */
        public virtual int get_mountOrientation()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _MOUNTORIENTATION2Int(_func.get_mountOrientation());
        }

        /**
         * <summary>
         *   Changes the compass and tilt sensor frame of reference.
         * <para>
         *   The magnetic compass
         *   and the tilt sensors (pitch and roll) naturally work in the plane
         *   parallel to the earth surface. In case the device is not installed upright
         *   and horizontally, you must select its reference orientation (parallel to
         *   the earth surface) so that the measures are made relative to this position.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="position">
         *   a value among the <c>YRefFrame.MOUNTPOSITION</c> enumeration
         *   (<c>YRefFrame.MOUNTPOSITION_BOTTOM</c>,   <c>YRefFrame.MOUNTPOSITION_TOP</c>,
         *   <c>YRefFrame.MOUNTPOSITION_FRONT</c>,    <c>YRefFrame.MOUNTPOSITION_RIGHT</c>,
         *   <c>YRefFrame.MOUNTPOSITION_REAR</c>,     <c>YRefFrame.MOUNTPOSITION_LEFT</c>),
         *   corresponding to the installation in a box, on one of the six faces.
         * </param>
         * <param name="orientation">
         *   a value among the enumeration <c>YRefFrame.MOUNTORIENTATION</c>
         *   (<c>YRefFrame.MOUNTORIENTATION_TWELVE</c>, <c>YRefFrame.MOUNTORIENTATION_THREE</c>,
         *   <c>YRefFrame.MOUNTORIENTATION_SIX</c>,     <c>YRefFrame.MOUNTORIENTATION_NINE</c>)
         *   corresponding to the orientation of the "X" arrow on the device,
         *   as on a clock dial seen from an observer in the center of the box.
         *   On the bottom face, the 12H orientation points to the front, while
         *   on the top face, the 12H orientation points to the rear.
         * </param>
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_mountPosition(int position, int orientation)
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.set_mountPosition(_Int2MOUNTPOSITION(position), _Int2MOUNTORIENTATION(orientation));
        }

        /**
         * <summary>
         *   Returns the 3D sensor calibration state (Yocto-3D-V2 only).
         * <para>
         *   This function returns
         *   an integer representing the calibration state of the 3 inertial sensors of
         *   the BNO055 chip, found in the Yocto-3D-V2. Hundredths show the calibration state
         *   of the accelerometer, tenths show the calibration state of the magnetometer while
         *   units show the calibration state of the gyroscope. For each sensor, the value 0
         *   means no calibration and the value 3 means full calibration.
         * </para>
         * </summary>
         * <returns>
         *   an integer representing the calibration state of Yocto-3D-V2:
         *   333 when fully calibrated, 0 when not calibrated at all.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         *   For the Yocto-3D (V1), this function always return -3 (unsupported function).
         * </para>
         */
        public virtual int get_calibrationState()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_calibrationState();
        }

        /**
         * <summary>
         *   Returns estimated quality of the orientation (Yocto-3D-V2 only).
         * <para>
         *   This function returns
         *   an integer between 0 and 3 representing the degree of confidence of the position
         *   estimate. When the value is 3, the estimation is reliable. Below 3, one should
         *   expect sudden corrections, in particular for heading (<c>compass</c> function).
         *   The most frequent causes for values below 3 are magnetic interferences, and
         *   accelerations or rotations beyond the sensor range.
         * </para>
         * </summary>
         * <returns>
         *   an integer between 0 and 3 (3 when the measure is reliable)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         *   For the Yocto-3D (V1), this function always return -3 (unsupported function).
         * </para>
         */
        public virtual int get_measureQuality()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_measureQuality();
        }

        /**
         * <summary>
         *   Initiates the sensors tridimensional calibration process.
         * <para>
         *   This calibration is used at low level for inertial position estimation
         *   and to enhance the precision of the tilt sensors.
         * </para>
         * <para>
         *   After calling this method, the device should be moved according to the
         *   instructions provided by method <c>get_3DCalibrationHint</c>,
         *   and <c>more3DCalibration</c> should be invoked about 5 times per second.
         *   The calibration procedure is completed when the method
         *   <c>get_3DCalibrationProgress</c> returns 100. At this point,
         *   the computed calibration parameters can be applied using method
         *   <c>save3DCalibration</c>. The calibration process can be cancelled
         *   at any time using method <c>cancel3DCalibration</c>.
         * </para>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         * </summary>
         */
        public virtual int start3DCalibration()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.start3DCalibration();
        }

        /**
         * <summary>
         *   Continues the sensors tridimensional calibration process previously
         *   initiated using method <c>start3DCalibration</c>.
         * <para>
         *   This method should be called approximately 5 times per second, while
         *   positioning the device according to the instructions provided by method
         *   <c>get_3DCalibrationHint</c>. Note that the instructions change during
         *   the calibration process.
         * </para>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         * </summary>
         */
        public virtual int more3DCalibration()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.more3DCalibration();
        }

        /**
         * <summary>
         *   Returns instructions to proceed to the tridimensional calibration initiated with
         *   method <c>start3DCalibration</c>.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a character string.
         * </returns>
         */
        public virtual string get_3DCalibrationHint()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_3DCalibrationHint();
        }

        /**
         * <summary>
         *   Returns the global process indicator for the tridimensional calibration
         *   initiated with method <c>start3DCalibration</c>.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer between 0 (not started) and 100 (stage completed).
         * </returns>
         */
        public virtual int get_3DCalibrationProgress()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_3DCalibrationProgress();
        }

        /**
         * <summary>
         *   Returns index of the current stage of the calibration
         *   initiated with method <c>start3DCalibration</c>.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer, growing each time a calibration stage is completed.
         * </returns>
         */
        public virtual int get_3DCalibrationStage()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_3DCalibrationStage();
        }

        /**
         * <summary>
         *   Returns the process indicator for the current stage of the calibration
         *   initiated with method <c>start3DCalibration</c>.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer between 0 (not started) and 100 (stage completed).
         * </returns>
         */
        public virtual int get_3DCalibrationStageProgress()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_3DCalibrationStageProgress();
        }

        /**
         * <summary>
         *   Returns the latest log message from the calibration process.
         * <para>
         *   When no new message is available, returns an empty string.
         * </para>
         * </summary>
         * <returns>
         *   a character string.
         * </returns>
         */
        public virtual string get_3DCalibrationLogMsg()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_3DCalibrationLogMsg();
        }

        /**
         * <summary>
         *   Applies the sensors tridimensional calibration parameters that have just been computed.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>  method of the module if the changes
         *   must be kept when the device is restarted.
         * </para>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         * </summary>
         */
        public virtual int save3DCalibration()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.save3DCalibration();
        }

        /**
         * <summary>
         *   Aborts the sensors tridimensional calibration process et restores normal settings.
         * <para>
         * </para>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         * </summary>
         */
        public virtual int cancel3DCalibration()
        {
            if (_func == null)
            {
                string msg = "No RefFrame connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.cancel3DCalibration();
        }
    }
    //--- (end of YRefFrame implementation)
}

