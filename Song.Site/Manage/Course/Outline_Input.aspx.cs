using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;
using WeiSha.Common;
using Song.ServiceInterfaces;
using Song.Entities;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml;

namespace Song.Site.Manage.Course
{
    public partial class Outline_Input : Extend.CustomPage
    {
        //����get�����Ŀγ�id
        int couid_get = WeiSha.Common.Request.QueryString["couid"].Int32 ?? 0;
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void ExcelInput1_OnInput(object sender, EventArgs e)
        {
            //xml�����ļ�
            XmlDocument xmldoc = new XmlDocument();
            string confing = WeiSha.Common.App.Get["ExcelInputConfig"].VirtualPath + ExcelInput1.Config;
            xmldoc.Load(WeiSha.Common.Server.MapPath(confing));
            XmlNodeList nodes = xmldoc.GetElementsByTagName("item");
            //�������е�����
            DataTable dt = ExcelInput1.SheetDataTable;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    //���������е������ݿ�
                    _inputData(dt.Rows[i], nodes);                  
                }
                catch(Exception ex)
                {
                    //��������������з��ظ��ؼ�
                    ExcelInput1.AddError(dt.Rows[i]);
                }
            }
            Business.Do<IOutline>().OnSave(null, EventArgs.Empty);
        }
        

        #region ��������
        /// <summary>
        /// ��ĳһ�����ݼ��뵽���ݿ�
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="dl"></param>
        private void _inputData(DataRow dr,XmlNodeList nodes)
        {
            Song.Entities.Outline obj = new Song.Entities.Outline();
            foreach (KeyValuePair<String, String> rel in ExcelInput1.DataRelation)
            {               
                //Excel���е�ֵ
                string column = dr[rel.Key].ToString();
                //���ݿ��ֶε�����
                string field = rel.Value;
                if (field == "Ol_ID")
                {
                    if (string.IsNullOrEmpty(column) || column.Trim() == "") continue;
                    int id = Convert.ToInt32(column);
                    Song.Entities.Outline isHavObj = Business.Do<IOutline>().OutlineSingle(id);
                    if (isHavObj != null) obj = isHavObj;
                }
                //�½�����
                if (field == "�½�����")
                {
                    if (string.IsNullOrWhiteSpace(column)|| column.Trim()=="") return;
                    obj.Ol_Name = column.Trim();
                    Song.Entities.Outline isHavObj = Business.Do<IOutline>().OutlineSingle(obj.Ol_Name);
                    if (isHavObj != null) obj = isHavObj;
                }
                PropertyInfo[] properties = obj.GetType().GetProperties();
                for (int j = 0; j < properties.Length; j++)
                {
                    PropertyInfo pi = properties[j];
                    if (field == pi.Name && !string.IsNullOrEmpty(column))
                    {
                        object val = getValue(field, column, nodes);
                        if (val == null) continue;
                        pi.SetValue(obj, Convert.ChangeType(val, pi.PropertyType), null);
                    }
                }  
            }
            obj.Cou_ID = couid_get;
            Song.Entities.Course cur = Business.Do<ICourse>().CourseSingle(couid_get);
            obj.Sbj_ID = cur.Sbj_ID;
            obj.Org_ID = cur.Org_ID;
            Business.Do<IOutline>().OutlineInput(obj); 
        }
        /// <summary>
        /// ��ȡ�����ֵ
        /// </summary>
        /// <param name="field">�ֶ�����</param>
        /// <param name="val">�ֶζ�Ӧ��ֵ</param>
        /// <param name="nodes">�����������xml����</param>
        /// <returns></returns>
        private object getValue(string field, string val, XmlNodeList nodes)
        {
            object obj = null;
            for (int j = 0; j < nodes.Count; j++)
            {
                string _field = nodes[j].Attributes["Field"] != null ? nodes[j].Attributes["Field"].Value : "";
                if (field != _field) continue;
                //Ĭ�ϵĶ�Ӧ��ϵ
                string _defvalue = nodes[j].Attributes["DefautValue"] != null ? nodes[j].Attributes["DefautValue"].Value : "";
                if (string.IsNullOrWhiteSpace(_defvalue))
                {
                    obj = val;
                }
                else
                {
                    foreach (string s in _defvalue.Split('|'))
                    {
                        string h = s.Substring(0, s.IndexOf("="));
                        string f = s.Substring(s.LastIndexOf("=") + 1);
                        if (val == f) obj = h;
                    }
                }
            }
            return obj;
        }
        #endregion
      
    }
}
