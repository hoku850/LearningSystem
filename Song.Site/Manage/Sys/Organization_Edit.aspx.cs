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
using WeiSha.WebControl;

namespace Song.Site.Manage.Sys
{
    public partial class Organization_Edit : Extend.CustomPage
    {
        private int id = WeiSha.Common.Request.QueryString["id"].Decrypt().Int32 ?? 0;
        //��������
        private string type = WeiSha.Common.Request.QueryString["type"].String;
        //��������վ������
        private string site = WeiSha.Common.Request.QueryString["site"].String;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                init();
                fill();
            }
        }
        private void init()
        {
            Song.Entities.OrganLevel[] orglv = Business.Do<IOrganization>().LevelAll(true);
            ddlLevel.DataSource = orglv;
            ddlLevel.DataTextField = "olv_name";
            ddlLevel.DataValueField = "olv_id";
            ddlLevel.DataBind();
            //������
            lbDomain.Text = WeiSha.Common.Server.MainName;
        }
        /// <summary>
        /// ���ó�ʼ����
        /// </summary>
        private void fill()
        {
            Song.Entities.Organization org;
            if (id != 0)
            {
                org = Business.Do<IOrganization>().OrganSingle(id);
                cbIsUse.Checked = org.Org_IsUse;
            }
            else 
            {
                org = new Song.Entities.Organization();
            }
            //ƽ̨����
            Org_PlatformName.Text = org.Org_PlatformName;
            //��������
            tbName.Text = org.Org_Name;
            tbAbbrName.Text = org.Org_AbbrName;
            //Ӣ������
            tbEnName.Text = org.Org_EnName;
            tbAbbrEnName.Text = org.Org_AbbrEnName;
            //�ϼ�����
            //��ַ,������Ϣ����γ�ȣ�
            tbAddress.Text = org.Org_Address;
            tbLng.Text = org.Org_Longitude;
            tbLat.Text = org.Org_Latitude;
            //�绰
            tbPhone.Text = org.Org_Phone;
            //����
            tbFax.Text = org.Org_Fax;
            //�ʱ�
            tbZip.Text = org.Org_Zip;
            //������Ϣ
            tbMail.Text = org.Org_Email;
            //��ϵ������ϵ�˵绰
            tbLinkman.Text = org.Org_Linkman;
            tbLinkmanPhone.Text = org.Org_LinkmanPhone;
            //��ҵ΢��
            tbWeixin.Text = org.Org_Weixin;
            //���ڻ����ȼ�
            ListItem liLv = ddlLevel.Items.FindByValue(org.Olv_ID.ToString());
            if (liLv != null)
            { liLv.Selected = true; }
            else
            {
                Song.Entities.OrganLevel lv = Business.Do<IOrganization>().LevelDefault();
                if (lv != null)
                {
                    liLv = ddlLevel.Items.FindByValue(lv.Olv_ID.ToString());
                    if (liLv != null) liLv.Selected = true;
                }
            }
            //����
            tbDomain.Text = org.Org_TwoDomain;
            tbTemplate.Text = org.Org_Template;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnEnter_Click(object sender, EventArgs e)
        {
            Song.Entities.Organization org;
            try
            {
                if (id!=0)
                {
                    org = Business.Do<IOrganization>().OrganSingle(id);
                }
                else org = new Song.Entities.Organization();
                //ƽ̨����
                org.Org_PlatformName = Org_PlatformName.Text.Trim();
                //��������
                org.Org_Name = tbName.Text.Trim();
                org.Org_AbbrName = tbAbbrName.Text.Trim();
                //Ӣ������
                org.Org_EnName = tbEnName.Text.Trim();
                org.Org_AbbrEnName = tbAbbrEnName.Text.Trim();
                //��ַ
                org.Org_Address = tbAddress.Text.Trim();
                org.Org_Longitude = tbLng.Text.Trim();
                org.Org_Latitude = tbLat.Text.Trim();
                //�绰
                org.Org_Phone = tbPhone.Text.Trim();
                //����
                org.Org_Fax = tbFax.Text.Trim();
                //�ʱ�
                org.Org_Zip = tbZip.Text.Trim();
                //������Ϣ
                org.Org_Email = tbMail.Text.Trim();
                //��ϵ������ϵ�˵绰
                org.Org_Linkman = tbLinkman.Text.Trim();
                org.Org_LinkmanPhone = tbLinkmanPhone.Text.Trim();
                //��ҵ΢��
                org.Org_Weixin = tbWeixin.Text.Trim();
                //���ڵȼ�
                org.Olv_ID = Convert.ToInt32(ddlLevel.SelectedValue);
                org.Olv_Name = ddlLevel.SelectedItem.Text;
                //�Ƿ�����
                org.Org_IsUse = cbIsUse.Checked;
                //����
                org.Org_TwoDomain = tbDomain.Text.Trim();
                org.Org_Template = tbTemplate.Text.Trim();
                if (id != 0)
                {
                    Business.Do<IOrganization>().OrganSave(org);
                }
                else
                {
                    Business.Do<IOrganization>().OrganAdd(org);
                }            
                Master.AlertCloseAndRefresh("�����ɹ���");
            }
            catch (Exception ex)
            {
                Master.Alert(ex.Message);
            }
        }
    }
}
