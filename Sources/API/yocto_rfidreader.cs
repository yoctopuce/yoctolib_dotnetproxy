namespace YoctoLib 
{/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements yFindRfidReader(), the high-level API for RfidReader functions
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
//--- (generated code: YRfidTagInfo return codes)
//--- (end of generated code: YRfidTagInfo return codes)
//--- (generated code: YRfidTagInfo dlldef)
//--- (end of generated code: YRfidTagInfo dlldef)
//--- (generated code: YRfidTagInfo yapiwrapper)
//--- (end of generated code: YRfidTagInfo yapiwrapper)
//--- (generated code: YRfidTagInfo class start)
/**
 * <c>YRfidTagInfo</c> objects are used to describe RFID tag attributes,
 * such as the tag type and its storage size. These objects are returned by
 * method <c>get_tagInfo()</c> of class <c>YRfidReader</c>.
 * <para>
 * </para>
 */
public class YRfidTagInfo
{
//--- (end of generated code: YRfidTagInfo class start)
    //--- (generated code: YRfidTagInfo definitions)

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
    protected string _tagId;
    protected int _tagType = 0;
    protected string _typeStr;
    protected int _size = 0;
    protected int _usable = 0;
    protected int _blksize = 0;
    protected int _fblk = 0;
    protected int _lblk = 0;
    //--- (end of generated code: YRfidTagInfo definitions)

    public YRfidTagInfo()
    {
        //--- (generated code: YRfidTagInfo attributes initialization)
        //--- (end of generated code: YRfidTagInfo attributes initialization)
    }

    //--- (generated code: YRfidTagInfo implementation)



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
        return this._tagId;
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
        return this._tagType;
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
        return this._typeStr;
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
        return this._size;
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
        return this._usable;
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
        return this._blksize;
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
        return this._fblk;
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
        return this._lblk;
    }


    public virtual void imm_init(string tagId, int tagType, int size, int usable, int blksize, int fblk, int lblk)
    {
        string typeStr;
        typeStr = "unknown";
        if (tagType == IEC_15693) {
            typeStr = "IEC 15693";
        }
        if (tagType == IEC_14443) {
            typeStr = "IEC 14443";
        }
        if (tagType == IEC_14443_MIFARE_ULTRALIGHT) {
            typeStr = "MIFARE Ultralight";
        }
        if (tagType == IEC_14443_MIFARE_CLASSIC1K) {
            typeStr = "MIFARE Classic 1K";
        }
        if (tagType == IEC_14443_MIFARE_CLASSIC4K) {
            typeStr = "MIFARE Classic 4K";
        }
        if (tagType == IEC_14443_MIFARE_DESFIRE) {
            typeStr = "MIFARE DESFire";
        }
        if (tagType == IEC_14443_NTAG_213) {
            typeStr = "NTAG 213";
        }
        if (tagType == IEC_14443_NTAG_215) {
            typeStr = "NTAG 215";
        }
        if (tagType == IEC_14443_NTAG_216) {
            typeStr = "NTAG 216";
        }
        if (tagType == IEC_14443_NTAG_424_DNA) {
            typeStr = "NTAG 424 DNA";
        }
        this._tagId = tagId;
        this._tagType = tagType;
        this._typeStr = typeStr;
        this._size = size;
        this._usable = usable;
        this._blksize = blksize;
        this._fblk = fblk;
        this._lblk = lblk;
    }

    //--- (end of generated code: YRfidTagInfo implementation)

    //--- (generated code: YRfidTagInfo functions)

    //--- (end of generated code: YRfidTagInfo functions)
}
#pragma warning restore 1591


#pragma warning disable 1591
//--- (generated code: YRfidOptions return codes)
//--- (end of generated code: YRfidOptions return codes)
//--- (generated code: YRfidOptions dlldef)
//--- (end of generated code: YRfidOptions dlldef)
//--- (generated code: YRfidOptions yapiwrapper)
//--- (end of generated code: YRfidOptions yapiwrapper)
//--- (generated code: YRfidOptions class start)
/**
 * <c>YRfidOptions</c> objects are used to provide optional
 * parameters to RFID commands that interact with tags, and in
 * particular to provide security keys when required.
 * <para>
 * </para>
 */
public class YRfidOptions
{
//--- (end of generated code: YRfidOptions class start)
    //--- (generated code: YRfidOptions definitions)

    public const int NO_RFID_KEY = 0;
    public const int MIFARE_KEY_A = 1;
    public const int MIFARE_KEY_B = 2;

    /**
     * <summary>
     *   Type of security key to be used to access the RFID tag.
     * <para>
     *   For MIFARE Classic tags, allowed values are
     *   <c>Y_MIFARE_KEY_A</c> or <c>Y_MIFARE_KEY_B</c>.
     *   The default value is <c>Y_NO_RFID_KEY</c>, in that case
     *   the reader will use the most common default key for the
     *   tag type.
     *   When a security key is required, it must be provided
     *   using property <c>HexKey</c>.
     * </para>
     * </summary>
     */
    public int KeyType;

    /**
     * <summary>
     *   Security key to be used to access the RFID tag, as an
     *   hexadecimal string.
     * <para>
     *   The key will only be used if you
     *   also specify which type of key it is, using property
     *   <c>KeyType</c>.
     * </para>
     * </summary>
     */
    public string HexKey;

    /**
     * <summary>
     *   Force the use of single-block commands to access RFID tag memory blocks.
     * <para>
     *   By default, the Yoctopuce library uses the most efficient access strategy
     *   generally available for each tag type, but you can force the use of
     *   single-block commands if the RFID tags you are using do not support
     *   multi-block commands. If opération speed is not a priority, choose
     *   single-block mode as it will work with any mode.
     * </para>
     * </summary>
     */
    public bool ForceSingleBlockAccess;

    /**
     * <summary>
     *   Force the use of multi-block commands to access RFID tag memory blocks.
     * <para>
     *   By default, the Yoctopuce library uses the most efficient access strategy
     *   generally available for each tag type, but you can force the use of
     *   multi-block commands if you know for sure that the RFID tags you are using
     *   do support multi-block commands. Be  aware that even if a tag allows multi-block
     *   operations, the maximum number of blocks that can be written or read at the same
     *   time can be (very) limited. If the tag does not support multi-block mode
     *   for the wanted opération, the option will be ignored.
     * </para>
     * </summary>
     */
    public bool ForceMultiBlockAccess;

    /**
     * <summary>
     *   Enable direct access to RFID tag control blocks.
     * <para>
     *   By default, Yoctopuce library read and write functions only work
     *   on data blocks and automatically skip special blocks, as specific functions are provided
     *   to configure security parameters found in control blocks.
     *   If you need to access control blocks in your own way using
     *   read/write functions, enable this option.  Use this option wisely,
     *   as overwriting a special block migth very well irreversibly alter your
     *   tag behavior.
     * </para>
     * </summary>
     */
    public bool EnableRawAccess;

    /**
     * <summary>
     *   Disables the tag memory overflow test.
     * <para>
     *   By default, the Yoctopuce
     *   library's read/write functions detect overruns and do not run
     *   commands that are likely to fail. If you nevertheless wish to
     *   access more memory than the tag announces, you can try to use
     *   this option.
     * </para>
     * </summary>
     */
    public bool DisableBoundaryChecks;

    /**
     * <summary>
     *   Enable simulation mode to check the affected block range as well
     *   as access rights.
     * <para>
     *   When this option is active, the operation is
     *   not fully applied to the RFID tag but the affected block range
     *   is determined and the optional access key is tested on these blocks.
     *   The access key rights are not tested though. This option applies to
     *   write / configure operations only, it is ignored for read operations.
     * </para>
     * </summary>
     */
    public bool EnableDryRun;

    //--- (end of generated code: YRfidOptions definitions)

    public YRfidOptions()
    {
        //--- (generated code: YRfidOptions attributes initialization)
        //--- (end of generated code: YRfidOptions attributes initialization)
    }

    //--- (generated code: YRfidOptions implementation)



    public virtual string imm_getParams()
    {
        int opt;
        string res;
        if (this.ForceSingleBlockAccess) {
            opt = 1;
        } else {
            opt = 0;
        }
        if (this.ForceMultiBlockAccess) {
            opt = ((opt) | (2));
        }
        if (this.EnableRawAccess) {
            opt = ((opt) | (4));
        }
        if (this.DisableBoundaryChecks) {
            opt = ((opt) | (8));
        }
        if (this.EnableDryRun) {
            opt = ((opt) | (16));
        }
        res = "&o="+Convert.ToString(opt);
        if (this.KeyType != 0) {
            res = ""+ res+"&k="+String.Format("{0:x02}", this.KeyType)+":"+this.HexKey;
        }
        return res;
    }

    //--- (end of generated code: YRfidOptions implementation)

    //--- (generated code: YRfidOptions functions)

    //--- (end of generated code: YRfidOptions functions)
}
#pragma warning restore 1591


#pragma warning disable 1591
//--- (generated code: YRfidStatus return codes)
//--- (end of generated code: YRfidStatus return codes)
//--- (generated code: YRfidStatus dlldef)
//--- (end of generated code: YRfidStatus dlldef)
//--- (generated code: YRfidStatus yapiwrapper)
//--- (end of generated code: YRfidStatus yapiwrapper)
//--- (generated code: YRfidStatus class start)
/**
 * <c>YRfidStatus</c> objects provide additional information about
 * operations on RFID tags, including the range of blocks affected
 * by read/write operations and possible errors when communicating
 * with RFID tags.
 * This makes it possible, for example, to distinguish communication
 * errors that can be recovered by an additional attempt, from
 * security or other errors on the tag.
 * <para>
 * </para>
 */
public class YRfidStatus
{
//--- (end of generated code: YRfidStatus class start)
    //--- (generated code: YRfidStatus definitions)

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
    protected string _tagId;
    protected int _errCode = 0;
    protected int _errBlk = 0;
    protected string _errMsg;
    protected int _yapierr = 0;
    protected int _fab = 0;
    protected int _lab = 0;
    //--- (end of generated code: YRfidStatus definitions)

    public YRfidStatus()
    {
        //--- (generated code: YRfidStatus attributes initialization)
        //--- (end of generated code: YRfidStatus attributes initialization)
    }

    //--- (generated code: YRfidStatus implementation)



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
        return this._tagId;
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
        return this._errCode;
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
        return this._errBlk;
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
        return this._errMsg;
    }


    public virtual int get_yapiError()
    {
        return this._yapierr;
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
        return this._fab;
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
        return this._lab;
    }


    public virtual void imm_init(string tagId, int errCode, int errBlk, int fab, int lab)
    {
        string errMsg;
        if (errCode == 0) {
            this._yapierr = YAPI.SUCCESS;
            errMsg = "Success (no error)";
        } else {
            if (errCode < 0) {
                if (errCode > -50) {
                    this._yapierr = errCode;
                    errMsg = "YoctoLib error "+Convert.ToString(errCode);
                } else {
                    this._yapierr = YAPI.RFID_HARD_ERROR;
                    errMsg = "Non-recoverable RFID error "+Convert.ToString(errCode);
                }
            } else {
                if (errCode > 1000) {
                    this._yapierr = YAPI.RFID_SOFT_ERROR;
                    errMsg = "Recoverable RFID error "+Convert.ToString(errCode);
                } else {
                    this._yapierr = YAPI.RFID_HARD_ERROR;
                    errMsg = "Non-recoverable RFID error "+Convert.ToString(errCode);
                }
            }
            if (errCode == TAG_NOTFOUND) {
                errMsg = "Tag not found";
            }
            if (errCode == TAG_JUSTLEFT) {
                errMsg = "Tag left during operation";
            }
            if (errCode == TAG_LEFT) {
                errMsg = "Tag not here anymore";
            }
            if (errCode == READER_BUSY) {
                errMsg = "Reader is busy";
            }
            if (errCode == INVALID_CMD_ARGUMENTS) {
                errMsg = "Invalid command arguments";
            }
            if (errCode == UNKNOWN_CAPABILITIES) {
                errMsg = "Unknown capabilities";
            }
            if (errCode == MEMORY_NOT_SUPPORTED) {
                errMsg = "Memory no present";
            }
            if (errCode == INVALID_BLOCK_INDEX) {
                errMsg = "Invalid block index";
            }
            if (errCode == MEM_SPACE_UNVERRUN_ATTEMPT) {
                errMsg = "Tag memory space overrun attempt";
            }
            if (errCode == COMMAND_NOT_SUPPORTED) {
                errMsg = "The command is not supported";
            }
            if (errCode == COMMAND_NOT_RECOGNIZED) {
                errMsg = "The command is not recognized";
            }
            if (errCode == COMMAND_OPTION_NOT_RECOGNIZED) {
                errMsg = "The command option is not supported.";
            }
            if (errCode == COMMAND_CANNOT_BE_PROCESSED_IN_TIME) {
                errMsg = "The command cannot be processed in time";
            }
            if (errCode == UNDOCUMENTED_ERROR) {
                errMsg = "Error with no information given";
            }
            if (errCode == BLOCK_NOT_AVAILABLE) {
                errMsg = "Block is not available";
            }
            if (errCode == BLOCK_ALREADY_LOCKED) {
                errMsg = "Block is already locked and thus cannot be locked again.";
            }
            if (errCode == BLOCK_LOCKED) {
                errMsg = "Block is locked and its content cannot be changed";
            }
            if (errCode == BLOCK_NOT_SUCESSFULLY_PROGRAMMED) {
                errMsg = "Block was not successfully programmed";
            }
            if (errCode == BLOCK_NOT_SUCESSFULLY_LOCKED) {
                errMsg = "Block was not successfully locked";
            }
            if (errCode == BLOCK_IS_PROTECTED) {
                errMsg = "Block is protected";
            }
            if (errCode == CRYPTOGRAPHIC_ERROR) {
                errMsg = "Generic cryptographic error";
            }
            if (errCode == BROWNOUT_DETECTED) {
                errMsg = "BrownOut detected (BOD)";
            }
            if (errCode == BUFFER_OVERFLOW) {
                errMsg = "Buffer Overflow (BOF)";
            }
            if (errCode == CRC_ERROR) {
                errMsg = "Communication CRC Error (CCE)";
            }
            if (errCode == COLLISION_DETECTED) {
                errMsg = "Collision Detected (CLD/CDT)";
            }
            if (errCode == COMMAND_RECEIVE_TIMEOUT) {
                errMsg = "Command Receive Timeout (CRT)";
            }
            if (errCode == DID_NOT_SLEEP) {
                errMsg = "Did Not Sleep (DNS)";
            }
            if (errCode == ERROR_DECIMAL_EXPECTED) {
                errMsg = "Error Decimal Expected (EDX)";
            }
            if (errCode == HARDWARE_FAILURE) {
                errMsg = "Error Hardware Failure (EHF)";
            }
            if (errCode == ERROR_HEX_EXPECTED) {
                errMsg = "Error Hex Expected (EHX)";
            }
            if (errCode == FIFO_LENGTH_ERROR) {
                errMsg = "FIFO length error (FLE)";
            }
            if (errCode == FRAMING_ERROR) {
                errMsg = "Framing error (FER)";
            }
            if (errCode == NOT_IN_CNR_MODE) {
                errMsg = "Not in CNR Mode (NCM)";
            }
            if (errCode == NUMBER_OU_OF_RANGE) {
                errMsg = "Number Out of Range (NOR)";
            }
            if (errCode == NOT_SUPPORTED) {
                errMsg = "Not Supported (NOS)";
            }
            if (errCode == NO_RF_FIELD_ACTIVE) {
                errMsg = "No RF field active (NRF)";
            }
            if (errCode == READ_DATA_LENGTH_ERROR) {
                errMsg = "Read data length error (RDL)";
            }
            if (errCode == WATCHDOG_RESET) {
                errMsg = "Watchdog reset (SRT)";
            }
            if (errCode == TAG_COMMUNICATION_ERROR) {
                errMsg = "Tag Communication Error (TCE)";
            }
            if (errCode == TAG_NOT_RESPONDING) {
                errMsg = "Tag Not Responding (TNR)";
            }
            if (errCode == TIMEOUT_ERROR) {
                errMsg = "TimeOut Error (TOE)";
            }
            if (errCode == UNKNOW_COMMAND) {
                errMsg = "Unknown Command (UCO)";
            }
            if (errCode == UNKNOW_ERROR) {
                errMsg = "Unknown error (UER)";
            }
            if (errCode == UNKNOW_PARAMETER) {
                errMsg = "Unknown Parameter (UPA)";
            }
            if (errCode == UART_RECEIVE_ERROR) {
                errMsg = "UART Receive Error (URE)";
            }
            if (errCode == WRONG_DATA_LENGTH) {
                errMsg = "Wrong Data Length (WDL)";
            }
            if (errCode == WRONG_MODE) {
                errMsg = "Wrong Mode (WMO)";
            }
            if (errCode == UNKNOWN_DWARFxx_ERROR_CODE) {
                errMsg = "Unknown DWARF15 error code";
            }
            if (errCode == UNEXPECTED_TAG_ID_IN_RESPONSE) {
                errMsg = "Unexpected Tag id in response";
            }
            if (errCode == UNEXPECTED_TAG_INDEX) {
                errMsg = "internal error : unexpected TAG index";
            }
            if (errCode == TRANSFER_CLOSED) {
                errMsg = "transfer closed";
            }
            if (errCode == WRITE_DATA_MISSING) {
                errMsg = "Missing write data";
            }
            if (errCode == WRITE_TOO_MUCH_DATA) {
                errMsg = "Attempt to write too much data";
            }
            if (errCode == COULD_NOT_BUILD_REQUEST) {
                errMsg = "Could not not request";
            }
            if (errCode == INVALID_OPTIONS) {
                errMsg = "Invalid transfer options";
            }
            if (errCode == UNEXPECTED_RESPONSE) {
                errMsg = "Unexpected Tag response";
            }
            if (errCode == AFI_NOT_AVAILABLE) {
                errMsg = "AFI not available";
            }
            if (errCode == DSFID_NOT_AVAILABLE) {
                errMsg = "DSFID not available";
            }
            if (errCode == TAG_RESPONSE_TOO_SHORT) {
                errMsg = "Tag's response too short";
            }
            if (errCode == DEC_EXPECTED) {
                errMsg = "Error Decimal value Expected, or is missing";
            }
            if (errCode == HEX_EXPECTED) {
                errMsg = "Error Hexadecimal value Expected, or is missing";
            }
            if (errCode == NOT_SAME_SECOR) {
                errMsg = "Input and Output block are not in the same Sector";
            }
            if (errCode == MIFARE_AUTHENTICATED) {
                errMsg = "No chip with MIFARE Classic technology Authenticated";
            }
            if (errCode == NO_DATABLOCK) {
                errMsg = "No Data Block";
            }
            if (errCode == KEYB_IS_READABLE) {
                errMsg = "Key B is Readable";
            }
            if (errCode == OPERATION_NOT_EXECUTED) {
                errMsg = "Operation Not Executed, would have caused an overflow";
            }
            if (errCode == BLOK_MODE_ERROR) {
                errMsg = "Block has not been initialized as a 'value block'";
            }
            if (errCode == BLOCK_NOT_WRITABLE) {
                errMsg = "Block Not Writable";
            }
            if (errCode == BLOCK_ACCESS_ERROR) {
                errMsg = "Block Access Error";
            }
            if (errCode == BLOCK_NOT_AUTHENTICATED) {
                errMsg = "Block Not Authenticated";
            }
            if (errCode == ACCESS_KEY_BIT_NOT_WRITABLE) {
                errMsg = "Access bits or Keys not Writable";
            }
            if (errCode == USE_KEYA_FOR_AUTH) {
                errMsg = "Use Key B for authentication";
            }
            if (errCode == USE_KEYB_FOR_AUTH) {
                errMsg = "Use Key A for authentication";
            }
            if (errCode == KEY_NOT_CHANGEABLE) {
                errMsg = "Key(s) not changeable";
            }
            if (errCode == BLOCK_TOO_HIGH) {
                errMsg = "Block index is too high";
            }
            if (errCode == AUTH_ERR) {
                errMsg = "Authentication Error (i.e. wrong key)";
            }
            if (errCode == NOKEY_SELECT) {
                errMsg = "No Key Select, select a temporary or a static key";
            }
            if (errCode == CARD_NOT_SELECTED) {
                errMsg = " Card is Not Selected";
            }
            if (errCode == BLOCK_TO_READ_NONE) {
                errMsg = "Number of Blocks to Read is 0";
            }
            if (errCode == NO_TAG) {
                errMsg = "No Tag detected";
            }
            if (errCode == TOO_MUCH_DATA) {
                errMsg = "Too Much Data (i.e. Uart input buffer overflow)";
            }
            if (errCode == CON_NOT_SATISFIED) {
                errMsg = "Conditions Not Satisfied";
            }
            if (errCode == BLOCK_IS_SPECIAL) {
                errMsg = "Bad parameter: block is a special block";
            }
            if (errCode == READ_BEYOND_ANNOUNCED_SIZE) {
                errMsg = "Attempt to read more than announced size.";
            }
            if (errCode == BLOCK_ZERO_IS_RESERVED) {
                errMsg = "Block 0 is reserved and cannot be used";
            }
            if (errCode == VALUE_BLOCK_BAD_FORMAT) {
                errMsg = "One value block is not properly initialized";
            }
            if (errCode == ISO15693_ONLY_FEATURE) {
                errMsg = "Feature available on ISO 15693 only";
            }
            if (errCode == ISO14443_ONLY_FEATURE) {
                errMsg = "Feature available on ISO 14443 only";
            }
            if (errCode == MIFARE_CLASSIC_ONLY_FEATURE) {
                errMsg = "Feature available on ISO 14443 MIFARE Classic only";
            }
            if (errCode == BLOCK_MIGHT_BE_PROTECTED) {
                errMsg = "Block might be protected";
            }
            if (errCode == NO_SUCH_BLOCK) {
                errMsg = "No such block";
            }
            if (errCode == COUNT_TOO_BIG) {
                errMsg = "Count parameter is too large";
            }
            if (errCode == UNKNOWN_MEM_SIZE) {
                errMsg = "Tag memory size is unknown";
            }
            if (errCode == MORE_THAN_2BLOCKS_MIGHT_NOT_WORK) {
                errMsg = "Writing more than two blocks at once might not be supported by this tag";
            }
            if (errCode == READWRITE_NOT_SUPPORTED) {
                errMsg = "Read/write operation not supported for this tag";
            }
            if (errCode == UNEXPECTED_VICC_ID_IN_RESPONSE) {
                errMsg = "Unexpected VICC ID in response";
            }
            if (errCode == LOCKBLOCK_NOT_SUPPORTED) {
                errMsg = "This tag does not support the Lock block function";
            }
            if (errCode == INTERNAL_ERROR_SHOULD_NEVER_HAPPEN) {
                errMsg = "Yoctopuce RFID code ran into an unexpected state, please contact support";
            }
            if (errCode == INVLD_BLOCK_MODE_COMBINATION) {
                errMsg = "Invalid combination of block mode options";
            }
            if (errCode == INVLD_ACCESS_MODE_COMBINATION) {
                errMsg = "Invalid combination of access mode options";
            }
            if (errCode == INVALID_SIZE) {
                errMsg = "Invalid data size parameter";
            }
            if (errCode == BAD_PASSWORD_FORMAT) {
                errMsg = "Bad password format or type";
            }
            if (errBlk >= 0) {
                errMsg = ""+ errMsg+" (block "+Convert.ToString(errBlk)+")";
            }
        }
        this._tagId = tagId;
        this._errCode = errCode;
        this._errBlk = errBlk;
        this._errMsg = errMsg;
        this._fab = fab;
        this._lab = lab;
    }

    //--- (end of generated code: YRfidStatus implementation)

    //--- (generated code: YRfidStatus functions)

    //--- (end of generated code: YRfidStatus functions)
}
#pragma warning restore 1591


#pragma warning disable 1591
//--- (generated code: YRfidReader return codes)
//--- (end of generated code: YRfidReader return codes)
//--- (generated code: YRfidReader dlldef)
//--- (end of generated code: YRfidReader dlldef)
//--- (generated code: YRfidReader yapiwrapper)
//--- (end of generated code: YRfidReader yapiwrapper)
//--- (generated code: YRfidReader class start)
/**
 * <summary>
 *   The <c>RfidReader</c> class provides access detect,
 *   read and write RFID tags.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YRfidReader : YFunction
{
//--- (end of generated code: YRfidReader class start)
    //--- (generated code: YRfidReader definitions)
    public new delegate void ValueCallback(YRfidReader func, string value);
    public new delegate void TimedReportCallback(YRfidReader func, YMeasure measure);
    public delegate void YEventCallback(YRfidReader obj, int timestamp, string eventType, string eventData);

    protected static void yInternalEventCallback(YRfidReader obj, String value)
    {
        obj._internalEventHandler(value);
    }


    public const int NTAGS_INVALID = YAPI.INVALID_UINT;
    public const int REFRESHRATE_INVALID = YAPI.INVALID_UINT;
    protected int _nTags = NTAGS_INVALID;
    protected int _refreshRate = REFRESHRATE_INVALID;
    protected ValueCallback _valueCallbackRfidReader = null;
    protected YEventCallback _eventCallback;
    protected bool _isFirstCb;
    protected int _prevCbPos = 0;
    protected int _eventPos = 0;
    protected int _eventStamp = 0;
    //--- (end of generated code: YRfidReader definitions)

    public YRfidReader(string func)
        : base(func)
    {
        _className = "RfidReader";
        //--- (generated code: YRfidReader attributes initialization)
        //--- (end of generated code: YRfidReader attributes initialization)
    }

    //--- (generated code: YRfidReader implementation)

    protected override void _parseAttr(YAPI.YJSONObject json_val)
    {
        if (json_val.has("nTags"))
        {
            _nTags = json_val.getInt("nTags");
        }
        if (json_val.has("refreshRate"))
        {
            _refreshRate = json_val.getInt("refreshRate");
        }
        base._parseAttr(json_val);
    }


    /**
     * <summary>
     *   Returns the number of RFID tags currently detected.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of RFID tags currently detected
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRfidReader.NTAGS_INVALID</c>.
     * </para>
     */
    public int get_nTags()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return NTAGS_INVALID;
                }
            }
            res = this._nTags;
        }
        return res;
    }


    /**
     * <summary>
     *   Returns the tag list refresh rate, measured in Hz.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the tag list refresh rate, measured in Hz
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRfidReader.REFRESHRATE_INVALID</c>.
     * </para>
     */
    public int get_refreshRate()
    {
        int res;
        lock (_thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI._yapiContext.GetCacheValidity()) != YAPI.SUCCESS) {
                    return REFRESHRATE_INVALID;
                }
            }
            res = this._refreshRate;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the present tag list refresh rate, measured in Hz.
     * <para>
     *   The reader will do
     *   its best to respect it. Note that the reader cannot detect tag arrival or removal
     *   while it is  communicating with a tag.  Maximum frequency is limited to 100Hz,
     *   but in real life it will be difficult to do better than 50Hz.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the present tag list refresh rate, measured in Hz
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
    public int set_refreshRate(int newval)
    {
        string rest_val;
        lock (_thisLock) {
            rest_val = (newval).ToString();
            return _setAttr("refreshRate", rest_val);
        }
    }


    /**
     * <summary>
     *   Retrieves a RFID reader for a given identifier.
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
     *   This function does not require that the RFID reader is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YRfidReader.isOnline()</c> to test if the RFID reader is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a RFID reader by logical name, no error is notified: the first instance
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
     *   a string that uniquely characterizes the RFID reader, for instance
     *   <c>MyDevice.rfidReader</c>.
     * </param>
     * <returns>
     *   a <c>YRfidReader</c> object allowing you to drive the RFID reader.
     * </returns>
     */
    public static YRfidReader FindRfidReader(string func)
    {
        YRfidReader obj;
        lock (YAPI.globalLock) {
            obj = (YRfidReader) YFunction._FindFromCache("RfidReader", func);
            if (obj == null) {
                obj = new YRfidReader(func);
                YFunction._AddToCache("RfidReader", func, obj);
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
        this._valueCallbackRfidReader = callback;
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
        if (this._valueCallbackRfidReader != null) {
            this._valueCallbackRfidReader(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }


    public virtual int _chkerror(string tagId, byte[] json, ref YRfidStatus status)
    {
        string jsonStr;
        int errCode;
        int errBlk;
        int fab;
        int lab;
        int retcode;

        if ((json).Length == 0) {
            errCode = this.get_errorType();
            errBlk = -1;
            fab = -1;
            lab = -1;
        } else {
            jsonStr = YAPI.DefaultEncoding.GetString(json);
            errCode = YAPI._atoi(this._json_get_key(json, "err"));
            errBlk = YAPI._atoi(this._json_get_key(json, "errBlk"))-1;
            if ((jsonStr).IndexOf("\"fab\":") >= 0) {
                fab = YAPI._atoi(this._json_get_key(json, "fab"))-1;
                lab = YAPI._atoi(this._json_get_key(json, "lab"))-1;
            } else {
                fab = -1;
                lab = -1;
            }
        }
        status.imm_init(tagId, errCode, errBlk, fab, lab);
        retcode = status.get_yapiError();
        if (!(retcode == YAPI.SUCCESS)) {
            this._throw(retcode, status.get_errorMessage());
            return retcode;
        }
        return YAPI.SUCCESS;
    }


    public virtual int reset()
    {
        byte[] json = new byte[0];
        YRfidStatus status;
        status = new YRfidStatus();

        json = this._download("rfid.json?a=reset");
        return this._chkerror("", json, ref status);
    }


    /**
     * <summary>
     *   Returns the list of RFID tags currently detected by the reader.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a list of strings, corresponding to each tag identifier.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list.
     * </para>
     */
    public virtual List<string> get_tagIdList()
    {
        byte[] json = new byte[0];
        List<string> jsonList = new List<string>();
        List<string> taglist = new List<string>();

        json = this._download("rfid.json?a=list");
        taglist.Clear();
        if ((json).Length > 3) {
            jsonList = this._json_get_array(json);
            for (int ii_0 = 0; ii_0 <  jsonList.Count; ii_0++) {
                taglist.Add(this._json_get_string(YAPI.DefaultEncoding.GetBytes(jsonList[ii_0])));
            }
        }
        return taglist;
    }


    /**
     * <summary>
     *   Retourne la description des propriétés d'un tag RFID présent.
     * <para>
     *   Cette fonction peut causer des communications avec le tag.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to check
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty <c>YRfidTagInfo</c> objact.
     *   When it happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual YRfidTagInfo get_tagInfo(string tagId, ref YRfidStatus status)
    {
        string url;
        byte[] json = new byte[0];
        int tagType;
        int size;
        int usable;
        int blksize;
        int fblk;
        int lblk;
        YRfidTagInfo res;
        url = "rfid.json?a=info&t="+tagId;

        json = this._download(url);
        this._chkerror(tagId, json, ref status);
        tagType = YAPI._atoi(this._json_get_key(json, "type"));
        size = YAPI._atoi(this._json_get_key(json, "size"));
        usable = YAPI._atoi(this._json_get_key(json, "usable"));
        blksize = YAPI._atoi(this._json_get_key(json, "blksize"));
        fblk = YAPI._atoi(this._json_get_key(json, "fblk"));
        lblk = YAPI._atoi(this._json_get_key(json, "lblk"));
        res = new YRfidTagInfo();
        res.imm_init(tagId, tagType, size, usable, blksize, fblk, lblk);
        return res;
    }


    /**
     * <summary>
     *   Change an RFID tag configuration to prevents any further write to
     *   the selected blocks.
     * <para>
     *   This operation is definitive and irreversible.
     *   Depending on the tag type and block index, adjascent blocks may become
     *   read-only as well, based on the locking granularity.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   first block to lock
     * </param>
     * <param name="nBlocks">
     *   number of blocks to lock
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual int tagLockBlocks(string tagId, int firstBlock, int nBlocks, YRfidOptions options, ref YRfidStatus status)
    {
        string optstr;
        string url;
        byte[] json = new byte[0];
        optstr = options.imm_getParams();
        url = "rfid.json?a=lock&t="+tagId+"&b="+Convert.ToString(firstBlock)+"&n="+Convert.ToString(nBlocks)+""+optstr;

        json = this._download(url);
        return this._chkerror(tagId, json, ref status);
    }


    /**
     * <summary>
     *   Reads the locked state for RFID tag memory data blocks.
     * <para>
     *   FirstBlock cannot be a special block, and any special
     *   block encountered in the middle of the read operation will be
     *   skipped automatically.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   number of the first block to check
     * </param>
     * <param name="nBlocks">
     *   number of blocks to check
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   a list of booleans with the lock state of selected blocks
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual List<bool> get_tagLockState(string tagId, int firstBlock, int nBlocks, YRfidOptions options, ref YRfidStatus status)
    {
        string optstr;
        string url;
        byte[] json = new byte[0];
        byte[] binRes = new byte[0];
        List<bool> res = new List<bool>();
        int idx;
        int val;
        bool isLocked;
        optstr = options.imm_getParams();
        url = "rfid.json?a=chkl&t="+tagId+"&b="+Convert.ToString(firstBlock)+"&n="+Convert.ToString(nBlocks)+""+optstr;

        json = this._download(url);
        this._chkerror(tagId, json, ref status);
        if (status.get_yapiError() != YAPI.SUCCESS) {
            return res;
        }
        binRes = YAPI._hexStrToBin(this._json_get_key(json, "bitmap"));
        idx = 0;
        while (idx < nBlocks) {
            val = binRes[((idx) >> (3))];
            isLocked = (((val) & (((1) << (((idx) & (7)))))) != 0);
            res.Add(isLocked);
            idx = idx + 1;
        }
        return res;
    }


    /**
     * <summary>
     *   Tells which block of a RFID tag memory are special and cannot be used
     *   to store user data.
     * <para>
     *   Mistakely writing a special block can lead to
     *   an irreversible alteration of the tag.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   number of the first block to check
     * </param>
     * <param name="nBlocks">
     *   number of blocks to check
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   a list of booleans with the lock state of selected blocks
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual List<bool> get_tagSpecialBlocks(string tagId, int firstBlock, int nBlocks, YRfidOptions options, ref YRfidStatus status)
    {
        string optstr;
        string url;
        byte[] json = new byte[0];
        byte[] binRes = new byte[0];
        List<bool> res = new List<bool>();
        int idx;
        int val;
        bool isLocked;
        optstr = options.imm_getParams();
        url = "rfid.json?a=chks&t="+tagId+"&b="+Convert.ToString(firstBlock)+"&n="+Convert.ToString(nBlocks)+""+optstr;

        json = this._download(url);
        this._chkerror(tagId, json, ref status);
        if (status.get_yapiError() != YAPI.SUCCESS) {
            return res;
        }
        binRes = YAPI._hexStrToBin(this._json_get_key(json, "bitmap"));
        idx = 0;
        while (idx < nBlocks) {
            val = binRes[((idx) >> (3))];
            isLocked = (((val) & (((1) << (((idx) & (7)))))) != 0);
            res.Add(isLocked);
            idx = idx + 1;
        }
        return res;
    }


    /**
     * <summary>
     *   Reads data from an RFID tag memory, as an hexadecimal string.
     * <para>
     *   The read operation may span accross multiple blocks if the requested
     *   number of bytes is larger than the RFID tag block size. By default
     *   firstBlock cannot be a special block, and any special block encountered
     *   in the middle of the read operation will be skipped automatically.
     *   If you rather want to read special blocks, use EnableRawAccess option.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   block number where read should start
     * </param>
     * <param name="nBytes">
     *   total number of bytes to read
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   an hexadecimal string if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty binary buffer. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual string tagReadHex(string tagId, int firstBlock, int nBytes, YRfidOptions options, ref YRfidStatus status)
    {
        string optstr;
        string url;
        byte[] json = new byte[0];
        string hexbuf;
        optstr = options.imm_getParams();
        url = "rfid.json?a=read&t="+tagId+"&b="+Convert.ToString(firstBlock)+"&n="+Convert.ToString(nBytes)+""+optstr;

        json = this._download(url);
        this._chkerror(tagId, json, ref status);
        if (status.get_yapiError() == YAPI.SUCCESS) {
            hexbuf = this._json_get_key(json, "res");
        } else {
            hexbuf = "";
        }
        return hexbuf;
    }


    /**
     * <summary>
     *   Reads data from an RFID tag memory, as a binary buffer.
     * <para>
     *   The read operation
     *   may span accross multiple blocks if the requested number of bytes
     *   is larger than the RFID tag block size.  By default
     *   firstBlock cannot be a special block, and any special block encountered
     *   in the middle of the read operation will be skipped automatically.
     *   If you rather want to read special blocks, use EnableRawAccess option.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   block number where read should start
     * </param>
     * <param name="nBytes">
     *   total number of bytes to read
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   a binary object with the data read if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty binary buffer. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual byte[] tagReadBin(string tagId, int firstBlock, int nBytes, YRfidOptions options, ref YRfidStatus status)
    {
        return YAPI._hexStrToBin(this.tagReadHex(tagId, firstBlock, nBytes, options, ref status));
    }


    /**
     * <summary>
     *   Reads data from an RFID tag memory, as a byte list.
     * <para>
     *   The read operation
     *   may span accross multiple blocks if the requested number of bytes
     *   is larger than the RFID tag block size.  By default
     *   firstBlock cannot be a special block, and any special block encountered
     *   in the middle of the read operation will be skipped automatically.
     *   If you rather want to read special blocks, use EnableRawAccess option.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   block number where read should start
     * </param>
     * <param name="nBytes">
     *   total number of bytes to read
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   a byte list with the data read if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual List<int> tagReadArray(string tagId, int firstBlock, int nBytes, YRfidOptions options, ref YRfidStatus status)
    {
        byte[] blk = new byte[0];
        int idx;
        int endidx;
        List<int> res = new List<int>();
        blk = this.tagReadBin(tagId, firstBlock, nBytes, options, ref status);
        endidx = (blk).Length;
        idx = 0;
        while (idx < endidx) {
            res.Add(blk[idx]);
            idx = idx + 1;
        }
        return res;
    }


    /**
     * <summary>
     *   Reads data from an RFID tag memory, as a text string.
     * <para>
     *   The read operation
     *   may span accross multiple blocks if the requested number of bytes
     *   is larger than the RFID tag block size.  By default
     *   firstBlock cannot be a special block, and any special block encountered
     *   in the middle of the read operation will be skipped automatically.
     *   If you rather want to read special blocks, use EnableRawAccess option.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   block number where read should start
     * </param>
     * <param name="nChars">
     *   total number of characters to read
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   a text string with the data read if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual string tagReadStr(string tagId, int firstBlock, int nChars, YRfidOptions options, ref YRfidStatus status)
    {
        return YAPI.DefaultEncoding.GetString(this.tagReadBin(tagId, firstBlock, nChars, options, ref status));
    }


    /**
     * <summary>
     *   Writes data provided as a binary buffer to an RFID tag memory.
     * <para>
     *   The write operation may span accross multiple blocks if the
     *   number of bytes to write is larger than the RFID tag block size.
     *   By default firstBlock cannot be a special block, and any special block
     *   encountered in the middle of the write operation will be skipped
     *   automatically. If you rather want to rewrite special blocks as well,
     *   use EnableRawAccess option.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   block number where write should start
     * </param>
     * <param name="buff">
     *   the binary buffer to write
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual int tagWriteBin(string tagId, int firstBlock, byte[] buff, YRfidOptions options, ref YRfidStatus status)
    {
        string optstr;
        string hexstr;
        int buflen;
        string fname;
        byte[] json = new byte[0];
        buflen = (buff).Length;
        if (buflen <= 16) {
            // short data, use an URL-based command
            hexstr = YAPI._bytesToHexStr(buff, 0, buff.Length);
            return this.tagWriteHex(tagId, firstBlock, hexstr, options, ref status);
        } else {
            // long data, use an upload command
            optstr = options.imm_getParams();
            fname = "Rfid:t="+tagId+"&b="+Convert.ToString(firstBlock)+"&n="+Convert.ToString(buflen)+""+optstr;
            json = this._uploadEx(fname, buff);
            return this._chkerror(tagId, json, ref status);
        }
    }


    /**
     * <summary>
     *   Writes data provided as a list of bytes to an RFID tag memory.
     * <para>
     *   The write operation may span accross multiple blocks if the
     *   number of bytes to write is larger than the RFID tag block size.
     *   By default firstBlock cannot be a special block, and any special block
     *   encountered in the middle of the write operation will be skipped
     *   automatically. If you rather want to rewrite special blocks as well,
     *   use EnableRawAccess option.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   block number where write should start
     * </param>
     * <param name="byteList">
     *   a list of byte to write
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual int tagWriteArray(string tagId, int firstBlock, List<int> byteList, YRfidOptions options, ref YRfidStatus status)
    {
        int bufflen;
        byte[] buff = new byte[0];
        int idx;
        int hexb;
        bufflen = byteList.Count;
        buff = new byte[bufflen];
        idx = 0;
        while (idx < bufflen) {
            hexb = byteList[idx];
            buff[idx] = (byte)(hexb & 0xff);
            idx = idx + 1;
        }

        return this.tagWriteBin(tagId, firstBlock, buff, options, ref status);
    }


    /**
     * <summary>
     *   Writes data provided as an hexadecimal string to an RFID tag memory.
     * <para>
     *   The write operation may span accross multiple blocks if the
     *   number of bytes to write is larger than the RFID tag block size.
     *   By default firstBlock cannot be a special block, and any special block
     *   encountered in the middle of the write operation will be skipped
     *   automatically. If you rather want to rewrite special blocks as well,
     *   use EnableRawAccess option.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   block number where write should start
     * </param>
     * <param name="hexString">
     *   a string of hexadecimal byte codes to write
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual int tagWriteHex(string tagId, int firstBlock, string hexString, YRfidOptions options, ref YRfidStatus status)
    {
        int bufflen;
        string optstr;
        string url;
        byte[] json = new byte[0];
        byte[] buff = new byte[0];
        int idx;
        int hexb;
        bufflen = (hexString).Length;
        bufflen = ((bufflen) >> (1));
        if (bufflen <= 16) {
            // short data, use an URL-based command
            optstr = options.imm_getParams();
            url = "rfid.json?a=writ&t="+tagId+"&b="+Convert.ToString(firstBlock)+"&w="+hexString+""+optstr;
            json = this._download(url);
            return this._chkerror(tagId, json, ref status);
        } else {
            // long data, use an upload command
            buff = new byte[bufflen];
            idx = 0;
            while (idx < bufflen) {
                hexb = YAPI._hexStrToInt((hexString).Substring( 2 * idx, 2));
                buff[idx] = (byte)(hexb & 0xff);
                idx = idx + 1;
            }
            return this.tagWriteBin(tagId, firstBlock, buff, options, ref status);
        }
    }


    /**
     * <summary>
     *   Writes data provided as an ASCII string to an RFID tag memory.
     * <para>
     *   The write operation may span accross multiple blocks if the
     *   number of bytes to write is larger than the RFID tag block size.
     *   By default firstBlock cannot be a special block, and any special block
     *   encountered in the middle of the write operation will be skipped
     *   automatically. If you rather want to rewrite special blocks as well,
     *   use EnableRawAccess option.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="tagId">
     *   identifier of the tag to use
     * </param>
     * <param name="firstBlock">
     *   block number where write should start
     * </param>
     * <param name="text">
     *   the text string to write
     * </param>
     * <param name="options">
     *   an <c>YRfidOptions</c> object with the optional
     *   command execution parameters, such as security key
     *   if required
     * </param>
     * <param name="status">
     *   an <c>RfidStatus</c> object that will contain
     *   the detailled status of the operation
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code. When it
     *   happens, you can get more information from the <c>status</c> object.
     * </para>
     */
    public virtual int tagWriteStr(string tagId, int firstBlock, string text, YRfidOptions options, ref YRfidStatus status)
    {
        byte[] buff = new byte[0];
        buff = YAPI.DefaultEncoding.GetBytes(text);

        return this.tagWriteBin(tagId, firstBlock, buff, options, ref status);
    }


    /**
     * <summary>
     *   Returns a string with last tag arrival/removal events observed.
     * <para>
     *   This method return only events that are still buffered in the device memory.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string with last events observed (one per line).
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YAPI.INVALID_STRING</c>.
     * </para>
     */
    public virtual string get_lastEvents()
    {
        byte[] content = new byte[0];

        content = this._download("events.txt");
        return YAPI.DefaultEncoding.GetString(content);
    }


    /**
     * <summary>
     *   Registers a callback function to be called each time that an RFID tag appears or
     *   disappears.
     * <para>
     *   The callback is invoked only during the execution of
     *   <c>ySleep</c> or <c>yHandleEvents</c>. This provides control over the time when
     *   the callback is triggered. For good responsiveness, remember to call one of these
     *   two functions periodically. To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer.
     *   The callback function should take four arguments:
     *   the <c>YRfidReader</c> object that emitted the event, the
     *   UTC timestamp of the event, a character string describing
     *   the type of event ("+" or "-") and a character string with the
     *   RFID tag identifier.
     *   On failure, throws an exception or returns a negative error code.
     * </param>
     */
    public virtual int registerEventCallback(YEventCallback callback)
    {
        this._eventCallback = callback;
        this._isFirstCb = true;
        if (callback != null) {
            this.registerValueCallback(yInternalEventCallback);
        } else {
            this.registerValueCallback((ValueCallback) null);
        }
        return 0;
    }


    public virtual int _internalEventHandler(string cbVal)
    {
        int cbPos;
        int cbDPos;
        int cbNtags;
        int searchTags;
        string url;
        byte[] content = new byte[0];
        string contentStr;
        List<string> currentTags = new List<string>();
        List<string> eventArr = new List<string>();
        int arrLen;
        List<int> lastEvents = new List<int>();
        string lenStr;
        int arrPos;
        string eventStr;
        int eventLen;
        string hexStamp;
        int typePos;
        int dataPos;
        int evtStamp;
        string evtType;
        string evtData;
        int tagIdx;
        // detect possible power cycle of the reader to clear event pointer
        cbPos = YAPI._atoi(cbVal);
        cbNtags = ((cbPos) % (1000));
        cbPos = ((cbPos) / (1000));
        cbDPos = ((cbPos - this._prevCbPos) & (0x7ffff));
        this._prevCbPos = cbPos;
        if (cbDPos > 16384) {
            this._eventPos = 0;
        }
        if (!(this._eventCallback != null)) {
            return YAPI.SUCCESS;
        }
        // load all events since previous call
        url = "events.txt?pos="+Convert.ToString(this._eventPos);

        content = this._download(url);
        contentStr = YAPI.DefaultEncoding.GetString(content);
        eventArr = new List<string>(contentStr.Split(new Char[] {'\n'}));
        arrLen = eventArr.Count;
        if (!(arrLen > 0)) {
            this._throw(YAPI.IO_ERROR, "fail to download events");
            return YAPI.IO_ERROR;
        }
        // last element of array is the new position preceeded by '@'
        arrLen = arrLen - 1;
        lenStr = eventArr[arrLen];
        lenStr = (lenStr).Substring( 1, (lenStr).Length-1);
        // update processed event position pointer
        this._eventPos = YAPI._atoi(lenStr);
        if (this._isFirstCb) {
            // first emulated value callback caused by registerValueCallback:
            // attempt to retrieve arrivals of all tags present to emulate arrival
            this._isFirstCb = false;
            this._eventStamp = 0;
            if (cbNtags == 0) {
                return YAPI.SUCCESS;
            }
            currentTags = this.get_tagIdList();
            cbNtags = currentTags.Count;
            searchTags = cbNtags;
            lastEvents.Clear();
            arrPos = arrLen - 1;
            while ((arrPos >= 0) && (searchTags > 0)) {
                eventStr = eventArr[arrPos];
                typePos = (eventStr).IndexOf(":")+1;
                if (typePos > 8) {
                    dataPos = (eventStr).IndexOf("=")+1;
                    evtType = (eventStr).Substring( typePos, 1);
                    if ((dataPos > 10) && evtType == "+") {
                        evtData = (eventStr).Substring( dataPos, (eventStr).Length-dataPos);
                        tagIdx = searchTags - 1;
                        while (tagIdx >= 0) {
                            if (evtData == currentTags[tagIdx]) {
                                lastEvents.Add(0+arrPos);
                                currentTags[tagIdx] = "";
                                while ((searchTags > 0) && currentTags[searchTags-1] == "") {
                                    searchTags = searchTags - 1;
                                }
                                tagIdx = -1;
                            }
                            tagIdx = tagIdx - 1;
                        }
                    }
                }
                arrPos = arrPos - 1;
            }
            // If we have any remaining tags without a known arrival event,
            // create a pseudo callback with timestamp zero
            tagIdx = 0;
            while (tagIdx < searchTags) {
                evtData = currentTags[tagIdx];
                if (!(evtData == "")) {
                    this._eventCallback(this, 0, "+", evtData);
                }
                tagIdx = tagIdx + 1;
            }
        } else {
            // regular callback
            lastEvents.Clear();
            arrPos = arrLen - 1;
            while (arrPos >= 0) {
                lastEvents.Add(0+arrPos);
                arrPos = arrPos - 1;
            }
        }
        // now generate callbacks for each selected event
        arrLen = lastEvents.Count;
        arrPos = arrLen - 1;
        while (arrPos >= 0) {
            tagIdx = lastEvents[arrPos];
            eventStr = eventArr[tagIdx];
            eventLen = (eventStr).Length;
            if (eventLen >= 1) {
                hexStamp = (eventStr).Substring( 0, 8);
                evtStamp = YAPI._hexStrToInt(hexStamp);
                typePos = (eventStr).IndexOf(":")+1;
                if ((evtStamp >= this._eventStamp) && (typePos > 8)) {
                    this._eventStamp = evtStamp;
                    dataPos = (eventStr).IndexOf("=")+1;
                    evtType = (eventStr).Substring( typePos, 1);
                    evtData = "";
                    if (dataPos > 10) {
                        evtData = (eventStr).Substring( dataPos, eventLen-dataPos);
                    }
                    this._eventCallback(this, evtStamp, evtType, evtData);
                }
            }
            arrPos = arrPos - 1;
        }
        return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Continues the enumeration of RFID readers started using <c>yFirstRfidReader()</c>.
     * <para>
     *   Caution: You can't make any assumption about the returned RFID readers order.
     *   If you want to find a specific a RFID reader, use <c>RfidReader.findRfidReader()</c>
     *   and a hardwareID or a logical name.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRfidReader</c> object, corresponding to
     *   a RFID reader currently online, or a <c>null</c> pointer
     *   if there are no more RFID readers to enumerate.
     * </returns>
     */
    public YRfidReader nextRfidReader()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindRfidReader(hwid);
    }

    //--- (end of generated code: YRfidReader implementation)

    //--- (generated code: YRfidReader functions)

    /**
     * <summary>
     *   Starts the enumeration of RFID readers currently accessible.
     * <para>
     *   Use the method <c>YRfidReader.nextRfidReader()</c> to iterate on
     *   next RFID readers.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRfidReader</c> object, corresponding to
     *   the first RFID reader currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YRfidReader FirstRfidReader()
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
        err = YAPI.apiGetFunctionsByClass("RfidReader", 0, p, size, ref neededsize, ref errmsg);
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
        return FindRfidReader(serial + "." + funcId);
    }

    //--- (end of generated code: YRfidReader functions)
}
#pragma warning restore 1591


}