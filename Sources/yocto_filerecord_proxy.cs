/*********************************************************************
 *
 *  $Id: yocto_filerecord_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YFileRecordProxy, the Proxy API for FileRecord
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
    //--- (generated code: YFileRecord class start)
    public class YFileRecordProxy
    {
        private YFileRecord _objref;
        internal YFileRecordProxy(YFileRecord objref)
        {
             _objref = objref;
        }
        //--- (end of generated code: YFileRecord class start)
        //--- (generated code: YFileRecord definitions)
        //--- (end of generated code: YFileRecord definitions)
        //--- (generated code: YFileRecord implementation)

        // property with cached value for instant access
        public string Name
        {
            get
            {
                return this.get_name();
            }
        }
        /**
         * <summary>
         *   Returns the name of the file.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string with the name of the file.
         * </returns>
         */
        public virtual string get_name()
        {
            if (_objref == null)
            {
                string msg = "No FileRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_name();
        }

        // property with cached value for instant access
        public int Size
        {
            get
            {
                return this.get_size();
            }
        }
        /**
         * <summary>
         *   Returns the size of the file in bytes.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the size of the file.
         * </returns>
         */
        public virtual int get_size()
        {
            if (_objref == null)
            {
                string msg = "No FileRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_size();
        }

        // property with cached value for instant access
        public int Crc
        {
            get
            {
                return this.get_crc();
            }
        }
        /**
         * <summary>
         *   Returns the 32-bit CRC of the file content.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the 32-bit CRC of the file content.
         * </returns>
         */
        public virtual int get_crc()
        {
            if (_objref == null)
            {
                string msg = "No FileRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_crc();
        }
    }
    //--- (end of generated code: YFileRecord implementation)
}

