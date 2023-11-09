/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YRfidStatusProxy, the Proxy API for RfidStatus
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
    //--- (YRfidStatus class start)
    public class YRfidStatusProxy
    {
        public YRfidStatus _objref;
        internal YRfidStatusProxy(YRfidStatus objref)
        {
             _objref = objref;
        }
        //--- (end of YRfidStatus class start)
        //--- (YRfidStatus definitions)
    public const int SUCCESS = 0;
    public const int COMMAND_NOT_SUPPORTED = 1;
    public const int COMMAND_NOT_RECOGNIZED = 2;
    public const int COMMAND_OPTION_NOT_RECOGNIZED = 3;
    public const int COMMAND_CANNOT_BE_PROCESSED_IN_TIME = 4;
    public const int UNDOCUMENTED_ERROR = 15;
    public const int BLOCK_NOT_AVAILABLE = 16;
    public const int BLOCK_ALREADY_LOCKED = 17;
    public const int BLOCK_LOCKED = 18;
    public const int BLOCK_NOT_SUCESSFULLY_PROGRAMMED = 19;
    public const int BLOCK_NOT_SUCESSFULLY_LOCKED = 20;
    public const int BLOCK_IS_PROTECTED = 21;
    public const int CRYPTOGRAPHIC_ERROR = 64;
    public const int READER_BUSY = 1000;
    public const int TAG_NOTFOUND = 1001;
    public const int TAG_LEFT = 1002;
    public const int TAG_JUSTLEFT = 1003;
    public const int TAG_COMMUNICATION_ERROR = 1004;
    public const int TAG_NOT_RESPONDING = 1005;
    public const int TIMEOUT_ERROR = 1006;
    public const int COLLISION_DETECTED = 1007;
    public const int INVALID_CMD_ARGUMENTS = -66;
    public const int UNKNOWN_CAPABILITIES = -67;
    public const int MEMORY_NOT_SUPPORTED = -68;
    public const int INVALID_BLOCK_INDEX = -69;
    public const int MEM_SPACE_UNVERRUN_ATTEMPT = -70;
    public const int BROWNOUT_DETECTED = -71     ;
    public const int BUFFER_OVERFLOW = -72;
    public const int CRC_ERROR = -73;
    public const int COMMAND_RECEIVE_TIMEOUT = -75;
    public const int DID_NOT_SLEEP = -76;
    public const int ERROR_DECIMAL_EXPECTED = -77;
    public const int HARDWARE_FAILURE = -78;
    public const int ERROR_HEX_EXPECTED = -79;
    public const int FIFO_LENGTH_ERROR = -80;
    public const int FRAMING_ERROR = -81;
    public const int NOT_IN_CNR_MODE = -82;
    public const int NUMBER_OU_OF_RANGE = -83;
    public const int NOT_SUPPORTED = -84;
    public const int NO_RF_FIELD_ACTIVE = -85;
    public const int READ_DATA_LENGTH_ERROR = -86;
    public const int WATCHDOG_RESET = -87;
    public const int UNKNOW_COMMAND = -91;
    public const int UNKNOW_ERROR = -92;
    public const int UNKNOW_PARAMETER = -93;
    public const int UART_RECEIVE_ERROR = -94;
    public const int WRONG_DATA_LENGTH = -95;
    public const int WRONG_MODE = -96;
    public const int UNKNOWN_DWARFxx_ERROR_CODE = -97;
    public const int RESPONSE_SHORT = -98;
    public const int UNEXPECTED_TAG_ID_IN_RESPONSE = -99;
    public const int UNEXPECTED_TAG_INDEX = -100;
    public const int READ_EOF = -101;
    public const int READ_OK_SOFAR = -102;
    public const int WRITE_DATA_MISSING = -103;
    public const int WRITE_TOO_MUCH_DATA = -104;
    public const int TRANSFER_CLOSED = -105;
    public const int COULD_NOT_BUILD_REQUEST = -106;
    public const int INVALID_OPTIONS = -107;
    public const int UNEXPECTED_RESPONSE = -108;
    public const int AFI_NOT_AVAILABLE = -109;
    public const int DSFID_NOT_AVAILABLE = -110;
    public const int TAG_RESPONSE_TOO_SHORT = -111;
    public const int DEC_EXPECTED = -112 ;
    public const int HEX_EXPECTED = -113;
    public const int NOT_SAME_SECOR = -114;
    public const int MIFARE_AUTHENTICATED = -115;
    public const int NO_DATABLOCK = -116;
    public const int KEYB_IS_READABLE = -117;
    public const int OPERATION_NOT_EXECUTED = -118;
    public const int BLOK_MODE_ERROR = -119;
    public const int BLOCK_NOT_WRITABLE = -120;
    public const int BLOCK_ACCESS_ERROR = -121;
    public const int BLOCK_NOT_AUTHENTICATED = -122;
    public const int ACCESS_KEY_BIT_NOT_WRITABLE = -123;
    public const int USE_KEYA_FOR_AUTH = -124;
    public const int USE_KEYB_FOR_AUTH = -125;
    public const int KEY_NOT_CHANGEABLE = -126;
    public const int BLOCK_TOO_HIGH = -127;
    public const int AUTH_ERR = -128;
    public const int NOKEY_SELECT = -129;
    public const int CARD_NOT_SELECTED = -130;
    public const int BLOCK_TO_READ_NONE = -131;
    public const int NO_TAG = -132;
    public const int TOO_MUCH_DATA = -133;
    public const int CON_NOT_SATISFIED = -134;
    public const int BLOCK_IS_SPECIAL = -135;
    public const int READ_BEYOND_ANNOUNCED_SIZE = -136;
    public const int BLOCK_ZERO_IS_RESERVED = -137;
    public const int VALUE_BLOCK_BAD_FORMAT = -138;
    public const int ISO15693_ONLY_FEATURE = -139;
    public const int ISO14443_ONLY_FEATURE = -140;
    public const int MIFARE_CLASSIC_ONLY_FEATURE = -141;
    public const int BLOCK_MIGHT_BE_PROTECTED = -142;
    public const int NO_SUCH_BLOCK = -143;
    public const int COUNT_TOO_BIG = -144;
    public const int UNKNOWN_MEM_SIZE = -145;
    public const int MORE_THAN_2BLOCKS_MIGHT_NOT_WORK = -146;
    public const int READWRITE_NOT_SUPPORTED = -147;
    public const int UNEXPECTED_VICC_ID_IN_RESPONSE = -148;
    public const int LOCKBLOCK_NOT_SUPPORTED = -150;
    public const int INTERNAL_ERROR_SHOULD_NEVER_HAPPEN = -151;
    public const int INVLD_BLOCK_MODE_COMBINATION = -152;
    public const int INVLD_ACCESS_MODE_COMBINATION = -153;
    public const int INVALID_SIZE = -154;
    public const int BAD_PASSWORD_FORMAT = -155;
        //--- (end of YRfidStatus definitions)
        //--- (YRfidStatus implementation)

        /**
         * <summary>
         *   Returns RFID tag identifier related to the status.
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
         *   Returns the detailled error code, or 0 if no error happened.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a numeric error code
         * </returns>
         */
        public virtual int get_errorCode()
        {
            return _objref.get_errorCode();
        }

        // property with cached value for instant access (storage object)
        public int ErrorCode
        {
            get
            {
                return this.get_errorCode();
            }
        }

        /**
         * <summary>
         *   Returns the RFID tag memory block number where the error was encountered, or -1 if the
         *   error is not specific to a memory block.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an RFID tag block number
         * </returns>
         */
        public virtual int get_errorBlock()
        {
            return _objref.get_errorBlock();
        }

        // property with cached value for instant access (storage object)
        public int ErrorBlock
        {
            get
            {
                return this.get_errorBlock();
            }
        }

        /**
         * <summary>
         *   Returns a string describing precisely the RFID commande result.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an error message string
         * </returns>
         */
        public virtual string get_errorMessage()
        {
            return _objref.get_errorMessage();
        }

        // property with cached value for instant access (storage object)
        public string ErrorMessage
        {
            get
            {
                return this.get_errorMessage();
            }
        }

        /**
         * <summary>
         *   Returns the block number of the first RFID tag memory block affected
         *   by the operation.
         * <para>
         *   Depending on the type of operation and on the tag
         *   memory granularity, this number may be smaller than the requested
         *   memory block index.
         * </para>
         * </summary>
         * <returns>
         *   an RFID tag block number
         * </returns>
         */
        public virtual int get_firstAffectedBlock()
        {
            return _objref.get_firstAffectedBlock();
        }

        // property with cached value for instant access (storage object)
        public int FirstAffectedBlock
        {
            get
            {
                return this.get_firstAffectedBlock();
            }
        }

        /**
         * <summary>
         *   Returns the block number of the last RFID tag memory block affected
         *   by the operation.
         * <para>
         *   Depending on the type of operation and on the tag
         *   memory granularity, this number may be bigger than the requested
         *   memory block index.
         * </para>
         * </summary>
         * <returns>
         *   an RFID tag block number
         * </returns>
         */
        public virtual int get_lastAffectedBlock()
        {
            return _objref.get_lastAffectedBlock();
        }

        // property with cached value for instant access (storage object)
        public int LastAffectedBlock
        {
            get
            {
                return this.get_lastAffectedBlock();
            }
        }
    }
    //--- (end of YRfidStatus implementation)
}

