using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;
using SharePointEmails.Core;

namespace SharepointEmails
{
    class SPAssociationControl:BaseFieldControl
    {
        const int NAME_COLUMN_INDEX = 2;
        const int ID_COLUMN_INDEX = 1;

        GridView grd_Asses;

        Button btn_Create;
        Button btn_Create_Hide;
        Button btn_Add;
        Button btn_Delete;

        Panel p_Create;
        TextBox Create_tb_Name;
        TextBox Create_tb_Desc;
        DropDownList Create_cb_AssType;

        TextBox Create_ById_tb_ItemId;
        DropDownList Create_ByGroup_cb_GroupType;

        CustomValidator cv_Create;
        ValidationSummary vs_Create;

        MultiView mv_Main;
        MultiView mv_CreateMain;

        View v_Displaying;
        View v_Editing;

        CustomValidator cv_General;
        ValidationSummary vs_Total;

        bool Edit
        {
            get { return ControlMode == SPControlMode.New || ControlMode == SPControlMode.Edit; }
        }

        protected override string DefaultTemplateName
        {
            get
            {
                return "SPAssociationEditTemplate";
            }
        }

        public override string DisplayTemplateName
        {
            get
            {
                return "SPAssociationEditTemplate";
            }
            set
            {
                base.DisplayTemplateName = value;
            }
        }

        public TemplateConfiguration Temp
        {
            get
            {
              //  return TempFromValue;
                try
                {
                    return TemplateConfiguration.Parse(SPContext.Current.Web.Properties["boo"] as string);
                }
                catch
                {
                    return new TemplateConfiguration();
                }
            }

            set
            {
               // TempFromValue = value;
                try
                {
                    if (value != null)
                        SPContext.Current.Web.Properties["boo"] = value.ToString();
                    else
                        SPContext.Current.Web.Properties.Remove("boo");
                    SPContext.Current.Web.AllowUnsafeUpdates = true;
                    SPContext.Current.Web.Properties.Update();
                    SPContext.Current.Web.AllowUnsafeUpdates = false;
                }
                catch { }
            }

        }

        public override object Value
        {
            get
            {
                //return base.Value;
                return Temp.ToString();
                //try
                //{
                //    return GetValue();
                //}
                //catch
                //{
                //}
                //return string.Empty;
            }
            set
            {
                base.Value = value;
                //TemplateConfiguration config = null;
                //try
                //{
                //    config = TemplateConfiguration.Parse(value as string);
                //}
                //catch
                //{
                //    config = new TemplateConfiguration();
                //}
                //Render(config);
            }
        }

        public TemplateConfiguration TempFromValue
        {
            get
            {
                try
                {
                    return TemplateConfiguration.Parse(Value as string);
                }
                catch
                {
                    return new TemplateConfiguration();
                }
            }

            set
            {
                Value = value.ToString();
            }
        }

        TemplateConfiguration FromItem
        {
            get
            {
                //return new TemplateConfiguration()
                //{
                //    Associations = new List<Association>
                //    {
                //        new IDAssociation
                //        {
                //            ID="idforitem1",
                //            ItemID=new Guid("{6790B5D7-4955-4F60-9589-967F6A9DBE7C}"),
                //            Name="ID Ass 1 Name",
                //            ItemName="Item1 Name"
                //        },
                //        new GroupAssociation
                //        {
                //            ID="idforitem2",
                //            Name="Group Ass 2 Name",
                //            ItemType=ItemType.AllDiscusionBoard

                //        }
                //    }
                //};
                return TemplateConfiguration.Parse(this.ItemFieldValue as string);
            }
        }

        void ShowError(CustomValidator validator, string message)
        {
            validator.IsValid = false;
            validator.ErrorMessage = message;
            validator.Validate();
        }

        Association FromCreatePanel()
        {
            EnsureChildControls();

            if (string.IsNullOrEmpty(Create_cb_AssType.Text))
            {
                ShowError(cv_Create,"No association type");
                return null;
            }
            if (string.IsNullOrEmpty(Create_tb_Name.Text))
            {
                ShowError(cv_Create,"No association name");
                return null;
            }

            var name = Create_tb_Name.Text;
            switch ((AssType)Convert.ToInt32(Create_cb_AssType.Text))
            {
                case AssType.ID:
                    {
                        return new IDAssociation
                        {
                            Name = name,
                            ItemID = new Guid(Create_ById_tb_ItemId.Text),
                            Description = Create_tb_Desc.Text
                        };
                    }

                case AssType.Group:
                    {

                        return new GroupAssociation
                        {
                            Name = name,
                            ItemType = (GroupType)Convert.ToInt32(Create_ByGroup_cb_GroupType.SelectedValue),
                            Description = Create_tb_Desc.Text
                        };
                    }
            }
            return null;
        }

        void Assign()
        {
            grd_Asses = (GridView)TemplateContainer.FindControl("grd_Asses");

            mv_Main = (MultiView)TemplateContainer.FindControl("mv_Main");
            v_Displaying = (View)TemplateContainer.FindControl("v_Displaying");
            v_Editing = (View)TemplateContainer.FindControl("v_Editing");
            mv_CreateMain = (MultiView)TemplateContainer.FindControl("mv_CreateMain");
            btn_Create = (Button)TemplateContainer.FindControl("btn_Create");
            btn_Create_Hide = (Button)TemplateContainer.FindControl("btn_Create_Hide");
            btn_Delete = (Button)TemplateContainer.FindControl("btn_Delete");
            btn_Add = (Button)TemplateContainer.FindControl("btn_Add");
            p_Create = (Panel)TemplateContainer.FindControl("p_Create");
            Create_tb_Name = (TextBox)TemplateContainer.FindControl("Create_tb_Name");
            Create_tb_Desc = (TextBox)TemplateContainer.FindControl("Create_tb_Desc");
            Create_cb_AssType = (DropDownList)TemplateContainer.FindControl("Create_cb_AssType");

            Create_ById_tb_ItemId = (TextBox)TemplateContainer.FindControl("Create_ById_tb_ItemId");
            Create_ByGroup_cb_GroupType = (DropDownList)TemplateContainer.FindControl("Create_ByGroup_cb_GroupType");

            cv_Create = (CustomValidator)TemplateContainer.FindControl("cv_Create");
            vs_Create = (ValidationSummary)TemplateContainer.FindControl("vs_Create");

            cv_General = (CustomValidator)TemplateContainer.FindControl("cv_General");
            vs_Total = (ValidationSummary)TemplateContainer.FindControl("vs_Total");
        }

        void InitControlEvents()
        {
            grd_Asses.SelectedIndexChanged += new EventHandler(grd_Asses_SelectedIndexChanged);
            Create_cb_AssType.SelectedIndexChanged += new EventHandler(cb_AssType_SelectedIndexChanged);
            btn_Add.Click += new EventHandler(btn_Add_Click);
            btn_Delete.Click += new EventHandler(btn_Delete_Click);
            btn_Create.Click += new EventHandler(btn_Create_Click);
            btn_Create_Hide.Click += new EventHandler(btn_Create_Hide_Click);
        }

        void ShowCreatePanel(bool visible)
        {
            p_Create.Visible = visible;
            btn_Create.Visible = !visible;
            btn_Create_Hide.Visible = visible;
        }

       

        void InitView()
        {
              mv_CreateMain.Visible = Edit;

            if (Edit)
            {
                mv_Main.SetActiveView(v_Editing);
            }
            else
            {
                mv_Main.SetActiveView(v_Displaying);
            }

            foreach (var association in Temp.Associations)
            {
                var sufix = association.ID.ToString();
                var panel = new Panel()
                {
                    ID = "panel_" + sufix,
                    Visible=false
                };

                if (Edit)
                {
                    var tb = new TextBox()
                    {
                        ID = "tbName_" + sufix,
                        Text = association.Name
                    };
                    panel.Controls.Add(tb);
                    v_Editing.Controls.Add(panel);
                }
                else
                {
                    var tb = new Label()
                    {
                        ID = "lblName_" + sufix,
                        Text = association.Name
                    };
                    v_Displaying.Controls.Add(panel);
                }
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Assign();
            InitControlEvents();
            InitView();
        }

        void btn_Delete_Click(object sender, EventArgs e)
        {
            if (grd_Asses.SelectedRow != null)
            {
                Guid g;
                var id = grd_Asses.SelectedRow.Cells[ID_COLUMN_INDEX].Text;
                if (!string.IsNullOrEmpty(id))
                {
                    DeleteAss(id);
                    var view = (Edit) ? v_Editing : v_Displaying;
                    foreach (Panel c in view.Controls.OfType<Panel>().ToList())
                    {
                        if (c.ID.ToLower().Contains(id.ToLower()))
                            view.Controls.Remove(c);
                    }
                }
            }
        }

        private void DeleteAss(string id)
        {
            var t = Temp;
            t.Associations.RemoveAll(p => p.ID == id);

            Temp = t; 


        }

        void btn_Add_Click(object sender, EventArgs e)
        {

            try
            {
                Page.Validate("CreateGroup");
                if (Page.IsValid)
                {
                    var ass = FromCreatePanel();
                    if (ass != null)
                    {
                        var t = Temp;
                        t.Associations.Add(ass);
                        Temp = t;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(cv_Create,ex.Message);
                Page.Validate("CreateGroup");
            }
        }

        void btn_Create_Hide_Click(object sender, EventArgs e)
        {
            ShowCreatePanel(false);
        }

        void btn_Create_Click(object sender, EventArgs e)
        {
            ShowCreatePanel(true);
        }

        void cb_AssType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Create_cb_AssType.SelectedValue))
            {
                var type = (AssType)Convert.ToInt32(Create_cb_AssType.SelectedValue);
                switch (type)
                {
                    case AssType.ID: mv_CreateMain.SetActiveView(mv_CreateMain.Views[0]);break;
                    case AssType.Group: mv_CreateMain.SetActiveView(mv_CreateMain.Views[1]);break;
                }
            }
        }

        void grd_Asses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (grd_Asses.SelectedRow != null)
            {
                string id = string.Empty;
                id = grd_Asses.SelectedRow.Cells[ID_COLUMN_INDEX].Text;
                if (!string.IsNullOrEmpty(id))
                {
                    var view = (Edit) ? v_Editing : v_Displaying;
                    foreach (Panel c in view.Controls.OfType<Panel>())
                    {
                        c.Visible = c.ID.ToLower().Contains(id.ToLower().Trim('{','}'));
                    }
                }
            }
        }        

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            grd_Asses.DataSource = Temp.Associations.Select(p => new AssInfo
            {
                ID = p.ID,
                Name=p.Name,
                Type=p.Type.ToString()
            });
            grd_Asses.DataBind();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!this.Page.IsPostBack)
                Temp = FromItem;
        }
    }

    class AssInfo
    {
        public string Name { set; get; }
        public string Type { set; get; }
        public string ID { set; get; }
    }
}
