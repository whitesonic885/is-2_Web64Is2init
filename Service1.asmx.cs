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
	// �C������
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j��
	//	disposeReader(reader);
	//	reader = null;
	//--------------------------------------------------------------------------
	// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	//--------------------------------------------------------------------------
	// ADD 2007.10.19 ���s�j���� �[���o�[�W�����Ǘ�
	//--------------------------------------------------------------------------
	// ADD 2008.03.21 ���s�j���� ���x���A�b�v���i�Ή�
	// ADD 2008.05.21 ���s�j���� ���O�C���G���[�񐔂��T��ɂ��� 
	// ADD 2008.06.17 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒ǉ� 
	// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� 
	// ADD 2008.07.07 ���s�j���� �l�`�b�A�h���X�擾���s���Ή� 
	// ADD 2008.07.07 ���s�j���� ����̉���ɂ̓��b�Z�[�W��\�� 
	// ADD 2008.07.07 ���s�j���� ���x���A�b�v���i�̃��x���ύX 
	// MOD 2008.07.16 ���s�j���� �l�`�b�A�h���X�̊ȈՃ}�X�L���O 
	// DEL 2008.10.22 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒�~ 
	// MOD 2008.12.22 ���s�j���� �L�������͈̔͂̏C�� 
	// ADD 2008.12.25 ���s�j���� ���O�C�����̃��x���A�b�v���i���b�Z�[�W�̒ǉ� 
	//--------------------------------------------------------------------------
	// MOD 2009.02.12 ���s�j���� �o�[�W������1.9�ȑO�̃��[�U�ł̓��O�C���s�� 
	// ADD 2009.04.02 ���s�j���� �ғ����Ή� 
	// ADD 2009.01.30 ���s�j���� ���шꗗ����I�v�V�������ڂ̒ǉ� 
	// MOD 2009.05.27 ���s�j���� �p�X���[�h�G���[���Ƀp�X���[�h�X�V����\�� 
	// MOD 2009.08.19 ���s�j���� �[���b�c�̘A�Ԍ͊��Ή� 
	// MOD 2009.09.14 ���s�j���� �p�X���[�h�G���[���̖₢���킹��̕ύX 
	// MOD 2009.10.05 ���s�j���� �}�C�i�[�o�[�W�����Q���Ή��iVer.2.10�`�j
	//--------------------------------------------------------------------------
	// MOD 2010.05.21 ���s�j���� �h�o�ԍ������擾�Ή� 
	// MOD 2010.07.29 ���s�j���� ����̉���ɂ̓��b�Z�[�W��\�� 
	//                           [�ې��������@���s�x��]�l
	//--------------------------------------------------------------------------
	// MOD 2011.05.09 ���s�j���� ���q�l���̏d�ʓ��͕s�Ή� 
	//�ۗ� MOD 2011.09.16 ���s�j���� �����於�������ꍇ�̃\�[�g���Ή� 
	// MOD 2011.10.11 ���s�j���� �r�r�k�ؖ���������ԂȂǂ̃`�F�b�N 
	//--------------------------------------------------------------------------
	// MOD 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
	//--------------------------------------------------------------------------
	// MOD 2014.10.16 BEVAS)�O�c �������胍�O�C���΍�
	//--------------------------------------------------------------------------
	// MOD 2016.04.05 BEVAS�j���{ Windows10�Ή�
	//--------------------------------------------------------------------------
	// MOD 2016.05.24 BEVAS�j���{ �Z�N�V�����ؑ։�ʉ��C�Ή�
	//--------------------------------------------------------------------------
	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2init")]

	public class Service1 : is2common.CommService
	{

		public Service1()
		{
			//CODEGEN: ���̌Ăяo���́AASP.NET Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
			InitializeComponent();

			connectService();
		}

		#region �R���|�[�l���g �f�U�C�i�Ő������ꂽ�R�[�h 
		
		//Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
		private IContainer components = null;
				
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
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
		 * �[�����o�^
		 * �����F����b�c�A���p�҂b�c�A�p�X���[�h�A�T�[�}���v�����^�L���A
		 *		 �v�����^��ށA�R���s���[�^���A�h�o�A�h���X�A�l�`�b�A�h���X
		 * �ߒl�F�X�e�[�^�X�A�[���b�c�A������A���p�Җ��A����b�c�A���喼
		 *
		 *********************************************************************/
// ADD 2005.06.02 ���s�j���� ORA-03113�΍�H START
		private static string SET_TANMATSU_SELECT_1
			= "SELECT �����, \n"
			+       " �g�p�J�n��, \n"
			+       " �g�p�I����, \n"
			+       " SYSDATE \n"
			+       " FROM �b�l�O�P��� \n";
		private static string SET_TANMATSU_SELECT_2
			= "SELECT RIY.���p�Җ�, \n"
			+       " RIY.\"�p�X���[�h\", \n"
			+       " RIY.����b�c, \n"
			+       " BUM.���喼 , \n"
			+       " RIY.\"�F�؃G���[��\" \n"
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
			+       ", RIY.�o�^�o�f \n"
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END
			+  " FROM �b�l�O�S���p�� RIY, \n"
			+       " �b�l�O�Q���� BUM \n";
		private static string SET_TANMATSU_SELECT_2_WHERE
			=   " AND RIY.����b�c = BUM.����b�c \n"
			+   " AND RIY.����b�c = BUM.����b�c \n"
// ADD 2005.08.19 ���s�j���� ����폜�̑Ή� START
			+   " AND BUM.�폜�e�f = '0' \n"
// ADD 2005.08.19 ���s�j���� ����폜�̑Ή� END
			+   " AND RIY.�폜�e�f = '0' \n";
// MOD 2009.08.19 ���s�j���� �[���b�c�̘A�Ԍ͊��Ή� START
//		private static string SET_TANMATSU_SELECT_3
//			= "SELECT �r�p�O�P�[���b�c.nextval \n"
//			+  " FROM DUAL \n";
		private static string SET_TANMATSU_SELECT_3
			= "SELECT �r�p�O�Q�[���b�c.nextval \n"
			+  " FROM DUAL \n";
// MOD 2009.08.19 ���s�j���� �[���b�c�̘A�Ԍ͊��Ή� END
// ADD 2005.06.02 ���s�j���� ORA-03113�΍�H END
		[WebMethod]
		public String[] Set_tanmatsu(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�[�����o�^�J�n");

// MOD 2014.10.16 BEVAS)�O�c ����IP����̑�������U���΍� START
			// �N���C�A���g����IP�A�h���X�̎擾
			
// MOD 2014.10.16 BEVAS)�O�c ����IP����̑�������U���΍� END

			OracleConnection conn2 = null;
			String[] sRet = new string[8];

// MOD 2009.02.12 ���s�j���� �o�[�W������1.9�ȑO�̃��[�U�ł̓��O�C���s�� START
			if(sUser.Length < 4)
			{
						// �P�Q�R�S�T�U�V�W�X���P�Q�R�S�T�U�V�W�X���P�Q�R�S�T�U�V�W�X��
				sRet[0] = "���q�l�ɂ����p���������Ă���A�v���P�[�V�������ŐV�̂��̂ɍX�V����K�v������܂��B�@\n"
						+ "�o�[�W�����A�b�v�̍�Ƃ����肢�v���܂��B�@\n"
						+ "�ڂ����́A���R�ʉ^�̂��r�s�`�q�|�Q�_�E�����[�h��ʂɂ���m�ăZ�b�g�A�b�v�菇���n�������������B�@\n"
						;
				sRet[1] = " ";
				return sRet;
			}
// MOD 2009.02.12 ���s�j���� �o�[�W������1.9�ȑO�̃��[�U�ł̓��O�C���s�� END

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			string sQuery    = "";
			string s����b�c = sKey[0];
			string s�����   = "";
			string s���p�b�c = sKey[1];
			string s���p�Җ� = "";
			string s�p�X     = "";
			string s����b�c = "";
			string s���喼   = "";
			string s�[���b�c = "";
			string s�F�؃G���[�� = "0";
// DEL 2005.06.02 ���s�j���� ORA-03113�΍�H START
//			string s���b�Z�[�W = "";
// DEL 2005.06.02 ���s�j���� ORA-03113�΍�H END
			int i�F�؃G���[�� = 0;

			try
			{
				sQuery
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//					= "SELECT �����, \"���b�Z�[�W\" \n"
//					+  " FROM �b�l�O�P��� \n"
//					+ " WHERE ����b�c = '"+ s����b�c + "' \n"
//					+   " AND TO_CHAR(SYSDATE,'YYYYMMDD') BETWEEN �g�p�J�n�� AND �g�p�I���� \n"
//					+   " AND �폜�e�f = '0' \n"
					= SET_TANMATSU_SELECT_1
					+ " WHERE ����b�c = '"+ s����b�c + "' \n"
					+   " AND �폜�e�f = '0' \n"
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
				int    i����       = 0;
				int    i�p�X�X�V�� = 0;
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END
				if (reader.Read())
				{
					s�����     = reader.GetString(0).Trim();
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//					s���b�Z�[�W = reader.GetString(1).Trim();
					string s�g�p�J�n�� = reader.GetString(1).Trim();
					int    i�g�p�J�n�� = int.Parse(s�g�p�J�n��);
					int    i�g�p�I���� = int.Parse(reader.GetString(2).Trim());
// MOD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
//					int    i����       = int.Parse(reader.GetDateTime(3).ToString("yyyyMMdd").Trim());
					       i����       = int.Parse(reader.GetDateTime(3).ToString("yyyyMMdd").Trim());
// MOD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
					if (i���� < i�g�p�J�n��)
					{
						if(s�g�p�J�n��.Length == 8)
						{
							string s�N = s�g�p�J�n��.Substring(0,4);
							string s�� = s�g�p�J�n��.Substring(4,2);
							string s�� = s�g�p�J�n��.Substring(6,2);
							if(s��[0] == '0') s�� = s��.Substring(1,1);
							if(s��[0] == '0') s�� = s��.Substring(1,1);
							sRet[0] = s�N + "�N" + s�� + "��"
									+ s�� + "�����g�p�ł��܂�";
						}
						else
						{
							sRet[0] = "�g�p�J�n�����g�p�ł��܂�";
						}
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
						return sRet;
					}
					if (i���� > i�g�p�I����)
					{
						sRet[0] = "�g�p�������؂�Ă��܂�";
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
						return sRet;
					}
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
// ADD 2007.04.26 ���s�j���� �s�v�ȃR�}���h�̍폜 START
				}else{
					sRet[0] = "����b�c�A���p�҂b�c �������� �p�X���[�h �Ɍ�肪����܂�";
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
					return sRet;
// ADD 2007.04.26 ���s�j���� �s�v�ȃR�}���h�̍폜 END
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				// ���p�ҏ��̎擾
				sQuery
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//					= "SELECT RIY.���p�Җ�, RIY.\"�p�X���[�h\", RIY.����b�c, \n"
//					+       " BUM.���喼 , \n"
//					+       " \"�F�؃G���[��\" \n"
//					+  " FROM �b�l�O�S���p�� RIY, �b�l�O�Q���� BUM \n"
					= SET_TANMATSU_SELECT_2
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
					+ " WHERE RIY.����b�c   = '"+ s����b�c + "' \n"
					+   " AND RIY.���p�҂b�c = '"+ s���p�b�c + "' \n"
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//					+   " AND RIY.����b�c   =  BUM.����b�c \n"
//					+   " AND RIY.����b�c   =  BUM.����b�c \n"
//					+   " AND RIY.�폜�e�f   = '0' \n"
					+ SET_TANMATSU_SELECT_2_WHERE
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
					;

				reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					s���p�Җ� = reader.GetString(0).Trim();
					s�p�X     = reader.GetString(1).Trim();
					s����b�c = reader.GetString(2).Trim();
					s���喼   = reader.GetString(3).Trim();
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//					s�F�؃G���[�� = reader.GetString(4);
					s�F�؃G���[�� = reader.GetDecimal(4).ToString().Trim();
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
					i�F�؃G���[�� = int.Parse(s�F�؃G���[��);
					sRet[0]   = "����I��";
					sRet[6]   = s�F�؃G���[��;
// ADD 2008.05.21 ���s�j���� ���O�C���G���[�񐔂��T��ɂ��� START
//					if (i�F�؃G���[�� >= 10)
					if (i�F�؃G���[�� >= 5)
// ADD 2008.05.21 ���s�j���� ���O�C���G���[�񐔂��T��ɂ��� END
					{
// MOD 2005.08.19 ���s�j���� ���b�Z�[�W�̕ύX START
//						sRet[0] = "�F�؃G���[�F�Ŋ�̉c�Ə��܂Ō�A��������";
						sRet[0] = "��q�l�̂h�c�́A���p����������Ă��܂��@\n"
								+ "�Ŋ�̉c�Ə��܂Ō�A��������";
// MOD 2005.08.19 ���s�j���� ���b�Z�[�W�̕ύX END
					}
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
					try{
						i�p�X�X�V�� = int.Parse(reader.GetString(5).Trim());
					}catch (Exception ){
						;
					}
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END
// ADD 2007.04.26 ���s�j���� �s�v�ȃR�}���h�̍폜 START
				}else{
					sRet[0] = "���p�҂b�c �������� �p�X���[�h �Ɍ�肪����܂�";
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
					return sRet;
// ADD 2007.04.26 ���s�j���� �s�v�ȃR�}���h�̍폜 END
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
				DateTime dt���� = new DateTime(
					  int.Parse(i����.ToString().Substring(0,4))
					, int.Parse(i����.ToString().Substring(4,2))
					, int.Parse(i����.ToString().Substring(6,2))
					);
				DateTime dt�p�X�X�V�� = new DateTime(
					int.Parse(i�p�X�X�V��.ToString().Substring(0,4))
					, int.Parse(i�p�X�X�V��.ToString().Substring(4,2))
					, int.Parse(i�p�X�X�V��.ToString().Substring(6,2))
					);
				DateTime dt�p�X�L������ = dt�p�X�X�V��.AddMonths(6);
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END

				// �p�X���[�h�̃`�F�b�N
				if(s�p�X != sKey[2])
				{
// MOD 2009.05.27 ���s�j���� �p�X���[�h�G���[���Ƀp�X���[�h�X�V����\�� START
//					sRet[0] = "���p�҂b�c �������� �p�X���[�h �Ɍ�肪����܂�";
					sRet[0] = "���p�҂b�c �������� �p�X���[�h �Ɍ�肪����܂�\n"
							+ "�@�@�@�@�@�i"
							+ int.Parse(i�p�X�X�V��.ToString().Substring(4,2)) + "/"
							+ int.Parse(i�p�X�X�V��.ToString().Substring(6,2))
							+ " �ɕύX����Ă��܂��j"
							;
// MOD 2009.05.27 ���s�j���� �p�X���[�h�G���[���Ƀp�X���[�h�X�V����\�� END
				}

				OracleTransaction tran;

				// ���p�҃}�X�^���X�V����
				if(sRet[0].Length != 4 || i�F�؃G���[�� != 0)
				{
					// ����I����
					if (sRet[0].Length == 4){
						i�F�؃G���[�� = 0;
					}else if(i�F�؃G���[�� < 90){
						i�F�؃G���[��++;
					}else{
						i�F�؃G���[�� = 90;
					}

//					OracleTransaction tran = conn2.BeginTransaction();
					tran = conn2.BeginTransaction();
					try
					{
						// ���p�҃}�X�^�̍X�V
						sQuery
							= "UPDATE �b�l�O�S���p�� \n"
							+   " SET �F�؃G���[�� = " + i�F�؃G���[�� + ", \n"
							+       " �X�V����       = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
							+       " �X�V�o�f       = '" + "�����o�^" + "', \n"
							+       " �X�V��         = '" + s���p�b�c + "'  \n"
							+ " WHERE ����b�c       = '" + s����b�c + "'  \n"
							+   " AND ���p�҂b�c     = '" + s���p�b�c + "'  \n"
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
						sRet[0] = "�T�[�o�G���[�F" + ex.Message;
						logWriter(sUser, ERR, sRet[0]);
					}
					
					// ����I���ł͂Ȃ��ꍇ�ɂ͏I������
					if(sRet[0].Length != 4)
					{
// DEL 2007.04.26 ���s�j���� �s�v�ȃR�}���h�̍폜 START
// finally���Ŏ��s�����וs�v
//						disconnect2(sUser, conn2);
//						logFileClose();
// DEL 2007.04.26 ���s�j���� �s�v�ȃR�}���h�̍폜 END

						return sRet;
					}
				}

// ADD 2007.02.13 ���s�j���� �[���h�c�Ď擾�Ή� START
				bool b���[���b�c = false;
// ADD 2008.07.16 ���s�j���� �l�`�b�A�h���X�̊ȈՃ}�X�L���O START
				if(sKey[7].Length == 17 && sKey[7].Substring(0,1).Equals("Z")){
					sKey[7] = sKey[7].Substring( 1,2) + "-"
							+ sKey[7].Substring( 4,2) + "-"
							+ sKey[7].Substring( 7,2) + "-"
							+ sKey[7].Substring(10,2) + "-"
							+ sKey[7].Substring(13,2) + "-"
							+ sKey[7].Substring(15,2)
							;
				}
// ADD 2008.07.16 ���s�j���� �l�`�b�A�h���X�̊ȈՃ}�X�L���O END
// MOD 2010.05.21 ���s�j���� �h�o�ԍ������擾�Ή� START
				if(sKey[5].Length == 0) sKey[5] = " "; // �}�V����
				if(sKey[6].Length == 0) sKey[6] = " "; // �h�o
// MOD 2010.05.21 ���s�j���� �h�o�ԍ������擾�Ή� END
// ADD 2008.07.07 ���s�j���� �l�`�b�A�h���X�擾���s���Ή� START
//				if(sKey[7].Length == 0) sKey[7] = sKey[5];
				if(sKey[7].Length == 0) sKey[7] = "-";
// ADD 2008.07.07 ���s�j���� �l�`�b�A�h���X�擾���s���Ή� END
				// �ȑO�o�^����Ă���[�������擾����(����b�c�A�}�V�����A�l�`�b)
				sQuery = "SELECT NVL(MAX(�[���b�c), ' ') \n"
						+ " FROM �b�l�O�R�[�� \n";
				sQuery += " WHERE ����b�c = '" + s����b�c + "' \n";
				sQuery += " AND �}�V���� = '" + sKey[5] + "' \n";
				sQuery += " AND �l�`�b = '" + sKey[7] + "' \n";

				reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					s�[���b�c = reader.GetString(0).Trim();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				if(s�[���b�c.Length > 0){
					b���[���b�c = true;
				}else{
					b���[���b�c = false;
// ADD 2007.02.13 ���s�j���� �[���h�c�Ď擾�Ή� END

					// �[���b�c�̐V�K�擾
					sQuery
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//						= "SELECT TO_CHAR(�r�p�O�P�[���b�c.nextval) FROM DUAL";
						= SET_TANMATSU_SELECT_3;
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
					reader = CmdSelect(sUser, conn2, sQuery);

					if (reader.Read())
					{
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//						s�[���b�c = "IS2" + reader.GetString(0).Trim();
// MOD 2009.08.19 ���s�j���� �[���b�c�̘A�Ԍ͊��Ή� START
//						s�[���b�c = "IS2" + reader.GetDecimal(0).ToString().Trim();
						s�[���b�c = "IS" + reader.GetDecimal(0).ToString().Trim();
// MOD 2009.08.19 ���s�j���� �[���b�c�̘A�Ԍ͊��Ή� END
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
					}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

// ADD 2007.02.13 ���s�j���� �[���h�c�Ď擾�Ή� START
				}
// ADD 2007.02.13 ���s�j���� �[���h�c�Ď擾�Ή� END

//				OracleTransaction tran = conn2.BeginTransaction();
				tran = conn2.BeginTransaction();
				string sUserHostName = "";
				if(this.Context.Request.UserHostName.Length > 30)
					sUserHostName = this.Context.Request.UserHostName.Substring(0,30);
				else
					sUserHostName = this.Context.Request.UserHostName;

				try
				{
// ADD 2007.02.13 ���s�j���� �[���h�c�Ď擾�Ή� START
					if(b���[���b�c){
						// �[���}�X�^�̍X�V
						sQuery
							= "UPDATE �b�l�O�R�[�� \n"
							+ "SET �v�����^�e�f = '"+ sKey[3] +"' \n"
							+ ", �v�����^���ʎq = '"+ sKey[4] +"' \n"
							+ ", �h���C�� = '" + sUserHostName + "' \n"
							+ ", �h�o = '" + sKey[6] + "' \n"
							+ ", �폜�e�f = '0' \n"
							+ ", �X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
							+ ", �X�V�o�f = '�����o�^' \n"
							+ ", �X�V�� = '" + s���p�b�c + "' \n"
							;
						sQuery += " WHERE �[���b�c = '" + s�[���b�c + "' \n";
					}else{
// ADD 2007.02.13 ���s�j���� �[���h�c�Ď擾�Ή� END
						// �[���}�X�^�̓o�^
						sQuery
							= "INSERT INTO �b�l�O�R�[�� \n"
							+ "VALUES ( \n"
							+ "'" + s�[���b�c + "', '" + s����b�c + "', \n"	// �[���b�c, ����b�c,
							+ "'" + sKey[3] + "', '" + sKey[4] + "', \n"		// �v�����^�e�f, �v�����^���ʎq,
							+ "TO_CHAR(SYSDATE,'YYYYMMDD'), \n"					// �g�p�J�n��,
							+ "'"+ sUserHostName +"', \n"	// �h���C����
							+ "'"+ sKey[5] + "', '" + sKey[6] + "', '" + sKey[7] + "', \n"			// �}�V����, �h�o, �l�`�b, 
							+ "'1', '�����o�^', '�X�V', TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"	// �N�����, ���s���, ���s�R�}���h, ���s����,
// ADD 2005.06.07 ���s�j���� �s���{���I���̕ύX START
							+ "'0',"						// �s���{���b�c
// ADD 2005.06.07 ���s�j���� �s���{���I���̕ύX END
							+ "'0', \n" // �폜�e�f, 
							+ "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), '�����o�^', '"+ s���p�b�c +"', \n" // �o�^����, �o�^�o�f, �o�^��,
							+ "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), '�����o�^', '"+ s���p�b�c +"'  \n" // �X�V����, �X�V�o�f, �X�V��,
							+ ")  "
							;
// ADD 2007.02.13 ���s�j���� �[���h�c�Ď擾�Ή� START
					}
// ADD 2007.02.13 ���s�j���� �[���h�c�Ď擾�Ή� END

					CmdUpdate(sUser, conn2, sQuery);

					tran.Commit();
					sRet[0] = "����I��";
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
					//�U�����ȏ�o�߂������_�ŁA�p�X���[�h�X�V�x���\��
					if(dt����.CompareTo(dt�p�X�L������) > 0){
						sRet[0] = "�����؂�";
					//�T�����ȏ�o�߂������_�ŁA�p�X���[�h�X�V�x���\��
// MOD 2008.12.22 ���s�j���� �L�������͈̔͂̏C�� START
//					}else if(dt����.CompareTo(dt�p�X�X�V��.AddMonths(5)) >= 0){
					}else if(dt����.CompareTo(dt�p�X�X�V��.AddMonths(5)) > 0){
// MOD 2008.12.22 ���s�j���� �L�������͈̔͂̏C�� END
						sRet[0] = dt�p�X�L������.ToString("MMdd").Trim();
					}
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END


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
					sRet[0] = "�T�[�o�G���[�F" + ex.Message;
					logWriter(sUser, ERR, sRet[0]);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			sRet[1] = s�[���b�c;
			sRet[2] = s�����;
			sRet[3] = s���p�Җ�;
			sRet[4] = s����b�c;
			sRet[5] = s���喼;
			sRet[6] = s�F�؃G���[��;
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//			sRet[7] = s���b�Z�[�W;
			sRet[7] = "";
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
			return sRet;
		}

		/*********************************************************************
		 * �[�����X�V
		 * �����F�[���b�c�A�T�[�}���v�����^�L���A�v�����^��ށA�o�f�h�c�A���p�҂b�c
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String Upd_tanmatsu(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�[���}�X�^�̍X�V�J�n");

			OracleConnection conn2 = null;
			String sRet = "";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet = userCheck2(conn2, sUser);
//			if(sRet.Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			string sQuery    = "";
			string s�[���b�c = sKey[0];
			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				// �[���}�X�^�̍X�V
				sQuery
					= "UPDATE �b�l�O�R�[�� \n"
					+   " SET �v�����^�e�f   = '" + sKey[1] + "', \n"
					+       " �v�����^���ʎq = '" + sKey[2] + "', \n"
// DEL 2007.02.08 ���s�j���� �N���C�A���g�A�v���̍����� START
//					+       " �N����� = '1', \n"
//					+       " ���s��� = '" + sKey[3] + "', \n"
//					+       " ���s�R�}���h = '�X�V', \n"
//					+       " ���s���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
// DEL 2007.02.08 ���s�j���� �N���C�A���g�A�v���̍����� END
					+       " �X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
					+       " �X�V�o�f = '" + sKey[3] + "', \n"
					+       " �X�V��   = '" + sKey[4] + "'  \n"
					+ " WHERE �[���b�c = '" + s�[���b�c + "' \n"
					;

				CmdUpdate(sUser, conn2, sQuery);

				tran.Commit();
				sRet = "����I��";

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
				sRet = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}

// ADD 2007.10.19 ���s�j���� �[���o�[�W�����Ǘ� START
		/*********************************************************************
		 * �o�[�W�������̍X�V�i�[���}�X�^�A����}�X�^�j
		 * �����F����b�c�A�o�f�h�c
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		private String Upd_bumon_ver(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "�o�[�W�������̍X�V�J�n");

			if(sUser.Length < 4) return "";

			OracleConnection conn2 = null;
			String sRet = "";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet = "�c�a�ڑ��G���[";
				return sRet;
			}

			string sQuery = "";
			string sVer   = sUser[3] + "    ";
			sVer = sVer.Substring(0,4);
			string sVer00 = "";
			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				// �[���}�X�^�̌���
				sQuery
					= "SELECT ���s��� FROM �b�l�O�R�[�� \n"
					+ " WHERE �[���b�c = '" + sUser[2] + "' \n"
					;
				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);
				if (reader.Read())
				{
					sVer00 = reader.GetString(0);
					sVer00 = sVer00.Substring(0,4);
				}
				disposeReader(reader);
				reader = null;
				
				//�[���̃o�[�W������񂪈قȂ�ꍇ�̂ݍX�V
				if(sVer != sVer00)
				{
					// �[���}�X�^�̍X�V
					sQuery
						= "UPDATE �b�l�O�R�[�� \n"
//						+   " SET ���s��� = '" + sVer + "' || SUBSTRB(���s���,5), \n"
						+   " SET ���s��� = '" + sVer + "', \n"
						+       " �X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+       " �X�V�o�f = '" + sKey[1] + "', \n"
						+       " �X�V��   = '" + sUser[1] + "'  \n"
						+ " WHERE �[���b�c = '" + sUser[2] + "' \n"
						;
					CmdUpdate(sUser, conn2, sQuery);

					// ����}�X�^�̍X�V
					sQuery
						= "UPDATE �b�l�O�Q���� \n"
						+   " SET �g�D�b�c = '" + sVer + "' || SUBSTRB(�g�D�b�c,5), \n"
						+       " �X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+       " �X�V�o�f = '" + sKey[1] + "', \n"
						+       " �X�V��   = '" + sUser[1] + "'  \n"
						+ " WHERE ����b�c = '" + sUser[0] + "' \n"
						+ " AND ����b�c = '" + sKey[0] + "' \n"
						;
					CmdUpdate(sUser, conn2, sQuery);
				}

				tran.Commit();
				sRet = "����I��";

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
				sRet = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}
// ADD 2007.10.19 ���s�j���� �[���o�[�W�����Ǘ� END

		/*********************************************************************
		 * ���p�҃}�X�^�̍X�V�i�p�X���[�h�j
		 * �����F����b�c�A���p�҂b�c�A�p�X���[�h�A�o�f�h�c�A���p�҂b�c
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String Upd_riyou(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "���p�҃}�X�^�̍X�V�J�n");

			OracleConnection conn2 = null;
			String sRet = "";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet = userCheck2(conn2, sUser);
//			if(sRet.Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			string sQuery = "";
			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
				string s�p�X = "";
				// ���p�҃}�X�^�̍X�V
				sQuery
					= "SELECT \"�p�X���[�h\" \n"
					+   "FROM �b�l�O�S���p�� \n"
					+  "WHERE ����b�c       = '" + sKey[0] + "'  \n"
					+    "AND ���p�҂b�c     = '" + sKey[1] + "'  "
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if(reader.Read()){
					s�p�X     = reader.GetString(0).Trim();
				}
				disposeReader(reader);
				reader = null;

				if(s�p�X.Equals(sKey[2])){
					tran.Commit();
					sRet = "�O��Ɠ����p�X���[�h�ɂ͕ύX�ł��܂���B";
				}else{
					sQuery = "";
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END

					// ���p�҃}�X�^�̍X�V
					sQuery
						= "UPDATE �b�l�O�S���p�� \n"
						+    "SET �p�X���[�h     = '" + sKey[2] + "', \n"
// ADD 2008.05.21 ���s�j���� ���O�C���G���[�񐔂��T��ɂ��� START
						+        "�o�^�o�f       = TO_CHAR(SYSDATE,'YYYYMMDD'), \n"
// ADD 2008.05.21 ���s�j���� ���O�C���G���[�񐔂��T��ɂ��� END
						+        "�X�V����       = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+        "�X�V�o�f       = '" + sKey[3] + "', \n"
						+        "�X�V��         = '" + sKey[4] + "'  \n"
						+  "WHERE ����b�c       = '" + sKey[0] + "'  \n"
						+    "AND ���p�҂b�c     = '" + sKey[1] + "'  "
						;

					CmdUpdate(sUser, conn2, sQuery);

					tran.Commit();
					sRet = "����I��";
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
				}
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END

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
				sRet = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}

		private static string GET_TANMATSU_SELECT_1
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//			= "SELECT TAN.����b�c, TAN.�v�����^�e�f, TAN.�v�����^���ʎq, \n"
//			+       " KAI.����� \n"
//			+  " FROM �b�l�O�R�[�� TAN, �b�l�O�P��� KAI \n";
// MOD 2005.06.07 ���s�j���� �s���{���I���̕ύX START
//			= "SELECT ����b�c, �v�����^�e�f, �v�����^���ʎq \n"
			= "SELECT ����b�c, �v�����^�e�f, �v�����^���ʎq, �s���{���b�c \n"
// ADD 2008.12.25 ���s�j���� ���O�C�����̃��x���A�b�v���i���b�Z�[�W�̒ǉ� START
// DEL 2009.02.12 ���s�j���� �o�[�W������1.9�ȑO�̃��[�U�ł̓��O�C���s�� START
//			+ ", ���s�R�}���h, TO_CHAR(SYSDATE,'YYYYMMDD') \n"
// DEL 2009.02.12 ���s�j���� �o�[�W������1.9�ȑO�̃��[�U�ł̓��O�C���s�� END
// ADD 2008.12.25 ���s�j���� ���O�C�����̃��x���A�b�v���i���b�Z�[�W�̒ǉ� END
// MOD 2005.06.07 ���s�j���� �s���{���I���̕ύX END
// MOD 2011.10.11 ���s�j���� �r�r�k�ؖ���������ԂȂǂ̃`�F�b�N START
			+ ", �N�����, ���s�R�}���h \n"
// MOD 2011.10.11 ���s�j���� �r�r�k�ؖ���������ԂȂǂ̃`�F�b�N END
			+  " FROM �b�l�O�R�[�� \n";
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END

		private static string GET_TANMATSU_SELECT_1_WHERE
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//			=   " AND TAN.�폜�e�f = '0' \n"
//			+   " AND TAN.����b�c = KAI.����b�c \n"
//			+   " AND TO_CHAR(SYSDATE,'YYYYMMDD') BETWEEN KAI.�g�p�J�n�� AND KAI.�g�p�I���� \n"
//			+   " AND KAI.�폜�e�f = '0' \n";
			=   " AND �폜�e�f = '0' \n";
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END

		private static string GET_TANMATSU_SELECT_2
			= "SELECT RIY.���p�Җ�, RIY.����b�c, RIY.�ב��l�b�c, \n"
			+       " BUM.���喼, BUM.�o�ד�, \n"
// ADD 2005.07.21 ���s�j���� �X�����[�U�Ή� START
			+       " RIY.�����P \n"
// ADD 2005.07.21 ���s�j���� �X�����[�U�Ή� END
			+  " FROM �b�l�O�S���p�� RIY, �b�l�O�Q���� BUM \n";

		private static string GET_TANMATSU_SELECT_2_WHERE
			=   " AND RIY.�폜�e�f   = '0' \n"
			+   " AND RIY.����b�c   =  BUM.����b�c \n"
			+   " AND RIY.����b�c   =  BUM.����b�c \n";

		private static string SELECT_COUNT
//			= "SELECT TO_CHAR(COUNT(*)) \n";
//			= "SELECT NVL(COUNT(*),0) \n";
//			= "SELECT COUNT(*) \n";
			= "SELECT COUNT(ROWID) \n";

		private static string SELECT_COUNT_S
			= "SELECT COUNT(S.ROWID) \n";

		/*********************************************************************
		 * �[�����擾�Q
		 * �����F�[���b�c
		 * �ߒl�F�X�e�[�^�X�A����b�c�A�T�[�}���v�����^�L���A�v�����^��ށA�����
		 * 
		 *********************************************************************/
		[WebMethod]
		public String[] Get_tanmatsu2(string[] sUser, string sKey1)
// ADD 2008.06.17 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒ǉ� START
		{
			string[] sKey2 = new string[]{sKey1};
			return Get_tanmatsu3(sUser, sKey2);
		}
		[WebMethod]
		public String[] Get_tanmatsu3(string[] sUser, string[] sKey1)
// ADD 2008.06.17 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒ǉ� END
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�[�����擾�Q�J�n");

			OracleConnection conn2 = null;
			String[] sRet = new string[5];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			string sQuery      = "";
// MOD 2008.06.17 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒ǉ� START
//			string s�[���b�c   = sKey1;
			string s�[���b�c   = sKey1[0];
// MOD 2008.06.17 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒ǉ� END
			string s����b�c   = "";
			string sPrtFg      = "";
			string sPrtKind    = "";
// DEL 2005.06.02 ���s�j���� ORA-03113�΍�H START
//			string s�����     = "";
// DEL 2005.06.02 ���s�j���� ORA-03113�΍�H END
// ADD 2005.06.07 ���s�j���� �s���{���I���̕ύX START
			string s�s���{���b�c = "";
// ADD 2005.06.07 ���s�j���� �s���{���I���̕ύX END
// MOD 2011.10.11 ���s�j���� �r�r�k�ؖ���������ԂȂǂ̃`�F�b�N START
			string s�N�����         = "";
			string s���s�R�}���h     = "";
			string sInitProxyExists  = (sKey1.Length >  6) ? sKey1[ 6] : "";
			string sInitSyukkaExists = (sKey1.Length >  7) ? sKey1[ 7] : "";
			string sOSVer            = (sKey1.Length >  8) ? sKey1[ 8] : "";
			string sNetVer           = (sKey1.Length >  9) ? sKey1[ 9] : "";
			string sSSLStatus        = (sKey1.Length > 10) ? sKey1[10] : "";
// MOD 2011.10.11 ���s�j���� �r�r�k�ؖ���������ԂȂǂ̃`�F�b�N END
			try
			{
				sQuery
					= GET_TANMATSU_SELECT_1
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//					+ " WHERE TAN.�[���b�c = '"+ s�[���b�c + "' "
					+ " WHERE �[���b�c = '"+ s�[���b�c + "' \n"
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
// MOD 2008.06.17 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒ǉ� START
//					+ GET_TANMATSU_SELECT_1_WHERE;
					;

// DEL 2008.10.22 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒�~ START
//				if(sKey1.Length >= 4 && sKey1[3].Length > 0){
//					sQuery += " AND �}�V���� = '"+ sKey1[3] +"' \n";
//				}
// DEL 2008.10.22 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒�~ END

//				if(sKey1.Length >= 5 && sKey1[4].Length > 0){
//					sQuery += " AND �h�o = '"+ sKey1[4] +"' \n";
//				}
// MOD 2008.07.16 ���s�j���� �l�`�b�A�h���X�̊ȈՃ}�X�L���O START
//// ADD 2008.07.07 ���s�j���� �l�`�b�A�h���X�擾���s���Ή� START
//				if(sKey1.Length >= 6 && sKey1[5].Length == 0) sKey1[5] = sKey1[3];
//// ADD 2008.07.07 ���s�j���� �l�`�b�A�h���X�擾���s���Ή� END
//				if(sKey1.Length >= 6 && sKey1[5].Length > 0){
//					sQuery += " AND �l�`�b = '"+ sKey1[5] +"' \n";
//				}
//				sQuery += GET_TANMATSU_SELECT_1_WHERE;
//// MOD 2008.06.17 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒ǉ� END
// DEL 2008.10.22 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒�~ START
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
//						sQuery += " AND �l�`�b = '"+ sKey1[5] +"' \n";
//					}
//				}
// DEL 2008.10.22 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒�~ END
				sQuery += GET_TANMATSU_SELECT_1_WHERE;
// MOD 2008.07.16 ���s�j���� �l�`�b�A�h���X�̊ȈՃ}�X�L���O END

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					s����b�c = reader.GetString(0).Trim();
					sPrtFg    = reader.GetString(1).Trim();
					sPrtKind  = reader.GetString(2).Trim();
// DEL 2005.06.02 ���s�j���� ORA-03113�΍�H START
//					s�����   = reader.GetString(3).Trim();
// DEL 2005.06.02 ���s�j���� ORA-03113�΍�H END
// ADD 2005.06.07 ���s�j���� �s���{���I���̕ύX START
					s�s���{���b�c = reader.GetString(3).Trim();;
// ADD 2005.06.07 ���s�j���� �s���{���I���̕ύX END
// MOD 2011.10.11 ���s�j���� �r�r�k�ؖ���������ԂȂǂ̃`�F�b�N START
					s�N�����     = reader.GetString(4).TrimEnd();
					s���s�R�}���h = reader.GetString(5).TrimEnd();
// MOD 2011.10.11 ���s�j���� �r�r�k�ؖ���������ԂȂǂ̃`�F�b�N END
// MOD 2005.07.01 ���s�j���� �[����񂪍폜����Ă����ꍇ�̑Ή� START
//				}
//
//				sRet[0] = "����I��";
					sRet[0] = "����I��";
// ADD 2008.12.25 ���s�j���� ���O�C�����̃��x���A�b�v���i���b�Z�[�W�̒ǉ� START
					if(sUser.Length < 4)
					{
// MOD 2009.02.12 ���s�j���� �o�[�W������1.9�ȑO�̃��[�U�ł̓��O�C���s�� START
//						// [���s�R�}���h]��[�c�a���t]���قȂ�ꍇ
//						if(!reader.GetString(4).Trim().Equals(reader.GetString(5).Trim()))
//						{
//							disposeReader(reader);
//							reader = null;
//							
//							OracleTransaction tran = conn2.BeginTransaction();
//							try
//							{
//								// ���p�҃}�X�^�̍X�V
//								sQuery
//									= "UPDATE �b�l�O�R�[�� \n"
//									+   " SET ���s�R�}���h = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
//									+ " WHERE �[���b�c = '"+ s�[���b�c + "' \n"
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
//									// �P�Q�R�S�T�U�V�W�X���P�Q�R�S�T�U�V�W�X���P�Q�R�S�T�U�V�W�X��
//							sRet[0] = "���q�l�ɂ����p���������Ă���A�v���P�[�V�������ŐV�̂��̂ɍX�V����K�v������܂��B�@\n"
//									+ "�o�[�W�����A�b�v�̍�Ƃ����肢�v���܂��B�@\n"
//									+ "�ڂ����́A���R�ʉ^�̂��r�s�`�q�|�Q�_�E�����[�h��ʂɂ���m�ăZ�b�g�A�b�v�菇���n�������������B�@\n"
//									;
//						}
								// �P�Q�R�S�T�U�V�W�X���P�Q�R�S�T�U�V�W�X���P�Q�R�S�T�U�V�W�X��
						sRet[0] = "���q�l�ɂ����p���������Ă���A�v���P�[�V�������ŐV�̂��̂ɍX�V����K�v������܂��B�@\n"
								+ "�o�[�W�����A�b�v�̍�Ƃ����肢�v���܂��B�@\n"
								+ "�ڂ����́A���R�ʉ^�̂��r�s�`�q�|�Q�_�E�����[�h��ʂɂ���m�ăZ�b�g�A�b�v�菇���n�������������B�@\n"
								;
// MOD 2009.02.12 ���s�j���� �o�[�W������1.9�ȑO�̃��[�U�ł̓��O�C���s�� END
					}
// ADD 2008.12.25 ���s�j���� ���O�C�����̃��x���A�b�v���i���b�Z�[�W�̒ǉ� END
// MOD 2011.10.11 ���s�j���� �r�r�k�ؖ���������ԂȂǂ̃`�F�b�N START
					if(sRet[0].Length == 4){
						string w�N����� = "";
						if(sSSLStatus.Length > 0){
							w�N����� = sSSLStatus.Substring(0,1);
						}
						string w���s�R�}���h = sOSVer;
						if(sOSVer.StartsWith("4.0.")){
							w���s�R�}���h = "NT4.0" + " " + sOSVer.Substring(4); 
						}else if(sOSVer.StartsWith("4.10.")){
							w���s�R�}���h = "98" + " " + sOSVer.Substring(5);
						}else if(sOSVer.StartsWith("4.90.")){
							w���s�R�}���h = "Me" + " " + sOSVer.Substring(5);
						}else if(sOSVer.StartsWith("5.0.")){
							w���s�R�}���h = "2000" + " " + sOSVer.Substring(4);
						}else if(sOSVer.StartsWith("5.1.")){
							w���s�R�}���h = "XP" + " " + sOSVer.Substring(4);
						}else if(sOSVer.StartsWith("5.2.")){
							w���s�R�}���h = "2003" + " " + sOSVer.Substring(4);
						}else if(sOSVer.StartsWith("6.0.")){
							w���s�R�}���h = "Vista" + " " + sOSVer.Substring(4);
						}else if(sOSVer.StartsWith("6.1.")){
							w���s�R�}���h = "7" + " " + sOSVer.Substring(4);
						}
// MOD 2016.04.05 BEVAS�j���{ Windows10�Ή� START
						//��Win8,Win8.1�̑Ή��R�ꂪ�������ׁAWin10�ƕ����đΉ�
						else if(sOSVer.StartsWith("6.2."))
						{
							//Windows8
							w���s�R�}���h = "8" + " " + sOSVer.Substring(4);
						}
						else if(sOSVer.StartsWith("6.3."))
						{
							//Windows8.1
							w���s�R�}���h = "8.1" + " " + sOSVer.Substring(4);
						}						
						else if(sOSVer.StartsWith("10.0."))
						{
							//Windows10
							w���s�R�}���h = "10" + " " + sOSVer.Substring(4);
						}
// MOD 2016.04.05 BEVAS�j���{ Windows10�Ή� END
						if(w���s�R�}���h.Length > 8){
							w���s�R�}���h = w���s�R�}���h.Substring(0,8);
						}

						if(s�N����� != sSSLStatus || s���s�R�}���h != w�N����� ){
							logWriter(sUser, INF, "�[�����擾�R"
								+ " SSL���["+ sSSLStatus +"]"
								+ " OS["+ w���s�R�}���h +"]["+ sOSVer +"] .NET["+ sNetVer +"]"
								+ " �v���L�V�ݒ�F["+ sInitProxyExists +"]"
								+ " �����o�͐ݒ�F["+ sInitSyukkaExists +"]"
								);

							if(w�N�����.Length == 0) w�N����� = " ";
							if(w���s�R�}���h.Length == 0) w���s�R�}���h = " ";

							OracleTransaction tran = conn2.BeginTransaction();
							try{
								// ���p�҃}�X�^�̍X�V
								sQuery = "UPDATE �b�l�O�R�[�� \n"
									+   "SET �N�����   = '"+ w�N����� +"' \n"
									+   ", ���s�R�}���h = '"+ w���s�R�}���h +"' \n"
									+   ", ���s���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
									+ " WHERE �[���b�c = '"+ s�[���b�c +"' \n"
									;
								CmdUpdate(sUser, conn2, sQuery);
								tran.Commit();
							}catch(OracleException ex){
								logWriter(sUser, ERR, chgDBErrMsg(sUser, ex));
								tran.Rollback();
//								throw ex;
							}catch (Exception ex){
								logWriter(sUser, ERR, "�T�[�o�G���[�F" + ex.Message);
								tran.Rollback();
//								throw ex;
							}
						}
					}
// MOD 2011.10.11 ���s�j���� �r�r�k�ؖ���������ԂȂǂ̃`�F�b�N END
				}
				else
				{
					sRet[0] = "�[�����";
				}
// MOD 2005.07.01 ���s�j���� �[����񂪍폜����Ă����ꍇ�̑Ή� END
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			sRet[1] = s����b�c;
			sRet[2] = sPrtFg;
			sRet[3] = sPrtKind;
// MOD 2005.06.07 ���s�j���� �s���{���I���̕ύX START
//// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
////			sRet[4] = s�����;
//			sRet[4] = "";
//// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
			sRet[4] = s�s���{���b�c;
// MOD 2005.06.07 ���s�j���� �s���{���I���̕ύX END

			return sRet;
		}

		/*********************************************************************
		 * ���p�ҏ��擾
		 * �����F����b�c�A���p�҂b�c
		 * �ߒl�F�X�e�[�^�X�A���p�Җ��A����b�c�A���喼�A�o�ד��A�ב��l�b�c�A
		 *		 ���吔�A���Ӑ搔
		 *		 ���Ӑ�b�c�A���Ӑ敔�ۂb�c�A���Ӑ敔�ۖ�
		 *********************************************************************/
		[WebMethod]
		public String[] Get_riyou(string[] sUser, string sKey1, string sKey2)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "���p�ҏ��擾�J�n");

			OracleConnection conn2 = null;
			String[] sRet = new string[8];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL �F�،�̎��s�Ȃ̂ŕs�v ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				logFileClose();
//				conn.Close();
//				return sRet;
//			}
// DEL �F�،�̎��s�Ȃ̂ŕs�v ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			string sQuery      = "";
			string sQuery1     = "";
			string sQuery2     = "";
			string s����b�c   = sKey1;
			string s���p�b�c   = sKey2;
			string s���p�Җ�   = "";
			string s����b�c   = "";
			string s���喼     = "";
			string s�o�ד�     = "";
			string s�ב��l�b�c = "";
			string s���吔     = "0";
			string s���Ӑ搔   = "0";
// ADD 2005.07.21 ���s�j���� �X�����[�U�Ή� START
			string s�����P     = "";
// ADD 2005.07.21 ���s�j���� �X�����[�U�Ή� END
			try
			{
				// ���p�ҏ��̎擾
				sQuery
					= GET_TANMATSU_SELECT_2
					+ " WHERE RIY.����b�c   = '"+ s����b�c + "' \n"
					+   " AND RIY.���p�҂b�c = '"+ s���p�b�c + "' \n"
					+ GET_TANMATSU_SELECT_2_WHERE;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					s���p�Җ�   = reader.GetString(0).Trim();
					s����b�c   = reader.GetString(1).Trim();
					s�ב��l�b�c = reader.GetString(2).Trim();
					s���喼     = reader.GetString(3).Trim();
					s�o�ד�     = reader.GetString(4).Trim();
// ADD 2005.07.21 ���s�j���� �X�����[�U�Ή� START
					s�����P     = reader.GetString(5).Trim();
// ADD 2005.07.21 ���s�j���� �X�����[�U�Ή� END
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				// ���吔�̎擾
				int iCntBumon = 0;
				sQuery1
					=  " FROM �b�l�O�Q���� \n"
					+ " WHERE ����b�c = '"+ s����b�c + "' \n"
					+   " AND �폜�e�f = '0' \n"
					;

				reader = CmdSelect(sUser, conn2, SELECT_COUNT + sQuery1);

				if (reader.Read())
				{
//					s���吔 = reader.GetString(0);
					s���吔 = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				iCntBumon = int.Parse(s���吔);

				// ���Ӑ搔�̎擾
				int iCntTokui = 0;
				sQuery2
					=  " FROM �b�l�O�Q����     B, \n"
					+       " �r�l�O�S������   S  \n"
					+ " WHERE B.����b�c = '"+ s����b�c + "' \n"
					+   " AND B.����b�c = '"+ s����b�c + "' \n"
					+   " AND B.�폜�e�f = '0' \n"
					+   " AND B.�X�֔ԍ�      = S.�X�֔ԍ� \n"
					+   " AND '"+s����b�c+"' = S.����b�c \n"
					+   " AND '0'             = S.�폜�e�f \n"
					;

				reader = CmdSelect(sUser, conn2, SELECT_COUNT_S + sQuery2);

				if (reader.Read())
				{
//					s���Ӑ搔 = reader.GetString(0);
					s���Ӑ搔 = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				iCntTokui = int.Parse(s���Ӑ搔);

				int iPos = sRet.Length;
				if(iCntBumon > 0 || iCntTokui > 0)
				{
					sRet = new string[iPos + (iCntBumon * 3) + (iCntTokui * 3) ];
				}

				// ������̎擾
				if(iCntBumon > 0)
				{
					sQuery
						= "SELECT ����b�c, ���喼, �o�ד� \n"
						+ sQuery1
						+ " ORDER BY �g�D�b�c, �o�͏� \n"
						;

					reader = CmdSelect(sUser, conn2, sQuery);

					while (reader.Read())
					{
						sRet[iPos++] = reader.GetString(0).Trim();
						sRet[iPos++] = reader.GetString(1).Trim();
						sRet[iPos++] = reader.GetString(2).Trim();
					}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				}

				// ���Ӑ���̎擾
				if(iCntTokui > 0)
				{
					sQuery
						= "SELECT S.���Ӑ�b�c, S.���Ӑ敔�ۂb�c, S.���Ӑ敔�ۖ� \n"
						+ sQuery2
//�ۗ� MOD 2011.09.16 ���s�j���� �����於�������ꍇ�̃\�[�g���Ή� START
						+ " ORDER BY S.���Ӑ敔�ۖ� \n"
//						+ " ORDER BY S.���Ӑ敔�ۖ�, S.���Ӑ�b�c, S.���Ӑ敔�ۂb�c \n"
//�ۗ� MOD 2011.09.16 ���s�j���� �����於�������ꍇ�̃\�[�g���Ή� END
						;

					reader = CmdSelect(sUser, conn2, sQuery);

					while (reader.Read())
					{
						sRet[iPos++] = reader.GetString(0).Trim();
						sRet[iPos++] = reader.GetString(1).Trim();
						sRet[iPos++] = reader.GetString(2).Trim();
					}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				}
				sRet[0] = "����I��";

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			sRet[1] = s���p�Җ�;
			sRet[2] = s����b�c;
			sRet[3] = s���喼;
			sRet[4] = s�o�ד�;
			sRet[5] = s�ב��l�b�c;
			sRet[6] = s���吔;
			sRet[7] = s���Ӑ搔;

// ADD 2005.07.21 ���s�j���� �X�����[�U�Ή� START
			sRet.CopyTo(sRet = new string[sRet.Length + 1], 0);
			sRet[sRet.Length - 1] = s�����P;
// ADD 2005.07.21 ���s�j���� �X�����[�U�Ή� END

// ADD 2007.10.19 ���s�j���� �[���o�[�W�����Ǘ� START
			if(sUser.Length >= 4)
			{
				// �[���o�[�W�����X�V
				sUser[0] = s����b�c;
				sUser[1] = s���p�b�c;
				string[] sKeyVer = {s����b�c,"���j���["};
				string   sRetVer = Upd_bumon_ver(sUser, sKeyVer);
			}
// ADD 2007.10.19 ���s�j���� �[���o�[�W�����Ǘ� END

			return sRet;
		}

		/*********************************************************************
		 * ��������擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A���Ӑ搔
		 *		 ���Ӑ�b�c�A���Ӑ敔�ۂb�c�A���Ӑ敔�ۖ�
		 *********************************************************************/
		[WebMethod]
		public String[] Get_seikyu(string[] sUser, string sKey1, string sKey2)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "��������擾�J�n");

			OracleConnection conn2 = null;
			String[] sRet = new string[2];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			string sQuery    = "";
			string sQuery1   = "";
			string s���Ӑ搔 = "0";
			try
			{
				// ���Ӑ搔�̎擾
				int iCntTokui = 0;
				sQuery1
					=  " FROM �b�l�O�Q����     B, \n"
					+       " �r�l�O�S������   S  \n"
					+ " WHERE B.����b�c = '"+ sKey1 + "' \n"
					+   " AND B.����b�c = '"+ sKey2 + "' \n"
					+   " AND B.�폜�e�f = '0' \n"
					+   " AND B.�X�֔ԍ�     = S.�X�֔ԍ� \n"
					+   " AND '"+ sKey1 + "' = S.����b�c \n"
					+   " AND '0'            = S.�폜�e�f \n"
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, SELECT_COUNT_S + sQuery1);

				if (reader.Read())
				{
//					s���Ӑ搔 = reader.GetString(0);
					s���Ӑ搔 = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				iCntTokui = int.Parse(s���Ӑ搔);

				int iPos = 2;
				if(iCntTokui > 0)
				{
					sRet = new string[2 + (iCntTokui * 3) ];
				}

				// ���Ӑ���̎擾
				if(iCntTokui > 0)
				{
					sQuery
						= "SELECT S.���Ӑ�b�c, S.���Ӑ敔�ۂb�c, S.���Ӑ敔�ۖ� \n"
						+ sQuery1
//�ۗ� MOD 2011.09.16 ���s�j���� �����於�������ꍇ�̃\�[�g���Ή� START
						+  "ORDER BY S.���Ӑ敔�ۖ� \n"
//						+  "ORDER BY S.���Ӑ敔�ۖ�, S.���Ӑ�b�c, S.���Ӑ敔�ۂb�c \n"
//�ۗ� MOD 2011.09.16 ���s�j���� �����於�������ꍇ�̃\�[�g���Ή� END
						;

					reader = CmdSelect(sUser, conn2, sQuery);

					while (reader.Read())
					{
						sRet[iPos++] = reader.GetString(0).Trim();
						sRet[iPos++] = reader.GetString(1).Trim();
						sRet[iPos++] = reader.GetString(2).Trim();
					}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				}
				sRet[0] = "����I��";

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			sRet[1] = s���Ӑ搔;

			return sRet;
		}

		/*********************************************************************
		 * ���O�C���F��
		 * �����F����b�c�A���p�b�c�A�p�X���[�h
		 * �ߒl�F�X�e�[�^�X�A����b�c�A������A���b�Z�[�W�A���p�҂b�c�A���p�Җ��A���b�Z�[�W
		 *********************************************************************/
		private static string LOGIN_SELECT
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//			= "SELECT CM01.����b�c, CM01.�����, \n"
//			+       " CM04.���p�҂b�c, CM04.���p�Җ�, \n"
//			+       " CM04.�p�X���[�h, TO_CHAR(CM04.�F�؃G���[��), \n"
//			+       " CM01.\"���b�Z�[�W\" \n"
			= "SELECT CM01.����b�c, \n"
			+       " CM01.�����, \n"
			+       " CM04.���p�҂b�c, \n"
			+       " CM04.���p�Җ�, \n"
			+       " CM04.\"�p�X���[�h\", \n"
			+       " CM04.\"�F�؃G���[��\", \n"
			+       " CM01.�g�p�J�n��, \n"
			+       " CM01.�g�p�I����, \n"
			+       " SYSDATE \n"
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
			+       ", CM04.�o�^�o�f \n"
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
			+  " FROM �b�l�O�P���   CM01, \n"
// ADD 2005.08.19 ���s�j���� ����폜�̑Ή� START
			+       " �b�l�O�Q����   CM02, \n"
// ADD 2005.08.19 ���s�j���� ����폜�̑Ή� END
			+       " �b�l�O�S���p�� CM04  \n";

		private static string LOGIN_SELECT_WHERE
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//			=   " AND CM01.�g�p�J�n�� <= TO_CHAR(SYSDATE,'YYYYMMDD') \n"
//			+   " AND CM01.�g�p�I���� >= TO_CHAR(SYSDATE,'YYYYMMDD') \n"
//			+   " AND CM01.�Ǘ��ҋ敪 IN ('0','1','9') \n"  // 0:��� 1:�Ǘ��� 9:�����e�i���X
//			+   " AND CM01.�폜�e�f = '0' \n"
//			+   " AND CM04.�폜�e�f = '0' \n";
			=    " AND CM04.�폜�e�f = '0' \n"
			+    " AND CM04.����b�c = CM01.����b�c \n"
			+    " AND CM01.�폜�e�f = '0' \n"
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
// ADD 2005.08.19 ���s�j���� ����폜�̑Ή� START
			+    " AND CM04.����b�c = CM02.����b�c \n"
			+    " AND CM04.����b�c = CM02.����b�c \n"
			+    " AND CM02.�폜�e�f = '0' \n";
// ADD 2005.08.19 ���s�j���� ����폜�̑Ή� END

		[WebMethod]
		public string[] login(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "���O�C���F�؊J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[6];

// ADD 2008.03.21 ���s�j���� ���x���A�b�v���i�Ή� START
// �� is2Webapplication�ł��g�p���Ă���̂Œ���
//			if(sUser.Length < 4)
//			{
//				sRet[0] = "��q�l�̂h�c�́A���p����������Ă��܂��@\n"
//						+ "�Ŋ�̉c�Ə��܂Ō�A��������";
//				return sRet;
//			}
// ADD 2008.03.21 ���s�j���� ���x���A�b�v���i�Ή� END

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			string sQuery = "";
			string s����b�c = sKey[0];
			string s���p�b�c = sKey[1];
			string s�p�X���h = sKey[2];
			int i�F�؃G���[�� = 0;

			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				sQuery
					= LOGIN_SELECT
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//					+ " WHERE CM01.����b�c   = '" + s����b�c + "' \n"
//					+   " AND CM01.����b�c   = CM04.����b�c \n"
//					+   " AND CM04.���p�҂b�c = '" + s���p�b�c + "' \n"
////					+   " AND CM04.�p�X���[�h = '" + s�p�X���h + "' \n"
					+ " WHERE CM04.����b�c = '" + s����b�c + "' \n"
					+   " AND CM04.���p�҂b�c = '" + s���p�b�c + "' \n"
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
					+ LOGIN_SELECT_WHERE
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
					sRet[3] = reader.GetString(2).Trim();
					sRet[4] = reader.GetString(3).Trim();
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H START
//					i�F�؃G���[�� = int.Parse(reader.GetString(5));
//					sRet[5] = reader.GetString(6).Trim();
					i�F�؃G���[�� = int.Parse(reader.GetDecimal(5).ToString());
					sRet[5] = "";
					string s�g�p�J�n�� = reader.GetString(6).Trim();
					int    i�g�p�J�n�� = int.Parse(s�g�p�J�n��);
					int    i�g�p�I���� = int.Parse(reader.GetString(7).Trim());
					int    i����       = int.Parse(reader.GetDateTime(8).ToString("yyyyMMdd").Trim());
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
					int    i�p�X�X�V�� = 0;
					try{
						i�p�X�X�V�� = int.Parse(reader.GetString(9).Trim());
					}catch (Exception ){
						;
					}
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END
					if (i���� < i�g�p�J�n��)
					{
						if(s�g�p�J�n��.Length == 8)
						{
							string s�N = s�g�p�J�n��.Substring(0,4);
							string s�� = s�g�p�J�n��.Substring(4,2);
							string s�� = s�g�p�J�n��.Substring(6,2);
							if(s��[0] == '0') s�� = s��.Substring(1,1);
							if(s��[0] == '0') s�� = s��.Substring(1,1);
							sRet[0] = s�N + "�N" + s�� + "��"
									+ s�� + "�����g�p�ł��܂�";
						}
						else
						{
							sRet[0] = "�g�p�J�n�����g�p�ł��܂�";
						}
						tran.Commit();
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
						return sRet;
					}
					if (i���� > i�g�p�I����)
					{
						sRet[0] = "�g�p�������؂�Ă��܂�";
						tran.Commit();
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
						return sRet;
					}
// MOD 2005.06.02 ���s�j���� ORA-03113�΍�H END
// ADD 2008.05.21 ���s�j���� ���O�C���G���[�񐔂��T��ɂ��� START
//					if (i�F�؃G���[�� >= 10)
					if (i�F�؃G���[�� >= 5)
// ADD 2008.05.21 ���s�j���� ���O�C���G���[�񐔂��T��ɂ��� END
					{
// MOD 2005.08.19 ���s�j���� ���b�Z�[�W�̕ύX START
//						sRet[0] = "�F�؃G���[�F�Ŋ�̉c�Ə��܂Ō�A��������";
						sRet[0] = "��q�l�̂h�c�́A���p����������Ă��܂��@\n"
// MOD 2009.09.14 ���s�j���� �p�X���[�h�G���[���̖₢���킹��̕ύX START
//								+ "�Ŋ�̉c�Ə��܂Ō�A��������";
								+ "�p�`�Z���^�[�܂��͍Ŋ�̉c�Ə��܂Ō�A��������";
// MOD 2009.09.14 ���s�j���� �p�X���[�h�G���[���̖₢���킹��̕ύX END
// MOD 2005.08.19 ���s�j���� ���b�Z�[�W�̕ύX END
						//Session.Clear();
					}
					else if(reader.GetString(4).Trim() == s�p�X���h)
					{
						sRet[0] = "����I��";
						//Session.Add("member", sRet[1]);
						//Session.Add("user",   sRet[3]);
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� START
						DateTime dt���� = new DateTime(
							int.Parse(i����.ToString().Substring(0,4))
							, int.Parse(i����.ToString().Substring(4,2))
							, int.Parse(i����.ToString().Substring(6,2))
							);
						DateTime dt�p�X�X�V�� = new DateTime(
							int.Parse(i�p�X�X�V��.ToString().Substring(0,4))
							, int.Parse(i�p�X�X�V��.ToString().Substring(4,2))
							, int.Parse(i�p�X�X�V��.ToString().Substring(6,2))
							);
						DateTime dt�p�X�L������ = dt�p�X�X�V��.AddMonths(6);

						//�U�����ȏ�o�߂������_�ŁA�p�X���[�h�X�V�x���\��
						if(dt����.CompareTo(dt�p�X�L������) > 0){
							sRet[0] = "�����؂�";
						//�T�����ȏ�o�߂������_�ŁA�p�X���[�h�X�V�x���\��
						}else if(dt����.CompareTo(dt�p�X�X�V��.AddMonths(5)) > 0){
							sRet[0] = dt�p�X�L������.ToString("MMdd").Trim();
						}
// ADD 2008.06.30 ���s�j���� �p�X���[�h�`�F�b�N�̋��� END
					}
					else
					{
// MOD 2009.05.27 ���s�j���� �p�X���[�h�G���[���Ƀp�X���[�h�X�V����\�� START
//						sRet[0] = "���p�҂b�c �������� �p�X���[�h �Ɍ�肪����܂�";
						sRet[0] = "���p�҂b�c �������� �p�X���[�h �Ɍ�肪����܂�\n"
								+ "�@�@�@�@�@�i"
								+ int.Parse(i�p�X�X�V��.ToString().Substring(4,2)) + "/"
								+ int.Parse(i�p�X�X�V��.ToString().Substring(6,2))
								+ " �ɕύX����Ă��܂��j"
								;
// MOD 2009.05.27 ���s�j���� �p�X���[�h�G���[���Ƀp�X���[�h�X�V����\�� END
						//Session.Clear();
					}
				}
				else
				{
					sRet[0] = "���p�҂b�c �������� �p�X���[�h �Ɍ�肪����܂�";
					//Session.Clear();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				// ���p�҃}�X�^���X�V����
				if(sRet[0].Length != 4 || i�F�؃G���[�� != 0)
				{
					if (sRet[0].Length == 4){
						i�F�؃G���[�� = 0;
					}else if(i�F�؃G���[�� < 90){
						i�F�؃G���[��++;
					}else{
						i�F�؃G���[�� = 90;
					}

					// ���p�҃}�X�^�̍X�V
					sQuery
						= "UPDATE �b�l�O�S���p�� \n"
						+   " SET �F�؃G���[�� = " + i�F�؃G���[�� + ", \n"
						+       " �X�V����   = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+       " �X�V�o�f   = '" + "���O�C��" + "', \n"
						+       " �X�V��     = '" + s���p�b�c + "' \n"
						+ " WHERE ����b�c   = '" + s����b�c + "' \n"
						+   " AND ���p�҂b�c = '" + s���p�b�c + "' \n"
						;
					CmdUpdate(sUser, conn2, sQuery);
				}

// DEL 2007.02.08 ���s�j���� �N���C�A���g�A�v���̍����� START
//// ADD 2005.06.17 ���s�j���� �[���}�X�^�̍X�V START
//				if(sRet[0].Length == 4)
//				{
//					// �[���}�X�^�̍X�V
//					sQuery
//						= "UPDATE �b�l�O�R�[�� \n"
//						+   " SET �N����� = '1', \n"
//						+       " ���s��� = '���O�C��', \n"
//						+       " ���s�R�}���h = '���O�C��', \n"
//						+       " ���s���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
//						+       " �X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
//						+       " �X�V�o�f = '���O�C��', \n"
//						+       " �X�V��   = '" + s���p�b�c + "' \n"
//						+ " WHERE �[���b�c = '" + sUser[2]  + "' \n"
//						;
//
//					CmdUpdate(sUser, conn2, sQuery);
//				}
//// ADD 2005.06.17 ���s�j���� �[���}�X�^�̍X�V END
// DEL 2007.02.08 ���s�j���� �N���C�A���g�A�v���̍����� END

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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}

		/*********************************************************************
		 * ��Ԉꗗ�擾
		 * �����F�Ȃ�
		 * �ߒl�F�X�e�[�^�X�A��Ԉꗗ
		 *********************************************************************/
		private static string GET_JYOTAI_COUNT
//			= "SELECT TO_CHAR(COUNT(DISTINCT ��Ԗ�)) \n"
//			= "SELECT NVL(COUNT(DISTINCT ��Ԗ�),0) \n"
// MOD 2005.06.10 ���s�j���� �r�p�k�̊ȗ��� START
//			= "SELECT COUNT(DISTINCT ��Ԗ�) \n"
//			+  " FROM �`�l�O�R��� \n"
//			+ " WHERE �폜�e�f = '0' \n";
//			= "SELECT COUNT(*) \n"
			= "SELECT COUNT(ROWID) \n"
			+  " FROM �`�l�O�R��� \n"
			+ " WHERE ��ԏڍׂb�c = ' ' \n"
			+ " AND �폜�e�f = '0' \n";
// MOD 2005.06.10 ���s�j���� �r�p�k�̊ȗ��� END

		private static string GET_JYOTAI
// MOD 2005.06.10 ���s�j���� �r�p�k�̊ȗ��� START
//			= "SELECT DISTINCT ��Ԃb�c, ��Ԗ� \n"
//			+  " FROM �`�l�O�R��� \n"
//			+ " WHERE �폜�e�f = '0' \n"
//			+ " ORDER BY ��Ԃb�c, ��Ԗ� \n";
			= "SELECT ��Ԃb�c, ��Ԗ� \n"
			+  " FROM �`�l�O�R��� \n"
			+ " WHERE ��ԏڍׂb�c = ' ' \n"
			+ " AND �폜�e�f = '0' \n"
			+ " ORDER BY ��Ԃb�c, ��Ԗ� \n";
// MOD 2005.06.10 ���s�j���� �r�p�k�̊ȗ��� END

		[WebMethod]
		public string[] Get_jyotai(string[] sUser )
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "��Ԉꗗ�擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			string s��Ԑ� = "0";
			int    i��Ԑ� = 0;
			try
			{
				OracleDataReader reader = CmdSelect(sUser, conn2, GET_JYOTAI_COUNT);
				if (reader.Read() )
				{
//					s��Ԑ� = reader.GetString(0);
					s��Ԑ� = reader.GetDecimal(0).ToString().Trim();
				}
				i��Ԑ� = int.Parse(s��Ԑ�);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				reader = CmdSelect(sUser, conn2, GET_JYOTAI);

				int iPos = 2;
				sRet = new string[i��Ԑ� * 2 + iPos];
				while (reader.Read() && iPos < sRet.Length)
				{
					sRet[iPos++] = reader.GetString(0).Trim();
					sRet[iPos++] = reader.GetString(1).Trim();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				if(iPos > 2){
					sRet[0] = "����I��";
					sRet[1] = s��Ԑ�;
				}else{
					sRet[0] = "�T�[�o�G���[�F��ԃ}�X�^���ݒ肳��Ă��܂���";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}

		/*********************************************************************
		 * �o�ד��擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�o�ד�
		 *********************************************************************/
		[WebMethod]
		public String[] Get_syukabi(string[] sUser, string sKey1, string sKey2)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�ד��擾�J�n");

			OracleConnection conn2 = null;
			String[] sRet = new string[2];
			// ADD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			string sQuery  = "";
			try
			{
				sQuery
					= "SELECT �o�ד� \n"
					+  " FROM �b�l�O�Q���� \n"
					+ " WHERE ����b�c = '"+ sKey1 + "' \n"
					+   " AND ����b�c = '"+ sKey2 + "' \n"
					+   " AND �폜�e�f = '0' \n"
					;

				// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
				//OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);
				logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + sQuery);	//�C���O��UPDATE�������O�o��

				sQuery
					= "SELECT �o�ד� \n"
					+  " FROM �b�l�O�Q���� \n"
					+ " WHERE ����b�c = :p_KaiinCD \n"
					+   " AND ����b�c = :p_BumonCD \n"
					+   " AND �폜�e�f = '0' \n"
					;
				wk_opOraParam = new OracleParameter[2];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKey1, ParameterDirection.Input);
				wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sKey2, ParameterDirection.Input);

				OracleDataReader	reader = CmdSelect(sUser, conn2, sQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

				if (reader.Read())
				{
					sRet[0] = "����I��";
					sRet[1] = reader.GetString(0);
				}else{
					sRet[0] = "�Y���f�[�^������܂���";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}

		/*********************************************************************
		 * ������擾
		 * �����F����b�c
		 * �ߒl�F�X�e�[�^�X�A���吔�A����b�c�A���喼�A�o�ד�
		 *********************************************************************/
		[WebMethod]
		public String[] Get_bumon(string[] sUser, string sKey1)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "������擾�J�n");

			OracleConnection conn2 = null;
			String[] sRet = new string[2];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			string s����b�c = sKey1;
			string sQuery  = "";
			string sQuery1 = "";
			string s���吔 = "0";
			try
			{
				// ���吔�̎擾
				int iCntBumon = 0;
				sQuery1
					=  " FROM �b�l�O�Q���� \n"
					+ " WHERE ����b�c = '"+ s����b�c + "' \n"
					+   " AND �폜�e�f = '0' \n"
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, SELECT_COUNT + sQuery1);

				if (reader.Read())
				{
//					s���吔 = reader.GetString(0);
					s���吔 = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				iCntBumon = int.Parse(s���吔);

				int iPos = sRet.Length;
				if(iCntBumon > 0)
				{
					sRet = new string[iPos + (iCntBumon * 3)];
				}

				// ������̎擾
				if(iCntBumon > 0)
				{
					sQuery
						= "SELECT ����b�c, ���喼, �o�ד� \n"
						+ sQuery1
						+ " ORDER BY �g�D�b�c, �o�͏� \n"
						;

					reader = CmdSelect(sUser, conn2, sQuery);

					while (reader.Read())
					{
						sRet[iPos++] = reader.GetString(0).Trim();
						sRet[iPos++] = reader.GetString(1).Trim();
						sRet[iPos++] = reader.GetString(2).Trim();
					}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				}

				sRet[0] = "����I��";

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			sRet[1] = s���吔;

			return sRet;
		}

		/*********************************************************************
		 * ���b�Z�[�W�̎擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�X�����b�Z�[�W�A������b�Z�[�W
		 *********************************************************************/
// ADD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� START
		private static string GET_MESSAGE_SELECT_1
			= "SELECT \"���b�Z�[�W\" \n"
			+  " FROM �`�l�O�P�V�X�e���Ǘ� \n"
			+ " WHERE �V�X�e���Ǘ��b�c = 'is2' \n";
// ADD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� END

		[WebMethod]
		public String[] Get_message(string[] sUser, string sKey1, string sKey2)
		{
//			logFileOpen(sUser);
//			logWriter(sUser, INF, "���b�Z�[�W�擾�J�n");

// MOD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� START
//			String[] sRet = new string[3];
			OracleConnection conn2 = null;
			string[] sRet = new string[4];
// MOD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� END

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//
//				sRet[0] = "����I��";
//				sRet[1] = "";
//				sRet[2] = "";
//				return sRet;
//			}
// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			string sQuery  = "";
// ADD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� START
			string s�V�X�e�����b�Z�[�W = "";
// ADD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� END
			string s�X�����b�Z�[�W = "";
			string s������b�Z�[�W = "";
			try
			{
// ADD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� START
				OracleDataReader reader;

				sQuery = GET_MESSAGE_SELECT_1;

				reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					s�V�X�e�����b�Z�[�W = reader.GetString(0).Trim();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// ADD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� END

// DEL 2005.06.02 ���s�j���� �s�v�ȗp���Ȃ̂ŃR�����g�� START
//				sQuery
//					= "SELECT T.\"���b�Z�[�W\" \n"
//					+  " FROM �b�l�O�Q���� B, �b�l�P�S�X�֔ԍ� Y, �b�l�P�O�X�� T \n"
//					+ " WHERE B.����b�c = '"+ sKey1 + "' \n"
//					+   " AND B.����b�c = '"+ sKey2 + "' \n"
//					+   " AND B.�X�֔ԍ� = Y.�X�֔ԍ� \n"
//					+   " AND Y.�X���b�c = T.�X���b�c \n"
//					;
//
//				reader = CmdSelect(sUser, conn2, sQuery);
//
//				if (reader.Read())
//				{
//					s�X�����b�Z�[�W = reader.GetString(0).Trim();
//				}
// DEL 2005.06.02 ���s�j���� �s�v�ȗp���Ȃ̂ŃR�����g�� END

// DEL 2007.02.08 ���s�j���� �N���C�A���g�A�v���̍����� START
//				sQuery
//					= "SELECT \"���b�Z�[�W\" \n"
//					+  " FROM �b�l�O�P��� \n"
//					+ " WHERE ����b�c = '"+ sKey1 + "' \n"
//					;
//
//				reader = CmdSelect(sUser, conn2, sQuery);
//
//				if (reader.Read())
//				{
//					s������b�Z�[�W = reader.GetString(0).Trim();
//				}
// DEL 2007.02.08 ���s�j���� �N���C�A���g�A�v���̍����� END

				sRet[0] = "����I��";
// MOD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� START
//				sRet[1] = s�X�����b�Z�[�W;
//				sRet[2] = s������b�Z�[�W;

// ADD 2008.07.07 ���s�j���� ����̉���ɂ̓��b�Z�[�W��\�� START
				//����̉���̏ꍇ�ɂ́A���b�Z�[�W��\�����Ȃ�
				//���[�i�L�j�≺����]�l
				//����[�i���j�~�}�L�G���W�j�A�����O]�l
				if(sKey1.Trim().Equals("0268631151")){
// MOD 2008.09.19 ���s�j���� ���x���A�b�v���i�Ή� START
//					sRet[0] = "����I��";
//					s�V�X�e�����b�Z�[�W = "";
//					sRet[2] = "";
//					sRet[3] = "";
//					return sRet;
					s�V�X�e�����b�Z�[�W = "";
// MOD 2008.09.19 ���s�j���� ���x���A�b�v���i�Ή� END
				}
				//���[�i���j�h�H��]�l
				//����[�i���j�h�H��]�l
				if(sKey1.Trim().Equals("0849213322")
				&& sKey2.Trim().Equals("0849213322")){
// MOD 2008.09.19 ���s�j���� ���x���A�b�v���i�Ή� START
//					sRet[0] = "����I��";
//					s�V�X�e�����b�Z�[�W = "";
//					sRet[2] = "";
//					sRet[3] = "";
//					return sRet;
					s�V�X�e�����b�Z�[�W = "";
// MOD 2008.09.19 ���s�j���� ���x���A�b�v���i�Ή� END
				}
// ADD 2008.07.07 ���s�j���� ����̉���ɂ̓��b�Z�[�W��\�� END
// MOD 2010.07.29 ���s�j���� ����̉���ɂ̓��b�Z�[�W��\�� START
				//���[�ې��������@���s�x��]�l
				if(sKey1.Trim().Equals("0756213939")){
					s�V�X�e�����b�Z�[�W = "";
				}
// MOD 2010.07.29 ���s�j���� ����̉���ɂ̓��b�Z�[�W��\�� END

// ADD 2008.03.21 ���s�j���� ���x���A�b�v���i�Ή� START
// �� is2Webapplication�ł��g�p���Ă���̂Œ���
				if(sUser.Length < 4)
				{
					s�V�X�e�����b�Z�[�W
						= "�v���O�����̓���ւ����K�v�ł��B"
						+ "�ڂ����́A���R�ʉ^�̂��r�s�`�q�|�Q�_�E�����[�h��ʂɂ���"
						+ "�m�ăZ�b�g�A�b�v�菇���n�������������B"
//						= "���x�����p���肪�Ƃ��������܂��B"
//						= "�{�A�v���̃o�[�W�����A�b�v��Ƃ��K�v�Ȏ����ɂȂ�܂����B"
//						+ "�ڂ����́A�m�w���v�n�́m�ăZ�b�g�A�b�v�菇�n�܂ŁB"
						+ s�V�X�e�����b�Z�[�W
						+ "";
				}
// MOD 2009.10.05 ���s�j���� �}�C�i�[�o�[�W�����Q���Ή��iVer.2.10�`�jSTART
//// MOD 2008.07.07 ���s�j���� ���x���A�b�v���i�̃��x���ύX START
////				else if (double.Parse(sUser[3]) < 2.1)
//				else if (double.Parse(sUser[3]) < 2.3)
//// MOD 2008.07.07 ���s�j���� ���x���A�b�v���i�̃��x���ύX END
				else
				{
					try
					{
						//�P�ڂ̃s���I�h
						int iPos = sUser[3].IndexOf('.');
						int iMajor = int.Parse(sUser[3].Substring(0,iPos));
						int iMiner = int.Parse(sUser[3].Substring(iPos+1));
						if((iMajor < 2)
							|| (iMajor == 2 && iMiner < 3))
// MOD 2009.10.05 ���s�j���� �}�C�i�[�o�[�W�����Q���Ή��iVer.2.10�`�jEND
						{
							s�V�X�e�����b�Z�[�W
								= "�v���O�����̓���ւ����K�v�ł��B"
								+ "�ڂ����́A���R�ʉ^�̂��r�s�`�q�|�Q�_�E�����[�h��ʂɂ���"
								+ "�m�ăZ�b�g�A�b�v�菇���n�������������B"
								//						= "���x�����p���肪�Ƃ��������܂��B"
								//						= "�{�A�v���̃o�[�W�����A�b�v��Ƃ��K�v�Ȏ����ɂȂ�܂����B"
								//						+ "�ڂ����́A�m�w���v�n�́m�ăZ�b�g�A�b�v�菇�n�܂ŁB"
								+ s�V�X�e�����b�Z�[�W
								+ "";
						}
// ADD 2008.03.21 ���s�j���� ���x���A�b�v���i�Ή� END
// MOD 2009.10.05 ���s�j���� �}�C�i�[�o�[�W�����Q���Ή��iVer.2.10�`�jSTART
					}
					catch(Exception)
					{
						;
					}
				}
// MOD 2009.10.05 ���s�j���� �}�C�i�[�o�[�W�����Q���Ή��iVer.2.10�`�jEND

				sRet[1] = s�V�X�e�����b�Z�[�W;
				sRet[2] = s�X�����b�Z�[�W;
				sRet[3] = s������b�Z�[�W;
// MOD 2005.05.24 ���s�j���� �V�X�e�����b�Z�[�W�̒ǉ� END

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}

// ADD 2005.05.18 ���s�j�����J ��ʐ���̎擾 START
		/*********************************************************************
		 * ��ʐ���̎擾
		 * �����F����b�c�A����b�c�A���ڂb�c
		 * �ߒl�F�X�e�[�^�X�A��\���e�f
		 *********************************************************************/
		[WebMethod]
// MOD 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX START
//		public String[] Get_seigyo(string[] sUser, string sKey1, string sKey2, string sKey3)
		public String[] Get_seigyo(string[] sUser, string sKey1, string sKey2)
// MOD 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX END
// ADD 2009.01.30 ���s�j���� ���шꗗ����I�v�V�������ڂ̒ǉ� START
		{
			return Get_seigyo2(sUser, sKey1, sKey2, 11);
		}
		[WebMethod]
		public String[] Get_seigyo2(string[] sUser, string sKey1, string sKey2, int iLength)
// ADD 2009.01.30 ���s�j���� ���шꗗ����I�v�V�������ڂ̒ǉ� END
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "��ʐ���擾�J�n");

			OracleConnection conn2 = null;
			String[] sRet = new string[1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			string sQuery  = "";
// ADD 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX START
// MOD 2006.06.28 ���s�j�R�{�@�G���g���I�v�V�������ڒǉ� START
//			sRet = new String[11];
// MOD 2009.01.30 ���s�j���� ���шꗗ����I�v�V�������ڂ̒ǉ� START
//			sRet = new String[12];
			sRet = new String[1 + iLength];
// MOD 2009.01.30 ���s�j���� ���шꗗ����I�v�V�������ڂ̒ǉ� END
// MOD 2006.06.28 ���s�j�R�{�@�G���g���I�v�V�������ڒǉ� END
			for (int iCnt = 1; iCnt < sRet.Length; iCnt++)
			{
				sRet[iCnt] = "9";
			}
// ADD 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX END
			try
			{
				sQuery
// MOD 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX START
//					= "SELECT ��\���e�f \n"
					= "SELECT ���ڂb�c,��\���e�f,�폜�e�f \n"
// MOD 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX END
					+  " FROM �`�l�O�S��ʐ��� \n"
					+ " WHERE ����b�c = '"+ sKey1 + "' \n"
					+   " AND ����b�c = '"+ sKey2 + "' \n"
// DEL 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX START
//					+   " AND ���ڂb�c = '"+ sKey3 + "' \n"
//					+   " AND �폜�e�f = '0' \n"
// DEL 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX END
// ADD 2009.01.30 ���s�j���� ���шꗗ����I�v�V�������ڂ̒ǉ� START
					+ " ORDER BY LENGTH(TRIM(���ڂb�c)), ���ڂb�c \n"
// ADD 2009.01.30 ���s�j���� ���шꗗ����I�v�V�������ڂ̒ǉ� END
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

// MOD 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX START
				while (reader.Read())
				{
					int i���� = int.Parse(reader.GetString(0).Trim());
// ADD 2009.01.30 ���s�j���� ���шꗗ����I�v�V�������ڂ̒ǉ� START
					//�z�񐔂𒴂��鍀�ڔԍ��̏ꍇ�ɂ̓p�X����
					if(i���� >= sRet.Length) continue;
// ADD 2009.01.30 ���s�j���� ���шꗗ����I�v�V�������ڂ̒ǉ� END
					if (reader.GetString(2).Trim().Equals("0"))
					{
						sRet[i����] = reader.GetString(1).Trim();
					}
					else
					{
						sRet[i����] = "0";
					}
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				sRet[0] = "����I��";
// MOD 2005.06.10 ���s�j�ɉ�@��ʐ���擾�����ύX END

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}
// ADD 2005.05.18 ���s�j�����J ��ʐ���̎擾 END
// MOD 2011.05.09 ���s�j���� ���q�l���̏d�ʓ��͕s�Ή� START
		[WebMethod]
		public String[] Get_seigyo3(string[] sUser, string sKey1, string sKey2)
		{
			logWriter(sUser, INF, "��ʐ���擾�R�J�n");

			OracleConnection conn2 = null;
			String[] sRet = new string[]{"",""};

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			string sQuery  = "";
			try
			{
				sQuery
					= "SELECT �ۗ�����e�f \n"
					+  " FROM �b�l�O�P��� CM01 \n"
					+ " WHERE CM01.����b�c = '"+ sKey1 + "' \n"
					;
				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);
				if(reader.Read()){
					sRet[1] = reader.GetString(0).TrimEnd();
				}
				disposeReader(reader);
				reader = null;

				sRet[0] = "����I��";
			}catch (OracleException ex){
				sRet[0] = chgDBErrMsg(sUser, ex);
			}catch (Exception ex){
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}finally{
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			if(sRet[1].Length == 0) sRet[1] = "9";

			return sRet;
		}
// MOD 2011.05.09 ���s�j���� ���q�l���̏d�ʓ��͕s�Ή� END

// ADD 2005.05.20 ���s�j�����J ��ʐ���̓o�^ START
		/*********************************************************************
		 * ��ʐ���̓o�^
		 * �����F����b�c�A����b�c�A���ڂb�c�A���ږ��A��\���e�f..
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String Ins_seigyo(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "��ʐ���o�^�J�n");

			OracleConnection conn2 = null;
			string sRet = "";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet = userCheck2(conn2, sUser);
//			if(sRet.Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery 
					= "DELETE FROM �`�l�O�S��ʐ��� \n"
					+ " WHERE ����b�c           = '" + sKey[0] +"' \n"
					+ "   AND ����b�c           = '" + sKey[1] +"' \n"
					+ "   AND ���ڂb�c           = '" + sKey[2] +"' \n"
					+ "   AND �폜�e�f           = '1'";

				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);

				cmdQuery 
					= "INSERT INTO �`�l�O�S��ʐ��� \n"
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
				sRet = "����I��";
				
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
// DEL 2005.05.31 ���s�j���� �s�v�Ȉ׍폜 START
//				string sErr = ex.Message.Substring(0,9);
//				if(sErr == "ORA-00001")
//					sRet = "����̃R�[�h�����ɑ��̒[�����o�^����Ă��܂��B\r\n�ēx�A�ŐV�f�[�^���Ăяo���čX�V���Ă��������B";
//				else
// DEL 2005.05.31 ���s�j���� �s�v�Ȉ׍폜 END
				sRet = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.05.20 ���s�j�����J ��ʐ���̓o�^ END


// ADD 2005.05.20 ���s�j�����J ��ʐ���̍X�V START
		/*********************************************************************
		 * ��ʐ���̍X�V
		 * �����F����b�c�A����b�c�A���ڂb�c�A���ږ��A��\���e�f..
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String Upd_seigyo(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "��ʐ���}�X�^�̍X�V�J�n");

			OracleConnection conn2 = null;
			String sRet = "";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet = userCheck2(conn2, sUser);
//			if(sRet.Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.24 ���s�j���� ����`�F�b�N�ǉ� END

			string sQuery    = "";
			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				// ��ʐ���}�X�^�̍X�V
				sQuery
					= "UPDATE �`�l�O�S��ʐ��� \n"
					+    "SET ��\���e�f     = '" + sKey[4] + "', \n"
					+        "�폜�e�f       = '0', \n"
					+        "�X�V����       = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
					+        "�X�V�o�f       = '" + sKey[5] + "', \n"
					+        "�X�V��         = '" + sKey[6] + "'  \n"
					+  "WHERE ����b�c       = '" + sKey[0] + "'  \n"
					+  "  AND ����b�c       = '" + sKey[1] + "'  \n"
					+  "  AND ���ڂb�c       = '" + sKey[2] + "'  "
					;

				CmdUpdate(sUser, conn2, sQuery);

				tran.Commit();
				sRet = "����I��";

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
				sRet = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}
// ADD 2005.05.20 ���s�j�����J ��ʐ���̍X�V END

// ADD 2005.05.18 ���s�j���� �A�v���̋N���̍����� START
		/*********************************************************************
		 * ���O�C���F�؂Q
		 * �����F�[���b�c�A���p�b�c�A�p�X���[�h
		 * �ߒl�F�X�e�[�^�X�A����b�c�A������A���b�Z�[�W�A���p�҂b�c�A���p�Җ��A���b�Z�[�W
		 * �@�@�@�v�����^�e�f�A�v�����^���ʎq
		 *********************************************************************/
		[WebMethod]
		public string[] login2(string[] sUser, string[] sKey)
		{
			// ����b�c�̎擾
// MOD 2008.06.17 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒ǉ� START
//			string[] sRet1 = Get_tanmatsu2(sUser, sKey[0]);
			string[] sRet1 = Get_tanmatsu3(sUser, sKey);
// MOD 2008.06.17 ���s�j���� �l�`�b�A�h���X�`�F�b�N�̒ǉ� END
			if(sRet1[0].Length != 4)
				return sRet1;

			sKey[0] = sRet1[1];
			// ���O�C���F��
			string[] sRet = login(sUser, sKey);
			if(sRet[0].Length != 4)
				return sRet;

// MOD 2005.06.07 ���s�j���� �s���{���I���̕ύX START
//			sRet.CopyTo(sRet = new string[6 + 2], 0);
			sRet.CopyTo(sRet = new string[6 + 3], 0);
// MOD 2005.06.07 ���s�j���� �s���{���I���̕ύX END
			sRet[6] = sRet1[2];	// �v�����^�e�f
			sRet[7] = sRet1[3];	// �v�����^���ʎq
// ADD 2005.06.07 ���s�j���� �s���{���I���̕ύX START
			sRet[8] = sRet1[4];	// �s���{���b�c
// ADD 2005.06.07 ���s�j���� �s���{���I���̕ύX END

			return sRet;
		}
// ADD 2005.05.18 ���s�j���� �A�v���̋N���̍����� END

// ADD 2005.06.30 ���s�j�����J �I�����ɒ[���}�X�^�X�V START
		/*********************************************************************
		 * �[���}�X�^�X�V
		 * �����F�[���b�c�A����b�c�A���p�b�c
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Upd_tanmatu(string[] sUser, string sKey)
		{
// MOD 2007.02.08 ���s�j���� �N���C�A���g�A�v���̍����� START
//			logFileOpen(sUser);
//			logWriter(sUser, INF, "�[���}�X�^�X�V�J�n");
//
//			OracleConnection conn2 = null;
//			string[] sRet = new string[1];
//
//			// �c�a�ڑ�
//			conn2 = connect2(sUser);
//			if(conn2 == null)
//			{
//				logFileClose();
//				sRet[0] = "�c�a�ڑ��G���[";
//				return sRet;
//			}
//
//			string sQuery = "";
//
//			OracleTransaction tran = conn2.BeginTransaction();
//			try
//			{
//				// �[���}�X�^�̍X�V
//				sQuery
//					= "UPDATE �b�l�O�R�[�� \n"
//					+   " SET �N����� = '2', \n"
//					+       " ���s��� = '���j���[', \n"
//					+       " ���s�R�}���h = '�I��', \n"
//					+       " ���s���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
//					+       " �X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
//					+       " �X�V�o�f = '���j���[', \n"
//					+       " �X�V��   = '" + sKey + "' \n"
//					+ " WHERE �[���b�c = '" + sUser[2]  + "' \n"
//					;
//				CmdUpdate(sUser, conn2, sQuery);
//
//				tran.Commit();
//				sRet[0] = "����I��";
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
//				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
//				logWriter(sUser, ERR, sRet[0]);
//			}
//			finally
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//			}
//
//			return sRet;
			return new string[1]{"����I��"};
// MOD 2007.02.08 ���s�j���� �N���C�A���g�A�v���̍����� END
		}
// ADD 2005.06.30 ���s�j�����J �I�����ɒ[���}�X�^�X�V END

// ADD 2006.12.22 ���s�j�����J ���͐�S���폜�p���O�C���F�� START
		/*********************************************************************
		 * ���͐�S���폜�p���O�C���F��
		 * �����F����b�c�A���p�b�c�A�p�X���[�h
		 * �ߒl�F�X�e�[�^�X�A����b�c�A������A���b�Z�[�W�A���p�҂b�c�A���p�Җ��A���b�Z�[�W
		 *********************************************************************/
		private static string LOGIN_SELECT3
			= "SELECT CM04.����b�c, \n"
			+       " CM02.���喼, \n"
			+       " CM04.\"�F�؃G���[��\", \n"
			+       " CM01.�g�p�J�n��, \n"
			+       " CM01.�g�p�I����, \n"
			+       " SYSDATE \n"
			+  " FROM �b�l�O�P���   CM01, \n"
			+       " �b�l�O�Q����   CM02, \n"
			+       " �b�l�O�S���p�� CM04  \n";

		private static string LOGIN_SELECT_WHERE3
			=    " AND CM04.�폜�e�f = '0' \n"
			+    " AND CM04.����b�c = CM01.����b�c \n"
			+    " AND CM01.�폜�e�f = '0' \n"
			+    " AND CM04.����b�c = CM02.����b�c \n"
			+    " AND CM04.����b�c = CM02.����b�c \n"
			+    " AND CM02.�폜�e�f = '0' \n";

		[WebMethod]
		public string[] login3(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "���O�C���F�؊J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			string sQuery = "";
			string s����b�c = sKey[0];
			string s���p�b�c = sKey[1];
			string s�p�X���h = sKey[2];
			string s����b�c = sKey[3];
			int i�F�؃G���[�� = 0;

			OracleTransaction tran = conn2.BeginTransaction();
			try
			{
				sQuery
					= LOGIN_SELECT3
					+ " WHERE CM04.����b�c = '" + s����b�c + "' \n"
					+   " AND CM04.���p�҂b�c = '" + s���p�b�c + "' \n"
					+   " AND CM04.�p�X���[�h = '" + s�p�X���h + "' \n"
					+ LOGIN_SELECT_WHERE3
					;

				OracleDataReader reader = CmdSelect(sUser, conn2, sQuery);

				if (reader.Read())
				{
					i�F�؃G���[�� = int.Parse(reader.GetDecimal(2).ToString());
					string s�g�p�J�n�� = reader.GetString(3).Trim();
					int    i�g�p�J�n�� = int.Parse(s�g�p�J�n��);
					int    i�g�p�I���� = int.Parse(reader.GetString(4).Trim());
					int    i����       = int.Parse(reader.GetDateTime(5).ToString("yyyyMMdd").Trim());
					if (i���� < i�g�p�J�n��)
					{
						if(s�g�p�J�n��.Length == 8)
						{
							string s�N = s�g�p�J�n��.Substring(0,4);
							string s�� = s�g�p�J�n��.Substring(4,2);
							string s�� = s�g�p�J�n��.Substring(6,2);
							if(s��[0] == '0') s�� = s��.Substring(1,1);
							if(s��[0] == '0') s�� = s��.Substring(1,1);
							sRet[0] = s�N + "�N" + s�� + "��"
									+ s�� + "�����g�p�ł��܂�";
						}
						else
						{
							sRet[0] = "�g�p�J�n�����g�p�ł��܂�";
						}
						tran.Commit();
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
						return sRet;
					}
					if (i���� > i�g�p�I����)
					{
						sRet[0] = "�g�p�������؂�Ă��܂�";
						tran.Commit();
						logWriter(sUser, INF, sRet[0]);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
						return sRet;
					}
// ADD 2008.05.21 ���s�j���� ���O�C���G���[�񐔂��T��ɂ��� START
//					if (i�F�؃G���[�� >= 10)
					if (i�F�؃G���[�� >= 5)
// ADD 2008.05.21 ���s�j���� ���O�C���G���[�񐔂��T��ɂ��� END
					{
						sRet[0] = "��q�l�̂h�c�́A���p����������Ă��܂��@\n"
// MOD 2009.09.14 ���s�j���� �p�X���[�h�G���[���̖₢���킹��̕ύX START
//								+ "�Ŋ�̉c�Ə��܂Ō�A��������";
								+ "�p�`�Z���^�[�܂��͍Ŋ�̉c�Ə��܂Ō�A��������";
// MOD 2009.09.14 ���s�j���� �p�X���[�h�G���[���̖₢���킹��̕ύX END
						//Session.Clear();
					}
					else if(reader.GetString(0).Trim() == s����b�c)
					{
						sRet[0] = "����I��";
					}
					else
					{
						sRet[0] = "���͂��ꂽ���p�҂ł́A���̃Z�N�V�����́@\n"
								+ "���͐�͍폜�ł��܂���";
					}
				}
				else
				{
					sRet[0] = "���p�҂b�c �������� �p�X���[�h �Ɍ�肪����܂�";
					//Session.Clear();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				// ���p�҃}�X�^���X�V����
				if(sRet[0].Length != 4 || i�F�؃G���[�� != 0)
				{
					if (sRet[0].Length == 4){
						i�F�؃G���[�� = 0;
					}else if(i�F�؃G���[�� < 90){
						i�F�؃G���[��++;
					}else{
						i�F�؃G���[�� = 90;
					}

					// ���p�҃}�X�^�̍X�V
					sQuery
						= "UPDATE �b�l�O�S���p�� \n"
						+   " SET �F�؃G���[�� = " + i�F�؃G���[�� + ", \n"
						+       " �X�V����   = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
						+       " �X�V�o�f   = '" + "���O�C��" + "', \n"
						+       " �X�V��     = '" + s���p�b�c + "' \n"
						+ " WHERE ����b�c   = '" + s����b�c + "' \n"
						+   " AND ���p�҂b�c = '" + s���p�b�c + "' \n"
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}
// ADD 2006.12.22 ���s�j�����J ���͐�S���폜�p���O�C�� END
// ADD 2007.03.28 ���s�j���� �W�דX�擾�G���[�Ή� START
		/*********************************************************************
		 * ���X�擾
		 * �����F�ב��l�b�c
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X�����A�s���{���b�c�A�s�撬���b�c�A�厚�ʏ̂b�c
		 *
		 *********************************************************************/
		private static string GET_HATUTEN3_SELECT
			= "SELECT CM14.�X���b�c \n"
			+  " FROM �b�l�O�Q���� CM02 \n"
			+      ", �b�l�P�S�X�֔ԍ� CM14 \n"
//			+      ", �b�l�P�O�X�� CM10 \n"
			;

		[WebMethod]
		public String[] Get_hatuten3(string[] sUser, string sKcode, string sBcode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "���X�擾�R�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2]{"",""};

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				sbQuery.Append(GET_HATUTEN3_SELECT);
				sbQuery.Append(" WHERE CM02.����b�c = '" + sKcode + "' \n");
				sbQuery.Append(" AND CM02.����b�c = '" + sBcode + "' \n");
//				sbQuery.Append(" AND CM02.�폜�e�f = '0' \n");
				sbQuery.Append(" AND CM02.�X�֔ԍ� = CM14.�X�֔ԍ� \n");
//				sbQuery.Append(" AND CM14.�X���b�c = CM10.�X���b�c \n");
//				sbQuery.Append(" AND CM10.�폜�e�f = '0' \n";);

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				if(reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim();

					sRet[0] = "����I��";
				}
				else
				{
					sRet[0] = "���p�҂̏W�דX�擾�Ɏ��s���܂���";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2007.03.28 ���s�j���� �W�דX�擾�G���[�Ή� END
// ADD 2009.04.02 ���s�j���� �ғ����Ή� START
		/*********************************************************************
		 * �ғ����擾
		 * �����F���[�U�[���A�J�n���A�I�����i���P�����ȓ��j
		 * �ߒl�F����
		 *
		 *********************************************************************/
		private static string GET_KADOBI_SELECT
			= " SELECT CM07.�N����, CM07.�ғ����e�f \n"
			+ " FROM �b�l�O�V�ғ��� CM07 \n"
			;

		[WebMethod]
		public String[] Get_Kadobi(string[] sUser, string sDateStart, string sDateEnd)
		{
			logWriter(sUser, INF, "�ғ����擾�J�n");
			string[] sRet = new string[3];
			OracleConnection conn2 = null;

			int iCnt;
			for(iCnt = 0; iCnt < sRet.Length; iCnt++){
				sRet[iCnt] = "";
			}

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				OracleDataReader reader;

				sbQuery.Append(GET_KADOBI_SELECT);
				sbQuery.Append(" WHERE CM07.�N���� >= " + sDateStart + " \n");
				sbQuery.Append(  " AND CM07.�N���� <= " + sDateEnd + " \n");
				sbQuery.Append(  " AND CM07.�폜�e�f = '0' \n");
				sbQuery.Append(" ORDER BY CM07.�N���� \n");

				reader = CmdSelect(sUser, conn2, sbQuery);

				string s�J�n�� = "";
				string s�ғ����e�f   = "";
				string s�O�V�ғ����� = "";
				string s�P�S�ғ����� = "";
				iCnt = 1;
				while(reader.Read()){
					if(s�J�n��.Length == 0){
						s�J�n�� = reader.GetDecimal(0).ToString();
						if(!s�J�n��.Equals(sDateStart)) break;
					}
					s�ғ����e�f = reader.GetString(1);

					//�x���̎��̓J�E���g���Ȃ�
					if(s�ғ����e�f.Equals("1")) continue;

					//�ғ����Ƃ��̑��̓J�E���g����
					if(iCnt ==  7){
						s�O�V�ғ����� = reader.GetDecimal(0).ToString();
					}else if(iCnt == 14){
						s�P�S�ғ����� = reader.GetDecimal(0).ToString();
					}
					iCnt++;
				}

				disposeReader(reader);
				reader = null;
				sRet[0] = "����I��";
				sRet[1] = s�O�V�ғ�����;
				sRet[2] = s�P�S�ғ�����;
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
// ADD 2009.04.02 ���s�j���� �ғ����Ή� END
// ADD 2016.05.24 BEVAS�j���{ �Z�N�V�����ؑ։�ʉ��C�Ή� START
		/*********************************************************************
		 * ������擾�Q
		 * �����F����b�c�A����b�c�A���喼
		 * �ߒl�F�X�e�[�^�X�A����b�c�A���喼�A�o�ד��A�E�E�E
		 *********************************************************************/
		[WebMethod]
		public string[] Get_bumon2(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "������擾�Q�J�n");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[1];
			string sKey����b�c = sKey[0];
			string sKey����b�c = sKey[1];
			string sKey���喼 = sKey[2];

			//�c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			string cmdQuery  = "";
			try
			{
				//������̎擾
				cmdQuery = "SELECT '|' "
						+     " || TRIM(����b�c) || '|' "
						+     " || TRIM(���喼) || '|' "
						+     " || TRIM(�o�ד�) || '|' "
						+     " || TRIM(����b�c) || '|' \n"
						+  "  FROM �b�l�O�Q���� \n"
						+  " WHERE �폜�e�f = '0' \n"
						+  "   AND ����b�c = '" + sKey����b�c + "' \n";

				//���������F����b�c
				if(sKey[1].Length != 0)
				{
					cmdQuery += "   AND ����b�c LIKE '" + sKey����b�c + "%' \n";
				}

				//���������F���喼
				if(sKey[2].Length != 0)
				{
					cmdQuery += "   AND ���喼 LIKE '%" + sKey���喼 + "%' \n";
				}

				//�\�[�g���F����b�c�i�����j�A�g�D�b�c�i�����j�A�o�͏��i�����j
				cmdQuery += " ORDER BY ����b�c, �g�D�b�c, �o�͏� \n";

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
					sRet[0] = "�Y���f�[�^������܂���";
				}
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// ADD 2016.05.24 BEVAS�j���{ �Z�N�V�����ؑ։�ʉ��C�Ή� END
	}
}
