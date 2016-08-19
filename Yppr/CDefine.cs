using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yppr
{
    //--------------------------------------------------------------
    #region 列挙型
    /// <summary>
    /// ゆうプリR配送伝票番号要素
    /// </summary>
    public enum EnumYpprDeliNoItem
    {
        /// <summary>フリー項目０１</summary>
        FREE_ITEM = 0,
        /// <summary>お客様側管理番号</summary>
        USER_ID,
        /// <summary>お問い合わせ番号</summary>
        DELIVERY_NO,

        MAX
    }
    /// <summary>
    /// カラーミー配送伝票番号要素
    /// </summary>
    public enum EnumColorMeDeliNoItem
    {
        /// <summary>売上ID</summary>
        ORDER_ID = 0,
        /// <summary>配送先ID</summary>
        SHIPPING_ID,
        /// <summary>配送伝票番号</summary>
        DELIVERY_NO,

        MAX
    }
    /// <summary>
    /// アマゾン配送伝票番号要素
    /// </summary>
    public enum EnumAmazonDeliNoItem
    {
        /// <summary>売上ID</summary>
        ORDER_ID = 0,
        /// <summary>出荷日</summary>
        SHIPPING_DATE = 3,
        /// <summary>配送業者コード</summary>
        CARROER_CODE = 4,
        /// <summary>配送業者名</summary>
        CARROER_NAME = 5,
        /// <summary>トラッキング番号</summary>
        TRACKING_NUMVER = 6,
        /// <summary>代金引換</summary>
        COD_COLLECTION_METHOD = 8,

        MAX
    }
    #endregion
    //--------------------------------------------------------------
    /// <summary>
    /// 定数宣言クラス
    /// </summary>
    class CDefine
    {
    }
    //--------------------------------------------------------------
}
