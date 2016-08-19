using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Yppr
{
    public partial class Form1 : Form
    {
        //--------------------------------------------------------------
        // 読み込みデーター
        List<string[]> m_listReadData = new List<string[]>();
        //--------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
        }
        //--------------------------------------------------------------
        private void Pl_FileLoad_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
            }
        }
        //--------------------------------------------------------------
        private void Pl_FileLoad_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたすべてのファイル名を取得する
            string[] FileNameList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            for (int i = 0; i < FileNameList.Length; i++)
            {
                if (LoadDataCsv(FileNameList[i]) == false)
                {
                    MessageBox.Show("読み込みエラー");
                    return;
                }
                // コンバート処理
                if (ConvertCsv(FileNameList[i]) == false)
                {
                    MessageBox.Show("書き込みエラー");
                    return;
                }
            }        
        }
        //--------------------------------------------------------------
        // ファイルからデータ読み込み
        public bool LoadDataCsv(string strFileName)
        {
            // ヘッダー
            string[] strHeader;

            // ファイルを読み込む
            StreamReader clsSr;
            try
            {
                clsSr = new StreamReader(strFileName, System.Text.Encoding.GetEncoding("shift_jis"));
                try
                {
                    string strLine;

                    // 1行目読み込み
                    if ((strLine = clsSr.ReadLine()) == null)
                    {
                        throw new FormatException();
                    }
                    // ヘッダーの文字列を読み、種類を判断する
                    strHeader = strLine.Split(',');
                    if (strHeader[0] != "フリー項目０１")
                    {
                        throw new FormatException();
                    }
                    // データーを読み込む                
                    while ((strLine = clsSr.ReadLine()) != null)
                    {
                        if (strLine == "")
                        {
                            break;
                        }
                        // 全てのデーターを格納
                        m_listReadData.Add(strLine.Split(','));
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    clsSr.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        //--------------------------------------------------------------
        // コンバート処理実行
        public bool ConvertCsv(string strFileName)
        {
            // 配送伝票番号情報出力ファイル名
            string strColorMeFileName = Path.GetDirectoryName(strFileName) + "\\" + "カラーミー.csv";
            string strAmazonFileName = Path.GetDirectoryName(strFileName) + "\\" + "Amazon.txt";

            while (m_listReadData.Count > 0)
            {
                if (m_listReadData[0][(int)EnumYpprDeliNoItem.FREE_ITEM] != "")
                {
                    if (m_listReadData[0][(int)EnumYpprDeliNoItem.FREE_ITEM].Substring(0, 1) == "1")
                    {
                        // カラーミー
                        string[] strColorMeDeliNo = new string[(int)EnumColorMeDeliNoItem.MAX];

                        // 売上ID
                        strColorMeDeliNo[(int)EnumColorMeDeliNoItem.ORDER_ID] = m_listReadData[0][(int)EnumYpprDeliNoItem.FREE_ITEM].Substring(1, m_listReadData[0][(int)EnumYpprDeliNoItem.FREE_ITEM].Length - 1);
                        // 配送先ID
                        strColorMeDeliNo[(int)EnumColorMeDeliNoItem.SHIPPING_ID] = "";
                        // 配送伝票番号
                        strColorMeDeliNo[(int)EnumColorMeDeliNoItem.DELIVERY_NO] = m_listReadData[0][(int)EnumYpprDeliNoItem.DELIVERY_NO];

                        // 書き込む
                        StreamWriter clsSw;
                        try
                        {
                            clsSw = new StreamWriter(strColorMeFileName, true, Encoding.GetEncoding("Shift_JIS"));
                        }
                        catch
                        {
                            return false;
                        }
                        for (int i = 0; i < (int)EnumColorMeDeliNoItem.MAX; i++)
                        {
                            if (i != 0)
                            {
                                clsSw.Write(",");
                            }
                            clsSw.Write("{0}", strColorMeDeliNo[i]);
                        }
                        clsSw.Write("\n");

                        clsSw.Flush();
                        clsSw.Close();
                    
                    }
                    else if (m_listReadData[0][(int)EnumYpprDeliNoItem.FREE_ITEM].Substring(0, 1) == "2")
                    {
                        // Amazon
                        string[] strAmazonDeliNo = new string[(int)EnumAmazonDeliNoItem.MAX];
                        bool bHeaderFlg = false;

                        // 売上ID
                        strAmazonDeliNo[(int)EnumAmazonDeliNoItem.ORDER_ID] = m_listReadData[0][(int)EnumYpprDeliNoItem.FREE_ITEM].Substring(1, m_listReadData[0][(int)EnumYpprDeliNoItem.FREE_ITEM].Length - 1);
                        // 出荷日
                        strAmazonDeliNo[(int)EnumAmazonDeliNoItem.SHIPPING_DATE] = DateTime.Now.ToString("yyyy-MM-dd");
                        // 配送業者コード
                        strAmazonDeliNo[(int)EnumAmazonDeliNoItem.CARROER_CODE] = "Other";
                        // 配送業者名
                        strAmazonDeliNo[(int)EnumAmazonDeliNoItem.CARROER_NAME] = "日本郵便";
                        // トラッキング番号
                        strAmazonDeliNo[(int)EnumAmazonDeliNoItem.TRACKING_NUMVER] = m_listReadData[0][(int)EnumYpprDeliNoItem.DELIVERY_NO];
                        // 代金引換
                        strAmazonDeliNo[(int)EnumAmazonDeliNoItem.COD_COLLECTION_METHOD] = "";

                        // ファイルが存在しない場合には、ヘッダーを追加
                        if (System.IO.File.Exists(strAmazonFileName) == false)
                        {
                            bHeaderFlg = true;
                        }

                        // 書き込む
                        StreamWriter clsSw;
                        try
                        {
                            clsSw = new StreamWriter(strAmazonFileName, true, Encoding.GetEncoding("Shift_JIS"));
                        }
                        catch
                        {
                            return false;
                        }
                        // データ書き込み
                        if (bHeaderFlg)
                        {
                            clsSw.Write("TemplateType=OrderFulfillment	Version=2011.1102	この行はAmazonが使用しますので変更や削除しないでください。						\n");
                            clsSw.Write("注文番号	注文商品番号	出荷数	出荷日	配送業者コード	配送業者名	お問い合わせ伝票番号	配送方法	代金引換\n");
                            clsSw.Write("order-id	order-item-id	quantity	ship-date	carrier-code	carrier-name	tracking-number	ship-method	cod-collection-method\n");
                        }
                        for (int i = 0; i < (int)EnumAmazonDeliNoItem.MAX; i++)
                        {
                            if (i != 0)
                            {
                                clsSw.Write("	");
                            }
                            clsSw.Write("{0}", strAmazonDeliNo[i]);
                        }
                        clsSw.Write("\n");

                        clsSw.Flush();
                        clsSw.Close();
                    }
                }

                // データ削除
                string strID = m_listReadData[0][(int)EnumYpprDeliNoItem.FREE_ITEM];
                for (int i = m_listReadData.Count - 1; i >= 0; i--)
                {
                    if (strID == m_listReadData[i][(int)EnumYpprDeliNoItem.FREE_ITEM])
                    {
                        m_listReadData.RemoveAt(i);
                    }
                }

            }

            return true;
        }
        //--------------------------------------------------------------
    }
}
