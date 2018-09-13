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
using WeiSha.Common;

using Song.ServiceInterfaces;
using Song.Entities;
using System.Collections.Generic;



namespace Song.Site.Manage.Student
{
    public partial class Students_Details : Extend.CustomPage
    {
        //ѧԱ���id��
        private string sts = WeiSha.Common.Request.QueryString["sts"].String;
        //Ա���ϴ����ϵ�����·��
        private string _uppath = "Student";
        //ѧԱ�б���
        List<Song.Entities.Accounts> accounts = new List<Accounts>();
        protected Song.Entities.Organization org;
        //����·����λ��
        string stamp = string.Empty;
        string positon = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            org = Business.Do<IOrganization>().OrganCurrent();
            if (!this.IsPostBack)
            {
                //���ڹ���
                WeiSha.Common.CustomConfig config = CustomConfig.Load(org.Org_Config);
                //�����Ĺ���
                stamp = config["Stamp"].Value.String;
                stamp = System.IO.File.Exists(Upload.Get["Org"].Physics + stamp) ? Upload.Get["Org"].Virtual + stamp : string.Empty;
                //������ʾλ��
                positon = config["StampPosition"].Value.String;
                if (string.IsNullOrEmpty(positon)) positon = "right-bottom";
                //ȡѧԱ�б�
                if (sts == "-1")
                {
                    Song.Entities.Accounts acc = Extend.LoginState.Accounts.CurrentUser;
                    if (acc != null) accounts.Add(acc);
                }
                else
                {
                    accounts = Business.Do<IAccounts>().AccountsCount(org.Org_ID, true, sts, -1);
                }
                foreach (Accounts acc in accounts)
                {
                    acc.Ac_Age = DateTime.Now.Year - acc.Ac_Age;
                    //������Ƭ
                    if (!string.IsNullOrEmpty(acc.Ac_Photo) && acc.Ac_Photo.Trim() != "")
                    {
                        acc.Ac_Photo = Upload.Get[_uppath].Virtual + acc.Ac_Photo;
                    }
                }
                //��
                rptAccounts.DataSource = accounts;
                rptAccounts.DataBind();
            }

        }
        /// <summary>
        /// ��ȡѧ�������ݿ��м�¼����ѧ�����
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected string getEdu(object val)
        {
            if (val != null)
            {
                ListItem li = ddlEducation.Items.FindByValue(val.ToString());
                if (li != null)
                {
                    return li.Text;
                }
            }
            return "";
        }
        /// <summary>
        /// ѧԱ���¼���������ʾѧϰ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptAccounts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //��ѧԱ�Ŀγ�ѧϰ��¼
                Song.Entities.Accounts acc = this.accounts[e.Item.ItemIndex];
                Repeater rtp = (Repeater)e.Item.FindControl("rtpLearnInfo");
                DataTable dt = Business.Do<IStudent>().StudentStudyCourseLog(org.Org_ID, acc.Ac_ID);
                if (dt != null)
                {
                    rtp.DataSource = dt;
                    rtp.DataBind();
                }
                //����
                Image img = (Image)e.Item.FindControl("imgStamp");
                img.Visible = !string.IsNullOrWhiteSpace(stamp);
                img.ImageUrl = stamp;
                img.CssClass = "stamp " + positon;
            }
        }

    }
}