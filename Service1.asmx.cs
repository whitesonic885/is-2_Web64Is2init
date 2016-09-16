using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Services;
using Oracle.DataAccess.Client;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace is2init
{
	/// <summary>
	/// [is2init]
	/// </summary>
	//--------------------------------------------------------------------------
	// 修正履歴
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 東都）高木 オブジェクトの破棄
	//	disposeReader(reader);
	//	reader = null;
	//--------------------------------------------------------------------------
	// DEL 2007.05.10 東都）高木 未使用関数のコメント化
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	//--------------------------------------------------------------------------
	// ADD 2007.10.19 東都）高木 端末バージョン管理
	//--------------------------------------------------------------------------
	// ADD 2008.03.21 東都）高木 レベルアップ促進対応
	// ADD 2008.05.21 東都）高木 ログインエラー回数を５回にする 
	// ADD 2008.06.17 東都）高木 ＭＡＣアドレスチェックの追加 
	// ADD 2008.06.30 東都）高木 パスワードチェックの強化 
	// ADD 2008.07.07 東都）高木 ＭＡＣアドレス取得失敗時対応 
	// ADD 2008.07.07 東都）高木 特定の会員にはメッセージ非表示 
	// ADD 2008.07.07 東都）高木 レベルアップ促進のレベル変更 
	// MOD 2008.07.16 東都）高木 ＭＡＣアドレスの簡易マスキング 
	// DEL 2008.10.22 東都）高木 ＭＡＣアドレスチェックの停止 
	// MOD 2008.12.22 東都）高木 有効期限の範囲の修正 
	// ADD 2008.12.25 東都）高木 ログイン時のレベルアップ促進メッセージの追加 
	//--------------------------------------------------------------------------
	// MOD 2009.02.12 東都）高木 バージョンが1.9以前のユーザではログイン不可 
	// ADD 2009.04.02 東都）高木 稼働日対応 
	// ADD 2009.01.30 東都）高木 実績一覧印刷オプション項目の追加 
	// MOD 2009.05.27 東都）高木 パスワードエラー時にパスワード更新日を表示 
	// MOD 2009.08.19 東都）高木 端末ＣＤの連番枯渇対応 
	// MOD 2009.09.14 東都）高木 パスワードエラー時の問い合わせ先の変更 
	// MOD 2009.10.05 東都）高木 マイナーバージョン２桁対応（Ver.2.10～）
	//--------------------------------------------------------------------------
	// MOD 2010.05.21 東都）高木 ＩＰ番号が未取得対応 
	// MOD 2010.07.29 東都）高木 特定の会員にはメッセージ非表示 
	//                           [丸成商事㈱　京都支社]様
	//--------------------------------------------------------------------------
	// MOD 2011.05.09 東都）高木 お客様毎の重量入力不可対応 
	//保留 MOD 2011.09.16 東都）高木 請求先名が同じ場合のソート順対応 
	// MOD 2011.10.11 東都）高木 ＳＳＬ証明書導入状態などのチェック 
	//--------------------------------------------------------------------------
	// MOD 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
	//--------------------------------------------------------------------------
	// MOD 2014.10.16 BEVAS)前田 総当たりログイン対策
	//--------------------------------------------------------------------------
	// MOD 2016.04.05 BEVAS）松本 Windows10対応
	//--------------------------------------------------------------------------
	// MOD 2016.05.24 BEVAS）松本 セクション切替画面改修対応
	//--------------------------------------------------------------------------
	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2init")]

	public class Service1 : is2common.CommService
	{

		public Service1()
		{
			//CODEGEN: この呼び出しは、ASP.NET Web サービス デザイナで必要です。
			InitializeComponent();

			connectService();
		}

		#region コンポーネント デザイナで生成されたコード 
		
		//Web サービス デザイナで必要です。
		private IContainer components = null;
				
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		/*********************************************************************
		 * 端末情報登録
		 * 引数：会員ＣＤ、利用者ＣＤ、パスワード、サーマルプリンタ有無、
		 *		 プリンタ種類、コンピュータ名、ＩＰアドレス、ＭＡＣアドレス
		 * 戻値：ステータス、端末ＣＤ、会員名、利用者名、部門ＣＤ、部門名
		 *
		 *********************************************************************/
// ADD 2005.06.02 東都）高木 ORA-03113対策？ START
		private static string SET_TANMATSU_SELECT_1
			= "SELECT 会員名, \n"
			+       " 使用開始日, \n"
			+       " 使用終了日, \n"
			+       " SYSDATE \n"
			+       " FROM ＣＭ０１会員 \n";
		private static string SET_TANMATSU_SELECT_2
			= "SELECT RIY.利用者名, \n"
			+       " RIY.\"パスワード\", \n"
			+       " RIY.部門ＣＤ, \n"
			+       " BUM.部門名 , \n"
			+       " RIY.\"認証エラー回数\" \n"
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
			+       ", RIY.登録ＰＧ \n"
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END
			+  " FROM ＣＭ０４利用者 RIY, \n"
			+       " ＣＭ０２部門 BUM \n";
		private static string SET_TANMATSU_SELECT_2_WHERE
			=   " AND RIY.会員ＣＤ = BUM.会員ＣＤ \n"
			+   " AND RIY.部門ＣＤ = BUM.部門ＣＤ \n"
// ADD 2005.08.19 東都）高木 部門削除の対応 START
			+   " AND BUM.削除ＦＧ = '0' \n"
// ADD 2005.08.19 東都）高木 部門削除の対応 END
			+   " AND RIY.削除ＦＧ = '0' \n";
// MOD 2009.08.19 東都）高木 端末ＣＤの連番枯渇対応 START
//		private static string SET_TANMATSU_SELECT_3
//			= "SELECT ＳＱ０１端末ＣＤ.nextval \n"
//			+  " FROM DUAL \n";
		private static string SET_TANMATSU_SELECT_3
			= "SELECT ＳＱ０２端末ＣＤ.nextval \n"
			+  " FROM DUAL \n";
// MOD 2009.08.19 東都）高木 端末ＣＤの連番枯渇対応 END
// ADD 2005.06.02 東都）高木 ORA-03113対策？ END
		[WebMethod]
		public String[] Set_tanmatsu(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "端末情報登録開始");

// MOD 2014.10.16 BEVAS)前田 特定IPからの総当たり攻撃対策 START
			// クライアント側のIPアドレスの取得
			
// MOD 2014.10.16 BEVAS)前田 特定IPからの総当たり攻撃対策 END

			OracleConnection conn2 = null;
			String[] sRet = new string[8];

// MOD 2009.02.12 東都）高木 バージョンが1.9以前のユーザではログイン不可 START
			if(sUser.Length < 4)
			{
						// １２３４５６７８９＊１２３４５６７８９＊１２３４５６７８９＊
				sRet[0] = "お客様にご利用いただいているアプリケーションを最新のものに更新する必要があります。　\n"
						+ "バージョンアップの作業をお願い致します。　\n"
						+ "詳しくは、福山通運のｉＳＴＡＲ－２ダウンロード画面にある［再セットアップ手順書］をご覧下さい。　\n"
						;
				sRet[1] = " ";
				return sRet;
			}
// MOD 2009.02.12 東都）高木 バージョンが1.9以前のユーザではログイン不可 END

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			string sQuery    = "";
			string s会員ＣＤ = sKey[0];
			string s会員名   = "";
			string s利用ＣＤ = sKey[1];
			string s利用者名 = "";
			string sパス     = "";
			string s部門ＣＤ = "";
			string s部門名   = "";
			string s端末ＣＤ = "";
			string s認証エラー回数 = "0";
// DEL 2005.06.02 東都）高木 ORA-03113対策？ START
//			string sメッセージ = "";
// DEL 2005.06.02 東都）高木 ORA-03113対策？ END
			int i認証エラー回数 = 0;

			try
			{
				sQuery
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//					= "SELECT 会員名, \"メッセージ\" \n"
//					+  " FROM ＣＭ０１会員 \n"
//					+ " WHERE 会員ＣＤ = '"+ s会員ＣＤ + "' \n"
//					+   " AND TO_CHAR(SYSDATE,'YYYYMMDD') BETWEEN 使用開始日 AND 使用終了日 \n"
//					+   " AND 削除ＦＧ = '0' \n"
					= SET_TANMATSU_SELECT_1
					+ " WHERE 会員ＣＤ = '"+ s会員ＣＤ + "' \n"
					+   " AND 削除ＦＧ = '0' \n"
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
				int    i当日       = 0;
				int    iパス更新日 = 0;
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END
				if (reader.Read())
				{
					s会員名     = reader.GetString(0).Trim();
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//					sメッセージ = reader.GetString(1).Trim();
					string s使用開始日 = reader.GetString(1).Trim();
					int    i使用開始日 = int.Parse(s使用開始日);
					int    i使用終了日 = int.Parse(reader.GetString(2).Trim());
// MOD 2008.06.30 東都）高木 パスワードチェックの強化 START
//					int    i当日       = int.Parse(reader.GetDateTime(3).ToString("yyyyMMdd").Trim());
					       i当日       = int.Parse(reader.GetDateTime(3).ToString("yyyyMMdd").Trim());
// MOD 2008.06.30 東都）高木 パスワードチェックの強化 START
					if (i当日 < i使用開始日)
					{
						if(s使用開始日.Length == 8)
						{
							string s年 = s使用開始日.Substring(0,4);
							string s月 = s使用開始日.Substring(4,2);
							string s日 = s使用開始日.Substring(6,2);
							if(s月[0] == '0') s月 = s月.Substring(1,1);
							if(s日[0] == '0') s日 = s日.Substring(1,1);
							sRet[0] = s年 + "年" + s月 + "月"
									+ s日 + "日より使用できます";
						}
						else
						{
							sRet[0] = "使用開始日より使用できます";
						}
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
						return sRet;
					}
					if (i当日 > i使用終了日)
					{
						sRet[0] = "使用期限が切れています";
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
						return sRet;
					}
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
// ADD 2007.04.26 東都）高木 不要なコマンドの削除 START
				}else{
					sRet[0] = "会員ＣＤ、利用者ＣＤ もしくは パスワード に誤りがあります";
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
					return sRet;
// ADD 2007.04.26 東都）高木 不要なコマンドの削除 END
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				// 利用者情報の取得
				sQuery
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//					= "SELECT RIY.利用者名, RIY.\"パスワード\", RIY.部門ＣＤ, \n"
//					+       " BUM.部門名 , \n"
//					+       " \"認証エラー回数\" \n"
//					+  " FROM ＣＭ０４利用者 RIY, ＣＭ０２部門 BUM \n"
					= SET_TANMATSU_SELECT_2
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
					+ " WHERE RIY.会員ＣＤ   = '"+ s会員ＣＤ + "' \n"
					+   " AND RIY.利用者ＣＤ = '"+ s利用ＣＤ + "' \n"
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//					+   " AND RIY.会員ＣＤ   =  BUM.会員ＣＤ \n"
//					+   " AND RIY.部門ＣＤ   =  BUM.部門ＣＤ \n"
//					+   " AND RIY.削除ＦＧ   = '0' \n"
					+ SET_TANMATSU_SELECT_2_WHERE
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
					;

				reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					s利用者名 = reader.GetString(0).Trim();
					sパス     = reader.GetString(1).Trim();
					s部門ＣＤ = reader.GetString(2).Trim();
					s部門名   = reader.GetString(3).Trim();
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//					s認証エラー回数 = reader.GetString(4);
					s認証エラー回数 = reader.GetDecimal(4).ToString().Trim();
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
					i認証エラー回数 = int.Parse(s認証エラー回数);
					sRet[0]   = "正常終了";
					sRet[6]   = s認証エラー回数;
// ADD 2008.05.21 東都）高木 ログインエラー回数を５回にする START
//					if (i認証エラー回数 >= 10)
					if (i認証エラー回数 >= 5)
// ADD 2008.05.21 東都）高木 ログインエラー回数を５回にする END
					{
// MOD 2005.08.19 東都）高木 メッセージの変更 START
//						sRet[0] = "認証エラー：最寄の営業所まで御連絡下さい";
						sRet[0] = "御客様のＩＤは、利用制限がされています　\n"
								+ "最寄の営業所まで御連絡下さい";
// MOD 2005.08.19 東都）高木 メッセージの変更 END
					}
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
					try{
						iパス更新日 = int.Parse(reader.GetString(5).Trim());
					}catch (Exception ){
						;
					}
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END
// ADD 2007.04.26 東都）高木 不要なコマンドの削除 START
				}else{
					sRet[0] = "利用者ＣＤ もしくは パスワード に誤りがあります";
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
					return sRet;
// ADD 2007.04.26 東都）高木 不要なコマンドの削除 END
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
				DateTime dt当日 = new DateTime(
					  int.Parse(i当日.ToString().Substring(0,4))
					, int.Parse(i当日.ToString().Substring(4,2))
					, int.Parse(i当日.ToString().Substring(6,2))
					);
				DateTime dtパス更新日 = new DateTime(
					int.Parse(iパス更新日.ToString().Substring(0,4))
					, int.Parse(iパス更新日.ToString().Substring(4,2))
					, int.Parse(iパス更新日.ToString().Substring(6,2))
					);
				DateTime dtパス有効期限 = dtパス更新日.AddMonths(6);
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END

				// パスワードのチェック
				if(sパス != sKey[2])
				{
// MOD 2009.05.27 東都）高木 パスワードエラー時にパスワード更新日を表示 START
//					sRet[0] = "利用者ＣＤ もしくは パスワード に誤りがあります";
					sRet[0] = "利用者ＣＤ もしくは パスワード に誤りがあります\n"
							+ "　　　　　（"
							+ int.Parse(iパス更新日.ToString().Substring(4,2)) + "/"
							+ int.Parse(iパス更新日.ToString().Substring(6,2))
							+ " に変更されています）"
							;
// MOD 2009.05.27 東都）高木 パスワードエラー時にパスワード更新日を表示 END
				}

				OracleTransaction tran;

				// 利用者マスタを更新する
				if(sRet[0].Length != 4 || i認証エラー回数 != 0)
				{
					// 正常終了時
					if (sRet[0].Length == 4){
						i認証エラー回数 = 0;
					}else if(i認証エラー回数 < 90){
						i認証エラー回数++;
					}else{
						i認証エラー回数 = 90;
					}

//					OracleTransaction tran = conn2.BeginTransaction();
					tran = conn2.BeginTransaction();
					try
					{
						// 利用者マスタの更新
						sQuery
							= "UPDATE ＣＭ０４利用者 \n"
							+   " SET 認証エラー回数 = " + i認証エラー回数 + ", \n"
							+       " 更新日時       = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
							+       " 更新ＰＧ       = '" + "初期登録" + "', \n"
							+       " 更新者         = '" + s利用ＣＤ + "'  \n"
							+ " WHERE 会員ＣＤ       = '" + s会員ＣＤ + "'  \n"
							+   " AND 利用者ＣＤ     = '" + s利用ＣＤ + "'  \n"
							;
						CmdUpdate(sUser, conn2, sQuery);

						tran.Commit();
					}
					catch (OracleException ex)
					{
						tran.Rollback();
						sRet[0] = chgDBErrMsg(sUser, ex);
					}
					catch (Exception ex)
					{
						tran.Rollback();
						sRet[0] = "サーバエラー：" + ex.Message;
						logWriter(sUser, ERR, sRet[0]);
					}
					
					// 正常終了ではない場合には終了する
					if(sRet[0].Length != 4)
					{
// DEL 2007.04.26 東都）高木 不要なコマンドの削除 START
// finally内で実行される為不要
//						disconnect2(sUser, conn2);
//						logFileClose();
// DEL 2007.04.26 東都）高木 不要なコマンドの削除 END

						return sRet;
					}
				}

// ADD 2007.02.13 東都）高木 端末ＩＤ再取得対応 START
				bool b旧端末ＣＤ = false;
// ADD 2008.07.16 東都）高木 ＭＡＣアドレスの簡易マスキング START
				if(sKey[7].Length == 17 && sKey[7].Substring(0,1).Equals("Z")){
					sKey[7] = sKey[7].Substring( 1,2) + "-"
							+ sKey[7].Substring( 4,2) + "-"
							+ sKey[7].Substring( 7,2) + "-"
							+ sKey[7].Substring(10,2) + "-"
							+ sKey[7].Substring(13,2) + "-"
							+ sKey[7].Substring(15,2)
							;
				}
// ADD 2008.07.16 東都）高木 ＭＡＣアドレスの簡易マスキング END
// MOD 2010.05.21 東都）高木 ＩＰ番号が未取得対応 START
				if(sKey[5].Length == 0) sKey[5] = " "; // マシン名
				if(sKey[6].Length == 0) sKey[6] = " "; // ＩＰ
// MOD 2010.05.21 東都）高木 ＩＰ番号が未取得対応 END
// ADD 2008.07.07 東都）高木 ＭＡＣアドレス取得失敗時対応 START
//				if(sKey[7].Length == 0) sKey[7] = sKey[5];
				if(sKey[7].Length == 0) sKey[7] = "-";
// ADD 2008.07.07 東都）高木 ＭＡＣアドレス取得失敗時対応 END
				// 以前登録されている端末情報を取得する(会員ＣＤ、マシン名、ＭＡＣ)
				sQuery = "SELECT NVL(MAX(端末ＣＤ), ' ') \n"
						+ " FROM ＣＭ０３端末 \n";
				sQuery += " WHERE 会員ＣＤ = '" + s会員ＣＤ + "' \n";
				sQuery += " AND マシン名 = '" + sKey[5] + "' \n";
				sQuery += " AND ＭＡＣ = '" + sKey[7] + "' \n";

				reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					s端末ＣＤ = reader.GetString(0).Trim();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				if(s端末ＣＤ.Length > 0){
					b旧端末ＣＤ = true;
				}else{
					b旧端末ＣＤ = false;
// ADD 2007.02.13 東都）高木 端末ＩＤ再取得対応 END

					// 端末ＣＤの新規取得
					sQuery
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//						= "SELECT TO_CHAR(ＳＱ０１端末ＣＤ.nextval) FROM DUAL";
						= SET_TANMATSU_SELECT_3;
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
					reader = CmdSelect(sUser, conn2, sQuery);

					if (reader.Read())
					{
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//						s端末ＣＤ = "IS2" + reader.GetString(0).Trim();
// MOD 2009.08.19 東都）高木 端末ＣＤの連番枯渇対応 START
//						s端末ＣＤ = "IS2" + reader.GetDecimal(0).ToString().Trim();
						s端末ＣＤ = "IS" + reader.GetDecimal(0).ToString().Trim();
// MOD 2009.08.19 東都）高木 端末ＣＤの連番枯渇対応 END
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
					}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

// ADD 2007.02.13 東都）高木 端末ＩＤ再取得対応 START
				}
// ADD 2007.02.13 東都）高木 端末ＩＤ再取得対応 END

//				OracleTransaction tran = conn2.BeginTransaction();
				tran = conn2.BeginTransaction();
				string sUserHostName = "";
				if(this.Context.Request.UserHostName.Length > 30)
					sUserHostName = this.Context.Request.UserHostName.Substring(0,30);
				else
					sUserHostName = this.Context.Request.UserHostName;

				try
				{
// ADD 2007.02.13 東都）高木 端末ＩＤ再取得対応 START
					if(b旧端末ＣＤ){
						// 端末マスタの更新
						sQuery
							= "UPDATE ＣＭ０３端末 \n"
							+ "SET プリンタＦＧ = '"+ sKey[3] +"' \n"
							+ ", プリンタ識別子 = '"+ sKey[4] +"' \n"
							+ ", ドメイン = '" + sUserHostName + "' \n"
							+ ", ＩＰ = '" + sKey[6] + "' \n"
							+ ", 削除ＦＧ = '0' \n"
							+ ", 更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
							+ ", 更新ＰＧ = '初期登録' \n"
							+ ", 更新者 = '" + s利用ＣＤ + "' \n"
							;
						sQuery += " WHERE 端末ＣＤ = '" + s端末ＣＤ + "' \n";
					}else{
// ADD 2007.02.13 東都）高木 端末ＩＤ再取得対応 END
						// 端末マスタの登録
						sQuery
							= "INSERT INTO ＣＭ０３端末 \n"
							+ "VALUES ( \n"
							+ "'" + s端末ＣＤ + "', '" + s会員ＣＤ + "', \n"	// 端末ＣＤ, 会員ＣＤ,
							+ "'" + sKey[3] + "', '" + sKey[4] + "', \n"		// プリンタＦＧ, プリンタ識別子,
							+ "TO_CHAR(SYSDATE,'YYYYMMDD'), \n"					// 使用開始日,
							+ "'"+ sUserHostName +"', \n"	// ドメイン名
							+ "'"+ sKey[5] + "', '" + sKey[6] + "', '" + sKey[7] + "', \n"			// マシン名, ＩＰ, ＭＡＣ, 
							+ "'1', '初期登録', '更新', TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"	// 起動状態, 実行画面, 実行コマンド, 実行日時,
// ADD 2005.06.07 東都）高木 都道府県選択の変更 START
							+ "'0',"						// 都道府県ＣＤ
// ADD 2005.06.07 東都）高木 都道府県選択の変更 END
							+ "'0', \n" // 削除ＦＧ, 
							+ "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), '初期登録', '"+ s利用ＣＤ +"', \n" // 登録日時, 登録ＰＧ, 登録者,
							+ "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), '初期登録', '"+ s利用ＣＤ +"'  \n" // 更新日時, 更新ＰＧ, 更新者,
							+ ")  "
							;
// ADD 2007.02.13 東都）高木 端末ＩＤ再取得対応 START
					}
// ADD 2007.02.13 東都）高木 端末ＩＤ再取得対応 END

					CmdUpdate(sUser, conn2, sQuery);

					tran.Commit();
					sRet[0] = "正常終了";
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
					//６ヶ月以上経過した時点で、パスワード更新警告表示
					if(dt当日.CompareTo(dtパス有効期限) > 0){
						sRet[0] = "期限切れ";
					//５ヶ月以上経過した時点で、パスワード更新警告表示
// MOD 2008.12.22 東都）高木 有効期限の範囲の修正 START
//					}else if(dt当日.CompareTo(dtパス更新日.AddMonths(5)) >= 0){
					}else if(dt当日.CompareTo(dtパス更新日.AddMonths(5)) > 0){
// MOD 2008.12.22 東都）高木 有効期限の範囲の修正 END
						sRet[0] = dtパス有効期限.ToString("MMdd").Trim();
					}
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END


					logWriter(sUser, INF, sRet[0]);
				}
				catch (OracleException ex)
				{
					tran.Rollback();
					sRet[0] = chgDBErrMsg(sUser, ex);
				}
				catch (Exception ex)
				{
					tran.Rollback();
					sRet[0] = "サーバエラー：" + ex.Message;
					logWriter(sUser, ERR, sRet[0]);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			sRet[1] = s端末ＣＤ;
			sRet[2] = s会員名;
			sRet[3] = s利用者名;
			sRet[4] = s部門ＣＤ;
			sRet[5] = s部門名;
			sRet[6] = s認証エラー回数;
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//			sRet[7] = sメッセージ;
			sRet[7] = "";
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
			return sRet;
		}

		/*********************************************************************
		 * 端末情報更新
		 * 引数：端末ＣＤ、サーマルプリンタ有無、プリンタ種類、ＰＧＩＤ、利用者ＣＤ
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String Upd_tanmatsu(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "端末マスタの更新開始");

			OracleConnection conn2 = null;
			String sRet = "";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet = userCheck2(conn2, sUser);
//			if(sRet.Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 東都）高木 会員チェック追加 END

			string sQuery    = "";
			string s端末ＣＤ = sKey[0];
			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				// 端末マスタの更新
				sQuery
					= "UPDATE ＣＭ０３端末 \n"
					+   " SET プリンタＦＧ   = '" + sKey[1] + "', \n"
					+       " プリンタ識別子 = '" + sKey[2] + "', \n"
// DEL 2007.02.08 東都）高木 クライアントアプリの高速化 START
//					+       " 起動状態 = '1', \n"
//					+       " 実行画面 = '" + sKey[3] + "', \n"
//					+       " 実行コマンド = '更新', \n"
//					+       " 実行日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
// DEL 2007.02.08 東都）高木 クライアントアプリの高速化 END
					+       " 更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
					+       " 更新ＰＧ = '" + sKey[3] + "', \n"
					+       " 更新者   = '" + sKey[4] + "'  \n"
					+ " WHERE 端末ＣＤ = '" + s端末ＣＤ + "' \n"
					;

				CmdUpdate(sUser, conn2, sQuery);

				tran.Commit();
				sRet = "正常終了";

				logWriter(sUser, INF, sRet);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}

// ADD 2007.10.19 東都）高木 端末バージョン管理 START
		/*********************************************************************
		 * バージョン情報の更新（端末マスタ、部門マスタ）
		 * 引数：部門ＣＤ、ＰＧＩＤ
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		private String Upd_bumon_ver(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "バージョン情報の更新開始");

			if(sUser.Length < 4) return "";

			OracleConnection conn2 = null;
			String sRet = "";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet = "ＤＢ接続エラー";
				return sRet;
			}

			string sQuery = "";
			string sVer   = sUser[3] + "    ";
			sVer = sVer.Substring(0,4);
			string sVer00 = "";
			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				// 端末マスタの検索
				sQuery
					= "SELECT 実行画面 FROM ＣＭ０３端末 \n"
					+ " WHERE 端末ＣＤ = '" + sUser[2] + "' \n"
					;
				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);
				if (reader.Read())
				{
					sVer00 = reader.GetString(0);
					sVer00 = sVer00.Substring(0,4);
				}
				disposeReader(reader);
				reader = null;
				
				//端末のバージョン情報が異なる場合のみ更新
				if(sVer != sVer00)
				{
					// 端末マスタの更新
					sQuery
						= "UPDATE ＣＭ０３端末 \n"
//						+   " SET 実行画面 = '" + sVer + "' || SUBSTRB(実行画面,5), \n"
						+   " SET 実行画面 = '" + sVer + "', \n"
						+       " 更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+       " 更新ＰＧ = '" + sKey[1] + "', \n"
						+       " 更新者   = '" + sUser[1] + "'  \n"
						+ " WHERE 端末ＣＤ = '" + sUser[2] + "' \n"
						;
					CmdUpdate(sUser, conn2, sQuery);

					// 部門マスタの更新
					sQuery
						= "UPDATE ＣＭ０２部門 \n"
						+   " SET 組織ＣＤ = '" + sVer + "' || SUBSTRB(組織ＣＤ,5), \n"
						+       " 更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+       " 更新ＰＧ = '" + sKey[1] + "', \n"
						+       " 更新者   = '" + sUser[1] + "'  \n"
						+ " WHERE 会員ＣＤ = '" + sUser[0] + "' \n"
						+ " AND 部門ＣＤ = '" + sKey[0] + "' \n"
						;
					CmdUpdate(sUser, conn2, sQuery);
				}

				tran.Commit();
				sRet = "正常終了";

				logWriter(sUser, INF, sRet);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}
// ADD 2007.10.19 東都）高木 端末バージョン管理 END

		/*********************************************************************
		 * 利用者マスタの更新（パスワード）
		 * 引数：会員ＣＤ、利用者ＣＤ、パスワード、ＰＧＩＤ、利用者ＣＤ
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String Upd_riyou(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "利用者マスタの更新開始");

			OracleConnection conn2 = null;
			String sRet = "";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet = userCheck2(conn2, sUser);
//			if(sRet.Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 東都）高木 会員チェック追加 END

			string sQuery = "";
			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
				string sパス = "";
				// 利用者マスタの更新
				sQuery
					= "SELECT \"パスワード\" \n"
					+   "FROM ＣＭ０４利用者 \n"
					+  "WHERE 会員ＣＤ       = '" + sKey[0] + "'  \n"
					+    "AND 利用者ＣＤ     = '" + sKey[1] + "'  "
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if(reader.Read()){
					sパス     = reader.GetString(0).Trim();
				}
				disposeReader(reader);
				reader = null;

				if(sパス.Equals(sKey[2])){
					tran.Commit();
					sRet = "前回と同じパスワードには変更できません。";
				}else{
					sQuery = "";
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END

					// 利用者マスタの更新
					sQuery
						= "UPDATE ＣＭ０４利用者 \n"
						+    "SET パスワード     = '" + sKey[2] + "', \n"
// ADD 2008.05.21 東都）高木 ログインエラー回数を５回にする START
						+        "登録ＰＧ       = TO_CHAR(SYSDATE,'YYYYMMDD'), \n"
// ADD 2008.05.21 東都）高木 ログインエラー回数を５回にする END
						+        "更新日時       = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+        "更新ＰＧ       = '" + sKey[3] + "', \n"
						+        "更新者         = '" + sKey[4] + "'  \n"
						+  "WHERE 会員ＣＤ       = '" + sKey[0] + "'  \n"
						+    "AND 利用者ＣＤ     = '" + sKey[1] + "'  "
						;

					CmdUpdate(sUser, conn2, sQuery);

					tran.Commit();
					sRet = "正常終了";
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
				}
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END

				logWriter(sUser, INF, sRet);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}

		private static string GET_TANMATSU_SELECT_1
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//			= "SELECT TAN.会員ＣＤ, TAN.プリンタＦＧ, TAN.プリンタ識別子, \n"
//			+       " KAI.会員名 \n"
//			+  " FROM ＣＭ０３端末 TAN, ＣＭ０１会員 KAI \n";
// MOD 2005.06.07 東都）高木 都道府県選択の変更 START
//			= "SELECT 会員ＣＤ, プリンタＦＧ, プリンタ識別子 \n"
			= "SELECT 会員ＣＤ, プリンタＦＧ, プリンタ識別子, 都道府県ＣＤ \n"
// ADD 2008.12.25 東都）高木 ログイン時のレベルアップ促進メッセージの追加 START
// DEL 2009.02.12 東都）高木 バージョンが1.9以前のユーザではログイン不可 START
//			+ ", 実行コマンド, TO_CHAR(SYSDATE,'YYYYMMDD') \n"
// DEL 2009.02.12 東都）高木 バージョンが1.9以前のユーザではログイン不可 END
// ADD 2008.12.25 東都）高木 ログイン時のレベルアップ促進メッセージの追加 END
// MOD 2005.06.07 東都）高木 都道府県選択の変更 END
// MOD 2011.10.11 東都）高木 ＳＳＬ証明書導入状態などのチェック START
			+ ", 起動状態, 実行コマンド \n"
// MOD 2011.10.11 東都）高木 ＳＳＬ証明書導入状態などのチェック END
			+  " FROM ＣＭ０３端末 \n";
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END

		private static string GET_TANMATSU_SELECT_1_WHERE
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//			=   " AND TAN.削除ＦＧ = '0' \n"
//			+   " AND TAN.会員ＣＤ = KAI.会員ＣＤ \n"
//			+   " AND TO_CHAR(SYSDATE,'YYYYMMDD') BETWEEN KAI.使用開始日 AND KAI.使用終了日 \n"
//			+   " AND KAI.削除ＦＧ = '0' \n";
			=   " AND 削除ＦＧ = '0' \n";
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END

		private static string GET_TANMATSU_SELECT_2
			= "SELECT RIY.利用者名, RIY.部門ＣＤ, RIY.荷送人ＣＤ, \n"
			+       " BUM.部門名, BUM.出荷日, \n"
// ADD 2005.07.21 東都）高木 店所ユーザ対応 START
			+       " RIY.権限１ \n"
// ADD 2005.07.21 東都）高木 店所ユーザ対応 END
			+  " FROM ＣＭ０４利用者 RIY, ＣＭ０２部門 BUM \n";

		private static string GET_TANMATSU_SELECT_2_WHERE
			=   " AND RIY.削除ＦＧ   = '0' \n"
			+   " AND RIY.会員ＣＤ   =  BUM.会員ＣＤ \n"
			+   " AND RIY.部門ＣＤ   =  BUM.部門ＣＤ \n";

		private static string SELECT_COUNT
//			= "SELECT TO_CHAR(COUNT(*)) \n";
//			= "SELECT NVL(COUNT(*),0) \n";
//			= "SELECT COUNT(*) \n";
			= "SELECT COUNT(ROWID) \n";

		private static string SELECT_COUNT_S
			= "SELECT COUNT(S.ROWID) \n";

		/*********************************************************************
		 * 端末情報取得２
		 * 引数：端末ＣＤ
		 * 戻値：ステータス、会員ＣＤ、サーマルプリンタ有無、プリンタ種類、会員名
		 * 
		 *********************************************************************/
		[WebMethod]
		public String[] Get_tanmatsu2(string[] sUser, string sKey1)
// ADD 2008.06.17 東都）高木 ＭＡＣアドレスチェックの追加 START
		{
			string[] sKey2 = new string[]{sKey1};
			return Get_tanmatsu3(sUser, sKey2);
		}
		[WebMethod]
		public String[] Get_tanmatsu3(string[] sUser, string[] sKey1)
// ADD 2008.06.17 東都）高木 ＭＡＣアドレスチェックの追加 END
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "端末情報取得２開始");

			OracleConnection conn2 = null;
			String[] sRet = new string[5];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			string sQuery      = "";
// MOD 2008.06.17 東都）高木 ＭＡＣアドレスチェックの追加 START
//			string s端末ＣＤ   = sKey1;
			string s端末ＣＤ   = sKey1[0];
// MOD 2008.06.17 東都）高木 ＭＡＣアドレスチェックの追加 END
			string s会員ＣＤ   = "";
			string sPrtFg      = "";
			string sPrtKind    = "";
// DEL 2005.06.02 東都）高木 ORA-03113対策？ START
//			string s会員名     = "";
// DEL 2005.06.02 東都）高木 ORA-03113対策？ END
// ADD 2005.06.07 東都）高木 都道府県選択の変更 START
			string s都道府県ＣＤ = "";
// ADD 2005.06.07 東都）高木 都道府県選択の変更 END
// MOD 2011.10.11 東都）高木 ＳＳＬ証明書導入状態などのチェック START
			string s起動状態         = "";
			string s実行コマンド     = "";
			string sInitProxyExists  = (sKey1.Length >  6) ? sKey1[ 6] : "";
			string sInitSyukkaExists = (sKey1.Length >  7) ? sKey1[ 7] : "";
			string sOSVer            = (sKey1.Length >  8) ? sKey1[ 8] : "";
			string sNetVer           = (sKey1.Length >  9) ? sKey1[ 9] : "";
			string sSSLStatus        = (sKey1.Length > 10) ? sKey1[10] : "";
// MOD 2011.10.11 東都）高木 ＳＳＬ証明書導入状態などのチェック END
			try
			{
				sQuery
					= GET_TANMATSU_SELECT_1
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//					+ " WHERE TAN.端末ＣＤ = '"+ s端末ＣＤ + "' "
					+ " WHERE 端末ＣＤ = '"+ s端末ＣＤ + "' \n"
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
// MOD 2008.06.17 東都）高木 ＭＡＣアドレスチェックの追加 START
//					+ GET_TANMATSU_SELECT_1_WHERE;
					;

// DEL 2008.10.22 東都）高木 ＭＡＣアドレスチェックの停止 START
//				if(sKey1.Length >= 4 && sKey1[3].Length > 0){
//					sQuery += " AND マシン名 = '"+ sKey1[3] +"' \n";
//				}
// DEL 2008.10.22 東都）高木 ＭＡＣアドレスチェックの停止 END

//				if(sKey1.Length >= 5 && sKey1[4].Length > 0){
//					sQuery += " AND ＩＰ = '"+ sKey1[4] +"' \n";
//				}
// MOD 2008.07.16 東都）高木 ＭＡＣアドレスの簡易マスキング START
//// ADD 2008.07.07 東都）高木 ＭＡＣアドレス取得失敗時対応 START
//				if(sKey1.Length >= 6 && sKey1[5].Length == 0) sKey1[5] = sKey1[3];
//// ADD 2008.07.07 東都）高木 ＭＡＣアドレス取得失敗時対応 END
//				if(sKey1.Length >= 6 && sKey1[5].Length > 0){
//					sQuery += " AND ＭＡＣ = '"+ sKey1[5] +"' \n";
//				}
//				sQuery += GET_TANMATSU_SELECT_1_WHERE;
//// MOD 2008.06.17 東都）高木 ＭＡＣアドレスチェックの追加 END
// DEL 2008.10.22 東都）高木 ＭＡＣアドレスチェックの停止 START
//				if(sKey1.Length >= 6){
//					if(sKey1[5].Length == 0){
////						sKey1[5] = sKey1[3];
//						sKey1[5] = "-";
//					}else if(sKey1[5].Length == 17 && sKey1[5].Substring(0,1).Equals("Z")){
//						sKey1[5] = sKey1[5].Substring(1,2) + "-"
//								  + sKey1[5].Substring(4,2) + "-"
//								  + sKey1[5].Substring(7,2) + "-"
//								  + sKey1[5].Substring(10,2) + "-"
//								  + sKey1[5].Substring(13,2) + "-"
//								  + sKey1[5].Substring(15,2)
//								  ;
//					}
//					if(sKey1[5].Length > 0){
//						sQuery += " AND ＭＡＣ = '"+ sKey1[5] +"' \n";
//					}
//				}
// DEL 2008.10.22 東都）高木 ＭＡＣアドレスチェックの停止 END
				sQuery += GET_TANMATSU_SELECT_1_WHERE;
// MOD 2008.07.16 東都）高木 ＭＡＣアドレスの簡易マスキング END

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					s会員ＣＤ = reader.GetString(0).Trim();
					sPrtFg    = reader.GetString(1).Trim();
					sPrtKind  = reader.GetString(2).Trim();
// DEL 2005.06.02 東都）高木 ORA-03113対策？ START
//					s会員名   = reader.GetString(3).Trim();
// DEL 2005.06.02 東都）高木 ORA-03113対策？ END
// ADD 2005.06.07 東都）高木 都道府県選択の変更 START
					s都道府県ＣＤ = reader.GetString(3).Trim();;
// ADD 2005.06.07 東都）高木 都道府県選択の変更 END
// MOD 2011.10.11 東都）高木 ＳＳＬ証明書導入状態などのチェック START
					s起動状態     = reader.GetString(4).TrimEnd();
					s実行コマンド = reader.GetString(5).TrimEnd();
// MOD 2011.10.11 東都）高木 ＳＳＬ証明書導入状態などのチェック END
// MOD 2005.07.01 東都）高木 端末情報が削除されていた場合の対応 START
//				}
//
//				sRet[0] = "正常終了";
					sRet[0] = "正常終了";
// ADD 2008.12.25 東都）高木 ログイン時のレベルアップ促進メッセージの追加 START
					if(sUser.Length < 4)
					{
// MOD 2009.02.12 東都）高木 バージョンが1.9以前のユーザではログイン不可 START
//						// [実行コマンド]と[ＤＢ日付]が異なる場合
//						if(!reader.GetString(4).Trim().Equals(reader.GetString(5).Trim()))
//						{
//							disposeReader(reader);
//							reader = null;
//							
//							OracleTransaction tran = conn2.BeginTransaction();
//							try
//							{
//								// 利用者マスタの更新
//								sQuery
//									= "UPDATE ＣＭ０３端末 \n"
//									+   " SET 実行コマンド = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
//									+ " WHERE 端末ＣＤ = '"+ s端末ＣＤ + "' \n"
//									;
//
//								CmdUpdate(sUser, conn2, sQuery);
//								tran.Commit();
//							}
//							catch (OracleException ex)
//							{
//								tran.Rollback();
//								throw ex;
//							}
//							catch (Exception ex)
//							{
//								tran.Rollback();
//								throw ex;
//							}
//
//									// １２３４５６７８９＊１２３４５６７８９＊１２３４５６７８９＊
//							sRet[0] = "お客様にご利用いただいているアプリケーションを最新のものに更新する必要があります。　\n"
//									+ "バージョンアップの作業をお願い致します。　\n"
//									+ "詳しくは、福山通運のｉＳＴＡＲ－２ダウンロード画面にある［再セットアップ手順書］をご覧下さい。　\n"
//									;
//						}
								// １２３４５６７８９＊１２３４５６７８９＊１２３４５６７８９＊
						sRet[0] = "お客様にご利用いただいているアプリケーションを最新のものに更新する必要があります。　\n"
								+ "バージョンアップの作業をお願い致します。　\n"
								+ "詳しくは、福山通運のｉＳＴＡＲ－２ダウンロード画面にある［再セットアップ手順書］をご覧下さい。　\n"
								;
// MOD 2009.02.12 東都）高木 バージョンが1.9以前のユーザではログイン不可 END
					}
// ADD 2008.12.25 東都）高木 ログイン時のレベルアップ促進メッセージの追加 END
// MOD 2011.10.11 東都）高木 ＳＳＬ証明書導入状態などのチェック START
					if(sRet[0].Length == 4){
						string w起動状態 = "";
						if(sSSLStatus.Length > 0){
							w起動状態 = sSSLStatus.Substring(0,1);
						}
						string w実行コマンド = sOSVer;
						if(sOSVer.StartsWith("4.0.")){
							w実行コマンド = "NT4.0" + " " + sOSVer.Substring(4); 
						}else if(sOSVer.StartsWith("4.10.")){
							w実行コマンド = "98" + " " + sOSVer.Substring(5);
						}else if(sOSVer.StartsWith("4.90.")){
							w実行コマンド = "Me" + " " + sOSVer.Substring(5);
						}else if(sOSVer.StartsWith("5.0.")){
							w実行コマンド = "2000" + " " + sOSVer.Substring(4);
						}else if(sOSVer.StartsWith("5.1.")){
							w実行コマンド = "XP" + " " + sOSVer.Substring(4);
						}else if(sOSVer.StartsWith("5.2.")){
							w実行コマンド = "2003" + " " + sOSVer.Substring(4);
						}else if(sOSVer.StartsWith("6.0.")){
							w実行コマンド = "Vista" + " " + sOSVer.Substring(4);
						}else if(sOSVer.StartsWith("6.1.")){
							w実行コマンド = "7" + " " + sOSVer.Substring(4);
						}
// MOD 2016.04.05 BEVAS）松本 Windows10対応 START
						//※Win8,Win8.1の対応漏れがあった為、Win10と併せて対応
						else if(sOSVer.StartsWith("6.2."))
						{
							//Windows8
							w実行コマンド = "8" + " " + sOSVer.Substring(4);
						}
						else if(sOSVer.StartsWith("6.3."))
						{
							//Windows8.1
							w実行コマンド = "8.1" + " " + sOSVer.Substring(4);
						}						
						else if(sOSVer.StartsWith("10.0."))
						{
							//Windows10
							w実行コマンド = "10" + " " + sOSVer.Substring(4);
						}
// MOD 2016.04.05 BEVAS）松本 Windows10対応 END
						if(w実行コマンド.Length > 8){
							w実行コマンド = w実行コマンド.Substring(0,8);
						}

						if(s起動状態 != sSSLStatus || s実行コマンド != w起動状態 ){
							logWriter(sUser, INF, "端末情報取得３"
								+ " SSL状態["+ sSSLStatus +"]"
								+ " OS["+ w実行コマンド +"]["+ sOSVer +"] .NET["+ sNetVer +"]"
								+ " プロキシ設定F["+ sInitProxyExists +"]"
								+ " 自動出力設定F["+ sInitSyukkaExists +"]"
								);

							if(w起動状態.Length == 0) w起動状態 = " ";
							if(w実行コマンド.Length == 0) w実行コマンド = " ";

							OracleTransaction tran = conn2.BeginTransaction();
							try{
								// 利用者マスタの更新
								sQuery = "UPDATE ＣＭ０３端末 \n"
									+   "SET 起動状態   = '"+ w起動状態 +"' \n"
									+   ", 実行コマンド = '"+ w実行コマンド +"' \n"
									+   ", 実行日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
									+ " WHERE 端末ＣＤ = '"+ s端末ＣＤ +"' \n"
									;
								CmdUpdate(sUser, conn2, sQuery);
								tran.Commit();
							}catch(OracleException ex){
								logWriter(sUser, ERR, chgDBErrMsg(sUser, ex));
								tran.Rollback();
//								throw ex;
							}catch (Exception ex){
								logWriter(sUser, ERR, "サーバエラー：" + ex.Message);
								tran.Rollback();
//								throw ex;
							}
						}
					}
// MOD 2011.10.11 東都）高木 ＳＳＬ証明書導入状態などのチェック END
				}
				else
				{
					sRet[0] = "端末情報無";
				}
// MOD 2005.07.01 東都）高木 端末情報が削除されていた場合の対応 END
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			sRet[1] = s会員ＣＤ;
			sRet[2] = sPrtFg;
			sRet[3] = sPrtKind;
// MOD 2005.06.07 東都）高木 都道府県選択の変更 START
//// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
////			sRet[4] = s会員名;
//			sRet[4] = "";
//// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
			sRet[4] = s都道府県ＣＤ;
// MOD 2005.06.07 東都）高木 都道府県選択の変更 END

			return sRet;
		}

		/*********************************************************************
		 * 利用者情報取得
		 * 引数：会員ＣＤ、利用者ＣＤ
		 * 戻値：ステータス、利用者名、部門ＣＤ、部門名、出荷日、荷送人ＣＤ、
		 *		 部門数、得意先数
		 *		 得意先ＣＤ、得意先部課ＣＤ、得意先部課名
		 *********************************************************************/
		[WebMethod]
		public String[] Get_riyou(string[] sUser, string sKey1, string sKey2)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "利用者情報取得開始");

			OracleConnection conn2 = null;
			String[] sRet = new string[8];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 認証後の実行なので不要 ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				logFileClose();
//				conn.Close();
//				return sRet;
//			}
// DEL 認証後の実行なので不要 ADD 2005.05.24 東都）高木 会員チェック追加 END

			string sQuery      = "";
			string sQuery1     = "";
			string sQuery2     = "";
			string s会員ＣＤ   = sKey1;
			string s利用ＣＤ   = sKey2;
			string s利用者名   = "";
			string s部門ＣＤ   = "";
			string s部門名     = "";
			string s出荷日     = "";
			string s荷送人ＣＤ = "";
			string s部門数     = "0";
			string s得意先数   = "0";
// ADD 2005.07.21 東都）高木 店所ユーザ対応 START
			string s権限１     = "";
// ADD 2005.07.21 東都）高木 店所ユーザ対応 END
			try
			{
				// 利用者情報の取得
				sQuery
					= GET_TANMATSU_SELECT_2
					+ " WHERE RIY.会員ＣＤ   = '"+ s会員ＣＤ + "' \n"
					+   " AND RIY.利用者ＣＤ = '"+ s利用ＣＤ + "' \n"
					+ GET_TANMATSU_SELECT_2_WHERE;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					s利用者名   = reader.GetString(0).Trim();
					s部門ＣＤ   = reader.GetString(1).Trim();
					s荷送人ＣＤ = reader.GetString(2).Trim();
					s部門名     = reader.GetString(3).Trim();
					s出荷日     = reader.GetString(4).Trim();
// ADD 2005.07.21 東都）高木 店所ユーザ対応 START
					s権限１     = reader.GetString(5).Trim();
// ADD 2005.07.21 東都）高木 店所ユーザ対応 END
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				// 部門数の取得
				int iCntBumon = 0;
				sQuery1
					=  " FROM ＣＭ０２部門 \n"
					+ " WHERE 会員ＣＤ = '"+ s会員ＣＤ + "' \n"
					+   " AND 削除ＦＧ = '0' \n"
					;

				reader = CmdSelect(sUser, conn2, SELECT_COUNT + sQuery1);

				if (reader.Read())
				{
//					s部門数 = reader.GetString(0);
					s部門数 = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				iCntBumon = int.Parse(s部門数);

				// 得意先数の取得
				int iCntTokui = 0;
				sQuery2
					=  " FROM ＣＭ０２部門     B, \n"
					+       " ＳＭ０４請求先   S  \n"
					+ " WHERE B.会員ＣＤ = '"+ s会員ＣＤ + "' \n"
					+   " AND B.部門ＣＤ = '"+ s部門ＣＤ + "' \n"
					+   " AND B.削除ＦＧ = '0' \n"
					+   " AND B.郵便番号      = S.郵便番号 \n"
					+   " AND '"+s会員ＣＤ+"' = S.会員ＣＤ \n"
					+   " AND '0'             = S.削除ＦＧ \n"
					;

				reader = CmdSelect(sUser, conn2, SELECT_COUNT_S + sQuery2);

				if (reader.Read())
				{
//					s得意先数 = reader.GetString(0);
					s得意先数 = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				iCntTokui = int.Parse(s得意先数);

				int iPos = sRet.Length;
				if(iCntBumon > 0 || iCntTokui > 0)
				{
					sRet = new string[iPos + (iCntBumon * 3) + (iCntTokui * 3) ];
				}

				// 部門情報の取得
				if(iCntBumon > 0)
				{
					sQuery
						= "SELECT 部門ＣＤ, 部門名, 出荷日 \n"
						+ sQuery1
						+ " ORDER BY 組織ＣＤ, 出力順 \n"
						;

					reader = CmdSelect(sUser, conn2, sQuery);

					while (reader.Read())
					{
						sRet[iPos++] = reader.GetString(0).Trim();
						sRet[iPos++] = reader.GetString(1).Trim();
						sRet[iPos++] = reader.GetString(2).Trim();
					}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				}

				// 得意先情報の取得
				if(iCntTokui > 0)
				{
					sQuery
						= "SELECT S.得意先ＣＤ, S.得意先部課ＣＤ, S.得意先部課名 \n"
						+ sQuery2
//保留 MOD 2011.09.16 東都）高木 請求先名が同じ場合のソート順対応 START
						+ " ORDER BY S.得意先部課名 \n"
//						+ " ORDER BY S.得意先部課名, S.得意先ＣＤ, S.得意先部課ＣＤ \n"
//保留 MOD 2011.09.16 東都）高木 請求先名が同じ場合のソート順対応 END
						;

					reader = CmdSelect(sUser, conn2, sQuery);

					while (reader.Read())
					{
						sRet[iPos++] = reader.GetString(0).Trim();
						sRet[iPos++] = reader.GetString(1).Trim();
						sRet[iPos++] = reader.GetString(2).Trim();
					}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				}
				sRet[0] = "正常終了";

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			sRet[1] = s利用者名;
			sRet[2] = s部門ＣＤ;
			sRet[3] = s部門名;
			sRet[4] = s出荷日;
			sRet[5] = s荷送人ＣＤ;
			sRet[6] = s部門数;
			sRet[7] = s得意先数;

// ADD 2005.07.21 東都）高木 店所ユーザ対応 START
			sRet.CopyTo(sRet = new string[sRet.Length + 1], 0);
			sRet[sRet.Length - 1] = s権限１;
// ADD 2005.07.21 東都）高木 店所ユーザ対応 END

// ADD 2007.10.19 東都）高木 端末バージョン管理 START
			if(sUser.Length >= 4)
			{
				// 端末バージョン更新
				sUser[0] = s会員ＣＤ;
				sUser[1] = s利用ＣＤ;
				string[] sKeyVer = {s部門ＣＤ,"メニュー"};
				string   sRetVer = Upd_bumon_ver(sUser, sKeyVer);
			}
// ADD 2007.10.19 東都）高木 端末バージョン管理 END

			return sRet;
		}

		/*********************************************************************
		 * 請求先情報取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、得意先数
		 *		 得意先ＣＤ、得意先部課ＣＤ、得意先部課名
		 *********************************************************************/
		[WebMethod]
		public String[] Get_seikyu(string[] sUser, string sKey1, string sKey2)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "請求先情報取得開始");

			OracleConnection conn2 = null;
			String[] sRet = new string[2];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 東都）高木 会員チェック追加 END

			string sQuery    = "";
			string sQuery1   = "";
			string s得意先数 = "0";
			try
			{
				// 得意先数の取得
				int iCntTokui = 0;
				sQuery1
					=  " FROM ＣＭ０２部門     B, \n"
					+       " ＳＭ０４請求先   S  \n"
					+ " WHERE B.会員ＣＤ = '"+ sKey1 + "' \n"
					+   " AND B.部門ＣＤ = '"+ sKey2 + "' \n"
					+   " AND B.削除ＦＧ = '0' \n"
					+   " AND B.郵便番号     = S.郵便番号 \n"
					+   " AND '"+ sKey1 + "' = S.会員ＣＤ \n"
					+   " AND '0'            = S.削除ＦＧ \n"
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, SELECT_COUNT_S + sQuery1);

				if (reader.Read())
				{
//					s得意先数 = reader.GetString(0);
					s得意先数 = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				iCntTokui = int.Parse(s得意先数);

				int iPos = 2;
				if(iCntTokui > 0)
				{
					sRet = new string[2 + (iCntTokui * 3) ];
				}

				// 得意先情報の取得
				if(iCntTokui > 0)
				{
					sQuery
						= "SELECT S.得意先ＣＤ, S.得意先部課ＣＤ, S.得意先部課名 \n"
						+ sQuery1
//保留 MOD 2011.09.16 東都）高木 請求先名が同じ場合のソート順対応 START
						+  "ORDER BY S.得意先部課名 \n"
//						+  "ORDER BY S.得意先部課名, S.得意先ＣＤ, S.得意先部課ＣＤ \n"
//保留 MOD 2011.09.16 東都）高木 請求先名が同じ場合のソート順対応 END
						;

					reader = CmdSelect(sUser, conn2, sQuery);

					while (reader.Read())
					{
						sRet[iPos++] = reader.GetString(0).Trim();
						sRet[iPos++] = reader.GetString(1).Trim();
						sRet[iPos++] = reader.GetString(2).Trim();
					}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				}
				sRet[0] = "正常終了";

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			sRet[1] = s得意先数;

			return sRet;
		}

		/*********************************************************************
		 * ログイン認証
		 * 引数：会員ＣＤ、利用ＣＤ、パスワード
		 * 戻値：ステータス、会員ＣＤ、会員名、メッセージ、利用者ＣＤ、利用者名、メッセージ
		 *********************************************************************/
		private static string LOGIN_SELECT
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//			= "SELECT CM01.会員ＣＤ, CM01.会員名, \n"
//			+       " CM04.利用者ＣＤ, CM04.利用者名, \n"
//			+       " CM04.パスワード, TO_CHAR(CM04.認証エラー回数), \n"
//			+       " CM01.\"メッセージ\" \n"
			= "SELECT CM01.会員ＣＤ, \n"
			+       " CM01.会員名, \n"
			+       " CM04.利用者ＣＤ, \n"
			+       " CM04.利用者名, \n"
			+       " CM04.\"パスワード\", \n"
			+       " CM04.\"認証エラー回数\", \n"
			+       " CM01.使用開始日, \n"
			+       " CM01.使用終了日, \n"
			+       " SYSDATE \n"
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
			+       ", CM04.登録ＰＧ \n"
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
			+  " FROM ＣＭ０１会員   CM01, \n"
// ADD 2005.08.19 東都）高木 部門削除の対応 START
			+       " ＣＭ０２部門   CM02, \n"
// ADD 2005.08.19 東都）高木 部門削除の対応 END
			+       " ＣＭ０４利用者 CM04  \n";

		private static string LOGIN_SELECT_WHERE
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//			=   " AND CM01.使用開始日 <= TO_CHAR(SYSDATE,'YYYYMMDD') \n"
//			+   " AND CM01.使用終了日 >= TO_CHAR(SYSDATE,'YYYYMMDD') \n"
//			+   " AND CM01.管理者区分 IN ('0','1','9') \n"  // 0:一般 1:管理者 9:メンテナンス
//			+   " AND CM01.削除ＦＧ = '0' \n"
//			+   " AND CM04.削除ＦＧ = '0' \n";
			=    " AND CM04.削除ＦＧ = '0' \n"
			+    " AND CM04.会員ＣＤ = CM01.会員ＣＤ \n"
			+    " AND CM01.削除ＦＧ = '0' \n"
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
// ADD 2005.08.19 東都）高木 部門削除の対応 START
			+    " AND CM04.会員ＣＤ = CM02.会員ＣＤ \n"
			+    " AND CM04.部門ＣＤ = CM02.部門ＣＤ \n"
			+    " AND CM02.削除ＦＧ = '0' \n";
// ADD 2005.08.19 東都）高木 部門削除の対応 END

		[WebMethod]
		public string[] login(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "ログイン認証開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[6];

// ADD 2008.03.21 東都）高木 レベルアップ促進対応 START
// ※ is2Webapplicationでも使用しているので注意
//			if(sUser.Length < 4)
//			{
//				sRet[0] = "御客様のＩＤは、利用制限がされています　\n"
//						+ "最寄の営業所まで御連絡下さい";
//				return sRet;
//			}
// ADD 2008.03.21 東都）高木 レベルアップ促進対応 END

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			string sQuery = "";
			string s会員ＣＤ = sKey[0];
			string s利用ＣＤ = sKey[1];
			string sパスワド = sKey[2];
			int i認証エラー回数 = 0;

			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				sQuery
					= LOGIN_SELECT
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//					+ " WHERE CM01.会員ＣＤ   = '" + s会員ＣＤ + "' \n"
//					+   " AND CM01.会員ＣＤ   = CM04.会員ＣＤ \n"
//					+   " AND CM04.利用者ＣＤ = '" + s利用ＣＤ + "' \n"
////					+   " AND CM04.パスワード = '" + sパスワド + "' \n"
					+ " WHERE CM04.会員ＣＤ = '" + s会員ＣＤ + "' \n"
					+   " AND CM04.利用者ＣＤ = '" + s利用ＣＤ + "' \n"
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
					+ LOGIN_SELECT_WHERE
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
					sRet[3] = reader.GetString(2).Trim();
					sRet[4] = reader.GetString(3).Trim();
// MOD 2005.06.02 東都）高木 ORA-03113対策？ START
//					i認証エラー回数 = int.Parse(reader.GetString(5));
//					sRet[5] = reader.GetString(6).Trim();
					i認証エラー回数 = int.Parse(reader.GetDecimal(5).ToString());
					sRet[5] = "";
					string s使用開始日 = reader.GetString(6).Trim();
					int    i使用開始日 = int.Parse(s使用開始日);
					int    i使用終了日 = int.Parse(reader.GetString(7).Trim());
					int    i当日       = int.Parse(reader.GetDateTime(8).ToString("yyyyMMdd").Trim());
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
					int    iパス更新日 = 0;
					try{
						iパス更新日 = int.Parse(reader.GetString(9).Trim());
					}catch (Exception ){
						;
					}
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END
					if (i当日 < i使用開始日)
					{
						if(s使用開始日.Length == 8)
						{
							string s年 = s使用開始日.Substring(0,4);
							string s月 = s使用開始日.Substring(4,2);
							string s日 = s使用開始日.Substring(6,2);
							if(s月[0] == '0') s月 = s月.Substring(1,1);
							if(s日[0] == '0') s日 = s日.Substring(1,1);
							sRet[0] = s年 + "年" + s月 + "月"
									+ s日 + "日より使用できます";
						}
						else
						{
							sRet[0] = "使用開始日より使用できます";
						}
						tran.Commit();
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
						return sRet;
					}
					if (i当日 > i使用終了日)
					{
						sRet[0] = "使用期限が切れています";
						tran.Commit();
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
						return sRet;
					}
// MOD 2005.06.02 東都）高木 ORA-03113対策？ END
// ADD 2008.05.21 東都）高木 ログインエラー回数を５回にする START
//					if (i認証エラー回数 >= 10)
					if (i認証エラー回数 >= 5)
// ADD 2008.05.21 東都）高木 ログインエラー回数を５回にする END
					{
// MOD 2005.08.19 東都）高木 メッセージの変更 START
//						sRet[0] = "認証エラー：最寄の営業所まで御連絡下さい";
						sRet[0] = "御客様のＩＤは、利用制限がされています　\n"
// MOD 2009.09.14 東都）高木 パスワードエラー時の問い合わせ先の変更 START
//								+ "最寄の営業所まで御連絡下さい";
								+ "ＱＡセンターまたは最寄の営業所まで御連絡下さい";
// MOD 2009.09.14 東都）高木 パスワードエラー時の問い合わせ先の変更 END
// MOD 2005.08.19 東都）高木 メッセージの変更 END
						//Session.Clear();
					}
					else if(reader.GetString(4).Trim() == sパスワド)
					{
						sRet[0] = "正常終了";
						//Session.Add("member", sRet[1]);
						//Session.Add("user",   sRet[3]);
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 START
						DateTime dt当日 = new DateTime(
							int.Parse(i当日.ToString().Substring(0,4))
							, int.Parse(i当日.ToString().Substring(4,2))
							, int.Parse(i当日.ToString().Substring(6,2))
							);
						DateTime dtパス更新日 = new DateTime(
							int.Parse(iパス更新日.ToString().Substring(0,4))
							, int.Parse(iパス更新日.ToString().Substring(4,2))
							, int.Parse(iパス更新日.ToString().Substring(6,2))
							);
						DateTime dtパス有効期限 = dtパス更新日.AddMonths(6);

						//６ヶ月以上経過した時点で、パスワード更新警告表示
						if(dt当日.CompareTo(dtパス有効期限) > 0){
							sRet[0] = "期限切れ";
						//５ヶ月以上経過した時点で、パスワード更新警告表示
						}else if(dt当日.CompareTo(dtパス更新日.AddMonths(5)) > 0){
							sRet[0] = dtパス有効期限.ToString("MMdd").Trim();
						}
// ADD 2008.06.30 東都）高木 パスワードチェックの強化 END
					}
					else
					{
// MOD 2009.05.27 東都）高木 パスワードエラー時にパスワード更新日を表示 START
//						sRet[0] = "利用者ＣＤ もしくは パスワード に誤りがあります";
						sRet[0] = "利用者ＣＤ もしくは パスワード に誤りがあります\n"
								+ "　　　　　（"
								+ int.Parse(iパス更新日.ToString().Substring(4,2)) + "/"
								+ int.Parse(iパス更新日.ToString().Substring(6,2))
								+ " に変更されています）"
								;
// MOD 2009.05.27 東都）高木 パスワードエラー時にパスワード更新日を表示 END
						//Session.Clear();
					}
				}
				else
				{
					sRet[0] = "利用者ＣＤ もしくは パスワード に誤りがあります";
					//Session.Clear();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				// 利用者マスタを更新する
				if(sRet[0].Length != 4 || i認証エラー回数 != 0)
				{
					if (sRet[0].Length == 4){
						i認証エラー回数 = 0;
					}else if(i認証エラー回数 < 90){
						i認証エラー回数++;
					}else{
						i認証エラー回数 = 90;
					}

					// 利用者マスタの更新
					sQuery
						= "UPDATE ＣＭ０４利用者 \n"
						+   " SET 認証エラー回数 = " + i認証エラー回数 + ", \n"
						+       " 更新日時   = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+       " 更新ＰＧ   = '" + "ログイン" + "', \n"
						+       " 更新者     = '" + s利用ＣＤ + "' \n"
						+ " WHERE 会員ＣＤ   = '" + s会員ＣＤ + "' \n"
						+   " AND 利用者ＣＤ = '" + s利用ＣＤ + "' \n"
						;
					CmdUpdate(sUser, conn2, sQuery);
				}

// DEL 2007.02.08 東都）高木 クライアントアプリの高速化 START
//// ADD 2005.06.17 東都）高木 端末マスタの更新 START
//				if(sRet[0].Length == 4)
//				{
//					// 端末マスタの更新
//					sQuery
//						= "UPDATE ＣＭ０３端末 \n"
//						+   " SET 起動状態 = '1', \n"
//						+       " 実行画面 = 'ログイン', \n"
//						+       " 実行コマンド = 'ログイン', \n"
//						+       " 実行日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
//						+       " 更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
//						+       " 更新ＰＧ = 'ログイン', \n"
//						+       " 更新者   = '" + s利用ＣＤ + "' \n"
//						+ " WHERE 端末ＣＤ = '" + sUser[2]  + "' \n"
//						;
//
//					CmdUpdate(sUser, conn2, sQuery);
//				}
//// ADD 2005.06.17 東都）高木 端末マスタの更新 END
// DEL 2007.02.08 東都）高木 クライアントアプリの高速化 END

				tran.Commit();

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}

		/*********************************************************************
		 * 状態一覧取得
		 * 引数：なし
		 * 戻値：ステータス、状態一覧
		 *********************************************************************/
		private static string GET_JYOTAI_COUNT
//			= "SELECT TO_CHAR(COUNT(DISTINCT 状態名)) \n"
//			= "SELECT NVL(COUNT(DISTINCT 状態名),0) \n"
// MOD 2005.06.10 東都）高木 ＳＱＬの簡略化 START
//			= "SELECT COUNT(DISTINCT 状態名) \n"
//			+  " FROM ＡＭ０３状態 \n"
//			+ " WHERE 削除ＦＧ = '0' \n";
//			= "SELECT COUNT(*) \n"
			= "SELECT COUNT(ROWID) \n"
			+  " FROM ＡＭ０３状態 \n"
			+ " WHERE 状態詳細ＣＤ = ' ' \n"
			+ " AND 削除ＦＧ = '0' \n";
// MOD 2005.06.10 東都）高木 ＳＱＬの簡略化 END

		private static string GET_JYOTAI
// MOD 2005.06.10 東都）高木 ＳＱＬの簡略化 START
//			= "SELECT DISTINCT 状態ＣＤ, 状態名 \n"
//			+  " FROM ＡＭ０３状態 \n"
//			+ " WHERE 削除ＦＧ = '0' \n"
//			+ " ORDER BY 状態ＣＤ, 状態名 \n";
			= "SELECT 状態ＣＤ, 状態名 \n"
			+  " FROM ＡＭ０３状態 \n"
			+ " WHERE 状態詳細ＣＤ = ' ' \n"
			+ " AND 削除ＦＧ = '0' \n"
			+ " ORDER BY 状態ＣＤ, 状態名 \n";
// MOD 2005.06.10 東都）高木 ＳＱＬの簡略化 END

		[WebMethod]
		public string[] Get_jyotai(string[] sUser )
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "状態一覧取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			string s状態数 = "0";
			int    i状態数 = 0;
			try
			{
				OracleDataReader reader = CmdSelect(sUser, conn2, GET_JYOTAI_COUNT);
				if (reader.Read() )
				{
//					s状態数 = reader.GetString(0);
					s状態数 = reader.GetDecimal(0).ToString().Trim();
				}
				i状態数 = int.Parse(s状態数);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				reader = CmdSelect(sUser, conn2, GET_JYOTAI);

				int iPos = 2;
				sRet = new string[i状態数 * 2 + iPos];
				while (reader.Read() && iPos < sRet.Length)
				{
					sRet[iPos++] = reader.GetString(0).Trim();
					sRet[iPos++] = reader.GetString(1).Trim();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				if(iPos > 2){
					sRet[0] = "正常終了";
					sRet[1] = s状態数;
				}else{
					sRet[0] = "サーバエラー：状態マスタが設定されていません";
					sRet[1] = "0";
				}

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}

		/*********************************************************************
		 * 出荷日取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、出荷日
		 *********************************************************************/
		[WebMethod]
		public String[] Get_syukabi(string[] sUser, string sKey1, string sKey2)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷日取得開始");

			OracleConnection conn2 = null;
			String[] sRet = new string[2];
			// ADD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 東都）高木 会員チェック追加 END

			string sQuery  = "";
			try
			{
				sQuery
					= "SELECT 出荷日 \n"
					+  " FROM ＣＭ０２部門 \n"
					+ " WHERE 会員ＣＤ = '"+ sKey1 + "' \n"
					+   " AND 部門ＣＤ = '"+ sKey2 + "' \n"
					+   " AND 削除ＦＧ = '0' \n"
					;

				// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
				//OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);
				logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + sQuery);	//修正前のUPDATE文をログ出力

				sQuery
					= "SELECT 出荷日 \n"
					+  " FROM ＣＭ０２部門 \n"
					+ " WHERE 会員ＣＤ = :p_KaiinCD \n"
					+   " AND 部門ＣＤ = :p_BumonCD \n"
					+   " AND 削除ＦＧ = '0' \n"
					;
				wk_opOraParam = new OracleParameter[2];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKey1, ParameterDirection.Input);
				wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sKey2, ParameterDirection.Input);

				OracleDataReader	reader = CmdSelect(sUser, conn2, sQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

				if (reader.Read())
				{
					sRet[0] = "正常終了";
					sRet[1] = reader.GetString(0);
				}else{
					sRet[0] = "該当データがありません";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}

		/*********************************************************************
		 * 部門情報取得
		 * 引数：会員ＣＤ
		 * 戻値：ステータス、部門数、部門ＣＤ、部門名、出荷日
		 *********************************************************************/
		[WebMethod]
		public String[] Get_bumon(string[] sUser, string sKey1)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "部門情報取得開始");

			OracleConnection conn2 = null;
			String[] sRet = new string[2];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 東都）高木 会員チェック追加 END

			string s会員ＣＤ = sKey1;
			string sQuery  = "";
			string sQuery1 = "";
			string s部門数 = "0";
			try
			{
				// 部門数の取得
				int iCntBumon = 0;
				sQuery1
					=  " FROM ＣＭ０２部門 \n"
					+ " WHERE 会員ＣＤ = '"+ s会員ＣＤ + "' \n"
					+   " AND 削除ＦＧ = '0' \n"
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, SELECT_COUNT + sQuery1);

				if (reader.Read())
				{
//					s部門数 = reader.GetString(0);
					s部門数 = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				iCntBumon = int.Parse(s部門数);

				int iPos = sRet.Length;
				if(iCntBumon > 0)
				{
					sRet = new string[iPos + (iCntBumon * 3)];
				}

				// 部門情報の取得
				if(iCntBumon > 0)
				{
					sQuery
						= "SELECT 部門ＣＤ, 部門名, 出荷日 \n"
						+ sQuery1
						+ " ORDER BY 組織ＣＤ, 出力順 \n"
						;

					reader = CmdSelect(sUser, conn2, sQuery);

					while (reader.Read())
					{
						sRet[iPos++] = reader.GetString(0).Trim();
						sRet[iPos++] = reader.GetString(1).Trim();
						sRet[iPos++] = reader.GetString(2).Trim();
					}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				}

				sRet[0] = "正常終了";

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			sRet[1] = s部門数;

			return sRet;
		}

		/*********************************************************************
		 * メッセージの取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、店所メッセージ、会員メッセージ
		 *********************************************************************/
// ADD 2005.05.24 東都）高木 システムメッセージの追加 START
		private static string GET_MESSAGE_SELECT_1
			= "SELECT \"メッセージ\" \n"
			+  " FROM ＡＭ０１システム管理 \n"
			+ " WHERE システム管理ＣＤ = 'is2' \n";
// ADD 2005.05.24 東都）高木 システムメッセージの追加 END

		[WebMethod]
		public String[] Get_message(string[] sUser, string sKey1, string sKey2)
		{
//			logFileOpen(sUser);
//			logWriter(sUser, INF, "メッセージ取得開始");

// MOD 2005.05.24 東都）高木 システムメッセージの追加 START
//			String[] sRet = new string[3];
			OracleConnection conn2 = null;
			string[] sRet = new string[4];
// MOD 2005.05.24 東都）高木 システムメッセージの追加 END

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//
//				sRet[0] = "正常終了";
//				sRet[1] = "";
//				sRet[2] = "";
//				return sRet;
//			}
// ADD 2005.05.24 東都）高木 会員チェック追加 END

			string sQuery  = "";
// ADD 2005.05.24 東都）高木 システムメッセージの追加 START
			string sシステムメッセージ = "";
// ADD 2005.05.24 東都）高木 システムメッセージの追加 END
			string s店所メッセージ = "";
			string s会員メッセージ = "";
			try
			{
// ADD 2005.05.24 東都）高木 システムメッセージの追加 START
				OracleDataReader reader;

				sQuery = GET_MESSAGE_SELECT_1;

				reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					sシステムメッセージ = reader.GetString(0).Trim();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// ADD 2005.05.24 東都）高木 システムメッセージの追加 END

// DEL 2005.06.02 東都）高木 不要な用件なのでコメント化 START
//				sQuery
//					= "SELECT T.\"メッセージ\" \n"
//					+  " FROM ＣＭ０２部門 B, ＣＭ１４郵便番号 Y, ＣＭ１０店所 T \n"
//					+ " WHERE B.会員ＣＤ = '"+ sKey1 + "' \n"
//					+   " AND B.部門ＣＤ = '"+ sKey2 + "' \n"
//					+   " AND B.郵便番号 = Y.郵便番号 \n"
//					+   " AND Y.店所ＣＤ = T.店所ＣＤ \n"
//					;
//
//				reader = CmdSelect(sUser, conn2, sQuery);
//
//				if (reader.Read())
//				{
//					s店所メッセージ = reader.GetString(0).Trim();
//				}
// DEL 2005.06.02 東都）高木 不要な用件なのでコメント化 END

// DEL 2007.02.08 東都）高木 クライアントアプリの高速化 START
//				sQuery
//					= "SELECT \"メッセージ\" \n"
//					+  " FROM ＣＭ０１会員 \n"
//					+ " WHERE 会員ＣＤ = '"+ sKey1 + "' \n"
//					;
//
//				reader = CmdSelect(sUser, conn2, sQuery);
//
//				if (reader.Read())
//				{
//					s会員メッセージ = reader.GetString(0).Trim();
//				}
// DEL 2007.02.08 東都）高木 クライアントアプリの高速化 END

				sRet[0] = "正常終了";
// MOD 2005.05.24 東都）高木 システムメッセージの追加 START
//				sRet[1] = s店所メッセージ;
//				sRet[2] = s会員メッセージ;

// ADD 2008.07.07 東都）高木 特定の会員にはメッセージ非表示 START
				//特定の会員の場合には、メッセージを表示しない
				//会員[（有）岩下物流]様
				//部門[（株）ミマキエンジニアリング]様
				if(sKey1.Trim().Equals("0268631151")){
// MOD 2008.09.19 東都）高木 レベルアップ促進対応 START
//					sRet[0] = "正常終了";
//					sシステムメッセージ = "";
//					sRet[2] = "";
//					sRet[3] = "";
//					return sRet;
					sシステムメッセージ = "";
// MOD 2008.09.19 東都）高木 レベルアップ促進対応 END
				}
				//会員[（株）栄工社]様
				//部門[（株）栄工社]様
				if(sKey1.Trim().Equals("0849213322")
				&& sKey2.Trim().Equals("0849213322")){
// MOD 2008.09.19 東都）高木 レベルアップ促進対応 START
//					sRet[0] = "正常終了";
//					sシステムメッセージ = "";
//					sRet[2] = "";
//					sRet[3] = "";
//					return sRet;
					sシステムメッセージ = "";
// MOD 2008.09.19 東都）高木 レベルアップ促進対応 END
				}
// ADD 2008.07.07 東都）高木 特定の会員にはメッセージ非表示 END
// MOD 2010.07.29 東都）高木 特定の会員にはメッセージ非表示 START
				//会員[丸成商事㈱　京都支社]様
				if(sKey1.Trim().Equals("0756213939")){
					sシステムメッセージ = "";
				}
// MOD 2010.07.29 東都）高木 特定の会員にはメッセージ非表示 END

// ADD 2008.03.21 東都）高木 レベルアップ促進対応 START
// ※ is2Webapplicationでも使用しているので注意
				if(sUser.Length < 4)
				{
					sシステムメッセージ
						= "プログラムの入れ替えが必要です。"
						+ "詳しくは、福山通運のｉＳＴＡＲ－２ダウンロード画面にある"
						+ "［再セットアップ手順書］をご覧下さい。"
//						= "毎度ご利用ありがとうございます。"
//						= "本アプリのバージョンアップ作業が必要な時期になりました。"
//						+ "詳しくは、［ヘルプ］の［再セットアップ手順］まで。"
						+ sシステムメッセージ
						+ "";
				}
// MOD 2009.10.05 東都）高木 マイナーバージョン２桁対応（Ver.2.10～）START
//// MOD 2008.07.07 東都）高木 レベルアップ促進のレベル変更 START
////				else if (double.Parse(sUser[3]) < 2.1)
//				else if (double.Parse(sUser[3]) < 2.3)
//// MOD 2008.07.07 東都）高木 レベルアップ促進のレベル変更 END
				else
				{
					try
					{
						//１個目のピリオド
						int iPos = sUser[3].IndexOf('.');
						int iMajor = int.Parse(sUser[3].Substring(0,iPos));
						int iMiner = int.Parse(sUser[3].Substring(iPos+1));
						if((iMajor < 2)
							|| (iMajor == 2 && iMiner < 3))
// MOD 2009.10.05 東都）高木 マイナーバージョン２桁対応（Ver.2.10～）END
						{
							sシステムメッセージ
								= "プログラムの入れ替えが必要です。"
								+ "詳しくは、福山通運のｉＳＴＡＲ－２ダウンロード画面にある"
								+ "［再セットアップ手順書］をご覧下さい。"
								//						= "毎度ご利用ありがとうございます。"
								//						= "本アプリのバージョンアップ作業が必要な時期になりました。"
								//						+ "詳しくは、［ヘルプ］の［再セットアップ手順］まで。"
								+ sシステムメッセージ
								+ "";
						}
// ADD 2008.03.21 東都）高木 レベルアップ促進対応 END
// MOD 2009.10.05 東都）高木 マイナーバージョン２桁対応（Ver.2.10～）START
					}
					catch(Exception)
					{
						;
					}
				}
// MOD 2009.10.05 東都）高木 マイナーバージョン２桁対応（Ver.2.10～）END

				sRet[1] = sシステムメッセージ;
				sRet[2] = s店所メッセージ;
				sRet[3] = s会員メッセージ;
// MOD 2005.05.24 東都）高木 システムメッセージの追加 END

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}

// ADD 2005.05.18 東都）小童谷 画面制御の取得 START
		/*********************************************************************
		 * 画面制御の取得
		 * 引数：会員ＣＤ、部門ＣＤ、項目ＣＤ
		 * 戻値：ステータス、非表示ＦＧ
		 *********************************************************************/
		[WebMethod]
// MOD 2005.06.10 東都）伊賀　画面制御取得処理変更 START
//		public String[] Get_seigyo(string[] sUser, string sKey1, string sKey2, string sKey3)
		public String[] Get_seigyo(string[] sUser, string sKey1, string sKey2)
// MOD 2005.06.10 東都）伊賀　画面制御取得処理変更 END
// ADD 2009.01.30 東都）高木 実績一覧印刷オプション項目の追加 START
		{
			return Get_seigyo2(sUser, sKey1, sKey2, 11);
		}
		[WebMethod]
		public String[] Get_seigyo2(string[] sUser, string sKey1, string sKey2, int iLength)
// ADD 2009.01.30 東都）高木 実績一覧印刷オプション項目の追加 END
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "画面制御取得開始");

			OracleConnection conn2 = null;
			String[] sRet = new string[1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 東都）高木 会員チェック追加 END

			string sQuery  = "";
// ADD 2005.06.10 東都）伊賀　画面制御取得処理変更 START
// MOD 2006.06.28 東都）山本　エントリオプション項目追加 START
//			sRet = new String[11];
// MOD 2009.01.30 東都）高木 実績一覧印刷オプション項目の追加 START
//			sRet = new String[12];
			sRet = new String[1 + iLength];
// MOD 2009.01.30 東都）高木 実績一覧印刷オプション項目の追加 END
// MOD 2006.06.28 東都）山本　エントリオプション項目追加 END
			for (int iCnt = 1; iCnt < sRet.Length; iCnt++)
			{
				sRet[iCnt] = "9";
			}
// ADD 2005.06.10 東都）伊賀　画面制御取得処理変更 END
			try
			{
				sQuery
// MOD 2005.06.10 東都）伊賀　画面制御取得処理変更 START
//					= "SELECT 非表示ＦＧ \n"
					= "SELECT 項目ＣＤ,非表示ＦＧ,削除ＦＧ \n"
// MOD 2005.06.10 東都）伊賀　画面制御取得処理変更 END
					+  " FROM ＡＭ０４画面制御 \n"
					+ " WHERE 会員ＣＤ = '"+ sKey1 + "' \n"
					+   " AND 部門ＣＤ = '"+ sKey2 + "' \n"
// DEL 2005.06.10 東都）伊賀　画面制御取得処理変更 START
//					+   " AND 項目ＣＤ = '"+ sKey3 + "' \n"
//					+   " AND 削除ＦＧ = '0' \n"
// DEL 2005.06.10 東都）伊賀　画面制御取得処理変更 END
// ADD 2009.01.30 東都）高木 実績一覧印刷オプション項目の追加 START
					+ " ORDER BY LENGTH(TRIM(項目ＣＤ)), 項目ＣＤ \n"
// ADD 2009.01.30 東都）高木 実績一覧印刷オプション項目の追加 END
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

// MOD 2005.06.10 東都）伊賀　画面制御取得処理変更 START
				while (reader.Read())
				{
					int i項目 = int.Parse(reader.GetString(0).Trim());
// ADD 2009.01.30 東都）高木 実績一覧印刷オプション項目の追加 START
					//配列数を超える項目番号の場合にはパスする
					if(i項目 >= sRet.Length) continue;
// ADD 2009.01.30 東都）高木 実績一覧印刷オプション項目の追加 END
					if (reader.GetString(2).Trim().Equals("0"))
					{
						sRet[i項目] = reader.GetString(1).Trim();
					}
					else
					{
						sRet[i項目] = "0";
					}
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				sRet[0] = "正常終了";
// MOD 2005.06.10 東都）伊賀　画面制御取得処理変更 END

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}
// ADD 2005.05.18 東都）小童谷 画面制御の取得 END
// MOD 2011.05.09 東都）高木 お客様毎の重量入力不可対応 START
		[WebMethod]
		public String[] Get_seigyo3(string[] sUser, string sKey1, string sKey2)
		{
			logWriter(sUser, INF, "画面制御取得３開始");

			OracleConnection conn2 = null;
			String[] sRet = new string[]{"",""};

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			string sQuery  = "";
			try
			{
				sQuery
					= "SELECT 保留印刷ＦＧ \n"
					+  " FROM ＣＭ０１会員 CM01 \n"
					+ " WHERE CM01.会員ＣＤ = '"+ sKey1 + "' \n"
					;
				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);
				if(reader.Read()){
					sRet[1] = reader.GetString(0).TrimEnd();
				}
				disposeReader(reader);
				reader = null;

				sRet[0] = "正常終了";
			}catch (OracleException ex){
				sRet[0] = chgDBErrMsg(sUser, ex);
			}catch (Exception ex){
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}finally{
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			if(sRet[1].Length == 0) sRet[1] = "9";

			return sRet;
		}
// MOD 2011.05.09 東都）高木 お客様毎の重量入力不可対応 END

// ADD 2005.05.20 東都）小童谷 画面制御の登録 START
		/*********************************************************************
		 * 画面制御の登録
		 * 引数：会員ＣＤ、部門ＣＤ、項目ＣＤ、項目名、非表示ＦＧ..
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String Ins_seigyo(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "画面制御登録開始");

			OracleConnection conn2 = null;
			string sRet = "";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet = userCheck2(conn2, sUser);
//			if(sRet.Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 東都）高木 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery 
					= "DELETE FROM ＡＭ０４画面制御 \n"
					+ " WHERE 会員ＣＤ           = '" + sKey[0] +"' \n"
					+ "   AND 部門ＣＤ           = '" + sKey[1] +"' \n"
					+ "   AND 項目ＣＤ           = '" + sKey[2] +"' \n"
					+ "   AND 削除ＦＧ           = '1'";

				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);

				cmdQuery 
					= "INSERT INTO ＡＭ０４画面制御 \n"
					+ "VALUES ('" + sKey[0] +"', \n"
					+ "        '" + sKey[1] +"', \n"
					+ "        '" + sKey[2] +"', \n"
					+ "        '" + sKey[3] +"', \n"
					+ "        '" + sKey[4] +"', \n"
					+ "        '0', \n"
					+ "        TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
					+ "        '" + sKey[5] +"', \n"
					+ "        '" + sKey[6] +"', \n"
					+ "        TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
					+ "        '" + sKey[5] +"', \n"
					+ "        '" + sKey[6] +"')";

				iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
				tran.Commit();
				sRet = "正常終了";
				
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
// DEL 2005.05.31 東都）高木 不要な為削除 START
//				string sErr = ex.Message.Substring(0,9);
//				if(sErr == "ORA-00001")
//					sRet = "同一のコードが既に他の端末より登録されています。\r\n再度、最新データを呼び出して更新してください。";
//				else
// DEL 2005.05.31 東都）高木 不要な為削除 END
				sRet = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.05.20 東都）小童谷 画面制御の登録 END


// ADD 2005.05.20 東都）小童谷 画面制御の更新 START
		/*********************************************************************
		 * 画面制御の更新
		 * 引数：会員ＣＤ、部門ＣＤ、項目ＣＤ、項目名、非表示ＦＧ..
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String Upd_seigyo(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "画面制御マスタの更新開始");

			OracleConnection conn2 = null;
			String sRet = "";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.24 東都）高木 会員チェック追加 START
//			// 会員チェック
//			sRet = userCheck2(conn2, sUser);
//			if(sRet.Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 東都）高木 会員チェック追加 END

			string sQuery    = "";
			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				// 画面制御マスタの更新
				sQuery
					= "UPDATE ＡＭ０４画面制御 \n"
					+    "SET 非表示ＦＧ     = '" + sKey[4] + "', \n"
					+        "削除ＦＧ       = '0', \n"
					+        "更新日時       = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
					+        "更新ＰＧ       = '" + sKey[5] + "', \n"
					+        "更新者         = '" + sKey[6] + "'  \n"
					+  "WHERE 会員ＣＤ       = '" + sKey[0] + "'  \n"
					+  "  AND 部門ＣＤ       = '" + sKey[1] + "'  \n"
					+  "  AND 項目ＣＤ       = '" + sKey[2] + "'  "
					;

				CmdUpdate(sUser, conn2, sQuery);

				tran.Commit();
				sRet = "正常終了";

				logWriter(sUser, INF, sRet);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}
// ADD 2005.05.20 東都）小童谷 画面制御の更新 END

// ADD 2005.05.18 東都）高木 アプリの起動の高速化 START
		/*********************************************************************
		 * ログイン認証２
		 * 引数：端末ＣＤ、利用ＣＤ、パスワード
		 * 戻値：ステータス、会員ＣＤ、会員名、メッセージ、利用者ＣＤ、利用者名、メッセージ
		 * 　　　プリンタＦＧ、プリンタ識別子
		 *********************************************************************/
		[WebMethod]
		public string[] login2(string[] sUser, string[] sKey)
		{
			// 会員ＣＤの取得
// MOD 2008.06.17 東都）高木 ＭＡＣアドレスチェックの追加 START
//			string[] sRet1 = Get_tanmatsu2(sUser, sKey[0]);
			string[] sRet1 = Get_tanmatsu3(sUser, sKey);
// MOD 2008.06.17 東都）高木 ＭＡＣアドレスチェックの追加 END
			if(sRet1[0].Length != 4)
				return sRet1;

			sKey[0] = sRet1[1];
			// ログイン認証
			string[] sRet = login(sUser, sKey);
			if(sRet[0].Length != 4)
				return sRet;

// MOD 2005.06.07 東都）高木 都道府県選択の変更 START
//			sRet.CopyTo(sRet = new string[6 + 2], 0);
			sRet.CopyTo(sRet = new string[6 + 3], 0);
// MOD 2005.06.07 東都）高木 都道府県選択の変更 END
			sRet[6] = sRet1[2];	// プリンタＦＧ
			sRet[7] = sRet1[3];	// プリンタ識別子
// ADD 2005.06.07 東都）高木 都道府県選択の変更 START
			sRet[8] = sRet1[4];	// 都道府県ＣＤ
// ADD 2005.06.07 東都）高木 都道府県選択の変更 END

			return sRet;
		}
// ADD 2005.05.18 東都）高木 アプリの起動の高速化 END

// ADD 2005.06.30 東都）小童谷 終了時に端末マスタ更新 START
		/*********************************************************************
		 * 端末マスタ更新
		 * 引数：端末ＣＤ、会員ＣＤ、利用ＣＤ
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Upd_tanmatu(string[] sUser, string sKey)
		{
// MOD 2007.02.08 東都）高木 クライアントアプリの高速化 START
//			logFileOpen(sUser);
//			logWriter(sUser, INF, "端末マスタ更新開始");
//
//			OracleConnection conn2 = null;
//			string[] sRet = new string[1];
//
//			// ＤＢ接続
//			conn2 = connect2(sUser);
//			if(conn2 == null)
//			{
//				logFileClose();
//				sRet[0] = "ＤＢ接続エラー";
//				return sRet;
//			}
//
//			string sQuery = "";
//
//			OracleTransaction tran = conn2.BeginTransaction();
//			try
//			{
//				// 端末マスタの更新
//				sQuery
//					= "UPDATE ＣＭ０３端末 \n"
//					+   " SET 起動状態 = '2', \n"
//					+       " 実行画面 = 'メニュー', \n"
//					+       " 実行コマンド = '終了', \n"
//					+       " 実行日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
//					+       " 更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
//					+       " 更新ＰＧ = 'メニュー', \n"
//					+       " 更新者   = '" + sKey + "' \n"
//					+ " WHERE 端末ＣＤ = '" + sUser[2]  + "' \n"
//					;
//				CmdUpdate(sUser, conn2, sQuery);
//
//				tran.Commit();
//				sRet[0] = "正常終了";
//				logWriter(sUser, INF, sRet[0]);
//			}
//			catch (OracleException ex)
//			{
//				tran.Rollback();
//				sRet[0] = chgDBErrMsg(sUser, ex);
//			}
//			catch (Exception ex)
//			{
//				tran.Rollback();
//				sRet[0] = "サーバエラー：" + ex.Message;
//				logWriter(sUser, ERR, sRet[0]);
//			}
//			finally
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//			}
//
//			return sRet;
			return new string[1]{"正常終了"};
// MOD 2007.02.08 東都）高木 クライアントアプリの高速化 END
		}
// ADD 2005.06.30 東都）小童谷 終了時に端末マスタ更新 END

// ADD 2006.12.22 東都）小童谷 お届先全件削除用ログイン認証 START
		/*********************************************************************
		 * お届先全件削除用ログイン認証
		 * 引数：会員ＣＤ、利用ＣＤ、パスワード
		 * 戻値：ステータス、会員ＣＤ、会員名、メッセージ、利用者ＣＤ、利用者名、メッセージ
		 *********************************************************************/
		private static string LOGIN_SELECT3
			= "SELECT CM04.部門ＣＤ, \n"
			+       " CM02.部門名, \n"
			+       " CM04.\"認証エラー回数\", \n"
			+       " CM01.使用開始日, \n"
			+       " CM01.使用終了日, \n"
			+       " SYSDATE \n"
			+  " FROM ＣＭ０１会員   CM01, \n"
			+       " ＣＭ０２部門   CM02, \n"
			+       " ＣＭ０４利用者 CM04  \n";

		private static string LOGIN_SELECT_WHERE3
			=    " AND CM04.削除ＦＧ = '0' \n"
			+    " AND CM04.会員ＣＤ = CM01.会員ＣＤ \n"
			+    " AND CM01.削除ＦＧ = '0' \n"
			+    " AND CM04.会員ＣＤ = CM02.会員ＣＤ \n"
			+    " AND CM04.部門ＣＤ = CM02.部門ＣＤ \n"
			+    " AND CM02.削除ＦＧ = '0' \n";

		[WebMethod]
		public string[] login3(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "ログイン認証開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			string sQuery = "";
			string s会員ＣＤ = sKey[0];
			string s利用ＣＤ = sKey[1];
			string sパスワド = sKey[2];
			string s部門ＣＤ = sKey[3];
			int i認証エラー回数 = 0;

			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				sQuery
					= LOGIN_SELECT3
					+ " WHERE CM04.会員ＣＤ = '" + s会員ＣＤ + "' \n"
					+   " AND CM04.利用者ＣＤ = '" + s利用ＣＤ + "' \n"
					+   " AND CM04.パスワード = '" + sパスワド + "' \n"
					+ LOGIN_SELECT_WHERE3
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					i認証エラー回数 = int.Parse(reader.GetDecimal(2).ToString());
					string s使用開始日 = reader.GetString(3).Trim();
					int    i使用開始日 = int.Parse(s使用開始日);
					int    i使用終了日 = int.Parse(reader.GetString(4).Trim());
					int    i当日       = int.Parse(reader.GetDateTime(5).ToString("yyyyMMdd").Trim());
					if (i当日 < i使用開始日)
					{
						if(s使用開始日.Length == 8)
						{
							string s年 = s使用開始日.Substring(0,4);
							string s月 = s使用開始日.Substring(4,2);
							string s日 = s使用開始日.Substring(6,2);
							if(s月[0] == '0') s月 = s月.Substring(1,1);
							if(s日[0] == '0') s日 = s日.Substring(1,1);
							sRet[0] = s年 + "年" + s月 + "月"
									+ s日 + "日より使用できます";
						}
						else
						{
							sRet[0] = "使用開始日より使用できます";
						}
						tran.Commit();
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
						return sRet;
					}
					if (i当日 > i使用終了日)
					{
						sRet[0] = "使用期限が切れています";
						tran.Commit();
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
						return sRet;
					}
// ADD 2008.05.21 東都）高木 ログインエラー回数を５回にする START
//					if (i認証エラー回数 >= 10)
					if (i認証エラー回数 >= 5)
// ADD 2008.05.21 東都）高木 ログインエラー回数を５回にする END
					{
						sRet[0] = "御客様のＩＤは、利用制限がされています　\n"
// MOD 2009.09.14 東都）高木 パスワードエラー時の問い合わせ先の変更 START
//								+ "最寄の営業所まで御連絡下さい";
								+ "ＱＡセンターまたは最寄の営業所まで御連絡下さい";
// MOD 2009.09.14 東都）高木 パスワードエラー時の問い合わせ先の変更 END
						//Session.Clear();
					}
					else if(reader.GetString(0).Trim() == s部門ＣＤ)
					{
						sRet[0] = "正常終了";
					}
					else
					{
						sRet[0] = "入力された利用者では、このセクションの　\n"
								+ "お届先は削除できません";
					}
				}
				else
				{
					sRet[0] = "利用者ＣＤ もしくは パスワード に誤りがあります";
					//Session.Clear();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				// 利用者マスタを更新する
				if(sRet[0].Length != 4 || i認証エラー回数 != 0)
				{
					if (sRet[0].Length == 4){
						i認証エラー回数 = 0;
					}else if(i認証エラー回数 < 90){
						i認証エラー回数++;
					}else{
						i認証エラー回数 = 90;
					}

					// 利用者マスタの更新
					sQuery
						= "UPDATE ＣＭ０４利用者 \n"
						+   " SET 認証エラー回数 = " + i認証エラー回数 + ", \n"
						+       " 更新日時   = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+       " 更新ＰＧ   = '" + "ログイン" + "', \n"
						+       " 更新者     = '" + s利用ＣＤ + "' \n"
						+ " WHERE 会員ＣＤ   = '" + s会員ＣＤ + "' \n"
						+   " AND 利用者ＣＤ = '" + s利用ＣＤ + "' \n"
						;
					CmdUpdate(sUser, conn2, sQuery);
				}

				tran.Commit();

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}
// ADD 2006.12.22 東都）小童谷 お届先全件削除用ログイン END
// ADD 2007.03.28 東都）高木 集荷店取得エラー対応 START
		/*********************************************************************
		 * 発店取得
		 * 引数：荷送人ＣＤ
		 * 戻値：ステータス、店所ＣＤ、店所名、都道府県ＣＤ、市区町村ＣＤ、大字通称ＣＤ
		 *
		 *********************************************************************/
		private static string GET_HATUTEN3_SELECT
			= "SELECT CM14.店所ＣＤ \n"
			+  " FROM ＣＭ０２部門 CM02 \n"
			+      ", ＣＭ１４郵便番号 CM14 \n"
//			+      ", ＣＭ１０店所 CM10 \n"
			;

		[WebMethod]
		public String[] Get_hatuten3(string[] sUser, string sKcode, string sBcode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "発店取得３開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2]{"",""};

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				sbQuery.Append(GET_HATUTEN3_SELECT);
				sbQuery.Append(" WHERE CM02.会員ＣＤ = '" + sKcode + "' \n");
				sbQuery.Append(" AND CM02.部門ＣＤ = '" + sBcode + "' \n");
//				sbQuery.Append(" AND CM02.削除ＦＧ = '0' \n");
				sbQuery.Append(" AND CM02.郵便番号 = CM14.郵便番号 \n");
//				sbQuery.Append(" AND CM14.店所ＣＤ = CM10.店所ＣＤ \n");
//				sbQuery.Append(" AND CM10.削除ＦＧ = '0' \n";);

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				if(reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim();

					sRet[0] = "正常終了";
				}
				else
				{
					sRet[0] = "利用者の集荷店取得に失敗しました";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2007.03.28 東都）高木 集荷店取得エラー対応 END
// ADD 2009.04.02 東都）高木 稼働日対応 START
		/*********************************************************************
		 * 稼働日取得
		 * 引数：ユーザー情報、開始日、終了日（※１ヶ月以内）
		 * 戻値：結果
		 *
		 *********************************************************************/
		private static string GET_KADOBI_SELECT
			= " SELECT CM07.年月日, CM07.稼働日ＦＧ \n"
			+ " FROM ＣＭ０７稼働日 CM07 \n"
			;

		[WebMethod]
		public String[] Get_Kadobi(string[] sUser, string sDateStart, string sDateEnd)
		{
			logWriter(sUser, INF, "稼働日取得開始");
			string[] sRet = new string[3];
			OracleConnection conn2 = null;

			int iCnt;
			for(iCnt = 0; iCnt < sRet.Length; iCnt++){
				sRet[iCnt] = "";
			}

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				OracleDataReader reader;

				sbQuery.Append(GET_KADOBI_SELECT);
				sbQuery.Append(" WHERE CM07.年月日 >= " + sDateStart + " \n");
				sbQuery.Append(  " AND CM07.年月日 <= " + sDateEnd + " \n");
				sbQuery.Append(  " AND CM07.削除ＦＧ = '0' \n");
				sbQuery.Append(" ORDER BY CM07.年月日 \n");

				reader = CmdSelect(sUser, conn2, sbQuery);

				string s開始日 = "";
				string s稼働日ＦＧ   = "";
				string s０７稼働日目 = "";
				string s１４稼働日目 = "";
				iCnt = 1;
				while(reader.Read()){
					if(s開始日.Length == 0){
						s開始日 = reader.GetDecimal(0).ToString();
						if(!s開始日.Equals(sDateStart)) break;
					}
					s稼働日ＦＧ = reader.GetString(1);

					//休日の時はカウントしない
					if(s稼働日ＦＧ.Equals("1")) continue;

					//稼働日とその他はカウントする
					if(iCnt ==  7){
						s０７稼働日目 = reader.GetDecimal(0).ToString();
					}else if(iCnt == 14){
						s１４稼働日目 = reader.GetDecimal(0).ToString();
					}
					iCnt++;
				}

				disposeReader(reader);
				reader = null;
				sRet[0] = "正常終了";
				sRet[1] = s０７稼働日目;
				sRet[2] = s１４稼働日目;
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				sbQuery = null;
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			
			return sRet;
		}
// ADD 2009.04.02 東都）高木 稼働日対応 END
// ADD 2016.05.24 BEVAS）松本 セクション切替画面改修対応 START
		/*********************************************************************
		 * 部門情報取得２
		 * 引数：会員ＣＤ、部門ＣＤ、部門名
		 * 戻値：ステータス、部門ＣＤ、部門名、出荷日、・・・
		 *********************************************************************/
		[WebMethod]
		public string[] Get_bumon2(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "部門情報取得２開始");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[1];
			string sKey会員ＣＤ = sKey[0];
			string sKey部門ＣＤ = sKey[1];
			string sKey部門名 = sKey[2];

			//ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			string cmdQuery  = "";
			try
			{
				//部門情報の取得
				cmdQuery = "SELECT '|' "
						+     " || TRIM(部門ＣＤ) || '|' "
						+     " || TRIM(部門名) || '|' "
						+     " || TRIM(出荷日) || '|' "
						+     " || TRIM(会員ＣＤ) || '|' \n"
						+  "  FROM ＣＭ０２部門 \n"
						+  " WHERE 削除ＦＧ = '0' \n"
						+  "   AND 会員ＣＤ = '" + sKey会員ＣＤ + "' \n";

				//検索条件：部門ＣＤ
				if(sKey[1].Length != 0)
				{
					cmdQuery += "   AND 部門ＣＤ LIKE '" + sKey部門ＣＤ + "%' \n";
				}

				//検索条件：部門名
				if(sKey[2].Length != 0)
				{
					cmdQuery += "   AND 部門名 LIKE '%" + sKey部門名 + "%' \n";
				}

				//ソート順：部門ＣＤ（昇順）、組織ＣＤ（昇順）、出力順（昇順）
				cmdQuery += " ORDER BY 部門ＣＤ, 組織ＣＤ, 出力順 \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				while(reader.Read())
				{
					sList.Add(reader.GetString(0));
				}

				disposeReader(reader);
				reader = null;

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0)
				{
					sRet[0] = "該当データがありません";
				}
				else
				{
					sRet[0] = "正常終了";
					int iCnt = 1;
					IEnumerator enumList = sList.GetEnumerator();
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();
						iCnt++;
					}
				}
				logWriter(sUser, INF, sRet[0]);
			}
			catch(OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch(Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// ADD 2016.05.24 BEVAS）松本 セクション切替画面改修対応 END
	}
}
