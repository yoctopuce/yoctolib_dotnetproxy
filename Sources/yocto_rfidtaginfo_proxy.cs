/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YRfidTagInfoProxy, the Proxy API for RfidTagInfo
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
    //--- (YRfidTagInfo class start)
    public class YRfidTagInfoProxy
    {
        public YRfidTagInfo _objref;
        internal YRfidTagInfoProxy(YRfidTagInfo objref)
        {
             _objref = objref;
        }
        //--- (end of YRfidTagInfo class start)
        //--- (YRfidTagInfo definitions)
    public const int IEC_15693 = 1;
    public const int IEC_14443 = 2;
    public const int IEC_14443_MIFARE_ULTRALIGHT = 3;
    public const int IEC_14443_MIFARE_CLASSIC1K = 4;
    public const int IEC_14443_MIFARE_CLASSIC4K = 5;
    public const int IEC_14443_MIFARE_DESFIRE = 6;
    public const int IEC_14443_NTAG_213 = 7;
    public const int IEC_14443_NTAG_215 = 8;
    public const int IEC_14443_NTAG_216 = 9;
    public const int IEC_14443_NTAG_424_DNA = 10;
        //--- (end of YRfidTagInfo definitions)
        //--- (YRfidTagInfo implementation)

        /**
         * <summary>
         *   Returns the RFID tag identifier.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string with the RFID tag identifier.
         * </returns>
         */
        public virtual string get_tagId()
        {
            return _objref.get_tagId();
        }

        // property with cached value for instant access (storage object)
        public string TagId
        {
            get
            {
                return this.get_tagId();
            }
        }

        /**
         * <summary>
         *   Returns the type of the RFID tag, as a numeric constant.
         * <para>
         *   (<c>IEC_14443_MIFARE_CLASSIC1K</c>, ...).
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the RFID tag type
         * </returns>
         */
        public virtual int get_tagType()
        {
            return _objref.get_tagType();
        }

        // property with cached value for instant access (storage object)
        public int TagType
        {
            get
            {
                return this.get_tagType();
            }
        }

        /**
         * <summary>
         *   Returns the type of the RFID tag, as a string.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the RFID tag type
         * </returns>
         */
        public virtual string get_tagTypeStr()
        {
            return _objref.get_tagTypeStr();
        }

        // property with cached value for instant access (storage object)
        public string TagTypeStr
        {
            get
            {
                return this.get_tagTypeStr();
            }
        }

        /**
         * <summary>
         *   Returns the total memory size of the RFID tag, in bytes.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the total memory size of the RFID tag
         * </returns>
         */
        public virtual int get_tagMemorySize()
        {
            return _objref.get_tagMemorySize();
        }

        // property with cached value for instant access (storage object)
        public int TagMemorySize
        {
            get
            {
                return this.get_tagMemorySize();
            }
        }

        /**
         * <summary>
         *   Returns the usable storage size of the RFID tag, in bytes.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the usable storage size of the RFID tag
         * </returns>
         */
        public virtual int get_tagUsableSize()
        {
            return _objref.get_tagUsableSize();
        }

        // property with cached value for instant access (storage object)
        public int TagUsableSize
        {
            get
            {
                return this.get_tagUsableSize();
            }
        }

        /**
         * <summary>
         *   Returns the block size of the RFID tag, in bytes.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the block size of the RFID tag
         * </returns>
         */
        public virtual int get_tagBlockSize()
        {
            return _objref.get_tagBlockSize();
        }

        // property with cached value for instant access (storage object)
        public int TagBlockSize
        {
            get
            {
                return this.get_tagBlockSize();
            }
        }

        /**
         * <summary>
         *   Returns the index of the first usable storage block on the RFID tag.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the index of the first usable storage block on the RFID tag
         * </returns>
         */
        public virtual int get_tagFirstBlock()
        {
            return _objref.get_tagFirstBlock();
        }

        // property with cached value for instant access (storage object)
        public int TagFirstBlock
        {
            get
            {
                return this.get_tagFirstBlock();
            }
        }

        /**
         * <summary>
         *   Returns the index of the last usable storage block on the RFID tag.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the index of the last usable storage block on the RFID tag
         * </returns>
         */
        public virtual int get_tagLastBlock()
        {
            return _objref.get_tagLastBlock();
        }

        // property with cached value for instant access (storage object)
        public int TagLastBlock
        {
            get
            {
                return this.get_tagLastBlock();
            }
        }
    }
    //--- (end of YRfidTagInfo implementation)
}

