@{
    ViewBag.Title = "Open School Stream Report";
    Layout = "~/Views/Shared/_Card_Header.cshtml";
}
<style type="text/css">
    .table-responsive td {
        font-size: 10px;
    }

    body {
        font-size: 12px;
        margin: 0;
        padding: 0;
    }

    .footer {
        position: fixed;
        width: 100%;
        bottom: 0;
        z-index: 9999999;
        display: block;
    }
</style>
<link rel="stylesheet" href="https://registration2022.pseb.ac.in//Content/cardprintr.css" />


@using (Html.BeginForm("OpenSchoolStreamDataReport", "Admin", FormMethod.Post, new { @enctype = "multipart/form-data" }))
{
    <div class="row noprint">
        <div class="col-md-12">
            <h1>Open School Report</h1>
        </div>
        <div class="clearfix"></div>
        <div class="col-md-12">
            <fieldset style="margin-bottom:15px">
                <legend>Search</legend>
                <div class="col-md-2">Choose District</div>
                <div class="col-md-2">@Html.DropDownList("ddlDist", new SelectList(ViewBag.Districts, "Value", "Text", ViewBag.SelectedDist), new { @class = "", id = "SelectDist", @required = "required" })</div>
                <div class="col-md-2"><input type="submit" value="Search" /></div>
            </fieldset>
        </div>
    </div>
}
<div style="color:#cc0000;width:100%;text-align:right;" class="noprint">
    * If Punjabi font is not visible properly <a href="/PNB_FONT.ZIP" target="_blank" style="font-size:14px;"><b style="color:#cc0000"> CLICK HERE </b></a> for Download Punjabi PNB Font

</div>
<div style="width:100%;text-align:right;" class="noprint"><input type="button" value="Print" class="btn noprint" onclick="window.print()" />&nbsp;<a href="javascript:history.back()" class="btn noprint" style="color:#fff">Back</a> </div>

@if (ViewBag.data != null)
{
    System.Data.DataSet ds = (System.Data.DataSet)ViewBag.data;
    try
    {
        int sr = 0;
        <span>&nbsp;</span>
        <div class="table-responsive">
            <table id="t01" class="table table-bordered">
                <thead>
                    <tr>
                        <td colspan="3" style="border-right:0 !important">&nbsp;Session: 2023-24</td>
                        <td colspan="7" style="text-align:center;border-left:0 !important;border-right:0 !important"><span style="font-size:24px">Punjab School Education Board</span><br />List of School (Open School Study Centers)</td>
                        <td colspan="3" style="border-left:0 !important;text-align:right">Print Date: @DateTime.Today.ToString("dd/MM/yyyy") &nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="13" style="border-top:0 !important">
                            @if (ViewBag.SelectedDist != null)
                            {
                                string dist = ViewBag.SelectedDist;
                                List<SelectListItem> dists = (List<SelectListItem>)ViewBag.Districts;
                                string distnm = dists.Find(f => f.Value == dist.Trim()).Text;
                                <span>District: (@dist) @distnm</span>

                            }
                        </td>
                    </tr>

                    <tr class="active-tr">
                        <th rowspan="2" width="3%">
                            SR
                        </th>
                        <th rowspan="2" width="7%" style="text-align:center">
                            Study<br /><span style="font-size:7px">Center Code</span>
                        </th>
                        <th rowspan="2" width="7%" style="text-align:center">
                            School<br /><span style="font-size:7px">Center Code</span>
                        </th>
                        <th rowspan="2">
                            School Name
                        </th>
                        <th rowspan="2">
                            Appeared Student
                        </th>
                        <th rowspan="2">
                            Passed Student
                        </th>
                        <th colspan="6">
                            Sr.Sec Stream
                        </th>
                    </tr>
                    <tr class="active-tr">
                        <th style="padding-top:0;padding-bottom:0" colspan="2">HUM</th>
                        <th style="padding-top:0;padding-bottom:0">SCI</th>
                        <th style="padding-top:0;padding-bottom:0">COMM</th>
                        <th style="padding-top:0;padding-bottom:0">VOC</th>
                    </tr>
                </thead>
                @foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    sr++;
                    <tr>
                        <td>
                            @sr
                        </td>
                        <td>
                            @dr["studyCenterCode"].ToString()
                        </td>
                        <td>
                            @dr["schoolcode"].ToString()
                        </td>
                        <td>
                            @dr["schoolname"].ToString()
                        </td>
                        <td>
                            @dr["Appeared_Student"].ToString()
                        </td>
                        <td>
                            @dr["Passed_Student"].ToString()
                        </td>
                        <td colspan="2">
                            @dr["HUMANITIES"].ToString()
                        </td>
                        <td colspan="1">
                           @dr["SCIENCE"].ToString()
                        </td>
                        <td>
                            @dr["COMMERCE"].ToString()
                        </td>
                        <td>
                            @dr["INTIGRATEDGROUP"].ToString()
                        </td>
                    </tr>

                }
            </table>
        </div>
    }
    catch (Exception e)
    {
        <div class="text-center">
            <span class="rqf">No Data Found</span>
        </div>
    }
}
else
{

}


