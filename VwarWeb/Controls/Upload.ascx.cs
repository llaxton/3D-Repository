﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using vwarDAL;
using System.Collections.Generic;
using Telerik.Web.UI;

public partial class Controls_Upload : Website.Pages.ControlBase
{


    private Utility_3D.ConvertedModel mModel;

    public void SetModel(Utility_3D.ConvertedModel inModel)
    {
        mModel = inModel;

        Context.Session[MODELKEY] = inModel;

        Session[MODELKEY] = inModel;
    }
    public static readonly string MODELKEY = "Model";
    public Utility_3D.ConvertedModel GetModel()
    {
        mModel = (Utility_3D.ConvertedModel)Session[MODELKEY];
        return mModel;
    }

    protected bool IsNew
    {
        get
        {

            return string.IsNullOrEmpty(this.ContentObjectID);


        }

    }

    private bool IsModelUpload
    {
        get
        {
            return ddlAssetType.SelectedValue.Equals("Model", StringComparison.InvariantCultureIgnoreCase);
        }
    }
    private const string FEDORACONTENTOBJECT = "FedoraContentObject";
    private ContentObject FedoraContentObject
    {
        get
        {
            return DAL.GetContentObjectById(ContentObjectID,false,false);
        }


    }

    protected string ContentObjectID
    {
        get
        {
            string rv = "";
            if (Request.QueryString["ContentObjectID"] != null)
            {
                rv = Request.QueryString["ContentObjectID"].Trim();
            }
            else if (ViewState["ContentObjectID"] != null)
            {
                rv = ViewState["ContentObjectID"].ToString();
            }


            return rv;
        }
        set { ViewState["ContentObjectID"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        //RadAjaxManager manager = RadAjaxManager.GetCurrent(this.Page);

        //manager.AjaxSettings.AddAjaxSetting(manager, this.ThumbnailFileImage);
        //manager.AjaxSettings.AddAjaxSetting(manager, this.DeveloperLogoImage);
        //manager.AjaxSettings.AddAjaxSetting(manager, this.SponsorLogoImage);


        //manager.AjaxSettings.AddAjaxSetting(manager, UnitScaleTextBox);

        ///*manager.AjaxSettings.AddAjaxSetting(manager, UpAxisRadioButtonList);        
        //manager.AjaxSettings.AddAjaxSetting(manager, NumPolygonsTextBox);
        //manager.AjaxSettings.AddAjaxSetting(manager, NumTexturesTextBox);
        //manager.AjaxSettings.AddAjaxSetting(manager, UVCoordinateChannelTextBox);*/
        //manager.AjaxRequest += new RadAjaxControl.AjaxRequestDelegate(manager_AjaxRequest);

        //maintain scroll position


        if (this.Page.Master.FindControl("SearchPanel") != null)
        {
            //hide the search panel
            this.Page.Master.FindControl("SearchPanel").Visible = false;
        }

        //redirect if user is not authenticated
        if (!Context.User.Identity.IsAuthenticated)
        {
            Response.Redirect(Website.Pages.Types.Default);
        }

        if (!Page.IsPostBack)
        {


            this.MultiView1.ActiveViewIndex = 0;
            this.BindCCLHyperLink();
            this.BindContentObject();

            ContentFileUploadRequiredFieldValidator.Enabled = IsNew;

        }



    }

    private void BindContentObject()
    {


        if (!this.IsNew)
        {

            if (this.FedoraContentObject != null)
            {


                if (!string.IsNullOrEmpty(this.FedoraContentObject.DisplayFile))
                {
                    this.ContentFileUploadRequiredFieldValidator.Enabled = false;
                }


               // this.ThumbnailFileUploadRequiredFieldValidator.Enabled = false;

                this.BindThumbnail();

                //redirect if the user is not the owner
                if (!this.FedoraContentObject.SubmitterEmail.Equals(Context.User.Identity.Name, StringComparison.InvariantCultureIgnoreCase) & !Website.Security.IsAdministrator())
                {
                    Response.Redirect(Website.Pages.Types.Default);
                    return;
                }

                //Asset Type
                if (!string.IsNullOrEmpty(this.FedoraContentObject.AssetType))
                {
                    //set selected
                    if (this.ddlAssetType.SelectedItem != null)
                    {
                        this.ddlAssetType.ClearSelection();
                    }

                    this.ddlAssetType.Items.FindItemByValue(this.FedoraContentObject.AssetType.Trim()).Selected = true;

                }


                //Title
                if (!string.IsNullOrEmpty(this.FedoraContentObject.Title))
                {
                    this.TitleTextBox.Text = this.FedoraContentObject.Title.Trim();
                }


                //Developer name
                if (!string.IsNullOrEmpty(this.FedoraContentObject.DeveloperName))
                {
                    this.DeveloperNameTextBox.Text = this.FedoraContentObject.DeveloperName.Trim();
                }



                //Sponsor Name
                if (!string.IsNullOrEmpty(this.FedoraContentObject.SponsorName))
                {
                    this.SponsorNameTextBox.Text = this.FedoraContentObject.SponsorName.Trim();
                }

                //Artist Name
                if (!string.IsNullOrEmpty(this.FedoraContentObject.ArtistName))
                {
                    this.ArtistNameTextBox.Text = this.FedoraContentObject.ArtistName.Trim();
                }


                //CC License
                if (!string.IsNullOrEmpty(this.FedoraContentObject.CreativeCommonsLicenseURL))
                {


                    //set selected
                    if (this.CCLicenseDropDownList.Items.FindItemByValue(this.FedoraContentObject.CreativeCommonsLicenseURL) != null)
                    {
                        //clear selection
                        if (this.CCLicenseDropDownList.SelectedItem != null)
                        {
                            this.CCLicenseDropDownList.ClearSelection();
                        }

                        this.CCLicenseDropDownList.Items.FindItemByValue(this.FedoraContentObject.CreativeCommonsLicenseURL).Selected = true;
                    }

                    //set the hyperlink
                    this.CCLHyperLink.NavigateUrl = this.FedoraContentObject.CreativeCommonsLicenseURL.Trim();
                }
                else
                {
                    //set none selected
                    if (this.CCLicenseDropDownList.SelectedItem != null)
                    {
                        this.CCLicenseDropDownList.ClearSelection();
                    }

                    this.CCLicenseDropDownList.Items.Last().Selected = true;


                }



                //Description
                if (!string.IsNullOrEmpty(this.FedoraContentObject.Description))
                {
                    this.DescriptionTextBox.Text = this.FedoraContentObject.Description.Trim();

                }

                //More Information
                if (!string.IsNullOrEmpty(this.FedoraContentObject.MoreInformationURL))
                {
                    this.MoreInformationURLTextBox.Text = this.FedoraContentObject.MoreInformationURL.Trim();

                }

                //Keywords
                if (!string.IsNullOrEmpty(this.FedoraContentObject.Keywords))
                {
                    char[] delimiters = new char[] { ',' };
                    string[] words = this.FedoraContentObject.Keywords.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in words)
                    {
                        this.KeywordsListBox.Items.Add(new ListItem(s, s));
                    }

                }


                //Unit Scale
                if (!string.IsNullOrEmpty(this.FedoraContentObject.UnitScale))
                {


                    this.UnitScaleTextBox.Text = this.FedoraContentObject.UnitScale.Trim();
                }

                //Up Axis
                if (!string.IsNullOrEmpty(this.FedoraContentObject.UpAxis))
                {
                    this.UpAxisRadioButtonList.ClearSelection();
                    UpAxisRadioButtonList.SelectedValue = this.FedoraContentObject.UpAxis;
                }

                //NumPolygons
                if (!string.IsNullOrEmpty(this.FedoraContentObject.NumPolygons.ToString()))
                {
                    this.NumPolygonsTextBox.Text = this.FedoraContentObject.NumPolygons.ToString();
                }

                //NumTextures
                if (!string.IsNullOrEmpty(this.FedoraContentObject.NumTextures.ToString()))
                {
                    this.NumTexturesTextBox.Text = this.FedoraContentObject.NumTextures.ToString();
                }

                //UV Coordinate Channel
                if (!string.IsNullOrEmpty(this.FedoraContentObject.UVCoordinateChannel))
                {
                    this.UVCoordinateChannelTextBox.Text = this.FedoraContentObject.UVCoordinateChannel.Trim();
                }



            }
            else
            {
                //Show error message
                this.errorMessage.Text = "Model not found.";
            }


        }



        //bind developer logo
        this.BindDeveloperLogo();

        //bind sponsor logo
        this.BindSponsorLogo();


    }

    protected void Step1NextButton_Click(object sender, EventArgs e)
    {

        //update
        if (IsModelUpload)
        {

            vwarDAL.IDataRepository dal = DAL;


            if (this.IsNew)
            {
                //create new & add to session       
                ContentObject co = new ContentObject();
                co.Title = this.TitleTextBox.Text.Trim();
                dal.InsertContentObject(co);
                this.ContentObjectID = co.PID;

            }
            else
            {
                FedoraContentObject.Title = TitleTextBox.Text.Trim();
            }


            if (this.FedoraContentObject != null)
            {

                //asset type                       
                this.FedoraContentObject.AssetType = this.ddlAssetType.SelectedValue;

                //model upload
                Utility_3D.ConvertedModel model = null;
                if (this.ContentFileUpload.HasFile)
                {
                    Utility_3D.Model_Packager pack = new Utility_3D.Model_Packager();
                    Utility_3D _3d = new Utility_3D();
                    _3d.Initialize(Website.Config.ConversionLibarayLocation);

                    string UploadedFilename = this.ContentFileUpload.PostedFile.FileName;
                    if (Path.GetExtension(UploadedFilename) != ".zip")
                        UploadedFilename = Path.ChangeExtension(UploadedFilename, ".zip");

                    try
                    {
                        model = pack.Convert(this.ContentFileUpload.PostedFile.InputStream, this.ContentFileUpload.PostedFile.FileName);
                        SetModel(model);
                    }
                    catch
                    {
                        //setup default models properties for an unconverted model
                        model = new Utility_3D.ConvertedModel();
                        model._ModelData = new Utility_3D.Parser.ModelData();
                        model._ModelData.TextureMappings = new Dictionary<string, string>() { };
                        model._ModelData.TransformProperties = new Utility_3D.Parser.ModelTransformProperties();
                        model._ModelData.TransformProperties.UnitMeters = 1;
                        model._ModelData.TransformProperties.UnitName = "meters";
                        model._ModelData.TransformProperties.UpAxis = "Z";

                        model._ModelData.ReferencedTextures = new string[0];
                        model.missingTextures = new List<string>() { };
                        model.textureFiles = new List<string>() { };
                        model.type = "UNKNOWN";

                        //read the upload stream strait into the model file
                        model.data = new Byte[this.ContentFileUpload.PostedFile.InputStream.Length];
                        this.ContentFileUpload.PostedFile.InputStream.Read(model.data, 0, (int)this.ContentFileUpload.PostedFile.InputStream.Length);
                        this.ContentFileUpload.PostedFile.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                        SetModel(model);

                    }


                    var displayFilePath = "";
                    FedoraContentObject.Location = UploadedFilename;
                    FedoraContentObject.DisplayFileId =
                    dal.UploadFile(model.data, FedoraContentObject.PID, UploadedFilename);
                    if (model.type != "UNKNOWN")
                    {
                        string destPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                        var tempfile = destPath + ".zip";// ExtractFile(model.data, Path.GetFileNameWithoutExtension(this.ContentFileUpload.PostedFile.FileName));
                        System.IO.FileStream savefile = new FileStream(tempfile, FileMode.CreateNew);
                        byte[] filedata = new Byte[model.data.Length];
                        model.data.CopyTo(filedata, 0);
                        savefile.Write(model.data, 0, (int)model.data.Length);
                        savefile.Close();

                        string converterdtempfile = ConvertFileToO3D(tempfile);
                        FedoraContentObject.DisplayFile = UploadedFilename.Replace("zip", "o3d");


                        displayFilePath = FedoraContentObject.DisplayFile;

                        FedoraContentObject.DisplayFile = Path.GetFileName(FedoraContentObject.DisplayFile);
                        FedoraContentObject.DisplayFileId = dal.UploadFile(converterdtempfile, FedoraContentObject.PID, FedoraContentObject.DisplayFile);


                    }
                    else
                    {
                        FedoraContentObject.DisplayFile = string.Empty;

                    }


                    FedoraContentObject.UnitScale = model._ModelData.TransformProperties.UnitMeters.ToString();
                    FedoraContentObject.UpAxis = model._ModelData.TransformProperties.UpAxis;
                    FedoraContentObject.NumPolygons = model._ModelData.VertexCount.Polys;
                    FedoraContentObject.NumTextures = model._ModelData.ReferencedTextures.Length;

                    PopulateValidationViewMetadata(FedoraContentObject);

                }


                //upload thumbnail 
                if (this.ThumbnailFileUpload.HasFile)
                {
                    this.FedoraContentObject.ScreenShot = this.ThumbnailFileUpload.PostedFile.FileName;

                    int length = (int)this.ThumbnailFileUpload.PostedFile.InputStream.Length;
                    byte[] data = new byte[length];

                    using (Stream stream = this.ThumbnailFileUpload.PostedFile.InputStream)
                    {
                        stream.Read(data, 0, length);
                    }

                    //set screenshot
                    FedoraContentObject.ScreenShotId = dal.UploadFile(data, this.FedoraContentObject.PID, this.ThumbnailFileUpload.PostedFile.FileName);

                }



                //creative commons license url
                if (this.CCLicenseDropDownList.SelectedItem != null && this.CCLicenseDropDownList.SelectedValue != "None")
                {
                    this.FedoraContentObject.CreativeCommonsLicenseURL = this.CCLicenseDropDownList.SelectedValue.Trim();

                }

                //developer logo
                this.UploadDeveloperLogo(dal, this.FedoraContentObject);

                //developer name
                if (!string.IsNullOrEmpty(this.DeveloperNameTextBox.Text))
                {
                    this.FedoraContentObject.DeveloperName = this.DeveloperNameTextBox.Text.Trim();
                }

                //sponsor logo
                this.UploadSponsorLogo(dal, this.FedoraContentObject);


                //sponsor name
                if (!string.IsNullOrEmpty(this.SponsorNameTextBox.Text))
                {
                    this.FedoraContentObject.SponsorName = this.SponsorNameTextBox.Text.Trim();
                }

                //artist name
                if (!string.IsNullOrEmpty(this.ArtistNameTextBox.Text))
                {
                    this.FedoraContentObject.ArtistName = this.ArtistNameTextBox.Text.Trim();
                }

                //format
                if (!string.IsNullOrEmpty(this.FormatTextBox.Text))
                {
                    this.FedoraContentObject.Format = this.FormatTextBox.Text.Trim();
                }


                //description
                if (!string.IsNullOrEmpty(this.DescriptionTextBox.Text))
                {
                    this.FedoraContentObject.Description = this.DescriptionTextBox.Text.Trim();
                }

                //more information url
                if (!string.IsNullOrEmpty(this.MoreInformationURLTextBox.Text))
                {
                    this.FedoraContentObject.MoreInformationURL = this.MoreInformationURLTextBox.Text.Trim();

                }

                //keywords
                string words = "";
                int count = 0;
                foreach (ListItem li in this.KeywordsListBox.Items)
                {
                    count += 1;

                    if (count > 1)
                    {
                        words += "," + li.Text.Trim();
                    }
                    else
                    {
                        words = li.Text;
                    }


                    count++;
                }


                this.FedoraContentObject.Keywords = words;





            }


            FedoraContentObject.UploadedDate = DateTime.Now;
            FedoraContentObject.LastModified = DateTime.Now;
            FedoraContentObject.Views = 0;
            FedoraContentObject.SubmitterEmail = Context.User.Identity.Name.Trim();

            dal.UpdateContentObject(FedoraContentObject);

            this.PopulateValidationViewMetadata(FedoraContentObject);
            this.MultiView1.SetActiveView(this.ValidationView);
            var admins = UserProfileDB.GetAllAdministrativeUsers();
            foreach (DataRow row in admins.Rows)
            {

                var url = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, this.ResolveUrl(Website.Pages.Types.FormatModel(this.ContentObjectID)));
                Website.Mail.SendSingleMessage(url, row["Email"].ToString(), "New Model Uploaded", Context.User.Identity.Name, Context.User.Identity.Name, "", "", false, "");
            }
            SetModelDisplay(!IsNew);
        }

    }

    protected void PopulateValidationViewMetadata(ContentObject co)
    {


        UnitScaleTextBox.Text = co.UnitScale;
        try
        {
            UpAxisRadioButtonList.SelectedValue = co.UpAxis;
        }
        catch { }
        NumPolygonsTextBox.Text = co.NumPolygons.ToString();
        NumTexturesTextBox.Text = co.NumTextures.ToString();
        UVCoordinateChannelTextBox.Text = "1";



    }




    protected void ValidationViewSubmitButton_Click(object sender, EventArgs e)
    {

        if (this.FedoraContentObject != null)
        {

            vwarDAL.IDataRepository dal = DAL;


            if (!string.IsNullOrEmpty(this.UnitScaleTextBox.Text))
            {
                this.FedoraContentObject.UnitScale = this.UnitScaleTextBox.Text.Trim();
            }

            this.FedoraContentObject.UpAxis = this.UpAxisRadioButtonList.SelectedValue.Trim();

            //polygons
            if (!string.IsNullOrEmpty(this.NumPolygonsTextBox.Text.Trim()))
            {

                try
                {
                    this.FedoraContentObject.NumPolygons = Int32.Parse(this.NumPolygonsTextBox.Text.Trim());
                }
                catch
                {


                }


            }


            dal.UpdateContentObject(this.FedoraContentObject);



        }


        //redirect
        Response.Redirect(Website.Pages.Types.FormatModel(this.ContentObjectID));

    }

    private string ConvertFileToO3D(string path)
    {
        var application = Path.Combine(Path.Combine(Request.PhysicalApplicationPath, "bin"), "o3dConverter.exe");
        System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo(application);
        processInfo.Arguments = String.Format("\"{0}\" \"{1}\"", path, path.ToLower().Replace("zip", "o3d"));
        processInfo.WindowStyle = ProcessWindowStyle.Hidden;
        processInfo.RedirectStandardError = true;
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        var p = Process.Start(processInfo);
        var error = p.StandardError.ReadToEnd();
        return path.ToLower().Replace("zip", "o3d");
    }

    private void ConvertToVastpark(string path)
    {
        var application = Path.Combine(Path.Combine(Request.PhysicalApplicationPath, "bin"), "ModelPackager.exe");
        System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo(application);
        processInfo.Arguments = path;
        processInfo.WindowStyle = ProcessWindowStyle.Hidden;
        processInfo.RedirectStandardError = true;
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        var p = Process.Start(processInfo);
        var error = p.StandardError.ReadToEnd();
    }

    private void SetModelDisplay(bool displayAlways = false)
    {
        if ((GetModel() != null && GetModel().type != "UNKNOWN")|| displayAlways)
        {
            string proxyTemplate = "Model.ashx?pid={0}&file=";
            HtmlGenericControl body = this.Page.Master.FindControl("bodyTag") as HtmlGenericControl;
            var uri = Request.Url;
            //var url = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            var url = String.Format(proxyTemplate, this.FedoraContentObject.PID);//, this.FedoraContentObject.Location);//.Replace("&", "_Amp_") + "&UpAxis=" + this.FedoraContentObject.UpAxis + "&UnitScale=" + this.FedoraContentObject.UnitScale.ToString() + "&ContentObjectID=" + this.FedoraContentObject.PID;
            //url += String.Format("VwarWeb/Public/Model.ashx?pid={0}&file={1}", FedoraContentObject.PID, FedoraContentObject.Location);
            ContentObject co = this.FedoraContentObject;

            string script = string.Format("var vLoader = new ViewerLoader('{0}', '{1}', '{2}', '{3}', '{4}');", url,
                                                                                                                      co.Location+"&AllowScreenshotButton=true",
                                                                                                                      co.DisplayFile,
                                                                                                                      (co.UpAxis != null && co.UpAxis != "?") ? co.UpAxis : "",
                                                                                                                      (co.UnitScale != null) ? co.UnitScale : "");

            script += "vLoader.LoadViewer();";
            Page.ClientScript.RegisterStartupScript(GetType(), "loadViewer", script, true );
            //body.Attributes.Add("onLoad", "DoLoadURL('" + url + "');");
        }
    }

    private string ExtractFile(byte[] data, string destination)
    {
        string destPath = Path.Combine(Path.GetTempPath(), destination);
        using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(data))
        {
            zipFile.ExtractAll(destPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
        }
        return destPath;
    }

    protected void ddlAssetType_Changed(object sender, EventArgs e)
    {
        thumbNailArea.Visible = IsModelUpload;
    }

    protected void MissingTextureViewBackButton_Click(object sender, EventArgs e)
    {
        this.MultiView1.SetActiveView(this.DefaultView);
    }

    private void GenerateMissingTextureUI(Utility_3D.ConvertedModel model)
    {
        foreach (var texture in model.missingTextures)
        {
            var textureDialog = new Controls_MissingTextures { OldFile = texture };
            MissingTextureArea.Controls.Add(textureDialog);
        }
    }

    protected void MissingTextureViewNextButton_Click(object sender, EventArgs e)
    {
        //get a reference to the model
        Utility_3D.ConvertedModel model = GetModel();
        foreach (Control c in MissingTextureArea.Controls)
        {
            if (c is Controls_MissingTextures)
            {
                //loop over each dialog and find out if the user uploaded a file
                Utility_3D.Model_Packager pack = new Utility_3D.Model_Packager();
                var uploadfile = (Controls_MissingTextures)c;
                //if they uploaded a file, push the file and model into the dll which will add the file
                if (uploadfile.FileName != "")
                    pack.AddTextureToModel(ref model, uploadfile.FileContent, uploadfile.FileName);
            }
        }
        //save the changes to the model
        SetModel(model);
        //Get the DAL

        vwarDAL.IDataRepository dal = DAL;

        //Get the content object for this model
        ContentObject contentObj = dal.GetContentObjectById(ContentObjectID, false);

        //upload the modified datastream to the dal
        dal.UpdateFile(model.data, contentObj.PID, contentObj.Location);

        this.MultiView1.SetActiveView(this.ValidationView);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(Website.Pages.Types.Default);
    }

    protected void MissingTextureViewCancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(Website.Pages.Types.Default);
    }

    protected void SkipStepButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(Website.Pages.Types.FormatModel(this.ContentObjectID));
    }

    protected void CCLicenseDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.BindCCLHyperLink();
    }

    private void BindCCLHyperLink()
    {

        if (this.CCLicenseDropDownList.SelectedItem != null)
        {
            this.CCLHyperLink.NavigateUrl = string.Empty;
            this.CCLHyperLink.Visible = false;

            if (!string.IsNullOrEmpty(this.CCLicenseDropDownList.SelectedValue))
            {
                this.CCLHyperLink.NavigateUrl = this.CCLicenseDropDownList.SelectedValue.Trim();
                this.CCLHyperLink.Visible = true;

            }

        }




    }

    protected void AddKeywordButton_Click(object sender, EventArgs e)
    {
        //NOTE: actually a combobox
        if (!string.IsNullOrEmpty(this.KeywordsTextBox.Text))
        {

            if (this.KeywordsListBox.Items.FindByText(this.KeywordsTextBox.Text.Trim()) == null)
            {
                this.KeywordsListBox.Items.Add(this.KeywordsTextBox.Text.Trim());

            }


        }
    }

    protected void RemoveKeywordsButton_Click(object sender, EventArgs e)
    {
        List<ListItem> selectedItems = new List<ListItem>();

        //get all selected items add to collection
        foreach (ListItem li in this.KeywordsListBox.Items)
        {
            //add to new collection
            if (li.Selected)
            {
                selectedItems.Add(li);
            }
        }

        //remove selected items
        foreach (ListItem li2 in selectedItems)
        {
            this.KeywordsListBox.Items.Remove(li2);
        }

    }

    protected void DeveloperLogoRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //show/hide
        if (!string.IsNullOrEmpty(this.DeveloperLogoRadioButtonList.SelectedValue))
        {


            switch (this.DeveloperLogoRadioButtonList.SelectedValue.Trim())
            {
                case "0":
                    //use current logo
                    this.DeveloperLogoImage.Visible = true;
                    this.DeveloperLogoFileUploadPanel.Visible = false;
                    this.DeveloperLogoRadioButtonList.ClearSelection();


                    break;

                case "1":
                    //show upload control                   
                    this.DeveloperLogoImage.Visible = false;
                    this.DeveloperLogoFileUploadPanel.Visible = true;


                    break;

                case "2":
                    //none 

                    this.DeveloperLogoImage.Visible = false;
                    this.DeveloperLogoFileUploadPanel.Visible = false;


                    break;
            }

        }



    }

    protected void SponsorLogoRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.SponsorLogoRadioButtonList.SelectedValue))
        {


            switch (this.SponsorLogoRadioButtonList.SelectedValue.Trim())
            {
                case "0":

                    this.SponsorLogoImage.Visible = true;
                    this.SponsorLogoFileUploadPanel.Visible = false;

                    break;

                case "1":
                    //show upload control
                    this.SponsorLogoImage.Visible = false;
                    this.SponsorLogoFileUploadPanel.Visible = true;


                    break;

                case "2":
                    //none                  
                    this.SponsorLogoImage.Visible = false;
                    this.SponsorLogoFileUploadPanel.Visible = false;

                    break;
            }

        }
    }

    private void BindThumbnail()
    {
        string logoImageURL = "";
        this.ThumbnailFileImage.Visible = false;

        if (!this.IsNew && this.FedoraContentObject != null && !string.IsNullOrEmpty(this.FedoraContentObject.ScreenShot))
        {


            try
            {

                vwarDAL.IDataRepository vd = DAL;
                logoImageURL = vd.GetContentUrl(this.FedoraContentObject.PID, this.FedoraContentObject.ScreenShot).Trim();
            }
            catch
            {


            }

            if (!string.IsNullOrEmpty(logoImageURL))
            {
                this.ThumbnailFileImage.DataValue = null;
                this.ThumbnailFileImage.ImageUrl = logoImageURL.Trim();
                this.ThumbnailFileImage.Visible = true;
                return;

            }

        }



    }

    private void BindSponsorLogo()
    {
        string logoImageURL = "";


        //check fedora
        if (!this.IsNew && this.FedoraContentObject != null && !string.IsNullOrEmpty(this.FedoraContentObject.SponsorLogoImageFileName))
        {


            try
            {
                logoImageURL = DAL.GetContentUrl(this.FedoraContentObject.PID, this.FedoraContentObject.SponsorLogoImageFileName).Trim();
            }
            catch
            {


            }

            if (!string.IsNullOrEmpty(logoImageURL))
            {
                this.SponsorLogoImage.ImageUrl = logoImageURL.Trim();
                return;

            }

        }



        if (Context.User.Identity.IsAuthenticated)
        {
            UserProfile p = null;

            try
            {
                p = UserProfileDB.GetUserProfileByUserName(Context.User.Identity.Name);
            }
            catch
            {


            }



            //check profile if authenticated
            if (p != null && Context.User.Identity.IsAuthenticated)
            {
                if (p.SponsorLogo != null && !string.IsNullOrEmpty(p.SponsorLogoContentType))
                {

                    logoImageURL = Website.Pages.Types.FormatProfileImageHandler(p.UserID.ToString(), "Sponsor");

                    if (!string.IsNullOrEmpty(logoImageURL))
                    {
                        this.SponsorLogoImage.ImageUrl = logoImageURL.Trim();
                        return;

                    }

                }

            }



        }



        //Rremove use current logo from radiobuttonlist, show file upload
        if (string.IsNullOrEmpty(logoImageURL))
        {
            //clear selection
            if (this.SponsorLogoRadioButtonList.SelectedItem != null)
            {
                this.SponsorLogoRadioButtonList.ClearSelection();
            }

            //remove
            this.SponsorLogoRadioButtonList.Items.RemoveAt(0);

            //set upload new selected
            this.SponsorLogoRadioButtonList.Items.FindByValue("1").Selected = true;
            this.SponsorLogoImage.Visible = false;
            this.SponsorLogoFileUploadPanel.Visible = true;
        }

    }

    private void BindDeveloperLogo()
    {

        string logoImageURL = "";


        //check fedora
        if (!this.IsNew && this.FedoraContentObject != null && !string.IsNullOrEmpty(this.FedoraContentObject.DeveloperLogoImageFileName))
        {


            try
            {

                vwarDAL.IDataRepository vd = DAL;
                logoImageURL = vd.GetContentUrl(this.FedoraContentObject.PID, this.FedoraContentObject.DeveloperLogoImageFileName).Trim();
            }
            catch
            {


            }

            if (!string.IsNullOrEmpty(logoImageURL))
            {
                this.DeveloperLogoImage.DataValue = null;
                this.DeveloperLogoImage.ImageUrl = logoImageURL.Trim();
                return;

            }

        }

        //get from profile
        if (Context.User.Identity.IsAuthenticated)
        {
            UserProfile p = null;


            try
            {
                p = UserProfileDB.GetUserProfileByUserName(Context.User.Identity.Name);
            }
            catch
            {


            }

            //check profile if authenticated
            if (Context.User.Identity.IsAuthenticated && p != null)
            {
                if (p.DeveloperLogo != null && !string.IsNullOrEmpty(p.DeveloperLogoContentType))
                {

                    logoImageURL = Website.Pages.Types.FormatProfileImageHandler(p.UserID.ToString(), "Developer");

                    if (!string.IsNullOrEmpty(logoImageURL))
                    {
                        this.DeveloperLogoImage.DataValue = null;
                        this.DeveloperLogoImage.ImageUrl = logoImageURL.Trim();
                        return;

                    }

                }

            }



        }


        //set selected
        //remove use current from radiobuttonlist, show file upload
        if (string.IsNullOrEmpty(logoImageURL))
        {
            //clear selection
            if (this.DeveloperLogoRadioButtonList.SelectedItem != null)
            {
                this.DeveloperLogoRadioButtonList.ClearSelection();
            }

            //remove
            this.DeveloperLogoRadioButtonList.Items.RemoveAt(0);

            //set upload new selected
            this.DeveloperLogoRadioButtonList.Items.FindByValue("1").Selected = true;
            this.DeveloperLogoImage.Visible = false;
            this.DeveloperLogoFileUploadPanel.Visible = true;
        }


    }

    private void UploadDeveloperLogo(vwarDAL.IDataRepository dal, ContentObject co)
    {
        if (this.DeveloperLogoRadioButtonList.SelectedItem != null)
        {
            switch (this.DeveloperLogoRadioButtonList.SelectedValue.Trim())
            {
                case "0": //use current

                    //if use current and there's an empty string then use the profile logo
                    if (string.IsNullOrEmpty(co.DeveloperLogoImageFileName))
                    {
                        //use the profile image
                        DataTable dt = UserProfileDB.GetUserProfileDeveloperLogoByUserName(Context.User.Identity.Name);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];

                            if (dr["Logo"] != System.DBNull.Value && dr["LogoContentType"] != System.DBNull.Value && !string.IsNullOrEmpty(dr["LogoContentType"].ToString()))
                            {
                                var data = (byte[])dr["Logo"];
                                using (MemoryStream s = new MemoryStream())
                                {
                                    s.Write(data, 0, data.Length);
                                    s.Position = 0;

                                    //filename
                                    co.DeveloperLogoImageFileName = "developer.jpg"; ;


                                    if (!string.IsNullOrEmpty(dr["FileName"].ToString()))
                                    {
                                        co.DeveloperLogoImageFileName = dr["FileName"].ToString();
                                    }

                                    //upload the file
                                    FedoraContentObject.DeveloperLogoImageFileNameId = dal.UploadFile(s, co.PID, co.DeveloperLogoImageFileName);
                                }
                            }
                        }



                    }




                    break;

                case "1": //Upload logo

                    if (this.DeveloperLogoFileUpload.FileContent.Length > 0 && !string.IsNullOrEmpty(this.DeveloperLogoFileUpload.FileName))
                    {
                        co.DeveloperLogoImageFileName = this.DeveloperLogoFileUpload.FileName;
                        co.DeveloperLogoImageFileNameId = dal.UploadFile(this.DeveloperLogoFileUpload.FileContent, co.PID, this.DeveloperLogoFileUpload.FileName);
                    }


                    break;

                case "2": //none                       

                    break;
            }

        }





    }

    private void UploadSponsorLogo(vwarDAL.IDataRepository dal, ContentObject co)
    {

        if (this.SponsorLogoRadioButtonList.SelectedItem != null)
        {
            switch (this.SponsorLogoRadioButtonList.SelectedValue.Trim())
            {
                case "0": //use profile logo

                    //use profile logo if use current and there's an empty file name otherwise don't change
                    if (string.IsNullOrEmpty(co.SponsorLogoImageFileName))
                    {

                        DataTable dt = UserProfileDB.GetUserProfileSponsorLogoByUserName(Context.User.Identity.Name);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];

                            if (dr["Logo"] != System.DBNull.Value && dr["LogoContentType"] != System.DBNull.Value && !string.IsNullOrEmpty(dr["LogoContentType"].ToString()))
                            {
                                var data = (byte[])dr["Logo"];
                                using (MemoryStream s = new MemoryStream())
                                {
                                    s.Write(data, 0, data.Length);
                                    s.Position = 0;

                                    //filename
                                    co.SponsorLogoImageFileName = "sponsor.jpg";


                                    if (!string.IsNullOrEmpty(dr["FileName"].ToString()))
                                    {
                                        co.SponsorLogoImageFileName = dr["FileName"].ToString();
                                    }
                                    FedoraContentObject.SponsorLogoImageFileNameId =
                                    dal.UploadFile(s, co.PID, co.SponsorLogoImageFileName);
                                }
                            }


                        }


                    }



                    break;

                case "1": //Upload logo
                    if (this.SponsorLogoFileUpload.FileContent.Length > 0 && !string.IsNullOrEmpty(this.SponsorLogoFileUpload.FileName))
                    {
                        co.SponsorLogoImageFileName = this.SponsorLogoFileUpload.FileName;
                        co.DeveloperLogoImageFileNameId = dal.UploadFile(this.SponsorLogoFileUpload.FileContent, co.PID, this.SponsorLogoFileUpload.FileName);
                    }

                    break;

                case "2": //none
                    break;
            }

        }



    }

}