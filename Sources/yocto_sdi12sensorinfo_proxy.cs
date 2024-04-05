/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YSdi12SensorInfoProxy, the Proxy API for Sdi12SensorInfo
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
    //--- (YSdi12SensorInfo class start)
    public class YSdi12SensorInfoProxy
    {
        public YSdi12SensorInfo _objref;
        internal YSdi12SensorInfoProxy(YSdi12SensorInfo objref)
        {
             _objref = objref;
        }
        //--- (end of YSdi12SensorInfo class start)
        //--- (YSdi12SensorInfo definitions)
        //--- (end of YSdi12SensorInfo definitions)
        //--- (YSdi12SensorInfo implementation)

        /**
         * <summary>
         *   Returns the sensor state.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the sensor state.
         * </returns>
         */
        public virtual bool isValid()
        {
            return _objref.isValid();
        }

        // property with cached value for instant access (storage object)
        public bool IsValid
        {
            get
            {
                return this.isValid();
            }
        }

        /**
         * <summary>
         *   Returns the sensor address.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the sensor address.
         * </returns>
         */
        public virtual string get_sensorAddress()
        {
            return _objref.get_sensorAddress();
        }

        // property with cached value for instant access (storage object)
        public string SensorAddress
        {
            get
            {
                return this.get_sensorAddress();
            }
        }

        /**
         * <summary>
         *   Returns the compatible SDI-12 version of the sensor.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the compatible SDI-12 version of the sensor.
         * </returns>
         */
        public virtual string get_sensorProtocol()
        {
            return _objref.get_sensorProtocol();
        }

        // property with cached value for instant access (storage object)
        public string SensorProtocol
        {
            get
            {
                return this.get_sensorProtocol();
            }
        }

        /**
         * <summary>
         *   Returns the sensor vendor identification.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the sensor vendor identification.
         * </returns>
         */
        public virtual string get_sensorVendor()
        {
            return _objref.get_sensorVendor();
        }

        // property with cached value for instant access (storage object)
        public string SensorVendor
        {
            get
            {
                return this.get_sensorVendor();
            }
        }

        /**
         * <summary>
         *   Returns the sensor model number.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the sensor model number.
         * </returns>
         */
        public virtual string get_sensorModel()
        {
            return _objref.get_sensorModel();
        }

        // property with cached value for instant access (storage object)
        public string SensorModel
        {
            get
            {
                return this.get_sensorModel();
            }
        }

        /**
         * <summary>
         *   Returns the sensor version.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the sensor version.
         * </returns>
         */
        public virtual string get_sensorVersion()
        {
            return _objref.get_sensorVersion();
        }

        // property with cached value for instant access (storage object)
        public string SensorVersion
        {
            get
            {
                return this.get_sensorVersion();
            }
        }

        /**
         * <summary>
         *   Returns the sensor serial number.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the sensor serial number.
         * </returns>
         */
        public virtual string get_sensorSerial()
        {
            return _objref.get_sensorSerial();
        }

        // property with cached value for instant access (storage object)
        public string SensorSerial
        {
            get
            {
                return this.get_sensorSerial();
            }
        }

        /**
         * <summary>
         *   Returns the number of sensor measurements.
         * <para>
         *   This function only works if the sensor is in version 1.4 SDI-12
         *   and supports metadata commands.
         * </para>
         * </summary>
         * <returns>
         *   the number of sensor measurements.
         * </returns>
         */
        public virtual int get_measureCount()
        {
            return _objref.get_measureCount();
        }

        // property with cached value for instant access (storage object)
        public int MeasureCount
        {
            get
            {
                return this.get_measureCount();
            }
        }

        /**
         * <summary>
         *   Returns the sensor measurement command.
         * <para>
         *   This function only works if the sensor is in version 1.4 SDI-12
         *   and supports metadata commands.
         * </para>
         * </summary>
         * <param name="measureIndex">
         *   measurement index
         * </param>
         * <returns>
         *   the sensor measurement command.
         *   On failure, throws an exception or returns an empty string.
         * </returns>
         */
        public virtual string get_measureCommand(int measureIndex)
        {
            return _objref.get_measureCommand(measureIndex);
        }

        /**
         * <summary>
         *   Returns sensor measurement position.
         * <para>
         *   This function only works if the sensor is in version 1.4 SDI-12
         *   and supports metadata commands.
         * </para>
         * </summary>
         * <param name="measureIndex">
         *   measurement index
         * </param>
         * <returns>
         *   the sensor measurement command.
         *   On failure, throws an exception or returns 0.
         * </returns>
         */
        public virtual int get_measurePosition(int measureIndex)
        {
            return _objref.get_measurePosition(measureIndex);
        }

        /**
         * <summary>
         *   Returns the measured value symbol.
         * <para>
         *   This function only works if the sensor is in version 1.4 SDI-12
         *   and supports metadata commands.
         * </para>
         * </summary>
         * <param name="measureIndex">
         *   measurement index
         * </param>
         * <returns>
         *   the sensor measurement command.
         *   On failure, throws an exception or returns an empty string.
         * </returns>
         */
        public virtual string get_measureSymbol(int measureIndex)
        {
            return _objref.get_measureSymbol(measureIndex);
        }

        /**
         * <summary>
         *   Returns the unit of the measured value.
         * <para>
         *   This function only works if the sensor is in version 1.4 SDI-12
         *   and supports metadata commands.
         * </para>
         * </summary>
         * <param name="measureIndex">
         *   measurement index
         * </param>
         * <returns>
         *   the sensor measurement command.
         *   On failure, throws an exception or returns an empty string.
         * </returns>
         */
        public virtual string get_measureUnit(int measureIndex)
        {
            return _objref.get_measureUnit(measureIndex);
        }

        /**
         * <summary>
         *   Returns the description of the measured value.
         * <para>
         *   This function only works if the sensor is in version 1.4 SDI-12
         *   and supports metadata commands.
         * </para>
         * </summary>
         * <param name="measureIndex">
         *   measurement index
         * </param>
         * <returns>
         *   the sensor measurement command.
         *   On failure, throws an exception or returns an empty string.
         * </returns>
         */
        public virtual string get_measureDescription(int measureIndex)
        {
            return _objref.get_measureDescription(measureIndex);
        }
    }
    //--- (end of YSdi12SensorInfo implementation)
}

