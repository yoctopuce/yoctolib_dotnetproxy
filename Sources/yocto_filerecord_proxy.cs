/*********************************************************************
 *
 *  $Id: svn_id $
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
using YoctoLib;

namespace YoctoProxyAPI
{
    //--- (YFileRecord class start)
    public class YFileRecordProxy
    {
        private YFileRecord _objref;
        internal YFileRecordProxy(YFileRecord objref)
        {
             _objref = objref;
        }
        //--- (end of YFileRecord class start)
        //--- (YFileRecord definitions)
        //--- (end of YFileRecord definitions)
        //--- (YFileRecord implementation)

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
            return _objref.get_name();
        }

        // property with cached value for instant access (storage object)
        public string Name
        {
            get
            {
                return this.get_name();
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
            return _objref.get_size();
        }

        // property with cached value for instant access (storage object)
        public int Size
        {
            get
            {
                return this.get_size();
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
            return _objref.get_crc();
        }

        // property with cached value for instant access (storage object)
        public int Crc
        {
            get
            {
                return this.get_crc();
            }
        }
    }
    //--- (end of YFileRecord implementation)
}

